
using Ensek.MeterReadingService.Data;
using Ensek.MeterReadingService.Services;
using Ensek.MeterReadingService.Swagger;
using Microsoft.EntityFrameworkCore;

namespace Ensek.MeterReadingService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<IMeterReadingService, MeterReadingServiceImplementation>();


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Ensek Meter Reading API",
                Version = "v1"
            });

            // Remove all operation filters for now
            // Do NOT add FileUploadOperationFilter or anything custom
        });



        // Register SQL Server DbContext
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("EnsekDB")));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi2_0;
            });
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated(); // Make sure DB exists
            DataSeeder.SeedAccounts(db);   // Seed from CSV
        }


        app.Run();
    }
}
