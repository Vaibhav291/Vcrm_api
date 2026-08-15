using Moq;
using Vcrm.Models;
using Vcrm.Repositories;
using Vcrm.Services;

namespace Vcrm.Tests.Services;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _repositoryMock = new();
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _service = new CustomerService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetCustomersAsync_ReturnsCustomersFromRepository()
    {
        var customers = new List<Customer> { new() { CustomerId = 1 } };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        var result = await _service.GetCustomersAsync();

        Assert.Same(customers, result);
    }

    [Fact]
    public async Task GetCustomerAsync_ReturnsCustomerFromRepository()
    {
        var customer = new Customer { CustomerId = 1 };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

        var result = await _service.GetCustomerAsync(1);

        Assert.Same(customer, result);
    }

    [Fact]
    public async Task GetCustomerAsync_ReturnsNull_WhenCustomerDoesNotExist()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _service.GetCustomerAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCustomerAsync_ResetsIdAndStampsTimestamps_BeforeInserting()
    {
        var input = new Customer { CustomerId = 99, FirstName = "Ada", LastName = "Lovelace", Email = "ada@example.com" };
        _repositoryMock
            .Setup(r => r.InsertAsync(It.IsAny<Customer>()))
            .ReturnsAsync((Customer c) => c);

        var before = DateTime.UtcNow;
        var result = await _service.CreateCustomerAsync(input);
        var after = DateTime.UtcNow;

        Assert.Equal(0, result.CustomerId);
        Assert.InRange(result.CreatedAt, before, after);
        Assert.Equal(result.CreatedAt, result.UpdatedAt);
        _repositoryMock.Verify(r => r.InsertAsync(input), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_StampsUpdatedAt_BeforeUpdating()
    {
        var customer = new Customer { CustomerId = 1, UpdatedAt = DateTime.UtcNow.AddDays(-1) };
        _repositoryMock.Setup(r => r.UpdateAsync(customer)).ReturnsAsync(true);

        var before = DateTime.UtcNow;
        var result = await _service.UpdateCustomerAsync(1, customer);
        var after = DateTime.UtcNow;

        Assert.True(result);
        Assert.InRange(customer.UpdatedAt, before, after);
        _repositoryMock.Verify(r => r.UpdateAsync(customer), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ReturnsFalse_WhenRepositoryReportsNoMatch()
    {
        var customer = new Customer { CustomerId = 1 };
        _repositoryMock.Setup(r => r.UpdateAsync(customer)).ReturnsAsync(false);

        var result = await _service.UpdateCustomerAsync(1, customer);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCustomerAsync_DelegatesToRepository()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteCustomerAsync(1);

        Assert.True(result);
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAsync_ReturnsFalse_WhenCustomerDoesNotExist()
    {
        _repositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(false);

        var result = await _service.DeleteCustomerAsync(1);

        Assert.False(result);
    }
}
