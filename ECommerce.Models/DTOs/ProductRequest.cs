namespace ECommerce.Models.DTOs;

public class ProductRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? Category { get; set; }

    // Nullable: when null, don't modify inventory; when set, use value (including 0)
    public int? Quantity { get; set; }
}
