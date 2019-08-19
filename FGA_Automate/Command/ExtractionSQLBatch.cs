using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CommandLine.Utility;
using log4net;
using FGA.Automate.Consumer;
using FGA.Automate.Config;
using FGA.Automate.Producer;
using System.Threading;
using System.Reflection;
using System.Deployment.Application;
using System.Globalization;
using FGA.SQLCopy;
using FGA.Automate.Dataconverter.Producer.Financial;
using FGA.Automate.Dataconverter.Producer;

namespace FGA.Automate.Command
{
    public class ExtractionSQLBatch : IntegrateCommand 
    {
        private static DateTime now;
        private Arguments CommandLine;

        /// <summary>
        /// Utilitaire pour convertir les chaines de carac d ascii en ANSI FR (codepage 863)
        /// </summary>
        /// <param name="par"></param>
        private static void ConvertANSI(ref string[] par)
        {
            for (int i = 0; i < par.Length; i++)
            {
                byte[] inArg = Encoding.ASCII.GetBytes(par[i]);
                string outArg = Encoding.GetEncoding(863).GetString(inArg);
                par[i] = outArg;
            }
        }

        public string usage()
        {
            StringBuilder sb = new StringBuilder("Usage: SQLextract.exe -sql=<chemin_sur_requete>\n");
            sb.AppendLine("Optionel: -xls=<chemin_sur_la_sortie_au_format_Excel>");
            sb.AppendLine("Optionel: -oxlsx=<chemin_sur_la_sortie_au_format_OpenXML_Excel>");
            sb.AppendLine("Optionel: -bp2s=<chemin_sur_la_sortie_au_format_CSV_compatible BP2S>");
            sb.AppendLine("Optionel: -bp2sregex=<contenu de la validation des champs>");
            sb.AppendLine("Optionel: -ini=<chemin_sur_la_config_de_connexion base de données>");
            sb.AppendLine("Optionel: -datastore=nom de la BDD parmi : OMEGA(defaut) OMEGA_RECETTE FGA_PROD FGA_RECETTE");
            sb.AppendLine("Optionel: -@XXX=YYY où XXX est le nom d une variable SQL et YYY la valeur attribuée\n");
            sb.AppendLine("Optionel: -#XXX=YYY où XXX est le nom d une variable utiliser par un producer\n");
            sb.AppendLine("Exemple: SQLextract.exe -sql=\"G:\\,FGA Middle office\\OTC Forward-Cadrage\\BP2S\\1.Génération_fichier_BP2S\\Requête_quoti_dev.sql\"");
            sb.AppendLine("                        -xls=\"G:\\,FGA Middle office\\OTC Forward-Cadrage\\BP2S\\1.Génération_fichier_BP2S\\spécs\\Dev\\OTCTrades-YYYYMMDD.xls\"");
            sb.AppendLine("                        -bp2s=\"G:\\,FGA Middle office\\OTC Forward-Cadrage\\BP2S\\1.Génération_fichier_BP2S\\spécs\\Dev\\FDR-OTCTrade-YYYYMMDD.txt\"");
            sb.AppendLine("                        -bp2sregex=\"Resources\\Config\\BP2StextValidation.txt\"");
            sb.AppendLine("                        -ini=\"Resources\\Config\\login.ini\" (par defaut: le fichier login.ini de l'application sur G:\\FGA_SOFT");
            return sb.ToString();
        }
   
