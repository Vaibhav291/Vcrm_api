using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Vcrm.Models.Logging;

namespace Vcrm.Services.Logging;

// Best-effort by design: centralized logging must never break the request it's reporting on,
// so failures are caught and logged locally rather than propagated to the caller.
public class HttpCentralLogPublisher : ICentralLogPublisher
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpCentralLogPublisher> _logger;

    public HttpCentralLogPublisher(HttpClient httpClient, ILogger<HttpCentralLogPublisher> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task PublishAsync(CentralLogEntry entry, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("vcrmmessaging/Logs", entry, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Central logging service responded with {StatusCode} for event in service {Service}",
                    response.StatusCode, entry.Service);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish log entry to central logging service");
        }
    }
}
