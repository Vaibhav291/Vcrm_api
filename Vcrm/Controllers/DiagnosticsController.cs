using Microsoft.AspNetCore.Mvc;

namespace Vcrm.Controllers;

// Temporary - exists only to verify the CentralLoggingExceptionHandler wiring end-to-end.
// Remove once confirmed working.
[ApiController]
[Route("vcrm/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    [HttpGet("throw")]
    public IActionResult Throw()
    {
        if (AppSettings.IsProduction)
        {
            return NotFound();
        }

        throw new InvalidOperationException("Deliberate test exception to verify central logging integration.");
    }
}
