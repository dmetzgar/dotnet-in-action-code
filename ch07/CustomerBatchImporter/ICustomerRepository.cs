namespace CustomerBatchImporter;

public interface ICustomerRepository
{
  Task CreateAsync(NewCustomerDto customer);

  Task UpdateAsync(UpdateCustomerDto customer);

  Task<Customer?> GetByEmailAsync(string email);
}

public record NewCustomerDto(
  string Email,
  string Name,
  string License
) { }

public record UpdateCustomerDto(
  int Id,
  string? NewName,
  string? NewLicense
) { }

public class Customer
{
  public int Id { get; set; }
  public string Email { get; set; }
  public string Name { get; set; }
  public string License { get; set; }
}
