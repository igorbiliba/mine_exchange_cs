using mine_exchange_cs.Components;
using mine_exchange_cs.Data;
using mine_exchange_cs.Drivers;
using mine_exchange_cs.Helpers;
using mine_exchange_cs.Models;
using mine_exchange_cs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static mine_exchange_cs.Data.ProxySettings;

namespace mine_exchange_cs
{
    class Program
    {
        const int ACTION_ID = 0;

        static int MAX_CNT_CHECK_PROXY = 30;
        static void Main(string[] args)
        {
            //args = "--create 5500 +79062614762 3DQjZmcaiCJ9sKCkLk1QZ9CdLv5btKzrHb".Split(' ');
            try
            {
                Settings settings                   = new Settings() {  data       = SettingsData.Load(),
                                                                        proxyItems = ProxySettings.Load() };
                DB db                               = new DB();
                StorageModel storageModel           = new StorageModel(db).MigrateUp();
                EmailStorageModel emailStorageModel = new EmailStorageModel(storageModel);
                EmailStack emailStack               = new EmailStack(emailStorageModel, settings.data.allowEmails);
                ProxyStorageModel proxyStorageModel = new ProxyStorageModel(storageModel);
                ProxySettingsItem proxy             = null;
                MineExchangeDriver driver           = new MineExchangeDriver();

                if (args.Length == 0)
                {
                    Console.WriteLine("no args");
                    return;
                }

                switch (args[ACTION_ID])
                {
                    case "--rate":
                        (double, double) rateList = driver.GetRate();
                        Console.WriteLine(new RateResponseType()
                        {
                            rate = rateList.Item1,
                            balance = rateList.Item2
                        }
                        .toJson());
                        break;
                    case "--create":
                        for (int nTry = 0; nTry < settings.data.cntTryOnFault; nTry++)
                        {
                            try
                            {
                                if (settings.proxyItems != null)
                                {
                                    ProxyStack proxyStack = new ProxyStack(settings.proxyItems, proxyStorageModel);
                                    while (--MAX_CNT_CHECK_PROXY > 0)
                                    {
                                        try
                                        {
                                            var tester = new MineExchangeDriver();
                                            proxy = proxyStack.Next();
                                            tester.httpRequest.Proxy = proxy.CreateProxyClient();

                                            if (tester.CheckIsWork()) break;
                                            else proxy = null;
                                        }
                                        catch (Exception) { }
                                    }
                                }

                                if (proxy != null)
                                    driver.httpRequest.Proxy = proxy.CreateProxyClient();

                                double amountRUB = MoneyHelper.ToDouble(args[1]);
                                string phone = PhoneHelper.PhoneReplacer(args[2]);
                                string addressBTC = args[3];

                                string email = new String(
                                    phone.Where(Char.IsDigit).ToArray()
                                ) + emailStack.Next();

                                var urlCreate = driver.GetUrlForCreate(email,
                                                                       amountRUB,
                                                                       phone.Replace(" ", "").Replace("+", ""),
                                                                       addressBTC);
                                double amountBTC = 0;
                                var urlFianlPage = driver.GetUrlFinalPage(urlCreate, out amountBTC);
                                var recvisits = driver.GetRecvisitsOnFinalPage(urlFianlPage);

                                RequestPaymentResponseType response = new RequestPaymentResponseType()
                                {
                                    account = PhoneHelper.PhoneReplacer(recvisits.extra_account),
                                    comment = recvisits.extra_comment,
                                    email = email,
                                    btc_amount = amountBTC,
                                    ip = proxy == null ? "local" : proxy.ip
                                };

                                if (!response.IsValid())
                                    throw new Exception();

                                Console.WriteLine(response.toJson());
                                return;
                            }
                            catch (Exception) { }
                        }
                        break;
                }
            } catch (Exception) { }
        }
    }
}
