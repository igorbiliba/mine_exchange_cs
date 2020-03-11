using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_exchange_cs.Drivers.Responses
{
    public class AjaxCreateResponse
    {
        public string status { get; set; }
        public string response { get; set; }
        public int status_code { get; set; }
        public string status_text { get; set; }
        public string url { get; set; }
        public int account1_error { get; set; }
        public string account1_error_text { get; set; }
        public int account2_error { get; set; }
        public string account2_error_text { get; set; }
        public int summ1_error { get; set; }
        public string summ1_error_text { get; set; }
        public int summ2_error { get; set; }
        public string summ2_error_text { get; set; }
        public int summ1c_error { get; set; }
        public string summ1c_error_text { get; set; }
        public int summ2c_error { get; set; }
        public string summ2c_error_text { get; set; }
        public List<object> cf { get; set; }
        public List<object> cf_er { get; set; }
        public List<object> cfc { get; set; }
        public List<object> cfc_er { get; set; }
    }
}
