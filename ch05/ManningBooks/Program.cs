using ManningBooks;
using Microsoft.Data.Sqlite;

using var keepAliveConnection = new SqliteConnection(
  CatalogContext.ConnectionString);
keepAliveConnection.Open();

CatalogContext.SeedBooks();

var userRequests = new[] {
 ".NET in Action",
 "Grokking Simplicity",
 "API Design Patterns",
 "EF Core in Action",
};
var tasks = new List<Task>();
foreach (var userRequest in userRequests)
{
  CatalogContext.WriteBookToConsole(userRequest);
}

/*

using var dbContext = new CatalogContext();
dbContext.Database.EnsureCreated();

dbContext.Add(new Book(".NET in Action"));
dbContext.Add(new Book("API Design Patterns"));
dbContext.Add(new Book("Grokking Simplicity"));
dbContext.Add(new Book("The Programmer's Brain"));
var efBook = new Book("EF Core in Action");
efBook.Ratings.Add(new Rating { Comment = "Great!" });
efBook.Ratings.Add(new Rating { Stars = 4 });
efBook.Ratings.Add(new Rating { Stars = 10 });
dbContext.Add(efBook);

dbContext.SaveChanges();

foreach (var book in dbContext.Books.OrderByDescending(b => b.Id))
{
  Console.WriteLine($"Book \"{book.Title}\" has id {book.Id}");
}

var efBook = new Book("EF Core in Action");
efBook.Ratings.Add(new Rating { Comment = "Great!" });
efBook.Ratings.Add(new Rating { Stars = 4 });
efBook.Ratings.Add(new Rating { Stars = 10 });
dbContext.Add(efBook);
dbContext.SaveChanges();

var efRatings = (from b in dbContext.Books
                where b.Title == "EF Core in Action"
                select b.Ratings).FirstOrDefault();
efRatings?.ForEach(r => 
  Console.WriteLine(
    $"{r.Stars} stars: {r.Comment ?? "-blank-"}"));

Console.WriteLine($"{ efBook.Title + $", Id={efBook.Id}"}");

foreach (var book in dbContext.Books
           .Include(b => b.Ratings))
{
  Console.WriteLine($"Book \"{book.Title}\" has id {book.Id}");
  book.Ratings.ForEach(r => 
    Console.WriteLine(
    $"\t{r.Stars} stars: {r.Comment ?? "-blank-"}"));
}
*/
