using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.SignalR;
using ECommerceAPI.Hubs;

namespace ECommerceAPI.Services;

public class OrderNotificationService : INotificationService
{
    private readonly IHubContext<OrderHub> _hub;

    public OrderNotificationService(IHubContext<OrderHub> hub)
    {
        _hub = hub;
    }

    public async Task NotifyOrderUpdatedAsync(int orderId, string status)
    {
        await _hub.Clients.All.SendAsync("OrderUpdated", new { orderId, status });
    }
}
