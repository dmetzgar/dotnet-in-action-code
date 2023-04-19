using System.Text.Json;

namespace AcController;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var cultures = new[] { "en-US", "es-MX", "ar-SA" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(cultures[0])
            .AddSupportedCultures(cultures)
            .AddSupportedUICultures(cultures);
        app.UseRequestLocalization(localizationOptions);

        app.MapControllers();

        app.Run();
    }
}
