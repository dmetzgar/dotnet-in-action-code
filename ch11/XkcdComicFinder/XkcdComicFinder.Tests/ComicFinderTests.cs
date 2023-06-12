using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace XkcdComicFinder.Tests;

public class ComicFinderTests : IDisposable
{
  private const string NumberLink = "https://xkcd.com/{0}/info.0.json";
  private const string LatestLink = "https://xkcd.com/info.0.json";
  private readonly ComicDbContext _comicDbContext;
  private readonly SqliteConnection _keepAliveConn;
  private readonly IProtectedMock<HttpMessageHandler>
    _protectedMsgHandler;
  private readonly ComicFinder _comicFinder;

  public ComicFinderTests()
  {
    (_comicDbContext, _keepAliveConn) = 
      ComicRepositoryTests.SetupSqlite("comics_int");
    var comicRepo = new ComicRepository(_comicDbContext);

    (_protectedMsgHandler, var httpClient) =
      XkcdClientTests.SetupHttpClient();
    var xkcdClient = new XkcdClient(httpClient);

    _comicFinder = new ComicFinder(xkcdClient, comicRepo);
  }

  [Fact]
  [Trait("Category", "Integration")]
  public async Task StartWithEmptyRepo()
  {
    SetResponseComics(_protectedMsgHandler,
      new Comic() { Number = 12, Title = "b" },
      new Comic() { Number = 1,  Title = "a" },
      new Comic() { Number = 4,  Title = "c" });

    var foundComics = (await _comicFinder.FindAsync("b"))
      .ToBlockingEnumerable();
    Assert.Single(foundComics);
    Assert.Single(foundComics, c => c.Number == 12);
  }

  private static Uri GetUri(Comic c) => 
    new(string.Format(NumberLink, c.Number));

  internal static void SetResponseComics(
    IProtectedMock<HttpMessageHandler> mockMsgHandler,
    params Comic[] comics)
  {
    var responses = comics.ToDictionary(
      GetUri, 
      c => JsonSerializer.Serialize(c));
    responses.Add(new Uri(LatestLink), 
      JsonSerializer.Serialize(comics[0]));

    mockMsgHandler
      .Setup<Task<HttpResponseMessage>>("SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>())
      .ReturnsAsync(
        (HttpRequestMessage req, CancellationToken _) => {
          var response = new HttpResponseMessage();
          response.StatusCode = NotFound;
          if (responses.TryGetValue(req.RequestUri!, 
            out var content))
          {
            response.StatusCode = OK;
            response.Content = new StringContent(content);
          }
          return response;
        });
  }

  public void Dispose()
  {
    _keepAliveConn.Close();
    _comicDbContext.Dispose();
  }
}
