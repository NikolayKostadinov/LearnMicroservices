using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

Console.WriteLine("--> Chron job Started!");
var start = await GetStartIdFromDatastore();
var end = 100;
var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
var url = $"http://special-offers:5002/events?start={start}&end={end}";
Console.WriteLine($"--> Get from {url}");
using var resp = await client.GetAsync(new Uri(url));
await ProcessEvents(await resp.Content.ReadAsStreamAsync());
await SaveStartIdToDataStore(start);

// fake implementation. Should get from a real database
Task<long> GetStartIdFromDatastore() => Task.FromResult(0L);

// fake implementation. Should apply business rules to events
async Task ProcessEvents(Stream content)
{
    var events = await JsonSerializer.DeserializeAsync<SpecialOfferEvent[]>(content) ?? new SpecialOfferEvent[0];
    foreach (var @event in events)
    {
        Console.WriteLine(@event);
        start = Math.Max(start, @event.SequenceNumber + 1);
    }
}

Task SaveStartIdToDataStore(long startId) => Task.CompletedTask;

public record SpecialOfferEvent(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);
