using ManningBooksAsync;
using Microsoft.Data.Sqlite;

using var keepAliveConnection = new SqliteConnection(
  CatalogContext.ConnectionString);
keepAliveConnection.Open();

await CatalogContext.SeedBooks();

var userRequests = new[] {
 ".NET in Action",
 "Grokking Simplicity",
 "API Design Patterns",
 "EF Core in Action",
};
var tasks = new List<Task>();
foreach (var userRequest in userRequests)
{
  tasks.Add(
    CatalogContext.WriteBookToConsoleAsync(userRequest)
  );
}

//Task.WaitAll(tasks.ToArray());
await Task.WhenAll(tasks.ToArray());
