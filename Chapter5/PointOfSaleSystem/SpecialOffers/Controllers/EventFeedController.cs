namespace SpecialOffers.Controllers;

using Data;
using Microsoft.AspNetCore.Mvc;
using Model;

[Route("/events")]
public class EventFeedController : ControllerBase
{
    private readonly IEventStore eventStore;

    public EventFeedController(IEventStore eventStore)
    {
        this.eventStore = eventStore;
    }

    [HttpGet("")]
    public ActionResult<EventFeedEvent[]> GetEvents([FromQuery] int start, [FromQuery] int end)
    {
        if (start < 0 || end < start)
            return BadRequest();
        var events = eventStore.GetEvents(start, end).ToArray();
        Console.WriteLine($"--> {events.Count()} Events were ret!");
        return events;
    }
}
