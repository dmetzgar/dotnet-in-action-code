namespace ManningBooks;

public class Book
{
  public int Id { get; set; }

  public string Title { get; set; }

  public List<Rating> Ratings { get; } = new();

  public Book(string title)
  {
    Title = title;
  }
}