using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using IpInfoViewer.Libs.Implementation;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

var configBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
var config = configBuilder.Build();
IpInfoViewerDbRepository repository = new(config["IpInfoViewerProcessedConnectionString"]);
await repository.SeedTables();
MFileDbRepository mfileRepo = new(config["MFileConnectionString"]);
var allAddresses = await repository.GetIpAddresses();
var addressesGroupedByLocation = allAddresses.GroupBy(GetApproximateLocation);
await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(new DateTime(2011, 10, 3), DateTime.Today),
    async (week, token) =>
    {
        var ipAveragePings = await mfileRepo.GetAverageRtTForIpForWeek(week);
        var mapPoints = addressesGroupedByLocation.Select(x =>
        {
            var pings = x.Select(addr =>
                ipAveragePings.FirstOrDefault(p => p.Item1.Item1.Equals(addr.IpValue.Item1))?.Item2);
            if (!pings.Any(p => p.HasValue))
                return null;
            var result = new MapIpAddressesRepresentation()
            {
                Latitude = x.Key.Item1,
                Longitude = x.Key.Item2,
                IpAddressesCount = x.Count(),
                AveragePingRtT = Convert.ToSingle(pings.Where(p => p is > 0).Average(p => p ?? 0)),
                ValidFrom = week.Monday,
                ValidTo = week.Next().Monday.AddTicks(-1)
            };
            return result;
        }).Where(x => x != null);
        foreach (var point in mapPoints)
        {
            await repository.SaveMapIpAddressRepresentation(point);
        }

        Console.WriteLine($"{DateTime.Now} Week from {week.Monday} processed.");
    }
);


Console.WriteLine("done");
static ValueTuple<int, int> GetApproximateLocation(IpAddressInfo ipAddressInfo)
{
    int latitudeApproximation = 3;
    int longitudeApproximation = 8; //the lesser, more approximate map
    int roundedLatitude = Convert.ToInt32(ipAddressInfo.Latitude);
    int roundedLongitude = Convert.ToInt32(ipAddressInfo.Longitude);
    return (roundedLatitude - roundedLatitude % latitudeApproximation, roundedLongitude - roundedLongitude % longitudeApproximation);
}
