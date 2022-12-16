namespace CustomerBatchImporter;

public class CsvImporter
{
  private readonly ICustomerRepository _customerRepo;

  public CsvImporter( ICustomerRepository customerRepo)
    => _customerRepo = customerRepo;

  public async Task ReadAsync(Stream stream)
  {
    var reader = new StreamReader(stream);
    string? line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
      var customer = ReadCsvLine(line);
      if (customer == null)
      {
        continue;
      }

      var existing = await
        _customerRepo.GetByEmailAsync(customer.Email);
      
      if (existing == null)
      {
        await _customerRepo.CreateAsync(customer);
      }
      else
      {
        await _customerRepo.UpdateAsync(
          new UpdateCustomerDto(
            existing.Id,
            customer.Name,
            customer.License
          ));
      }
    }
  }

  private NewCustomerDto? ReadCsvLine(string line)
  {
    var el = line.Split(',');
    return el.Length != 3 ? null
      : new NewCustomerDto(el[0], el[1], el[2]);
  }
}
