using Vcrm;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddVcrmServices();

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

// Skip HTTPS redirection in Development so the local Vite dev server (http)
// can call the API without needing the .NET dev cert trusted in the browser.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(ServiceConfiguration.UiCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
