using RO.DevTest.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public string? UserId { get; set; }
    public User? User { get; set; }

    public List<SaleItem> Items { get; set; } = new();
}
