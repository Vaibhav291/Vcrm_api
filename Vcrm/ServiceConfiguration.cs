using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vcrm.Data;
using Vcrm.Repositories;
using Vcrm.Services;

namespace Vcrm;

public static class ServiceConfiguration
{
    public const string UiCorsPolicy = "UiCorsPolicy";

    public static IServiceCollection AddVcrmServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        services.AddCors(options =>
        {
            options.AddPolicy(UiCorsPolicy, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string 'DefaultConnection' is not configured for environment '{AppSettings.EnvironmentName}'. " +
                "For Production, set it via Azure App Service Configuration / Key Vault, not in appsettings.json.");
        }

        services.AddDbContext<VcrmDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }
}
