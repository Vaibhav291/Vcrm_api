using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Vcrm;
using Vcrm.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        $"Connection string 'DefaultConnection' is not configured for environment '{AppSettings.EnvironmentName}'. " +
        "For Production, set it via Azure App Service Configuration / Key Vault, not in appsettings.json.");
}

builder.Services.AddDbContext<VcrmDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure())
        .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

var app = builder.Build();

// Migrations are applied by a dedicated CI/CD pipeline, not on app startup.

// Configure the HTTP request pipeline.
// OpenAPI/Swagger is exposed on every environment except Production.
if (!AppSettings.IsProduction)
{
    app.MapOpenApi("/vcrm/openapi/{documentName}.json");
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/vcrm/openapi/v1.json", "Vcrm API v1");
        options.RoutePrefix = "vcrm/swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
