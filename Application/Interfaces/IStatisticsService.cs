using Domain.Models;

namespace Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<List<ValueEntry>> GetLastValuesByFileNameAsync(string fileName);
    }
}
