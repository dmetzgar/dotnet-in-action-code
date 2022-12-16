using FindText;
using CommandLine;
using CommandLine.Text;
using System.Security;

var results = Parser.Default.ParseArguments<Options>(args)
  .WithParsed<Options>(options => {
    if (options.Filename != null)
    {
      SearchFile(new FileInfo(options.Filename),
        options.Text!);
    }
    else
    {
      string? filename;
      while (!string.IsNullOrWhiteSpace(
        filename = ReadLine()))
      {
        SearchFile(new FileInfo(filename),
          options.Text!);
      }
    }
  });

results.WithNotParsed(_ =>
  WriteLine(HelpText.RenderUsageText(results)));

static void SearchFile(
  FileInfo file,
  string text)
{
  if (!file.Exists)
  {
    Console.Error.WriteLine(
      $"{file.FullName} does not exist");
    return;
  }

  try 
  {
    using var reader = file.OpenText();
    string? line;
    while ((line = reader.ReadLine()) != null)
    {
      if (line.Contains(text,
        StringComparison.OrdinalIgnoreCase))
      {
        WriteLine(line);
      }
    }
  }
  catch (UnauthorizedAccessException)
  {
    Console.Error.WriteLine(
      $"Unauthorized: {file.FullName}");
  }
  catch (IOException)
  {
    Console.Error.WriteLine(
      $"IO error: {file.FullName}");
  }
}
