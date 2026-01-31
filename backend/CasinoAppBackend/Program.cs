using System.Text;
using CasinoAppBackend.Configurations;
using CasinoAppBackend.Core.Filters;
using CasinoAppBackend.Data;
using CasinoAppBackend.Data.Seeders;
using CasinoAppBackend.Extensions;
using CasinoAppBackend.Middleware;
using CasinoAppBackend.Repositories;
using CasinoAppBackend.Services.FileStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace CasinoAppBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration));

            var connString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<CasinoAppBackendDbContext>(options => options.UseSqlServer(connString));

            builder.Services.AddSingleton<IFileStorageService, AzureBlobStorageService>();

            builder.Services.AddRepositories();
            builder.Services.AddApplicationServices();
            builder.Services.AddBackgroundServices();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MapperConfig>());

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("Jwt"));

            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.IncludeErrorDetails = builder.Environment.IsDevelopment();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings!.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    b => b.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());

                options.AddPolicy("ReactClient",
                    b => b.WithOrigins("http://localhost:3000")
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Casino App API",
                    Version = "v1"
                });

                options.SupportNonNullableReferenceTypes();
                options.OperationFilter<AuthorizeOperationFilter>();

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {your token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });
            });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<SanitizeInputFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            var app = builder.Build();

            var staticFileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "wwwroot")
);

            var contentTypeProvider = new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings[".wasm"] = "application/wasm";
            contentTypeProvider.Mappings[".data"] = "application/octet-stream";
            contentTypeProvider.Mappings[".framework.js"] = "application/javascript";

            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<CasinoAppBackendDbContext>().SeedAll();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = staticFileProvider,
                ContentTypeProvider = contentTypeProvider,
                ServeUnknownFileTypes = true
            });

            app.UseSerilogRequestLogging(options =>
                options.MessageTemplate = "Handled {RequestPath} in {Elapsed:0.0000} ms");

            app.UseCors("AllowAll");

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
