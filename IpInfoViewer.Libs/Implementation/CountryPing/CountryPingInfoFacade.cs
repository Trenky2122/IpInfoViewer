﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> GetColoredSvgMapForWeek(Week week)
        {
            var svg = GcSvgDocument.FromFile(@"/app/bin/debug/net6.0/Assets/world.svg");
            var countryPingInfo = await _localDb.GetCountryPingInfoForWeek(week);
            foreach (var country in countryPingInfo)
            {
                var countriesSvg = svg.GetElementsByClass(country.CountryCode);

                var color = CalculateColor(country.AveragePingRtT);
                foreach (var countrySvg in countriesSvg)
                {
                    countrySvg.Fill = new SvgPaint(Color.FromArgb(color.Red, color.Green, 0));
                }
            }
            StringBuilder resultBuilder = new();
            svg.Save(resultBuilder);
            return resultBuilder.ToString();
        }

        private (int Red, int Green) CalculateColor(double ping)
        {
            const int upperBound = 500;
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
    }
}