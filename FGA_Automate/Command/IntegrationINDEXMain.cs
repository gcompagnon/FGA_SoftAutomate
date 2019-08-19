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
using FGA.Automate.IndexIntegration;

namespace FGA.Automate.Command
{
    /// <summary>
    /// Code pour executer l integration des indices
    /// </summary>
    public class IntegrationINDEX_Base : IntegrateCommand
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
        public static string ENV = "PROD";

        private static Arguments CommandLine;

        //public static void Main(string[] args)
        //{
        //    CommandLine = new Arguments(args);

        //    new IntegrationINDEX_Base().Execute(CommandLine);

        //}
        public string usage()
        {
            StringBuilder sb = new StringBuilder("Usage: -dateStart=<a partir de date incluse>\n");
            sb.AppendLine("-dateEnd=<jusqu a la date incluse>");
            sb.AppendLine("-msci=<Index Integration MSCI>");
            sb.AppendLine("-barclays=<Index Integration BARCLAYS Nominal>");
            sb.AppendLine("-iboxx=<Index Integration Markit IBOXX>");
            sb.AppendLine("-env=<PROD ou PREPROD ou ... configuration de la base dans App.config>");
            return sb.ToString(); 
        }
        public void Execute(Arguments CommandLine)
        {

            string[] inutile = CommandLine.Intercept(new string[] { "dateStart", "dateEnd", "msci", "iboxx", "barclays", "env", "ROOT_PATH", "INDEX", "INDEX_UNIVERSE" });
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
            else
            {
                ENV = "PREPROD";
            }
            //------------------------------------------------------------------------------------------
            if (CommandLine["msci"] != null)
            {
                if (CommandLine["dateStart"] != null)
                {
                    DateTime d1;
                    if (DateTime.TryParse(CommandLine["dateStart"], out d1))
                    {
                        DateTime d2;
                        DateTime.TryParse(CommandLine["dateEnd"], out d2);

                        //string root = CommandLine["factsetPath"] ?? @"\\vill1\Partage\,FGA MarketData\FACTSET";                        
                        MSCIIndexFile f = new MSCIIndexFile(ENV);
                        f.ExecuteIndexFileIntegration(d1, d2);
                    }
                }
            }
            //------------------------------------------------------------------------------------------
            if (CommandLine["iboxx"] != null)
            {
                if (CommandLine["dateStart"] != null)
                {
                    DateTime d1;
                    if (DateTime.TryParse(CommandLine["dateStart"], out d1))
                    {
                        DateTime d2;
                        DateTime.TryParse(CommandLine["dateEnd"], out d2);

                        //string root = CommandLine["factsetPath"] ?? @"\\vill1\Partage\,FGA MarketData\FACTSET";                        
                        iBoxxIndexFile f = new iBoxxIndexFile(ENV);
                        f.ExecuteIndexFileIntegration(d1, d2);
                    }
                }
            }


            //------------------------------------------------------------------------------------------
            if (CommandLine["barclays"] != null)
            {
                if (CommandLine["dateStart"] != null)
                {
                    DateTime d1;
                    if (DateTime.TryParse(CommandLine["dateStart"], out d1))
                    {
                        DateTime d2;
                        DateTime.TryParse(CommandLine["dateEnd"], out d2);
                        
                        String root_path = CommandLine["ROOT_PATH"] ?? BarclaysIndexFile.INDEX_PATH;

                        //string root = CommandLine["factsetPath"] ?? @"\\vill1\Partage\,FGA MarketData\FACTSET";                        
                        BarclaysIndexFile f = new BarclaysIndexFile(ENV);
                        f.ExecuteIndexFileIntegration(d1, d2, new object[] { root_path, CommandLine["INDEX_UNIVERSE"], CommandLine["INDEX"] });
                    }
                }
            }

            //------------------------------------------------------------------------------------------


        }





    }
}
