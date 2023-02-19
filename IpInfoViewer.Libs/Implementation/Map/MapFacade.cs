using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrapeCity.Documents.Svg;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.Map
{
    public class MapFacade: IMapFacade
    {
        private readonly IIpInfoViewerDbRepository _localDb;
        private readonly IMFileDbRepository _mFileDb;
        public MapFacade(IIpInfoViewerDbRepository localDb, IMFileDbRepository mFileDb)
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
                var result = new MapIpAddressesRepresentation()
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
    }
}
