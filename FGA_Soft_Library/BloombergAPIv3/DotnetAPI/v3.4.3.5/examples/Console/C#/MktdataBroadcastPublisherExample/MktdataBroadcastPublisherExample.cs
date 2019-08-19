using System;
using System.Collections.Generic;
using System.Text;
using Bloomberglp.Blpapi;
using System.Collections;
using System.Threading;

namespace MktdataBroadcastPublisherExample
{
	class MktdataBroadcastPublisherExample
	{
		private const String AUTH_USER = "AuthenticationType=OS_LOGON";
		private const String AUTH_APP_PREFIX = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=";
		private const String AUTH_DIR_PREFIX = "AuthenticationType=DIRECTORY_SERVICE;DirSvcPropertyName=";
		private const String AUTH_OPTION_NONE = "none";
		private const String AUTH_OPTION_USER = "user";
		private const String AUTH_OPTION_APP = "app=";
		private const String AUTH_OPTION_DIR = "dir=";

		private Name AUTHORIZATION_SUCCESS = Name.GetName("AuthorizationSuccess");
		private Name AUTHORIZATION_FAILURE = Name.GetName("AuthorizationFailure");
		private Name TOKEN_SUCCESS = Name.GetName("TokenGenerationSuccess");

		private Name TOPIC_SUBSCRIBED = Name.GetName("TopicSubscribed");
		private Name TOPIC_UNSUBSCRIBED = Name.GetName("TopicUnsubscribed");
		private Name TOPIC_RECAP = Name.GetName("TopicRecap");
		private Name RESOLUTION_SUCCESS = Name.GetName("ResolutionSuccess");

		private string d_service = "//viper/mktdata";
		private int d_verbose = 0;
		private List<string> d_hosts = new List<string>();
		private int d_port = 8194;
		private int d_maxEvents = 100;

		private string d_authOptions = AUTH_USER;
		private List<string> d_topics = new List<string>();

		private string d_groupId = null;
		private int d_priority = int.MaxValue;

		class MyStream
		{
			String d_id;
			Topic d_topic;

			public MyStream()
			{
				d_id = "";
			}

			public MyStream(String id)
			{
				d_id = id;
			}

			public void setTopic(Topic topic)
			{
				d_topic = topic;
			}

			public String getId()
			{
				return d_id;
			}

			public Topic getTopic()
			{
				return d_topic;
			}
		}

		public void Run(String[] args)
		{
			if (!ParseCommandLine(args))
			{
				return;
			}

			SessionOptions.ServerAddress[] servers = new SessionOptions.ServerAddress[d_hosts.Count];
			for (int i = 0; i < d_hosts.Count; ++i)
			{
				servers[i] = new SessionOptions.ServerAddress(d_hosts[i], d_port);
			}

			SessionOptions sessionOptions = new SessionOptions();
			sessionOptions.ServerAddresses = servers;
			sessionOptions.AuthenticationOptions = d_authOptions;
			sessionOptions.AutoRestartOnDisconnection = true;
			sessionOptions.NumStartAttempts = servers.Length;

			System.Console.Write("Connecting to");
			foreach (SessionOptions.ServerAddress server in sessionOptions.ServerAddresses)
			{
				System.Console.Write(" " + server);
			}
			System.Console.WriteLine();

			ProviderSession session = new ProviderSession(sessionOptions, ProcessEvent);

			if (!session.Start())
			{
				System.Console.Error.WriteLine("Failed to start session");
				return;
			}

			Identity identity = null;
			if (d_authOptions != null)
			{
				if (!Authorize(out identity, session))
				{
					return;
				}
			}

			if (d_groupId != null)
			{
				// NOTE: will perform explicit service registration here, instead of letting
				//       createTopics do it, as the latter approach doesn't allow for custom
				//       ServiceRegistrationOptions
				ServiceRegistrationOptions serviceRegistrationOptions = new ServiceRegistrationOptions();
				serviceRegistrationOptions.GroupId = d_groupId;
				serviceRegistrationOptions.ServicePriority = d_priority;

				if (!session.RegisterService(d_service, identity, serviceRegistrationOptions))
				{
					System.Console.Write("Failed to register " + d_service);
					return;
				}
			}

			TopicList topicList = new TopicList();
			for (int i = 0; i < d_topics.Count; i++)
			{
				topicList.Add(
						d_service + "/" + d_topics[i],
						new CorrelationID(new MyStream(d_topics[i])));
			}

			session.CreateTopics(
					topicList,
					ResolveMode.AUTO_REGISTER_SERVICES,
					identity);
			// createTopics() is synchronous, topicList will be updated
			// with the results of topic creation (resolution will happen
			// under the covers)

			List<MyStream> myStreams = new List<MyStream>();

			for (int i = 0; i < topicList.Size; ++i)
			{
				MyStream stream = (MyStream)topicList.CorrelationIdAt(i).Object;
				if (topicList.StatusAt(i) == TopicList.TopicStatus.CREATED)
				{
					Message msg = topicList.MessageAt(i);
					stream.setTopic(session.GetTopic(msg));
					myStreams.Add(stream);
					System.Console.WriteLine("Topic created: " + topicList.TopicStringAt(i));
				}
				else
				{
					System.Console.WriteLine("Stream '" + stream.getId()
							+ "': topic not resolved, status = " + topicList.StatusAt(i));
				}
			}
			Service service = session.GetService(d_service);

			// Now we will start publishing
			Name eventName = Name.GetName("MarketDataEvents");
			Name high = Name.GetName("HIGH");
			Name low = Name.GetName("LOW");
			long tickCount = 1;
			for (int eventCount = 0; eventCount < d_maxEvents; ++eventCount)
			{
				Event eventObj = service.CreatePublishEvent();
				EventFormatter eventFormatter = new EventFormatter(eventObj);

				for (int index = 0; index < myStreams.Count; index++)
				{
					Topic topic = myStreams[index].getTopic();
					if (!topic.IsActive())
					{
						continue;
					}
					eventFormatter.AppendMessage(eventName, topic);
					if (1 == tickCount)
					{
						eventFormatter.SetElement("OPEN", 1.0);
					}
					else if (2 == tickCount)
					{
						eventFormatter.SetElement("BEST_BID", 3.0);
					}
					eventFormatter.SetElement(high, tickCount * 1.0);
					eventFormatter.SetElement(low, tickCount * 0.5);
					++tickCount;
				}

				foreach (Message msg in eventObj)
				{
					System.Console.WriteLine(msg);
				}

				session.Publish(eventObj);
				Thread.Sleep(2 * 1000);
			}

			session.Stop();
		}

