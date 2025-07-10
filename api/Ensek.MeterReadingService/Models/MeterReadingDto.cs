namespace Ensek.MeterReadingService.Models;

public class MeterReadingDto
{
    public int AccountId { get; set; }
    public DateTime MeterReadingDateTime { get; set; }
    public string MeterReadValue { get; set; } = string.Empty;
}
