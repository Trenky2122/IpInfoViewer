using System.Text;
using GrapeCity.Documents.Svg;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.Map
{
    public class MapPointsFacade: IMapPointsFacade
    {
        private readonly IIpInfoViewerDbRepository _localDb;
        private readonly IMFileDbRepository _mFileDb;
        public MapPointsFacade(IIpInfoViewerDbRepository localDb, IMFileDbRepository mFileDb)
        {
            _localDb = localDb;
            _mFileDb = mFileDb;
        }
        public async Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<(int latitude, int longitude), IpAddressInfo>> addressesGroupedByLocation)
        {
            var ipAveragePings = await _mFileDb.GetAverageRtTForIpForWeek(week);
            var mapPoints = addressesGroupedByLocation.Select(x =>
            {
                var pings = x.Select(addr =>
                    ipAveragePings.FirstOrDefault(p => p.Item1.Item1.Equals(addr.IpValue.Item1))?.Item2).ToList();
                if (!pings.Any(p => p.HasValue))
                    return null;
                var result = new MapPoint()
                {
                    Latitude = x.Average(x => x.Latitude),
                    Longitude = x.Average(x => x.Longitude),
                    IpAddressesCount = x.Count(),
                    AveragePingRtT = Convert.ToSingle(pings.Where(p => p is > 0).Average(p => p ?? 0)),
                    ValidFrom = week.Monday,
                    ValidTo = week.Next().Monday.AddTicks(-1)
                };
                return result;
            }).Where(x => x != null).ToList();
            foreach (var point in mapPoints)
            {
                await _localDb.SaveMapIpAddressRepresentation(point);
            }
        }

        public string GetIpMapLegend(
            List<(float Radius, int Count)> sizeInformation,
            int pingUpperBound)
        {
            var svg = GcSvgDocument.FromFile(@"/app/bin/debug/net6.0/Assets/ipInfoLegend.svg");
            var legendPingValues = GetLegendPingValues(pingUpperBound);
            for (int i = 1; i <= 5; i++)
            {
                var legendPlaceholderContent = svg.GetElementByID($"ph{i}").Children[0] as SvgContentElement;
                legendPlaceholderContent.Content = $"Ping {legendPingValues[i - 1]} ms";

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

        public async Task<string?> GetLastProcessedWeek()
        {
            DateTime? lastProcessedDate = await _localDb.GetLastDateWhenMapIsProcessed();
            if (!lastProcessedDate.HasValue)
                return null;
            return new Week(lastProcessedDate.Value).ToString();
        }

        public async Task<IEnumerable<MapPoint>> GetMapPointsForDayOfWeek(DateTime dayFromWeek)
        {
            return await _localDb.GetMapForWeek(new Week(dayFromWeek));
        }

        public async Task<IEnumerable<MapPoint>> GetMapPoinsForWeek(string week)
        {
            return await _localDb.GetMapForWeek(new Week(week));
        }

        private List<int> GetLegendPingValues(int upperBound)
        {
            const int lowerBound = 20;
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
