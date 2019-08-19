using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FGABusinessComponent.BusinessComponent;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.Holding;
using FGABusinessComponent.BusinessComponent.Holding.IndexComp;
using FGABusinessComponent.BusinessComponent.Security;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using FGABusinessComponent.BusinessComponent.Util;
using log4net;
using SQLCopy.Helpers;
using FGABusinessComponent.BusinessComponent.Security.Fx;
using FGABusinessComponent.BusinessComponent.Core;
using System.Data.Entity;
using log4net.Config;

// lecture du fichier de configuration pour les LOG
//[assembly: XmlConfigurator(ConfigFile = "Resources/Config/log4net.config", Watch = false)]

namespace FGA.Automate.IndexIntegration
{
    #region Utility Classes
    [Serializable()]
    internal class MSCIFamilyObject : Stringeable
    {
        public MSCIFamilyObject()
        {
        }
        /// <summary>
        /// Read a Equity line to extract the family Key
        /// </summary>
        /// <param name="sec"></param>
        public MSCIFamilyObject(MSCIBarra_EquityIndex.package_D15DEntry sec)
        {
            if (sec.ISO_country_symbol != null)
                ISOCountryCode = sec.ISO_country_symbol.ToCharArray(0, 2);
            DM_flag = sec.DM_universe_flag == 1;
            EM_flag = sec.EM_universe_flag == 1;
            AP_flag = false;

            // Disponible seulement sur les fichiers Value/Growth Indices / D50
            value_flag = false;
            growth_flag = false;
            // sur le fichier D15, c'est Large ou Middle , sur le fichier D17, le flag est disponible
            small_cap_flag = false;
            mid_cap_flag = sec.family_mid_flag == 1;
            large_cap_flag = sec.family_large_flag == 1;
            micro_cap_flag = false;
            islamic_index_flag = false;
            high_dividend_yield = false;
            standard_index_family = sec.std_IIF == 1;
            if (sec.sector != 0)
                sector = Convert.ToString(sec.sector).ToCharArray(0, 2);
            if (sec.industry_group != 0)
                industry_group = Convert.ToString(sec.industry_group).ToCharArray(0, 4);
            if (sec.industry != 0)
                industry = Convert.ToString(sec.industry).ToCharArray(0, 6);
            if (sec.sub_industry != 0)
                sub_industry = Convert.ToString(sec.sub_industry).ToCharArray(0, 8);
        }
        /// <summary>
        /// Read a Index Line to extract a pattern , that will be use for searching equities into that index
        /// </summary>
        /// <param name="i"></param>
        public MSCIFamilyObject(MSCIBarra_EquityIndex.package_D51DEntry i)
        {
            if (i.ISO_country_symbol != null)
                ISOCountryCode = i.ISO_country_symbol.ToCharArray(0, 2);
            DM_flag = i.DM_flag == 1;
            EM_flag = i.EM_flag == 1;
            AP_flag = i.AP_flag == 1;
            value_flag = i.value_flag == 1;
            growth_flag = i.growth_flag == 1;
            small_cap_flag = i.small_cap_flag == 1;
            mid_cap_flag = i.mid_cap_flag == 1;
            large_cap_flag = i.large_cap_flag == 1;
            micro_cap_flag = i.micro_cap_flag == 1;
            islamic_index_flag = i.islamic_index_flag == 1;
            high_dividend_yield = false;
            standard_index_family = true;
            if (i.sector != 0)
                sector = Convert.ToString(i.sector).ToCharArray(0, 2);
            if (i.industry_group != 0)
                industry_group = Convert.ToString(i.industry_group).ToCharArray(0, 4);
            if (i.industry != 0)
                industry = Convert.ToString(i.industry).ToCharArray(0, 6);
            if (i.sub_industry != 0)
                sub_industry = Convert.ToString(i.sub_industry).ToCharArray(0, 8);
        }
        public char[] ISOCountryCode { get; set; } // 2chars

        public Boolean DM_flag { get; set; }

        public Boolean EM_flag { get; set; }

        public Boolean AP_flag { get; set; }

        public Boolean value_flag { get; set; }

        public Boolean growth_flag { get; set; }

        public Boolean small_cap_flag { get; set; }

        public Boolean mid_cap_flag { get; set; }

        public Boolean large_cap_flag { get; set; }

        public Boolean micro_cap_flag { get; set; }

        public Boolean islamic_index_flag { get; set; }

        public Boolean high_dividend_yield { get; set; }

        public Boolean standard_index_family { get; set; }

        public char[] sector { get; set; }// 2chars

        public char[] industry_group { get; set; }// 4chars

        public char[] industry { get; set; }// 6chars

        public char[] sub_industry { get; set; }// 8chars
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append(ISOCountryCode == null ? "__" : new string(ISOCountryCode));
            sb.Append(DM_flag ? '1' : '0');
            sb.Append(EM_flag ? '1' : '0');
            sb.Append(AP_flag ? (DM_flag ? '_' : '1') : '0');
            sb.Append(SEPARATOR);
            sb.Append(value_flag ? '1' : '0');
            sb.Append(growth_flag ? '1' : '0');
            sb.Append(SEPARATOR);
            // 2 choices: just one char is the family is only one size , OR 2 or more characters if the family is wide
            // ALL CAP correspond à [LMSI]
            string _sizeFlags = "";
            if (large_cap_flag)
                _sizeFlags += 'L';
            if (mid_cap_flag)
                _sizeFlags += 'M';
            if (small_cap_flag)
                _sizeFlags += 'S';
            if (micro_cap_flag) //I for mIcro
                _sizeFlags += 'I';
            if (_sizeFlags.Length > 1)
            {
                _sizeFlags = '[' + _sizeFlags + ']';
            }
            sb.Append(_sizeFlags);
            sb.Append(SEPARATOR);
            sb.Append(islamic_index_flag ? '1' : '0');
            sb.Append(high_dividend_yield ? '1' : '0');
            sb.Append(standard_index_family ? '1' : '0');
            sb.Append(SEPARATOR);
            if (sub_industry != null)
            {
                sb.Append(new string(sub_industry));
            }
            else if (industry != null)
            {
                sb.Append(new string(industry)).Append("__");
            }
            else if (industry_group != null)
            {
                sb.Append(new string(industry_group)).Append("____");
            }
            else if (sector != null)
            {
                sb.Append(new string(sector)).Append("______");
            }
            else
            {
                sb.Append("________");
            }
            return sb.ToString();
        }

