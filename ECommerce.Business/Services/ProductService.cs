using ECommerce.Business.Interfaces;
using ECommerce.Data.Repositories;
using ECommerce.Models.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.Business.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache _cache;
    private readonly Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions _cacheOptions =
        new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5)
        };

    public ProductService(IProductRepository repository, Microsoft.Extensions.Caching.Memory.IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        const string key = "products_all";
        if (_cache.TryGetValue(key, out var cachedObj) && cachedObj is List<Product> cached)
        {
            return cached;
        }

        var products = await _repository.GetAllAsync();
        _cache.Set(key, products, _cacheOptions);

        return products;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        var key = $"product_{id}";
        if (_cache.TryGetValue(key, out var cachedObj2) && cachedObj2 is Product cached2)
        {
            return cached2;
        }

        var product = await _repository.GetByIdAsync(id);
        if (product != null)
        {
            _cache.Set(key, product, _cacheOptions);
        }

        return product;
    }

    public async Task<Product> AddAsync(Product product)
    {
        var created = await _repository.AddAsync(product);
        _cache.Remove("products_all");
        return created;
    }

    public async Task UpdateAsync(Product product)
    {
        await _repository.UpdateAsync(product);
        _cache.Remove("products_all");
        _cache.Remove($"product_{product.Id}");
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product != null)
        {
            await _repository.DeleteAsync(product);
            _cache.Remove("products_all");
            _cache.Remove($"product_{product.Id}");
        }
    }
}
