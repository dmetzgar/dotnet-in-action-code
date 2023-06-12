using XkcdComicFinder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

namespace ComicFinderService;

public class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    var services = builder.Services;
    var cfg = builder.Configuration;
    var connStr = cfg.GetConnectionString("Sqlite");
    var baseAddr = cfg.GetValue<string>("BaseAddr");

    services.AddScoped<IComicRepository, 
      ComicRepository>();
    services.AddScoped<IXkcdClient, XkcdClient>();
    services.AddScoped<ComicFinder>();
    services.AddControllers();
    services.AddDbContext<ComicDbContext>(option =>
      option.UseSqlite(connStr));
    services.AddHttpClient<IXkcdClient, XkcdClient>(
      client => client.BaseAddress = 
      new Uri(baseAddr!))
      .AddPolicyHandler(GetRetryPolicy());

    var app = builder.Build();
    app.MapControllers();

    using var keepAliveConn = 
      new SqliteConnection(connStr);
    keepAliveConn.Open();

    using (var scope = app.Services.CreateScope())
    {
      var dbCtxt = scope.ServiceProvider
        .GetRequiredService<ComicDbContext>();
      dbCtxt.Database.EnsureCreated();
    }

    app.Run();
  }

  static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    => HttpPolicyExtensions
      .HandleTransientHttpError()
      .WaitAndRetryAsync(6, retryCount => 
        TimeSpan.FromMilliseconds(
          100 * Math.Pow(2, retryCount)));
}
