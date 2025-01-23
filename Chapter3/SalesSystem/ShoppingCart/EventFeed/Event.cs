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
