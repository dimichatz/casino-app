using CasinoAppBackend.Data;

namespace CasinoAppBackend.Services.Access
{
    public class PlayerAccessResult
    {
        public bool IsAllowed { get; init; }
        public Player? Player { get; init; }
        public PlayerAccessFailure Failure { get; init; }
    }

}
