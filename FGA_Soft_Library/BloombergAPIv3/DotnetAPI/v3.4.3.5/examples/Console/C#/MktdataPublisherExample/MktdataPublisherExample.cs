using System;
using System.Collections.Generic;
using System.Text;
using Bloomberglp.Blpapi;
using System.Collections;
using System.Threading;

namespace MktdataBroadcastPublisherExample
{
	public class MktdataPublisherExample
	{
		private const String AUTH_USER = "AuthenticationType=OS_LOGON";
		private const String AUTH_APP_PREFIX = "AuthenticationMode=APPLICATION_ONLY;ApplicationAuthenticationType=APPNAME_AND_KEY;ApplicationName=";
		private const String AUTH_DIR_PREFIX = "AuthenticationType=DIRECTORY_SERVICE;DirSvcPropertyName=";
		private const String AUTH_OPTION_NONE = "none";
		private const String AUTH_OPTION_USER = "user";
		private const String AUTH_OPTION_APP = "app=";
		private const String AUTH_OPTION_DIR = "dir=";

		private static readonly Name SERVICE_REGISTERED = Name.GetName("ServiceRegistered");
		private static readonly Name SERVICE_REGISTER_FAILURE = Name.GetName("ServiceRegisterFailure");
		private static readonly Name TOPIC_SUBSCRIBED = Name.GetName("TopicSubscribed");
		private static readonly Name TOPIC_UNSUBSCRIBED = Name.GetName("TopicUnsubscribed");
		private static readonly Name TOPIC_CREATED = Name.GetName("TopicCreated");
		private static readonly Name TOPIC_RECAP = Name.GetName("TopicRecap");
		private static readonly Name RESOLUTION_SUCCESS = Name.GetName("ResolutionSuccess");
		private static readonly Name RESOLUTION_FAILURE = Name.GetName("ResolutionFailure");
		private static readonly Name PERMISSION_REQUEST = Name.GetName("PermissionRequest");
		private static readonly Name TOKEN_SUCCESS = Name.GetName("TokenGenerationSuccess");
		private static readonly Name TOKEN_FAILURE = Name.GetName("TokenGenerationFailure");
		private static readonly Name AUTHORIZATION_SUCCESS = Name.GetName("AuthorizationSuccess");
		private static readonly Name AUTHORIZATION_FAILURE = Name.GetName("AuthorizationFailure");
		private static readonly Name SESSION_TERMINATED = Name.GetName("SessionTerminated");

		private String d_service = "//viper/mktdata";
		private int d_verbose = 0;
		private List<String> d_hosts = new List<String>();
		private int d_port = 8194;

		private readonly Dictionary<Topic, Topic> d_topicSet = new Dictionary<Topic, Topic>();
		private String d_token = null;
		private bool? d_tokenGenerationResponse = null;
		private bool? d_authorizationResponse = null;
		private CorrelationID d_authorizationResponseCorrelationId = null;
		private bool? d_registerServiceResponse = null;
		private String d_groupId = null;
		private int d_priority = int.MaxValue;

		private String d_authOptions = AUTH_USER;

		private static volatile bool g_running = true;

