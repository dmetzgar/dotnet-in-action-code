using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ManningBooksAsync;

public class DelayInterceptor : DbCommandInterceptor
{
  public override async 
    ValueTask<InterceptionResult<DbDataReader>> 
    ReaderExecutingAsync(
      DbCommand command, 
      CommandEventData eventData, 
      InterceptionResult<DbDataReader> result, 
      CancellationToken cancellationToken = default)
  {
    var cmdText = command.CommandText.Trim();
    if (!cmdText.StartsWith("INSERT")) 
    {
      Console.WriteLine(
        $"Delaying command {command.CommandText.Trim()}");
      await Task.Delay(TimeSpan.FromSeconds(1));
    }

    return await base.ReaderExecutingAsync(command, eventData, 
      result, cancellationToken);
  }
}