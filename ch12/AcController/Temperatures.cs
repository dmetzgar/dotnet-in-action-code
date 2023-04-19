using System.Resources;

namespace AcController;

public record Temperatures (
  int UnitId,
  string Site,
  DateTimeOffset Timestamp,
  IEnumerable<Measurement> Measurements
);

public class Measurement
{
  private static readonly ResourceManager s_resMan =
    new ("AcController.SensorNames",
    typeof(Measurement).Assembly);

  public string SensorName { get; set; }
  public decimal Value { get; set; }
  public string Description { 
    get => s_resMan.GetString(SensorName)!;
  }

  public Measurement(string sensorName, decimal value)
  {
    SensorName = sensorName;
    Value = value;
  }
}
