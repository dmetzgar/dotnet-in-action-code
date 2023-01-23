namespace ManningBooksApi;

public class Book
{
  public int Id { get; set; }

  public string Title { get; set; }

  public string? Description { get; set; }

  public List<Rating> Ratings { get; } = new();

  public Book(string title, string? description = null)
  {
    Title = title;
    Description = description;
  }
}
