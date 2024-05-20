using Microsoft.AspNetCore.Mvc;

namespace HelloMicroservices.Controllers;

[ApiController]
[Route("/")]
public class CurrentDateTimeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index() => Ok(DateTime.UtcNow);

}
