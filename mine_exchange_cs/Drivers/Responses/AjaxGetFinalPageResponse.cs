using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_exchange_cs.Drivers.Responses
{
    public class AjaxGetFinalPageResponse
    {
        public string status { get; set; }
        public string response { get; set; }
        public int status_code { get; set; }
        public string status_text { get; set; }
        public string url { get; set; }
    }
}
