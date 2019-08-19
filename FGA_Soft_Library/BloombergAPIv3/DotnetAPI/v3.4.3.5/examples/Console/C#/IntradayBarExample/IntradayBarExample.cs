//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using DateTime = System.DateTime;
using DayOfWeek = System.DayOfWeek;
using Datetime = Bloomberglp.Blpapi.Datetime;
using Event = Bloomberglp.Blpapi.Event;
using Element = Bloomberglp.Blpapi.Element;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Request = Bloomberglp.Blpapi.Request;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;

namespace Bloomberglp.Blpapi.Examples
{

	public class IntradayBarExample
	{
		private static readonly Name BAR_DATA       = new Name("barData");
		private static readonly Name BAR_TICK_DATA  = new Name("barTickData");
		private static readonly Name OPEN 	        = new Name("open");
		private static readonly Name HIGH 	        = new Name("high");
		private static readonly Name LOW 	        = new Name("low");
		private static readonly Name CLOSE	        = new Name("close");
		private static readonly Name VOLUME	        = new Name("volume");
		private static readonly Name NUM_EVENTS     = new Name("numEvents");
		private static readonly Name TIME	        = new Name("time");
		private static readonly Name RESPONSE_ERROR = new Name("responseError");
		private static readonly Name CATEGORY       = new Name("category");
		private static readonly Name MESSAGE        = new Name("message");

		private string					d_host;
		private int						d_port;
		private string                  d_security;
		private string                  d_eventType;
		private int						d_barInterval;
		private bool					d_gapFillInitialBar;
		private string                  d_startDateTime;
		private string                  d_endDateTime;

		public static void Main(string[] args)
		{
			System.Console.WriteLine("Intraday Bars Example");
			IntradayBarExample example = new IntradayBarExample();
			example.run(args);

			System.Console.WriteLine("Press ENTER to quit");
			System.Console.Read();
		}

        private DateTime getPreviousTradingDate()
        {
            DateTime tradedOn = DateTime.Now;
            tradedOn = tradedOn.AddDays(-1);
            if (tradedOn.DayOfWeek == DayOfWeek.Sunday)
            {
                tradedOn = tradedOn.AddDays(-2);
            }
            else if (tradedOn.DayOfWeek == DayOfWeek.Saturday)
            {
                tradedOn = tradedOn.AddDays(-1);
            }
            return tradedOn;
        }

		public IntradayBarExample()
		{
			d_host = "localhost";
			d_port = 8294;
			d_barInterval = 60;
			d_security = "IBM US Equity";
			d_eventType = "TRADE";
			d_gapFillInitialBar = false;
            DateTime prevTradedDate = getPreviousTradingDate();
            d_startDateTime = prevTradedDate.Year.ToString() + "-" +
                              prevTradedDate.Month.ToString() + "-" +
                              prevTradedDate.Day.ToString() +
                              "T13:30:00";
            prevTradedDate = prevTradedDate.AddDays(1); // next day for end date
            d_endDateTime = prevTradedDate.Year.ToString() + "-" +
                              prevTradedDate.Month.ToString() + "-" +
                              prevTradedDate.Day.ToString() +
                              "T13:30:00";        
		}

		private void run(string[] args)
		{
			if (!parseCommandLine(args)) return;

			SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = d_host;
            sessionOptions.ServerPort = d_port;		

			System.Console.WriteLine("Connecting to " + d_host + ":" + d_port);
			Session session = new Session(sessionOptions);
			bool sessionStarted = session.Start();
			if (!sessionStarted) 
			{
				System.Console.Error.WriteLine("Failed to start session.");
				return;
			}
			
			sendIntradayBarRequest(session);

			// wait for events from session.
			eventLoop(session);

			session.Stop();
		}

		private void eventLoop(Session session)
		{
			bool done = false;
			while (!done) 
			{
				Event eventObj = session.NextEvent();
				if (eventObj.Type == Event.EventType.PARTIAL_RESPONSE) 
				{
					System.Console.WriteLine("Processing Partial Response");
					processResponseEvent(eventObj, session);
				}
				else if (eventObj.Type == Event.EventType.RESPONSE) 
				{
					System.Console.WriteLine("Processing Response");
					processResponseEvent(eventObj, session);
					done = true;
				} 
				else 
				{
                    foreach (Message msg in eventObj)
					{						
						System.Console.WriteLine(msg.AsElement);
						if (eventObj.Type == Event.EventType.SESSION_STATUS) 
						{
							if (msg.MessageType.Equals("SessionTerminated")) 
							{							
								done = true;
							}
						}
					}
				}
			}
		}

