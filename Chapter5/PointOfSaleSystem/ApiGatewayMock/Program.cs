﻿namespace ApiGatewayMock;

using System.Text.Json;
using Models;
using static Console;
using static Environment;

public class Program
{
    private static string host = "https://localhost:5001";
    private static LoyaltyProgramClient? client;

    public static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            host = args[0];
        }

        client = new LoyaltyProgramClient(new HttpClient { BaseAddress = new Uri(host) });

        var processCommand =
            new Dictionary<char, (string description, Func<string, Task<(bool, HttpResponseMessage)>> handler)>
            {
                {
                    'r',
                    ("r <user name> - to register a user with name <user name> with the Loyalty Program Microservice.",
                        async c => (true, await client.RegisterUser(c.Substring(1))))
                },
                {
                    'q',
                    ("q <userid> - to query the Loyalty Program Microservice for a user with id <userid>.",
                        async c => (true, await client.QueryUser(c.Substring(1))))
                },
                {
                    'u',
                    ("u <userid> <interests> - to update a user with new interests", HandleUpdateInterestsCommand)
                },
                {
                    'x',
                    ("x - to exit",
                        _ => Task.FromResult((false, new HttpResponseMessage(0))))
                },
            };

        WriteLine("Welcome to the API Gateway Mock.");

        var cont = true;
        while (cont)
        {
            WriteLine();
            WriteLine();
            WriteLine("********************");
            WriteLine("Choose one of:");
            foreach (var c in processCommand.Values)
                WriteLine(c.description);
            WriteLine("********************");
            var cmd = ReadLine() ?? string.Empty;
            if (processCommand.TryGetValue(cmd[0], out var command))
            {
                var (@continue, response) = await command.handler(cmd);
                await PrettyPrint(response);
                cont = @continue;
            }
        }
    }

    static async Task<(bool, HttpResponseMessage)> HandleUpdateInterestsCommand(string cmd)
    {
        var response = await client!.QueryUser(cmd.Split(' ').Skip(1).First());
        if (!response.IsSuccessStatusCode)
            return (true, response);

        var user = JsonSerializer.Deserialize<LoyaltyProgramUser>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (user is null)
            return (true, response);
        var newInterests = cmd[cmd.IndexOf(' ', 2)..].Split(',').Select(i => i.Trim());
        var res = await client.UpdateUser(
            user with
            {
                Settings = user.Settings with
                {
                    Interests = user.Settings.Interests.Concat(newInterests).ToArray()
                }
            });
        return (true, res);
    }


    static async Task PrettyPrint(HttpResponseMessage response)
    {
        if (response.StatusCode == 0) return;
        WriteLine("********** Response **********");
        WriteLine($"status code: {response.StatusCode}");
        WriteLine("Headers: " + response.Headers.Aggregate("",
            (acc, h) => $"{acc}{NewLine}\t{h.Key}: {h.Value.Aggregate((hAcc, hVal) => $"{hAcc}{hVal}, ")}") ?? "");
        if (response.Content.Headers.ContentLength > 0)
            WriteLine(@$"Body:{NewLine}{
                JsonSerializer.Serialize(await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync()), new JsonSerializerOptions { WriteIndented = true })
            }");
    }
}
