using Dysnomia.SpaceTradersAPIClient.ConsoleApp;

using Microsoft.Extensions.Configuration;

using System.Net.Http.Headers;
using System.Reflection;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
    .Build();

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["TOKEN"]);
var client = new SpaceTradersClient(httpClient);

var me = (await client.GetMyAgentAsync()).Data;
Console.WriteLine($"Logged in as {me.Symbol}");

var contracts = (await client.GetContractsAsync(1, 20)).Data;
Console.WriteLine($"Found {contracts.Count} contracts");

var ships = (await client.GetMyShipsAsync(1, 20)).Data;
Console.WriteLine($"Found {ships.Count} ships");

var waypoints = await client.GetSystemWaypointsAsync(1, 20, WaypointType.ENGINEERED_ASTEROID, null, "X1-CA97");
foreach (var contract in contracts) {
    if (contract.Fulfilled) {
        Console.WriteLine($"Contract {contract.Id} already finished. Skipping.");
        continue;
    }

    if (!contract.Accepted) {
        var res = await client.AcceptContractAsync(contract.Id);

        if (res.Data.Contract.Accepted) {
            Console.WriteLine($"Contract {contract.Id} has been accepted");
        } else {
            Console.WriteLine($"Contract {contract.Id} cannot be accepted");
        }
    } else {
        Console.WriteLine($"Contract {contract.Id} has already been accepted");
    }

    Console.WriteLine($"Contract {contract.Id} expiring at {contract.Expiration}");
}
contracts = (await client.GetContractsAsync(1, 20)).Data;

foreach (var ship in ships) {
    var navRes = await client.NavigateShipAsync(new Body9 {
        WaypointSymbol = waypoints.Data.First().SystemSymbol
    }, ship.Symbol);

    await client.OrbitShipAsync(ship.Symbol);

    await client.ExtractResourcesAsync(new Body6 {
    }, ship.Symbol);
}