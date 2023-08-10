using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EnvTest.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly CosmosDbSettings _cosmos;

    public ConfigController(IConfiguration config,
      IOptions<CosmosDbSettings> cosmosOptions)
    {
        _config = config;
        _cosmos = cosmosOptions.Value;
    }

    [HttpGet]
    public string Get() => $$"""
        {
            "TestOverwrite" : "{{_config["TestOverwrite"]}}",
            "EnvironmentName" : "{{_config["EnvironmentName"]}}",
            "RegionName" : "{{_config["RegionName"]}}",
            "Me" : "{{_config["Level1:Level2Array:0:Level3"]}}"
        }
        """;

    [HttpGet("cosmos")]
    public CosmosDbSettings GetCosmos() => _cosmos;
}