		public void ProcessEvent(Event eventObj, ProviderSession session)
		{

			if (d_verbose > 0)
			{
				System.Console.WriteLine("Received event " + eventObj.Type);
				foreach (Message msg in eventObj)
				{
					System.Console.WriteLine("cid = " + msg.CorrelationID);
					System.Console.WriteLine("Message = " + msg);
				}
			}

			if (eventObj.Type == Event.EventType.SESSION_STATUS)
			{
				foreach (Message msg in eventObj)
				{
					if (msg.MessageType == SESSION_TERMINATED)
					{
						g_running = false;
						break;
					}
				}
			}
			else if (eventObj.Type == Event.EventType.TOKEN_STATUS)
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
			else if (eventObj.Type == Event.EventType.TOPIC_STATUS)
			{
				TopicList topicList = new TopicList();
				foreach (Message msg in eventObj)
				{
					if (msg.MessageType == TOPIC_SUBSCRIBED)
					{
						Topic topic = session.GetTopic(msg);
						if (topic == null)
						{
							CorrelationID cid = new CorrelationID(msg.GetElementAsString("topic"));
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
						// Here we send a recap in response to a Recap Request.
						Topic topic = session.GetTopic(msg);
						lock (d_topicSet)
						{
							if (!d_topicSet.ContainsKey(topic))
							{
								continue;
							}
						}
						Service service = topic.Service;
						Event recapEvent = service.CreatePublishEvent();
						EventFormatter eventFormatter = new EventFormatter(recapEvent);
						eventFormatter.AppendRecapMessage(topic, msg.CorrelationID);
						eventFormatter.SetElement("OPEN", 100.0);

						session.Publish(recapEvent);
						foreach (Message recapMsg in recapEvent)
						{
							System.Console.WriteLine(recapMsg);
						}
					}
				}

				// createTopicsAsync will result in RESOLUTION_STATUS, TOPIC_CREATED events.
				if (topicList.Size > 0)
				{
					session.CreateTopicsAsync(topicList);
				}
			}
			else if (eventObj.Type == Event.EventType.SERVICE_STATUS)
			{
				foreach (Message msg in eventObj)
				{
					if (msg.MessageType == SERVICE_REGISTERED)
					{
						Object registerServiceResponseMonitor = msg.CorrelationID.Object;
						lock (registerServiceResponseMonitor)
						{
							d_registerServiceResponse = true;
							Monitor.PulseAll(registerServiceResponseMonitor);
						}
					}
					else if (msg.MessageType == SERVICE_REGISTER_FAILURE)
					{
						Object registerServiceResponseMonitor = msg.CorrelationID.Object;
						lock (registerServiceResponseMonitor)
						{
							d_registerServiceResponse = false;
							Monitor.PulseAll(registerServiceResponseMonitor);
						}
					}
				}
			}
			else if (eventObj.Type == Event.EventType.RESOLUTION_STATUS)
			{
				foreach (Message msg in eventObj)
				{
					if (msg.MessageType == RESOLUTION_SUCCESS)
					{
						String resolvedTopic = msg.GetElementAsString(Name.GetName("resolvedTopic"));
						System.Console.WriteLine("ResolvedTopic: " + resolvedTopic);
					}
					else if (msg.MessageType == RESOLUTION_FAILURE)
					{
						System.Console.WriteLine(
								"Topic resolution failed (cid = " +
								msg.CorrelationID +
								")");
					}
				}
			}
			else if (eventObj.Type == Event.EventType.REQUEST)
			{
				Service service = session.GetService(d_service);
				foreach (Message msg in eventObj)
				{
					if (msg.MessageType == PERMISSION_REQUEST)
					{
						// Similar to createPublishEvent. We assume just one
						// service - d_service. A responseEvent can only be
						// for single request so we can specify the
						// correlationId - which establishes context -
						// when we create the Event.
						Event response = service.CreateResponseEvent(msg.CorrelationID);
						EventFormatter ef = new EventFormatter(response);
						int permission = 1; // ALLOWED: 0, DENIED: 1
						if (msg.HasElement("uuid"))
						{
							int uuid = msg.GetElementAsInt32("uuid");
							System.Console.WriteLine("UUID = " + uuid);
							permission = 0;
						}
						if (msg.HasElement("applicationId"))
						{
							int applicationId = msg.GetElementAsInt32("applicationId");
							System.Console.WriteLine("APPID = " + applicationId);
							permission = 0;
						}
						if (msg.HasElement("seatType"))
						{
							SeatType seatType = (SeatType)msg.GetElementAsInt32("seatType");
							if (seatType == SeatType.INVALID_SEAT)
							{
								permission = 1;
							}
							else
							{
								permission = 0;
							}
						}
						// In appendResponse the string is the name of the
						// operation, the correlationId indicates
						// which request we are responding to.
						ef.AppendResponse("PermissionResponse");
						ef.PushElement("topicPermissions");
						// For each of the topics in the request, add an entry
						// to the response
						Element topicsElement = msg.GetElement(Name.GetName("topics"));
						for (int i = 0; i < topicsElement.NumValues; ++i)
						{
							ef.AppendElement();
							ef.SetElement("topic", topicsElement.GetValueAsString(i));

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
							ef.PopElement();
						}
						ef.PopElement();
						// Service is implicit in the Event. sendResponse has a
						// second parameter - partialResponse -
						// that defaults to false.
						session.SendResponse(response);
					}
					else
					{
						System.Console.WriteLine("Received unknown request: " + msg);
					}
				}
			}
			else if (eventObj.Type == Event.EventType.RESPONSE
				  || eventObj.Type == Event.EventType.PARTIAL_RESPONSE
				  || eventObj.Type == Event.EventType.REQUEST_STATUS)
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
								System.Console.Error.WriteLine("Not authorized: " + msg.GetElement("reason"));
								Monitor.PulseAll(authorizationResponseMonitor);
							}
							else
							{
								System.Diagnostics.Trace.Assert(d_authorizationResponse.Value);
								System.Console.WriteLine("Permissions updated");
							}
						}
					}
				}
			}
		}

		private void PrintUsage()
		{
			System.Console.WriteLine("Publish market data.");
			System.Console.WriteLine("Usage:");
			System.Console.WriteLine("\t[-ip   <ipAddress>]  \tserver name or IP (default: localhost)");
			System.Console.WriteLine("\t[-p    <tcpPort>]    \tserver port (default: 8194)");
			System.Console.WriteLine("\t[-s    <service>]    \tservice name (default: //viper/mktdata)");
			System.Console.WriteLine("\t[-g    <groupId>]    \tpublisher groupId (defaults to a unique value)");
			System.Console.WriteLine("\t[-pri  <piority>]    \tpublisher priority (default: Integer.MAX_VALUE)");
			System.Console.WriteLine("\t[-v]                 \tincrease verbosity (can be specified more than once)");
			System.Console.WriteLine("\t[-auth <option>]     \tauthentication option: user|none|app=<app>|userapp=<app>|dir=<property> (default: user)");
		}

		private bool ParseCommandLine(String[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				if (string.Compare("-ip", args[i], true) == 0
						&& i + 1 < args.Length)
				{
					d_hosts.Add(args[++i]);
				}
				else if (string.Compare("-p", args[i], true) == 0
						&& i + 1 < args.Length)
				{
					d_port = int.Parse(args[++i]);
				}
				else if (string.Compare("-s", args[i], true) == 0
						&& i + 1 < args.Length)
				{
					d_service = args[++i];
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
				else if (string.Compare("-h", args[i], true) == 0)
				{
					PrintUsage();
					return false;
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

			return true;
		}

		public void Run(String[] args)
		{
			if (!ParseCommandLine(args))
				return;

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

			ServiceRegistrationOptions serviceRegistrationOptions = new ServiceRegistrationOptions();
			serviceRegistrationOptions.GroupId = d_groupId;
			serviceRegistrationOptions.ServicePriority = d_priority;

			bool wantAsyncRegisterService = true;
			if (wantAsyncRegisterService)
			{
				Object registerServiceResponseMonitor = new Object();
				CorrelationID registerCID = new CorrelationID(registerServiceResponseMonitor);
				lock (registerServiceResponseMonitor)
				{
					if (d_verbose > 0)
					{
						System.Console.WriteLine("start registerServiceAsync, cid = " + registerCID);
					}
					session.RegisterServiceAsync(d_service, identity, registerCID, serviceRegistrationOptions);
					for (int i = 0; d_registerServiceResponse == null && i < 10; ++i)
					{
						Monitor.Wait(registerServiceResponseMonitor, 1000);
					}
				}
			}
			else
			{
				bool result = session.RegisterService(d_service, identity, serviceRegistrationOptions);
				d_registerServiceResponse = result;
			}

			Service service = session.GetService(d_service);
			if (service != null && d_registerServiceResponse == true)
			{
				System.Console.WriteLine("Service registered: " + d_service);
			}
			else
			{
				System.Console.Error.WriteLine("Service registration failed: " + d_service);
				System.Environment.Exit(1);
			}

			// Dump schema for the service
			if (d_verbose > 1)
			{
				System.Console.WriteLine("Schema for service:" + d_service);
				for (int i = 0; i < service.NumEventDefinitions; ++i)
				{
					SchemaElementDefinition eventDefinition = service.GetEventDefinition(i);
					System.Console.WriteLine(eventDefinition);
				}
			}

			// Now we will start publishing

			long tickCount = 1;
			while (g_running)
			{
				Event eventObj;
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

					eventObj = service.CreatePublishEvent();
					EventFormatter eventFormatter = new EventFormatter(eventObj);

					foreach (Topic topic in d_topicSet.Keys)
					{
						if (!topic.IsActive())
						{
							continue;
						}
						eventFormatter.AppendMessage("MarketDataEvents", topic);
						if (1 == tickCount)
						{
							eventFormatter.SetElement("BEST_ASK", 100.0);
						}
						else if (2 == tickCount)
						{
							eventFormatter.SetElement("BEST_BID", 99.0);
						}
						eventFormatter.SetElement("HIGH", 100 + tickCount * 0.01);
						eventFormatter.SetElement("LOW", 100 - tickCount * 0.005);
						++tickCount;
					}
				}

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
			System.Console.WriteLine("MktdataPublisherExample");
			MktdataPublisherExample example = new MktdataPublisherExample();
			example.Run(args);
			System.Console.WriteLine("Press ENTER to quit");
			System.Console.Read();
		}
	}
}
