using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CasinoAppBackend.Core.Filters
{
    /// <summary>
    /// Adds JWT Bearer security requirements to Swagger operations whose
    /// controller or action is decorated with <see cref="AuthorizeAttribute"/>.
    /// </summary>
    public class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var methodAuth = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>();

            var classAuth = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>();

            var allAuth = methodAuth.Concat(classAuth ?? Enumerable.Empty<AuthorizeAttribute>())
                                    .Distinct();

            if (allAuth.Any())
            {
                operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>();

                var roles = allAuth
                    .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
                    .SelectMany(a => a.Roles!.Split(','))
                    .Select(r => r.Trim());

                operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Description = "Add token to header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                roles.ToList()
            }
        });
            }
        }
    }
}