      private void processMessage(Message msg)
      {
          Element data = msg.GetElement(BAR_DATA).GetElement(BAR_TICK_DATA);
          int numBars = data.NumValues;
          System.Console.WriteLine("Response contains " + numBars + " bars");
          System.Console.WriteLine("Datetime\t\tOpen\t\tHigh\t\tLow\t\tClose" +
              "\t\tNumEvents\tVolume");
          for (int i = 0; i < numBars; ++i)
          {
              Element bar = data.GetValueAsElement(i);
              Datetime time = bar.GetElementAsDate(TIME);
              double open = bar.GetElementAsFloat64(OPEN);
              double high = bar.GetElementAsFloat64(HIGH);
              double low = bar.GetElementAsFloat64(LOW);
              double close = bar.GetElementAsFloat64(CLOSE);
              int numEvents = bar.GetElementAsInt32(NUM_EVENTS);
              long volume = bar.GetElementAsInt64(VOLUME);
              System.DateTime sysDatetime = time.ToSystemDateTime();
              System.Console.WriteLine(
                  sysDatetime.ToString("s") + "\t" +
                  open.ToString("C") + "\t\t" +
                  high.ToString("C") + "\t\t" +
                  low.ToString("C") + "\t\t" +
                  close.ToString("C") + "\t\t" +
                  numEvents + "\t\t" +
                  volume);
          }
      }


		// return true if processing is completed, false otherwise
		private void processResponseEvent(Event eventObj, Session session)
		{
            foreach (Message msg in eventObj)
			{
                processMessage(msg); 
			}
		}

		private void sendIntradayBarRequest(Session session)
		{
            session.OpenService("//blp/refdata");
            Service refDataService = session.GetService("//blp/refdata");
			Request request =  refDataService.CreateRequest("IntradayBarRequest");

			// only one security/eventType per request
			request.Set("security", d_security);
			request.Set("eventType", d_eventType);
			request.Set("interval", d_barInterval);

			request.Set("startDateTime", d_startDateTime);
			request.Set("endDateTime", d_endDateTime);

			if (d_gapFillInitialBar) 
			{
				request.Set("gapFillInitialBar", d_gapFillInitialBar);
			}

			System.Console.WriteLine("Sending Request: " + request);
			session.SendRequest(request, null);
		}

		private bool parseCommandLine(string[] args)
		{
            bool flag = true;
            string dateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
			for (int i = 0; i < args.Length; ++i) 
			{
				if (string.Compare(args[i], "-s", true) == 0) 
				{
					d_security = args[i+1];
				}
				else if (string.Compare(args[i],"-e", true) == 0) 
				{
					d_eventType = args[i+1];
				}
				else if (string.Compare(args[i], "-ip", true) == 0) 
				{
					d_host = args[i+1];
				}
				else if (string.Compare(args[i],"-p", true) == 0) 
				{
                    int outPort = 0;
                    if (int.TryParse(args[i + 1], out outPort))
                    {
                        d_port = outPort;
                    }
                }
				else if (string.Compare(args[i],"-b", true) == 0) 
				{
					d_barInterval = int.Parse(args[i+i]);
				}
                else if (string.Compare(args[i], "-g", true) == 0)
                {
                    d_gapFillInitialBar = true;
                }
                else if (string.Compare(args[i], "-sd", true) == 0)
                {
                    d_startDateTime = args[i + 1];

                    if (!isDateTimeValid(d_startDateTime, dateTimeFormat))
                    {
                        flag = false;
                        System.Console.WriteLine("Invalid start date/time: " + d_startDateTime + ".");
                    }
                }
                else if (string.Compare(args[i], "-ed", true) == 0)
                {
                    d_endDateTime = args[i + 1];

                    if (!isDateTimeValid(d_endDateTime, dateTimeFormat))
                    {
                        flag = false;
                        System.Console.WriteLine("Invalid end date/time: " + d_endDateTime + ".");
                    }
                }
                else if (string.Compare(args[i], "-h", true) == 0)
                {
					printUsage();
					return false;
				}
			}

            return flag;
		}

        /// <summary>
        /// Validate if date or date/time is valid
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool isDateTimeValid(string dateTime, string format)
        {
            DateTime outDateTime;
            System.IFormatProvider formatProvider = new System.Globalization.CultureInfo("en-US", true);
            return DateTime.TryParseExact(dateTime, format, formatProvider,
                System.Globalization.DateTimeStyles.AllowWhiteSpaces, out outDateTime);
        }

		private void printErrorInfo(string leadingStr, Element errorInfo)    
		{
			System.Console.WriteLine(leadingStr + errorInfo.GetElementAsString(CATEGORY) +
				" (" + errorInfo.GetElementAsString(MESSAGE) + ")");
		}

		private void printUsage()
		{
			System.Console.WriteLine("Usage:");
			System.Console.WriteLine("	Retrieve intraday bars");
			System.Console.WriteLine("		[-s		<security	= IBM US Equity>");
			System.Console.WriteLine("		[-e		<event		= TRADE>");
			System.Console.WriteLine("		[-b		<barInterval= 60>");
			System.Console.WriteLine("      [-sd    <startDateTime  = 2008-08-11T13:30:00>");
			System.Console.WriteLine("      [-ed    <endDateTime    = 2008-08-12T13:30:00>");
			System.Console.WriteLine("      [-g     <gapFillInitialBar = false>");
			System.Console.WriteLine("		[-ip    <ipAddress	= localhost>");
			System.Console.WriteLine("		[-p     <tcpPort	= 8194>");
			System.Console.WriteLine("1) All times are in GMT.");
			System.Console.WriteLine("2) Only one security can be specified.");
			System.Console.WriteLine("3) Only one event can be specified.");
		}
	}
}