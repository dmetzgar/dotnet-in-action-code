using System.Text;

namespace CustomerBatchImporter.UnitTests;

public class CsvImporterTests
{
    private readonly CsvImporter _csvImporter;
    private readonly ICustomerRepository _fakeCustomerRepo;

    public CsvImporterTests()
    {
        _fakeCustomerRepo = A.Fake<ICustomerRepository>();
        _csvImporter = new(_fakeCustomerRepo);
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
        // Arrange
        string email = "some@email.com";
        string name = "A Customer";
        string license = "Basic";
        string csvLine = string.Join(',', email, name, license);
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(email))
            .Returns(default(Customer));

        // Act
        var stream = GetStreamFromString(csvLine);
        await _csvImporter.ReadAsync(stream);

        // Assert
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(email)).MustHaveHappened();
        A.CallTo(() => _fakeCustomerRepo.CreateAsync(A<NewCustomerDto>.That.Matches(n =>
            n.Email == email
            && n.Name == name
            && n.License == license)))
            .MustHaveHappened();
    }

    [Fact]
    public async Task InvalidLine()
    {
        var stream = GetStreamFromString("not a valid line");
        await _csvImporter.ReadAsync(stream);

        var calls = Fake.GetCalls(_fakeCustomerRepo);
        Assert.Empty(calls);
    }

    [Fact]
    public async Task ThreeLinesOneInvalid()
    {
        string email1 = "some@email.com";
        string email2 = "another@email.com";
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(A<string>.Ignored))
            .Returns(default(Customer));

        var stream = GetStreamFromString($"{email1},customer1,None\ninvalidline\n{email2},customer2,None");
        await _csvImporter.ReadAsync(stream);
        
        A.CallTo(() => _fakeCustomerRepo.CreateAsync(A<NewCustomerDto>.Ignored)).MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task GetThrows()
    {
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync("")).Throws<ArgumentException>();

        var stream = GetStreamFromString(",name,license");
        await Assert.ThrowsAsync<ArgumentException>(() => _csvImporter.ReadAsync(stream));
    }

    [Fact]
    public async Task UpdateExisting()
    {
        // Arrange
        string email = "some@email.com";
        var existing = new Customer() {
            Id = 1,
            Email = email,
            Name = "A Customer",
            License = "Basic"
        };
        A.CallTo(() => _fakeCustomerRepo.GetByEmailAsync(email)).Returns(existing);

        // Act
        var stream = GetStreamFromString($"{email},customer1,None");
        await _csvImporter.ReadAsync(stream);

        // Assert
        A.CallTo(() => _fakeCustomerRepo.UpdateAsync(A<UpdateCustomerDto>.That.Matches(u =>
            u.Id == 1
            && u.NewName == "customer1"
            && u.NewLicense == "None")))
            .MustHaveHappened();
    }

    private Stream GetStreamFromString(string content)
        => new MemoryStream(Encoding.UTF8.GetBytes(content));
}