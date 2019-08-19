using System;
using System.Collections.Generic;
using System.Text;

using Bloomberglp.Blpapi.impl;
using System.Threading;
using System.Collections;


namespace Bloomberglp.Blpapi.Examples
{
	class PagePublisherExample
	{
		private const String AUTH_USER = "AuthenticationType=OS_LOGON";
		private const String AUTH_APP_PREFIX = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=";
		private const String AUTH_DIR_PREFIX = "AuthenticationType=DIRECTORY_SERVICE;DirSvcPropertyName=";
		private const String AUTH_OPTION_NONE = "none";
		private const String AUTH_OPTION_USER = "user";
		private const String AUTH_OPTION_APP = "app=";
		private const String AUTH_OPTION_DIR = "dir=";

		private static readonly Name AUTHORIZATION_SUCCESS = Name.GetName("AuthorizationSuccess");
		private static readonly Name AUTHORIZATION_FAILURE = Name.GetName("AuthorizationFailure");
		private static readonly Name TOPIC = Name.GetName("topic");

		private static readonly Name TOPIC_SUBSCRIBED = Name.GetName("TopicSubscribed");
		private static readonly Name TOPIC_UNSUBSCRIBED = Name.GetName("TopicUnsubscribed");
		private static readonly Name TOPIC_RECAP = Name.GetName("TopicRecap");
		private static readonly Name TOPIC_CREATED = Name.GetName("TopicCreated");
		private static readonly Name START_COL = Name.GetName("startCol");
		private static readonly Name TOKEN_SUCCESS = Name.GetName("TokenGenerationSuccess");
		private static readonly Name TOKEN_FAILURE = Name.GetName("TokenGenerationFailure");
		private static readonly Name SESSION_TERMINATED = Name.GetName("SessionTerminated");

		private Dictionary<Topic, Topic> d_topicSet = new Dictionary<Topic, Topic>(); // Hashset
		private String d_service = "//viper/page";
		private int d_verbose = 0;
		private List<String> d_hosts = new List<String>();
		private int d_port = 8194;

		const string authServiceName = "//blp/apiauth";
		private String d_token = null;
		private Identity d_identity = null;
		private String d_authOptions = AUTH_USER;
		private bool? d_tokenGenerationResponse = null;
		private bool? d_authorizationResponse = null;
		private CorrelationID d_authorizationResponseCorrelationId = null;

		private static volatile bool g_running = true;

		public PagePublisherExample()
		{
		}

		public void Run(string[] args) //throws Exception
		{
			if (!parseCommandLine(args)) return;

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

			Console.Write("Connecting to");
			foreach (SessionOptions.ServerAddress server in sessionOptions.ServerAddresses)
			{
				Console.Write(" " + server);
			}
			Console.WriteLine();

			ProviderSession session = new ProviderSession(
					sessionOptions,
					processEvent);

			if (!session.Start())
			{
				Console.Error.WriteLine("Failed to start session");
				return;
			}

			Identity identity = null;
			if (d_authOptions.Length != 0)
			{
				Object tokenResponseMonitor = new Object();
				lock (tokenResponseMonitor)
				{
					session.GenerateToken(new CorrelationID(tokenResponseMonitor));
					long waitTime = 10 * 1000;
					long tokenResponseTimeout = System.DateTime.Now.Ticks / 10000 + waitTime;
					while (d_tokenGenerationResponse == null && waitTime > 0)
					{
						Monitor.Wait(tokenResponseMonitor, (int)waitTime);
						waitTime = tokenResponseTimeout - System.DateTime.Now.Ticks / 10000;
					}
					if (d_tokenGenerationResponse == null)
					{
						System.Console.Error.WriteLine("Timeout waiting for token");
						System.Environment.Exit(1);
					}
					else if (d_tokenGenerationResponse == false || d_token == null)
					{
						System.Console.Error.WriteLine("Token generation failed");
						System.Environment.Exit(1);
					}
				}

				Object authorizationResponseMonitor = new Object();
				if (session.OpenService("//blp/apiauth"))
				{
					Service authService = session.GetService("//blp/apiauth");
					Request authRequest = authService.CreateAuthorizationRequest();
					authRequest.Set("token", d_token);
					identity = session.CreateIdentity();
					d_authorizationResponseCorrelationId =
						new CorrelationID(authorizationResponseMonitor);
					lock (authorizationResponseMonitor)
					{
						session.SendAuthorizationRequest(
								authRequest,
								identity,
								d_authorizationResponseCorrelationId);
						long waitTime = 60 * 1000;
						long authorizationResponseTimeout = System.DateTime.Now.Ticks / 10000 + waitTime;
						while (d_authorizationResponse == null && waitTime > 0)
						{
							Monitor.Wait(authorizationResponseMonitor, 1000);
							waitTime = authorizationResponseTimeout - System.DateTime.Now.Ticks / 10000;
						}
						if (d_authorizationResponse == null)
						{
							System.Console.Error.WriteLine("Timeout waiting for authorization");
							System.Environment.Exit(1);
						}
						else if (d_authorizationResponse == false)
						{
							System.Console.Error.WriteLine("Authorization failed");
							System.Environment.Exit(1);
						}
					}
				}
			}

			//Registering for subscription start and stop messages.
			if (!session.RegisterService(d_service, d_identity))
			{
				Console.WriteLine("Failed to register " + d_service);
				return;
			}
			Console.WriteLine("Service registered " + d_service);


			//Publishing events for the active topics of the designated service.
			PublishEvents(session);

			session.Stop();
		}

