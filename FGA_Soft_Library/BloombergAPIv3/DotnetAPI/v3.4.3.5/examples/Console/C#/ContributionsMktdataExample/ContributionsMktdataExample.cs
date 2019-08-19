using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using Bloomberglp.Blpapi;


namespace ContributionsMktdataExample
{
	class ContributionsMktdataExample
	{
		private const String AUTH_USER = "AuthenticationType=OS_LOGON";
		private const String AUTH_APP_PREFIX = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=";
		private const String AUTH_DIR_PREFIX = "AuthenticationType=DIRECTORY_SERVICE;DirSvcPropertyName=";
		private const String AUTH_OPTION_NONE = "none";
		private const String AUTH_OPTION_USER = "user";
		private const String AUTH_OPTION_APP = "app=";
		private const String AUTH_OPTION_DIR = "dir=";

		private static readonly Name BID = Name.GetName("BID");
		private static readonly Name ASK = Name.GetName("ASK");
		private static readonly Name BID_SIZE = Name.GetName("BID_SIZE");
		private static readonly Name ASK_SIZE = Name.GetName("ASK_SIZE");
		private static readonly Name AUTHORIZATION_SUCCESS = Name.GetName("AuthorizationSuccess");
		private static readonly Name TOKEN_SUCCESS = Name.GetName("TokenGenerationSuccess");

		private String d_serverHost;
		private int d_serverPort;
		private String d_serviceName;
		private int d_maxEvents = 100;
		private String d_authOptions;

		public ContributionsMktdataExample()
		{
			d_serviceName = "//blp/mpfbapi";
			d_serverPort = 8194;
			d_serverHost = "localhost";
			d_authOptions = AUTH_USER;
		}

		public void Run(String[] args)
		{
			if (!ParseCommandLine(args))
			{
				return;
			}

			SessionOptions sessionOptions = new SessionOptions();
			sessionOptions.ServerHost = d_serverHost;
			sessionOptions.ServerPort = d_serverPort;
			sessionOptions.AuthenticationOptions = d_authOptions;

			System.Console.WriteLine("Connecting to " + d_serverHost + ":" + d_serverPort);
			ProviderSession session = new ProviderSession(sessionOptions, ProcessEvent);

			if (!session.Start())
			{
				Console.Error.WriteLine("Failed to start session");
				return;
			}

			Identity identity = null;
			if (d_authOptions.Length != 0)
			{
				if (!Authorize(out identity, session))
				{
					return;
				}
			}

			TopicList topicList = new TopicList();
			topicList.Add(
				d_serviceName + "/ticker/929903DF6 Corp",
				new CorrelationID(new MyStream("AUDEUR Curncy")));
			topicList.Add(
				d_serviceName + "/ticker/EC070336 Corp",
				new CorrelationID(new MyStream("EC070336 Corp")));
			topicList.Add(
				d_serviceName + "/ticker/6832348A9 Corp",
				new CorrelationID(new MyStream("6832348A9 Corp")));

			session.CreateTopics(
				topicList,
				ResolveMode.AUTO_REGISTER_SERVICES,
				identity);

			Service service = session.GetService(d_serviceName);
			if (service == null)
			{
				System.Console.Error.WriteLine("Open service failed: " + d_serviceName);
				return;
			}

			List<MyStream> myStreams = new List<MyStream>();
			for (int i = 0; i < topicList.Size; ++ i) {
			if (topicList.StatusAt(i) == TopicList.TopicStatus.CREATED)
			{
				Topic topic = session.GetTopic(topicList.MessageAt(i));
				MyStream stream = (MyStream)topicList.CorrelationIdAt(i).Object;
				stream.SetTopic(topic);
				myStreams.Add(stream);
			}
			}

			int iteration = 0;
			while (iteration++ < d_maxEvents)
			{
				Event eventObj = service.CreatePublishEvent();
				EventFormatter eventFormatter = new EventFormatter(eventObj);

				foreach (MyStream stream in myStreams)
				{
					eventFormatter.AppendMessage("MarketData", stream.GetTopic());
					eventFormatter.SetElement(BID, stream.GetBid());
					eventFormatter.SetElement(ASK, stream.GetAsk());
					eventFormatter.SetElement(BID_SIZE, 1200);
					eventFormatter.SetElement(ASK_SIZE, 1400);
				}

				System.Console.WriteLine(System.DateTime.Now.ToString() + " -");

				foreach (Message msg in eventObj)
				{
					System.Console.WriteLine(msg);
				}

				session.Publish(eventObj);
				Thread.Sleep(10 * 1000);
			}

			session.Stop();
		}

		public static void Main(String[] args)
		{
			ContributionsMktdataExample example = new ContributionsMktdataExample();
			example.Run(args);
		}

		private void ProcessEvent(Event eventObj, ProviderSession session)
		{
			if (eventObj == null)
			{
				Console.WriteLine("Received null event ");
				return;
			}

			Console.WriteLine("Received event " + eventObj.Type.ToString());
			foreach (Message msg in eventObj)
			{
				Console.WriteLine("Message = " + msg);
			}

			//TO DO Process event if needed.
		}

