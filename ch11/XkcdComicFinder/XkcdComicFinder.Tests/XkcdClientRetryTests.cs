using Polly;
using static System.Net.HttpStatusCode;

namespace XkcdComicFinder.Tests;

public class XkcdClientRetryTests
{
  private const string LatestJson = 
    /*lang=json,strict*/ """
    {
      "month": "6",
      "num": 2630,
      "link": "",
      "year": "2022",
      "safe_title": "Shuttle Skeleton",
      "title": "Shuttle Skeleton",
      "day": "8"
    }
    """;

  [Fact]
  public async Task FailsTwiceThenSucceeds()
  {
    var policy = Policy<HttpResponseMessage>
      .Handle<HttpRequestException>()
      .RetryAsync(retryCount: 3);

    var handler =new TestPolicyHttpMessageHandler(policy);
    var httpClient = new HttpClient(handler);
    httpClient.BaseAddress = new Uri("https://xkcd.com");

    var callCount = 0;
    handler.OnSendAsync = (req, c, ct) =>
    {
      if (callCount < 2)
      {
        callCount++;
        throw new HttpRequestException();
      }
      else
      {
        return Task.FromResult(
          new HttpResponseMessage()
          {
            StatusCode = OK,
            Content = new StringContent(LatestJson)
          });
      }
    };
    var xkcdClient = new XkcdClient(httpClient);

    var comic = await xkcdClient.GetLatestAsync();
    Assert.Equal(2, callCount);
    Assert.Equal(2630, comic.Number);
  }
}
