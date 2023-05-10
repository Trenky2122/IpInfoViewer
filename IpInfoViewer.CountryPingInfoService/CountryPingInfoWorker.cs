using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.CountryPingInfoService
{
    public class CountryPingInfoWorker : BackgroundService
    {
        private readonly ILogger<CountryPingInfoWorker> _logger;
        private readonly ICountryPingInfoFacade _countryPingInfoFacade;

        public CountryPingInfoWorker(ILogger<CountryPingInfoWorker> logger, ICountryPingInfoFacade countryPingInfoFacade)
        {
            _logger = logger;
            _countryPingInfoFacade = countryPingInfoFacade;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MapPointsServiceWorker running at: {time}", DateTimeOffset.Now);
            await _countryPingInfoFacade.ExecuteSeedingAsync(stoppingToken);
            Environment.Exit(0);
        }
    }
}