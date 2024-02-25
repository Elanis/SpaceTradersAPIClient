using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dysnomia.SpaceTradersAPIClient.ConsoleApp {
    internal class Program {
        static async Task Main(string[] args) {
            var httpClient = new HttpClient();
            var client = new SpaceTradersClient(httpClient);

            var factions = await client.GetFactionsAsync(1, 20);
            Console.WriteLine("Done");
        }
    }
}
