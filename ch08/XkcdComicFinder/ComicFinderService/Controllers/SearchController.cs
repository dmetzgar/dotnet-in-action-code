using Microsoft.AspNetCore.Mvc;
using XkcdComicFinder;

namespace ComicFinderService.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
  private readonly ComicFinder _comicFinder;

  public SearchController(
    ComicFinder comicFinder)
  {
    _comicFinder = comicFinder;
  }

  [HttpGet]
  public Task<IAsyncEnumerable<Comic>> FindAsync(
    string searchText) =>
    _comicFinder.FindAsync(searchText);
}
