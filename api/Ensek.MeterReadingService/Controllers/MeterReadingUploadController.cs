using Ensek.MeterReadingService.Models;
using Ensek.MeterReadingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ensek.MeterReadingService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeterReadingUploadController : ControllerBase
    {
        private readonly IMeterReadingService _service;

        public MeterReadingUploadController(IMeterReadingService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("meter-reading-uploads")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadMeterReadings([FromForm] FileUploadRequest request)
        {
            var file = request.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!file.FileName.EndsWith(".csv"))
                return BadRequest("Only CSV files are allowed.");

            using var stream = file.OpenReadStream();
            var result = await _service.ProcessMeterReadingsAsync(stream);

            return Ok(result);
        }
    }
}