        private void interpretSizeFlag(char c)
        {
            switch (c)
            {
                case 'L':
                    large_cap_flag = true;
                    break;
                case 'M':
                    mid_cap_flag = true;
                    break;
                case 'S':
                    small_cap_flag = true;
                    break;
                case 'I':
                    micro_cap_flag = true;
                    break;
            }
        }

        public override void FromString(string s)
        {
            int i = 0;
            ISOCountryCode = s.Substring(i, 2).ToCharArray(0, 2);
            i += 2;
            DM_flag = s[i++] == '1' ? true : false;
            EM_flag = s[i++] == '1' ? true : false;
            AP_flag = s[i++] == '1' || (s[i++] == '_' && DM_flag) ? true : false;
            i++;
            value_flag = s[i++] == '1' ? true : false;
            growth_flag = s[i++] == '1' ? true : false;
            i++;
            small_cap_flag = false;
            mid_cap_flag = false;
            large_cap_flag = false;
            micro_cap_flag = false;
            i++;
            // 2 choices: just one char is the family is only one size , OR 2 or more characters if the family is wide
            char c = s[i++];
            if (c == '[')
            {
                while (c != ']')
                {
                    c = s[i++];
                    interpretSizeFlag(c);
                }
            }
            else
            {
                this.interpretSizeFlag(c);
            }
            i++;
            islamic_index_flag = s.Substring(i++, 1) == "1" ? true : false;
            high_dividend_yield = s.Substring(i++, 1) == "1" ? true : false;
            standard_index_family = s.Substring(i++, 1) == "1" ? true : false;
            i++;

            string sous_chaine = s.Substring(i, 2);
            if (!sous_chaine.Equals("__"))
            {
                sector = s.Substring(i, 2).ToCharArray(0, 2);
                sous_chaine = s.Substring(i, 4);
                if (!sous_chaine.Equals("____"))
                {
                    industry_group = s.Substring(i, 4).ToCharArray(0, 4);
                    sous_chaine = s.Substring(i, 6);
                    if (!sous_chaine.Equals("______"))
                    {
                        industry = s.Substring(i, 6).ToCharArray(0, 6);
                        sous_chaine = s.Substring(i, 8);
                        if (!sous_chaine.Equals("________"))
                        {
                            sub_industry = s.Substring(i, 8).ToCharArray(0, 8);
                        }
                    }
                }
            }
        }
    }

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
    internal class NextDayValue
    {
        public double marketCapInMMUSD;
        public double adjMarketCapInMMUSD;
        public int nbSecurity;
    }

    internal class SecurityPriceForex
    {
        public Int64 SecurityId;
        public string ISIN;
        public string Currency;
        public double Price;
        public double Forex;
    }

    #endregion

    public class MSCIIndexFile : IndexFileIntegration<MSCIBarra_EquityIndex.package_D51DEntry, MSCIBarra_EquityIndex.package_D15DEntry, MSCIBarra_EquityIndex.package_D_5DEntry>
    {
        private static string ENVIRONNEMENT = "DEV";
        public MSCIIndexFile(string env = "PREPROD")
        {
            MSCIIndexFile.ENVIRONNEMENT = env;
        }
        /// <summary>
        /// @TODO Injection parameters
        /// </summary>
        public const string MARKET_VALUE_CALCULATION_PATH = @"\\vill1\Partage\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REFERENTIEL\INDICES\extraction_MSCI_CalculateWeight.sql";
        public const string PRICE_FOREX_PATH = @"\\vill1\Partage\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REFERENTIEL\INDICES\extraction_MSCI_PriceForex.sql";
        public const string INDEX_PATH = @"\\vill1\Partage\,FGA MarketData\INDICES\MSCI\";

        private FGAContext _getFGAContext;
        private FGAContext getFGAContext
        {
            get
            {
                if (_getFGAContext == null)
                    _getFGAContext = new FGAContext(ENVIRONNEMENT, performanceOverValidationFlag: true);
                return _getFGAContext;
            }
        }
        /// <summary>
        /// Build the settings object using MSCI paths
        /// </summary>
        /// <param name="DATE">au format YYYYMMDD</param>
        private MSCI_SETTINGS GetPathSettings(string DATE, string MSCI_FILES_PATH)
        {
            MSCI_SETTINGS MSCI_Settings = new MSCI_SETTINGS();
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
            get { return LogManager.GetLogger("FGA_Soft_MSCI_EXTRACTION_LOG"); }
        }

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_MSCI_EXTRACTION"); }
        }

        #endregion

        #region Public API
        /// <summary>
        /// Execute FileIntegration - Point d entrée
        /// </summary>
        /// <param name="dateStart">au format "jj/mm/aaaa"</param>
        /// <param name="dateEnd"></param>
        /// <param name="rootPath">argumentFilesPath[0] constante INDEX_PATH par defaut </br>
        /// 
        /// 
        public override void ExecuteIndexFileIntegration(DateTime dateStart, DateTime dateEnd, params object[] argumentFilesPath)
        {
            string filePath = null;
            double mem0 = GC.GetTotalMemory(false) / 1024;
            double mem1, mem2, mem3;
            string rootPath;
            if (argumentFilesPath != null && argumentFilesPath.Length > 1 && argumentFilesPath[0] is string)
            {
                rootPath = argumentFilesPath[0] as string;
            }
            else
            {
                rootPath = INDEX_PATH;
            }


            for (DateTime dateOfData = dateStart; dateOfData <= dateEnd; dateOfData = dateOfData.AddDays(1))
            {
                try
                {
                    //DateTime dateOfData = DateTime.ParseExact("21/09/2012", "d", new CultureInfo("fr-FR"));
                    InfoLogger.Info("Donnees du " + dateOfData + " en cours: " + DateTime.Now.ToString());

                    if (dateOfData.DayOfWeek == DayOfWeek.Saturday || dateOfData.DayOfWeek == DayOfWeek.Sunday)
                        continue;

                    string d1 = dateOfData.ToString("yyMMdd");
                    string d2 = dateOfData.ToString("yyyyMMdd");
                    string d3 = dateOfData.ToString("yyyyMM");

                    MSCI_SETTINGS MSCI_Settings = this.GetPathSettings(d2, rootPath + d3 + @"\");

                    IDictionary<string, double> forexDateOfData;

                    mem1 = GC.GetTotalMemory(false) / 1024;
                    InfoLogger.InfoFormat("MEM1: ({0:0.0}%):{1} ", (double)((mem1 - mem0) / mem0) * 100, mem1);
                    //1. read the D5 file and return a hashtable for future calculation
                    this.extractForeXRateMain(MSCI_Settings, dateOfData, out forexDateOfData);

                    mem2 = GC.GetTotalMemory(false) / 1024;
                    InfoLogger.InfoFormat("MEM2: ({0:0.0}%):{1} ", (double)((mem2 - mem1) / mem1) * 100, mem2);

                    //read the D15 file and return the total adjusted MarketCap in Millions USD for the NextDay (based on the NextDay fields and Price/forex of the day )
                    // create if need the equity , and calculate holding for MSCI_SI
                    this.extractSecurityMain(MSCI_Settings, dateOfData, forexDateOfData);

                    mem3 = GC.GetTotalMemory(false) / 1024;
                    InfoLogger.InfoFormat("MEM3: ({0:0.0}%):{1} ", (double)((mem3 - mem2) / mem2) * 100, mem3);

                    IDictionary<string, NextDayValue> index_data_next_day = GetIndexMarketCapNextDay(dateOfData);
                    // read the D51 file to give the full composition/weighting of the MSCI Standard Indices
                    this.extractIndexMain(MSCI_Settings, dateOfData, index_data_next_day);

                    mem0 = GC.GetTotalMemory(false) / 1024;
                    InfoLogger.InfoFormat("MEM4: ({0:0.0}%):{1} ", (double)((mem0 - mem3) / mem3) * 100, mem0);

                    InfoLogger.Info("Donnees du " + dateOfData + " integrees: " + DateTime.Now.ToString());
                }
                catch (DbEntityValidationException validE)
                {
                    var entity = validE.EntityValidationErrors.First().Entry.Entity;

                    var error = validE.EntityValidationErrors.First().ValidationErrors.First();
                    ExceptionLogger.Error("Validation errors: " + error.PropertyName + " :" + error.ErrorMessage, validE);
                }
                catch (Exception e)
                {
                    ExceptionLogger.Error("File integration stopped:" + filePath, e);
                    getFGAContext.Dispose();
                }
                // Force the garbage collection
                System.GC.Collect();

            }

        }

        public void TestMSCIIndexAllMonthETL()
        {
            string msciFilesPath = @"G:\,FGA MarketData\INDICES\MSCI\";
            string start = "04/04/2014";
            string end = "04/04/2014";
            DateTime dateStart = DateTime.ParseExact(start, "d", new CultureInfo("fr-FR"));
            DateTime dateEnd = DateTime.ParseExact(end, "d", new CultureInfo("fr-FR"));
            this.ExecuteIndexFileIntegration(dateStart, dateEnd, msciFilesPath);
        }

        #endregion

        #region MARKETDATA_CALCULATION_NEXTDAY
        /// <summary>
        /// Calculate the Market Cap of each Indexes on the Next Day using the Market Cap of the global MSCI_SI_ND index
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <returns></returns>
        internal IDictionary<string, NextDayValue> GetIndexMarketCapNextDay(DateTime dateOfData)
        {
            // calculate the MarketCap for the next day on all indexes
            StreamReader sr = File.OpenText(MARKET_VALUE_CALCULATION_PATH);
            KeyValuePair<string, object>[] parameters = new KeyValuePair<string, object>[2];
            DateTime nd;
            IDictionary<string, NextDayValue> totalMarketCapInMMUSDNextDay = new Dictionary<string, NextDayValue>(30);
            if (dateOfData.DayOfWeek.Equals(DayOfWeek.Friday))
            {
                nd = dateOfData.AddDays(3);
            }
            else
            {
                nd = dateOfData.AddDays(1);
            }
            parameters[0] = new KeyValuePair<string, object>("dateIndex", nd);
            parameters[1] = new KeyValuePair<string, object>("indexHoldingType", "MSCI_SI_ND");

            using (System.Data.IDataReader dbDR = getFGAContext.ExecuteSelectSql(sr.ReadToEnd(), parameters))
            {

                while (dbDR.Read())
                {
                    totalMarketCapInMMUSDNextDay.Add(dbDR.GetString(1), new NextDayValue { marketCapInMMUSD = dbDR.GetDouble(3), adjMarketCapInMMUSD = dbDR.GetDouble(4), nbSecurity = dbDR.GetInt32(5) });
                }
            }

            return totalMarketCapInMMUSDNextDay;
        }
        #endregion

        #region PREVIOUSDAY
        /// <summary>
        /// Get The Price and FOrex for each Security contained into the MSCI SI Index on the next day
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <returns></returns>
        internal IDictionary<Int64, SecurityPriceForex> GetSecurityPriceFXPreviousDay(DateTime dateOfData)
        {
            // Get all prices and forex for every Security in the Index
            StreamReader sr = File.OpenText(PRICE_FOREX_PATH);
            KeyValuePair<string, object>[] parameters = new KeyValuePair<string, object>[2];
            DateTime pd;
            IDictionary<Int64, SecurityPriceForex> securityPriceFXPreviousDay = new Dictionary<Int64, SecurityPriceForex>(2000);
            if (dateOfData.DayOfWeek.Equals(DayOfWeek.Monday))
            {
                pd = dateOfData.AddDays(-3);
            }
            else
            {
                pd = dateOfData.AddDays(-1);
            }
            parameters[0] = new KeyValuePair<string, object>("dateIndex", pd);
            parameters[1] = new KeyValuePair<string, object>("indexHoldingType", "MSCI_SI");
            using (System.Data.IDataReader dbDR = getFGAContext.ExecuteSelectSql(sr.ReadToEnd(), parameters))
            {
                while (dbDR.Read())
                {
                    securityPriceFXPreviousDay.Add(dbDR.GetInt64(0), new SecurityPriceForex { SecurityId = dbDR.GetInt64(0), ISIN = dbDR.GetString(1), Price = dbDR.GetDouble(4), Forex = dbDR.GetDouble(6), Currency = dbDR.GetString(5) });
                }
            }
            return securityPriceFXPreviousDay;
        }
        #endregion

        #region INDEX
        internal void extractIndexMain(MSCI_SETTINGS MSCI_Settings, DateTime dateOfData, IDictionary<string, NextDayValue> indexDataNextDay)
        {
            var x = new
            {
                indexType = MSCI_Settings.indexType_DM,
                xmlPath = MSCI_Settings.rootPath,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D51D),
                xmlDataFile = MSCI_Settings.PREFIX_DATE + "CORE_DM_ALL_INDEX_MAIN_DAILY_D.xml",
                zipDataFile = MSCI_Settings.USED_DATE + "core_dm_daily_d.zip"
            };

            MSCIBarra_EquityIndex.package_D51D p = (MSCIBarra_EquityIndex.package_D51D)this.GetPackageClassInZIP(x.xmlPath, x.xsdClass, x.zipDataFile, x.xmlDataFile);
            FGAContext db = getFGAContext;
            int compteur = 0;
            foreach (MSCIBarra_EquityIndex.package_D51DEntry indexLine in p.dataset_D51D)
            {
                Index index = LookupIndexObject(db, indexLine);
                IndexValuation iv = AddIndexValuation(db, index, indexLine, dateOfData);

                // la MarketCap est celle du MSCI_SI_ND en global, il faudrait sommer pour les Equity faisant parti de l index seulement
                NextDayValue ndv;
                if (indexDataNextDay.TryGetValue(index.ISIN.ISINCode.TrimEnd(), out ndv))
                {
                    IndexValuation iv_nd = AddIndexValuationNextDay(db, index, indexLine, dateOfData, ndv.marketCapInMMUSD, ndv.adjMarketCapInMMUSD, ndv.nbSecurity);
                }
                if (++compteur % 500 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
        }

        protected override Index LookupIndexObject(FGAContext db, MSCIBarra_EquityIndex.package_D51DEntry indexLine, string ISIN = null)
        {
            Index index;
            string id = indexLine.msci_index_code.ToString();
            index = db.Indexes.Where<Index>(t => t.Identification.OtherIdentification == id).FirstOrDefault<Index>();

            if (index == null)
            {
                IDictionary<string, string> d = new Dictionary<string, string>();
                d["isin"] = indexLine.real_time_ticker ?? "MSCI" + id;
                d["currency"] = indexLine.mkt_cap_ISO_symbol_ccy;
                d["name"] = indexLine.index_name;
                d["id"] = id;
                d["ric"] = indexLine.real_time_ric;
                d["bloomberg"] = indexLine.real_time_ticker;
                d["country"] = indexLine.region_code;
                index = BusinessComponentHelper.CreateIndexObject(db, d, ExceptionLogger);
                db.SaveChanges();
            }
            MSCIFamilyObject f = new MSCIFamilyObject(indexLine);
            if (!f.Equals(index.FamilyKey))
            {
                index.FamilyKeyObject = f;
                //// specify modification
                db.Entry(index).Property(i => i.FamilyKey).IsModified = true;
            }
            //// eager loading / explicit load of Identification
            db.Entry(index).Reference(i => i.Identification).Load();

            return index;
        }

        protected override IndexValuation AddIndexValuation(FGAContext db, Index i, MSCIBarra_EquityIndex.package_D51DEntry indexLine, DateTime dateOfData, string ISIN = null)
        {
            //Valuation du sous indice                    
            string isin = i.ISIN.ISINCode;
            // chercher avec la date, l objet Valuation
            IndexValuation valuation = BusinessComponentHelper.LookupIndexValuationObject(db, isin, dateOfData, "MSCI_SI");
            if (valuation == null)
            {
                valuation = new IndexValuation(dateOfData,
                    Valuated: i,
                    BaseValue: (double)indexLine.std_index_base_value,
                    BaseDate: (indexLine.std_latest_index_base_dateSpecified ?  indexLine.std_latest_index_base_date : (DateTime?)null),
                    IndexPriceValue: (double)indexLine.std_eod00d_usd,
                    IndexGrossValue: (double)indexLine.grs_eod00d_usd,
                    IndexNetValue: (double)indexLine.net_eod00d_usd,
                    // Book Value in MM USD => Adjusted MarketCap
                    BookValue: new CurrencyAndAmount((double)indexLine.adj_mkt_cap_eod00d_usd, CurrencyCode.MUSD),
                    // Market Cap in MM USD => Initial MarketCap                     
                    MarketValue: new CurrencyAndAmount((double)indexLine.initial_mkt_cap_eod00d_usd, CurrencyCode.MUSD));
                valuation.IndexDivisor = (double)indexLine.std_index_divisor_usd;
                valuation.IndexNumberOfSecurities = (int)indexLine.count_of_securities;
                valuation.ValuationSource = "MSCI_SI";
                db.Valuations.Add(valuation);
                db.SaveChanges();
                // do not load the Pricing List. too large structure
                //i.Add(valuation);
            }
            else
            {
                // Book Value in MM USD => Adjusted MarketCap
                valuation.BookValue = new CurrencyAndAmount((double)indexLine.adj_mkt_cap_eod00d_usd, CurrencyCode.MUSD);
                // Market Cap in MM USD => Initial MarketCap
                valuation.MarketValue = new CurrencyAndAmount((double)indexLine.initial_mkt_cap_eod00d_usd, CurrencyCode.MUSD);
                valuation.IndexPriceValue = (double)indexLine.std_eod00d_usd;
                valuation.IndexGrossValue = (double)indexLine.grs_eod00d_usd;
                valuation.IndexNetValue = (double)indexLine.net_eod00d_usd;
                valuation.IndexDivisor = (double)indexLine.std_index_divisor_usd;
                valuation.IndexNumberOfSecurities = (int)indexLine.count_of_securities;
                valuation.ValuationSource = "MSCI_SI";
                db.Entry<IndexValuation>(valuation).State = EntityState.Modified;
            }
            // calcul des perf à partir de IndexNetValue en J-1 /J
            if (indexLine.initial_mkt_cap_eod00d_usd != 0)
            {
                valuation.Yield.ChangePrice_1D = new PercentageRate(100 * (double)(indexLine.adj_mkt_cap_eod00d_usd / indexLine.initial_mkt_cap_eod00d_usd - 1));
                valuation.Yield.ChangePrice_MTD = new PercentageRate((double)indexLine.daily_yield);
                db.Entry(valuation).Property(pr => pr.Yield).IsModified = true;
            }

            return valuation;
        }


        /// <summary>
        /// lookup (create) the main indices
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, Index> lookupMainIndexes(FGAContext db, String[] mainIndexes = null)
        {
            if (mainIndexes == null)
            {
                String[] s = { "MSCI_SI", "MSCI_SI_ND", "MXFR", "MXEM", "MXEUM", "MSCI650040", "MXEU", "MXPC", "MXEF", "MXWOM" };
                mainIndexes = s;
            }

            Dictionary<string, Index> r = new Dictionary<string, Index>();
            Index index;
            foreach (string id in mainIndexes)
            {
                if (id == "MSCI_SI")
                {
                    index = BusinessComponentHelper.LookupIndexObjectById(db, "MSCI_SI");
                    if (index == null)
                    {
                        IDictionary<string, string> d = new Dictionary<string, string>();
                        d["isin"] = "MSCI_SI";
                        d["currency"] = "USD";
                        d["country"] = null;
                        d["name"] = "MSCI StandardIndices";
                        d["id"] = null;
                        d["ric"] = null;
                        d["bloomberg"] = null;
                        index = BusinessComponentHelper.CreateIndexObject(db, d, ExceptionLogger);
                        db.SaveChanges();
                    }
                    r.Add(id, index);
                }
                if (id == "MSCI_SI_ND")
                {
                    index = BusinessComponentHelper.LookupIndexObjectById(db, "MSCI_SI_ND");
                    if (index == null)
                    {
                        IDictionary<string, string> d = new Dictionary<string, string>();
                        d["isin"] = "MSCI_SI_ND";
                        d["currency"] = "USD";
                        d["country"] = null;
                        d["name"] = "MSCI StandardIndices Next Day Calculation";
                        d["id"] = null;
                        d["ric"] = null;
                        d["bloomberg"] = null;
                        index = BusinessComponentHelper.CreateIndexObject(db, d, ExceptionLogger);
                        db.SaveChanges();
                    }
                    r.Add(id, index);
                }
            }
            return r;
        }
        #endregion INDEX

        #region SECURITY
        /// <summary>
        /// Manage Equity
        /// </summary>
        /// <param name="MSCI_Settings"></param>
        /// <param name="dateOfData"></param>
        /// <param name="forex">all Forex for dateofData</param>
        internal void extractSecurityMain(MSCI_SETTINGS MSCI_Settings, DateTime dateOfData, IDictionary<string, double> forex)
        {
            var x = new
            {
                indexType = MSCI_Settings.indexType_DM,
                xmlPath = MSCI_Settings.rootPath,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
                xmlDataFile = MSCI_Settings.PREFIX_DATE + "CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml",
                zipDataFile = MSCI_Settings.USED_DATE + "core_dm_daily_d.zip"
            };


            MSCIBarra_EquityIndex.package_D15D p = (MSCIBarra_EquityIndex.package_D15D)this.GetPackageClassInZIP(x.xmlPath, x.xsdClass, x.zipDataFile, x.xmlDataFile);
            FGAContext db = getFGAContext;
            int compteur = 0;
            IDictionary<string, Index> mainIndexes = lookupMainIndexes(db);
            //IDictionary<string, IndexValuation> mainIndexesValuation = new Dictionary<string, IndexValuation>(mainIndexes.Count);

            foreach (MSCIBarra_EquityIndex.package_D15DEntry equityLine in p.dataset_D15D)
            {
                Equity eq = (Equity)this.LookupSecurityObject(db, equityLine);
                AssetClassification ac = this.AddAssetClassification(db, eq, equityLine);
                SecuritiesPricing sp = AddSecurityValuation(db, eq, equityLine, dateOfData);

                // Add the holding for the current day and the next day. a holding on the all msci 
                this.AddAssetHolding(db, mainIndexes["MSCI_SI"], eq, indexLine: null, equityLine: equityLine, dateOfData: dateOfData);

                this.AddAssetHoldingNextDay(db, mainIndexes["MSCI_SI_ND"], eq, indexLine: null, equityLine: equityLine, dateOfData: dateOfData, additionalparameters: forex[equityLine.price_ISO_currency_symbol]);

                if (++compteur % 500 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
        }

        // lookup for the isin bond
        protected override Security LookupSecurityObject(FGAContext db, MSCIBarra_EquityIndex.package_D15DEntry equityLine, string ISIN = null)
        {
            Equity eq = null;
            string msci_id = Convert.ToString(equityLine.msci_security_code);
            List<SecuritiesIdentification> results = db.SecuritiesIds.Where<SecuritiesIdentification>(t => t.SecurityIdentification.ISINCode == equityLine.isin ||
                t.OtherIdentification == msci_id).ToList();
            SecuritiesIdentification id = null;

            CountryCode country = (CountryCode)equityLine.ISO_country_symbol;
            BloombergIdentifier ticker = (BloombergIdentifier)equityLine.bb_ticker;
            bool equityFound = false;


            if (results != null && results.Count > 0)
            {// get the existing security

                IEnumerator<SecuritiesIdentification> resultsEnumerator = results.AsEnumerable<SecuritiesIdentification>().GetEnumerator();
                while ((!equityFound) && resultsEnumerator.MoveNext())
                {
                    id = resultsEnumerator.Current;

                    // If the Domestic Id is the same country , then it is the equity we re looking for
                    if (equityLine.isin != null && id.SecurityIdentification.ISINCode.Equals(equityLine.isin) && id.DomesticIdentificationSource.Equals(country))
                    {
                        equityFound = true;
                    }
                    // otherwise, if the bloomberg ticker is the same, it is the value and the Domestic value must be set
                    else if (id.Bloomberg != null && id.Bloomberg.Equals(ticker) && id.SecurityIdentification.ISINCode.Equals(equityLine.isin) && id.DomesticIdentificationSource.isDefaultValue())
                    {
                        id.DomesticIdentificationSource = country;
                        equityFound = true;
                    }
                    // the msci identification has be found but the Domestic Value has not be set
                    else if (equityLine.isin != null && id.SecurityIdentification.ISINCode.Equals(equityLine.isin) && id.DomesticIdentificationSource.isDefaultValue())
                    {
                        id.DomesticIdentificationSource = country;
                        equityFound = true;
                    }
                    // NOT FOUND. Next result
                }
            }


            // if found: get the Equity object from the Identification object
            if (equityFound)
            {
                eq = db.Equities.Include("Identification").Where<Equity>(t => t.IdentificationId == id.Id).First<Equity>();
            }
            // if not found: create the security with its static data
            else
            {
                eq = new Equity(equityLine.isin, equityLine.security_name);
                eq.Identification.DomesticIdentificationSource = country;
                eq.Identification.OtherIdentification = Convert.ToString(equityLine.msci_security_code);
            }

            // overwrite data. for the change of name
            eq.FinancialInstrumentName = equityLine.security_name;
            eq.Identification.Name = equityLine.security_name;
            eq.Identification.SEDOL = (SEDOLIdentifier)equityLine.sedol;
            eq.Identification.Bloomberg = ticker;
            eq.Identification.RIC = (RICIdentifier)equityLine.RIC;

            // if not found, dump data to the DBMS, for getting generated id
            if (!equityFound)
            {
                db.Equities.Add(eq);
                db.SaveChanges();
            }
            return eq;
        }
        protected override AssetClassification AddAssetClassification(FGAContext db, Asset equity, MSCIBarra_EquityIndex.package_D15DEntry equityLine, string ISIN = null)
        {
            Equity eq = (Equity)equity;
            // update the classification
            // chercher avec la classif
            AssetClassification classif = BusinessComponentHelper.LookupAssetClassification(db, eq, "MSCI");
            if (classif == null)
            {
                classif = new AssetClassification("MSCI");
                eq.Add(classif);
                db.AssetClassifications.Add(classif);
            }

            classif.Classification1 = equityLine.sector.ToString("G");
            classif.Classification2 = equityLine.industry_group.ToString("G");
            classif.Classification3 = equityLine.industry.ToString("G");
            classif.Classification4 = equityLine.sub_industry.ToString("G");
            classif.Classification5 = null;
            classif.Classification6 = null;
            classif.Classification7 = null;

            db.Entry<Equity>(eq).State = EntityState.Modified;
            return classif;
        }

        protected override SecuritiesPricing AddSecurityValuation(FGAContext db, Security equity, MSCIBarra_EquityIndex.package_D15DEntry equityLine, DateTime dateOfData, string ISIN = null)
        {
            Equity e = (Equity)equity;
            Yield yield = new Yield(ChangePrice_1D: (double)equityLine.net_return_intl_usd); // #64
            SecuritiesPricing p;
            p = BusinessComponentHelper.LookupSecurityPricingObject(db, e, dateOfData, "MSCI");

            //Prix et performance
            CurrencyAndAmount price = new CurrencyAndAmount((double)equityLine.price, (CurrencyCode)equityLine.price_ISO_currency_symbol);
            if (p == null)
            {
                //PriceFactType priceFactType = new PriceFactType();
                p = new SecuritiesPricing(price, dateOfData, (TypeOfPriceCode)"MARKET", Yield: yield);
                p.PriceSource = "MSCI";
                p.Set(e);
                // do not load the Pricing List. too large structure
                //e.Pricing.Add(p);
                db.SecuritiesPricings.Add(p);
                db.SaveChanges();
            }
            else
            {
                p.Yield = yield;
                p.Price = price;
                //// specify modification
                db.Entry(p).Property(pr => pr.Yield).IsModified = true;
                db.Entry(p).Property(pr => pr.Price).IsModified = true;                
            }
            return p;
        }
        #endregion SECURITY

        #region HOLDING MNGT

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="index"></param>
        /// <param name="equity"></param>
        /// <param name="indexLine"></param>
        /// <param name="equityLine"></param>
        /// <param name="dateOfData"></param>
        /// <param name="additionalparameters">0 : forex for the previous day
        ///                                    1: optional: indexvaluation for the current day</param>
        /// <returns></returns>
        protected override AssetHolding AddAssetHolding(FGAContext db, Index index, Asset equity, MSCIBarra_EquityIndex.package_D51DEntry indexLine, MSCIBarra_EquityIndex.package_D15DEntry equityLine, DateTime dateOfData, params Object[] additionalparameters)
        {
            Equity e = (Equity)equity;

            AssetHolding holding;
            holding = BusinessComponentHelper.LookupAssetHolding(db, index, e, dateOfData);

            // calcul de la Market cap ajusté: #35 Foreign Inclusion Factor * #82 Sec Adjusted Mkt Cap (USD) 
            double adjustedMcap = (double)(equityLine.foreign_inclusion_factor * equityLine.adj_market_cap_usdol);

            // calcul de la InitialMarket Cap : #35 Foreign Inclusion Factor * #65 Sec Initial Full Mkt Cap (USD)
            double? initialMcap;
            initialMcap = (double)(equityLine.foreign_inclusion_factor * equityLine.initial_mkt_cap_usd);
         

            MSCIFamilyObject f = new MSCIFamilyObject(equityLine);
            if (holding == null)
            {
                holding = new AssetHolding(Date: dateOfData, ISIN: equityLine.isin,
                HoldAsset: e,
                Holder: index,
                    //Notional // Principal // Par Value * quantity
                    MarketValue: new CurrencyAndAmount(initialMcap, CurrencyCode.USD),
                    BookValue: new CurrencyAndAmount(adjustedMcap, CurrencyCode.USD),
                    Weight: new PercentageRate(),
                    Quantity: (float)equityLine.eod_number_of_shares_today);
                holding.FamilyKeyObject = f;

                db.AssetHoldings.Add(holding);
                db.SaveChanges();
            }
            else
            {
                holding.ISIN = (ISINIdentifier)equityLine.isin;
                holding.MarketValue = new CurrencyAndAmount(initialMcap, CurrencyCode.USD);
                holding.BookValue = new CurrencyAndAmount(adjustedMcap, CurrencyCode.USD);
                holding.Weight = new PercentageRate();
                holding.Quantity = (float)equityLine.eod_number_of_shares_today;
                db.Entry<AssetHolding>(holding).State = EntityState.Modified;
                if (!f.Equals(holding.FamilyKey))
                {
                    holding.FamilyKeyObject = f;
                    db.Entry(holding).State = EntityState.Modified;
                }
            }


            return holding;

        }
        #endregion

        #region NEXTDAY

        private IndexValuation AddIndexValuationNextDay(FGAContext db, Index i, MSCIBarra_EquityIndex.package_D51DEntry indexLine, DateTime dateOfData, double MarketCapInMMUSDNextDay, double AdjustedMarketCapInMMUSDNextDay, int nbSecNd)
        {

            if (dateOfData.DayOfWeek.Equals(DayOfWeek.Friday))
            {
                dateOfData = dateOfData.AddDays(3);
            }
            else
            {
                dateOfData = dateOfData.AddDays(1);
            }
            IndexValuation valuation;

            double yield_ChangePrice_1D = AdjustedMarketCapInMMUSDNextDay / MarketCapInMMUSDNextDay - 1;
            //Valuation du sous indice
            double priceIndexNextDay = (1 + yield_ChangePrice_1D) * (double)indexLine.std_eod00d_usd; 
            double grossIndexNextDay = (1 + yield_ChangePrice_1D) * (double)indexLine.grs_eod00d_usd;
            double netIndexNextDay = (1 + yield_ChangePrice_1D) * (double)indexLine.net_eod00d_usd;
            string isin = i.ISIN.ISINCode;
            // chercher avec la date, l objet Valuation
            valuation = BusinessComponentHelper.LookupIndexValuationObject(db, isin, dateOfData, "MSCI_SI_ND");
            if (valuation == null)
            {
                valuation = new IndexValuation(dateOfData,
                    Valuated: i,
                    BaseValue: (double)indexLine.std_index_base_value,
                    BaseDate: (indexLine.std_latest_index_base_dateSpecified ?  indexLine.std_latest_index_base_date : (DateTime?)null) );
                valuation.ValuationSource = "MSCI_SI_ND";

                db.Valuations.Add(valuation);
                db.SaveChanges();
                // do not load the Pricing List. too large structure
                //i.Add(valuation);
            }
            // Market Cap in MM USD
            valuation.MarketValue = new CurrencyAndAmount(MarketCapInMMUSDNextDay, CurrencyCode.MUSD);
            valuation.IndexPriceValue = priceIndexNextDay;
            valuation.IndexGrossValue = grossIndexNextDay;
            valuation.IndexNetValue = netIndexNextDay;
            valuation.IndexDivisor = (double)indexLine.std_index_divisor_usd_nd;
            valuation.IndexNumberOfSecurities = nbSecNd;
            db.Entry<IndexValuation>(valuation).State = EntityState.Modified;

            valuation.Yield.ChangePrice_1D = new PercentageRate((double)100 * yield_ChangePrice_1D);
            valuation.Yield.ChangePrice_MTD = new PercentageRate((double)indexLine.daily_yield + 100 * yield_ChangePrice_1D);
            db.Entry(valuation).Property(pr => pr.Yield).IsModified = true;

            return valuation;
        }

        /// <summary>
        /// AddAssetHoldingNextDay
        /// </summary>
        /// <param name="db"></param>
        /// <param name="index"></param>
        /// <param name="e"></param>
        /// <param name="indexLine"></param>
        /// <param name="equityLine"></param>
        /// <param name="dateOfData"></param>
        /// <param name="additionalparameters">0 : forex for the next day
        ///                                    1: optional: indexvaluation for the nextday</param>
        private void AddAssetHoldingNextDay(FGAContext db, Index index, Equity e, MSCIBarra_EquityIndex.package_D51DEntry indexLine, MSCIBarra_EquityIndex.package_D15DEntry equityLine, DateTime dateOfData, params Object[] additionalparameters)
        {
            double forex = (double)additionalparameters[0];

            if (dateOfData.DayOfWeek.Equals(DayOfWeek.Friday))
            {
                dateOfData = dateOfData.AddDays(3);
            }
            else
            {
                dateOfData = dateOfData.AddDays(1);
            }

            AssetHolding holding;
            holding = BusinessComponentHelper.LookupAssetHolding(db, index, e, dateOfData);

            // calcul de la Market cap ajusté:  #36 Foreign Inclusion Factor ND * #30 PAF NextDay * #33 End Of Day NumberOfShares (Nextday) * price t+1 / forex t 
            // price t+1 => price t
            double adjustedMarketCapInUSDNextDay = (double)(equityLine.foreign_inc_factor_next_day * equityLine.prelim_price_adj_fact_nxt_day * equityLine.eod_number_of_shares_next_day * equityLine.price) / forex;

            // Calcul de l initialMarketCap :  #36 Foreign Inclusion Factor ND * #66 Sec Initial Full Mkt Cap (USD) Next Day
            double initialMarketCapInUSDNextDay = (double)(equityLine.foreign_inc_factor_next_day * equityLine.initial_mkt_cap_usd_next_day);

            MSCIFamilyObject f = new MSCIFamilyObject(equityLine);
            if (holding == null)
            {
                holding = new AssetHolding(Date: dateOfData, ISIN: equityLine.isin,
                HoldAsset: e,
                Holder: index,
                MarketValue: new CurrencyAndAmount(initialMarketCapInUSDNextDay, CurrencyCode.USD),
                BookValue: new CurrencyAndAmount(adjustedMarketCapInUSDNextDay, CurrencyCode.USD),
                Quantity: (float)equityLine.eod_number_of_shares_next_day,
                Weight: new PercentageRate());
                holding.FamilyKeyObject = f;
                db.AssetHoldings.Add(holding);
                db.SaveChanges();
            }
            else
            {
                holding.ISIN = (ISINIdentifier)equityLine.isin;
                holding.MarketValue = new CurrencyAndAmount(initialMarketCapInUSDNextDay, CurrencyCode.USD);
                holding.BookValue = new CurrencyAndAmount(adjustedMarketCapInUSDNextDay, CurrencyCode.USD);
                holding.Weight = new PercentageRate();
                holding.Quantity = (float)equityLine.eod_number_of_shares_next_day;
                holding.FamilyKeyObject = f;
                db.Entry<AssetHolding>(holding).State = EntityState.Modified;
            }
        }
        #endregion

        #region ZIP_FILE
        /// <summary>
        /// Methode pour lire l object dans le répertoire XML
        /// Impossible d utilisr WindowsBase System.IO.Packaging car ZIP n'est pas implémenté dans toutes ses versions
        /// </summary>
        private Object GetPackageClassInZIP(string rootPath, Type xsdClass, string zipFile, string fileName)
        {
            InfoLogger.Info("GetPackageClassInZIP '" + zipFile + "', search '" + fileName + "' with XSD '" + xsdClass + "'");
            XmlSerializer x = new XmlSerializer(xsdClass);
            Object datas = null;
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
                        datas = x.Deserialize(reader);
                        reader.Close();

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
            return datas;
        }


        /// <summary>
        /// Methode pour lire l object dans le répertoire XML
        /// Impossible d utilisr WindowsBase System.IO.Packaging car ZIP n'est pas implémenté dans toutes ses versions
        /// </summary>
        /*
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
         */
        #endregion ZIP_FILE

        #region FXRATE

        /// <summary>
        /// Manage FX rate
        /// </summary>
        /// <param name="MSCI_Settings"></param>
        /// <param name="dateOfData"></param>
        internal void extractForeXRateMain(MSCI_SETTINGS MSCI_Settings, DateTime dateOfData, out IDictionary<string, double> forexValues)
        {
            var x = new
            {
                indexType = MSCI_Settings.indexType_DM,
                xmlPath = MSCI_Settings.rootPath,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D_5D),
                xmlDataFile = MSCI_Settings.PREFIX_DATE + "CORE_DM_ALL_COUNTRY_FXRATE_DAILY_D.xml",
                zipDataFile = MSCI_Settings.USED_DATE + "core_dm_daily_d.zip"
            };

            forexValues = new Dictionary<string, double>();

            MSCIBarra_EquityIndex.package_D_5D p = (MSCIBarra_EquityIndex.package_D_5D)this.GetPackageClassInZIP(x.xmlPath, x.xsdClass, x.zipDataFile, x.xmlDataFile);
            FGAContext db = getFGAContext;
            int compteur = 0;
            CurrencyExchange fx;
            foreach (MSCIBarra_EquityIndex.package_D_5DEntry fxLine in p.dataset_D_5D)
            {

                forexValues[fxLine.ISO_currency_symbol] = (double)fxLine.spot_fx_eod00d;

                fx = LookupForexRateObject(db, fxLine, dateOfData);
                if (fx != null)
                {
                    fx.ExchangeRate = (double)fxLine.spot_fx_eod00d;
                    fx.ValuationSource = "MSCI_SI";
                    //db.Entry<CurrencyExchange>(fx).State = EntityState.Modified;
                }
                if (++compteur % 50 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();

        }
        protected override CurrencyExchange LookupForexRateObject(FGAContext db, MSCIBarra_EquityIndex.package_D_5DEntry fxLine, DateTime dateOfData, string ISIN = null)
        {
            CurrencyCode currency = null;
            try
            {
                currency = (CurrencyCode)fxLine.ISO_currency_symbol;
            }
            catch (Exception e)
            {
                ExceptionLogger.Info("Currency code :" + fxLine.ISO_currency_symbol + " Not recognized", e);
                return null;
            }

            CurrencyExchange fx = BusinessComponentHelper.LookupCurrencyExchange(db, CurrencyCode.USD, currency, dateOfData);
            if (fx == null)
            {
                // Forex à créer
                fx = new CurrencyExchange();
                fx.Date = dateOfData;
                fx.UnitCurrency = CurrencyCode.USD;
                fx.QuotedCurrency = currency;
                db.CurrencyExchanges.Add(fx);
                db.SaveChanges();
            }
            fx.ValuationSource = "MSCI_SI";
            fx.ExchangeRate = (double)fxLine.spot_fx_eod00d;

            db.Entry<CurrencyExchange>(fx).State = EntityState.Modified;
            return fx;
        }
        #endregion FXRATE

        protected override Rating AddSecurityRating(FGAContext db, Security security, MSCIBarra_EquityIndex.package_D15DEntry securityLine, DateTime dateOfData, string ISIN = null)
        {
            throw new NotImplementedException();
        }
    }


}
