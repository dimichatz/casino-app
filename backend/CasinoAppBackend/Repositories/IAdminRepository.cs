using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersAdminsAsync();
        Task<List<Admin>> GetAdminsFilteredAsync(string? search, bool? isActive);
        Task<PaginatedResult<Admin>> GetPaginatedAdminsFilteredAsync(int pageNumber, int pageSize,
            string? search, bool? isActive);
        Task<Admin?> GetByIdAsync(Guid userId);
    }
}
