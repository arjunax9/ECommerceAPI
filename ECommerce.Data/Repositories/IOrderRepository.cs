using ECommerce.Models.Entities;

namespace ECommerce.Data.Repositories;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);

    Task<Order?> GetByIdAsync(int id);

    Task UpdateAsync(Order order);

    Task<List<Order>> GetByUserIdAsync(int userId);

    Task<List<Order>> GetAllAsync();
}
