using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;
using CommandLine;
using log4net.Config;
using log4net;
using System.Data;
using SQLCopy.Helpers.DataAdapter;
using FGA.SQLCopy;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using SQLCopy.Dbms;
using System.Text.RegularExpressions;
using FGA.Automate.Config;


namespace FGA.Automate.Command
{
    /// <summary>
    /// Code pour executer le process Action : integration des donnees Factset et calcul des agregats
    /// </summary>

    public class IntegrationPTF_Base : IntegrateCommand
    {

        public static ILog InfoLogger
        {
            get { return LogManager.GetLogger("IntegrationPTF_Base"); }
        }

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("IntegrationPTF_Base_ERREUR"); }
        }

        /// <summary>
        /// L environnement utilisé pour executer les requetes
        /// </summary>
        public static string ENV = "FGA_RW";

        public static void Main(string[] args)
        {
            Arguments CommandLine = new Arguments(args);
            new IntegrationPTF_Base().Execute(CommandLine);
        }
        public string usage()
        {
            StringBuilder sb = new StringBuilder("Usage: -dateStart=<date du fichier factset> -dateEnd=<end>\n");

            sb.AppendLine("-step1 -step2 -step3 -step4 -step5 les etapes declenchees pour l integration ");
            sb.AppendLine("-calculate execute le calcul des Z scores sur les aggregats sectoriels");
            sb.AppendLine("-factsetPath=chemin vers le fichier factset");
            sb.AppendLine("-modelClassificationPath=chemin vers le fichier Modele_Classification.xlsx");
            sb.AppendLine("-notationISR=chemin vers le fichier NotationISRbase.xlsx");
            sb.AppendLine("-actPtfAlimSQSLrequest=chemin vers le fichier FCP_Action_BaseTitresDirects.sql");
            sb.AppendLine("-env=<PROD ou PREPROD ou ... configuration de la base dans App.config>");
            return sb.ToString();
        }


        public void Execute(Arguments CommandLine)
        {
            string[] inutile = CommandLine.Intercept(new string[] { "dateStart", "dateEnd", "step1", "step2", "step3", "step4", "step5", "calculate", "factsetPath", "modelClassificationPath", "notationISR", "actPtfAlimSQSLrequest", "env" });
            // afficher les parametres passés et inutiles
            // prendre ceux qui commencent par @xxx ou #xxx qui représentent les variables
            if (inutile.Length > 0)
            {
                if (InfoLogger.IsInfoEnabled)
                {
                    string liste = "(";
                    foreach (string s in inutile)
                    {
                        if (!s.StartsWith("@") && !s.StartsWith("#"))
                            liste += s + " ";
                    }
                    liste += ")";
                    if (liste.Length > 2)
                        InfoLogger.Info("Les parametres suivants ne sont pas exploitees: " + liste);
                }
            }
            //------------------------------------------------------------------------------------------
            if (CommandLine["env"] != null)
            {
                ENV = CommandLine["env"];
            }
            //------------------------------------------------------------------------------------------
            // 2 configurations: dateStart et dateEnd au format JJ/MM/AAAA , donc toutes les dates comprises entre Start et End, sauf les WE
            //                   dateStart et dateEnd au format MM/AAAA , donc toutes les fins de mois comprises entre Start et End
            //------------------------------------------------------------------------------------------
            DateTime dStart, dEnd, dEom;
            bool eom_config = false;
            DateTime.TryParse(CommandLine["dateStart"], out dStart);
            if (!DateTime.TryParse(CommandLine["dateEnd"], out dEnd))
            {
                dEnd = dStart;
            }
            if (CommandLine["dateStart"].Length <= 7)
            {
                eom_config = true;
                if (CommandLine["dateEnd"].Length <= 7)
                {
                    // take the next 1rst day of month
                    dEnd = dEnd.AddMonths(1);
                    if (dEnd.DayOfWeek == DayOfWeek.Saturday)
                    {
                        dEnd = dEnd.AddDays(2);
                    }
                    else if (dEnd.DayOfWeek == DayOfWeek.Sunday)
                    {
                        dEnd = dEnd.AddDays(1);
                    }
                }
            }

            dEom = dStart;

            for (DateTime dateOfData = dStart; dateOfData <= dEnd; dateOfData = dateOfData.AddDays(1))
            {
                InfoLogger.Info("Donnees du " + dateOfData + " en cours: " + DateTime.Now.ToString());

                if (dateOfData.DayOfWeek == DayOfWeek.Saturday || dateOfData.DayOfWeek == DayOfWeek.Sunday)
                {
                    InfoLogger.Info("La date est un WE. Pas d integration " + dateOfData);
                    continue;
                }
                if (eom_config)
                {
                    // if the date end of month is the previous month, 
                    if ((dateOfData.Month == 1 && dEom.Month == 12) || dateOfData.Month > dEom.Month)
                    {
                        try
                        {
                            ACTION_PROCESS(dEom, CommandLine);
                        }
                        catch (DirectoryNotFoundException e)
                        {
                            InfoLogger.Error("File not found ... continue",e);
                        }
                    }
                }
                else
                {
                    ACTION_PROCESS(dateOfData, CommandLine);
                }
                dEom = dateOfData;
            }
        }


        public static void ACTION_PROCESS(DateTime d, Arguments CommandLine)
        {
            string date = d.ToString("dd/MM/yyyy");
            InfoLogger.Error("PROCESSING DATE" + date);
            //------------------------------------------------------------------------------------------
            if (CommandLine["histo"] != null)
            {
                string root = CommandLine["factsetPath"] ?? @"\\vill1\Partage\TQA\Datas Factset pour Guillaume";
                string filepath = root + @"\"+CommandLine["histo"];
                ACTION_PROCESS_BulkCopy_DATA_FACTSET_DATA_1(filepath);
            }

            //------------------------------------------------------------------------------------------
            if (CommandLine["step1"] != null)
            {
                string root = CommandLine["factsetPath"] ?? @"\\vill1\Partage\,FGA MarketData\FACTSET";
                string filepath = root + @"\" + d.ToString("yyyyMM") + @"\base_" + d.ToString("yyyyMMdd") + @".csv";
                ACTION_PROCESS_BulkCopy_DATA_FACTSET_DATA_1(filepath);
            }
            //------------------------------------------------------------------------------------------
            if (CommandLine["step2"] != null)
            {
                string filepath = CommandLine["modelClassificationPath"] ?? @"\\vill1\Partage\,FGA Front Office\02_Gestion_Actions\00_BASE\Base 2.0\Modele_Classification.xlsx";

                ACTION_PROCESS_BulkCopy_FACTSET_MODELE_CLASSIFICATION_2(date, filepath);
            }


            //------------------------------------------------------------------------------------------
            if (CommandLine["step3"] != null)
            {
                string filepath = CommandLine["notationISR"] ?? @"\\vill1\Partage\,FGA ISR\Notation Fédéris\NotationISRbase.xlsx";
                try
                {
                    ACTION_PROCESS_BulkCopy_IMPORT_ISR_3(filepath);
                }
                catch (System.Data.SqlClient.SqlException sqle)
                {
                    if (sqle.ErrorCode == 2627) // violation Primary Key
                    {
                        InfoLogger.Info("Data already inserted");
                    }
                    InfoLogger.Error("Problem on " + filepath, sqle);
                }
                catch (Exception e)
                {
                    InfoLogger.Error("Problem on " + filepath, e);
                }
            }
            //------------------------------------------------------------------------------------------
            if (CommandLine["step4"] != null)
            {
                string filePath = CommandLine["actPtfAlimSQSLrequest"] ?? @"\\vill1\Partage\,FGA Soft\SQL_SCRIPTS\AUTOMATE\GESTION_ACTION\FCP_Action_BaseTitresDirects.sql";
                ACTION_PROCESS_BulkCopy_Import_ACT_PTF_BaseTitreDirects_4(filePath, date);
            }

            if (CommandLine["step5"] != null)
            {
                ACTION_PROCESS_BulkCopy_Enrich_5(date);
            }
            if (CommandLine["calculate"] != null)
            {
                int to;
                if (!Int32.TryParse(CommandLine["timeout"], out to))
                {
                    to = 60 * 60 * 3; // 3 heures par defaut
                }
                ACTION_PROCESS_BulkCopy_Calculate_6(date, to);
            }
        }

        public static void ACTION_PROCESS_BulkCopy_DATA_FACTSET_DATA_1(string filepath)
        {
            DataSet ds = new DataSet("DATA_FACTSET");
            MSSQL2005_DBConnection dest = new MSSQL2005_DBConnection(ENV);

            using (var reader = new CsvReader(new StreamReader(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)), true, '|'))
            {
                reader.MissingFieldAction = MissingFieldAction.ReplaceByEmpty;
                CsvDataAdapter adapter = new CsvDataAdapter(reader);
                // TODO : ajouter un objet de mapping sur le csvDataApdapter
                int nbReadLines = adapter.Fill(dest, new DatabaseTable("dbo", "DATA_FACTSET"));

            }
        }

        public static void ACTION_PROCESS_BulkCopy_FACTSET_MODELE_CLASSIFICATION_2(string date, string filepath)
        {
            DataSet ds = new DataSet("DATA_FACTSET");
            MSDBIntegration s = new MSDBIntegration(destinationConnection: ENV);

            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(filepath, "Modele Classification", "B1", "V926", true);

            adapter.AddColumnMapping("DATA_FACTSET", "MXEU", "MXEU", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXUSLC", "MXUSLC", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXEM", "MXEM", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXEUM", "MXEUM", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXFR", "MXFR", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100001", "6100001", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100004", "6100004", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100030", "6100030", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100033", "6100033", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100063", "6100063", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "AVEURO", "AVEURO", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "AVEUROPE", "AVEUROPE", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100026", "6100026", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100062", "6100062", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100002", "6100002", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100024", "6100024", System.Type.GetType("System.Double"));

            adapter.FillColumnWithValue("DATE", date);

            adapter.Fill(ds);

            DataTable dt = ds.Tables["DATA_FACTSET"];

            s.bulkcopyData(dt);

        }

        public static void ACTION_PROCESS_BulkCopy_IMPORT_ISR_3(string filepath)
        {
            DataSet ds = new DataSet("ISR_NOTE");
            MSDBIntegration s = new MSDBIntegration(destinationConnection: ENV);

            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(filepath, "ISR", "A2", "F3000", true);
            adapter.AddColumnMapping("ISR_NOTE", "Note Actions", "Note Actions", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("ISR_NOTE", "Note Credit", "Note Credit", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("ISR_NOTE", "date ", "DATE", System.Type.GetType("System.DateTime"));
            adapter.AddColumnMapping("ISR_NOTE", "Name", "NAME");

            adapter.Fill(ds);

            DataTable dt = ds.Tables["ISR_NOTE"];
            s.bulkcopyData(dt);
        }


        public static string parameterRequest(string fileName, string[] parameters, string[] values)
        {
            // lecture du fichier de requete
            string request = InitFile.ReadFile(fileName);

            // on remplace le set @xxx=zzz de request par set @xxx=yyy passée en parametre
            for (int i = 0; i < parameters.Length; i++)
            {
                string pattern = @"set\s+" + parameters[i] + @"\s*=.*";

                if (values[i] != null && values[i].Trim().Length > 0)
                {
                    // gestion de la constante NOW 
                    if (values[i].Contains("NOW"))
                    {
                        values[i] = values[i].Replace("NOW", "GetDate()");
                    }
                    string replacement = "set " + parameters[i] + "=" + values[i];
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    request = rgx.Replace(request, replacement);
                }
            }
            return request;
        }


        public static void ACTION_PROCESS_BulkCopy_Import_ACT_PTF_BaseTitreDirects_4(string filePath, string date = null)
        {

            MSDBIntegration s = new MSDBIntegration("OMEGA", ENV);
            string request;
            if (date != null)
            {
                request = parameterRequest(filePath, new string[] { "@dateDemandee" }, new string[] { "'" + date + "'" });
            }
            else
            {
                request = parameterRequest(filePath, new string[] { }, new string[] { });
            }

            s.bulkcopySourceRequest(request, new DatabaseTable("ACT_PTF"));
        }


        public static void ACTION_PROCESS_BulkCopy_Enrich_5(string date)
        {
            string procStock = @"ACT_DailyImport";
            DBConnectionDelegate d = new MSSQL2005_DBConnection(ENV);
            DataSet ds = d.Execute("Execute " + procStock + " '" + date + "'", connection_timeout: 60 * 60 * 2);
        }

        public static void ACTION_PROCESS_BulkCopy_Calculate_6(string date, int timeout)
        {
            string procStock = @"ACT_Agregats_Secteur";
            DBConnectionDelegate d = new MSSQL2005_DBConnection(ENV);
            DataSet ds = d.Execute("Execute " + procStock + " '" + date + "'", connection_timeout: timeout);
        }

        // TODO: A TESTER 
        public static void ACTION_PROCESS_BulkCopy_FACTSET_TICKER_CONV()
        {
            string date = "17/01/2014";
            string filepath1 = @"G:\,FGA Front Office\02_Gestion_Actions\00_BASE\Base 2.0\TickerConversion.xlsx";
            string filepath2 = @"C:\TickerConversion.xlsx";


            DataSet ds = new DataSet("ACT_TICKER_CONVERSION");
            MSDBIntegration s = new MSDBIntegration(destinationConnection: "FGA_PREPROD_RW");

            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(filepath2, "Feuil1", "B3", "F51", true);

            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "ISIN", "ISIN");
            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "TICKER", "TICKER");
            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "BBG", "BBG");
            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "EXCH_F", "EXCH_B");

            adapter.Fill(ds);

            DataTable dt = ds.Tables["ACT_TICKER_CONVERSION"];

            s.bulkcopyData(dt);

        }
    }
}
