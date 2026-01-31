using HighLowGameApi.Data;

namespace HighLowGameApi.Repositories
{
    public class GameSessionRepository : BaseRepository<GameSession>, IGameSessionRepository
    {
        public GameSessionRepository(HighLowGameApiDbContext context) 
            : base(context)
        {
        }
    }
}
