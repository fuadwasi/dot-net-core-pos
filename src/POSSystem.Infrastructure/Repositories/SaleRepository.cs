using Microsoft.EntityFrameworkCore;
using POSSystem.Domain.Entities;
using POSSystem.Domain.Interfaces;
using POSSystem.Infrastructure.Data;

namespace POSSystem.Infrastructure.Repositories;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .Include(s => s.Customer)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<Sale?> GetSaleWithDetailsAsync(int saleId)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
            .ThenInclude(si => si.Product)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == saleId);
    }

    public async Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate && s.Status == "Completed")
            .SumAsync(s => s.TotalAmount);
    }
}
