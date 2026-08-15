using Microsoft.AspNetCore.Mvc;
using Vcrm.Models;
using Vcrm.Services;

namespace Vcrm.Controllers;

[ApiController]
[Route("vcrm/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _service;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService service, ILogger<CustomersController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        try
        {
            return Ok(await _service.GetCustomersAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving customers.");
            return StatusCode(500, "Internal server error");
        }
    }

    // GET: api/customers/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _service.GetCustomerAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        return customer;
    }

    // POST: api/customers
    [HttpPost]
    public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
    {
        try
        {
            var created = await _service.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomer), new { id = created.CustomerId }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a customer.");
            return StatusCode(500, "Internal server error");
        }
    }

    // PUT: api/customers/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
    {
        if (id != customer.CustomerId)
        {
            return BadRequest();
        }

        var updated = await _service.UpdateCustomerAsync(id, customer);
        return updated ? NoContent() : NotFound();
    }

    // DELETE: api/customers/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var deleted = await _service.DeleteCustomerAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
