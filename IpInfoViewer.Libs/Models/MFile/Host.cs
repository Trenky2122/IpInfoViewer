using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models.MFile
{
    public class Host
    {
        public ValueTuple<IPAddress, int> IpAddr { get; set; }
        public int RankCode { get; set; }
        public DateTime EnterDate { get; set; }
        public string Source { get; set; }
        public string Comment { get; set; }
        public bool Exclude { get; set; }
    }
}