		#region private member

		// Handling subscription start and stop events, which add and remove topics to the active publication set.
		private void processEvent(Event eventObj, ProviderSession session)
		{
			switch (eventObj.Type)
			{
				case Event.EventType.SESSION_STATUS:
					{
						foreach (Message msg in eventObj)
						{
							if (msg.MessageType == SESSION_TERMINATED)
							{
								g_running = false;
								break;
							}
						}
					} break;
				case Event.EventType.TOKEN_STATUS:
					ProcessTokenStatusMsg(session, eventObj);
					break;
				case Event.EventType.TOPIC_STATUS:
					ProcessTopicStatusMsg(session, eventObj);
					break;
				case Event.EventType.RESPONSE:
				case Event.EventType.PARTIAL_RESPONSE:
				case Event.EventType.REQUEST_STATUS:
					{
						foreach (Message msg in eventObj)
						{
							if (msg.CorrelationID.Equals(d_authorizationResponseCorrelationId))
							{
								Object authorizationResponseMonitor = msg.CorrelationID.Object;
								lock (authorizationResponseMonitor)
								{
									if (msg.MessageType == AUTHORIZATION_SUCCESS)
									{
										d_authorizationResponse = true;
										Monitor.PulseAll(authorizationResponseMonitor);
									}
									else if (msg.MessageType == AUTHORIZATION_FAILURE)
									{
										d_authorizationResponse = false;
										Monitor.PulseAll(authorizationResponseMonitor);
										System.Console.Error.WriteLine("Not authorized: " + msg.GetElement("reason"));
									}
									else
									{
										System.Diagnostics.Trace.Assert(d_authorizationResponse.Value);
										System.Console.WriteLine("Permissions updated");
									}
								}
							}
						}
					} break;
				default:
					if (d_verbose > 0)
					{
						foreach (Message msg in eventObj)
						{
							Console.WriteLine("Message = " + msg);
						}
					}
					break;
			}
		}

		private void PublishEvents(ProviderSession session)
		{
			Service service = session.GetService(d_service);
			while (g_running)
			{
				Event eventObj = null;
				lock (d_topicSet)
				{
					if (d_topicSet.Count == 0)
					{
						Monitor.Wait(d_topicSet, 100);
					}
					if (d_topicSet.Count == 0)
					{
						continue;
					}

					Console.WriteLine("Publishing");
					eventObj = service.CreatePublishEvent();
					EventFormatter eventFormatter = new EventFormatter(eventObj);

					foreach (Topic topic in d_topicSet.Keys)
					{
						String os = new DateTime().ToString();

						int numRows = 5;
						int numCols = 3;
						for (int i = 0; i < numRows; ++i)
						{
							eventFormatter.AppendMessage("RowUpdate", topic);
							eventFormatter.SetElement("rowNum", i);
							eventFormatter.PushElement("spanUpdate");
							for (int j = 0; j < numCols; ++j)
							{
								eventFormatter.AppendElement();
								eventFormatter.SetElement("startCol", j);
								eventFormatter.SetElement("length", os.Length);
								eventFormatter.SetElement("text", os);
								eventFormatter.PopElement();
							}
							eventFormatter.PopElement();
						}
					}
				}
				if (d_verbose > 1)
				{
					if (eventObj != null)
					{
						foreach (Message msg in eventObj)
						{
							Console.WriteLine(msg);
						}
					}
				}
				session.Publish(eventObj);
				Thread.Sleep(10 * 1000);
			}
		}

