using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpInfoViewer.Libs.Models;

namespace IpInfoViewer.Libs.Abstractions
{
    public interface IIpInfoViewerDbRepository
    {
        Task SeedTables();
        Task<IEnumerable<IpAddressInfo>> GetIpAddresses(int offset = 0, int limit = Int32.MaxValue);
    }
}
