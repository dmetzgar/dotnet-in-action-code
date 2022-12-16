using CommandLine;

namespace FindFiles;

public record Options
{
  [Value(0, Required = true, HelpText = "Some text")]
  public string RootFolder { get; init; } = ".";

  [Value(1, Required = false, HelpText = "Filename filter")]
  public string Filter { get; init; } = "*";

  [Option('r', "recurse", HelpText = "Search subfolders recursively")]
  public bool Recursive { get; init; } = false;
}
