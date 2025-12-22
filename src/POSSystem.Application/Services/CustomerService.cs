using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;

namespace POSSystem.Application.Services;

public class CustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
    {
        return await _unitOfWork.Customers.GetAllAsync();
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync()
    {
        return await _unitOfWork.Customers.GetActiveCustomersAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int id)
    {
        return await _unitOfWork.Customers.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        return await _unitOfWork.Customers.SearchCustomersAsync(searchTerm);
    }

    public async Task<Customer> CreateCustomerAsync(Customer customer)
    {
        var result = await _unitOfWork.Customers.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<Customer> UpdateCustomerAsync(Customer customer)
    {
        var result = await _unitOfWork.Customers.UpdateAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var result = await _unitOfWork.Customers.DeleteAsync(id);
        if (result)
            await _unitOfWork.SaveChangesAsync();
        return result;
    }
}
