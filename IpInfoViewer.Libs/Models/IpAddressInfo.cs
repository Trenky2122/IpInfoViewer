using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IpInfoViewer.Libs.Models.BaseModels;

namespace IpInfoViewer.Libs.Models
{
    public class IpAddressInfo: BaseModel
    {
        public string CountryCode { get; set; }
        public string City { get; set; }
        [JsonIgnore]
        public ValueTuple<IPAddress, int> IpValue { get; set; }
        [JsonPropertyName("ipValue")] 
        public string IpStringValue {
            get => IpValue.Item1.ToString();
            set => IpValue = (IPAddress.Parse(value), 32);
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
