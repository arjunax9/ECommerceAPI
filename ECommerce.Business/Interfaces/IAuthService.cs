using ECommerce.Models.Entities;

namespace ECommerce.Business.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(User user);

    Task<string?> LoginAsync(string email, string password);
}
