using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.Database.IpInfoViewer
{
    public interface IIpInfoViewerDbRepository
    {
        Task SeedTablesAsync();
        Task<IEnumerable<IpAddressInfo>> GetIpAddressesAsync(int offset = 0, int limit = int.MaxValue);
        Task<IEnumerable<MapPoint>> GetMapForWeekAsync(Week week);
        Task<string?> GetLastDateWhenMapIsProcessedAsync();
        Task SaveMapIpAddressRepresentationsAsync(IEnumerable<MapPoint> representations);
        Task SaveIpAddressInfoAsync(IpAddressInfo address);
        Task SaveCountryPingInfosAsync(IEnumerable<CountryPingInfo> countryPingInfos);
        Task<string?> GetLastDateWhenCountriesAreProcessedAsync();
        Task<IEnumerable<CountryPingInfo>> GetCountryPingInfoForWeekAsync(Week week);
    }
}
