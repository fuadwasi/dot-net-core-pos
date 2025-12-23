using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet.Where(c => c.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        return await _dbSet
            .Where(c => c.IsActive && 
                   (c.Name.Contains(searchTerm) || 
                    (c.Email != null && c.Email.Contains(searchTerm)) ||
                    (c.Phone != null && c.Phone.Contains(searchTerm))))
            .ToListAsync();
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);
    }
}
