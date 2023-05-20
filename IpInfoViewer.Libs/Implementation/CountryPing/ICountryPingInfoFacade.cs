using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Models.Enums;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.CountryPing
{
    public interface ICountryPingInfoFacade
    {
        Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<string, IpAddressInfo>> addressesGroupedByCountry);
        Task<string> GetColoredSvgMapForWeekAsync(string week, RequestedDataEnum requestedData, ScaleMode scaleMode);
        Task<string> GetLastProcessedWeekAsync();
        Task ExecuteSeedingAsync(CancellationToken stoppingToken);
        Task<IEnumerable<CountryPingInfo>> GetCountryPingInfoForWeekAsync(string week);
    }
}
