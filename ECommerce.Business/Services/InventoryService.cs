using ECommerce.Business.Interfaces;
using ECommerce.Data.Repositories;
using ECommerce.Models.Entities;

namespace ECommerce.Business.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;

    public InventoryService(IInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> GetQuantityAsync(int productId)
    {
        var inv = await _repository.GetByProductIdAsync(productId);
        return inv?.Quantity ?? 0;
    }

    public async Task UpdateQuantityAsync(int productId, int quantity)
    {
        var inv = await _repository.GetByProductIdAsync(productId);
        if (inv == null)
        {
            inv = new Inventory { ProductId = productId, Quantity = quantity };
            await _repository.AddAsync(inv);
        }
        else
        {
            inv.Quantity = quantity;
            await _repository.UpdateAsync(inv);
        }
    }
}
