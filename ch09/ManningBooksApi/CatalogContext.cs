using Microsoft.EntityFrameworkCore;

namespace ManningBooksApi;

public class CatalogContext : DbContext
{
  public DbSet<Book> Books => Set<Book>();

  public CatalogContext(DbContextOptions options) :
    base(options) { }
}
