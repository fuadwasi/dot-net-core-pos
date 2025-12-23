using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly POSDbContext _context;
    private IProductRepository? _products;
    private ICustomerRepository? _customers;
    private ISaleRepository? _sales;

    public UnitOfWork(POSDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products
    {
        get { return _products ??= new ProductRepository(_context); }
    }

    public ICustomerRepository Customers
    {
        get { return _customers ??= new CustomerRepository(_context); }
    }

    public ISaleRepository Sales
    {
        get { return _sales ??= new SaleRepository(_context); }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
