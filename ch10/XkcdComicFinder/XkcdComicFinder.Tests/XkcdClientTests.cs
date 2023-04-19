using System.Net;
using static System.Net.HttpStatusCode;

namespace XkcdComicFinder.Tests;

public class XkcdClientTests
{
  private readonly XkcdClient xkcdClient;
  private readonly IProtectedMock<HttpMessageHandler>
    _protectedMsgHandler;

  private const string LatestJson = /*lang=json,strict*/
@"{
  ""month"": ""6"",
  ""num"": 2630,
  ""link"": """",
  ""year"": ""2022"",
  ""news"": """",
  ""safe_title"": ""Shuttle Skeleton"",
  ""transcript"": """",
  ""alt"": ""It's believed to be related to the Stellar Sea Cow."",
  ""img"": ""https://imgs.xkcd.com/comics/shuttle_skeleton.png"",
  ""title"": ""Shuttle Skeleton"",
  ""day"": ""8""
}";

  public XkcdClientTests()
  {
    (_protectedMsgHandler, var httpClient) = 
      SetupHttpClient();
    xkcdClient = new(httpClient);
  }

  [Fact]
  public async Task GetLatest()
  {
    SetResponse(OK, LatestJson);
    var comic = await xkcdClient.GetLatestAsync();
    Assert.Equal(2630, comic.Number);
  }

  [Fact]
  public async Task NoComicFound()
  {
    SetResponse(NotFound);
    var comic = await xkcdClient.GetByNumberAsync(1);
    Assert.Null(comic);
  }

  [Fact]
  public async Task GetByNumber()
  {
    SetResponse(OK, LatestJson);
    var comic = await xkcdClient.GetByNumberAsync(2630);
    Assert.NotNull(comic);
    Assert.Equal(2630, comic.Number);
  }

  internal static (IProtectedMock<HttpMessageHandler>, 
    HttpClient) SetupHttpClient()
  {
    var mockMsgHandler = new Mock<HttpMessageHandler>();
    var protectedMock = mockMsgHandler.Protected();
    var httpClient = new HttpClient(mockMsgHandler.Object);
    httpClient.BaseAddress = new Uri("https://xkcd.com");
    return (protectedMock, httpClient);
  }

  private void SetResponse(
    HttpStatusCode statusCode,
    string content = "")
  {
    _protectedMsgHandler
      .Setup<Task<HttpResponseMessage>>("SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),  // Use ItExpr for protected mocks
        ItExpr.IsAny<CancellationToken>())
      .ReturnsAsync(new HttpResponseMessage()
      {
        StatusCode = statusCode,
        Content = new StringContent(content)
      });
  }
}
