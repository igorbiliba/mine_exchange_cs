using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_exchange_cs.Responses
{
    public class RateResponseType
    {
        public double rate;
        public double balance;
        
        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
}
