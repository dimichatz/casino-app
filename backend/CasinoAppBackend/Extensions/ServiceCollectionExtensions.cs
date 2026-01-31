using CasinoAppBackend.BackgroundServices;
using CasinoAppBackend.Services;
using CasinoAppBackend.Services.Access;
using CasinoAppBackend.Services.Games.HighLow;

namespace CasinoAppBackend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IPlayerService,  PlayerService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IGameService, GameService>();

            services.AddScoped<IPlayerAccessValidator, PlayerAccessValidator>();

            services.AddScoped<IHighLowGameService, HighLowGameService>();
            services.AddHttpClient<IHighLowGameIntegrationService, HighLowGameIntegrationService>((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["ExternalServices:HighLowGameApiBaseUrl"]!;

                client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }

        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<PlayerLimitActivationService>();
            services.AddHostedService<PlayerSelfExclusionExpirationService>();

            return services;
        }
    }
}