		#region private helper method

		private class MyStream
		{
			private String d_id;
			private Topic d_topic;
			private static Random d_market = new Random(System.DateTime.Now.Millisecond);
			private double d_lastValue;

			public Topic GetTopic()
			{
				return d_topic;
			}

			public void SetTopic(Topic topic)
			{
				d_topic = topic;
			}

			public String GetId()
			{
				return d_id;
			}

			public MyStream(String id)
			{
				d_id = id;
				d_topic = null;
				d_lastValue = d_market.NextDouble() * 100;
			}

			public void Next()
			{
				double delta = d_market.NextDouble();
				if (d_lastValue + delta < 1.0)
					delta = d_market.NextDouble();
				d_lastValue += delta;
			}

			public double GetAsk()
			{
				return Math.Round(d_lastValue * 101) / 100.0;
			}

			public double GetBid()
			{
				return Math.Round(d_lastValue * 98) / 100.0;
			}
		}

		private bool Authorize(out Identity identity, ProviderSession session)
		{
			identity = null;
			EventQueue tokenEventQueue = new EventQueue();
			session.GenerateToken(new CorrelationID(tokenEventQueue), tokenEventQueue);
			String token = null;
			const int timeoutMilliSeonds = 10000;
			Event eventObj = tokenEventQueue.NextEvent(timeoutMilliSeonds);
			if (eventObj.Type == Event.EventType.TOKEN_STATUS)
			{
				foreach (Message msg in eventObj)
				{
					System.Console.WriteLine(msg.ToString());
					if (msg.MessageType == TOKEN_SUCCESS)
					{
						token = msg.GetElementAsString("token");
					}
				}
			}
			if (token == null)
			{
				System.Console.WriteLine("Failed to get token");
				return false;
			}

			const string authServiceName = "//blp/apiauth";
			if (session.OpenService(authServiceName))
			{
				Service authService = session.GetService(authServiceName);
				Request authRequest = authService.CreateAuthorizationRequest();
				authRequest.Set("token", token);

				EventQueue authEventQueue = new EventQueue();
				identity = session.CreateIdentity();
				session.SendAuthorizationRequest(authRequest, identity, authEventQueue, new CorrelationID(identity));

				eventObj = authEventQueue.NextEvent();
				if (eventObj.Type == Event.EventType.RESPONSE
					|| eventObj.Type == Event.EventType.PARTIAL_RESPONSE
					|| eventObj.Type == Event.EventType.REQUEST_STATUS)
				{
					foreach (Message msg in eventObj)
					{
						System.Console.WriteLine(msg.ToString());
						if (msg.MessageType == AUTHORIZATION_SUCCESS)
						{
							return true;
						}
						else
						{
							return false ;
						}
					}
				}
			}
			return false;
		}

		private void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  Contribute market data to a topic");
			Console.WriteLine("     [-ip        <ipAddress = localhost>");
			Console.WriteLine("     [-p         <tcpPort   = 8194>");
			Console.WriteLine("     [-s         <service   = //viper/mktdata>]");
			Console.WriteLine("     [-me        <maxEvents>  max number of events (default = " + d_maxEvents + ")");
			Console.WriteLine("     [-auth      <option    = user> (user|none|app={app}|dir={property})](default = " + AUTH_OPTION_USER + ")");

			Console.WriteLine("Press ENTER to quit");
		}

		private bool ParseCommandLine(String[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				if (string.Compare("-s", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_serviceName = args[++i];
				}
				else if (string.Compare("-ip", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_serverHost = args[++i];
				}
				else if (string.Compare("-p", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_serverPort = int.Parse(args[++i]);
				}
				else if (string.Compare("-me", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_maxEvents = int.Parse(args[++i]);
				}
				else if (string.Compare("-auth", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					++i;
					if (string.Compare(AUTH_OPTION_NONE, args[i], true) == 0)
					{
						d_authOptions = "";
					}
					else if (string.Compare(AUTH_OPTION_USER, args[i], true)
																		== 0)
					{
						d_authOptions = AUTH_USER;
					}
					else if (string.Compare(AUTH_OPTION_APP, 0, args[i], 0,
											AUTH_OPTION_APP.Length, true) == 0)
					{
						d_authOptions = AUTH_APP_PREFIX
								+ args[i].Substring(AUTH_OPTION_APP.Length);
					}
					else if (string.Compare(AUTH_OPTION_DIR, 0, args[i], 0,
											AUTH_OPTION_DIR.Length, true) == 0)
					{
						d_authOptions = AUTH_DIR_PREFIX
								+ args[i].Substring(AUTH_OPTION_DIR.Length);
					}
					else
					{
						PrintUsage();
						return false;
					}
				}
				else
				{
					PrintUsage();
					return false;
				}
			}

			return true;
		}

		#endregion

	}

}
