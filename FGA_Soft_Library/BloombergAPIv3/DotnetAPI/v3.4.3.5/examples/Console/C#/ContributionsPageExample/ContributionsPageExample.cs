using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Threading;
using Bloomberglp.Blpapi;


namespace ContributionsPageExample
{
	class ContributionsPageExample
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

		private String d_serverHost;
		private int d_serverPort;
		private String d_serviceName;
		private ArrayList d_topics;
		private String d_authOptions;
		private int d_maxEvents = 100;

		public ContributionsPageExample()
		{
			d_serviceName = "//blp/mpfbapi";
			d_serverPort = 8194;
			d_serverHost = "localhost";
			d_authOptions = AUTH_USER;
			d_topics = new ArrayList();
			d_topics.Add("220/660/1");
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
				d_serviceName + "/220/660/1",
				new CorrelationID(new MyStream("220/660/1")));

			session.CreateTopics(
				topicList,
				ResolveMode.AUTO_REGISTER_SERVICES,
				identity);

			List<MyStream> myStreams = new List<MyStream>();
			for (int i = 0; i < topicList.Size; ++i)
			{
				if (topicList.StatusAt(i) == TopicList.TopicStatus.CREATED)
				{
					Topic topic = session.GetTopic(topicList.MessageAt(i));
					MyStream stream = (MyStream)topicList.CorrelationIdAt(i).Object;
					stream.SetTopic(topic);
					myStreams.Add(stream);
				}
			}

			PublishEvents(session, myStreams);

			session.Stop();
		}

		public static void Main(String[] args)
		{
			ContributionsPageExample example = new ContributionsPageExample();
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

		private void PublishEvents(ProviderSession session, List<MyStream> myStreams)
		{
			Service service = session.GetService(d_serviceName);
			Random rnd = new Random(DateTime.Now.Millisecond);

			int iteration = 0;
			while (iteration++ < d_maxEvents)
			{
				Event eventObj = service.CreatePublishEvent();
				EventFormatter eventFormatter = new EventFormatter(eventObj);

				foreach (MyStream stream in myStreams)
				{
					eventFormatter.AppendMessage("PageData", stream.GetTopic());
					eventFormatter.PushElement("rowUpdate");

					eventFormatter.AppendElement();
					eventFormatter.SetElement("rowNum", 1);
					eventFormatter.PushElement("spanUpdate");

					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 20);
					eventFormatter.SetElement("length", 4);
					eventFormatter.SetElement("text", "TEST");
					eventFormatter.SetElement("attr", "INTENSIFY");
					eventFormatter.PopElement();

					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 25);
					eventFormatter.SetElement("length", 4);
					eventFormatter.SetElement("text", "PAGE");
					eventFormatter.SetElement("attr", "BLINK");
					eventFormatter.PopElement();

					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 30);
					String timestamp = System.DateTime.Now.ToString();
					eventFormatter.SetElement("length", timestamp.Length);
					eventFormatter.SetElement("text", timestamp);
					eventFormatter.SetElement("attr", "REVERSE");
					eventFormatter.PopElement();

					eventFormatter.PopElement();
					eventFormatter.PopElement();

					eventFormatter.AppendElement();
					eventFormatter.SetElement("rowNum", 2);
					eventFormatter.PushElement("spanUpdate");
					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 20);
					eventFormatter.SetElement("length", 9);
					eventFormatter.SetElement("text", "---------");
					eventFormatter.SetElement("attr", "UNDERLINE");
					eventFormatter.PopElement();
					eventFormatter.PopElement();
					eventFormatter.PopElement();

					eventFormatter.AppendElement();
					eventFormatter.SetElement("rowNum", 3);
					eventFormatter.PushElement("spanUpdate");
					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 10);
					eventFormatter.SetElement("length", 9);
					eventFormatter.SetElement("text", "TEST LINE");
					eventFormatter.PopElement();
					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 23);
					eventFormatter.SetElement("length", 5);
					eventFormatter.SetElement("text", "THREE");
					eventFormatter.PopElement();
					eventFormatter.PopElement();
					eventFormatter.PopElement();
					eventFormatter.PopElement();

					eventFormatter.SetElement("productCode", 1);
					eventFormatter.SetElement("pageNumber", 1);
				}

				foreach (Message msg in eventObj)
				{
					Console.WriteLine(msg.ToString());
				}

				session.Publish(eventObj);

				int sleepSecs = rnd.Next(20);
				Thread.Sleep(sleepSecs * 1000);
			}

		}

		private class MyStream
		{
			private String d_id;
			private Topic d_topic;

			public Topic GetTopic()
			{
				return d_topic;
			}

			public void SetTopic(Topic topic)
			{
				d_topic = topic;
			}

			public String SetId()
			{
				return d_id;
			}

			public MyStream(String id)
			{
				d_id = id;
				d_topic = null;
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
							return false;
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
			Console.WriteLine("     [-s         <service   = //blp/mpfbapi>]");
			Console.WriteLine("     [-auth      <option    = user> (user|none|app={app}|dir={property})]");

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
