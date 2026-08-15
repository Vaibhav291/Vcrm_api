using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vcrm.Data;
using Vcrm.Models;

namespace Vcrm.Controllers;

[ApiController]
[Route("vcrm/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly VcrmDbContext _context;

    public CustomersController(VcrmDbContext context)
    {
        _context = context;
    }

    // GET: api/customers
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        return await _context.Customers.AsNoTracking().ToListAsync();
    }

    // GET: api/customers/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Customer>> GetCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

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
        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = customer.CreatedAt;

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCustomer), new { id = customer.CustomerId }, customer);
    }

    // PUT: api/customers/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCustomer(int id, Customer customer)
    {
        if (id != customer.CustomerId)
        {
            return BadRequest();
        }

        var createdAt = await _context.Customers
            .Where(c => c.CustomerId == id)
            .Select(c => (DateTime?)c.CreatedAt)
            .FirstOrDefaultAsync();

        if (createdAt is null)
        {
            return NotFound();
        }

        customer.CreatedAt = createdAt.Value;
        customer.UpdatedAt = DateTime.UtcNow;

        _context.Entry(customer).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CustomerExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/customers/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer is null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CustomerExists(int id)
    {
        return await _context.Customers.AnyAsync(c => c.CustomerId == id);
    }
}
