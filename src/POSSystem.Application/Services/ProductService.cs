using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;

namespace POSSystem.Application.Services;

public class ProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _unitOfWork.Products.GetAllAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _unitOfWork.Products.GetActiveProductsAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _unitOfWork.Products.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _unitOfWork.Products.SearchProductsAsync(searchTerm);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        var result = await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        var result = await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var result = await _unitOfWork.Products.DeleteAsync(id);
        if (result)
            await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantityChange)
    {
        var result = await _unitOfWork.Products.UpdateStockAsync(productId, quantityChange);
        if (result)
            await _unitOfWork.SaveChangesAsync();
        return result;
    }
}
