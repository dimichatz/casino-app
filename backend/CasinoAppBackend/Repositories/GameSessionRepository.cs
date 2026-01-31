using CasinoAppBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace CasinoAppBackend.Repositories
{
    public class GameSessionRepository : BaseRepository<GameSession>, IGameSessionRepository
    {
        public GameSessionRepository(CasinoAppBackendDbContext context) 
            : base(context)
        {
        }

        public async Task<GameSession?> GetByExternalSessionIdAsync(Guid externalSessionId) =>
             await context.GameSessions.FirstOrDefaultAsync(s => s.ExternalSessionId == externalSessionId);
    }
}
