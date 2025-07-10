namespace Ensek.MeterReadingService.Mappings;

using CsvHelper.Configuration;
using Ensek.MeterReadingService.Models;

public class MeterReadingCsvMap : ClassMap<MeterReadingDto>
{
    public MeterReadingCsvMap()
    {
        Map(m => m.AccountId);
        Map(m => m.MeterReadingDateTime).TypeConverterOption.Format("dd/MM/yyyy HH:mm");
        Map(m => m.MeterReadValue);
    }
}

