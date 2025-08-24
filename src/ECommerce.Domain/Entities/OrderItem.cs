namespace ECommerce.Domain.Entities;
 
/// <summary>
/// Future use: Order items can be persisted here if product-level tracking is needed
/// </summary>
public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string ProductId { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
