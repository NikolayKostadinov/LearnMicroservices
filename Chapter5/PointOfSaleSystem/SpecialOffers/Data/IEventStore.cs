namespace SpecialOffers.Data;

using Model;

public interface IEventStore
{
    void RaiseEvent(string name, object content);
    IEnumerable<EventFeedEvent> GetEvents(int start, int end);
}
