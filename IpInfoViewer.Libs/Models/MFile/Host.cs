using System.Net;

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
