using Microsoft.EntityFrameworkCore;

namespace XkcdComicFinder;

public class ComicDbContext : DbContext
{
  public DbSet<Comic> Comics { get; set; } = null!;

  public ComicDbContext(DbContextOptions options) :
    base(options) { }
}
