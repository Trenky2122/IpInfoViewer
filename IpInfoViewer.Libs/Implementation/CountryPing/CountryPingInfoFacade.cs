using System.Drawing;
using System.Text;
using GrapeCity.Documents.Svg;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.CountryPing
{
    public class CountryPingInfoFacade: ICountryPingInfoFacade
    {

        private readonly IIpInfoViewerDbRepository _localDb;
        private readonly IMFileDbRepository _mFileDb;

        public CountryPingInfoFacade(IIpInfoViewerDbRepository localDb, IMFileDbRepository mFileDb)
        {
            _localDb = localDb;
            _mFileDb = mFileDb;
        }

        public async Task ExecuteSeedingAsync(CancellationToken stoppingToken)
        {
            await _localDb.SeedTables();
            var allAddresses = await _localDb.GetIpAddresses();
            var addressesGroupedByLocation = allAddresses.GroupBy(address => address.CountryCode);
            var lastProcessedDate = await _localDb.GetLastDateWhenCountriesAreProcessed() ?? new DateTime(2008, 4, 26); //first data from mfile database are by this date
            Week lastProcessedWeek = new(lastProcessedDate);
            // parallel foreach used in case of first run or first run after weeks
            await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(lastProcessedWeek.Next().Monday, DateTime.Today.AddDays(-7) /* only already finished weeks*/),
                stoppingToken,
                async (week, token) =>
                {
                    await ProcessWeekAsync(week, addressesGroupedByLocation);

                    Console.WriteLine($"{DateTime.Now} Week from {week.Monday} processed.");
                }
            );
        }

        public async Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<string, IpAddressInfo>> addressesGroupedByCountry)
        {
            var ipAveragePings = await _mFileDb.GetAverageRtTForIpForWeek(week);
            var mapPoints = addressesGroupedByCountry.Select(group =>
            {
                var pings = group.Select(addr =>
                    ipAveragePings.FirstOrDefault(p => p.Item1.Item1.Equals(addr.IpValue.Item1))?.Item2).ToList();
                if (!pings.Any(p => p.HasValue))
                    return null;
                var result = new CountryPingInfo()
                {
                    CountryCode = group.Key,
                    IpAddressesCount = group.Count(),
                    AveragePingRtT = Convert.ToSingle(pings.Where(p => p is > 0).Average(p => p ?? 0)),
                    ValidFrom = week.Monday,
                    ValidTo = week.Next().Monday.AddTicks(-1)
                };
                return result;
            }).Where(x => x != null).ToList();
            foreach (var point in mapPoints)
            {
                await _localDb.SaveCountryPingInfo(point);
            }
        }

        public async Task<string> GetColoredSvgMapForWeek(string weekStr, bool fullScale)
        {
            Week week = new(weekStr);
            var svg = GcSvgDocument.FromFile(@"Assets/world.svg");
            var countryPingInfo = await _localDb.GetCountryPingInfoForWeek(week);
            const int defaultUpperBound = 500;
            int upperBound = fullScale ? await _localDb.GetMaximumCountryPingForWeek(week) : defaultUpperBound;
            foreach (var country in countryPingInfo)
            {
                var countriesSvg = svg.GetElementsByClass(country.CountryCode);
                var color = CalculateColor(country.AveragePingRtT, upperBound);
                foreach (var countrySvg in countriesSvg)
                {
                    countrySvg.Fill = new SvgPaint(Color.FromArgb(color.Red, color.Green, 0));
                }
            }

            var legendPingValues = GetLegendPingValues(upperBound);
            for (int i = 1; i <= 5; i++)
            {
                var legendPlaceholderContent = svg.GetElementByID($"ph{i}").Children[0] as SvgContentElement;
                legendPlaceholderContent.Content = $"Ping {legendPingValues[i - 1]}ms";
            }
            StringBuilder resultBuilder = new();
            svg.Save(resultBuilder);
            return resultBuilder.ToString();
        }

        public async Task<string?> GetLastProcessedWeek()
        {
            DateTime? lastProcessedDate = await _localDb.GetLastDateWhenCountriesAreProcessed();
            if (!lastProcessedDate.HasValue)
                return null;
            return new Week(lastProcessedDate.Value).ToString();
        }

        private (int Red, int Green) CalculateColor(double ping, int upperBound)
        {
            const int lowerBound = 20;
            int pingInBounds = Convert.ToInt32(ping);
            if (pingInBounds < lowerBound)
                pingInBounds = lowerBound;
            if (pingInBounds > upperBound)
                pingInBounds = upperBound; 
            double percent = (pingInBounds - lowerBound*1.0) / (upperBound - lowerBound);
            int red = Convert.ToInt32(255 * percent);
            int green = Convert.ToInt32((1 - percent) * 255);
            return (red, green);
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
