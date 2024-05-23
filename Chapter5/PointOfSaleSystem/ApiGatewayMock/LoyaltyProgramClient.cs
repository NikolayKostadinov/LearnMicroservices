namespace ApiGatewayMock;

using System.Text;
using System.Text.Json;
using Models;

public class LoyaltyProgramClient
{
    private readonly HttpClient _httpClient;

    public LoyaltyProgramClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> RegisterUser(string name)
    {
        var user = new { name, Settings = new { } };
        return await _httpClient.PostAsync(RequestUri(), CreateBody(user));
    }

    private static StringContent CreateBody(object user) =>
        new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

    public async Task<HttpResponseMessage> QueryUser(string arg) =>
        await _httpClient.GetAsync(RequestUri(int.Parse(arg)));

    public async Task<HttpResponseMessage> UpdateUser(LoyaltyProgramUser user) =>
        await _httpClient.PutAsync(RequestUri(user.Id), CreateBody(user));

    private static string RequestUri(int? id = null) => id is null ? $"/users/" : $"/users/{id}";
}
