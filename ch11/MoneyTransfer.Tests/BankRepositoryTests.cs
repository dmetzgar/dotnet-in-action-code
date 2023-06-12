using Microsoft.EntityFrameworkCore;

namespace MoneyTransfer.Tests;

public class BankRepositoryTests : IDisposable
{
  private readonly BankContext _bankContext;
  private readonly BankRepository _bankRepo;

  public BankRepositoryTests()
  {
    var optionsBuilder = new DbContextOptionsBuilder();
    optionsBuilder.UseSqlServer(
      "Server=.\\SQLExpress;Database=bank;" +
      "TrustServerCertificate=true;" +
      "Trusted_Connection=Yes;", 
      sqlOptions => sqlOptions.EnableRetryOnFailure());
    var options = optionsBuilder.Options;
    _bankContext = new(options);
    _bankContext.Database.EnsureCreated();
    _bankRepo = new(_bankContext);
  }

  [Fact]
  public async Task TransferSucceeds()
  {
    _bankContext.Accounts.Add(new("A", 100));
    _bankContext.Accounts.Add(new("B", 100));
    await _bankContext.SaveChangesAsync();

    await _bankRepo.TransferAsync("A", "B", 30);

    var fromAccount = _bankContext.Accounts.Find("A");
    Assert.NotNull(fromAccount);
    Assert.Equal(70, fromAccount.Balance);

    var toAccount = _bankContext.Accounts.Find("B");
    Assert.NotNull(toAccount);
    Assert.Equal(130, toAccount.Balance);
  }

  [Fact]
  public async Task TransferFails()
  {
    _bankContext.Accounts.Add(new("A", 100));
    _bankContext.Accounts.Add(new("B", 100));
    await _bankContext.SaveChangesAsync();

    await Assert.ThrowsAsync<InsufficientFundsException>(
      () => _bankRepo.TransferAsync("A", "B", 120));

    var fromAccount = _bankContext.Accounts.Find("A");
    Assert.NotNull(fromAccount);
    Assert.Equal(100, fromAccount.Balance);

    var toAccount = _bankContext.Accounts.Find("B");
    Assert.NotNull(toAccount);
    Assert.Equal(100, toAccount.Balance);
  }

  public void Dispose()
  {
    _bankContext.Database.ExecuteSqlRaw("TRUNCATE TABLE [Accounts]");
    _bankContext.Dispose();
  }
}
