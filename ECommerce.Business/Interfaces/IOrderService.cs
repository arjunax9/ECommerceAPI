using ECommerce.Models.DTOs;
using ECommerce.Models.Entities;

namespace ECommerce.Business.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);

    Task<Order?> GetByIdAsync(int id);

    Task CheckoutAsync(int id);

    Task<bool> ProcessPaymentAsync(int id);
}
