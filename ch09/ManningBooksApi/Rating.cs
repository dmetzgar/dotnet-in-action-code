namespace ManningBooksApi;

public class Rating
{
  public int Id { get; set; }

  public int Stars { get; set; } = 5;

  public string? Comment { get; set; }
}
