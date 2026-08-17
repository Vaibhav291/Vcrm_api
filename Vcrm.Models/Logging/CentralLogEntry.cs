namespace Vcrm.Models.Logging;

// Wire contract for Vcrm_Messaging's POST /vcrmmessaging/Logs endpoint.
public class CentralLogEntry
{
    public CentralLogSeverity Severity { get; set; }

    public string Service { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string? Exception { get; set; }

    public string? CorrelationId { get; set; }

    public Dictionary<string, string>? Properties { get; set; }
}
