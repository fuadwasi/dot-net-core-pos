using POSSystem.Domain.Entities;

namespace POSSystem.Domain.Interfaces;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> GetActiveCustomersAsync();
    Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByPhoneAsync(string phone);
}
