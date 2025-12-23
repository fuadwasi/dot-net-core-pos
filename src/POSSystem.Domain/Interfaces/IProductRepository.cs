using POSSystem.Domain.Entities;

namespace POSSystem.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<bool> UpdateStockAsync(int productId, int quantity);
}
