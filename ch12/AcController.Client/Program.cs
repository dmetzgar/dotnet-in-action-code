using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;

var culture = CultureInfo
  .CreateSpecificCulture("ar-SA");
Thread.CurrentThread.CurrentCulture = culture;
Thread.CurrentThread.CurrentUICulture = culture;

var client = new HttpClient() {
  BaseAddress = new Uri("http://localhost:5022")  
};
client.DefaultRequestHeaders.AcceptLanguage.Add(
  new StringWithQualityHeaderValue(culture.Name));

var responseBody = await client.GetStringAsync(
  "/measure/TIJ1/1");
var temps = JsonSerializer.Deserialize<TempsDto>(
  responseBody, new JsonSerializerOptions() {
    PropertyNameCaseInsensitive = true
  }
);

Console.WriteLine("Timestamp: " + 
  temps!.Timestamp.LocalDateTime);
Console.WriteLine("Site     : " + temps.Site);
Console.WriteLine("Unit ID  : " + temps.UnitId);
foreach (var measure in temps.Measurements)
{
  var reading = culture.TextInfo.IsRightToLeft 
    ? $"C {measure.Value} :{measure.Description}"
    : $"{measure.Description}: {measure.Value} C";
  Console.WriteLine(reading);
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
