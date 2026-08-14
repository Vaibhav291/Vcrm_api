using Microsoft.EntityFrameworkCore;
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
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Dev/Qa/Stage run against local databases with no deployment pipeline, so apply
// migrations automatically. Production migrations are applied as a controlled
// release step instead, not automatically on app startup.
if (!AppSettings.IsProduction)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<VcrmDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
// OpenAPI/Swagger is exposed on every environment except Production.
if (!AppSettings.IsProduction)
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
