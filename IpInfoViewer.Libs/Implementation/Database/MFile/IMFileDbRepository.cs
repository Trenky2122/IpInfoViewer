using IpInfoViewer.Libs.Models.MFile;
using IpInfoViewer.Libs.Utilities;
using System.Net;

namespace IpInfoViewer.Libs.Implementation.Database.MFile
{
    public interface IMFileDbRepository
    {
        Task<IEnumerable<Tuple<(IPAddress, int), double>>> GetAverageRtTForIpForWeek(Week week);
        Task<IEnumerable<Host>> GetHostsInRange(IPAddress start, IPAddress end);
    }
}
