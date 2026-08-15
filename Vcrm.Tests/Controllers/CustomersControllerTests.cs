using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Vcrm.Controllers;
using Vcrm.Models;
using Vcrm.Services;

namespace Vcrm.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<ICustomerService> _serviceMock = new();
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _controller = new CustomersController(_serviceMock.Object, NullLogger<CustomersController>.Instance);
    }

    [Fact]
    public async Task GetCustomers_ReturnsOkWithCustomers()
    {
        var customers = new List<Customer> { new() { CustomerId = 1, FirstName = "Ada" } };
        _serviceMock.Setup(s => s.GetCustomersAsync()).ReturnsAsync(customers);

        var result = await _controller.GetCustomers();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(customers, okResult.Value);
    }

    [Fact]
    public async Task GetCustomers_Returns500_WhenServiceThrows()
    {
        _serviceMock.Setup(s => s.GetCustomersAsync()).ThrowsAsync(new InvalidOperationException("boom"));

        var result = await _controller.GetCustomers();

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task GetCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        _serviceMock.Setup(s => s.GetCustomerAsync(1)).ReturnsAsync((Customer?)null);

        var result = await _controller.GetCustomer(1);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetCustomer_ReturnsCustomer_WhenFound()
    {
        var customer = new Customer { CustomerId = 1, FirstName = "Ada" };
        _serviceMock.Setup(s => s.GetCustomerAsync(1)).ReturnsAsync(customer);

        var result = await _controller.GetCustomer(1);

        Assert.Same(customer, result.Value);
    }

    [Fact]
    public async Task CreateCustomer_ReturnsCreatedAtAction_WithCreatedCustomer()
    {
        var input = new Customer { FirstName = "Ada", LastName = "Lovelace", Email = "ada@example.com" };
        var created = new Customer { CustomerId = 42, FirstName = "Ada", LastName = "Lovelace", Email = "ada@example.com" };
        _serviceMock.Setup(s => s.CreateCustomerAsync(input)).ReturnsAsync(created);

        var result = await _controller.CreateCustomer(input);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(CustomersController.GetCustomer), createdResult.ActionName);
        Assert.Equal(42, createdResult.RouteValues!["id"]);
        Assert.Same(created, createdResult.Value);
    }

    [Fact]
    public async Task CreateCustomer_Returns500_WhenServiceThrows()
    {
        var input = new Customer();
        _serviceMock.Setup(s => s.CreateCustomerAsync(input)).ThrowsAsync(new InvalidOperationException("boom"));

        var result = await _controller.CreateCustomer(input);

        var statusResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, statusResult.StatusCode);
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsBadRequest_WhenRouteIdDoesNotMatchBody()
    {
        var customer = new Customer { CustomerId = 2 };

        var result = await _controller.UpdateCustomer(1, customer);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsNoContent_WhenUpdated()
    {
        var customer = new Customer { CustomerId = 1 };
        _serviceMock.Setup(s => s.UpdateCustomerAsync(1, customer)).ReturnsAsync(true);

        var result = await _controller.UpdateCustomer(1, customer);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        var customer = new Customer { CustomerId = 1 };
        _serviceMock.Setup(s => s.UpdateCustomerAsync(1, customer)).ReturnsAsync(false);

        var result = await _controller.UpdateCustomer(1, customer);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsNoContent_WhenDeleted()
    {
        _serviceMock.Setup(s => s.DeleteCustomerAsync(1)).ReturnsAsync(true);

        var result = await _controller.DeleteCustomer(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        _serviceMock.Setup(s => s.DeleteCustomerAsync(1)).ReturnsAsync(false);

        var result = await _controller.DeleteCustomer(1);

        Assert.IsType<NotFoundResult>(result);
    }
}
