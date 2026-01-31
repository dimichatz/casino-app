using CasinoAppBackend.Core.Enums;
using CasinoAppBackend.Security;

namespace CasinoAppBackend.Data.Seeders
{
    public static class SuperAdminSeeder
    {
        public static void SeedSuperAdmin(this CasinoAppBackendDbContext context)
        {
            string superAdminUsername = "superadmin";

            if (!context.Users.Any(u => u.Username == superAdminUsername))
            {
                var superAdmin = new User
                {
                    Username = superAdminUsername,
                    Password = EncryptionUtil.Encrypt("Admin123!"),
                    Email = "superadmin@casinoapp.gr",
                    PhoneNumber = "2101234567",
                    Firstname = "SuperAdminFirstname",
                    Lastname = "SuperAdminLastName",
                    UserRole = UserRole.SuperAdmin
                };

                context.Users.Add(superAdmin);
                context.SaveChanges();
            }
        }
    }
}
