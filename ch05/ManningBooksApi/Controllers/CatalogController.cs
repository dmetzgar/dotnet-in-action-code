using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManningBooksApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
  private readonly CatalogContext _dbContext;

  public CatalogController(
    CatalogContext dbContext) 
  {
    _dbContext = dbContext;
  }

  [HttpGet]
  public IAsyncEnumerable<Book> GetBooks(
      string? titleFilter = null)
  {
    IQueryable<Book> query = _dbContext.Books
      .Include(b => b.Ratings);
    if (titleFilter != null) 
    {
      query = query.Where(b => 
        b.Title.ToLower().Contains(titleFilter.ToLower()));
    }

    return query.AsAsyncEnumerable();
  }

  [HttpGet("{id}")]
  public Task<Book?> GetBook(int id,
    CancellationToken cancellationToken)
  {
    return _dbContext.Books.FirstOrDefaultAsync(
      b => b.Id == id,
      cancellationToken);
  }

  [HttpPost]
  public async Task<Book> CreateBook(
    BookCreateCommand command,
    CancellationToken cancellationToken)
  {
    var book = new Book(
      command.Title,
      command.Description
    );

    var entity = _dbContext.Books.Add(book);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return entity.Entity;
  }

  [HttpPatch("{id}")]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> UpdateBook(
    int id, BookUpdateCommand command,
    CancellationToken cancellationToken)
  {
    var book = await _dbContext.FindAsync<Book>(
      new object?[] { id },
      cancellationToken);
    if (book == null)
    {
      return NotFound();
    }

    if (command.Title != null) 
    {
      book.Title = command.Title;
    }

    if (command.Description != null)
    {
      book.Description = command.Description;
    }

    await _dbContext.SaveChangesAsync(cancellationToken);
    return NoContent();
  }

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  public async Task<IActionResult> DeleteBook(int id,
    CancellationToken cancellationToken) 
  {
    var book = await _dbContext.Books
      .Include(b => b.Ratings)
      .FirstOrDefaultAsync(b => b.Id == id,
        cancellationToken);
    if (book == null)
    {
      return NotFound();
    }

    _dbContext.Remove(book);
    var rows = await _dbContext.SaveChangesAsync(
      cancellationToken);
    Console.WriteLine("Rows deleted: " + rows);
    return NoContent();
  }

  public record BookCreateCommand(string Title, string? Description) {}
  public record BookUpdateCommand(string? Title, string? Description) {}
}
