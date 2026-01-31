using System.Linq.Expressions;
using CasinoAppBackend.Data;
using CasinoAppBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class GameRepository : BaseRepository<Game>, IGameRepository
    {
        public GameRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<PaginatedResult<Game>> GetPaginatedGamesFilteredAsync(int pageNumber, int pageSize,
            string? search)
        {
            int skip = (pageNumber - 1) * pageSize;
            var predicates = new List<Expression<Func<Game, bool>>>();

            IQueryable<Game> query = context.Games;

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(g => EF.Functions.Like(g.Name, $"%{search}%"));

            query = query.Where(g => g.IsEnabled);

            var totalRecords = await query.CountAsync();

            var gamesFiltered = await query
                .OrderByDescending(a => a.InsertedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Game>
            {
                Data = gamesFiltered,
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Game?> GetByCodeAsync(string code) =>
            await context.Games.FirstOrDefaultAsync(g => g.Code == code);
    }
}
