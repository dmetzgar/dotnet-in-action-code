using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace XkcdComicFinder;

public record Comic
{
  [Key]
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
  [JsonPropertyName("alt")]
  public string? AltText { get; set; }
  [JsonIgnore]
  public DateOnly Date =>
    DateOnly.Parse($"{Year}-{Month}-{Day}");
}
