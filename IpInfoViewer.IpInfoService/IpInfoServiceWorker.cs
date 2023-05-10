using IpInfoViewer.Libs.Implementation.IpInfo;

namespace IpInfoViewer.IpInfoService
{
    public class IpInfoServiceWorker : BackgroundService
    {
        private readonly ILogger<IpInfoServiceWorker> _logger;
        private readonly IIpAddressInfoFacade _ipAddressInfoFacade;

        public IpInfoServiceWorker(ILogger<IpInfoServiceWorker> logger, IIpAddressInfoFacade ipAddressInfoFacade, IConfiguration config)
        {
            _logger = logger;
            _ipAddressInfoFacade = ipAddressInfoFacade;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("IpInfoServiceWorker running at: {time}", DateTimeOffset.Now);

            await _ipAddressInfoFacade.ExecuteSeedingAsync(stoppingToken);
            Environment.Exit(0);
        }
    }
}