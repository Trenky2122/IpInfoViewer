using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;

namespace IpInfoViewer.Libs.Implementation.Map
{
    public interface IMapPointsFacade
    {
        Task ProcessWeekAsync(Week week,
            IEnumerable<IGrouping<(int latitude, int longitude), IpAddressInfo>> addressesGroupedByLocation);

        string GetIpMapLegend(
            List<(float Radius, int Count)> sizeInformation,
            int pingUpperBound);


        Task<string> GetLastProcessedWeek();
        Task<IEnumerable<MapPoint>> GetMapPointsForDayOfWeek(DateTime dayFromWeek);
        Task<IEnumerable<MapPoint>> GetMapPoinsForWeek(string week);
    }
}
