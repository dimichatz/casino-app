using CasinoAppBackend.Data;
using CasinoAppBackend.DTO;

namespace CasinoAppBackend.Services
{
    public interface IUserService
    {
        Task<User> VerifyAndGetUserAsync(UserLoginDTO credentials);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        string CreateUserToken(User user);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    }
}
