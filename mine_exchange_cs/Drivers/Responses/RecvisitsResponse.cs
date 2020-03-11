using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mine_exchange_cs.Drivers.Responses
{
    public class RecvisitsResponse
    {
        public string extra_account;  //extra['account']
        public string source;         //source
        public string amountInteger;  //amountInteger
        public string amountFraction; //amountFraction
        public string currency;       //currency
        public string extra_comment;  //extra['comment']
    }
}
