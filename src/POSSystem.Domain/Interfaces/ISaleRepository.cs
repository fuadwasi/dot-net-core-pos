using POSSystem.Domain.Entities;

namespace POSSystem.Domain.Interfaces;

public interface ISaleRepository : IRepository<Sale>
{
    Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int customerId);
    Task<Sale?> GetSaleWithDetailsAsync(int saleId);
    Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate);
}
