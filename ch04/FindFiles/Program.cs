using FindFiles;
using CommandLine;
using CommandLine.Text;

var results = Parser.Default.ParseArguments<Options>(args)
  .WithParsed<Options>(options => {
    RecursiveFind(new DirectoryInfo(options.RootFolder), options.Filter, options.Recursive);
  });

results.WithNotParsed(_ =>
  WriteLine(HelpText.RenderUsageText(results)));

static void RecursiveFind(
  DirectoryInfo folder,
  string filter,
  bool recurse
)
{
  if (!folder.Exists)
  {
    WriteLine($"{folder.FullName} does not exist");
    return;
  }

  WriteLine(folder.FullName);
  try
  {
    foreach (var file in folder.EnumerateFiles(filter))
    {
      WriteLine($"\t{file.Name}");      
    }
  }
  catch (System.Security.SecurityException)
  {
    return;
  }

  if (recurse)
  {
    foreach (var subFolder in folder.GetDirectories())
    {
      RecursiveFind(subFolder, filter, recurse);
    }
  }
}
