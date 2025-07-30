using Application.Services;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Domain.DTOs;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Application.Interfaces;

namespace TestTask_tests;

[TestFixture]
public class FIleParserServiceTests
{
    private AppDbContext _dbContext;
    private CsvValidator _validator;
    private FileParserService _parser;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb" + Guid.NewGuid())
            .ConfigureWarnings(w=> w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new AppDbContext(options);
        _validator = new CsvValidator();
        _parser = new FileParserService(_dbContext, _validator);

    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    private IFormFile CreateFormFile(string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "file", "test.csv");
    }

    [Test]
    public void ParseAndValidateAsync_Throws_When_EmptyValues()
    {
        var content = "Date;ExecutionTime;Value\n;;\n";
        var file = CreateFormFile(content);

        Assert.ThrowsAsync<ValidationException>(() => _parser.ParseAndValidateAsync(file));
    }

    [Test]
    public void ParseAndValidateAsync_Throws_When_IncorrectTypes()
    {
        var content = "Date;ExecutionTime;Value\nabc;eee;zzz\n";
        var file = CreateFormFile(content);

        Assert.ThrowsAsync<ValidationException>(() => _parser.ParseAndValidateAsync(file));
    }

    [Test]
    public async Task SaveToDbAsync_SavesValuesAndResults()
    {
        var record_1 = new CsvValueDto { Date = DateTime.Today, Value = 1.0, ExecitionTime = 2.0 };
        var record_2 = new CsvValueDto { Date = DateTime.Today.AddDays(-1), Value = 5.0, ExecitionTime = 3.0 };
        var record_3 = new CsvValueDto { Date = new DateTime(2020, 10, 5), Value = 4.6, ExecitionTime = 5.2 };
        var records = new List<CsvValueDto>();
        records.Add(record_1);
        records.Add(record_2);
        records.Add(record_3);
        string fileName = "test1.csv";

        var result = await _parser.SaveToDbAsync(records, fileName);

        var valuesInDb = await _dbContext.Values.ToListAsync();
        var resultsInDb = await _dbContext.Results.ToListAsync();

        Assert.AreEqual(3, valuesInDb.Count);
        Assert.AreEqual(1, resultsInDb.Count);

        Assert.AreEqual(fileName, result.FileName);
        Assert.AreEqual((record_1.Date - record_3.Date).Seconds, result.DeltaDate);
        Assert.AreEqual(record_3.Date, result.MinDateAndTime);
        Assert.AreEqual((record_1.ExecitionTime + record_2.ExecitionTime + record_3.ExecitionTime)/ 3.0, result.AvgExecutionTime);
        Assert.AreEqual((record_1.Value + record_2.Value + record_3.Value)/3.0, result.AvgValue);
        Assert.AreEqual(record_3.Value, result.MedianValue);
        Assert.AreEqual(record_2.Value, result.MaxValue);
        Assert.AreEqual(record_1.Value, result.MinValue);
    }

    [Test]
    public async Task SaveToDbAsync_ReplacesExistingValues()
    {
        var fileName = "test_2.csv";
        var records_1 = new List<CsvValueDto> {
            new CsvValueDto{ Date = DateTime.Today, Value = 1.0, ExecitionTime = 2.3}
        };
        
        await _parser.SaveToDbAsync(records_1,fileName);

        var recornds_2 = new List<CsvValueDto> { 
            new CsvValueDto{ Date = DateTime.Today, Value = 1.9, ExecitionTime = 2.5},
            new CsvValueDto { Date = DateTime.Today.AddDays(-2), Value = 2.7, ExecitionTime = 7.0 }
        };

        await _parser.SaveToDbAsync(recornds_2, fileName);

        var valuesInDb = await _dbContext.Values.Where(r => r.FileName == fileName).ToListAsync();
        Assert.AreEqual(2, valuesInDb.Count);
    }

    [Test]
    public async Task SaveToDbAsync_UpdateResults_IfExists()
    {
        var fileName = "test_2.csv";
        var records_1 = new List<CsvValueDto> {
            new CsvValueDto{ Date = DateTime.Today, Value = 1.0, ExecitionTime = 2.3}
        };

        await _parser.SaveToDbAsync(records_1, fileName);

        var oldResult = await _dbContext.Results.FindAsync(fileName);
        Assert.IsNotNull(oldResult);

        var records_2 = new List<CsvValueDto> {
            new CsvValueDto{ Date = DateTime.Today, Value = 1.9, ExecitionTime = 2.5},
            new CsvValueDto { Date = DateTime.Today.AddDays(-2), Value = 2.7, ExecitionTime = 7.0 }
        };

        var newResult = await _parser.SaveToDbAsync(records_2, fileName);

        Assert.AreEqual(fileName,newResult.FileName);
        Assert.AreNotEqual(oldResult,newResult);
    }

    [Test]
    public async Task SaveToDbAsync_RollsBackTransaction_OnError()
    {
        var mockValidator = new Mock<ICsvValidator>();
        //mockValidator.Setup(v => v.Validate(It.IsAny<List <CsvValueDto>>()))
        //    .Throws(new ValidationException("Fail validation"));

        var service = new FileParserService(_dbContext, mockValidator.Object);

        var records = new List<CsvValueDto>();

        Assert.ThrowsAsync<InvalidOperationException>(async () => await service.SaveToDbAsync(records, "test_3.csv"));

        var values = await _dbContext.Values.ToListAsync();
        var results = await _dbContext.Results.ToListAsync();

        Assert.AreEqual(0, values.Count);
        Assert.AreEqual(0, results.Count);
    }
}
