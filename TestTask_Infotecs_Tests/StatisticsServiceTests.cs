using Application.Services;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace TestTask_tests;

[TestFixture]
public class StatisticsServiceTests
{
    private AppDbContext _dbContext;
    private StatisticsService _stat;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Name")
            .Options;
        _dbContext = new AppDbContext(options);
        _stat = new StatisticsService(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task GetLastValuesByFileNameAsync_ReturnsEmpty_IfFileNameNull()
    {
        var result = await _stat.GetLastValuesByFileNameAsync(null);
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetLastValuesByFileNameAsync_IfFileNameNotExists()
    {
        var result = await _stat.GetLastValuesByFileNameAsync("notExistFile");
        Assert.IsEmpty(result);
    }

    [Test]
    public async Task GetLastValuesByFileNameAsync_ReturnsValues_IfExists()
    {
        _dbContext.Values.AddRange(
            new ValueEntry { Date = DateTime.Today, ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 1},
            new ValueEntry { Date = DateTime.Today.AddDays(-1), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 2 },
            new ValueEntry { Date = DateTime.Today.AddDays(-2), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 3 },
            new ValueEntry { Date = DateTime.Today.AddHours(-1), ExecutionTime = 1.0, Value = 4.8, FileName = "file1.csv", Id = 4 },
            new ValueEntry { Date = DateTime.Today.AddDays(-3), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 5 },
            new ValueEntry { Date = DateTime.Today.AddHours(-4), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 6 },
            new ValueEntry { Date = DateTime.Today.AddHours(-5), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 7 },
            new ValueEntry { Date = DateTime.Today.AddDays(-11), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 8 },
            new ValueEntry { Date = DateTime.Today.AddDays(-1).AddHours(12), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 9 },
            new ValueEntry { Date = DateTime.Today.AddHours(-9), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 10 },
            new ValueEntry { Date = DateTime.Today.AddMinutes(-60), ExecutionTime = 1.0, Value = 4.8, FileName = "file1.csv", Id = 11 },
            new ValueEntry { Date = DateTime.Today.AddMinutes(-10), ExecutionTime = 1.0, Value = 4.8, FileName = "file.csv", Id = 12 }
        );
        await _dbContext.SaveChangesAsync();

        var result = await _stat.GetLastValuesByFileNameAsync("file.csv");
        Assert.IsNotEmpty(result);
        Assert.AreEqual("file.csv", result.First().FileName);
        Assert.AreEqual(10, result.Count);

    }
}
