namespace XkcdComicFinder;

public interface IComicRepository
{
  Task<int> GetLatestNumberAsync();

  Task AddComicAsync(Comic comic);

  IAsyncEnumerable<Comic> Find(string searchText);
}
