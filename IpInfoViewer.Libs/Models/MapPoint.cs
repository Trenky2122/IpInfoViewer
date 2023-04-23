using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpInfoViewer.Libs.Models.BaseModels;

namespace IpInfoViewer.Libs.Models
{
    public class MapPoint: BaseMapModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
