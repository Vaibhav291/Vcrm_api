using Microsoft.AspNetCore.Diagnostics;
using Vcrm.Models.Logging;
using Vcrm.Services.Logging;

namespace Vcrm.ExceptionHandling;

// Reports unhandled exceptions to Vcrm_Messaging's centralized log, then lets ASP.NET Core's
// default ProblemDetails handling still produce the response - returning false here means
// "I didn't fully handle this", so it falls through to the normal error response pipeline.
public class CentralLoggingExceptionHandler : IExceptionHandler
{
    private readonly ICentralLogPublisher _publisher;

    public CentralLoggingExceptionHandler(ICentralLogPublisher publisher)
    {
        _publisher = publisher;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(new CentralLogEntry
        {
            Severity = CentralLogSeverity.Error,
            Service = "Vcrm-api",
            Message = exception.Message,
            Exception = exception.ToString(),
            CorrelationId = httpContext.TraceIdentifier,
        }, cancellationToken);

        return false;
    }
}
