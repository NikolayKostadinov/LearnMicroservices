namespace HelloMicroservices.Controllers;

using Microsoft.AspNetCore.Mvc;

public class CurrentDateTimeController: ControllerBase
{
    [HttpGet("/")]
    public IActionResult GetCurrentDateTime() => new JsonResult(DateTime.UtcNow);
}
