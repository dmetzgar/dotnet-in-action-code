using Microsoft.AspNetCore.Mvc;

namespace AcController.Controllers;

[ApiController]
[Route("[controller]")]
public class MeasureController : ControllerBase
{
    private static readonly Measurement ExhaustAirTemp = 
        new (nameof(ExhaustAirTemp), 0);
    private static readonly Measurement CoolantTemp = 
        new (nameof(CoolantTemp), 0);
    private static readonly Measurement OutsideAirTemp = 
        new (nameof(OutsideAirTemp), 0);

    private static readonly Measurement[] _measurements = new[] {
        ExhaustAirTemp, CoolantTemp, OutsideAirTemp
    };

    [HttpGet("{site}/{unitId}")]
    public Temperatures Get(
        [FromRoute] string site, [FromRoute] int unitId)
    {
        return new Temperatures(unitId, site, 
            DateTimeOffset.UtcNow, _measurements);
    }
}
