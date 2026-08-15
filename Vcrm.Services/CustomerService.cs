using Vcrm.Models;
using Vcrm.Repositories;

namespace Vcrm.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<Customer?> GetCustomerAsync(int id)
    {
        return _repository.GetByIdAsync(id);
    }

    public Task<Customer> CreateCustomerAsync(Customer customer)
    {
        customer.CustomerId = 0;
        customer.CreatedAt = DateTime.UtcNow;
        customer.UpdatedAt = customer.CreatedAt;

        return _repository.InsertAsync(customer);
    }

    public Task<bool> UpdateCustomerAsync(int id, Customer customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;

        return _repository.UpdateAsync(customer);
    }

    public Task<bool> DeleteCustomerAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }
}
