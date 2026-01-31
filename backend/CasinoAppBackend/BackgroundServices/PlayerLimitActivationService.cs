using CasinoAppBackend.Data;
using CasinoAppBackend.Repositories;

namespace CasinoAppBackend.BackgroundServices
{
    public class PlayerLimitActivationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PlayerLimitActivationService> _logger;

        public PlayerLimitActivationService(IServiceProvider serviceProvider,
            ILogger<PlayerLimitActivationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PlayerLimitActivationService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessPendingLimitsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing pending player limits.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }

            _logger.LogInformation("PlayerLimitActivationService stopped.");
        }

        private async Task ProcessPendingLimitsAsync(CancellationToken token)
        {
            using var scope = _serviceProvider.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var playerLimitRepository = unitOfWork.PlayerLimitRepository;

            var now = DateTime.UtcNow;

            // Get all players that have any pending limit
            var pendingLimits = await playerLimitRepository.GetAllWithPendingLimitsAsync();

            foreach (var limit in pendingLimits)
            {
                bool updated = false;

                // deposit daily
                if (limit.PendingDepositDailyLimit.HasValue &&
                    limit.PendingDepositDailyLimitStart.HasValue &&
                    now >= limit.PendingDepositDailyLimitStart.Value)
                {
                    var oldValue = limit.DepositDailyLimit;
                    var newValue = limit.PendingDepositDailyLimit.Value;
                    limit.DepositDailyLimit = newValue;
                    limit.PendingDepositDailyLimit = null;
                    limit.PendingDepositDailyLimitStart = null;

                    await CreatePlayerLimitAuditAsync(unitOfWork, limit.PlayerId, "DepositDailyLimit", oldValue, newValue);

                    updated = true;
                    

                    _logger.LogInformation("Activated DepositDailyLimit for Player {PlayerId}.", limit.PlayerId);
                }

                // deposit weekly
                if (limit.PendingDepositWeeklyLimit.HasValue &&
                    limit.PendingDepositWeeklyLimitStart.HasValue &&
                    now >= limit.PendingDepositWeeklyLimitStart.Value)
                {
                    var oldValue = limit.DepositWeeklyLimit;
                    var newValue = limit.PendingDepositWeeklyLimit.Value;
                    limit.DepositWeeklyLimit = newValue;
                    limit.PendingDepositWeeklyLimit = null;
                    limit.PendingDepositWeeklyLimitStart = null;

                    await CreatePlayerLimitAuditAsync(unitOfWork, limit.PlayerId, "DepositWeeklyLimit", oldValue, newValue);

                    updated = true;

                    _logger.LogInformation("Activated DepositWeeklyLimit for Player {PlayerId}.", limit.PlayerId);
                }

                // deposit monthly
                if (limit.PendingDepositMonthlyLimit.HasValue &&
                    limit.PendingDepositMonthlyLimitStart.HasValue &&
                    now >= limit.PendingDepositMonthlyLimitStart.Value)
                {
                    var oldValue = limit.DepositMonthlyLimit;
                    var newValue = limit.PendingDepositMonthlyLimit.Value;
                    limit.DepositMonthlyLimit = newValue;
                    limit.PendingDepositMonthlyLimit = null;
                    limit.PendingDepositMonthlyLimitStart = null;

                    await CreatePlayerLimitAuditAsync(unitOfWork, limit.PlayerId, "DepositMonthlyLimit", oldValue, newValue);

                    updated = true;

                    _logger.LogInformation("Activated DepositMonthlyLimit for Player {PlayerId}.", limit.PlayerId);
                }

                // loss daily
                if (limit.PendingLossDailyLimit.HasValue &&
                    limit.PendingLossDailyLimitStart.HasValue &&
                    now >= limit.PendingLossDailyLimitStart.Value)
                {
                    var oldValue = limit.LossDailyLimit;
                    var newValue = limit.PendingLossDailyLimit.Value;
                    limit.LossDailyLimit = newValue;
                    limit.PendingLossDailyLimit = null;
                    limit.PendingLossDailyLimitStart = null;

                    await CreatePlayerLimitAuditAsync(unitOfWork, limit.PlayerId, "LossDailyLimit", oldValue, newValue);

                    updated = true;

                    _logger.LogInformation("Activated LossDailyLimit for Player {PlayerId}.", limit.PlayerId);
                }

                // loss weekly
                if (limit.PendingLossWeeklyLimit.HasValue &&
                    limit.PendingLossWeeklyLimitStart.HasValue &&
                    now >= limit.PendingLossWeeklyLimitStart.Value)
                {
                    var oldValue = limit.LossWeeklyLimit;
                    var newValue = limit.PendingLossWeeklyLimit.Value;
                    limit.LossWeeklyLimit = newValue;
                    limit.PendingLossWeeklyLimit = null;
                    limit.PendingLossWeeklyLimitStart = null;

                    await CreatePlayerLimitAuditAsync(unitOfWork, limit.PlayerId, "LossWeeklyLimit", oldValue, newValue);

                    updated = true;

                    _logger.LogInformation("Activated LossWeeklyLimit for Player {PlayerId}.", limit.PlayerId);
                }

                // loss monthly
                if (limit.PendingLossMonthlyLimit.HasValue &&
                    limit.PendingLossMonthlyLimitStart.HasValue &&
                    now >= limit.PendingLossMonthlyLimitStart.Value)
                {
                    var oldValue = limit.LossMonthlyLimit;
                    var newValue = limit.PendingLossMonthlyLimit.Value;
                    limit.LossMonthlyLimit = newValue;
                    limit.PendingLossMonthlyLimit = null;
                    limit.PendingLossMonthlyLimitStart = null;

                    await CreatePlayerLimitAuditAsync(unitOfWork, limit.PlayerId, "LossMonthlyLimit", oldValue, newValue);

                    updated = true;

                    _logger.LogInformation("Activated LossMonthlyLimit for Player {PlayerId}.", limit.PlayerId);
                }

                if (updated)
                {
                    await unitOfWork.SaveAsync();
                }
            }
        }

        private async Task CreatePlayerLimitAuditAsync(IUnitOfWork unitOfWork,
            Guid playerId, string fieldName, decimal oldLimit, decimal newLimit)
        {
            var audit = new PlayerLimitAudit
            {
                PlayerId = playerId,
                FieldName = fieldName,
                OldLimit = oldLimit,
                NewLimit = newLimit
            };

            await unitOfWork.PlayerLimitAuditRepository.AddAsync(audit);
            await unitOfWork.SaveAsync();
        }
    }
}
