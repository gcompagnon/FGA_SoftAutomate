using System;
using System.Collections.Generic;
using System.Threading;

using Bloomberglp.Blpapi;

namespace PageBroadcastPublisherExample
{
	class PageBroadcastPublisherExample
	{
		private static readonly Name AUTHORIZATION_SUCCESS = Name.GetName("AuthorizationSuccess");
		private static readonly Name PERMISSION_REQUEST = Name.GetName("PermissionRequest");
		private static readonly Name TOPIC_SUBSCRIBED = Name.GetName("TopicSubscribed");
		private static readonly Name TOPIC_RECAP = Name.GetName("TopicRecap");
		private static readonly Name TOKEN_SUCCESS = Name.GetName("TokenGenerationSuccess");

		private const String AUTH_USER = "AuthenticationType=OS_LOGON";
		private const String AUTH_APP_PREFIX = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=";
		private const String AUTH_DIR_PREFIX = "AuthenticationType=DIRECTORY_SERVICE;DirSvcPropertyName=";
		private const String AUTH_OPTION_NONE = "none";
		private const String AUTH_OPTION_USER = "user";
		private const String AUTH_OPTION_APP = "app=";
		private const String AUTH_OPTION_DIR = "dir=";

		private const string AUTH_SERVICE_NAME = "//blp/apiauth";

		private String d_host;
		private int d_port;
		private String d_service;

		private String d_authOptions = null;
		private int d_verbose = 0;

		private class MyStream
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
		};

		public PageBroadcastPublisherExample()
		{
			d_host = "localhost";
			d_port = 8194;
			d_service = "//viper/page";
			d_authOptions = AUTH_USER;
		}

		private void ProcessTopicStatusEvent(Event eventObj, ProviderSession session)
		{
			foreach (Message msg in eventObj)
			{
				System.Console.WriteLine(msg);
				if (msg.MessageType == TOPIC_RECAP
					|| msg.MessageType == TOPIC_SUBSCRIBED)
				{
					Topic topic = session.GetTopic(msg);
					if (topic != null)
					{
						// send initial paint, this should come from my own cache
						Service service = session.GetService(d_service);
						Event recapEvent = service.CreatePublishEvent();
						EventFormatter eventFormatter = new EventFormatter(recapEvent);
						CorrelationID recapCid = msg.MessageType == TOPIC_RECAP ? 
							msg.CorrelationID	//solicited recap
							: null;				//unsolicited recap
						eventFormatter.AppendRecapMessage(
							topic, 
							recapCid);
						eventFormatter.SetElement("numRows", 25);
						eventFormatter.SetElement("numCols", 80);
						eventFormatter.PushElement("rowUpdate");
						for (int i = 0; i < 5; ++i)
						{
							eventFormatter.AppendElement();
							eventFormatter.SetElement("rowNum", i);
							eventFormatter.PushElement("spanUpdate");
							eventFormatter.AppendElement();
							eventFormatter.SetElement("startCol", 1);
							eventFormatter.SetElement("length", 10);
							eventFormatter.SetElement("text", "RECAP");
							eventFormatter.PopElement();
							eventFormatter.PopElement();
							eventFormatter.PopElement();
						}
						eventFormatter.PopElement();
						session.Publish(recapEvent);
					}
				}
			}
		}

		private void ProcessRequestEvent(Event eventObj, ProviderSession session)
		{
			foreach (Message msg in eventObj)
			{
				if (msg.MessageType == PERMISSION_REQUEST)
				{
					Service pubService = session.GetService(d_service);
					Event response = pubService.CreateResponseEvent(msg.CorrelationID);
					EventFormatter ef = new EventFormatter(response);
					ef.AppendResponse("PermissionResponse");
					ef.PushElement("topicPermissions"); // TopicPermissions

					Element topicElement = msg.GetElement(Name.GetName("topics"));
					for (int i = 0; i < topicElement.NumValues; ++i)
					{
						ef.AppendElement();
						ef.SetElement("topic", topicElement.GetValueAsString(i));
						int permission = 0;
						ef.SetElement("result", permission); // ALLOWED: 0, DENIED: 1

						if (permission == 1)
						{// DENIED
							ef.PushElement("reason");
							ef.SetElement("source", "My Publisher Name");
							ef.SetElement("category", "NOT_AUTHORIZED"); // or BAD_TOPIC, or custom
							ef.SetElement("subcategory", "Publisher Controlled");
							ef.SetElement("description", "Permission denied by My Publisher Name");
							ef.PopElement();
						}
					}

					session.SendResponse(response);
				}
				else
				{
					System.Console.WriteLine("Received unknown request: " + msg);
				}
			}
		}

		private void ProcessEvent(Event eventObj, ProviderSession session)
		{
			PrintMessage(eventObj);
			switch (eventObj.Type)
			{
				case Event.EventType.TOPIC_STATUS:
					ProcessTopicStatusEvent(eventObj, session);
					break;
				case Event.EventType.REQUEST:
					ProcessRequestEvent(eventObj, session);
					break;
				default:
					break;
			}
			return;
		}

		private void PrintUsage()
		{
			System.Console.WriteLine("Usage:");
			System.Console.WriteLine("  Broadcast Publisher ");
			System.Console.WriteLine("  -ip   <ipAddress> server name or IP (default = localhost)");
			System.Console.WriteLine("  -p    <tcpPort>   server port (default = 8194)");
			System.Console.WriteLine("  -s    <service>   service name (default = //viper/page)");
			System.Console.WriteLine("  -v                verbose, use multiple times to increase verbosity");
			System.Console.WriteLine("  -auth <option>    authentication option: user|none|app=<app>|dir=<property> (default = user)");
		}

