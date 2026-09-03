using ECommerce.Models.Entities;

namespace ECommerce.Business.Interfaces;

public interface IInventoryService
{
    Task<int> GetQuantityAsync(int productId);

    Task UpdateQuantityAsync(int productId, int quantity);
}
