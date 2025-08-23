namespace ECommerce.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "Created"; 
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReservedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
