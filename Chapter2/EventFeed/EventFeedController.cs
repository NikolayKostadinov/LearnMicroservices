namespace ShoppingCart.EventFeed;

using System.Linq;
using Microsoft.AspNetCore.Mvc;

[Route("/events")]
public class EventFeedController : Controller
{
    private readonly IEventStore _eventStore;

    public EventFeedController(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    [HttpGet("")]
    public Event[] Get([FromQuery] long start, [FromQuery] long end = long.MaxValue) =>
            _eventStore
                .GetEvents(start, end)
                .ToArray();
}
