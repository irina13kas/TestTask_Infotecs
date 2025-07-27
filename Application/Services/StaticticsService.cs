using Application.Interfaces;
using Domain.Models;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StaticticsService : IStatisticsService
    {
        private readonly AppDbContext _db;

        public StaticticsService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<ValueEntry>> GetLastValuesByFileNameAsync(string fileName)
        {
            var query = await _db.Values
                .Where(x => x.FileName == fileName)
                .OrderByDescending(m => m.Date)
                .Take(10)
                .ToListAsync();

            return query;
        }
    }
}
