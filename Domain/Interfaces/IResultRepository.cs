namespace Domain.Interfaces
{
    public interface IResultRepository
    {
        Task Update(string fileName);
        Task Insert(string fileName);
        Task FilterByFileName(string fileName);
        Task FilterByFirstOperation(DateTime minValue, DateTime maxValue);
        Task FilterByAvgValue(double minValue, double maxValue);
        Task FilterByAvgExecutionTime(double minValue, double maxValue);
    }
}
