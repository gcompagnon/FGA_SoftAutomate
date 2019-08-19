using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine.Utility;
using CommandLine;
using log4net.Config;
using log4net;
using System.Data;
using System.Xml.Serialization;
using SQLCopy.Helpers;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using FGA.Automate.Consumer;

/// <summary>
/// En attentant l integration dans FGAAutomate : par injection de dépendances
/// </summary>

namespace FGA.Automate.Command
{
    internal class MSCI_SETTINGS
    {
        public DateTime RUNNING_TIME = DateTime.Now;
        public string USED_DATE;
        public string PREFIX_DATE;
        public string rootPath;
        public string rootPath_DM;
        public string indexType_DM;
        public string rootPath_EAFE;
        public string indexType_EAFE;
        public string rootPath_AMER;
        public string indexType_AMER;
        public string rootPath_DM_ACE;
        public string indexType_DM_ACE;
    }

    internal class XSDSpec
    {
        public Type xsdClass;
        public string xsdFile;
        public string xmlDataFile;
    }

    public class IntegrationMSCI_OMEGA : IntegrateCommand
    {

        internal MSCI_SETTINGS MSCI_Settings;
        internal ISet<string> excluded_security_ticker;

        /// <summary>
        /// Build the settings object using MSCI paths
        /// </summary>
        /// <param name="DATE">au format YYYYMMDD</param>
        private MSCI_SETTINGS GetPathSettings(string DATE, string MSCI_FILES_PATH = @"G:\,FGA MarketData\INDICES\MSCI\")
        {
            MSCI_Settings = new MSCI_SETTINGS();
            MSCI_Settings.USED_DATE = DATE;
            MSCI_Settings.PREFIX_DATE = DATE + "_" + DATE + "_";
            if (MSCI_FILES_PATH.EndsWith("\\"))
                MSCI_Settings.rootPath = MSCI_FILES_PATH + DATE + "\\";
            else
                MSCI_Settings.rootPath = MSCI_FILES_PATH + "\\" + DATE + "\\";

            // core_dm_daily
            MSCI_Settings.rootPath_DM = MSCI_Settings.rootPath + DATE + "core_dm_daily_d";
            MSCI_Settings.indexType_DM = "core_dm_daily";
            // core_eafe_daily
            MSCI_Settings.rootPath_EAFE = MSCI_Settings.rootPath + DATE + "core_eafe_daily_d";
            MSCI_Settings.indexType_EAFE = "core_eafe_daily";
            // core_amer_daily
            MSCI_Settings.rootPath_AMER = MSCI_Settings.rootPath + DATE + "core_amer_daily_d";
            MSCI_Settings.indexType_AMER = "core_amer_daily";

            // core_amer_daily
            MSCI_Settings.rootPath_DM_ACE = MSCI_Settings.rootPath + DATE + "core_dm_ace";
            MSCI_Settings.indexType_DM_ACE = "core_dm_ace";
            return MSCI_Settings;
        }

        #region Logger
        public static ILog InfoLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_MSCI_EXTRACTION"); }
        }

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_MSCI_EXTRACTION"); }
        }

        #endregion

        private static Arguments CommandLine;

        public IntegrationMSCI_OMEGA()
        {
        }

        public static void Main(string[] args)
        {
            CommandLine = new Arguments(args);
            new IntegrationMSCI_OMEGA().Execute(CommandLine);
        }

