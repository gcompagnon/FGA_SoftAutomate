using System;
using System.Collections.Generic;
using System.Text;
using Bloomberglp.Blpapi;
using System.IO;

namespace MktdataSubscriptionExample
{
	public class MktdataSubscriptionExample
	{
		private const String AUTH_USER = "AuthenticationType=OS_LOGON";
		private const String AUTH_APP_PREFIX = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=";
		private const String AUTH_DIR_PREFIX = "AuthenticationType=DIRECTORY_SERVICE;DirSvcPropertyName=";
		private const String AUTH_OPTION_NONE = "none";
		private const String AUTH_OPTION_USER = "user";
		private const String AUTH_OPTION_APP = "app=";
		private const String AUTH_OPTION_DIR = "dir=";

		private Name AUTHORIZATION_SUCCESS = Name.GetName("AuthorizationSuccess");
		private Name TOKEN_SUCCESS = Name.GetName("TokenGenerationSuccess");

		private const String d_defaultHost      = "localhost";
		private const int    d_defaultPort      = 8194;
		private const String d_defaultService   = "//viper/mktdata";
		private const int    d_defaultMaxEvents = int.MaxValue;

		private List<String>   d_hosts = new List<String>();
		private int    d_port       = d_defaultPort;
		private String d_service    = d_defaultService;
		private int    d_maxEvents  = d_defaultMaxEvents;
		private String d_authOptions = AUTH_USER;

		public MktdataSubscriptionExample()
		{
		}

		public void Run(String[] args)
		{
			if (!ParseCommandLine(args)) return;

			SessionOptions sessionOptions = new SessionOptions();
			SessionOptions.ServerAddress[] servers = new SessionOptions.ServerAddress[d_hosts.Count];
			for (int i = 0; i < d_hosts.Count; ++i)
			{
				servers[i] = new SessionOptions.ServerAddress(d_hosts[i], d_port);
			}
			sessionOptions.ServerAddresses = servers;
			sessionOptions.AutoRestartOnDisconnection = true;
			sessionOptions.NumStartAttempts = d_hosts.Count;
			sessionOptions.DefaultSubscriptionService = d_service;
			sessionOptions.DefaultTopicPrefix = ""; // normally defaults to "ticker"
			sessionOptions.AuthenticationOptions = d_authOptions;

			System.Console.WriteLine("Connecting to port " + d_port + " on ");
			foreach (string host in d_hosts)
			{
				System.Console.WriteLine(host + " ");
			}
			Session session = new Session(sessionOptions);

			if (!session.Start())
			{
				System.Console.Error.WriteLine("Failed to start session.");
				return;
			}

			Identity identity = null;
			if (d_authOptions.Length != 0)
			{
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
					System.Environment.Exit(1);
				}

				if (session.OpenService("//blp/apiauth"))
				{
					Service authService = session.GetService("//blp/apiauth");
					Request authRequest = authService.CreateAuthorizationRequest();
					authRequest.Set("token", token);

					EventQueue authEventQueue = new EventQueue();
					identity = session.CreateIdentity();
					session.SendAuthorizationRequest(authRequest, identity, authEventQueue, new CorrelationID(identity));

					bool isAuthorized = false;
					while (!isAuthorized)
					{
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
									isAuthorized = true;
									break;
								}
								else
								{
									System.Console.Error.WriteLine("Not authorized: " + msg);
									System.Environment.Exit(1);
								}
							}
						}
					}
				}
			}

			List<Subscription> subscriptions = new List<Subscription>();

			// Short topic string; will get qualified by subscribe() using session
			// options: 'DefaultSubscriptionService' and 'DefaultTopicPrefix'
			// configured above
			String topic1 = "IBM Equity";

			// Fully qualified topic
			String topic2 = d_service + "/RHAT Equity";

			// Add a topic on unknown service to demonstrate event sequence
			String topic3 = d_service + "-unknown/RHAT Equity";

			subscriptions.Add(new Subscription(topic1, new CorrelationID(topic1)));
			subscriptions.Add(new Subscription(topic2, new CorrelationID(topic2)));
			subscriptions.Add(new Subscription(topic3, new CorrelationID(topic3)));
			session.Subscribe(subscriptions, identity);

			ProcessSubscriptionResponse(session);

		}
	
		private void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine(" [-ip    <ipAddress  server name or IP    (default = " + d_defaultHost + ")>]");
			Console.WriteLine(" [-p     <tcpPort    server port          (default = " + d_defaultPort + ")>]");
			Console.WriteLine(" [-s     <service    service name         (default = " + d_defaultService + ">]");
			Console.WriteLine(" [-me    <maxEvents  max number of events (default = " + d_defaultMaxEvents + ")>]");
			Console.WriteLine(" [-auth  <option     authorization option (user|none|app={app}|dir={property})	(default = " + AUTH_OPTION_USER +")>]");
		}

		private bool ParseCommandLine(String[] args)
		{
			try
			{
				for (int i = 0; i < args.Length; ++i)
				{
					if (string.Compare("-s", args[i], true) == 0
						&& i + 1 < args.Length)
					{
						d_service = args[++i];
					}
					else if (string.Compare("-ip", args[i], true) == 0
						&& i + 1 < args.Length)
					{
						d_hosts.Add(args[++i]);
					}
					else if (string.Compare("-p", args[i], true) == 0
						&& i + 1 < args.Length)
					{
						d_port = int.Parse(args[++i]);
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
			}
			catch (Exception)
			{
				PrintUsage();
				return false;
			}

			return true;
		}

		private void ProcessSubscriptionResponse(Session session)
		{
			int eventCount = 0;
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

				if (eventObj.Type == Event.EventType.SUBSCRIPTION_DATA)
				{
					if (++eventCount >= d_maxEvents)
					{
						break;
					}
				}
			}
		}

		public static void Main(String[] args)
		{
			MktdataSubscriptionExample example = new MktdataSubscriptionExample();
			try
			{
				example.Run(args);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
	}

}
