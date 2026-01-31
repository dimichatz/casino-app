using CasinoAppBackend.Repositories;

namespace CasinoAppBackend.BackgroundServices
{
    public class PlayerSelfExclusionExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PlayerSelfExclusionExpirationService> _logger;

        public PlayerSelfExclusionExpirationService(IServiceProvider serviceProvider,
            ILogger<PlayerSelfExclusionExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PlayerSelfExclusionExpirationService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiredSelfExclusionsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing expired self-exclusions.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("PlayerSelfExclusionExpirationService stopped.");
        }

        private async Task ProcessExpiredSelfExclusionsAsync(CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var now = DateTime.UtcNow;

            var players = await unitOfWork.PlayerRepository
                .GetAllPlayersWithExpiredSelfExclusionAsync(now);

            if (!players.Any())
            {
                _logger.LogInformation("No expired self-exclusions found.");
                return;
            }

            foreach (var player in players)
            {
                try
                {
                    player.IsSelfExcluded = false;
                    player.SelfExclusionStart = null;
                    player.SelfExclusionEnd = null;
                    player.SelfExclusionPeriod = null;

                    player.User.IsActive = true;

                    _logger.LogInformation(
                        "Self-exclusion expired and player reactivated for Player {PlayerId}.",
                        player.Id
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error processing expired self-exclusion for Player {PlayerId}.",
                        player.Id);
                }
            }

            await unitOfWork.SaveAsync();
        }
    }
}
