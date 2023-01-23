using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ManningBooks;

public class DelayInterceptor : DbCommandInterceptor
{
  public override InterceptionResult<DbDataReader> 
  ReaderExecuting(
    DbCommand command,
    CommandEventData eventData,
    InterceptionResult<DbDataReader> result)
  {
    Console.WriteLine(
      $"Delaying command {command.CommandText.Trim()}");
    Thread.Sleep(TimeSpan.FromSeconds(1));
    return base.ReaderExecuting(
      command, eventData, result);
  }
}