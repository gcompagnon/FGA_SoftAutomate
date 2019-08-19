//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using Element = Bloomberglp.Blpapi.Element;
using Event = Bloomberglp.Blpapi.Event;
using EventHandler = Bloomberglp.Blpapi.EventHandler;
using EventQueue = Bloomberglp.Blpapi.EventQueue;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Request = Bloomberglp.Blpapi.Request;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;
using Subscription = Bloomberglp.Blpapi.Subscription;

namespace Examples.src
{
    class SimpleBlockingRequestExample
    {
        private Name LAST_PRICE = new Name("LAST_PRICE");


        public static void Main(String[] args)
        {
            SimpleBlockingRequestExample example = new SimpleBlockingRequestExample();
            example.run(args);
            // wait for enter key to exit application
            System.Console.Read();
            System.Console.WriteLine("Exiting");
        }

        private void run(String[] args)
        {
            String serverHost = "localhost";
            int serverPort = 8194;

            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = serverHost;
            sessionOptions.ServerPort = serverPort;

            System.Console.WriteLine("Connecting to " + serverHost + ":" + serverPort);
            Session session = new Session(sessionOptions,
                new EventHandler(processEvent));
            if (!session.Start())
            {
                System.Console.Error.WriteLine("Failed to start session.");
                return;
            }
            if (!session.OpenService("//blp/mktdata"))
            {
                System.Console.Error.WriteLine("Failed to open //blp/mktdata");
                return;
            }
            if (!session.OpenService("//blp/refdata"))
            {
                System.Console.Error.WriteLine("Failed to open //blp/refdata");
                return;
            }

            System.Console.WriteLine("Subscribing to IBM US Equity");
            Subscription s = new Subscription("IBM US Equity", "LAST_PRICE", "");
            List<Subscription> subscriptions = new List<Subscription>();
            subscriptions.Add(s);
            session.Subscribe(subscriptions);

            System.Console.WriteLine("Requesting reference data IBM US Equity");
            Service refDataService = session.GetService("//blp/refdata");
            Request request = refDataService.CreateRequest("ReferenceDataRequest");
            request.GetElement("securities").AppendValue("IBM US Equity");
            request.GetElement("fields").AppendValue("DS002");

            EventQueue eventQueue = new EventQueue();
            session.SendRequest(request, eventQueue, null);
            while (true)
            {
                Event eventObj = eventQueue.NextEvent();

                foreach (Message msg in eventObj)
                {
                    System.Console.WriteLine(msg);
                }
                if (eventObj.Type == Event.EventType.RESPONSE)
                {
                    break;
                }
            }
        }

        public void processEvent(Event eventObj, Session session)
        {
            try
            {
                if (eventObj.Type == Event.EventType.SUBSCRIPTION_DATA)
                {
                    foreach (Message msg in eventObj)
                    {
                        if (msg.HasElement(LAST_PRICE))
                        {
                            Element field = msg.GetElement(LAST_PRICE);
                            System.Console.WriteLine(eventObj.Type
                                + ": " + field.Name +
                                " = " + field.GetValueAsString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }
        }
    }
}