		private bool ParseCommandLine(String[] args)
		{
			try
			{
				for (int i = 0; i < args.Length; ++i)
				{
					if (string.Compare("-ip", args[i], true) == 0)
					{
						d_host = args[++i];
					}
					else if (string.Compare("-p", args[i], true) == 0)
					{
						d_port = int.Parse(args[++i]);
					}
					else if (string.Compare("-s", args[i], true) == 0)
					{
						d_service = args[++i];
					}
					else if (string.Compare("-v", args[i], true) == 0)
					{
						++d_verbose;
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

		private void PrintMessage(Event eventObj)
		{
			foreach (Message msg in eventObj)
			{
				System.Console.WriteLine(msg);
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

		private void Publish(TopicList topicList, ProviderSession session)
		{
			List<MyStream> myStreams = new List<MyStream>();
			for (int i = 0; i < topicList.Size; ++i)
			{
				if (topicList.StatusAt(i) == TopicList.TopicStatus.CREATED)
				{
					Message message = topicList.MessageAt(i);
					Topic topic = session.GetTopic(message);
					MyStream stream = (MyStream)topicList.CorrelationIdAt(i).Object;
					stream.setTopic(topic);
					myStreams.Add(stream);
					System.Console.WriteLine("Topic created: " + stream.getId());
				}
			}

			Service pubService = session.GetService(d_service);
			if (pubService == null)
			{
				System.Console.Error.WriteLine("service unavailable");
				return;
			}

			// Now we will start publishing
			Event eventObj = pubService.CreatePublishEvent();
			EventFormatter eventFormatter = new EventFormatter(eventObj);
			for (int index = 0; index < myStreams.Count; index++)
			{
				MyStream stream = (MyStream)myStreams[index];

				eventFormatter.AppendRecapMessage(stream.getTopic(), null);
				eventFormatter.SetElement("numRows", 25);
				eventFormatter.SetElement("numCols", 80);
				eventFormatter.PushElement("rowUpdate");
				for (int i = 0; i < 5; ++i)
				{
					eventFormatter.AppendElement();
					eventFormatter.SetElement("rowNum", i);
					eventFormatter.PushElement("spanUpdate");
					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 1);
					eventFormatter.SetElement("length", 10);
					eventFormatter.SetElement("text", "INTIAL");
					eventFormatter.SetElement("fgColor", "RED");
					eventFormatter.PushElement("attr");
					eventFormatter.AppendValue("UNDERLINE");
					eventFormatter.AppendValue("BLINK");
					eventFormatter.PopElement();
					eventFormatter.PopElement();
					eventFormatter.PopElement();
					eventFormatter.PopElement();
				}
				eventFormatter.PopElement();
			}
			if (d_verbose > 0)
			{
				PrintMessage(eventObj);
			}
			session.Publish(eventObj);

			while (true)
			{
				eventObj = pubService.CreatePublishEvent();
				eventFormatter = new EventFormatter(eventObj);

				for (int index = 0; index < myStreams.Count; index++)
				{
					MyStream stream = (MyStream)myStreams[index];
					eventFormatter.AppendMessage("RowUpdate", stream.getTopic());
					eventFormatter.SetElement("rowNum", 1);
					eventFormatter.PushElement("spanUpdate");
					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 1);
					String text = System.DateTime.Now.ToString();
					eventFormatter.SetElement("length", text.Length);
					eventFormatter.SetElement("text", text);
					eventFormatter.PopElement();
					eventFormatter.AppendElement();
					eventFormatter.SetElement("startCol", 10);
					text = System.DateTime.Now.ToString();
					eventFormatter.SetElement("length", text.Length);
					eventFormatter.SetElement("text", text);
					eventFormatter.PopElement();
					eventFormatter.PopElement();
				}

				if (d_verbose > 0)
				{
					PrintMessage(eventObj);
				}
				session.Publish(eventObj);
				Thread.Sleep(10 * 1000);
			}
		}

		public void Run(String[] args) //throws Exception
		{
			if (!ParseCommandLine(args))
			{
				return;
			}

			SessionOptions sessionOptions = new SessionOptions();
			sessionOptions.ServerHost = d_host;
			sessionOptions.ServerPort = d_port;
			sessionOptions.AuthenticationOptions = d_authOptions;

			System.Console.WriteLine("Connecting to " + d_host + ":" + d_port);
			ProviderSession session = new ProviderSession(sessionOptions, ProcessEvent);

			if (!session.Start())
			{
				System.Console.WriteLine("Failed to start session");
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
				d_service + "/1245/4/5",
				new CorrelationID(new MyStream("1245/4/5")));
			topicList.Add(
				d_service + "/330/1/1",
				new CorrelationID(new MyStream("330/1/1")));

			session.CreateTopics(
				topicList,
				ResolveMode.AUTO_REGISTER_SERVICES,
				identity);
			// createTopics() is synchronous, topicList will be updated
			// with the results of topic creation (resolution will happen
			// under the covers)

			Publish(topicList, session);

		}

		public static void Main(String[] args)
		{
			System.Console.WriteLine("PageBroadcastPublisherExample");
			PageBroadcastPublisherExample example = new PageBroadcastPublisherExample();
			example.Run(args);
		}
	}
}
