using Microsoft.EntityFrameworkCore;

namespace ManningBooksApi;

public class CatalogContext : DbContext
{
  public DbSet<Book> Books { get; set; } = null!;

  public CatalogContext(DbContextOptions options) :
    base(options) { }
}
