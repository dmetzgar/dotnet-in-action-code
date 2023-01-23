using Microsoft.EntityFrameworkCore;

namespace ManningBooks;

public class CatalogContext : DbContext
{
  public const string ConnectionString =
    "DataSource=manningbooks;mode=memory;cache=shared";

  public DbSet<Book> Books => Set<Book>();

  protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options
      .AddInterceptors(new DelayInterceptor())
      .UseSqlite(ConnectionString);

  public static void SeedBooks() 
  {
    using var dbContext = new CatalogContext();
    dbContext.Database.EnsureCreated();
    dbContext.Add(new Book("Grokking Simplicity"));
    dbContext.Add(new Book("API Design Patterns"));
    var efBook = new Book("EF Core in Action");
    efBook.Ratings.Add(new Rating { Comment = "Great!" });
    efBook.Ratings.Add(new Rating { Stars = 4 });
    dbContext.Add(efBook);
    dbContext.SaveChanges();
  }

  public static void WriteBookToConsole(string title)
  {
    using var dbContext = new CatalogContext();
    var book = dbContext.Books
      //.Where(b => /*!string.IsNullOrEmpty(title) ||*/ b.Title == title)
      .Where(b => title == "Blah" || b.Title == title)
      .Include(b => b.Ratings)
      .FirstOrDefault();
    if (book == null)
    {
      Console.WriteLine(@$"""{title}"" not found.");
    }
    else 
    {
      Console.WriteLine(@$"Book ""{book.Title}"" has id {book.Id}");
      book.Ratings.ForEach(r => 
        Console.WriteLine(
        $"\t{r.Stars} stars: {r.Comment ?? "-blank-"}"));
    }
  }
}