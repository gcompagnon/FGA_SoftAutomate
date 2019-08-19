//----------------------------------------------------------------------------
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR PURPOSE.
//----------------------------------------------------------------------------

using System;
using Event = Bloomberglp.Blpapi.Event;
using Element = Bloomberglp.Blpapi.Element;
using InvalidRequestException = Bloomberglp.Blpapi.InvalidRequestException;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Request = Bloomberglp.Blpapi.Request;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;

namespace Bloomberglp.Blpapi.Examples
{
    public class SimpleFieldInfoExample
    {
        private const String APIFLDS_SVC = "//blp/apiflds";
        private const int ID_LEN = 13;
        private const int MNEMONIC_LEN = 25;
        private const int DESC_LEN = 40;
        private const String PADDING =
            "                                            ";
        private static readonly Name FIELD_ID = new Name("id");
        private static readonly Name FIELD_MNEMONIC = new Name("mnemonic");
        private static readonly Name FIELD_DATA = new Name("fieldData");
        private static readonly Name FIELD_DESC = new Name("description");
        private static readonly Name FIELD_INFO = new Name("fieldInfo");
        private static readonly Name FIELD_ERROR = new Name("fieldError");
        private static readonly Name FIELD_MSG = new Name("message");

        private String d_serverHost;
        private int d_serverPort;

        public static void Main(String[] args)
        {
            SimpleFieldInfoExample example = new SimpleFieldInfoExample();
            example.run(args);

            System.Console.WriteLine("Press ENTER to quit");
            System.Console.Read();
        }

        private void run(String[] args)
        {
            d_serverHost = "localhost";
            d_serverPort = 8194;

            if (!parseCommandLine(args))
            {
                return;
            }

            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = d_serverHost;
            sessionOptions.ServerPort = d_serverPort;

            System.Console.WriteLine("Connecting to " + d_serverHost
                                                      + ":" + d_serverPort);
            Session session = new Session(sessionOptions);
            bool sessionStarted = session.Start();
            if (!sessionStarted)
            {
                System.Console.WriteLine("Failed to start session.");
                return;
            }
            if (!session.OpenService(APIFLDS_SVC))
            {
                System.Console.WriteLine("Failed to open service: "
                    + APIFLDS_SVC);
                return;
            }

            Service fieldInfoService = session.GetService(APIFLDS_SVC);
            Request request = fieldInfoService.CreateRequest(
                "FieldInfoRequest");
            Element idList = request.GetElement("id");
            request.Append("id", "LAST_PRICE");
            request.Append("id", "pq005");
            request.Append("id", "zz0002");

            request.Set("returnFieldDocumentation", false);

            System.Console.WriteLine("Sending Request: " + request);
            session.SendRequest(request, null);

            while (true)
            {
                try
                {
                    Event eventObj = session.NextEvent();
                    foreach (Message msg in eventObj)
                    {
                        if (eventObj.Type != Event.EventType.RESPONSE &&
                            eventObj.Type != Event.EventType.PARTIAL_RESPONSE)
                        {
                            continue;
                        }

                        Element fields = msg.GetElement(FIELD_DATA);
                        int numElements = fields.NumValues;

                        printHeader();
                        for (int i = 0; i < numElements; i++)
                        {
                            printField(fields.GetValueAsElement(i));
                        }
                        System.Console.WriteLine();
                    }
                    if (eventObj.Type == Event.EventType.RESPONSE) break;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Got Exception:" + ex);
                }
            }
        }

        private void printField(Element field)
        {
            String fldId, fldMnemonic, fldDesc;

            fldId = field.GetElementAsString(FIELD_ID);
            if (field.HasElement(FIELD_INFO))
            {
                Element fldInfo = field.GetElement(FIELD_INFO);
                fldMnemonic = fldInfo.GetElementAsString(FIELD_MNEMONIC);
                fldDesc = fldInfo.GetElementAsString(FIELD_DESC);

                System.Console.WriteLine(padString(fldId, ID_LEN) +
                                    padString(fldMnemonic, MNEMONIC_LEN) +
                                    padString(fldDesc, DESC_LEN));
            }
            else
            {
                Element fldError = field.GetElement(FIELD_ERROR);
                fldDesc = fldError.GetElementAsString(FIELD_MSG);

                System.Console.WriteLine("\n ERROR: " + fldId + " - " + fldDesc);
            }
        }

        private void printHeader()
        {
            System.Console.WriteLine(padString("FIELD ID", ID_LEN) +
                                padString("MNEMONIC", MNEMONIC_LEN) +
                                padString("DESCRIPTION", DESC_LEN));
            System.Console.WriteLine(padString("-----------", ID_LEN) +
                                padString("-----------", MNEMONIC_LEN) +
                                padString("-----------", DESC_LEN));
        }

        private static String padString(String str, int width)
        {
            if (str.Length >= width || str.Length >= PADDING.Length) return str;
            else return str + PADDING.Substring(0, width - str.Length);
        }

        private bool parseCommandLine(String[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                if (string.Compare(args[i], "-ip", true) == 0)
                {
                    d_serverHost = args[i + 1];
                    ++i;
                }
                else if (string.Compare(args[i], "-p", true) == 0)
                {
                    int outPort = 0;
                    if (int.TryParse(args[i + 1], out outPort))
                    {
                        d_serverPort = outPort;
                    }
                    ++i;
                }
                else if (string.Compare(args[i], "-h", true) == 0)
                {
                    printUsage();
                    return (false);
                }
                else
                {
                    System.Console.WriteLine("Ignoring unknown option:" + args[i]);
                }
            }
            return (true);
        }

        private void printUsage()
        {
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine(
               "	Retrieve field information in categorized form ");
            System.Console.WriteLine("		[-ip <ipAddress> default = "
                                          + d_serverHost + " ]");
            System.Console.WriteLine("		[-p  <tcpPort>   default = "
                                          + d_serverPort + " ]");
            System.Console.WriteLine("		[-h  print this message and quit]\n");
        }
    }
}
