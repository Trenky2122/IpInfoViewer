using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models.BaseModels
{
    public class BaseMapModel: BaseWeeklyProcessedModel
    {
        public int IpAddressesCount { get; set; }
        public float AveragePingRtT { get; set; }
    }
}
