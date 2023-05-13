using IpInfoViewer.Libs.Implementation.Database.IpInfoViewer;
using IpInfoViewer.Libs.Implementation.Map;
using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.MapPointsService
{
    public class MapPointsServiceWorker : BackgroundService
    {
        private readonly ILogger<MapPointsServiceWorker> _logger;
        private readonly IMapPointsFacade _mapPointsFacade;

        public MapPointsServiceWorker(ILogger<MapPointsServiceWorker> logger, IMapPointsFacade mapPointsFacade)
        {
            _logger = logger;
            _mapPointsFacade = mapPointsFacade;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("MapPointsServiceWorker running at: {time}", DateTimeOffset.Now);
                await _mapPointsFacade.ExecuteSeedingAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromDays(7), stoppingToken); // make sure program
            }
        }

        
    }
}