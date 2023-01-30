using IpInfoViewer.Libs.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;

namespace IpInfoViewer.Libs.Implementation.IpInfo
{
    public class IpAddressInfoFacade
    {
        private readonly IMFileDbRepository _mFileDb;
        private readonly IIpInfoViewerDbRepository _localDb;
        public IpAddressInfoFacade(IMFileDbRepository mFileDb, IIpInfoViewerDbRepository localDb)
        {
            _mFileDb = mFileDb;
            _localDb = localDb;
        }
        public async Task ProcessLine(string line)
        {
            if (line.Contains("\"")) return;
            var fields = line.Split(',');
            var pingsInRange = await _mFileDb.GetHostsInRange(IPAddress.Parse(fields[0]), IPAddress.Parse(fields[1]));
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
                await _localDb.SaveIpAddressInfo(ipInfo);
            }
        }
    }
}
