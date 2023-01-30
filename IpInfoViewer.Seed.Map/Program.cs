using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

var configBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
var config = configBuilder.Build();
IpInfoViewerDbRepository repository = new(config["IpInfoViewerProcessedConnectionString"]);
await repository.SeedTables();
MFileDbRepository mfileRepo = new(config["MFileConnectionString"]);
MapFacade mapFacadde = new(repository, mfileRepo);
var allAddresses = await repository.GetIpAddresses();
var addressesGroupedByLocation = allAddresses.GroupBy(GetApproximateLocation);
await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(new DateTime(2011, 10, 3), DateTime.Today),
    async (week, token) =>
    {
        await mapFacadde.ProcessWeekAsync(week, addressesGroupedByLocation);

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
