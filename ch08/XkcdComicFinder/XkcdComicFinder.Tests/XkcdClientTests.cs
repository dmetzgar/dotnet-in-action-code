using FakeItEasy;

namespace XkcdComicFinder.Tests;

public class XkcdClientTests
{
  private readonly XkcdClient xkcdClient;
  private readonly HttpMessageHandler _fakeMsgHandler;

  private const string LatestJson = /*lang=json,strict*/ """
    {
      "month": "6",
      "num": 2630,
      "link": "",
      "year": "2022",
      "news": "",
      "safe_title": "Shuttle Skeleton",
      "transcript": "",
      "alt": "It's believed to be related to the Stellar Sea Cow.",
      "img": "https://imgs.xkcd.com/comics/shuttle_skeleton.png",
      "title": "Shuttle Skeleton",
      "day": "8"
    }
    """;

  public XkcdClientTests()
  {
    _fakeMsgHandler = A.Fake<HttpMessageHandler>();
    var httpClient = SetupHttpClient(_fakeMsgHandler);
    xkcdClient = new(httpClient);
  }

  [Fact]
  public async Task GetLatest()
  {
    SetResponse(HttpStatusCode.OK, LatestJson);
    var comic = await xkcdClient.GetLatestAsync();
    Assert.Equal(2630, comic.Number);
  }

  [Fact]
  public async Task NoComicFound()
  {
    SetResponse(HttpStatusCode.NotFound);
    var comic = await xkcdClient.GetByNumberAsync(1);
    Assert.Null(comic);
  }

  [Fact]
  public async Task GetByNumber()
  {
    SetResponse(HttpStatusCode.OK, LatestJson);
    var comic = await xkcdClient.GetByNumberAsync(2630);
    Assert.NotNull(comic);
    Assert.Equal(2630, comic.Number);
  }

  internal static HttpClient SetupHttpClient(
    HttpMessageHandler msgHandler)
  {
    var httpClient = new HttpClient(msgHandler);
    httpClient.BaseAddress = new Uri("https://xkcd.com");
    return httpClient;
  }

  private void SetResponse(
    HttpStatusCode statusCode,
    string content = "")
  {
    A.CallTo(_fakeMsgHandler)
      .WithReturnType<Task<HttpResponseMessage>>()
      .Where(c => c.Method.Name == "SendAsync")
      .Returns(new HttpResponseMessage()
      {
        StatusCode = statusCode,
        Content = new StringContent(content),
      });
  }
}
