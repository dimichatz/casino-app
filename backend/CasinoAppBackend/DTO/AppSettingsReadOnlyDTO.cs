
namespace CasinoAppBackend.DTO
{
    public class AppSettingsReadOnlyDTO
    {
        public decimal DefaultSignupBonus { get; set; }
        public int DepositDailyLimit { get; set; }
        public int DepositWeeklyLimit { get; set; }
        public int DepositMonthlyLimit { get; set; }
        public int LossDailyLimit { get; set; }
        public int LossWeeklyLimit { get; set; }
        public int LossMonthlyLimit { get; set; }
        public int LimitIncreaseDelayDays { get; set; }
    }
}
