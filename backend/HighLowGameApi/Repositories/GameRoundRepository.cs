using HighLowGameApi.Data;

namespace HighLowGameApi.Repositories
{
    public class GameRoundRepository : BaseRepository<GameRound>, IGameRoundRepository
    {
        public GameRoundRepository(HighLowGameApiDbContext context) 
            : base(context)
        {
        }
    }
}
