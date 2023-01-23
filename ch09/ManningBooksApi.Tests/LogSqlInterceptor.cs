using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit.Abstractions;

namespace ManningBooksApi.Tests;

public class LogSqlInterceptor : DbCommandInterceptor
{
  private readonly ITestOutputHelper _testOutput;

  public LogSqlInterceptor(ITestOutputHelper testOutput)
    => _testOutput = testOutput;

  public override 
    ValueTask<InterceptionResult<DbDataReader>>
    ReaderExecutingAsync(
      DbCommand command,
      CommandEventData eventData,
      InterceptionResult<DbDataReader> result,
      CancellationToken cancelToken)
  {
    _testOutput.WriteLine(command.CommandText);
    return base.ReaderExecutingAsync(
      command, eventData, result, cancelToken);
  }
}
