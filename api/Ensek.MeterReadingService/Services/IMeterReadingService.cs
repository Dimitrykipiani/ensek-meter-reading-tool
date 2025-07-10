using Ensek.MeterReadingService.Models;

namespace Ensek.MeterReadingService.Services;

public interface IMeterReadingService
{
    Task<MeterReadingUploadResult> ProcessMeterReadingsAsync(Stream csvStream);
}
