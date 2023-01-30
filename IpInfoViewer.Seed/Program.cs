using System.Globalization;
using System.Net;
using System.Text.Json.Serialization;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql.Internal.TypeHandlers.GeometricHandlers;


var configBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
var config = configBuilder.Build();
IpInfoViewerDbRepository repository = new IpInfoViewerDbRepository(config["IpInfoViewerProcessedConnectionString"]);
await repository.SeedTables();
MFileDbRepository mfileRepo = new MFileDbRepository(config["MFileConnectionString"]);
var tasks = new List<Task>();
int seeded = 0;
int linesPassed = 0;
await Parallel.ForEachAsync(
    File.ReadLines(@"C:\Users\Peter\Downloads\dbip-city-lite-2022-11.csv\dbip-city-lite-2022-11.csv"),
    async (line, token) =>
    {
        if (line.Contains("\"")) return;
        var fields = line.Split(',');
        var pingsInRange = await mfileRepo.GetHostsInRange(IPAddress.Parse(fields[0]), IPAddress.Parse(fields[1]));
        foreach (var ping in pingsInRange)
        {
            var ipInfo = new IpAddressInfo()
            {
                City = fields[5],
                CountryCode = fields[3],
                IpValue = ping.IpAddr,
                Latitude = Convert.ToDouble(fields[6], CultureInfo.InvariantCulture),
                Longitude = Convert.ToDouble(fields[7], CultureInfo.InvariantCulture),
            };
            tasks.Add(repository.SaveIpAddressInfo(ipInfo));
            seeded++;
            if (seeded % 200 == 0)
            {
                Console.WriteLine($"{DateTime.Now} {seeded} seeded");
            }
        }

        linesPassed++;
        if (linesPassed % 2000 == 0)
        {
            Console.WriteLine($"{DateTime.Now} {linesPassed} lines passed");
        }
    });

await Task.WhenAll(tasks);
Console.WriteLine("Done.");