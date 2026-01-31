using CasinoAppBackend.Data;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Repositories
{
    public interface IGameRepository
    {
        Task<PaginatedResult<Game>> GetPaginatedGamesFilteredAsync(int pageNumber, int pageSize,
            string? search);
        Task<Game?> GetByCodeAsync(string code);
    }
}
