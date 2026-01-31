using CasinoAppBackend.Core.Enums;

namespace CasinoAppBackend.Data.Seeders
{
    public static class AppSettingsSeeder
    {
        public static void SeedSettings(this CasinoAppBackendDbContext context)
        {
            var defaultSettings = new[]
            {
                new AppSetting
                {
                    Key = "DefaultSignupBonus",
                    Value = "10"
                },
                new AppSetting
                {
                    Key = "DepositDailyLimit",
                    Value = "200"
                },
                new AppSetting
                {
                    Key = "DepositWeeklyLimit",
                    Value = "500"
                },
                new AppSetting
                {
                    Key = "DepositMonthlyLimit",
                    Value = "2000"
                },
                new AppSetting
                {
                    Key = "LossDailyLimit",
                    Value = "100"
                },
                new AppSetting
                {
                    Key = "LossWeeklyLimit",
                    Value = "400"
                },
                new AppSetting
                {
                    Key = "LossMonthlyLimit",
                    Value = "1500"
                },
                new AppSetting
                {
                    Key = "LimitIncreaseDelayDays",
                    Value = "7"
                },
                new AppSetting
                {
                    Key = "SystemCurrency",
                    Value = "EUR"
                },
                new AppSetting
                {
                    Key = "MinDepositAmount",
                    Value = "5"
                },
                new AppSetting
                {
                     Key = "MaxDepositAmount",
                    Value = "10000"
                }
            };

            foreach (var setting in defaultSettings)
            {
                if (!context.AppSettings.Any(s => s.Key == setting.Key))
                {
                    context.AppSettings.Add(setting);
                }
            }

            context.SaveChanges();
        }
    }
}