using Application.Interfaces;
using Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IFileParserService _fileParser;
        private readonly IResultFilterService _filter;
        private readonly IStatisticsService _statistics;

        public DataController(IFileParserService fileParser, IResultFilterService filter, IStatisticsService statistics)
        {
            _fileParser = fileParser;
            _filter = filter;
            _statistics = statistics;
        }

        /// <summary>
        /// Валидация и обработка файла. Вычисление интегральных результатов
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadFileDto dto)
        {
            var file = dto.File;
            if(file.Length == 0 || file == null)
            {
                return BadRequest("Файл пустой или не передан");
            }

            try
            {
                var parserData = await _fileParser.ParseAndValidateAsync(file);
                var resultRecord = await _fileParser.SaveToDbAsync(parserData, file.FileName);

                return Ok(resultRecord);
            }
            catch (ValidationException validEx)
            {
                return BadRequest($"Возникли ошибки при форматировании: {validEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ошибка на стороне сервера");
            }
        }

        /// <summary>
        /// Фильтрация результатов по названию файла, диапозону времени запуска первой операции, по среднему показателю и по среднему времени выполнения
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredResults([FromQuery] ResultFilterDto filter)
        {
            var results = await _filter.GetFilteredResultsAsync(filter);
            return Ok(results);
        }
        /// <summary>
        /// Список из 10 последних значений по имени файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetLastValuesByFileName([FromQuery] string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var values = await _statistics.GetLastValuesByFileNameAsync(fileName);
                return Ok(values);
            }
            return BadRequest("Не указано имя файла для поиска поиследних 10 значений");
        }
    }
}
