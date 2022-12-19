using CommandLine;

namespace HelloDotnet;

public record Options
{
  [Value(0, Required = true)]
  public string? Text { get; init; }

  [Option('f', "font")]
  public string? Font { get; init; }

  [Value(1)]
  public IEnumerable<string>? OtherWords { get; init; }
}
