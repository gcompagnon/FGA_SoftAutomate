//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Event = Bloomberglp.Blpapi.Event;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;
using Subscription = Bloomberglp.Blpapi.Subscription;
using CorrelationID = Bloomberglp.Blpapi.CorrelationID;

namespace Examples.src
{
    class SubscriptionCorrelationExample
    {
        private Session d_session;
        private SessionOptions d_sessionOptions;
        private List<string> d_securityList;
        private GridWindow d_gridWindow;

        public class GridWindow
        {
            private String          d_name;
            private List<string>    d_securityList;

            public GridWindow(String name, List<string> securityList)
            {
                d_name = name;
                d_securityList = securityList;
            }

            public void processSecurityUpdate(Message msg, long row)
            {
                System.Console.WriteLine(d_name + ": row " +
                    row + " got update for " + d_securityList[(int)row]);
            }
        }

        public SubscriptionCorrelationExample()
        {
            d_sessionOptions = new SessionOptions();
            d_sessionOptions.ServerHost = "localhost";
            d_sessionOptions.ServerPort = 8194;

            d_securityList = new List<string>();
            d_securityList.Add("IBM US Equity");
            d_securityList.Add("VOD LN Equity");
            d_gridWindow = new GridWindow("SecurityInfo", d_securityList);
        }

        private bool createSession()
        {
            System.Console.WriteLine("Connecting to "
                                + d_sessionOptions.ServerHost
                                + ":" + d_sessionOptions.ServerPort);
            d_session = new Session(d_sessionOptions);
            if (!d_session.Start())
            {
                System.Console.Error.WriteLine("Failed to connect!");
                return false;
            }
            if (!d_session.OpenService("//blp/mktdata"))
            {
                System.Console.Error.WriteLine("Failed to open //blp/mktdata");
                return false;
            }
            return true;
        }

        private void run(String[] args)
        {
            if (!createSession()) return;

            List<Subscription> subscriptionList = new List<Subscription>();
            for (int i = 0; i < d_securityList.Count; ++i)
            {
                subscriptionList.Add(new Subscription(
                                        (String)(d_securityList[i]),
                                        "LAST_PRICE", new CorrelationID(i))
                                     );
            }
            d_session.Subscribe(subscriptionList);

            while (true)
            {
                Event eventObj = d_session.NextEvent();
                foreach (Message msg in eventObj)
                {
                    if (eventObj.Type == Event.EventType.SUBSCRIPTION_DATA)
                    {
                        long row = msg.CorrelationID.Value;
                        d_gridWindow.processSecurityUpdate(msg, row);
                    }
                }
            }
        }

        public static void Main(String[] args)
        {
            System.Console.WriteLine("SubscriptionCorrelationExample");
            SubscriptionCorrelationExample example =
                new SubscriptionCorrelationExample();
            try
            {
                example.run(args);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
            System.Console.WriteLine("Press ENTER to quit");
            try
            {
                System.Console.Read();
            }
            catch (System.IO.IOException)
            {
            }
        }
    }
}
