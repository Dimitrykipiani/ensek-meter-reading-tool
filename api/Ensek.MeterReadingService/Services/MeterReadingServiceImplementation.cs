using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.MeterReadingService.Data;
using Ensek.MeterReadingService.Mappings;
using Ensek.MeterReadingService.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.MeterReadingService.Services
{
    public class MeterReadingServiceImplementation : IMeterReadingService
    {
        private readonly AppDbContext _context;

        public MeterReadingServiceImplementation(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MeterReadingUploadResult> ProcessMeterReadingsAsync(Stream csvStream)
        {
            var result = new MeterReadingUploadResult();

            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null
            });

            csv.Context.RegisterClassMap<MeterReadingCsvMap>();

            var records = csv.GetRecords<MeterReadingDto>();

            foreach (var record in records)
            {
                var accountExists = await _context.Accounts.AnyAsync(a => a.AccountId == record.AccountId);
                var isValidFormat = Regex.IsMatch(record.MeterReadValue, @"^\d{5}$");

                if (!accountExists)
                {
                    result.Errors.Add($"Account {record.AccountId} not found.");
                    result.FailedReadings++;
                    continue;
                }

                if (!isValidFormat)
                {
                    result.Errors.Add($"Invalid meter read value: {record.MeterReadValue}");
                    result.FailedReadings++;
                    continue;
                }

                var duplicate = await _context.MeterReadings.AnyAsync(r =>
                    r.AccountId == record.AccountId &&
                    r.MeterReadingDateTime == record.MeterReadingDateTime &&
                    r.MeterReadValue == record.MeterReadValue);

                if (duplicate)
                {
                    result.Errors.Add($"Duplicate entry for Account {record.AccountId}");
                    result.FailedReadings++;
                    continue;
                }

                var meterReading = new MeterReading
                {
                    AccountId = record.AccountId,
                    MeterReadingDateTime = record.MeterReadingDateTime,
                    MeterReadValue = record.MeterReadValue
                };

                _context.MeterReadings.Add(meterReading);
                result.SuccessfulReadings++;
            }

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
