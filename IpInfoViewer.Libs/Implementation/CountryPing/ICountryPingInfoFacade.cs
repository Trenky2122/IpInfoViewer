using IpInfoViewer.Libs.Models;
using IpInfoViewer.Libs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Implementation.CountryPing
{
    public interface ICountryPingInfoFacade
    {
        Task ProcessWeekAsync(Week week, IEnumerable<IGrouping<string, IpAddressInfo>> addressesGroupedByCountry);
        Task<string> GetColoredSvgMapForWeek(Week week);
    }
}
