using Microsoft.EntityFrameworkCore;

namespace MoneyTransfer;

public class BankContext : DbContext
{
  public DbSet<Account> Accounts { get; set; } = null!;

  public BankContext(DbContextOptions options) :
    base(options) { }
}
