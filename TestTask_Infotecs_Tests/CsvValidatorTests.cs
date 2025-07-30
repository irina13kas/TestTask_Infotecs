using Application.Services;
using Domain.DTOs;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace TestTask_tests
{
    [TestFixture]
    public class CsvValidatorTests
    {
        private CsvValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new CsvValidator();
        }

        [Test]
        public void Validate_When_TooManyOrEmptyRecords()
        {
            var tooMany = new List<CsvValueDto>();
            var emptyList = new List<CsvValueDto>();

            for (int i = 0; i < 20000; i++)
            {
                tooMany.Add(new CsvValueDto { Date = DateTime.Today, Value = i, ExecitionTime = i / 10.0 });
            }

            Assert.Throws<ValidationException>(() => _validator.Validate(tooMany));
            Assert.Throws<ValidationException>(() => _validator.Validate(emptyList));
        }

        [Test]
        public void Valide_When_DateIncorrect()
        {
            var tomorrowDate = DateTime.Today.AddDays(1);
            var oldDate = new DateTime(1999, 10, 10);
            var records = new List<CsvValueDto>();
            records.Add(new CsvValueDto { Date = tomorrowDate, Value = 1.0, ExecitionTime = 23.4 });
            records.Add(new CsvValueDto { Date = oldDate, Value = 1.0, ExecitionTime = 14.6 });

            foreach (var record in records)
            {
                Assert.Throws<ValidationException>(() => _validator.Validate(new List<CsvValueDto> { record }));
            }
        }

        [Test]
        public void Valide_When_ValueNegative()
        {
            var value = -1.0;
            var records = new List<CsvValueDto>();
            records.Add(new CsvValueDto { Date = DateTime.Today, Value = value, ExecitionTime = 1.0 });

            Assert.Throws<ValidationException>(() => _validator.Validate(records));
        }

        [Test]
        public void Valide_When_ExecutionTimeNegative()
        {
            var executionTime = -1.0;
            var records = new List<CsvValueDto>();
            records.Add(new CsvValueDto { Date = DateTime.Today, Value = 1.0, ExecitionTime = executionTime });

            Assert.Throws<ValidationException>(() => _validator.Validate(records));
        }

        [Test]
        public void Valide_When_InputCorrect()
        {
            var date = DateTime.Today;
            var value = 1.0;
            var executionTime = 1.0;
            var records = new List<CsvValueDto>();
            records.Add(new CsvValueDto { Date = date, Value = value, ExecitionTime = executionTime });

            Assert.DoesNotThrow(() => _validator.Validate(records));
        }


    }
}