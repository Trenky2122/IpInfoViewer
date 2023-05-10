using IpInfoViewer.Libs.Models;
using System.Globalization;
using System.Net;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Database.MFile;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IpInfoViewer.Libs.Implementation.IpInfo
{
    public class IpAddressInfoFacade: IIpAddressInfoFacade
    {
        private readonly IMFileDbRepository _mFileDb;
        private readonly IIpInfoViewerDbRepository _localDb;
        private readonly string _pathToCsvDatabase;
        private readonly ILogger<IpAddressInfoFacade> _logger;
        public IpAddressInfoFacade(IMFileDbRepository mFileDb, IIpInfoViewerDbRepository localDb, IConfiguration config, ILogger<IpAddressInfoFacade> logger)
        {
            _mFileDb = mFileDb;
            _localDb = localDb;
            _pathToCsvDatabase = config["PathToCsvDatabase"];
            _logger = logger;
        }

        public async Task ExecuteSeedingAsync(CancellationToken stoppingToken)
        {
            await _localDb.SeedTables();
            int ipAddressesSeeded = 1;
            int linesPassed = 0;
            await Parallel.ForEachAsync(
                File.ReadLines(_pathToCsvDatabase),
                stoppingToken,
                async (line, token) =>
                {
                    try
                    {
                        ipAddressesSeeded += await ProcessLine(line);
                        linesPassed++;
                        if (linesPassed % 2000 == 0)
                            _logger.LogInformation("{time}:{lines} lines passed", DateTime.Now, linesPassed);
                        if (ipAddressesSeeded % 200 == 0)
                            _logger.LogInformation("{time}:{ipAddr} addresses seeded", DateTime.Now, ipAddressesSeeded);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, e.Message);
                    }
                });
        }

        public async Task<int> ProcessLine(string line)
        {
            if (line.Contains("\"")) return 0;
            var fields = line.Split(',');
            var pingsInRange = (await _mFileDb.GetHostsInRange(IPAddress.Parse(fields[0]), IPAddress.Parse(fields[1]))).ToList();
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

            return pingsInRange.Count;
        }
    }
}
