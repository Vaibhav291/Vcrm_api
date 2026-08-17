using System.Text;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Vcrm.Data;
using Vcrm.ExceptionHandling;
using Vcrm.Repositories;
using Vcrm.Services;
using Vcrm.Services.Auth;
using Vcrm.Services.Email;
using Vcrm.Services.Logging;

namespace Vcrm;

public static class ServiceConfiguration
{
    public const string UiCorsPolicy = "UiCorsPolicy";

    public static WebApplicationBuilder AddVcrmServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        // In Azure, AZURE_KEY_VAULT_NAME is set as an App Service setting; locally it's absent,
        // so config falls back to user-secrets/appsettings as before.
        var keyVaultName = configuration["AZURE_KEY_VAULT_NAME"];
        if (!string.IsNullOrWhiteSpace(keyVaultName))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential());
        }

        services.AddApplicationInsightsTelemetry();

        // Always registered so unhandled exceptions get a consistent ProblemDetails response;
        // the Vcrm_Messaging publish step below is layered on top only when configured.
        services.AddProblemDetails();

        var centralLoggingBaseUrl = configuration["CentralLogging:BaseUrl"];
        var centralLoggingApiKey = configuration["CentralLogging:ApiKey"];
        if (!string.IsNullOrWhiteSpace(centralLoggingBaseUrl) && !string.IsNullOrWhiteSpace(centralLoggingApiKey))
        {
            services.AddHttpClient<ICentralLogPublisher, HttpCentralLogPublisher>(client =>
            {
                client.BaseAddress = new Uri(centralLoggingBaseUrl);
                client.DefaultRequestHeaders.Add("X-Api-Key", centralLoggingApiKey);
            });

            services.AddExceptionHandler<CentralLoggingExceptionHandler>();
        }

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

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();

        var smtpSection = configuration.GetSection("Smtp");
        services.Configure<SmtpSettings>(smtpSection);
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        if (string.IsNullOrWhiteSpace(smtpSection["Host"]) || string.IsNullOrWhiteSpace(smtpSection["Username"]))
        {
            throw new InvalidOperationException(
                $"Configuration 'Smtp:Host'/'Smtp:Username'/'Smtp:Password' is not fully configured for environment '{AppSettings.EnvironmentName}'. " +
                "Set SMTP settings via user-secrets locally or Azure App Service Configuration / Key Vault in Production.");
        }

        var jwtSection = configuration.GetSection("Jwt");
        services.Configure<JwtSettings>(jwtSection);

        var signingKey = jwtSection["SigningKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                $"Configuration 'Jwt:SigningKey' is not configured for environment '{AppSettings.EnvironmentName}'. " +
                "For Production, set it via Azure App Service Configuration / Key Vault, not in appsettings.json.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ValidateLifetime = true,
                };
            });

        services.AddAuthorization();

        return builder;
    }
}
