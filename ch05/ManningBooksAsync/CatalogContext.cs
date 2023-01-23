using Microsoft.EntityFrameworkCore;

namespace ManningBooksAsync;

public class CatalogContext : DbContext
{
  public const string ConnectionString =
    "DataSource=manningbooks;mode=memory;cache=shared";

  public DbSet<Book> Books => Set<Book>();

  protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options
      .AddInterceptors(new DelayInterceptor())
      .UseSqlite(ConnectionString);

  public static async Task SeedBooks() 
  {
    using var dbContext = new CatalogContext();
    await dbContext.Database.EnsureCreatedAsync();
    dbContext.Add(new Book("Grokking Simplicity"));
    dbContext.Add(new Book("API Design Patterns"));
    var efBook = new Book("EF Core in Action");
    efBook.Ratings.Add(new Rating { Comment = "Great!" });
    efBook.Ratings.Add(new Rating { Stars = 4 });
    dbContext.Add(efBook);
    await dbContext.SaveChangesAsync();
  }

  public static void WriteBookToConsole(string title)
  {
    using var dbContext = new CatalogContext();
    var book = dbContext.Books
      .Include(b => b.Ratings)
      .FirstOrDefault(b => b.Title == title);
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
  
  public static async Task WriteBookToConsoleAsync(string title)
  {
    using var dbContext = new CatalogContext();
    var book = await dbContext.Books
      .Include(b => b.Ratings)
      .FirstOrDefaultAsync(b => b.Title == title);
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