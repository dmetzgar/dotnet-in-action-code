using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MoneyTransfer;

public static class Extensions
{
  public static Task ExecuteRetryableTransactionAsync(
    this DbContext ctxt,
    Func<Task> action)
  {
    IExecutionStrategy strategy =
      ctxt.Database.CreateExecutionStrategy();
    return strategy.ExecuteAsync(async () => {
      using IDbContextTransaction tx =
        await ctxt.Database.BeginTransactionAsync();
      try
      {
        await action();
        await tx.CommitAsync();
      }
      catch
      {
        await tx.RollbackAsync();
        ctxt.ChangeTracker.Clear();
        throw;
      }
    });
  }

  public static Task ExecuteCompensatingTransactionAsync(
    this DbContext ctxt,
    Func<Task> action,
    Func<Task<bool>> didItWork)
  {
    IExecutionStrategy strategy =
      ctxt.Database.CreateExecutionStrategy();
    return strategy.ExecuteInTransactionAsync(
      action, didItWork);
  }
}
