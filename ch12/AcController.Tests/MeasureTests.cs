using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using static System.Net.HttpStatusCode;

namespace AcController.Tests;

public class MeasureTests
{
    private static JsonSerializerOptions s_jsonOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    private readonly WebApplicationFactory<Program>
        _factory;
    
    private readonly HttpClient _client;
    
    public MeasureTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async void RespondsOK() 
    {
        var response = await _client.GetAsync("/measure/TIJ1/1");
        Assert.Equal(OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var temps = JsonSerializer.Deserialize<TempsDto>(responseBody, s_jsonOptions);
        Assert.NotNull(temps);
        Assert.Collection(temps.Measurements,
          m => Assert.Equal("Exhaust Air Temperature", m.Description),
          m => Assert.Equal("Coolant Temperature", m.Description),
          m => Assert.Equal("Outside Air Temperature", m.Description)
        );
    }

    [Theory]
    [InlineData("/measure/TIJ1/1",
        "Exhaust Air Temperature",
        "Coolant Temperature",
        "Outside Air Temperature")]
    [InlineData("/measure/TIJ1/1?culture=es-MX", 
        "Temperatura del aire de escape",
        "Temperatura del refrigerante",
        "Temperatura del aire exterior")]
    public async void CultureDescriptions(
        string url, params string[] descriptions)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(OK, response.StatusCode);
        var responseBody = await response.Content
            .ReadAsStringAsync();
        var temps = JsonSerializer.Deserialize<TempsDto>(
            responseBody, s_jsonOptions);
        Assert.NotNull(temps);

        Assert.Collection(temps.Measurements,
            descriptions.Select(d => new Action<MeasurementDto>(
                m => Assert.Equal(d, m.Description))).ToArray());
    }

    internal record TempsDto(
        int UnitId,
        string Site,
        DateTimeOffset Timestamp,
        IEnumerable<MeasurementDto> Measurements
    ) { }

    public record MeasurementDto(
        string SensorName,
        decimal Value,
        string Description
    ) { }
}
