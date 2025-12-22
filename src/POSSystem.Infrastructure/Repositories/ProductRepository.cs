using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _dbSet.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _dbSet
            .Where(p => p.IsActive && 
                   (p.Name.Contains(searchTerm) || 
                    (p.Description != null && p.Description.Contains(searchTerm)) ||
                    (p.Barcode != null && p.Barcode.Contains(searchTerm))))
            .ToListAsync();
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Barcode == barcode);
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        var product = await GetByIdAsync(productId);
        if (product == null)
            return false;

        product.StockQuantity += quantity;
        product.ModifiedDate = DateTime.UtcNow;
        return true;
    }
}
