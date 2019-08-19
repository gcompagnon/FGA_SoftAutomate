

using CommandLine.Utility;
using FGA.Automate.Command;
using System.Linq;
using log4net;
using log4net.Config;
/// // lecture du fichier de configuration pour les LOG
[assembly: XmlConfigurator(ConfigFile = @"Resources\Config\log4net.config", Watch = false)]

namespace FGA.Automate
{
    /// <summary>
    /// Classe principale du prog en ligne de commande/batch pour l intégration
    /// </summary>

    public class IntegratorBatch
    {

        public static ILog InfoLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_Automate"); }
        }

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_ERREUR"); }
        }

        public static void Main(string[] args)
        {
            Arguments CommandLine = new Arguments(args);
            if ((CommandLine["sql"] != null) || (CommandLine["csvTransfer"] != null) || (CommandLine["bondPricer"] != null))
            {
                new ExtractionSQLBatch().Execute(CommandLine);
            }
            else if (CommandLine["msci_files_path"] != null)
            {
                new IntegrationMSCI_OMEGA().Execute(CommandLine);
            }
            else if ((CommandLine["msci"] != null) || (CommandLine["iboxx"] != null) || (CommandLine["barclays"] != null))
            {
                new IntegrationINDEX_Base().Execute(CommandLine);
            }
            else if ( (CommandLine["step1"] != null) || (CommandLine["step2"] != null) ||(CommandLine["step3"] != null)||(CommandLine["step4"] != null)||(CommandLine["step5"] != null) ||
                (CommandLine["calculate"] != null) ||
                (CommandLine["histo"] != null))
            {
                new IntegrationPTF_Base().Execute(CommandLine);
            }
            else
            {
                ExceptionLogger.Fatal("Pas de parametres corrects\n" + new ExtractionSQLBatch().usage());
            }
        }
    }
}
