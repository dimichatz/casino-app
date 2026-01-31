using AutoMapper;
using CasinoAppBackend.DTO;
using CasinoAppBackend.Models;
using CasinoAppBackend.Repositories;

namespace CasinoAppBackend.Services
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a paginated list of games, optionally filtered by a search term.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of records to include in each page.</param>
        /// <param name="search"> An optional search term used to filter games by name or other searchable fields.
        /// </param>
        /// <returns>
        /// A <see cref="PaginatedResult{GameReadOnlyDTO}"/> containing the filtered and paginated list of games.
        /// </returns>
        public async Task<PaginatedResult<GameReadOnlyDTO>> GetPaginatedGamesFilteredAsync(int pageNumber, int pageSize, string? search)
        {
            var paginatedGames = await _unitOfWork.GameRepository
               .GetPaginatedGamesFilteredAsync(pageNumber, pageSize, search);

            var gameDtos = _mapper.Map<List<GameReadOnlyDTO>>(paginatedGames.Data);

            return new PaginatedResult<GameReadOnlyDTO>
            {
                Data = gameDtos,
                TotalRecords = paginatedGames.TotalRecords,
                PageNumber = paginatedGames.PageNumber,
                PageSize = paginatedGames.PageSize
            };
        }
    }
}
