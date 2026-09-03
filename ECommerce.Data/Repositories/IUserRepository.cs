using ECommerce.Models.Entities;

namespace ECommerce.Data.Repositories;

public interface IUserRepository
{
    Task<User> AddAsync(User user);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);
}
