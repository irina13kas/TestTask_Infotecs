namespace Domain.DTOs
{
    public class ResultFilterDto
    {
        public string? FileName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? AvgValueMin { get; set; }
        public double? AvgValueMax { get; set; }
        public double? AvgExecutionTimeMin { get; set; }
        public double? AvgExecutionTimeMax { get; set; }
    }
}
