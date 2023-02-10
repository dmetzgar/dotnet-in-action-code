using System.Text.Json;
using System.Text.Json.Serialization;

namespace XkcdSearch;

public record Comic
{
  [JsonPropertyName("num")]
  public int Number { get; set; }

  [JsonPropertyName("safe_title")]
  public string? Title { get; set; }

  [JsonPropertyName("month")]
  public string? Month { get; set; }

  [JsonPropertyName("day")]
  public string? Day { get; set; }

  [JsonPropertyName("year")]
  public string? Year { get; set; }

  [JsonIgnore]
  public DateOnly Date =>
    DateOnly.Parse($"{Year}-{Month}-{Day}");

  private static HttpClient client = new HttpClient()
    { BaseAddress = new Uri("https://xkcd.com") };
  
  public static Comic? GetComic(int number)
  {
    try 
    {
      var path = number == 0 ? "info.0.json" 
        : $"{number}/info.0.json";
      var stream = client.GetStreamAsync(path).Result;

      return JsonSerializer.Deserialize<Comic>(stream);
    }
    catch (AggregateException e)
      when (e.InnerException is HttpRequestException)
    {
      WriteLine(e.InnerException.Message);
      return null;
    }
    catch (HttpRequestException e)
    {
      WriteLine(e.Message);
      return null;
    }
  }

  public static async Task<Comic?> GetComicAsync(
    int number,
    CancellationToken cancellationToken)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return null;
    }

    try
    {
      var path = number == 0 ? "info.0.json"
        : $"{number}/info.0.json";
      var stream = await client.GetStreamAsync(
        path, cancellationToken);

      return await JsonSerializer.DeserializeAsync<Comic>(
        stream, cancellationToken: cancellationToken);
    }
    catch (Exception ex) when (
      (ex is AggregateException 
        && ex.InnerException is HttpRequestException)
      || ex is HttpRequestException
      || ex is TaskCanceledException)
    {
      return null;
    }
    // catch (AggregateException e)
    //   when (e.InnerException is HttpRequestException)
    // {
    //   return null;
    // }
    // catch (HttpRequestException)
    // {
    //   return null;
    // }
    // catch (TaskCanceledException)
    // {
    //   return null;
    // }
  }
}
