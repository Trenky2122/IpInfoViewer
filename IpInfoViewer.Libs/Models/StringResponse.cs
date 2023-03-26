using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpInfoViewer.Libs.Models
{
    public class StringResponse
    {
        public string Response { get; set; }

        public StringResponse(string response)
        {
            Response = response;
        }
    }
}
