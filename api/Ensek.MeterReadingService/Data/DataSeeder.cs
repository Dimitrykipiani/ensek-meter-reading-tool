using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ensek.MeterReadingService.Models;

namespace Ensek.MeterReadingService.Data;

public class DataSeeder
{
    public static void SeedAccounts(AppDbContext context)
    {
        if (context.Accounts.Any())
            return;

        var filePath = Path.Combine(AppContext.BaseDirectory, "SeedData", "Test_Accounts.csv");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Test_Accounts.csv not found", filePath);

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        var accounts = csv.GetRecords<Account>().ToList();

        context.Accounts.AddRange(accounts);
        context.SaveChanges();
    }
}
