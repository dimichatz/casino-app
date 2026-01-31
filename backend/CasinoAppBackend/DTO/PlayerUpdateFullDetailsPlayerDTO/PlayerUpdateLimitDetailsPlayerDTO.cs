using CasinoAppBackend.Attributes;

namespace CasinoAppBackend.DTO.PlayerUpdateFullDetailsPlayerDTO
{
    public class PlayerUpdateLimitDetailsPlayerDTO
    {
        [GreaterThanZeroInteger]
        public int? DepositDailyLimit { get; set; }

        [GreaterThanZeroInteger]
        public int? DepositWeeklyLimit { get; set; }

        [GreaterThanZeroInteger]
        public int? DepositMonthlyLimit { get; set; }

        [GreaterThanZeroInteger]
        public int? LossDailyLimit { get; set; }

        [GreaterThanZeroInteger]
        public int? LossWeeklyLimit { get; set; }

        [GreaterThanZeroInteger]
        public int? LossMonthlyLimit { get; set; }
    }
}
