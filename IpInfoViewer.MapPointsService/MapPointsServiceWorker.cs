using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.MapPointsService
{
    public class MapPointsServiceWorker : BackgroundService
    {
        private readonly ILogger<MapPointsServiceWorker> _logger;
        private readonly IIpInfoViewerDbRepository _dbRepository;
        private readonly IMapFacade _mapFacade;

        public MapPointsServiceWorker(ILogger<MapPointsServiceWorker> logger, IIpInfoViewerDbRepository dbRepository, IMapFacade mapFacade)
        {
            _logger = logger;
            _dbRepository = dbRepository;
            _mapFacade = mapFacade;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("MapPointsServiceWorker running at: {time}", DateTimeOffset.Now);

            var allAddresses = await _dbRepository.GetIpAddresses();
            var addressesGroupedByLocation = allAddresses.GroupBy(GetApproximateLocation);
            var lastProcessedDate = await _dbRepository.GetLastDateWhenMapIsProcessed();
            if (!lastProcessedDate.HasValue)
                lastProcessedDate = new DateTime(2008, 5, 1); //first data from mfile database are by this date, TODO: find exact date;
            Week lastProcessedWeek = new(lastProcessedDate.Value);
            // parallel foreach used in case of first run or first run after weeks
            await Parallel.ForEachAsync(DateTimeUtilities.GetWeeksFromTo(lastProcessedWeek.Next().Monday, DateTime.Today.AddDays(-7) /* only already finished weeks*/),
                async (week, token) =>
                {
                    await _mapFacade.ProcessWeekAsync(week, addressesGroupedByLocation);

                    Console.WriteLine($"{DateTime.Now} Week from {week.Monday} processed.");
                }
            );
        }

        private static (int Latitude, int Longitude) GetApproximateLocation(IpAddressInfo ipAddressInfo)
        {
            int latitudeApproximation = 3;
            int longitudeApproximation = 8; //the lesser, more approximate map
            int roundedLatitude = Convert.ToInt32(ipAddressInfo.Latitude);
            int roundedLongitude = Convert.ToInt32(ipAddressInfo.Longitude);
            return (roundedLatitude - roundedLatitude % latitudeApproximation, roundedLongitude - roundedLongitude % longitudeApproximation);
        }
    }
}