		public static void Main(String[] args)
		{
			MktdataBroadcastPublisherExample example = new MktdataBroadcastPublisherExample();
			example.Run(args);
		}

		private void ProcessEvent(Event eventObj, ProviderSession session)
		{
			if (d_verbose > 0)
			{
				if (d_verbose > 1)
				{
					Console.WriteLine("Received event " + eventObj.Type);
				}
				foreach (Message msg in eventObj)
				{
					if (msg.CorrelationID != null && d_verbose > 1)
					{
						Console.WriteLine("cid = " + msg.CorrelationID);
					}
					Console.WriteLine("Message = " + msg);
				}
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

			if (session.OpenService("//blp/apiauth"))
			{
				Service authService = session.GetService("//blp/apiauth");
				Request authRequest = authService.CreateAuthorizationRequest();
				authRequest.Set("token", token);

				EventQueue authEventQueue = new EventQueue();
				identity = session.CreateIdentity();
				session.SendAuthorizationRequest(authRequest, identity, authEventQueue, new CorrelationID(identity));

				while (true)
				{
					eventObj = authEventQueue.NextEvent();
					if (eventObj.Type == Event.EventType.RESPONSE ||
						eventObj.Type == Event.EventType.PARTIAL_RESPONSE ||
						eventObj.Type == Event.EventType.REQUEST_STATUS)
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
								System.Console.WriteLine("Not authorized: " + msg);
								return false;
							}
						}
					}
				}
			}
			return false;
		}

		private void PrintUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  Publish on a topic ");
			Console.WriteLine("     [-ip        <ipAddress = localhost>");
			Console.WriteLine("     [-p         <tcpPort   = 8194>");
			Console.WriteLine("     [-s         <service   = //viper/mktdata>]");
			Console.WriteLine("     [-t         <topic     = testtopic1,testTopic2>]");
			Console.WriteLine("     [-g    <groupId>]    \tpublisher groupId (defaults to a unique value)");
			Console.WriteLine("     [-pri  <piority>]    \tpublisher priority (default: Integer.MAX_VALUE)");
			Console.WriteLine("     [-me   <maxEvents>]  \tstop after publishing this many events (default: 100)");
			Console.WriteLine("     [-v]                 \tincrease verbosity (can be specified more than once)");
			Console.WriteLine("     [-auth      <option    = user (user|none|app={app}|dir={property})>]");
			Console.WriteLine("Press ENTER to quit");
		}

		private bool ParseCommandLine(String[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				if (string.Compare("-s", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_service = args[++i];
				}
				else if (string.Compare("-t", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_topics.AddRange(args[++i].Split(','));
				}
				else if (string.Compare("-v", args[i], true) == 0)
				{
					++d_verbose;
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
				else if (string.Compare("-g", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_groupId = args[++i];
				}
				else if (string.Compare("-pri", args[i], true) == 0
					&& i + 1 < args.Length)
				{
					d_priority = int.Parse(args[++i]);
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
						d_authOptions = null;
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

			if (d_hosts.Count == 0)
			{
				d_hosts.Add("localhost");
			}
			if (d_topics.Count == 0)
			{
				d_topics.Add("IBM US Equity");
			}

			return true;
		}

	}

}
