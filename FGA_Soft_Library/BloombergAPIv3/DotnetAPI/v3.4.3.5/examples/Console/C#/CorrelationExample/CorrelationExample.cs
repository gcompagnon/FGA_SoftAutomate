//--------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.'
// --------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

using CorrelationID = Bloomberglp.Blpapi.CorrelationID;
using Event = Bloomberglp.Blpapi.Event;
using Message = Bloomberglp.Blpapi.Message;
using Request = Bloomberglp.Blpapi.Request;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;

namespace Bloomberglp.Blpapi.Examples
{
    /**
     * An example to demonstrate use of CorrelationID.
     */
    class CorrelationExample
    {
        private Session d_session;
        private SessionOptions d_sessionOptions;
        private Service d_refDataService;
        private Window d_secInfoWindow;

        /**
        * A helper class to simulate a GUI window.
        */
        public class Window
        {
            private String d_name;

            public Window(String name)
            {
                d_name = name;
            }

            public void displaySecurityInfo(Message msg)
            {
                System.Console.WriteLine(d_name + ": " + msg);
            }
        }

        public static void Main(String[] args)
        {
            System.Console.WriteLine("CorrelationExample");
            CorrelationExample example = new CorrelationExample();
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

        public CorrelationExample()
        {
            d_sessionOptions = new SessionOptions();
            d_sessionOptions.ServerHost = "localhost";
            d_sessionOptions.ServerPort = 8194;
            d_secInfoWindow = new Window("SecurityInfo");
        }

        private void run(String[] args)
        {
            if (!createSession()) return;

            Request request = d_refDataService.CreateRequest(
                "ReferenceDataRequest");
            request.GetElement("securities").AppendValue("IBM US Equity");
            request.GetElement("fields").AppendValue("PX_LAST");
            request.GetElement("fields").AppendValue("DS002");

            d_session.SendRequest(request, new CorrelationID(d_secInfoWindow));

            while (true)
            {
                Event eventObj = d_session.NextEvent();
                foreach (Message msg in eventObj)
                {
                    if (eventObj.Type == Event.EventType.RESPONSE ||
                        eventObj.Type == Event.EventType.PARTIAL_RESPONSE)
                    {
                        ((Window)(msg.CorrelationID.Object)).
                            displaySecurityInfo(msg);
                    }
                }
                if (eventObj.Type == Event.EventType.RESPONSE)
                {
                    // received final response
                    break;
                }
            }
        }


        private bool createSession()
        {
            System.Console.WriteLine("Connecting to " +
                                d_sessionOptions.ServerHost
                                + ":" + d_sessionOptions.ServerPort);
            d_session = new Session(d_sessionOptions);
            if (!d_session.Start())
            {
                System.Console.WriteLine("Failed to connect!");
                return false;
            }
            if (!d_session.OpenService("//blp/refdata"))
            {
                System.Console.WriteLine("Failed to open //blp/refdata");
                d_session.Stop();
                d_session = null;
                return false;
            }
            d_refDataService = d_session.GetService("//blp/refdata");
            return true;
        }
    }
}
