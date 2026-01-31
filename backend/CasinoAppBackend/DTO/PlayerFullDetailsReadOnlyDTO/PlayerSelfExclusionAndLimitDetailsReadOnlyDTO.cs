using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.DTO.PlayerFullDetailsReadOnlyDTO
{
    public class PlayerSelfExclusionAndLimitDetailsReadOnlyDTO
    {
        public bool? IsSelfExcluded { get; set; }
        public DateTime? SelfExclusionStart { get; set; }
        public DateTime? SelfExclusionEnd { get; set; }
        public SelfExclusionPeriod? SelfExclusionPeriod { get; set; }
        public decimal? DepositDailyLimit { get; set; }
        public decimal? PendingDepositDailyLimit { get; set; }
        public DateTimeOffset? PendingDepositDailyLimitStart { get; set; }
        public decimal? DepositWeeklyLimit { get; set; }
        public decimal? PendingDepositWeeklyLimit { get; set; }
        public DateTimeOffset? PendingDepositWeeklyLimitStart { get; set; }
        public decimal? DepositMonthlyLimit { get; set; }
        public decimal? PendingDepositMonthlyLimit { get; set; }
        public DateTimeOffset? PendingDepositMonthlyLimitStart { get; set; }
        public decimal? LossDailyLimit { get; set; }
        public decimal? PendingLossDailyLimit { get; set; }
        public DateTimeOffset? PendingLossDailyLimitStart { get; set; }
        public decimal? LossWeeklyLimit { get; set; }
        public decimal? PendingLossWeeklyLimit { get; set; }
        public DateTimeOffset? PendingLossWeeklyLimitStart { get; set; }
        public decimal? LossMonthlyLimit { get; set; }
        public decimal? PendingLossMonthlyLimit { get; set; }
        public DateTimeOffset? PendingLossMonthlyLimitStart { get; set; }

        
    }
}
