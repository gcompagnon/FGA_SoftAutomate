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
using Session = Bloomberglp.Blpapi.Session;

using ArrayList = System.Collections.ArrayList;
using Thread = System.Threading.Thread;
using System.Collections.Generic;

namespace Bloomberglp.Blpapi.Examples
{

    public class SubscriptionWithEventHandlerExample
    {
        private static readonly Name EXCEPTIONS = new Name("exceptions");
        private static readonly Name FIELD_ID = new Name("fieldId");
        private static readonly Name REASON = new Name("reason");
        private static readonly Name CATEGORY = new Name("category");
        private static readonly Name DESCRIPTION = new Name("description");

        private string d_host;
        private int d_port;
        private SessionOptions d_sessionOptions;
        private Session d_session;
        private List<string> d_securities;
        private List<string> d_fields;
        private List<string> d_options;
        private List<Subscription> d_subscriptions;

        public static void Main(string[] args)
        {
            System.Console.WriteLine("Realtime Event Handler Example");
            SubscriptionWithEventHandlerExample example = new SubscriptionWithEventHandlerExample();
            example.run(args);
        }

        public SubscriptionWithEventHandlerExample()
        {
            d_host = "localhost";
            d_port = 8194;
            d_sessionOptions = new SessionOptions();
            d_securities = new List<string>();
            d_fields = new List<string>();
            d_options = new List<string>();
            d_subscriptions = new List<Subscription>();
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

            System.Console.WriteLine("Connected successfully\n");

            if (!d_session.OpenService("//blp/mktdata"))
            {
                System.Console.Error.WriteLine("Failed to open service //blp/mktdata");
                d_session.Stop();
                return;
            }

            System.Console.WriteLine("Subscribing...\n");
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
                        processMiscEvents(eventObj, session);
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
            System.Console.WriteLine("Processing SUBSCRIPTION_STATUS");
            foreach (Message msg in eventObj)
            {
                string topic = (string)msg.CorrelationID.Object;
                System.Console.WriteLine(System.DateTime.Now.ToString("s") +
                    ": " + topic + " - " + msg.MessageType);

                if (msg.HasElement(REASON))
                {
                    // This can occur on SubscriptionFailure.
                    Element reason = msg.GetElement(REASON);
                    System.Console.WriteLine("\t" +
                            reason.GetElement(CATEGORY).GetValueAsString() +
                            ": " + reason.GetElement(DESCRIPTION).GetValueAsString());
                }

                if (msg.HasElement(EXCEPTIONS))
                {
                    // This can occur on SubscriptionStarted if at least
                    // one field is good while the rest are bad.
                    Element exceptions = msg.GetElement(EXCEPTIONS);
                    for (int i = 0; i < exceptions.NumValues; ++i) {
                        Element exInfo = exceptions.GetValueAsElement(i);
                        Element fieldId = exInfo.GetElement(FIELD_ID);
                        Element reason = exInfo.GetElement(REASON);
                        System.Console.WriteLine("\t" + fieldId.GetValueAsString() +
                                ": " + reason.GetElement(CATEGORY).GetValueAsString());
                    }
                }
                System.Console.WriteLine("");
            }
        }

        private void processSubscriptionDataEvent(Event eventObj, Session session)
        {
            System.Console.WriteLine("Processing SUBSCRIPTION_DATA");
            foreach (Message msg in eventObj)
            {
                string topic = (string)msg.CorrelationID.Object;
                System.Console.WriteLine(System.DateTime.Now.ToString("s")
                    + ": " + topic + " - " + msg.MessageType);

                foreach (Element field in msg.Elements)
                {
                    if (field.IsNull)
                    {
                        System.Console.WriteLine("\t\t" + field.Name + " is NULL");
                        continue;
                    }

                    // Assume all values are scalar.
                    System.Console.WriteLine("\t\t" + field.Name
                        + " = " + field.GetValueAsString());
                }
            }
        }

        private void processMiscEvents(Event eventObj, Session session)
        {
            System.Console.WriteLine("Processing " + eventObj.Type);
            foreach (Message msg in eventObj)
            {
                System.Console.WriteLine(System.DateTime.Now.ToString("s") +
                    ": " + msg.MessageType + "\n");
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
                else if (string.Compare(args[i], "-f", true) == 0)
                {
                    d_fields.Add(args[i + 1]);
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

            if (d_fields.Count == 0)
            {
                d_fields.Add("LAST_PRICE");
                d_fields.Add("TIME");
            }

            if (d_securities.Count == 0)
            {
                d_securities.Add("IBM US Equity");
            }

            foreach (string security in d_securities)
                d_subscriptions.Add(new Subscription(
                    security, d_fields, d_options, new CorrelationID(security)));

            return true;
        }

        private void printUsage()
        {
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine("	Retrieve realtime data");
            System.Console.WriteLine("		[-s			<security	= IBM US Equity>");
            System.Console.WriteLine("		[-f			<field		= LAST_PRICE>");
            System.Console.WriteLine("		[-o			<subscriptionOptions>");
            System.Console.WriteLine("		[-ip 		<ipAddress	= localhost>");
            System.Console.WriteLine("		[-p 		<tcpPort	= 8194>");
            System.Console.WriteLine("Press ENTER to quit");
        }
    }
}