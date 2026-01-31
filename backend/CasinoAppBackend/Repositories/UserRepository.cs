using CasinoAppBackend.Data;
using CasinoAppBackend.Security;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<User?> GetUserAsync(string usernameOrEmail, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Username == usernameOrEmail
                   || x.Email == usernameOrEmail);
            if (user == null)
            {
                return null;
            }
            if (!EncryptionUtil.IsValidPassword(password, user.Password))
            {
                return null;
            }
            return user;
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await context.Users.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<User?> GetByEmailAsync(string email) =>
            await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByPhoneNumberAsync(string phoneNumber) =>
            await context.Users.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);

        public async Task<bool> IsUserActiveAsync(Guid id)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user != null && user.IsActive;
        }
    }
}
