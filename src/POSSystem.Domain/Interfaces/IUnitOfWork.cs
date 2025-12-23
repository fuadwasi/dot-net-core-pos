namespace POSSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICustomerRepository Customers { get; }
    ISaleRepository Sales { get; }
    Task<int> SaveChangesAsync();
}
