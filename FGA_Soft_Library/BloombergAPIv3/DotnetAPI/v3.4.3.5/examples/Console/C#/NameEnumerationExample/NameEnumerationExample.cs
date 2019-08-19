//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using Event = Bloomberglp.Blpapi.Event;
using EventHandler = Bloomberglp.Blpapi.EventHandler;
using Element = Bloomberglp.Blpapi.Element;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using NameEnumeration = Bloomberglp.Blpapi.NameEnumeration;
using NameEnumerationTable = Bloomberglp.Blpapi.NameEnumerationTable;
using Session = Bloomberglp.Blpapi.Session;

using ArrayList = System.Collections.ArrayList;
using Thread = System.Threading.Thread;
using System.Collections.Generic;

namespace Bloomberglp.Blpapi.Examples
{

    public class NameEnumerationExample
    {
        private string             d_host;
        private int                d_port;
        private SessionOptions     d_sessionOptions;
        private Session            d_session;
        private List<string>       d_securities;
        private List<string>       d_options;
        private List<Subscription> d_subscriptions;

        private NameEnumerationTable d_subscriptionDataMsgEnumTable;
        private NameEnumerationTable d_subscriptionStatusMsgEnumTable;

        private const string BLP_MKTDATA_SVC = "//blp/mktdata";

        public class SubscriptionDataMsgType : NameEnumeration
        {
            public const int BID = 1;
            public const int ASK = 2;
            public const int LAST_PRICE = 3;
        }

        public class SubscriptionStatusMsgType : NameEnumeration
        {
            public const int SUBSCRIPTION_STARTED = 1;
            public const int SUBSCRIPTION_FAILURE = 2;
            public const int SUBSCRIPTION_TERMINATED = 3;

            public class NameBindings
            {
                public const string SUBSCRIPTION_STARTED =
                    "SubscriptionStarted";
                public const string SUBSCRIPTION_FAILURE =
                    "SubscriptionFailure";
                public const string SUBSCRIPTION_TERMINATED =
                    "SubscriptionTerminated";
            }
        }

        public static void Main(string[] args)
        {
            System.Console.WriteLine("Name Enumeration Example");
            NameEnumerationExample example = new NameEnumerationExample();
            example.run(args);
        }

        public NameEnumerationExample()
        {
            d_host = "localhost";
            d_port = 8194;
            d_sessionOptions = new SessionOptions();
            d_securities = new List<string>();
            d_options = new List<string>();
            d_subscriptions = new List<Subscription>();

            d_subscriptionDataMsgEnumTable = new NameEnumerationTable(
                new SubscriptionDataMsgType());
            d_subscriptionStatusMsgEnumTable = new NameEnumerationTable(
                new SubscriptionStatusMsgType());
        }

        private bool createSession()
        {
            if (d_session != null) d_session.Stop();

            System.Console.WriteLine("Connecting to " + d_host + ":" + d_port);
            d_session = new Session(d_sessionOptions, new EventHandler(processEvent));
            return d_session.Start();
        }

        private void run(string[] args)
        {
            if (!parseCommandLine(args)) return;

            d_sessionOptions.ServerHost = d_host;
            d_sessionOptions.ServerPort = d_port;

            bool success = createSession();
            if (!success) return;

            if (!d_session.OpenService(BLP_MKTDATA_SVC))
            {
                System.Console.Error.WriteLine("Failed to open service: " +
                    BLP_MKTDATA_SVC);
                d_session.Stop();
                return;
            }

            System.Console.WriteLine("Subscribing...");
            d_session.Subscribe(d_subscriptions);

            // wait for enter key to exit application
            System.Console.Read();

            d_session.Stop();
            System.Console.WriteLine("Exiting.");
        }

