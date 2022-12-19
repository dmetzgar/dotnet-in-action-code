using HelloDotnet;
using CommandLine;

Parser.Default.ParseArguments<Options>(args)
  .WithParsed<Options>(AsciiArt.Write)
  .WithNotParsed(_ => 
    {
      WriteLine("Usage: Hello Dotnet <text> --font Big");
      WriteLine("Available fonts are: ");
      AsciiArt.WriteAllFontNames();
    });
