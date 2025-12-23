namespace POSSystem.Domain.Entities;

public class Sale : BaseEntity
{
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string Status { get; set; } = "Completed";
    public string? Notes { get; set; }

    // Navigation properties
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
