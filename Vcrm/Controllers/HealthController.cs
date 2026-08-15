using Microsoft.AspNetCore.Mvc;

namespace Vcrm.Controllers;

[ApiController]
[Route("Vcrm")]
public class HealthController : ControllerBase
{
    private const string TimeFormat = "yyyy-MM-dd hh:mm:ss tt";

    private static readonly DateTime DeploymentTimeUtc =
        System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Message = "Vcrm is up and Running",
            DeploymentTimeUtc = DeploymentTimeUtc.ToString(TimeFormat),
            CurrentTimeUtc = DateTime.UtcNow.ToString(TimeFormat)
        });
    }
}
