using IpInfoViewer.Libs.Models.MFile;
using IpInfoViewer.Libs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Implementation.Database.MFile
{
    public interface IMFileDbRepository
    {
        Task<IEnumerable<Tuple<(IPAddress, int), double>>> GetAverageRtTForIpForWeek(Week week);
        Task<IEnumerable<Host>> GetHostsInRange(IPAddress start, IPAddress end);
    }
}
