using Microsoft.AspNetCore.Mvc;
using UP.Migrations.Services.Interfaces;

namespace UP.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class ServiceController: ControllerBase
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var users = await _serviceService.GetServices();
        return Ok(users);
    }
}