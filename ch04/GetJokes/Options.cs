using CommandLine;

namespace GetJokes;

public record Options
{
  [Value(0, Required = true, HelpText = "Input file")]
  public string? InputFile { get; init; }

  [Value(1, Required = false, HelpText = "Output file")]
  public string? OutputFile { get; init; }

  [Option('c', "category", HelpText = "Joke category")]
  public string Category { get; init; } = "programming";
}
