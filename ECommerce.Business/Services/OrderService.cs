using ECommerce.Business.Interfaces;
using ECommerce.Data.Repositories;
using ECommerce.Models.DTOs;
using ECommerce.Models.Entities;

namespace ECommerce.Business.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IPaymentService _paymentService;
    private readonly ECommerce.Business.Interfaces.INotificationService? _notificationService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        IPaymentService paymentService,
        ECommerce.Business.Interfaces.INotificationService? notificationService = null)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _paymentService = paymentService;
        _notificationService = notificationService;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        var order = new Order
        {
            UserId = request.UserId,
            Status = "Pending",
            OrderItems = new List<OrderItem>()
        };

        decimal total = 0m;

        foreach (var it in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(it.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product {it.ProductId} not found");

            var line = product.Price * it.Quantity;
            total += line;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = it.ProductId,
                Quantity = it.Quantity,
                Price = product.Price
            });

            // reduce inventory if exists
            var inv = await _inventoryRepository.GetByProductIdAsync(it.ProductId);
            if (inv != null)
            {
                inv.Quantity = Math.Max(0, inv.Quantity - it.Quantity);
                await _inventoryRepository.UpdateAsync(inv);
            }
        }

        order.TotalAmount = total;

        var created = await _orderRepository.AddAsync(order);
        // notify (if available)
        if (_notificationService != null)
        {
            await _notificationService.NotifyOrderUpdatedAsync(created.Id, created.Status);
        }

        return created;
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _orderRepository.GetByIdAsync(id);
    }

    public async Task CheckoutAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new InvalidOperationException("Order not found");

        order.Status = "CheckedOut";
        await _orderRepository.UpdateAsync(order);
        if (_notificationService != null)
        {
            await _notificationService.NotifyOrderUpdatedAsync(order.Id, order.Status);
        }
    }

    public async Task<bool> ProcessPaymentAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null) throw new InvalidOperationException("Order not found");

        var success = await _paymentService.ProcessPaymentAsync(order.TotalAmount);
        if (success)
        {
            order.Status = "Paid";
            await _orderRepository.UpdateAsync(order);
            if (_notificationService != null)
            {
                await _notificationService.NotifyOrderUpdatedAsync(order.Id, order.Status);
            }
        }

        return success;
    }
}
