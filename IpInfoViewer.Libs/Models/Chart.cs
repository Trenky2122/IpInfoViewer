using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models
{
    public class Chart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string XAxis { get; set; }
        public string YAxis { get; set; }
        public ChartTypeEnum ChartType { get; set; }
        
    }

    public enum ChartTypeEnum
    {
        Linear,
        Column
    }
}