        public void Execute(Arguments cmdLine)
        {

            this.CommandLine = cmdLine;
            // Conversion des paramètres entrées (ANSI to format FR)
            //ConvertANSI(ref args);

            Thread oThread = null;

            // lecture de la configuration
            now = DateTime.Now;

            // Affichage de la version du batch
            IntegratorBatch.InfoLogger.Debug(now + " DEBUT PROGRAMME");
#if ! DEBUG
            Assembly assem = Assembly.GetEntryAssembly();
            Assembly execAssem = Assembly.GetExecutingAssembly();
            AssemblyName assemName = assem.GetName();
            Version ver = assemName.Version;
            Version execVer = execAssem.GetName().Version;

            String message = "Application " + assemName.Name + " , Version(entry,exec,public) " + ver.ToString() + " " + execVer.ToString();
            try
            {
                Version publishVer = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                message += " " + publishVer.ToString();
            }
            catch
            {
                message += " non disponible";
            }
            IntegratorBatch.InfoLogger.Info(message);
#endif            

            string[] inutile = CommandLine.Intercept(new string[] { "csvTransfer", "bondPricer", "dbCopy", "csv", "oxlsx", "sql", "paramsql", "xls", "bp2s", "bp2sregex", "ini", "datastore", "nogui" });
            // afficher les parametres passés et inutiles
            // prendre ceux qui commencent par @xxx ou #xxx qui représentent les variables des connecteurs Producers/consumers
            if (inutile.Length > 0)
            {
                if (IntegratorBatch.InfoLogger.IsInfoEnabled)
                {
                    string liste = "(";
                    foreach (string s in inutile)
                    {
                        if (!s.StartsWith("@") && !s.StartsWith("#"))
                            liste += s + " ";
                    }
                    liste += ")";
                    if (liste.Length > 2)
                        IntegratorBatch.InfoLogger.Info("Les parametres suivants ne sont pas exploitees: " + liste);
                }
            }

            //------------------------------------------------------------------------------------------
            // programme executé avec des traces en lignes de commande
            if (CommandLine["nogui"] == null)
            {
                Console.WriteLine("FGA Automate: Execution d une extraction :");
                Console.WriteLine(CommandLine.ToString());

                oThread = new Thread(new ThreadStart(ConsoleSpiner.Go));
                oThread.Start();
            }

            //------------------------------------------------------------------------------------------
            // fichier de configuration des paramètres de base
            if (CommandLine["ini"] != null)
            {
                IntegratorBatch.InfoLogger.Debug("Lecture d un fichier de configuration " + CommandLine["ini"]);
                InitFile.loginIni = CommandLine["ini"];
            }
            else
            {
                IntegratorBatch.InfoLogger.Debug("Lecture d un fichier de configuration par defaut " + InitFile.loginIni);
            }

            //------------------------------------------------------------------------------------------
            // fichier de configuration des paramètres de base
            string datastore = "OMEGA";
            if (CommandLine["datastore"] != null)
            {
                IntegratorBatch.InfoLogger.Debug("La base de donnees utilisee " + CommandLine["datastore"]);
                datastore = CommandLine["datastore"];
            }
            //------------------------------------------------------------------------------------------
            // option -@xxxx=yyyy : recueillir les parametres personnalises
            string[] keys, values;
            // tester si il y a des pararametres personnalise
            if (CommandLine.GetStartsWith("@", out keys, out values) > 0)
            {
                IntegratorBatch.InfoLogger.Debug("Lecture d une requete avec parametres : " + keys);
            }
            //------------------------------------------------------------------------------------------
            // option -paramsql : executer la requete fournie afin de recuperer une liste de parametres/valeurs sous la forme d 'un tableau
            string paramsql = CommandLine["paramsql"];
            object[] paramsValue = null;
            string paramsName = null;
            if (paramsql != null)
            {
                DataSet DS_Params = new DataSet();
                try
                {
                    SQLrequest p_1 = null;
                    // tester si il y a des pararametres personnalise
                    if (keys != null && keys.Length > 0)
                    {
                        p_1 = new SQLrequest(paramsql, datastore, keys, values);
                    }
                    else
                    {
                        p_1 = new SQLrequest(paramsql, datastore);
                    }                                        
                    p_1.Execute(out DS_Params);
                    // transformer le dataset en tableau pour le reutiliser sur la requete SQL principale
                    paramsValue = getParamsArray(DS_Params, out paramsName);
                }
                catch (Exception e)
                {
                    IntegratorBatch.ExceptionLogger.Fatal("Impossible d executer la requete " + paramsql, e);
                    throw e;
                }
            }


            //------------------------------------------------------------------------------------------
            // lire un fichier csv et le mettre dans une table BDD
            string csvTransfer = CommandLine["csvTransfer"];
            if (csvTransfer != null)
            {
                string nomTable = CommandLine["#nomTable"];
                string nomSchema = CommandLine["#nomSchema"]??"dbo";
                CSVtoSQL.DataBaseIntegrate(csvTransfer, datastore, nomSchema, nomTable);
            }
            //------------------------------------------------------------------------------------------


            if (paramsValue != null)
            {
                foreach (object extraValue in paramsValue)
                {
                    System.Collections.ArrayList _Keys = new System.Collections.ArrayList(keys);
                    _Keys.Add('@'+paramsName);
                    System.Collections.ArrayList _Values = new System.Collections.ArrayList(values);
                    _Values.Add(FGA.Automate.Helpers.Helper.ValueToSQLField(extraValue));
                    output(datastore,
                        (string[])_Keys.ToArray(typeof(string)),
                        (string[])_Values.ToArray(typeof(string)),
                         paramsName, extraValue.ToString());
                }
            }
            else
            {
                output(datastore,
                       keys,
                       values);
            }




            //------------------------------------------------------------------------------------------
            // programme executé avec des traces en lignes de commande
            if (CommandLine["nogui"] == null)
            {
                // Le traitement est terminé : arreter propremen la gauge
                ConsoleSpiner.go = false;
                oThread.Join();

                Console.WriteLine("Pressez une touche pour terminer");
                Console.ReadKey();
            }


            IntegratorBatch.InfoLogger.Debug(now + " FIN PROGRAMME");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datastore"></param>
        /// <param name="keys">les parametres utilisés dans la requete</param>
        /// <param name="values"></param>
        /// <param name="superParamKey">mot cle utilise dans les parametres de nom de fichiers etc...</param>
        /// <param name="superParamValue">la valeur du mot cle</param>
        private void output(string datastore, string[] keys = null, string[] values = null,
            string superParamKey = null, string superParamValue = null)
        {
            BP2SText bp2stext = null;
            string sql = CommandLine["sql"];
            DataSet DS = new DataSet();
            // fichier de la requete SQL utilisée pour extraire les données
            if (sql != null)
            {
                // formater les paramètres:
                SQLrequest p = null;
                try
                {                 
                    // tester si il y a des pararametres personnalise
                    if (keys != null && keys.Length > 0)
                    {
                        IntegratorBatch.InfoLogger.Debug("Lecture d une requete avec parametres : " + keys);
                        p = new SQLrequest(sql, datastore, keys, values);
                    }
                    else
                    {
                        p = new SQLrequest(sql, datastore);
                    }
                    p.Execute(out DS);
                }
                catch (Exception e)
                {
                    IntegratorBatch.ExceptionLogger.Fatal("Impossible d executer la requete " + sql, e);
                    throw e;
                }
            }
            else
            {
                Console.WriteLine(usage());
                IntegratorBatch.ExceptionLogger.Fatal("Parametre obligatoire -sql inexistant");
                // sortie car aucune requete à jouer
                return;
            }
            //------------------------------------------------------------------------------------------

            // declencher un calcul de BondPricer
            string bondPricer = CommandLine["bondPricer"];
            if (bondPricer != null)
            {
                BondPricer.OPTION opt;
                switch (bondPricer)
                {
                    case "DURATION": opt = BondPricer.OPTION.DURATION; break;
                    case "ACCRUEDINTEREST": opt = BondPricer.OPTION.ACCRUEDINTEREST; break;
                    case "CLEANPRICE": opt = BondPricer.OPTION.CLEANPRICE; break;
                    case "DIRTYPRICE": opt = BondPricer.OPTION.DIRTYPRICE; break;
                    default: opt = BondPricer.OPTION.ALL; break;
                }

                BondPricer bp = new BondPricer();
                DS = bp.Execute(DS, opt);
            }

            //------------------------------------------------------------------------------------------
            if (CommandLine["bp2s"] != null)
            {
                bp2stext = new BP2SText();
                string bp2sValid = (CommandLine["bp2sregex"] == null ? @"Resources/Consumer/BP2StextValidation.txt" : CommandLine["bp2sregex"]);
                IntegratorBatch.InfoLogger.Debug("Lecture d un fichier de validation des champs (regex) " + bp2sValid);

                string bp2s = CommandLine["bp2s"];
                bp2stext.ValidationPath = bp2sValid;

                IntegratorBatch.InfoLogger.Debug("Configuration pour une sortie BP2S " + bp2s);
                bp2s = bp2s.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));
                // remplacer le mot cle par sa valeur
                if( superParamKey != null)
                    bp2s = bp2s.Replace(superParamKey, superParamValue);

                bp2stext.CreateFile(DS, bp2s, now);
            }

