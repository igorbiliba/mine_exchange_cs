using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_exchange_cs.Responses
{
    public class RequestPaymentResponseType
    {
        public string account;              //extra['account']
        public string comment;              //extra['comment']
        public double btc_amount;           //
        public string ip;                   //
        public string email;                //
        
        public string toJson() => Newtonsoft.Json.JsonConvert.SerializeObject(this);

        public bool IsValid()
        {
            if (account.Trim().Length == 0) return false;
            if (comment.Trim().Length == 0) return false;
            if (btc_amount <= 0) return false;

            return true;
        }
    }
}
