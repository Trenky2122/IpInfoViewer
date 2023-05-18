using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Models.Enums;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.CountryPing
{
    public interface ICountryPingInfoFacade
    {
        Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<string, IpAddressInfo>> addressesGroupedByCountry);
        Task<string> GetColoredSvgMapForWeek(string week, RequestedDataEnum requestedData, ScaleMode scaleMode);
        Task<string> GetLastProcessedWeek();
        Task ExecuteSeedingAsync(CancellationToken stoppingToken);
    }
}
