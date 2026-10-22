using Microsoft.AspNetCore.Mvc;

namespace OpenEduSyncShare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : Controller
{
    [HttpGet]
    public IActionResult GetHealthCheck()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}