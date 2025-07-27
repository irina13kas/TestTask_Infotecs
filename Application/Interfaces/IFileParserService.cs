using Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IFileParserService
    {
        Task<List<CsvValueDto>> ParseAndValidateAsync(IFormFile fileStream);
        Task SaveToDbAsync(List<CsvValueDto> records, string fileName);
    }
}
