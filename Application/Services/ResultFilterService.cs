using Application.Interfaces;
using Domain.DTOs;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ResultFilterService : IResultFilterService
    {
        private readonly AppDbContext _db;

        public ResultFilterService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<ResultEntry>> GetFilteredResultsAsync(ResultFilterDto filter)
        {
            var query = _db.Results.AsQueryable();

            if(!string.IsNullOrWhiteSpace(filter.FileName))
            {
                query = query.Where(x => x.FileName == filter.FileName);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(x => x.MinDateAndTime >= filter.StartDate);
            }

            if (filter.EndDate.HasValue) 
            {
                query = query.Where(x => x.MinDateAndTime <= filter.EndDate);
            }

            if (filter.AvgValueMin.HasValue)
            {
                query = query.Where(x => x.AvgValue >= filter.AvgValueMin);
            }

            if (filter.AvgValueMax.HasValue)
            {
                query = query.Where(x => x.AvgValue <= filter.AvgValueMax);
            }

            if (filter.AvgExecutionTimeMin.HasValue)
            {
                query = query.Where(x => x.AvgExecutionTime >= filter.AvgExecutionTimeMin);
            }

            if (filter.AvgExecutionTimeMax.HasValue)
            {
                query = query.Where(x => x.AvgExecutionTime <= filter.AvgExecutionTimeMax);
            }

            return  await query.ToListAsync();
        }
    }
}
