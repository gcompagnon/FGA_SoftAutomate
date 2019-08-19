//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using Event = Bloomberglp.Blpapi.Event;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;

namespace Bloomberglp.Blpapi.Examples
{
	public class SimpleSubscriptionExample
	{		
		public static void Main(string[] args)
		{
			SimpleSubscriptionExample example = new SimpleSubscriptionExample();
			example.run(args);
			System.Console.WriteLine("Press ENTER to quit");
			System.Console.Read();
		}

		private void run(string[] args)
		{
			string serverHost = "localhost";
			int serverPort = 8194;

			SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = serverHost;
            sessionOptions.ServerPort = serverPort;

			System.Console.WriteLine("Connecting to " + serverHost + 
                ":" + serverPort);
			Session session = new Session(sessionOptions);
			bool sessionStarted = session.Start();
			if (!sessionStarted) 
			{
				System.Console.Error.WriteLine("Failed to start session.");
				return;
			}
            if (!session.OpenService("//blp/mktdata"))
            {
                System.Console.Error.WriteLine("Failed to open //blp/mktdata");
                return;
            }
 
            string security1 = "IBM US Equity";
            string security2 = "/cusip/912828GM6@BGN";

            System.Collections.Generic.List<Subscription> subscriptions
                = new System.Collections.Generic.List<Subscription>();

            subscriptions.Add(new Subscription(
                                security1,
                                "LAST_PRICE,BID,ASK,PX_LAST", "",
                                new CorrelationID(security1))
                              );
            subscriptions.Add(new Subscription(
                                security2, 
                                "LAST_PRICE,BID,ASK,BID_YIELD,ASK_YIELD", "",
                                new CorrelationID(security2))
                              );
			session.Subscribe(subscriptions);

			while (true) 
			{
				Event eventObj = session.NextEvent();				
				foreach (Message msg in eventObj) 
				{
                    for (int i = 0; i < msg.NumElements; i++ )
                    {
                        System.Console.WriteLine(msg.AsElement.GetElement(i));
                    }
                    if (eventObj.Type == Event.EventType.SUBSCRIPTION_DATA ||
                        eventObj.Type == Event.EventType.SUBSCRIPTION_STATUS)
                    {
                        string topic = (string)msg.CorrelationID.Object;
                        System.Console.WriteLine(topic + ": " + msg.AsElement);
                    }
                    else
                    {
                        System.Console.WriteLine(msg.AsElement);
                    }
				}
			}
		}
	}
}
