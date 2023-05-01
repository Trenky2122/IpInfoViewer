using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.CountryPing
{
    public interface ICountryPingInfoFacade
    {
        Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<string, IpAddressInfo>> addressesGroupedByCountry);
        Task<string> GetColoredSvgMapForWeek(string week, bool fullScale);
        Task<string> GetLastProcessedWeek();
    }
}
