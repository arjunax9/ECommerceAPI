using ECommerce.Models.DTOs;
using ECommerce.Models.Entities;

namespace ECommerce.Business.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);

    Task<Order?> GetByIdAsync(int id);

    Task<List<ECommerce.Models.Entities.Order>> GetByUserIdAsync(int userId);

    Task<List<ECommerce.Models.Entities.Order>> GetAllAsync();

    Task CheckoutAsync(int id);

    Task<bool> ProcessPaymentAsync(int id);

    Task<Order> ApproveOrderAsync(int id);
}