		private void ProcessTokenStatusMsg(ProviderSession session, Event eventObj)
		{
			foreach (Message msg in eventObj)
			{
				Object tokenResponseMonitor = msg.CorrelationID.Object;
				lock (tokenResponseMonitor)
				{
					if (msg.MessageType == TOKEN_SUCCESS)
					{
						d_tokenGenerationResponse = true;
						d_token = msg.GetElementAsString("token");
					}
					else if (msg.MessageType == TOKEN_FAILURE)
					{
						d_tokenGenerationResponse = false;
					}
					Monitor.PulseAll(tokenResponseMonitor);
				}
			}
		}

		private void ProcessTopicStatusMsg(ProviderSession session, Event eventObj)
		{
			TopicList topicList = new TopicList();

			foreach (Message msg in eventObj)
			{
				Console.WriteLine(msg);

				if (msg.MessageType == TOPIC_SUBSCRIBED)
				{
					Topic topic = session.GetTopic(msg);
					if (topic == null)
					{
						CorrelationID cid = new CorrelationID(msg.GetElementAsString(TOPIC));
						topicList.Add(msg, cid);
					}
					else
					{
						lock (d_topicSet)
						{
							if (!d_topicSet.ContainsKey(topic))
							{
								d_topicSet[topic] = topic;
								Monitor.PulseAll(d_topicSet);
							}
						}
					}
				}
				else if (msg.MessageType == TOPIC_UNSUBSCRIBED)
				{
					Topic topic = session.GetTopic(msg);
					lock (d_topicSet)
					{
						d_topicSet.Remove(topic);
					}
				}
				else if (msg.MessageType == TOPIC_CREATED)
				{
					Topic topic = session.GetTopic(msg);
					lock (d_topicSet)
					{
						if (!d_topicSet.ContainsKey(topic))
						{
							d_topicSet[topic] = topic;
							Monitor.PulseAll(d_topicSet);
						}
					}
				}
				else if (msg.MessageType == TOPIC_RECAP)
				{
					Topic topic = session.GetTopic(msg);
					lock (d_topicSet)
					{
						if (!d_topicSet.ContainsKey(topic))
						{
							continue;
						}
					}
					// send initial paint.this should come from app's cache.
					Service service = session.GetService(d_service);
					Event recapEvent = service.CreatePublishEvent();
					EventFormatter eventFormatter = new EventFormatter(recapEvent);
					eventFormatter.AppendRecapMessage(topic, msg.CorrelationID);
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

			if (topicList.Size > 0)
			{
				session.CreateTopicsAsync(topicList);
			}
		}

		private void printUsage()
		{
			Console.WriteLine("Usage:");
			Console.WriteLine("  Publish on a topic ");
			Console.WriteLine("    -v                verbose, use multiple times to increase verbosity");
			Console.WriteLine("    -ip   <ipAddress> server name or IP (default = localhost)");
			Console.WriteLine("    -p    <tcpPort>   server port (default = 8194)");
			Console.WriteLine("    -s    <service>   service name (default = //viper/page>)");
			Console.WriteLine("    -auth <option>    authentication option: user|none|app=<app>|dir=<property> (default = user)");
		}

		private bool parseCommandLine(String[] args)
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
							printUsage();
							return false;
						}
					}
					else
					{
						printUsage();
						return false;
					}
				}
			}
			catch (Exception)
			{
				printUsage();
				return false;
			}

			if (d_hosts.Count == 0)
			{
				d_hosts.Add("localhost");
			}

			return true;
		}

		#endregion

		static void Main(string[] args)
		{
			System.Console.WriteLine("PagePublisherExample");
			PagePublisherExample example = new PagePublisherExample();
			example.Run(args);

			Console.WriteLine("Press <ENTER> to terminate.");
			Console.ReadLine();
		}

	}
}
