using Application.Interfaces;
using Domain.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Application.Services
{
    public class CsvValidator : ICsvValidator
    {
        public void Validate(List<CsvValueDto> records)
        {
            if(records.Count < 1 || records.Count > 10000)
            {
                throw new ValidationException("Количество строк не может быть меньше 1 и больше 10 000");
            }
            foreach (var record in records)
            {

                if(record.Date < new DateTime(2000,1,1) || record.Date > DateTime.Today)
                {
                    throw new ValidationException($"Дата не может быть позже текущей и раньше 01.01.2000: {record.Date}");
                }
                else if(record.ExecitionTime < 0)
                {
                    throw new ValidationException($"Время выполнения не может быть меньше 0: {record.ExecitionTime}");
                }
                else if(record.Value < 0)
                {
                    throw new ValidationException($"Значение показателя не может быть меньше 0: {record.Value}");
                }
            }
        }
    }
}
