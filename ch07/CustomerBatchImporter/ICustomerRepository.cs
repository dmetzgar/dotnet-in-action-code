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
