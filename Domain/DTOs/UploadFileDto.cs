using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Domain.DTOs
{
    public class UploadFileDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
