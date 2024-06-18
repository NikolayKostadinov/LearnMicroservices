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
    public async Task<Event[]> Get([FromQuery] long start, [FromQuery] long end = long.MaxValue)
        => (await _eventStore.GetEvents(start, end)).ToArray();
}
