using System.Text.Json;
using ComicFinderService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace XkcdComicFinder.Tests;

public class SearchControllerTests
{
  private const string BaseAddress = "https://xkcd.com";
  private readonly WebApplicationFactory<Program> _factory;
  private readonly IProtectedMock<HttpMessageHandler>
    _protectedMsgHandler;

  public SearchControllerTests()
  {
    var mockMsgHandler = new Mock<HttpMessageHandler>();
    _protectedMsgHandler = mockMsgHandler.Protected();

    _factory = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(builder =>
      {
        builder.UseEnvironment("Integration");
        builder.ConfigureServices(services =>
        {
          ServiceDescriptor sd = services.First(
            s => s.ServiceType == typeof(HttpClient));
          services.Remove(sd);
          services.AddHttpClient<IXkcdClient, XkcdClient>(
            h => h.BaseAddress = new Uri(BaseAddress))
            .ConfigurePrimaryHttpMessageHandler(
              () => mockMsgHandler.Object);
        });
      });
  }

  [Fact]
  public async Task FoundB()
  {
    ComicFinderTests.SetResponseComics(
      _protectedMsgHandler,
      new Comic() { Number = 12, Title = "b" },
      new Comic() { Number = 1,  Title = "a" },
      new Comic() { Number = 4,  Title = "c" });

    HttpClient client = _factory.CreateClient();
    var response = await client.GetAsync(
      "/search?searchText=b");

    Assert.Equal(OK, response.StatusCode);
    string content = await response.Content
      .ReadAsStringAsync();
    var comics = JsonSerializer.Deserialize<Comic[]>(
      content);
    Assert.NotNull(comics);
    Assert.Single(comics);
    Assert.Single(comics, c => c.Number == 12);
  } 
}
