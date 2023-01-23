using System.ComponentModel.DataAnnotations;

namespace ManningBooksAsync;

public class Rating
{
  public int Id { get; set; }

  [Range(1, 5)]
  public int Stars { get; set; } = 5;

  public string? Comment { get; set; }
}