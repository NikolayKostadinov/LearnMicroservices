namespace ShoppingCart.EventFeed;

public record Event(
    long SequenceNumber,
    DateTimeOffset OccuredAt,
    string Name,
    object Content);

public interface IEventStore
{
    IEnumerable<Event> GetEvents(
        long firstEventSequenceNumber, long lastEventSequenceNumber);

    void Raise(string eventName, object content);
}

public class EventStore : IEventStore
{
// omitted in this chapter
    public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
    {
        _database
            .Where(e =>
                e.SequenceNumber >= firstEventSequenceNumber &&
                e.SequenceNumber <= lastEventSequenceNumber)
            .OrderBy(e => e.SequenceNumber);
    }

    public void Raise(string eventName, object content)
    {
        var seqNumber = _database.NextSequenceNumber();
        _database.Add(
            new Event(seqNumber,
                DateTimeOffset.UtcNow,
                eventName,
                content));
    }

}
