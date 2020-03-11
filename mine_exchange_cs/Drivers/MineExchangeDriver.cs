using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AngleSharp.Dom;
using Leaf.xNet;
using mine_exchange_cs.Components;
using mine_exchange_cs.Drivers.Responses;
using mine_exchange_cs.Helpers;
using Newtonsoft.Json;

namespace mine_exchange_cs.Drivers
{
    public class MineExchangeDriver
    {
        public const string BASE_URL = "https://mine.exchange";
        public const string PAIR = "/exchange_QWRUB_to_BTC/";

        public Leaf.xNet.HttpRequest httpRequest = new Leaf.xNet.HttpRequest();
        public Leaf.xNet.HttpResponse current = null;

        public MineExchangeDriver()
        {
            httpRequest.UserAgentRandomize();
            httpRequest.ConnectTimeout = 10000;
        }

        public string GetUrlForCreate( string email, double amountRUB, string phone, string addressBTC )
        {
            this.current = httpRequest.Get(BASE_URL + PAIR);
            Parser parser = new Parser(this.current.ToString());
            IElement form = parser.GetForm("ajax_post_bids");
            List<ParserInputData> inputs = parser.GetInputsByForm(form);
            parser
                .ModifyInputs(ref inputs, "cf6", email)
                .ModifyInputs(ref inputs, "sum1", amountRUB.ToString())
                .ModifyInputs(ref inputs, "account1", phone)                
                .ModifyInputs(ref inputs, "account2", addressBTC.ToString())
                .ModifyInputs(ref inputs, "check_data", "1");

            string action = BASE_URL + form.GetAttribute("action");            
            var responseJson = httpRequest.Post(action, HtmlHelper.ConvertToRequestParams(inputs));
            var response     = JsonConvert.DeserializeObject<AjaxCreateResponse>(responseJson.ToString());

            return response.url;
        }

        public string GetUrlFinalPage(string url, out double amountBTC)
        {
            this.current = httpRequest.Get(url);
            var html = this.current.ToString();

            Parser parser = new Parser(html);            
            amountBTC = parser.GetAmount("block_xchdata_line", "Bitcoin BTC");

            IElement form = parser.GetForm("ajax_post_form");
            List<ParserInputData> inputs = parser.GetInputsByForm(form);
            parser.ModifyInputs(ref inputs, "check", "1");

            string action = BASE_URL + form.GetAttribute("action");
            var responseJson = httpRequest.Post(action, HtmlHelper.ConvertToRequestParams(inputs));
            var response     = JsonConvert.DeserializeObject<AjaxGetFinalPageResponse>(responseJson.ToString());

            return response.url;
        }

        public RecvisitsResponse GetRecvisitsOnFinalPage(string url)
        {
            this.current = httpRequest.Get(url);
            string html = this.current.ToString();            

            Parser parser = new Parser(html);
            string urlWithRecvisits = parser.GetHref("success_paybutton").Replace("&amp;", "&");
            Uri urlWithRecvisitsURI = new Uri(urlWithRecvisits);
            var urlParams = HttpUtility.ParseQueryString(urlWithRecvisitsURI.Query);
            
            RecvisitsResponse recvisits = new RecvisitsResponse() {
                extra_account  = urlParams.Get("extra['account']"),
                source         = urlParams.Get("source"),
                amountInteger  = urlParams.Get("amountInteger"),
                amountFraction = urlParams.Get("amountFraction"),
                currency       = urlParams.Get("currency"),
                extra_comment  = urlParams.Get("extra['comment']")
            };

            return recvisits;
        }

        public bool CheckIsWork()
        {
            try
            {
                Leaf.xNet.HttpResponse mainPageContent = httpRequest.Get(BASE_URL);
                if (mainPageContent.ToString().Length > 0) return true;                
            }
            catch (Exception) { }

            return false;
        }

        public (double, double) GetRate()
        {
            double rate = 0, balance = 0;

            this.current = httpRequest.Get(BASE_URL + PAIR);
            string html = this.current.ToString();
            Parser parser = new Parser(html);

            rate = MoneyHelper.ToDouble(parser.GetValueInput("sum1"));
            string draftBalance = parser.GetContent("span_get_max")
                .ToLower()
                .Replace("max.", "")
                .Replace("max", "")
                .Replace(":", "")
                .Replace("btc", "")
                .Trim();
            balance = MoneyHelper.ToDouble(draftBalance);

            return (rate, balance);
        }
    }
}
