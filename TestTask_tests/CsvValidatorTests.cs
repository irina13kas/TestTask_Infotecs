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

            for(int i = 0; i < 2000; i++)
            {
                tooMany.Add(new CsvValueDto { Date = DateTime.Today, Value = i, ExecitionTime = i / 10.0 });
            }

            Assert.Throws<ValidationException>(() => _validator.Validate(tooMany));
            Assert.Throws<ValidationException>(() => _validator.Validate(emptyList));
        }
    }
}