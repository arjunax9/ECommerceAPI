using System.Collections.Generic;

namespace ECommerce.Models.DTOs;

public class CreateOrderRequest
{
    public int UserId { get; set; }

    public List<OrderItemRequest> Items { get; set; } = new();
}
