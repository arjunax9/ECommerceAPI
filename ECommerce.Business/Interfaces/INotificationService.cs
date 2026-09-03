namespace ECommerce.Business.Interfaces;

public interface INotificationService
{
    Task NotifyOrderUpdatedAsync(int orderId, string status);
}
