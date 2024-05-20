namespace ShoppingCart.Data;

using EventFeed;

public class EventStore : IEventStore
{
    private static readonly Database _database = new ();

    public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)=>
        _database
        .Where(e =>
            e.SequenceNumber >= firstEventSequenceNumber &&
            e.SequenceNumber <= lastEventSequenceNumber)
        .OrderBy(e => e.SequenceNumber);

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


internal class Database:List<Event>
{

    public long NextSequenceNumber() => this.Count == 0? 1:this.Max(x => x.SequenceNumber) + 1;
}
