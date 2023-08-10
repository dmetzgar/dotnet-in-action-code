public record CosmosDbSettings
{
  public string DatabaseName { get; init; } = null!;
  public string Endpoint { get; init; } = null!;
  public string AuthKey { get; init; } = null!;
}