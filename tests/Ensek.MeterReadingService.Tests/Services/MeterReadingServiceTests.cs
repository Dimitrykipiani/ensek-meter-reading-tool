using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ensek.MeterReadingService.Data;
using Ensek.MeterReadingService.Models;
using Ensek.MeterReadingService.Services;
using Microsoft.EntityFrameworkCore;

namespace Ensek.MeterReadingService.Tests.Services;

public class MeterReadingServiceTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        var context = new AppDbContext(options);
        context.Accounts.Add(new Account { AccountId = 23444, FirstName = "Test", LastName = "User" });
        context.SaveChanges();
        return context;
    }

    private AppDbContext GetDbContextWithAccounts()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
            .Options;

        var context = new AppDbContext(options);
        context.Accounts.Add(new Account { AccountId = 1234, FirstName = "Test", LastName = "User" });
        context.SaveChanges();
        return context;
    }

    private Stream CreateCsvStream(string csv)
    {
        var bytes = Encoding.UTF8.GetBytes(csv);
        return new MemoryStream(bytes);
    }

    [Fact]
    public async Task Should_Fail_When_AccountId_Does_Not_Exist()
    {
        // Arrange
        var context = GetDbContextWithAccounts();
        var service = new MeterReadingServiceImplementation(context);
        var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n9999,22/04/2019 09:24,12345";

        // Act
        var result = await service.ProcessMeterReadingsAsync(CreateCsvStream(csv));

        // Assert
        Assert.Equal(0, result.SuccessfulReadings);
        Assert.Equal(1, result.FailedReadings);
        Assert.Contains(result.Errors, e => e.Contains("Account 9999 not found"));
    }

    [Fact]
    public async Task Should_Fail_When_MeterReadValue_Is_Not_5Digits()
    {
        var context = GetDbContextWithAccounts();
        var service = new MeterReadingServiceImplementation(context);
        var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1234,22/04/2019 09:24,abc";

        var result = await service.ProcessMeterReadingsAsync(CreateCsvStream(csv));

        Assert.Equal(0, result.SuccessfulReadings);
        Assert.Equal(1, result.FailedReadings);
        Assert.Contains(result.Errors, e => e.Contains("Invalid meter read value"));
    }

    [Fact]
    public async Task Should_Succeed_When_Valid_Reading_Is_Provided()
    {
        var context = GetDbContextWithAccounts();
        var service = new MeterReadingServiceImplementation(context);
        var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1234,22/04/2019 09:24,12345";

        var result = await service.ProcessMeterReadingsAsync(CreateCsvStream(csv));

        Assert.Equal(1, result.SuccessfulReadings);
        Assert.Equal(0, result.FailedReadings);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Should_Fail_When_Duplicate_Reading_Is_Uploaded()
    {
        var context = GetDbContextWithAccounts();
        var service = new MeterReadingServiceImplementation(context);

        var csv1 = "AccountId,MeterReadingDateTime,MeterReadValue\n1234,22/04/2019 09:24,12345";
        await service.ProcessMeterReadingsAsync(CreateCsvStream(csv1)); // first insert

        var csv2 = "AccountId,MeterReadingDateTime,MeterReadValue\n1234,22/04/2019 09:24,12345";
        var result = await service.ProcessMeterReadingsAsync(CreateCsvStream(csv2)); // duplicate

        Assert.Equal(0, result.SuccessfulReadings);
        Assert.Equal(1, result.FailedReadings);
        Assert.Contains(result.Errors, e => e.Contains("Duplicate entry"));
    }

    [Fact]
    public async Task Should_Fail_When_MeterReadValue_Is_Less_Than_5Digits()
    {
        var context = GetDbContextWithAccounts();
        var service = new MeterReadingServiceImplementation(context);
        var csv = "AccountId,MeterReadingDateTime,MeterReadValue\n1234,22/04/2019 09:24,1234";

        var result = await service.ProcessMeterReadingsAsync(CreateCsvStream(csv));

        Assert.Equal(0, result.SuccessfulReadings);
        Assert.Equal(1, result.FailedReadings);
        Assert.Contains(result.Errors, e => e.Contains("Invalid meter read value"));
    }


}
