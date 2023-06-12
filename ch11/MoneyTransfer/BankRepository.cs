namespace MoneyTransfer;

public class BankRepository
{
  private readonly BankContext _ctxt;

  public BankRepository(BankContext dbContext)
    => _ctxt = dbContext;

  public async Task DebitAsync(string acc, decimal amt)
  {
    var account = await GetAccountAsync(acc);

    if ((account.Balance - amt) < 0)
    {
      throw new InsufficientFundsException(
        $"Account {acc} funds insufficient");
    }

    account.Balance -= amt;
    await _ctxt.SaveChangesAsync();
  }

  public async Task CreditAsync(string acc, decimal amt)
  {
    var account = await GetAccountAsync(acc);
    account.Balance += amt;
    await _ctxt.SaveChangesAsync();
  }

  // public async Task TransferAsync(
  //   string from, string to, decimal amt)
  // {
  //   using var tx = await 
  //     _ctxt.Database.BeginTransactionAsync();
    
  //   try
  //   {
  //     await CreditAsync(to, amt);
  //     await DebitAsync(from, amt);
  //     await tx.CommitAsync();
  //   }
  //   catch
  //   {
  //     await tx.RollbackAsync();
  //     _ctxt.ChangeTracker.Clear();
  //     throw;
  //   }
  // }

  public Task TransferAsync(
    string from, string to, decimal amt)
    => _ctxt.ExecuteRetryableTransactionAsync(async() => {
      await CreditAsync(to, amt);
      await DebitAsync(from, amt);
    });

  private async Task<Account> GetAccountAsync(string acc)
  {
    var account = await _ctxt.Accounts.FindAsync(acc);
    return account ?? 
      throw new ArgumentException(
        "No account found: " + acc,
        nameof(acc));
  }
}
