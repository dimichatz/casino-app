using System.ComponentModel.DataAnnotations;
using CasinoAppBackend.Attributes;

namespace CasinoAppBackend.DTO
{
    public class AppSettingsUpdateDTO
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        [NonNegativeDecimal]
        public decimal DefaultSignupBonus { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int DepositDailyLimit { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int DepositWeeklyLimit { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int DepositMonthlyLimit { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int LossDailyLimit { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int LossWeeklyLimit { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int LossMonthlyLimit { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [GreaterThanZeroInteger]
        public int LimitIncreaseDelayDays { get; set; }
    }
}
