using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Application.Services;
using System.Threading.Tasks;
using Domain.DTOs;

namespace TestTask_tests;

public class ResultFilterServiceTests
{
    private AppDbContext _dbContext;
    private ResultFilterService _filter;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_Name")
            .Options;

        _dbContext = new AppDbContext(options);

        _dbContext.Results.AddRange(
            new ResultEntry { FileName = "file1.csv", AvgExecutionTime = 1.5, AvgValue = 3.4, MinDateAndTime = new DateTime(2020, 11, 1) },
            new ResultEntry { FileName = "file2.csv", AvgExecutionTime = 2.5, AvgValue = 11, MinDateAndTime = new DateTime(2021, 11, 11) },
            new ResultEntry { FileName = "file3.csv", AvgExecutionTime = 5.7, AvgValue = 9.78, MinDateAndTime = new DateTime(2022, 1, 1) }
        );

        _dbContext.SaveChangesAsync();

        _filter = new ResultFilterService(_dbContext);

    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }

    [Test]
    public async Task GetFilteredResultsAsync_ReturnsAll_WhenNoFilters()
    {
        var fil = new ResultFilterDto
        {
            FileName = null,
            StartDate = null,
            EndDate = null,
            AvgValueMin = null,
            AvgValueMax = null,
            AvgExecutionTimeMin = null,
            AvgExecutionTimeMax = null
        };
        var result = await _filter.GetFilteredResultsAsync(fil);
        Assert.AreEqual(3, result.Count);
    }

    [Test]
    public async Task GetFilteredResultsAsync_ReturnsFilteredByAvgValueInterval()
    {
        var fil = new ResultFilterDto
        {
            FileName = null,
            StartDate = null,
            EndDate = null,
            AvgValueMin = 3.0,
            AvgValueMax = 10,
            AvgExecutionTimeMin = null,
            AvgExecutionTimeMax = null
        };
        var result = await _filter.GetFilteredResultsAsync(fil);
        Assert.AreEqual(2, result.Count);
    }

    [Test]
    public async Task GetFilteredResultsAsync_ReturnsFilteredByAvgExecutionTimeInterval()
    {
        var fil = new ResultFilterDto
        {
            FileName = null,
            StartDate = null,
            EndDate = null,
            AvgValueMin = null,
            AvgValueMax = null,
            AvgExecutionTimeMin = 1,
            AvgExecutionTimeMax = 2
        };
        var result = await _filter.GetFilteredResultsAsync(fil);
        Assert.AreEqual(1, result.Count);
    }

    [Test]
    public async Task GetFilteredResultsAsync_ReturnsFilteredByDateInterval()
    {
        var fil = new ResultFilterDto
        {
            FileName = null,
            StartDate = new DateTime(2021,10,12),
            EndDate = DateTime.Today,
            AvgValueMin = null,
            AvgValueMax = null,
            AvgExecutionTimeMin = null,
            AvgExecutionTimeMax = null
        };
        var result = await _filter.GetFilteredResultsAsync(fil);
        Assert.AreEqual(2, result.Count);
    }

    [Test]
    public async Task GetFilteredResultsAsync_ReturnsFilteredByFileName()
    {
        var fil = new ResultFilterDto
        {
            FileName = "file1",
            StartDate = null,
            EndDate = null,
            AvgValueMin = null,
            AvgValueMax = null,
            AvgExecutionTimeMin = null,
            AvgExecutionTimeMax = null
        };
        var result = await _filter.GetFilteredResultsAsync(fil);
        Assert.AreEqual(1, result.Count);
    }
}
