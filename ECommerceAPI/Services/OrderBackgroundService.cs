using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ECommerce.Data.Data;
using Microsoft.AspNetCore.SignalR;
using ECommerceAPI.Hubs;

namespace ECommerceAPI.Services;

public class OrderBackgroundService : BackgroundService
{
    private readonly ILogger<OrderBackgroundService> _logger;
    private readonly IServiceProvider _services;
    private readonly IHubContext<OrderHub> _hub;

    public OrderBackgroundService(ILogger<OrderBackgroundService> logger, IServiceProvider services, IHubContext<OrderHub> hub)
    {
        _logger = logger;
        _services = services;
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                _logger.LogInformation("OrderBackgroundService: checking pending orders...");

                var pending = db.Orders.Where(o => o.Status == "Pending").ToList();
                foreach (var o in pending)
                {
                    // Send a small heartbeat notification for demo
                    await _hub.Clients.All.SendAsync("OrderBackgroundCheck", new { orderId = o.Id, status = o.Status }, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
