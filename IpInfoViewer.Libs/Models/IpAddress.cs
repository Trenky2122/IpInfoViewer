using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models
{
    public class IpAddress
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string IpValue { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
