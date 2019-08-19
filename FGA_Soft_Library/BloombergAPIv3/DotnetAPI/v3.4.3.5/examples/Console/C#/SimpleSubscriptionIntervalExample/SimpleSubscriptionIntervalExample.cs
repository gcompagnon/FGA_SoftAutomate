//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using Event = Bloomberglp.Blpapi.Event;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Session = Bloomberglp.Blpapi.Session;

namespace Bloomberglp.Blpapi.Examples
{

	public class SimpleSubscriptionIntervalExample
	{
		/**
		 * @param args
		 */
		public static void Main(string[] args)
		{
			SimpleSubscriptionIntervalExample example =
				new SimpleSubscriptionIntervalExample();
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

			System.Console.WriteLine("Connecting to " + serverHost + ":" + serverPort);
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

            System.Collections.Generic.List<Subscription> subscriptions
               = new System.Collections.Generic.List<Subscription>();

			// subscribe with a 1 second interval
            string security = "IBM US Equity";
            subscriptions.Add(new Subscription(
                                security,
                                "LAST_PRICE,BID,ASK",
                                "interval=1.0",
                                new CorrelationID(security))
                              );
			session.Subscribe(subscriptions);

			while (true) 
			{
				Event eventObj = session.NextEvent();				
				foreach (Message msg in eventObj) 
				{
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
