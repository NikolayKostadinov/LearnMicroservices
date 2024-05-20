namespace ShoppingCart.Controllers;

using Data;
using EventFeed;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/events")]
public class EventFeedController : ControllerBase
{
    private readonly IEventStore _eventStore;

    public EventFeedController(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    [HttpGet("")]
    public Event[] Get([FromQuery] long start, [FromQuery] long end = long.MaxValue)
        =>
            _eventStore
                .GetEvents(start, end)
                .ToArray();
}
