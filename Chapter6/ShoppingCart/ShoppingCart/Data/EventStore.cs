namespace ShoppingCart.Data;

using System.Data.SqlClient;
using System.Text.Json;
using Dapper;
using EventFeed;

public class EventStore : IEventStore
{
    private readonly string _connectionString = @"Data Source=localhost;Initial Catalog=ShoppingCart;User Id=SA; Password=P@ssw0rd";

    private const string writeEventSql =
        @"INSERT INTO EventStore(Name, OccurredAt, Content) 
          VALUES (@Name, @OccurredAt, @Content)";
    public async Task Raise(string eventName, object content)
    {
        var jsonContent = JsonSerializer.Serialize(content);
        await using var conn = new SqlConnection(_connectionString);
        await conn.ExecuteAsync(
            writeEventSql,
            new
            {
                Name = eventName,
                OccurredAt = DateTimeOffset.Now,
                Content = jsonContent
            });
    }

    private const string readEventsSql =
        @"SELECT * 
          FROM EventStore
          WHERE ID >= @Start AND ID <= @End";

    public async Task<IEnumerable<Event>> GetEvents(
        long firstEventSequenceNumber,
        long lastEventSequenceNumber)
    {
        await using var conn = new SqlConnection(_connectionString);
        return await conn.QueryAsync<Event>(
            readEventsSql,
            new
            {
                Start = firstEventSequenceNumber,
                End = lastEventSequenceNumber
            });
    }
}
