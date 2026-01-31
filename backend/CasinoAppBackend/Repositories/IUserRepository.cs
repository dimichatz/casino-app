using CasinoAppBackend.Data;

namespace CasinoAppBackend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(string usernameOrEmail, string password);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneNumberAsync(string phoneNumber);
        Task<bool> IsUserActiveAsync(Guid id);
    }
}
