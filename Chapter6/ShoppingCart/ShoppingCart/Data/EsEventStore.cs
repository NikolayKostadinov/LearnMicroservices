﻿namespace ShoppingCart.Data;

using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EventFeed;
using global::EventStore.ClientAPI;

public class EsEventStore : IEventStore
{
    private const string ConnectionString =
        "tcp://admin:changeit@localhost:1113";

    public async Task Raise(string eventName, object content)
    {
        var connectionSettings = ConnectionSettings.Create().DisableTls().Build();
        using var connection = EventStoreConnection.Create(connectionSettings, new Uri(ConnectionString));

        await connection.ConnectAsync();
        await connection.AppendToStreamAsync(
            "ShoppingCart",
            ExpectedVersion.Any,
            new EventData(
                eventId: Guid.NewGuid(),
                type: "ShoppingCartEvent",
                isJson: true,
                data: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(content)),
                metadata: Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(new EventMetadata(DateTimeOffset.UtcNow, eventName)))));
    }

    public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
    {
        var connectionSettings = ConnectionSettings.Create().DisableTls().Build();
        using var connection = EventStoreConnection.Create(connectionSettings, new Uri(ConnectionString));

        await connection.ConnectAsync();
        var result = await connection.ReadStreamEventsForwardAsync(
            "ShoppingCart",
            start: firstEventSequenceNumber,
            count: (int)(lastEventSequenceNumber - firstEventSequenceNumber),
            resolveLinkTos: false);

        return result.Events
            .Select(e =>
                new
                {
                    Content = Encoding.UTF8.GetString(e.Event.Data),
                    Metadata = JsonSerializer.Deserialize<EventMetadata>(
                        Encoding.UTF8.GetString(e.Event.Metadata),
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })!
                })
            .Select((e, i) =>
                new Event(
                    i + firstEventSequenceNumber,
                    e.Metadata.OccuredAt,
                    e.Metadata.EventName,
                    e.Content));
    }

    public record EventMetadata(DateTimeOffset OccuredAt, string EventName);
}
