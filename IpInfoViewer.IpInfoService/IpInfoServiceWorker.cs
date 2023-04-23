using IpInfoViewer.Libs.Implementation.IpInfo;

namespace IpInfoViewer.IpInfoService
{
    public class IpInfoServiceWorker : BackgroundService
    {
        private readonly ILogger<IpInfoServiceWorker> _logger;
        private readonly IIpAddressInfoFacade _ipAddressInfoFacade;
        private readonly string _pathToCsvDatabase;

        public IpInfoServiceWorker(ILogger<IpInfoServiceWorker> logger, IIpAddressInfoFacade ipAddressInfoFacade, IConfiguration config)
        {
            _logger = logger;
            _ipAddressInfoFacade = ipAddressInfoFacade;
            _pathToCsvDatabase = config["PathToCsvDatabase"];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IpInfoServiceWorker running at: {time}", DateTimeOffset.Now);

            await Parallel.ForEachAsync(
                File.ReadLines(_pathToCsvDatabase),
                async (line, token) =>
                {
                    int ipAdressesSeeded = await _ipAddressInfoFacade.ProcessLine(line);
                });
        }
    }
}