using System.Text;

namespace CustomerBatchImporter.UnitTests;

public class CsvImporterTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly CsvImporter _csvImporter;

    public CsvImporterTests()
    {
        _mockCustomerRepo = new();
        _csvImporter = new(_mockCustomerRepo.Object);
    }

    [Fact]
    public async Task OneCustomer()
    {
        string email = "some@email.com";
        string name = "A Customer";
        string license = "Basic";
        string csvLine = string.Join(',', email, name, license);

        var stream = GetStreamFromString(csvLine);
        await _csvImporter.ReadAsync(stream);
    }

    [Fact]
    public async Task ValidCustomerOneLine()
    {
        string email = "some@email.com";
        string name = "A Customer";
        string license = "Basic";
        string csvLine = string.Join(',', email, name, license);
        //_mockCustomerRepo.Setup(m => m.GetByEmailAsync(email)).ReturnsAsync(default(Customer));
        _mockCustomerRepo.Setup(m => m.GetByEmailAsync(email)).Returns(Task.FromResult<Customer?>(null));
        //_mockCustomerRepo.Setup(m => m.CreateAsync(It.IsAny<NewCustomerDto>()));
        _mockCustomerRepo.Setup(m => m.CreateAsync(It.Is<NewCustomerDto>(n => 
            n.Email == email
            && n.Name == name 
            && n.License == license)));

        var stream = GetStreamFromString(csvLine);
        await _csvImporter.ReadAsync(stream);

        _mockCustomerRepo.VerifyAll();
    }

    [Fact]
    public async Task InvalidLine()
    {
        var stream = GetStreamFromString("not a valid line");
        await _csvImporter.ReadAsync(stream);

        _mockCustomerRepo.VerifyAll();
    }

    [Fact]
    public async Task ThreeLinesOneInvalid()
    {
        string email1 = "some@email.com";
        string email2 = "another@email.com";
        int numGetCalls = 0;
        // _mockCustomerRepo.Setup(m => m.GetByEmailAsync(email1)).ReturnsAsync(default(Customer)).Verifiable();
        // _mockCustomerRepo.Setup(m => m.GetByEmailAsync(email2)).ReturnsAsync(default(Customer)).Verifiable();
        _mockCustomerRepo.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(default(Customer)).Callback(() => numGetCalls++).Verifiable();
        // _mockCustomerRepo.Setup(m => m.CreateAsync(It.IsAny<NewCustomerDto>())).Verifiable();

        var stream = GetStreamFromString($"{email1},customer1,None\ninvalidline\n{email2},customer2,None");
        await _csvImporter.ReadAsync(stream);
        
        _mockCustomerRepo.Verify();
        Assert.Equal(2, numGetCalls);
        _mockCustomerRepo.Verify(m => m.CreateAsync(It.IsAny<NewCustomerDto>()), Times.Exactly(2));
        _mockCustomerRepo.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetThrows()
    {
        _mockCustomerRepo.Setup(m => m.GetByEmailAsync(It.IsAny<string>())).ThrowsAsync(new ArgumentException("email cannot be empty"));

        var stream = GetStreamFromString(",name,license");
        await Assert.ThrowsAsync<ArgumentException>(() => _csvImporter.ReadAsync(stream));

        _mockCustomerRepo.VerifyAll();
    }

    [Fact]
    public async Task UpdateExisting()
    {
        string email = "some@email.com";
        var existing = new Customer() {
            Id = 1,
            Email = email,
            Name = "A Customer",
            License = "Basic"
        };
        _mockCustomerRepo.Setup(m => m.GetByEmailAsync(email)).ReturnsAsync(existing);
        _mockCustomerRepo.Setup(m => m.UpdateAsync(It.Is<UpdateCustomerDto>(c => 
            c.Id == 1 
            && c.NewName == "customer1"
            && c.NewLicense == "None")));

        var stream = GetStreamFromString($"{email},customer1,None");
        await _csvImporter.ReadAsync(stream);

        _mockCustomerRepo.VerifyAll();
    }

    private Stream GetStreamFromString(string content)
        => new MemoryStream(Encoding.UTF8.GetBytes(content));
}