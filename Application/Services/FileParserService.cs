using Application.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace Application.Services
{
    public class FileParserService : IFileParserService
    {
        private readonly AppDbContext _db;
        private readonly ICsvValidator _validator;

        public FileParserService(AppDbContext db, ICsvValidator csvValidator)
        {
            _db = db;
            _validator = csvValidator;
        }

        public async Task<List<CsvValueDto>> ParseAndValidateAsync(IFormFile fileStream)
        {
            var records = new List<CsvValueDto>();

            using var reader = new StreamReader(fileStream.OpenReadStream());
            string? line;
            bool isFirst = true;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                var parts = line.Split(';');
                if (parts.Count() != 3)
                {
                    throw new ValidationException("Неверный формат строки. Кол-во значений должно равняться трём");
                }
                if (!DateTime.TryParse(parts[0], out var date))
                {
                    throw new ValidationException($"Неверный формат даты: {parts[0]}");
                }
                if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var execTime))
                {
                    throw new ValidationException($"Неверный формат времени исполнения: {parts[1]}");
                }
                if (!double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                {
                    throw new ValidationException($"Неверный формат показателя: {parts[2]}");
                }

                records.Add(new CsvValueDto
                {
                    Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
                    ExecitionTime = execTime,
                    Value = value
                });
            }

            _validator.Validate(records);
            return records;

        }

        public async Task<ResultEntry> SaveToDbAsync(List<CsvValueDto> records, string fileName)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            ResultEntry result = new ResultEntry();
            try
            {
                var oldValues = _db.Values.Where(x => x.FileName == fileName);
                _db.RemoveRange(oldValues);

                await _db.SaveChangesAsync();

                var values = records.Select(r => new ValueEntry
                {
                    FileName = fileName,
                    Date = r.Date,
                    ExecutionTime = r.ExecitionTime,
                    Value = r.Value
                }).ToList();

                await _db.Values.AddRangeAsync(values);

                result = new ResultEntry
                {
                    FileName = fileName,
                    DeltaDate = (values.Max(x => x.Date) - values.Min(x => x.Date)).Seconds,
                    MinDateAndTime = values.Min(x => x.Date),
                    AvgExecutionTime = values.Average(x => x.ExecutionTime),
                    AvgValue = values.Average(x => x.Value),
                    MedianValue = GetMedian(values),
                    MaxValue = values.Max(x => x.Value),
                    MinValue = values.Min(x => x.Value)
                };
                var existingResult = await _db.Results.FindAsync(fileName);
                if (existingResult != null)
                {
                    existingResult.DeltaDate = result.DeltaDate;
                    existingResult.MinDateAndTime = result.MinDateAndTime;
                    existingResult.AvgExecutionTime = result.AvgExecutionTime;
                    existingResult.AvgValue = result.AvgValue;
                    existingResult.MedianValue = result.MedianValue;
                    existingResult.MaxValue = result.MaxValue;
                    existingResult.MinValue = result.MinValue;
                }
                else
                {

                    await _db.Results.AddAsync(result);
                }

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return result;
        }

        double GetMedian(List<ValueEntry> values)
        {
            double median = -1;
            var sortedValues = values.OrderBy(x => x.Value).ToList();
            int size = sortedValues.Count();
            if(size % 2 != 0)
            {
                median = sortedValues[size / 2].Value;
            }
            else
            {
                median = (sortedValues[size / 2].Value + sortedValues[size / 2 - 1].Value) / 2.0;
            }
            return median;
        }
    }
}
