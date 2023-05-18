using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models.Enums
{
    public enum RequestedDataEnum
    {
        Average,
        Minimum,
        Maximum
    }

    public enum ScaleMode
    {
        AverageToAverage,
        MaximumToMaximum,
        ConstantMaximum
    }
}
