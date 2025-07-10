using System.ComponentModel.DataAnnotations;

namespace Ensek.MeterReadingService.Models;

public class FileUploadRequest
{
    [Required]
    public IFormFile File { get; set; }
}

