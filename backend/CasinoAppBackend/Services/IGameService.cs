using CasinoAppBackend.DTO;
using CasinoAppBackend.Models;

namespace CasinoAppBackend.Services
{
    public interface IGameService
    {
        Task<PaginatedResult<GameReadOnlyDTO>> GetPaginatedGamesFilteredAsync(int pageNumber, int pageSize,
            string? search);
    }
}
