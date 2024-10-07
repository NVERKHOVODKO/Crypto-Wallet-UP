namespace UP.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class ServiceController(IServiceService serviceService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var users = await serviceService.GetServices();
        return Ok(users);
    }
}