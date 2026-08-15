using Microsoft.EntityFrameworkCore;
using Vcrm.Data;
using Vcrm.Models;

namespace Vcrm.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly VcrmDbContext _context;

    public CustomerRepository(VcrmDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .FromSqlRaw("EXEC dbo.GetAllCustomers")
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .FromSqlInterpolated($"EXEC dbo.GetCustomerById {id}")
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<Customer> InsertAsync(Customer customer)
    {
        var inserted = await _context.Customers
            .FromSqlInterpolated($@"EXEC dbo.InsertCustomer
                {customer.FirstName}, {customer.LastName}, {customer.CompanyName}, {customer.Email},
                {customer.Phone}, {customer.Industry}, {customer.Address}, {customer.AssignedToUserId},
                {customer.CreatedAt}, {customer.UpdatedAt}, {customer.IsActive}")
            .AsNoTracking()
            .ToListAsync();

        return inserted.Single();
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var updated = await _context.Customers
            .FromSqlInterpolated($@"EXEC dbo.UpdateCustomer
                {customer.CustomerId}, {customer.FirstName}, {customer.LastName}, {customer.CompanyName}, {customer.Email},
                {customer.Phone}, {customer.Industry}, {customer.Address}, {customer.AssignedToUserId},
                {customer.UpdatedAt}, {customer.IsActive}")
            .AsNoTracking()
            .ToListAsync();

        return updated.Count > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await _context.Customers
            .FromSqlInterpolated($"EXEC dbo.DeleteCustomer {id}")
            .AsNoTracking()
            .ToListAsync();

        return deleted.Count > 0;
    }
}
