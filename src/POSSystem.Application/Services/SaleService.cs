using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;

namespace POSSystem.Application.Services;

public class SaleService
{
    private readonly IUnitOfWork _unitOfWork;

    public SaleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Sale>> GetAllSalesAsync()
    {
        return await _unitOfWork.Sales.GetAllAsync();
    }

    public async Task<Sale?> GetSaleByIdAsync(int id)
    {
        return await _unitOfWork.Sales.GetSaleWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int customerId)
    {
        return await _unitOfWork.Sales.GetSalesByCustomerAsync(customerId);
    }

    public async Task<Sale> CreateSaleAsync(Sale sale)
    {
        // Calculate totals
        sale.SubTotal = sale.SaleItems.Sum(item => item.TotalPrice);
        sale.TotalAmount = sale.SubTotal - sale.DiscountAmount + sale.TaxAmount;
        sale.SaleDate = DateTime.UtcNow;

        var result = await _unitOfWork.Sales.AddAsync(sale);

        // Update product stock
        foreach (var item in sale.SaleItems)
        {
            await _unitOfWork.Products.UpdateStockAsync(item.ProductId, -item.Quantity);
        }

        // Update customer total purchases if customer exists
        if (sale.CustomerId.HasValue)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(sale.CustomerId.Value);
            if (customer != null)
            {
                customer.TotalPurchases += sale.TotalAmount;
                await _unitOfWork.Customers.UpdateAsync(customer);
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.Sales.GetTotalSalesAmountAsync(startDate, endDate);
    }
}
