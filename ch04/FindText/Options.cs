using CommandLine;

namespace FindText;

public record Options
{
  [Value(0, Required = true, HelpText = "Text to search for")]
  public string? Text { get; init; }

  [Value(1, Required = false, HelpText = "File to search")]
  public string? Filename { get; init; }
}
