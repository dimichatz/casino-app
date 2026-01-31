
using HighLowGameApi.Data;
using HighLowGameApi.Helpers;
using HighLowGameApi.Middleware;
using HighLowGameApi.Repositories;
using HighLowGameApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HighLowGameApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<HighLowGameApiDbContext>(options => options.UseSqlServer(connString));

            builder.Services.AddSingleton<ICardGenerator, CardGenerator>();

            builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
            builder.Services.AddScoped<IGameRoundRepository, GameRoundRepository>();

            builder.Services.AddScoped<IHighLowGameService, HighLowGameService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
