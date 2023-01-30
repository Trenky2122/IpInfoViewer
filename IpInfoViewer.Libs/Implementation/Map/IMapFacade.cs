using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Implementation.Map
{
    public interface IMapFacade
    {
        Task ProcessWeekAsync(Week week,
            IEnumerable<IGrouping<(int latitude, int longitude), IpAddressInfo>> addressesGroupedByLocation);
    }
}
