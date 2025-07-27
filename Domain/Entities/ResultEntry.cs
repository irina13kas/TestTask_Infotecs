namespace Domain.Models
{
    public class ResultEntry
    {
        public string FileName { get; set; }
        //дельта времени в секундах
        public int DeltaDate { get; set; }
        //минимальное дата и время, как момент запуска первой операции
        public DateTime MinDateAndTime { get; set; }
        //среднее время выполнения
        public double AvgExecutionTime { get; set; }
        //среднее значение по показателям
        public double AvgValue { get; set; }
        //медиана по показателям
        public double MedianValue { get; set; }
        //максимальное значение по показателям
        public double MaxValue { get; set; }
        //минимальное значение по показателям
        public double MinValue { get; set; }
    }
}
