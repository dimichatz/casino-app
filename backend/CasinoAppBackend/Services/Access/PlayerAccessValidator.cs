using CasinoAppBackend.Repositories;

namespace CasinoAppBackend.Services.Access
{
    public class PlayerAccessValidator : IPlayerAccessValidator
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayerAccessValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PlayerAccessResult> ValidateAsync(Guid userId)
        {
            var player = await _unitOfWork.PlayerRepository.GetByUserIdAsync(userId);

            if (player == null)
            {
                return new PlayerAccessResult
                {
                    IsAllowed = false,
                    Failure = PlayerAccessFailure.PlayerNotFound
                };
            }

            if (!player.User.IsActive)
            {
                return new PlayerAccessResult
                {
                    IsAllowed = false,
                    Failure = PlayerAccessFailure.AccountInactive
                };
            }

            if (!player.IsKycVerified)
            {
                return new PlayerAccessResult
                {
                    IsAllowed = false,
                    Failure = PlayerAccessFailure.KycPending
                };
            }

            if (player.IsSelfExcluded)
            {
                return new PlayerAccessResult
                {
                    IsAllowed = false,
                    Failure = PlayerAccessFailure.SelfExcluded
                };
            }

            return new PlayerAccessResult
            {
                IsAllowed = true,
                Player = player,
                Failure = PlayerAccessFailure.None
            };
        }
    }
}
