using XkcdSearch;
using CommandLine;
using CommandLine.Text;
using System.Diagnostics;

var results = await Parser.Default
  .ParseArguments<Options>(args)
  .WithParsedAsync<Options>(
    async options => {
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      var comic = await GetComicWithTitleAsync(
        options.Title!);
      stopwatch.Stop();
      WriteLine("Result returned in " +
        $"{stopwatch.ElapsedMilliseconds} ms");

      if (comic == null) 
      {
        WriteLine($"xkcd comic with title " +
          $"\"{options.Title}\" not found");
      }
      else
      {
        WriteLine($"xkcd \"{options.Title}\" is number " +
          $"{comic.Number} published on {comic.Date}");
      }
    });

results.WithNotParsed(_ =>
  WriteLine(HelpText.RenderUsageText(results)));

static Comic? GetComicWithTitle(string title)
{
  var lastComic = Comic.GetComic(0);
  for (int number = lastComic!.Number;
       number > 0;
       number--)
  {
    var comic = Comic.GetComic(number);
    if (comic != null &&
      string.Equals(title, comic!.Title,
      StringComparison.OrdinalIgnoreCase))
    {
      return comic;
    }
  }

  return null;
}

static async Task<Comic?> GetComicWithTitleAsync(
  string title)
{
  var cancellationToken = new CancellationTokenSource();
  var lastComic = await Comic.GetComicAsync(0,
    cancellationToken.Token);
  var tasks = new List<Task>();
  Comic? foundComic = null;
  for (int number = lastComic!.Number;
       number > 0;
       number--)
  {
    var localNumber = number;
    var getComicTask = Comic.GetComicAsync(localNumber,
      cancellationToken.Token);
    var continuationTask = getComicTask.ContinueWith(
      t => {
        try
        {
          var comic = t.Result;
          if (comic != null &&
            string.Equals(title, comic!.Title,
            StringComparison.OrdinalIgnoreCase))
          {
            cancellationToken.Cancel();
            foundComic = comic;
          }
        }
        catch (TaskCanceledException) {}
      });
    tasks.Add(continuationTask);
  }

  await Task.WhenAll(tasks);
  return foundComic;
}