        public string usage()
        {
            StringBuilder sb = new StringBuilder("Usage: -date=<date_de_fichier>\n");
            sb.AppendLine("-msci_files_path=<repertoire ou se trouvent les fichiers MSCI>");
            sb.AppendLine("-fichier_cours_xls=<chemin_sur_le_fichier des coursMSCI_au_format_Excel>");
            sb.AppendLine("-fichier_devises_xls=<chemin_sur_le fichiers des devises_au_format_Excel>");
            sb.AppendLine("-liste_exclusion_prix=<tickers bloomberg que l on ne veut pas dans le fichier sortie par exemple: 5 HK Equity,RDSA LN Equity>");
            return sb.ToString();
        }
        /// <summary>
        /// PROGRAMME EXECUTE 
        /// Parametres: fichier_cours_xls et/ou fichier_prix_xls pour les extractions en cours devises et prix des composants
        /// msci_files_path pour le chemin vers les fichiers sources (par defaut:G:\,FGA Systèmes\PRODUCTION\FTP\INDICES\MSCI\)
        /// date pour la date utilisée
        /// liste_exclusion_prix pour une liste des tickers bloomberg à exclure
        /// </summary>
        /// <param name="args"></param>
        public void Execute(Arguments CommandLine)
        {
            string[] inutile = CommandLine.Intercept(new string[] { "fichier_cours_csv","fichier_cours_xls", "fichier_devises_csv","fichier_devises_xls", "date", "msci_files_path", "liste_exclusion_prix" });
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

            DataSet DS_fichier_cours = null;
            DataSet DS_fichier_devises = null;
            //------------------------------------------------------------------------------------------
            if (CommandLine["date"] != null)
            {
                if (CommandLine["msci_files_path"] != null)
                    this.MSCI_Settings = this.GetPathSettings(CommandLine["date"], CommandLine["msci_files_path"]);
                else
                    this.MSCI_Settings = this.GetPathSettings(CommandLine["date"]);


                // Soit extraction des cours MSCI                 
                if ( (CommandLine["fichier_cours_xls"] != null) || (CommandLine["fichier_cours_csv"] != null) )
                {
                    if (CommandLine["liste_exclusion_prix"] != null)
                    {
                        string [] excluded = CommandLine["liste_exclusion_prix"].Split(new char[]{',',';',':'});
                        this.excluded_security_ticker = new HashSet<string>(excluded);
                    }
                    DS_fichier_cours = this.Extract_ZIPFile_MSCI_Indexes_Daily_Security();
                }

                if (CommandLine["fichier_cours_xls"] != null)
                {
                    string outputFileName = this.ParameterFileNameWithDateTime(CommandLine["fichier_cours_xls"], this.MSCI_Settings.RUNNING_TIME);
                    ExcelFile.CreateWorkbook(DS_fichier_cours, outputFileName);
                }
                if (CommandLine["fichier_cours_csv"] != null)
                {
                    string outputFileName = this.ParameterFileNameWithDateTime(CommandLine["fichier_cours_csv"], this.MSCI_Settings.RUNNING_TIME);
                    CSVFile.WriteToCSV(DS_fichier_cours, outputFileName);

                }

                // Soit extraction des devises MSCI
                if ( (CommandLine["fichier_devises_xls"] != null)|| (CommandLine["fichier_devises_csv"] != null) )
                {
                    DS_fichier_devises = this.Extract_ZIPFile_MSCI_Indexes_Daily_ForexRate();
                }

                
                if (CommandLine["fichier_devises_xls"] != null)
                {
                    string outputFileName = this.ParameterFileNameWithDateTime(CommandLine["fichier_devises_xls"], this.MSCI_Settings.RUNNING_TIME);
                    ExcelFile.CreateWorkbook(DS_fichier_devises, outputFileName);
                }
                if (CommandLine["fichier_devises_csv"] != null)
                {
                    string outputFileName = this.ParameterFileNameWithDateTime(CommandLine["fichier_devises_csv"], this.MSCI_Settings.RUNNING_TIME);                    
                    CSVFile.WriteToCSV(DS_fichier_devises, outputFileName);
                }

            }
        }

        /// <summary>
        /// Make the substitution of the filename using DateTime
        /// YYYYMMDD in the value of year, ... of DateTime : as 20131125 
        /// HHmmss in the value of time, ... of DateTime: as 153500
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private String ParameterFileNameWithDateTime(string input, DateTime RUNNING_TIME)
        {
            string output = input.Replace("YYYYMMDD", RUNNING_TIME.ToString("yyyyMMdd"));
            output = output.Replace("hhmmss", RUNNING_TIME.ToString("HHmmss"));
            return output;
        }

        #region Extract ALL ZIP FILES
        public void Extract_ALL_ZIPFile_ALL_MSCI_Packages()
        {
            this.Extract_DM_ZIPFile_ALL_MSCI_Packages();
            this.Extract_AMER_ZIPFile_ALL_MSCI_Packages();
            this.Extract_EAFE_ZIPFile_ALL_MSCI_Packages();
            this.Extract_ACE_ZIPFile_ALL_MSCI_Packages();
        }

