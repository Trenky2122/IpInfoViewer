using IpInfoViewer.Libs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Abstractions
{
    public interface IMFileDbRepository
    {
        Task<IEnumerable<Tuple<(IPAddress, int), double>>> GetAverageRtTForIpForWeek(Week week);
    }
}