        public void processEvent(Event eventObj, Session session)
        {
            try
            {
                switch (eventObj.Type)
                {
                    case Event.EventType.SUBSCRIPTION_DATA:
                        processSubscriptionDataEvent(eventObj, session);
                        break;
                    case Event.EventType.SUBSCRIPTION_STATUS:
                        processSubscriptionStatus(eventObj, session);
                        break;
                    default:
                        System.Console.WriteLine("Processing event: " +
                            eventObj.Type);
                        printEvent(eventObj, session);
                        break;
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }

        private void processSubscriptionStatus(Event eventObj, Session session)
        {
            System.Console.Out.WriteLine("Processing SUBSCRIPTION_STATUS");
            foreach (Message msg in eventObj)
            {
                string topic = (string)msg.CorrelationID.Object;
                switch (d_subscriptionStatusMsgEnumTable[msg.MessageType])
                {
                    case SubscriptionStatusMsgType.SUBSCRIPTION_STARTED:
                    {
                        System.Console.Out.WriteLine("Subscription for: " +
                            topic + " started");
                    } break;


                    case SubscriptionStatusMsgType.SUBSCRIPTION_FAILURE:
                    {
                        System.Console.Out.WriteLine("Subscription for: " +
                            topic + " failed");
                        printEvent(eventObj, session);
                    } break;


                    case SubscriptionStatusMsgType.SUBSCRIPTION_TERMINATED:
                    {
                        System.Console.Out.WriteLine("Subscription for: " +
                            topic + " has been terminated");
                        printEvent(eventObj, session);
                    } break;

                    default:
                        System.Console.Out.WriteLine(
                            "Unhandled subscription status: " + msg.MessageType);
                        break;

                }
            }
        }
        
        private void processSubscriptionDataEvent(Event eventObj, Session session)
        {
            System.Console.WriteLine("Processing SUBSCRIPTION_DATA");
            foreach (Message msg in eventObj)
            {
                string topic = (string)msg.CorrelationID.Object;
                foreach (Element field in msg.Elements)
                {
                    switch (d_subscriptionDataMsgEnumTable[field.Name])
                    {
                        case SubscriptionDataMsgType.BID:
                        case SubscriptionDataMsgType.ASK:
                        case SubscriptionDataMsgType.LAST_PRICE:
                        {
                            System.Console.WriteLine(System.DateTime.Now.ToString("s")
                                + ": " + topic + " " + field.Name + " " +
                                field.GetValueAsString());
                        } break;
                    }
                }
            }
        }

        private void printEvent(Event eventObj, Session session)
        {
            System.Console.WriteLine("Processing " + eventObj.Type);
            foreach (Message msg in eventObj)
            {
                System.Console.WriteLine(msg);
            }
        }


        private bool parseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (string.Compare(args[i], "-s", true) == 0)
                {
                    d_securities.Add(args[i + 1]);
                }
                else if (string.Compare(args[i], "-o", true) == 0)
                {
                    d_options.Add(args[i + 1]);
                }
                else if (string.Compare(args[i], "-ip", true) == 0)
                {
                    d_host = args[i + 1];
                }
                else if (string.Compare(args[i], "-p", true) == 0)
                {
                    int outPort = 0;
                    if (int.TryParse(args[i + 1], out outPort))
                    {
                        d_port = outPort;
                    }
                }
                if (string.Compare(args[i], "-h", true) == 0)
                {
                    printUsage();
                    return false;
                }
            }

            if (d_securities.Count == 0)
            {
                d_securities.Add("IBM US Equity");
            }

            foreach (string security in d_securities)
            {
                d_subscriptions.Add(new Subscription(security,
                    "BID,ASK,LAST_PRICE", "", new CorrelationID(security)));
            }

            return true;
        }

        private void printUsage()
        {
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("	Name Enumeration Example");
            System.Console.WriteLine("		[-s			<security	= IBM US Equity>");
            System.Console.WriteLine("		[-ip 		<ipAddress	= localhost>");
            System.Console.WriteLine("		[-p 		<tcpPort	= 8194>");
            System.Console.WriteLine("Press ENTER to quit");
        }
    }
}