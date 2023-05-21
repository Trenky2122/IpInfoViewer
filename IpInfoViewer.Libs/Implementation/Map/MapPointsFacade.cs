using System.Text;
using GrapeCity.Documents.Svg;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Logging;

namespace IpInfoViewer.Libs.Implementation.Map
{
    public class MapPointsFacade: IMapPointsFacade
    {
        private readonly IIpInfoViewerDbRepository _localDb;
        private readonly IMFileDbRepository _mFileDb;
        private readonly ILogger<MapPointsFacade> _logger;
        public MapPointsFacade(IIpInfoViewerDbRepository localDb, IMFileDbRepository mFileDb, ILogger<MapPointsFacade> logger)
        {
            _localDb = localDb;
            _mFileDb = mFileDb;
            _logger = logger;
        }

        public async Task ExecuteSeedingAsync(CancellationToken stoppingToken)
        {
            await _localDb.SeedTablesAsync();
            var allAddresses = await _localDb.GetIpAddressesAsync();
            var addressesGroupedByLocation = allAddresses.GroupBy(GetApproximateLocation);
            var lastProcessedDate = await _localDb.GetLastDateWhenMapIsProcessedAsync() ?? "2008-W16"; //first data from mfile database are by this date
            Week lastProcessedWeek = new(lastProcessedDate);
            // parallel foreach used in case of first run or first run after weeks
            await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(lastProcessedWeek.Next().Monday, DateTime.Today.AddDays(-7) /* only already finished weeks*/),
                stoppingToken,
                async (week, token) =>
                {
                    try
                    {
                        await ProcessWeekAsync(week, addressesGroupedByLocation);

                        _logger.LogInformation("{now} Week from {week} processed.", DateTime.Now, week);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to process week {week}.", week);
                    }
                }
            );
        }

        public static (int Latitude, int Longitude) GetApproximateLocation(IpAddressInfo ipAddressInfo)
        {
            int latitudeApproximation = 3;
            int longitudeApproximation = 6; //the lesser, the more approximate map
            int roundedLatitude = Convert.ToInt32(ipAddressInfo.Latitude);
            int roundedLongitude = Convert.ToInt32(ipAddressInfo.Longitude);
            return (roundedLatitude - roundedLatitude % latitudeApproximation, roundedLongitude - roundedLongitude % longitudeApproximation);
        }

        public async Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<(int latitude, int longitude), IpAddressInfo>> addressesGroupedByLocation)
        {
            var ipWeekData = await _mFileDb.GetWeekPingData(week);
            var mapPoints = addressesGroupedByLocation.Select(addressGroup =>
            {
                var pings = addressGroup.Select(addr =>
                    ipWeekData.FirstOrDefault(p => p.IpAddress.Item1.Equals(addr.IpValue.Item1))).
                    Where(p => p is not null)
                    .ToList();
                if (!pings.Any())
                    return null;
                var result = new MapPoint()
                {
                    Latitude = addressGroup.Average(x => x.Latitude),
                    Longitude = addressGroup.Average(x => x.Longitude),
                    IpAddressesCount = addressGroup.Count(),
                    AveragePingRtT = Convert.ToSingle(pings.Average(p => p.Average)),
                    MaximumPingRtT = Convert.ToSingle(pings.Max(p => p.Maximum)),
                    MinimumPingRtT = Convert.ToSingle(pings.Min(p => p.Minimum)),
                    Week = week.ToString()
                };
                return result;
            }).Where(x => x != null).ToList();
            await _localDb.SaveMapIpAddressRepresentationsAsync(mapPoints);
        }

        public string GetIpMapLegend(
            List<(float Radius, int Count)> sizeInformation,
            int pingUpperBound)
        {
            var svg = GcSvgDocument.FromFile(@"Assets/ipInfoLegend.svg");
            var legendPingValues = GetLegendPingValues(pingUpperBound);
            for (int i = 1; i <= 5; i++)
            {
                var legendPlaceholderContent = svg.GetElementByID($"ph{i}").Children[0] as SvgContentElement;
                legendPlaceholderContent!.Content = $"Ping {legendPingValues[i - 1]} ms";

                var circle = svg.GetElementByID($"size{i}") as SvgEllipseElement;
                var radiusLength = new SvgLength(sizeInformation[i-1].Radius, SvgLengthUnits.Pixels);
                circle.RadiusX = radiusLength;
                circle.RadiusY = radiusLength;
                var text = svg.GetElementByID($"size{i}text").Children[0] as SvgContentElement;
                text.Content = $"{sizeInformation[i-1].Count} addr.";
            }

            StringBuilder resultBuilder = new();
            svg.Save(resultBuilder);
            return resultBuilder.ToString();
        }

        public Task<string?> GetLastProcessedWeek()
        {
            return _localDb.GetLastDateWhenMapIsProcessedAsync();
        }

        public Task<IEnumerable<MapPoint>> GetMapPointsForWeek(string week)
        {
            return _localDb.GetMapForWeekAsync(new Week(week));
        }

        private List<int> GetLegendPingValues(int upperBound)
        {
            const int lowerBound = 5;
            return new List<int>
            {
                lowerBound,
                Convert.ToInt32((upperBound - lowerBound) * 0.25 + lowerBound),
                Convert.ToInt32((upperBound - lowerBound) * 0.5 + lowerBound),
                Convert.ToInt32((upperBound - lowerBound) * 0.75 + lowerBound),
                upperBound
            };
        }
    }
}
