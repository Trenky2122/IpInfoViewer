using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.Database.IpInfoViewer
{
    public interface IIpInfoViewerDbRepository
    {
        Task SeedTables();
        Task<IEnumerable<IpAddressInfo>> GetIpAddresses(int offset = 0, int limit = int.MaxValue);
        Task<IEnumerable<MapPoint>> GetMapForWeek(Week week);
        Task<DateTime?> GetLastDateWhenMapIsProcessed();
        Task SaveMapIpAddressRepresentation(MapPoint representation);
        Task SaveIpAddressInfo(IpAddressInfo address);
        Task SaveCountryPingInfo(CountryPingInfo countryPingInfo);
        Task<DateTime?> GetLastDateWhenCountriesAreProcessed();
        Task<IEnumerable<CountryPingInfo>> GetCountryPingInfoForWeek(Week week);
        Task<int> GetMaximumCountryPingForWeek(Week week);
    }
}
