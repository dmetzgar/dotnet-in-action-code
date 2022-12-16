namespace XkcdComicFinder;

public class ComicFinder
{
  private readonly IXkcdClient _xkcdClient;
  private readonly IComicRepository _repo;

  public ComicFinder(
    IXkcdClient xkcdClient, 
    IComicRepository repo)
  {
    _xkcdClient = xkcdClient;
    _repo = repo;
  }

  public async Task<IAsyncEnumerable<Comic>> FindAsync(string searchText)
  {
    // We'll check if we have the latest every time since we want all the matches instead of just one match
    var latestComic = await _xkcdClient.GetLatestAsync();
    int latestInRepo = await _repo.GetLatestNumberAsync();
    if (latestComic.Number > latestInRepo)
    {
      await FetchAsync(latestComic, latestInRepo);
    }

    return _repo.Find(searchText);
  }

  private async Task FetchAsync(Comic latestComic, int latestInRepo)
  {
    await _repo.AddComicAsync(latestComic);
    int current = latestComic.Number - 1;
    while (current > latestInRepo)
    {
      var comic = await _xkcdClient.GetByNumberAsync(current);
      if (comic != null)
      {
        await _repo.AddComicAsync(comic);
      }

      current--;
    }
  }
}
