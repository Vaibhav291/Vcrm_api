using Vcrm.Models;

namespace Vcrm.Services;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetCustomersAsync();

    Task<Customer?> GetCustomerAsync(int id);

    Task<Customer> CreateCustomerAsync(Customer customer);

    Task<bool> UpdateCustomerAsync(int id, Customer customer);

    Task<bool> DeleteCustomerAsync(int id);
}
