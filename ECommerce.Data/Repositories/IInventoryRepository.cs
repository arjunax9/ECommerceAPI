using ECommerce.Models.Entities;

namespace ECommerce.Data.Repositories;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(int productId);

    Task UpdateAsync(Inventory inventory);

    Task<Inventory> AddAsync(Inventory inventory);
}
