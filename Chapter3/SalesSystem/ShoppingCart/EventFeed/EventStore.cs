namespace ShoppingCart.EventFeed;

public class EventStore : IEventStore
{
    private static long currentSequenceNumber = 0;
    private static readonly IList<Event> Database = new List<Event>();

    public IEnumerable<Event> GetEvents(
        long firstEventSequenceNumber,
        long lastEventSequenceNumber)
        => Database
            .Where(e =>
                e.SequenceNumber >= firstEventSequenceNumber &&
                e.SequenceNumber <= lastEventSequenceNumber)
            .OrderBy(e => e.SequenceNumber);

    public void Raise(string eventName, object content)
    {
        var seqNumber = Interlocked.Increment(ref currentSequenceNumber);
        Database.Add(new Event(seqNumber, DateTimeOffset.UtcNow, eventName, content));
    }
}
