using System.Net;

namespace IpInfoViewer.Libs.Models.MFile
{
    public class Ping
    {
        public ValueTuple<IPAddress, int> IpAddr { get; set; }
        public float? PingRtTMin { get; set; }
        public float? PingRtTMax { get; set; }
        public float? PingRtTAvg { get; set; }
        public float? PingRtTMDev { get; set; }
        public int PingPLoss { get; set; }
        public DateTime PingDate { get; set; }
    }
}
