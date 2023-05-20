using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using GrapeCity.Documents.Svg;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Models.Enums;
using IpInfoViewer.Libs.Utilities;
using Microsoft.Extensions.Logging;

namespace IpInfoViewer.Libs.Implementation.CountryPing
{
    public class CountryPingInfoFacade: ICountryPingInfoFacade
    {

        private readonly IIpInfoViewerDbRepository _localDb;
        private readonly IMFileDbRepository _mFileDb;
        private readonly ILogger<CountryPingInfoFacade> _logger;

        public CountryPingInfoFacade(IIpInfoViewerDbRepository localDb, IMFileDbRepository mFileDb, ILogger<CountryPingInfoFacade> logger)
        {
            _localDb = localDb;
            _mFileDb = mFileDb;
            _logger = logger;
        }

        public async Task ExecuteSeedingAsync(CancellationToken stoppingToken)
        {
            await _localDb.SeedTablesAsync();
            var allAddresses = await _localDb.GetIpAddressesAsync();
            var addressesGroupedByCountry = allAddresses.GroupBy(address => address.CountryCode);
            var lastProcessedDate = await _localDb.GetLastDateWhenCountriesAreProcessedAsync() ?? "2008-W16"; //first data from mfile database are by this date
            Week lastProcessedWeek = new(lastProcessedDate);
            // parallel foreach used in case of first run or first run after weeks
            await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(lastProcessedWeek.Next().Monday, DateTime.Today.AddDays(-7) /* only already finished weeks*/),
                stoppingToken,
                async (week, token) =>
                {
                    try
                    {
                        await ProcessWeekAsync(week, addressesGroupedByCountry);

                        _logger.LogInformation("{now} Week from {week} processed.", DateTime.Now, week);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to process week {week}.", week);
                    }
                }
            );
        }

        public async Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<string, IpAddressInfo>> addressesGroupedByCountry)
        {
            var ipAveragePings = await _mFileDb.GetWeekPingData(week);
            var countryPingInfos = addressesGroupedByCountry.Select(group =>
            {
                var pings = group.Select(addr =>
                    ipAveragePings.FirstOrDefault(p => p.IpAddress.Item1.Equals(addr.IpValue.Item1))).
                    Where(p => p is not null)
                    .ToList();
                if (!pings.Any())
                    return null;
                var result = new CountryPingInfo()
                {
                    CountryCode = group.Key,
                    IpAddressesCount = group.Count(),
                    AveragePingRtT = Convert.ToSingle(pings.Average(p => p.Average)),
                    MinimumPingRtT = Convert.ToSingle(pings.Min(p => p.Minimum)),
                    MaximumPingRtT = Convert.ToSingle(pings.Max(p => p.Maximum)),
                    Week = week.ToString()
                };
                return result;
            }).Where(x => x != null).ToList();
            await _localDb.SaveCountryPingInfosAsync(countryPingInfos);
        }

        public Task<IEnumerable<CountryPingInfo>> GetCountryPingInfoForWeekAsync(string week)
        {
            return _localDb.GetCountryPingInfoForWeekAsync(new Week(week));
        }

        public async Task<string> GetColoredSvgMapForWeekAsync(string weekStr, RequestedDataEnum requestedData, ScaleMode scaleMode)
        {
            Week week = new(weekStr);
            var svg = GcSvgDocument.FromFile(@"Assets/world.svg");
            var countryPingInfo = (await _localDb.GetCountryPingInfoForWeekAsync(week)).ToList();
            const int defaultUpperBound = 500;
            int upperBound = scaleMode switch
            {
                ScaleMode.ConstantMaximum => defaultUpperBound,
                ScaleMode.MaximumToMaximum => GetMaximumPingValueForWeekForRequestedData(countryPingInfo, requestedData),
                ScaleMode.AverageToAverage => GetAveragePingValueForRequestedData(countryPingInfo, requestedData) * 2,
                _ => throw new NotImplementedException()
            };
            var countryPingDict = new Dictionary<string, (float PingAvg, float PingMin, float PingMax, int Count)>();
            foreach (var country in countryPingInfo)
            {
                var countriesSvg = svg.GetElementsByClass(country.CountryCode);
                var color = CalculateColor(GetRequestedPingValue(country, requestedData), upperBound);
                foreach (var countrySvg in countriesSvg)
                {
                    countrySvg.Fill = new SvgPaint(Color.FromArgb(color.Red, color.Green, 0));
                }
                countryPingDict.Add(country.CountryCode, (country.AveragePingRtT, country.MinimumPingRtT, country.MaximumPingRtT, country.IpAddressesCount));
            }

            foreach (var countryKvp in GeographicUtilities.CountryCodeToNameDictionary)
            {
                var countrySvgs = svg.GetElementsByClass(countryKvp.Key);
                foreach (var svgPath in countrySvgs)
                {
                    bool pingFound = countryPingDict.TryGetValue(countryKvp.Key, out var countryData);
                    svgPath.Children.Clear();
                    SvgTitleElement title = new();
                    title.Children.Add(new SvgContentElement()
                    {
                        Content = countryKvp.Value + (pingFound ?$": {countryData.PingAvg} ms avg, {countryData.PingMin} ms min, " +
                                                                 $"{countryData.PingMax} ms max, {countryData.Count} addr. total":"") 
                    });
                    svgPath.Children.Add(title);
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

        private static float GetRequestedPingValue(CountryPingInfo countryPingInfo, RequestedDataEnum requestedData)
        {
            switch (requestedData)
            {
                case RequestedDataEnum.Average:
                    return countryPingInfo.AveragePingRtT;
                case RequestedDataEnum.Maximum:
                    return countryPingInfo.MaximumPingRtT;
                case RequestedDataEnum.Minimum:
                    return countryPingInfo.MinimumPingRtT;
                default:
                    throw new NotImplementedException();
            }
        }

        private static int GetMaximumPingValueForWeekForRequestedData(IList<CountryPingInfo> countryPingInfos,
            RequestedDataEnum requestedData)
        {
            if(!countryPingInfos.Any())
                return 0;
            switch (requestedData)
            {
                case RequestedDataEnum.Maximum:
                    return Convert.ToInt32(countryPingInfos.MaxBy(cpi => cpi.MaximumPingRtT).MaximumPingRtT);
                case RequestedDataEnum.Minimum:
                    return Convert.ToInt32(countryPingInfos.MaxBy(cpi => cpi.MinimumPingRtT).MinimumPingRtT);
                case RequestedDataEnum.Average:
                    return Convert.ToInt32(countryPingInfos.MaxBy(cpi => cpi.AveragePingRtT).AveragePingRtT);
                default: throw new NotImplementedException();
            }
        }
        private static int GetAveragePingValueForRequestedData(IList<CountryPingInfo> countryPingInfos,
            RequestedDataEnum requestedData)
        {
            if (!countryPingInfos.Any())
                return 0;
            switch (requestedData)
            {
                case RequestedDataEnum.Maximum:
                    return Convert.ToInt32(countryPingInfos.Average(cpi => cpi.MaximumPingRtT));
                case RequestedDataEnum.Minimum:
                    return Convert.ToInt32(countryPingInfos.Average(cpi => cpi.MinimumPingRtT));
                case RequestedDataEnum.Average:
                    return Convert.ToInt32(countryPingInfos.Average(cpi => cpi.AveragePingRtT));
                default: throw new NotImplementedException();
            }
        }

        public Task<string?> GetLastProcessedWeekAsync()
        {
            return _localDb.GetLastDateWhenCountriesAreProcessedAsync();
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