            //------------------------------------------------------------------------------------------
            string excel = CommandLine["xls"];
            if (excel != null)
            {
                excel = excel.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));
                // remplacer le mot cle par sa valeur
                if (superParamKey != null)
                    excel = excel.Replace(superParamKey, superParamValue);

                IntegratorBatch.InfoLogger.Debug("Configuration pour une sortie Excel " + excel);

                ExcelFile.CreateWorkbook(DS, excel);
            }
            //------------------------------------------------------------------------------------------
            string openXMLExcel = CommandLine["oxlsx"];
            if (openXMLExcel != null)
            {
                openXMLExcel = openXMLExcel.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));
                // remplacer le mot cle par sa valeur
                if (superParamKey != null)
                    openXMLExcel = openXMLExcel.Replace(superParamKey, superParamValue);

                IntegratorBatch.InfoLogger.Debug("Configuration pour une sortie Open XML Excel " + openXMLExcel);

                // tester si il y a des pararametres personnalise
                string date = CommandLine["#date"] == null ? now.ToString("yyyyMMdd") : CommandLine["#date"];
                string sheetNames = CommandLine["#names"];
                string style = CommandLine["#style"];
                string template = CommandLine["#template"];
                string graph = CommandLine["#graph"];
                bool monoSheetFlag = ((CommandLine["#monoSheetFlag"] == null || CommandLine["#monoSheetFlag"] == "F" || CommandLine["#monoSheetFlag"] == "N") ? false : true);

                OpenXMLFile.CreateExcelWorkbook(DS, openXMLExcel, style, template, graph, date, sheetNames.Split(';'), monoSheetFlag);

            }
            //------------------------------------------------------------------------------------------
            string csv = CommandLine["csv"];
            if (csv != null)
            {
                string fileNames = CommandLine["#names"];
                if (fileNames != null)
                    fileNames = fileNames.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));
                else
                    fileNames = "";
                // remplacer le mot cle par sa valeur
                if (superParamKey != null)
                    fileNames = fileNames.Replace(superParamKey, superParamValue);

                CSVFile.WriteToCSV(DS, csv, fileNames);
            }
            //------------------------------------------------------------------------------------------
            //Si include et exclude sont vide : copie la BDD en entier
            //Si include et exclude non vide : aucune table ne sera copiée
            //Si include est vide : copie tout sauf les tables comprises dans exclude
            //Si exclude est vide : copie uniquement les tables dans include
            string bdd = CommandLine["dbCopy"];
            if (bdd != null)
            {
                MSDBIntegration.InfoLogger = IntegratorBatch.InfoLogger;
                MSDBIntegration.ExceptionLogger = IntegratorBatch.ExceptionLogger;

                string connection1 = CommandLine["#connectionSource"];
                string connection2 = CommandLine["#connectionDest"];
                // une seule liste autorisée
                List<string> liste =null;
                ListeMode mode;
                if (CommandLine["#include"] != null)
                {
                    liste = CommandLine["#include"].Split(';').ToList();
                    mode = ListeMode.Include;
                }
                else if (CommandLine["#include"] != null)
                {
                    liste = CommandLine["#exclude"].Split(';').ToList();
                    mode = ListeMode.Exclude;
                }
                else
                {
                    mode = ListeMode.Aucune;
                }

                List<string> nomTable = null;
                // si il y a une option -sql , le resultat de la requete sera mis dans la ou les tables nommées nomtable
                if (sql != null)
                {
                    nomTable = CommandLine["#nomTable"].Split(';').ToList();
                    DBCopy.DataBaseCopy(connection1, connection2, DS, nomTable);
                }
                else
                {
                    //TODO
                }                                
            }

            //------------------------------------------------------------------------------------------

        }



        private static NumberFormatInfo nfi = new NumberFormatInfo();
        /// <summary>
        /// Utilitaire transformant un dataset en un tableau d objects
        /// </summary>
        /// <param name="DS_Params"></param>
        /// <param name="ParamName"></param>
        /// <returns></returns>
        private static object[] getParamsArray(DataSet DS_Params, out string ParamName)
        {
            string columnName = null;
            object[] returns = null;
            foreach (DataTable table in DS_Params.Tables)
            {
                int nbColumns = 0;
                int nbRows = 0; // nb de lignes de données
                returns = new string[table.Rows.Count];
                foreach (DataRow row in table.Rows)
                {
                    nbColumns = 0;

                    foreach (DataColumn column in table.Columns)
                    {
                        Object field = row[column];
                        returns[nbRows] = field;
                        columnName = column.ColumnName;
                        nbColumns++;
                    }
                    nbRows++;
                }
            }
            ParamName = columnName;
            return returns;
        }


        //private static string FormatFieldValue(object field)
        //{


        //    string fieldString;

        //    if (field.GetType() == typeof(DateTime)
        //        || field.GetType() == typeof(SqlDateTime))
        //    {
        //        fieldString = "'" + ((DateTime)field).ToString("yyyy-MM-dd") + "'";
        //    }
        //    else if (field.GetType() == typeof(Decimal)
        //          || field.GetType() == typeof(SqlDecimal))
        //    {
        //        // mettre une , comme séparateur de decimal
        //        fieldString = ((Decimal)field).ToString("F2", nfi);
        //    }
        //    else if (field.GetType() == typeof(SqlMoney))
        //    {
        //        fieldString = ((SqlMoney)field).ToString();
        //    }
        //    else if (field.GetType() == typeof(SqlInt32))
        //    {
        //        fieldString = ((SqlInt32)field).ToString();
        //    }
        //    else
        //    {
        //        fieldString = field.ToString();
        //        fieldString = "'" + fieldString.TrimEnd() + "'";
        //    }
        //    return fieldString;
        //}
    }


    /// <summary>
    /// Utilitaire pour la gauge
    /// </summary>
    public static class ConsoleSpiner
    {
        public static bool go;
        static int counter;
        static ConsoleSpiner()
        {
            counter = 0;
            go = true;
        }
        public static void Go()
        {
            Console.Write("Merci de patienter quelques instants: ");
            while (go)
            {
                Turn();
                Thread.Sleep(100);
            }
            Console.WriteLine();
        }

        public static void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("-"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

    }
}
