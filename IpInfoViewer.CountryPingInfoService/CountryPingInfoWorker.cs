using IpInfoViewer.Libs.Implementation.CountryPing;
using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.CountryPingInfoService
{
    public class CountryPingInfoWorker : BackgroundService
    {
        private readonly ILogger<CountryPingInfoWorker> _logger;
        private readonly ICountryPingInfoFacade _countryPingInfoFacade;
        private readonly IIpInfoViewerDbRepository _localDbRepository;

        public CountryPingInfoWorker(ILogger<CountryPingInfoWorker> logger, ICountryPingInfoFacade countryPingInfoFacade, IIpInfoViewerDbRepository localDb)
        {
            _logger = logger;
            _countryPingInfoFacade = countryPingInfoFacade;
            _localDbRepository = localDb;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MapPointsServiceWorker running at: {time}", DateTimeOffset.Now);
            await _localDbRepository.SeedTables();
            var allAddresses = await _localDbRepository.GetIpAddresses();
            var addressesGroupedByLocation = allAddresses.GroupBy(address=>address.CountryCode);
            var lastProcessedDate = await _localDbRepository.GetLastDateWhenCountriesAreProcessed();
            if (!lastProcessedDate.HasValue)
                lastProcessedDate = new DateTime(2008, 4, 26); //first data from mfile database are by this date
            Week lastProcessedWeek = new(lastProcessedDate.Value);
            // parallel foreach used in case of first run or first run after weeks
            await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(lastProcessedWeek.Next().Monday, DateTime.Today.AddDays(-7) /* only already finished weeks*/), 
                stoppingToken,
                async (week, token) =>
                {
                    await _countryPingInfoFacade.ProcessWeekAsync(week, addressesGroupedByLocation);

                    Console.WriteLine($"{DateTime.Now} Week from {week.Monday} processed.");
                }
            );
        }
    }
}