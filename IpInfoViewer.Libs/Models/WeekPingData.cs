using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models
{
    public class WeekPingData
    {
        public double Average { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public (IPAddress, int) IpAddress { get; set; }
    }
}