        public void Extract_DM_ZIPFile_ALL_MSCI_Packages()
        {

            XSDSpec[] xsdClasses = new XSDSpec[]
                {
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D_5D),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_COUNTRY_FXRATE_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_COUNTRY_FXRATE_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D51D), 
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_INDEX_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_INDEX_MAIN_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D80DCTY),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_INDEX_WEIGHT_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_INDEX_WEIGHT_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D60DDVD),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_ADVD_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_ADVD_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D98D),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_CODE_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_CODE_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D16D),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_DTR_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_DTR_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml"
            }};

            string indexType = this.MSCI_Settings.indexType_DM;
            string xmlPath = this.MSCI_Settings.rootPath;
            string zipDataFile = this.MSCI_Settings.USED_DATE + "core_dm_daily_d.zip";

            Object[] o = this.GetAllPackageClassesInZIPFile(xmlPath, xsdClasses, zipDataFile);
            Console.WriteLine(o.Length);


        }
        public void Extract_EAFE_ZIPFile_ALL_MSCI_Packages()
        {

            XSDSpec[] xsdClasses = new XSDSpec[]
                {
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D_5F),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_COUNTRY_FXRATE_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_COUNTRY_FXRATE_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D51F), 
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_INDEX_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_INDEX_MAIN_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D80FCTY),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_INDEX_WEIGHT_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_INDEX_WEIGHT_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D60FDVD),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_SECURITY_ADVD_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_SECURITY_ADVD_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D98F),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_SECURITY_CODE_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_SECURITY_CODE_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D15F),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_SECURITY_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_EAFE_SECURITY_MAIN_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml"
            }};



            string indexType = this.MSCI_Settings.indexType_EAFE;
            string xmlPath = this.MSCI_Settings.rootPath;
            string zipDataFile = this.MSCI_Settings.USED_DATE + "core_eafe_daily_d.zip";

            Object[] o = this.GetAllPackageClassesInZIPFile(xmlPath, xsdClasses, zipDataFile);
            Console.WriteLine(o.Length);
        }
        public void Extract_AMER_ZIPFile_ALL_MSCI_Packages()
        {

            XSDSpec[] xsdClasses = new XSDSpec[]
                {
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D_5A),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_COUNTRY_FXRATE_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_COUNTRY_FXRATE_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D51A), 
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_INDEX_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_INDEX_MAIN_DAILY_D.xml"
            },
            new XSDSpec 
            {
                                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D80ACTY),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_INDEX_WEIGHT_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_INDEX_WEIGHT_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D60ADVD),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_ADVD_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_ADVD_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D98A),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_CODE_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_CODE_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D16A),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_DTR_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_DTR_DAILY_D.xml"
            },
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D15A),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_MAIN_DAILY_D.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_AMER_SECURITY_MAIN_DAILY_D.xml"
            }};
            string indexType = this.MSCI_Settings.indexType_AMER;
            string xmlPath = this.MSCI_Settings.rootPath;
            string zipDataFile = this.MSCI_Settings.USED_DATE + "core_amer_daily_d.zip";

            Object[] o = this.GetAllPackageClassesInZIPFile(xmlPath, xsdClasses, zipDataFile);
            Console.WriteLine(o.Length);
        }
        public void Extract_ACE_ZIPFile_ALL_MSCI_Packages()
        {

            XSDSpec[] xsdClasses = new XSDSpec[]
                {
            new XSDSpec 
            {
                xsdClass = typeof(MSCIBarra_EquityIndex.core_dm_ace.package_DM_ACE ),
                xsdFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_SECURITY_ACE_DAILY.xsd",
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE +"CORE_DM_SECURITY_ACE_DAILY.xml"
            }};
            string indexType = this.MSCI_Settings.indexType_DM_ACE;
            string xmlPath = this.MSCI_Settings.rootPath;
            string zipDataFile = this.MSCI_Settings.USED_DATE + "core_dm_ace.zip";

            Object[] o = this.GetAllPackageClassesInZIPFile(xmlPath, xsdClasses, zipDataFile);
            Console.WriteLine(o.Length);
        }
        #endregion Extract ALL ZIP FILES


        public DataSet Extract_ZIPFile_MSCI_Indexes_Daily_ForexRate()
        {
            var x = new
            {
                indexType = this.MSCI_Settings.indexType_DM,
                xmlPath = this.MSCI_Settings.rootPath,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D_5D),
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE + "CORE_DM_ALL_COUNTRY_FXRATE_DAILY_D.xml",
                zipDataFile = this.MSCI_Settings.USED_DATE + "core_dm_daily_d.zip"
            };

            MSCIBarra_EquityIndex.package_D_5D p = (MSCIBarra_EquityIndex.package_D_5D)this.GetPackageClassInZIP(x.xmlPath, x.xsdClass, x.zipDataFile, x.xmlDataFile);

            DataSet DS = new DataSet();

            DS.Tables.Add(read(p));

            return DS;
        }

        private static DataTable read(MSCIBarra_EquityIndex.package_D_5D p)
        {
            DataTable DT = new DataTable("MSCI_D5D");

            string columnsDef = "CODEDEVISE;DEVISE;DATE;OPEN_4PMLONDON;CLOSE_4PMLONDON";
            foreach (string c in columnsDef.Split(';'))
            {
                DT.Columns.Add(c);
            }
            decimal? forexUSDEUR = null;

            //Console.WriteLine("ALL:  Nb of Securities: {0}", p.dataset_D15D.Count);
            foreach (MSCIBarra_EquityIndex.package_D_5DEntry e in p.dataset_D_5D)
            {
                if (e.ISO_currency_symbol == "EUR")
                    forexUSDEUR = e.spot_fx_eod00d;
            }

            if (forexUSDEUR == null)
                throw new ApplicationException("USDEUR forex not available ( " + p.dataset_D_5D.Count + " forex available ");

            foreach (MSCIBarra_EquityIndex.package_D_5DEntry e in p.dataset_D_5D)
            {
                if (e.ISO_currency_symbol != "EUR")
                {
                    decimal? cours = e.spot_fx_eod00d / forexUSDEUR;
                    string coursS = (cours ==null ?"NA": String.Format("{0:0.########}", cours));
                    DT.Rows.Add("EUR", e.ISO_currency_symbol, String.Format("{0:dd/MM/yyyy}", e.calc_date), coursS, coursS);
                }
            }

            return DT;
        }


        public DataSet Extract_ZIPFile_MSCI_Indexes_Daily_Security()
        {
            var x = new
            {
                indexType = this.MSCI_Settings.indexType_DM,
                xmlPath = this.MSCI_Settings.rootPath,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
                xmlDataFile = this.MSCI_Settings.PREFIX_DATE + "CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml",
                zipDataFile = this.MSCI_Settings.USED_DATE + "core_dm_daily_d.zip"
            };

            MSCIBarra_EquityIndex.package_D15D p = (MSCIBarra_EquityIndex.package_D15D)this.GetPackageClassInZIP(x.xmlPath, x.xsdClass, x.zipDataFile, x.xmlDataFile);
            DataSet DS = new DataSet();

            DS.Tables.Add(read(p,this.excluded_security_ticker));

            return DS;
        }

        private static DataTable read(MSCIBarra_EquityIndex.package_D15D p, ISet<string> excludedTickers = null)
        {
            DataTable DT = new DataTable("MSCI_D15D");

            string columnsDef = "DATE;ISIN;SEDOL;CODE_RIC;CODE_BLOOM;NOM;PAYS;DEVISE;SECTEUR;SOUSSECTEUR;COURS Ouverture;COURS Cloture;FLOTTANT;ACTIF_NET;ACTIF_NET_USD;PERF_PRICE;PERF_PRICE_USD;PERF_GROSS;PERF_GROSS_USD;PERF_NET;PERF_NET_USD;FACTEUR_AJUSTEMENT_PRIX";
            foreach (string c in columnsDef.Split(';'))
            {
                DT.Columns.Add(c);
            }

            //Console.WriteLine("ALL:  Nb of Securities: {0}", p.dataset_D15D.Count);
            foreach (MSCIBarra_EquityIndex.package_D15DEntry e in p.dataset_D15D)
            {
                if ((excludedTickers == null) || !excludedTickers.Contains(e.bb_ticker))
                {
                    DT.Rows.Add(String.Format("{0:dd/MM/yyyy}", e.calc_date), e.isin, e.sedol, e.RIC, e.bb_ticker,
                        e.security_name, e.ISO_country_symbol, e.price_ISO_currency_symbol, e.sector, e.sub_industry, e.price, e.price, e.closing_number_of_shares, e.initial_mkt_cap_loc_next_day,
                        e.initial_mkt_cap_usd_next_day,
                        e.price_return_loc, e.price_return_usd,
                        e.gross_return_loc, e.gross_return_usd,
                        e.net_return_intl_loc, e.net_return_intl_usd,
                        e.price_adjustment_factor);
                }
            }
            return DT;
        }

        /// <summary>
        /// Methode pour lire l object dans le répertoire XML
        /// Impossible d utilisr WindowsBase System.IO.Packaging car ZIP n'est pas implémenté dans toutes ses versions
        /// </summary>
        private Object GetPackageClassInZIP(string rootPath, Type xsdClass, string zipFile, string fileName)
        {
            InfoLogger.Info("StreamOpen '" + zipFile + "', search '" + fileName + "' with XSD '" + xsdClass + "'");
            XmlSerializer x = new XmlSerializer(xsdClass);
            StreamReader reader = null;
            ZipArchive package = new ZipArchive(rootPath + @"\" + zipFile, FileAccess.Read);

            try
            {
                foreach (ZipArchiveFile part in package.Files)
                {
                    if (fileName.Equals(part.Name))
                    {
                        reader = part.OpenText();
                        // the class object has only one field : a List object
                        Object datas = x.Deserialize(reader);
                        return datas;
                    }
                    InfoLogger.Info("Extracted " + part.Name);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
                x = null;
                package.Close();
                package = null;
            }
            InfoLogger.Info("GetPackageClassInZIP END");
            return null;
        }


        /// <summary>
        /// Methode pour lire l object dans le répertoire XML
        /// Impossible d utilisr WindowsBase System.IO.Packaging car ZIP n'est pas implémenté dans toutes ses versions
        /// </summary>
        private Object[] GetAllPackageClassesInZIPFile(string rootPath, XSDSpec[] xsdSpec, string zipFile)
        {
            InfoLogger.Info("StreamOpen '" + zipFile + "', search all files nb='" + xsdSpec.Length);

            Object[] output = new Object[xsdSpec.Length];
            int i = 0;
            Dictionary<string, XmlSerializer> xsd = new Dictionary<string, XmlSerializer>(xsdSpec.Length);

            foreach (XSDSpec xsdS in xsdSpec)
            {
                xsd.Add(xsdS.xsdFile, new XmlSerializer(xsdS.xsdClass));
            }
            ZipArchive package = new ZipArchive(rootPath + @"\" + zipFile, FileAccess.Read);

            foreach (ZipArchiveFile part in package.Files)
            {
                StreamReader sr = part.OpenText();

                using (XmlTextReader xmlReader = new XmlTextReader(sr))
                {



                    while (xmlReader.Read())
                    {
                        //looking for a XML element like this
                        //xsi:schemaLocation="http://sample.com/sample/contentExchange sample.0.6.2.xsd "

                        string schemaLocation;
                        if (xmlReader.NodeType.Equals(XmlNodeType.Element)
                            && (schemaLocation = xmlReader.GetAttribute("xsi:schemaLocation")) != null)
                        {

                            InfoLogger.Info(schemaLocation);

                            // make a search for the corresponding XSD Class
                            foreach (XSDSpec xsdS in xsdSpec)
                            {
                                if (schemaLocation.EndsWith(xsdS.xsdFile))
                                {
                                    XmlSerializer x = xsd[xsdS.xsdFile];
                                    // the class object has only one field : a List object
                                    Object datas = x.Deserialize(xmlReader);
                                    output[i++] = datas;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                InfoLogger.Info("Extracted " + part.Name);
            }
            return output;
        }

    }



}

