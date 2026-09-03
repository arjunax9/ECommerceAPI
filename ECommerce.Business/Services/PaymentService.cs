using System.Threading.Tasks;

namespace ECommerce.Business.Services;

public interface IPaymentService
{
    Task<bool> ProcessPaymentAsync(decimal amount);
}

public class PaymentService : IPaymentService
{
    public async Task<bool> ProcessPaymentAsync(decimal amount)
    {
        await Task.Delay(100); // simulate processing
        return true;
    }
}
