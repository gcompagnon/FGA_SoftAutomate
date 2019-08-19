using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Holding;
using FGABusinessComponent.BusinessComponent.Holding.IndexComp;
using FGABusinessComponent.BusinessComponent.Security;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using FGABusinessComponent.BusinessComponent.Security.Roles;
using FileHelpers;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using ReactiveETL;
using FGABusinessComponent.BusinessComponent;
using FGABusinessComponent.BusinessComponent.Security.Fx;
using log4net;
using log4net.Config;
using FGABusinessComponent.BusinessComponent.Util;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
// lecture du fichier de configuration pour les LOG
//[assembly: XmlConfigurator(ConfigFile = "Resources/Config/log4net.config", Watch = false)]

namespace FGA.Automate.IndexIntegration
{
    [Serializable()]
    public class BarclaysNominalFamilyObject : Stringeable
    {
        private string FK_REGEX = @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*";

        public BarclaysNominalFamilyObject()
        {
        }
        /// <summary>
        /// Read a Equity line to extract the family Key
        /// </summary>
        /// <param name="sec"></param>
        public BarclaysNominalFamilyObject(DailyReturnsBond sec, string maturityLevel, string region = "EURO")
        {
            if (sec.Country != null)
                ISOCountryCode = sec.Country.ToCharArray(0, 2);
            Level1 = sec.IssrClsL1;
            Level2 = sec.IssrClsL2;
            Level3 = sec.IssrClsL3;
            Level4 = sec.IssrClsL4;
            Rating = sec.QualityE;
            RegionCode = region;
            MaturityLevel = maturityLevel;
        }
        /// <summary>
        /// Read a Index Line to extract a pattern , that will be use for searching equities into that index
        /// </summary>
        /// <param name="i"></param>
        public BarclaysNominalFamilyObject(string country, string level1, string level2, string level3, string level4, string rating, string maturityLevel, string region)
        {
            if (country != null)
                ISOCountryCode = country.ToCharArray(0, 2);
            Level1 = level1;
            Level2 = level2;
            Level3 = level3;
            Level4 = level4;
            Rating = rating;
            RegionCode = region;
            MaturityLevel = maturityLevel;
        }
        public string Level1 { get; set; }
        public string Level2 { get; set; }
        public string Level3 { get; set; }
        public string Level4 { get; set; }

        public string Rating { get; set; }
        public char[] ISOCountryCode { get; set; } // 2chars
        public string RegionCode { get; set; }

        public string MaturityLevel { get; set; }

        /// <summary>
        /// Override the FamilyKey with the given model (not null values only)
        /// </summary>
        /// <param name="model"></param>
        public void Set(BarclaysNominalFamilyObject model)
        {
            ISOCountryCode = model.ISOCountryCode ?? ISOCountryCode;
            Level1 = model.Level1 ?? Level1;
            Level2 = model.Level2 ?? Level2;
            Level3 = model.Level3 ?? Level3;
            Level4 = model.Level4 ?? Level4;
            Rating = model.Rating ?? Rating;
            RegionCode = model.RegionCode ?? RegionCode;
            MaturityLevel = model.MaturityLevel ?? MaturityLevel;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append(ISOCountryCode == null ? "__" : new string(ISOCountryCode)).Append(SEPARATOR);
            sb.Append(Level1 == null ? "%" : Level1).Append(SEPARATOR);
            sb.Append(Level2 == null ? "%" : Level2).Append(SEPARATOR);
            sb.Append(Level3 == null ? "%" : Level3).Append(SEPARATOR);
            sb.Append(Level4 == null ? "%" : Level4).Append(SEPARATOR);
            sb.Append(Rating == null ? "%" : Rating).Append(SEPARATOR);
            sb.Append(MaturityLevel == null ? "%" : MaturityLevel).Append(SEPARATOR);
            sb.Append(RegionCode == null ? "%" : RegionCode);
            return sb.ToString();
        }


        public override void FromString(string s)
        {
            MatchCollection mc = Regex.Matches(s, FK_REGEX);
            IEnumerator e = mc.GetEnumerator();
            Match m = (Match)e.Current;
            ISOCountryCode = (m.Value.Equals("%") ? null : m.Value.ToCharArray(0, 2)); e.MoveNext(); m = (Match)e.Current;
            Level1 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level2 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level3 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level4 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Rating = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            MaturityLevel = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            RegionCode = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override bool Equals(System.Object obj)
        {
            String s = obj as String;
            BarclaysNominalFamilyObject f = obj as BarclaysNominalFamilyObject;
            if ((f != null) && (f.ToString().Equals(this.ToString())))
            {
                return true;
            }
            if ((s != null) && (s.Equals(this.ToString())))
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Class to represent the correspondance between the Barclays Id and Ticker Id, plus the configuration not show in the files
    /// </summary>
    public class BarclaysIndexConfiguration
    {
        /// <summary>
        /// Internal Id of Barclays
        /// </summary>
        public string internalId;
        /// <summary>
        /// Our Id. we choose to use the BB Ticker Id
        /// </summary>
        public string tickerId;
        /// <summary>
        /// Label of the index
        /// </summary>
        public string label;
        /// <summary>
        /// These index could be part of a super index, or not (then null value)
        /// </summary>
        public string superIndexTickerId;
        /// <summary>
        /// the type of return taken. 1 = Unhedged 2 = Hedged
        /// </summary>
        public uint? returnType;
        /// <summary>
        /// the currency of the index 
        /// </summary>
        public CurrencyCode currency;
        /// <summary>
        /// the family key of the index, used for filtering the index constituants
        /// </summary>
        public BarclaysNominalFamilyObject indexFamilyKey;

        public BarclaysIndexConfiguration(string InternalId, string TickerId, string label, BarclaysNominalFamilyObject FamilyKey, string SuperIndexTickerId, uint? returnType, CurrencyCode currency = null)
        {
            if (InternalId == null || TickerId == null)
                throw new ApplicationException("Must have a correspondance between the Barclay Internal Id and a Ticker Id/ISIN");
            this.internalId = InternalId;
            this.tickerId = TickerId;
            this.superIndexTickerId = SuperIndexTickerId;
            this.label = label;
            this.indexFamilyKey = FamilyKey;
            this.returnType = returnType ?? 1;
            this.currency = currency ?? CurrencyCode.EUR;
        }
    }

    /// <summary>
    /// Class to represent the file configuration: name and the adapter / mapping between the model and the file (columns order)
    /// </summary>
    public class BarclaysFileConfiguration
    {
        //change the order if needed in the bond file
        public static Func<Row, Row> MAPPER_CURRENCY_COUNTRY_INVERSION = row =>
        {
            var r = row["Country"];
            row["Country"] = row["Currency"];
            row["Currency"] = r;
            return row;
        };

        //ignore the column named DurationModF
        public static Func<Row, Row> MAPPER_NO_DURATIONMODF = row =>
        {
            row["DurationModB"] = row["OutstandingE"];
            row["OutstandingE"] = row["Maturity"];
            row["Maturity"] = row["MarketValue"];
            row["MarketValue"] = row["YieldWorst"];
            row["YieldWorst"] = row["DurationModF"];
            row["DurationModF"] = null;
            return row;
        };


        public static Func<Row, Row> MAPPER_DEFAULT = row =>
        {
            return row;
        };

        public string filePattern;
        public Func<Row, Row> bondMapper, indexMapper;
        public BarclaysFileConfiguration(string filePattern, Func<Row, Row> rowIndexMapper = null, Func<Row, Row> rowBondMapper = null)
        {
            this.filePattern = filePattern;
            this.bondMapper = rowBondMapper ?? MAPPER_DEFAULT;
            this.indexMapper = rowIndexMapper ?? MAPPER_DEFAULT;
        }

        public string GetFilePath(DateTime date, string rootPath, string filepart)
        {
            string d1 = date.ToString("yyMMdd");
            string d2 = date.ToString("yyyyMMdd");
            return rootPath + d1 + filePattern + filepart + d2 + ".csv";
        }
    }

    /// <summary>
    /// Extension Methods for Config Objects 
    /// </summary>
    public static class BarclaysIndexConfigurationHelper
    {
        // extension method for the config statements
        public static void AddConfig(this Dictionary<string, BarclaysIndexConfiguration> dictionary, BarclaysIndexConfiguration configuration)
        {
            dictionary.Add(configuration.internalId, configuration);
            dictionary.Add(configuration.tickerId, configuration);
        }
    }

    /// <summary>
    /// Description résumée pour BarclaysIndexFile
    /// Integration des indices daily en Returns (stat possible avec le parametrage)
    /// </summary>
    public class BarclaysIndexFile : IndexFileIntegration<BarclaysIndexesFile, DailyReturnsBond, Object>
    {
        #region Logger
        public static ILog InfoLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_BARCLAYS_EXTRACTION_LOG"); }
        }

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_BARCLAYS_EXTRACTION"); }
        }
        #endregion

        #region Members: constant and static parameters
        public const string INDEX_PATH = @"\\vill1\Partage\,FGA MarketData\INDICES\BARCLAYS\NOMINAL\";

        public const string INDEX_UNIVERSE_RETURN = "RETURN";
        public const string INDEX_UNIVERSE_STAT = "STAT";
        public const string INDEX_UNIVERSE_ALL = "RETURNSTAT"; // the two universes are selected

        public const string INDEX_TREASURY = "TREASURY";
        public const string INDEX_OECD_7_10 = "OECD7-10";
        public const string INDEX_EUROAGG = "EUROAGG";
        public const string INDEX_EUROAGGCORP = "EUROAGG_CORPORATE";
        public const string INDEX_BARCLAYS_NOMINAL_ALL = "BARCLAYS_ALL";

        private static string ENVIRONNEMENT = "DEV";

        /// <summary>
        /// Correspondance Barclays internal code , to the "ticker" used id in the datamodel, the maturity level and the upper-index 
        /// </summary>
        // list of barclays key/database ticker , one for RETURN and one for the STAT
        private static Dictionary<string, BarclaysIndexConfiguration> INDEX_KEYS;

        // list of file names intialized // todo use injection
        public static Dictionary<string, BarclaysFileConfiguration[]> TREASURY_FILE;//key is STAT or RETURN
        public static Dictionary<string, BarclaysFileConfiguration[]> OECD_FILE;//key is STAT or RETURN
        public static Dictionary<string, BarclaysFileConfiguration[]> EUROAGG_FILE;//key is STAT or RETURN
        public static Dictionary<string, BarclaysFileConfiguration[]> EUROAGGCORP_FILE;//key is STAT or RETURN

        // the reference to the Entity Framework context
        private static FGAContext _getFGAContext;
        private static FGAContext getFGAContext
        {
            get
            {
                if (_getFGAContext == null)
                    _getFGAContext = new FGAContext(ENVIRONNEMENT, performanceOverValidationFlag: true);
                return _getFGAContext;
            }
        }

        // list of file names (first part)
        private Dictionary<string, BarclaysFileConfiguration[]> IndexFileNames;//key is STAT or RETURN

        // list of universe to be read
        private string CurrentIndexUniverse; // current universe to be read
        #endregion

        // TODO Injection or ressource file
        #region configuration object with all the correspondance between Barclays and ticker Id
        static BarclaysIndexFile()
        {
            //TODO TRANSCO code Barclays avec un tickr Bloom
            INDEX_KEYS = new Dictionary<string, BarclaysIndexConfiguration>();
            // EuroTreasury 1bn SelectCountries All
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28588", "BT11TREU", "Euro Treasury 1bn Select Countries", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: null, region: "EURO"), null, 1));
            // this index is a sub-index of  BT11TREU
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28581", "BTS1TREU", "Euro Treasury 1bn Select Countries 1-3 Yr", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "1-3Y", region: "EURO"), "BT11TREU", 1));
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28582", "BTS3TREU", "Euro Treasury 1bn Select Countries 3-5 Yr", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "3-5Y", region: "EURO"), "BT11TREU", 1));
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28583", "BTS5TREU", "Euro Treasury 1bn Select Countries 5-7 Yr", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "5-7Y", region: "EURO"), "BT11TREU", 1));
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28584", "BT7YTREU", "Euro Treasury 1bn Select Countries 7-10 Yr", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "7-10Y", region: "EURO"), "BT11TREU", 1));
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28585", "BT10TREU", "Euro Treasury 1bn Select Countries 10-15 Yr", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "10-15Y", region: "EURO"), "BT11TREU", 1));
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28586", "BT15TREU", "Euro Treasury 1bn Select Countries 15+ Yr", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "+15Y", region: "EURO"), "BT11TREU", 1));
            // EuroTreasury 5-7yr Custom A and Above
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("28587", "BT5ATREU", "Euro Treasury 5-7 Yr Custom A and Above", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: "5-7Y", region: "EURO"), null, 1));

            TREASURY_FILE = new Dictionary<string, BarclaysFileConfiguration[]>();
            TREASURY_FILE.Add("RETURN", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\daily returns.returns") });
            TREASURY_FILE.Add("STAT", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\daily stats.stats", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF) });

            //OECD IG Treasury 25%
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("31497", "BGT7TREU", "OECD IG Treasury 25% capped 7-10y", new BarclaysNominalFamilyObject(country: null, level1: "TREASURIES", level2: "TREASURIES", level3: "TREASURIES", level4: "TREASURIES", rating: null, maturityLevel: null, region: "EURO"), null, 1));

            OECD_FILE = new Dictionary<string, BarclaysFileConfiguration[]>();
            OECD_FILE.Add("RETURN", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\Rets.31497", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF) });
            OECD_FILE.Add("STAT", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\Stats.31497", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF) });

            // euro agregate all
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2000", "LBEATREU", "Euro-Aggregate", new BarclaysNominalFamilyObject(country: null, level1: null, level2: null, level3: null, level4: null, rating: null, maturityLevel: null, region: "EURO"), null, 1));
            // euro agregate corporate
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2002", "LECPTREU", "Euro-Aggregate: Corporates", new BarclaysNominalFamilyObject(country: null, level1: "CORPORATES", level2: null, level3: null, level4: null, rating: null, maturityLevel: null, region: "EURO"), "LBEATREU", 1));
            // euro agregate 1-3
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2109", "LE13TREU", "Euro-Aggregate: 1-3 Year", new BarclaysNominalFamilyObject(country: null, level1: null, level2: null, level3: null, level4: null, rating: null, maturityLevel: "1-3Y", region: "EURO"), "LBEATREU", 1));
            // euro agregate 3-5
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2110", "LE35TREU", "Euro-Aggregate: 3-5 Year", new BarclaysNominalFamilyObject(country: null, level1: null, level2: null, level3: null, level4: null, rating: null, maturityLevel: "3-5Y", region: "EURO"), "LBEATREU", 1));
            // euro agregate 5-7
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2111", "LE57TREU", "Euro-Aggregate: 5-7 Year", new BarclaysNominalFamilyObject(country: null, level1: null, level2: null, level3: null, level4: null, rating: null, maturityLevel: "+10Y", region: "EURO"), "LBEATREU", 1));
            // euro agregate 7-10
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2112", "LE71TREU", "Euro-Aggregate: 7-10 Year", new BarclaysNominalFamilyObject(country: null, level1: null, level2: null, level3: null, level4: null, rating: null, maturityLevel: "7-10Y", region: "EURO"), "LBEATREU", 1));
            // euro agregate 10+
            INDEX_KEYS.AddConfig(new BarclaysIndexConfiguration("2113", "LE10TREU", "Euro-Aggregate: 7-10 Year", new BarclaysNominalFamilyObject(country: null, level1: null, level2: null, level3: null, level4: null, rating: null, maturityLevel: "+10Y", region: "EURO"), "LBEATREU", 1));

            // euro aggregate all
            EUROAGG_FILE = new Dictionary<string, BarclaysFileConfiguration[]>();
            EUROAGG_FILE.Add("RETURN", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\Rets_EA.rets_EA", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF, BarclaysFileConfiguration.MAPPER_CURRENCY_COUNTRY_INVERSION) });
            EUROAGG_FILE.Add("STAT", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\Stats_EA.stats_EA", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF, BarclaysFileConfiguration.MAPPER_CURRENCY_COUNTRY_INVERSION) });

            // euro aggregate corporate
            EUROAGGCORP_FILE = new Dictionary<string, BarclaysFileConfiguration[]>();
            EUROAGGCORP_FILE.Add("RETURN", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\Stats_daily.returns", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF, BarclaysFileConfiguration.MAPPER_CURRENCY_COUNTRY_INVERSION) });
            EUROAGGCORP_FILE.Add("STAT", new BarclaysFileConfiguration[] { new BarclaysFileConfiguration(@"\Stats_daily.stats", BarclaysFileConfiguration.MAPPER_NO_DURATIONMODF, BarclaysFileConfiguration.MAPPER_CURRENCY_COUNTRY_INVERSION) });
        }
        #endregion
        public static string GetMaturityLevel(double? mat)
        {
            if (mat == null)
                return null;
            if (mat < 3)
                return "1-3Y";
            else if (mat < 5)
                return "3-5Y";
            else if (mat < 7)
                return "5-7Y";
            else if (mat < 10)
                return "7-10Y";
            else if (mat < 15)
                return "10-15Y";
            else
                return "+15Y";
        }

        public BarclaysIndexFile(string env = "PREPROD")
        {
            BarclaysIndexFile.ENVIRONNEMENT = env;
        }

        #region previousDays & bank holidays / no index files - Parameters reading
        //TODO manage holidays on a central service (data table ?) // EASTERN MONDAY
        public static List<DateTime> HOLIDAYS = new List<DateTime>() { 
new DateTime(1990, 4, 16), new DateTime(1990, 1, 1),
new DateTime(1991, 4,  1), new DateTime(1991, 1, 1),
new DateTime(1992, 4, 20), new DateTime(1992, 1, 1),
new DateTime(1993, 4, 12), new DateTime(1993, 1, 1),
new DateTime(1994, 4,  4), new DateTime(1994, 1, 1),
new DateTime(1995, 4, 17), new DateTime(1995, 1, 1),
new DateTime(1996, 4,  8), new DateTime(1996, 1, 1),
new DateTime(1997, 3, 31), new DateTime(1997, 1, 1),
new DateTime(1998, 4, 13), new DateTime(1998, 1, 1),
new DateTime(1999, 4,  5), new DateTime(1999, 1, 1),
new DateTime(2000, 4, 24), new DateTime(2000, 1, 1),
new DateTime(2001, 4, 16), new DateTime(2001, 1, 1),
new DateTime(2002, 4,  1), new DateTime(2002, 1, 1),
new DateTime(2003, 4, 21), new DateTime(2003, 1, 1),
new DateTime(2004, 4, 12), new DateTime(2004, 1, 1),
new DateTime(2005, 3, 28), new DateTime(2005, 1, 1),
new DateTime(2006, 4, 17), new DateTime(2006, 1, 1),
new DateTime(2007, 4,  9), new DateTime(2007, 1, 1),
new DateTime(2008, 3, 24), new DateTime(2008, 1, 1),
new DateTime(2009, 4, 13), new DateTime(2009, 1, 1),
new DateTime(2010, 4,  5), new DateTime(2010, 1, 1),
new DateTime(2011, 4, 25), new DateTime(2011, 1, 1),
new DateTime(2012, 4,  9), new DateTime(2012, 1, 1),
new DateTime(2013, 4,  1), new DateTime(2013, 1, 1),
new DateTime(2014, 4, 21), new DateTime(2014, 1, 1),
new DateTime(2015, 4,  6), new DateTime(2015, 1, 1),
new DateTime(2016, 3, 28), new DateTime(2016, 1, 1),
new DateTime(2017, 4, 17), new DateTime(2017, 1, 1),
new DateTime(2018, 4,  2), new DateTime(2018, 1, 1),
new DateTime(2019, 4, 22), new DateTime(2019, 1, 1),
new DateTime(2020, 4, 13), new DateTime(2020, 1, 1) };


        public static DateTime GetPreviousDay(DateTime dateOfData)
        {
            DateTime previousDate;
            if (dateOfData.DayOfWeek.Equals(DayOfWeek.Monday))
                previousDate = dateOfData.AddDays(-3);
            else
                previousDate = dateOfData.AddDays(-1);

            if (HOLIDAYS.Contains(previousDate))
            {
                if (previousDate.DayOfWeek.Equals(DayOfWeek.Monday))
                {
                    previousDate = previousDate.AddDays(-3);
                }
                else
                {
                    previousDate = previousDate.AddDays(-1);
                }
            }
            return previousDate;
        }

        /// <summary>
        /// Argument 0: root path
        /// Argument 1: Return or Stat
        /// Argument 2: File Type
        /// </summary>
        /// <param name="argumentFilesPath"></param>
        /// <returns></returns>
        private static Dictionary<string, string> InitializeParameters(object[] argumentFilesPath)
        {
            string FileRootPath, IndexUniverseType, Index;
            if (argumentFilesPath != null && argumentFilesPath.Length >= 1 && argumentFilesPath[0] is string)
            {
                FileRootPath = argumentFilesPath[0] as string;
            }
            else
            {
                FileRootPath = INDEX_PATH;
            }
            if (argumentFilesPath != null && argumentFilesPath.Length >= 2 && argumentFilesPath[1] is string)
            {
                IndexUniverseType = argumentFilesPath[1] as string;
            }
            else
            {
                IndexUniverseType = INDEX_UNIVERSE_ALL;
            }

            if (argumentFilesPath != null && argumentFilesPath.Length >= 3 && argumentFilesPath[2] is string)
            {

                Index = argumentFilesPath[2] as string;
            }
            else
            {
                Index = INDEX_BARCLAYS_NOMINAL_ALL;
            }

            return new Dictionary<string, string>()
            {
                {"ROOT_PATH",FileRootPath},
                {"INDEX_UNIVERSE",IndexUniverseType},
                {"INDEX",Index}
            };
        }
        private static Dictionary<string, BarclaysFileConfiguration[]> InitializeFileNames(string Index)
        {
            switch (Index)
            {
                case INDEX_TREASURY:
                    return TREASURY_FILE;
                case INDEX_OECD_7_10:
                    return OECD_FILE;
                case INDEX_EUROAGG:
                    return EUROAGG_FILE;
                case INDEX_EUROAGGCORP:
                    return EUROAGGCORP_FILE;
                case INDEX_BARCLAYS_NOMINAL_ALL: // all the files
                default:
                    uint filesNb = 4;
                    BarclaysFileConfiguration[] returnFileNames = new BarclaysFileConfiguration[filesNb];
                    BarclaysFileConfiguration[] statFileNames = new BarclaysFileConfiguration[filesNb];
                    returnFileNames[0] = TREASURY_FILE["RETURN"][0];
                    statFileNames[0] = TREASURY_FILE["STAT"][0];
                    returnFileNames[1] = OECD_FILE["RETURN"][0];
                    statFileNames[1] = OECD_FILE["STAT"][0];
                    returnFileNames[2] = EUROAGG_FILE["RETURN"][0];
                    statFileNames[2] = EUROAGG_FILE["STAT"][0];
                    returnFileNames[3] = EUROAGGCORP_FILE["RETURN"][0];
                    statFileNames[3] = EUROAGGCORP_FILE["STAT"][0];

                    return new Dictionary<string, BarclaysFileConfiguration[]>()
                        { 
                            {INDEX_UNIVERSE_RETURN, returnFileNames},
                            {INDEX_UNIVERSE_STAT, statFileNames} 
                        };
            }
        }
        #endregion

        #region API Public
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateStart">au format "jj/mm/aaaa"</param>
        /// <param name="dateEnd"></param>
        /// <param name="argumentFilesPath">argumentFilesPath[0] constante INDEX_PATH par defaut </br>
        /// argumentFilesPath[1] RETURN (defaut) ou STAT suivant le type de fichier ou </param>
        /// 
        /// 
        public override void ExecuteIndexFileIntegration(DateTime dateStart, DateTime dateEnd, params object[] argumentFilesPath)
        {
            // read the parameters
            Dictionary<string, string> parameters = InitializeParameters(argumentFilesPath);
            // 
            string[] Index_Universe;
            switch (parameters["INDEX_UNIVERSE"])
            {
                case INDEX_UNIVERSE_RETURN:
                    Index_Universe = new String[] { INDEX_UNIVERSE_RETURN };
                    break;
                case INDEX_UNIVERSE_STAT:
                    Index_Universe = new String[] { INDEX_UNIVERSE_STAT };
                    break;
                default:
                    Index_Universe = new String[] { INDEX_UNIVERSE_RETURN, INDEX_UNIVERSE_STAT };
                    break;
            }

            // build the list of filename to read
            IndexFileNames = InitializeFileNames(parameters["INDEX"]);

            IDictionary<ISINIdentifier, double> outstandings;
            IDictionary<ISINIdentifier, BarclaysNominalFamilyObject> bondsFamilyKeys;
            string filePath = null;

            for (DateTime dateOfData = dateStart; dateOfData <= dateEnd; dateOfData = dateOfData.AddDays(1))
            {
                foreach (string universe in Index_Universe)
                {
                    CurrentIndexUniverse = universe;
                    foreach (BarclaysFileConfiguration FileName in IndexFileNames[CurrentIndexUniverse])
                    {
                        try
                        {
                            if (dateOfData.DayOfWeek == DayOfWeek.Saturday || dateOfData.DayOfWeek == DayOfWeek.Sunday)
                                continue;

                            InfoLogger.Info(String.Format("Donnees du {0}  en cours/ Fichier {1} : {2}", dateOfData, FileName.filePattern, DateTime.Now.ToString()));
                            
                            filePath = FileName.GetFilePath(dateOfData, parameters["ROOT_PATH"], ".index.");
                            this.IntegrationBarclaysNominalIndexRerunETL(dateOfData, filePath, FileName, INDEX_KEYS);

                            InfoLogger.Info(String.Format("IntegrationBarclaysNominalIndexRerunETL {0}: OK",filePath));

                            filePath = FileName.GetFilePath(dateOfData, parameters["ROOT_PATH"], ".bond.");
                            this.IntegrationBarclaysNominalBondRerunETL(dateOfData, filePath, FileName, out outstandings, out bondsFamilyKeys);

                            InfoLogger.Info(String.Format("IntegrationBarclaysNominalBondRerunETL {0}: OK", filePath));

                            filePath = FileName.GetFilePath(dateOfData, parameters["ROOT_PATH"], ".map.");
                            this.IntegrationBarclaysNominalMapETL(dateOfData, filePath, INDEX_KEYS, outstandings, bondsFamilyKeys);

                            InfoLogger.Info(String.Format("IntegrationBarclaysNominalMapETL {0}: OK", filePath));
                            InfoLogger.Info(String.Format("Donnees du {0}  en cours/ Fichier {1} : {2}", dateOfData, FileName.filePattern, DateTime.Now.ToString()));
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
                            // next files
                        }
                    }
                }
            }
            getFGAContext.Dispose();
        }
        /// <summary>
        /// les 3 fichiers pour 1 mois en entier : extraction STATPRO
        /// </summary>
        public void TestBarclaysNominalAllMonthETL()
        {
            string FileRootPath = @"G:\,FGA MarketData\INDICES\BARCLAYS\NOMINAL\";
            // Copie en local mais cela n'accèlere pas l execution
            //string FileRootPath = @"C:\DATA\INDICES\BARCLAYS\NOMINAL\";

#if DEBUG
            // creation des scripts
            using (var db = new FGABusinessComponent.BusinessComponent.FGAContext())
            {

                FGABusinessComponent.BusinessComponent.Util.EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
            }
#endif
            DateTime dateStart = DateTime.ParseExact("01/04/2014", "d", new CultureInfo("fr-FR"));
            DateTime dateEnd = DateTime.ParseExact("01/04/2014", "d", new CultureInfo("fr-FR"));
            this.ExecuteIndexFileIntegration(dateStart, dateEnd, FileRootPath);

        }

        #endregion

        #region internal files procedure
        /// <summary>
        ///  Integration des fichiers Barclays Nominal Returns Bond
        ///  sans recreer les donnees en statiques
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <param name="filePath"></param>
        private void IntegrationBarclaysNominalBondRerunETL(DateTime dateOfData,
            string filePath,
            BarclaysFileConfiguration fileConfig,
            out IDictionary<ISINIdentifier, double> outstandingFacialAmount,
            out IDictionary<ISINIdentifier, BarclaysNominalFamilyObject> familyKeys)
        {
            List<DailyReturnsBond> dailyReturns = null;
            using (System.IO.StreamReader file = new StreamReader(filePath))
            {
                EtlFullResult o = Input
                    .ReadFile<DailyReturnsBond>(file)
                    .Transform(fileConfig.bondMapper)
                    .Execute();
                Type t = typeof(DailyReturnsBond);
                // recuperation des objets dans une liste
                dailyReturns = analyse<DailyReturnsBond>(o, t);
            }
            FGAContext db = getFGAContext;
            {
                // preparer la structure qui conservera les données pour le montant facial des bonds
                outstandingFacialAmount = new Dictionary<ISINIdentifier, double>(capacity: dailyReturns.Capacity);
                // preparer la structure qui conservera les clé pour chaque bond
                familyKeys = new Dictionary<ISINIdentifier, BarclaysNominalFamilyObject>(capacity: dailyReturns.Capacity);

                int compteur = 0;
                foreach (DailyReturnsBond bond in dailyReturns)
                {
                    Debt d = (Debt)this.LookupSecurityObject(db, bond);
                    SecuritiesPricing p = this.AddSecurityValuation(db, d, bond, dateOfData);
                    Rating r = this.AddSecurityRating(db, d, bond, dateOfData);
                    AssetClassification ac = this.AddAssetClassification(db, d, bond);
                    // @TODO ne pas calculer la tranche de maturité mais récuperer le sous indice ou se trouve le bond
                    // car un titre doit attendre le rebalancement pour passer à la tranche inferieure
                    BarclaysNominalFamilyObject f = new BarclaysNominalFamilyObject(bond, GetMaturityLevel(bond.Maturity), "EURO");
                    familyKeys.Add(d.ISIN, f);

                    // derniere etape : conserver la donnée du montant facial /outstanding pour la position dans l indice et les sous indices
                    if (CurrentIndexUniverse == INDEX_UNIVERSE_STAT)
                        // prendre la valeur en Ending
                        outstandingFacialAmount.Add(d.ISIN, bond.OutstandE);
                    else // en RETURN: prendre la valeur en Beginning
                        outstandingFacialAmount.Add(d.ISIN, bond.OutstandB);

                    if (++compteur % 500 == 0)
                    {
                        db.SaveChanges();
                    }
                }
                db.SaveChanges();

            }
        }


        /// <summary>
        ///  Integration des fichiers Barclays Nominal Returns Index
        ///  sans recreer les donnees en statiques
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <param name="filePath"></param>
        /// <param name="CODES">le code des indices</param>
        private void IntegrationBarclaysNominalIndexRerunETL(DateTime dateOfData,
            string filePath,
            BarclaysFileConfiguration fileConfig,
            IDictionary<string, BarclaysIndexConfiguration> CODES)
        {
            List<DailyReturnsIndex> dailyReturns = null;
            using (System.IO.StreamReader file = new StreamReader(filePath))
            {
                EtlFullResult o = Input
                    .ReadFile<DailyReturnsIndex>(file)
                    .Transform(fileConfig.indexMapper)
                    .Execute();
                Type t = typeof(DailyReturnsIndex);
                // recuperation des objets dans une liste
                dailyReturns = analyse<DailyReturnsIndex>(o, t);
            }

            FGAContext db = getFGAContext;
            int compteur = 0;
            foreach (DailyReturnsIndex indexLine in dailyReturns)
            {
                BarclaysIndexConfiguration config = CODES[indexLine.idIndex];
                // ignorer la ligne avec le type 2 . correspondant à l indice Hedged (couvert sur le risque de change)
                if (config == null || (indexLine.RetType != config.returnType) || (!config.currency.Equals((CurrencyCode)indexLine.Currency)))
                    continue;

                // search for the index or create it
                Index index = this.LookupIndexObject(db, indexLine, config.tickerId);
                if (!config.indexFamilyKey.Equals(index.FamilyKey))
                {
                    index.FamilyKeyObject = config.indexFamilyKey; 
                    db.Entry(index).Property(i => i.FamilyKey).IsModified = true;
                }

                //add the valuation/index price
                IndexValuation iv = AddIndexValuation(db, index, indexLine, dateOfData, config.tickerId);

                db.Entry(index).State = EntityState.Modified;
                if (++compteur % 500 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
        }

        /// <summary>
        ///  Integration des fichiers Barclays Nominal Returns Map
        ///  sans recreer les donnees en statiques
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <param name="filePath"></param>
        /// <param name="outstandingFacialAmount">l ensemble des montants faciaux pour chaque bond</param>
        private void IntegrationBarclaysNominalMapETL(DateTime dateOfData,
            string filePath,
            IDictionary<string, BarclaysIndexConfiguration> CODES,
            IDictionary<ISINIdentifier, double> outstandingFacialAmount,
            IDictionary<ISINIdentifier, BarclaysNominalFamilyObject> familyKeys)
        {
            List<DailyReturnsMap> dailyReturns = null;
            IDictionary<string, double?> MrktValueAmountByIdIndex = null;
            IDictionary<string, double?> FaceAmountByIdIndex = null;
            IDictionary<string, int?> NumberOfSecuritiesByIdIndex = null;

            using (System.IO.StreamReader file = new StreamReader(filePath))
            {
                EtlFullResult o = Input
                    .ReadFile<DailyReturnsMap>(file)
                    .Execute();
                Type t = typeof(DailyReturnsMap);
                // recuperation des objets dans une liste
                dailyReturns = analyse<DailyReturnsMap>(o, t);
            }
            FGAContext db = getFGAContext;

            // calcul des totaux sur les indices de la markets values
            var totalMrktValue = dailyReturns.GroupBy(dr => dr.idIndex)
                .Select(group => new
                {
                    group.Key,
                    Sum = group.Sum(dr => dr.MrktValue)
                });
            MrktValueAmountByIdIndex = new Dictionary<string, double?>(CODES.Count);
            if (totalMrktValue != null && totalMrktValue.Any())
            {
                foreach (var result in totalMrktValue)
                {
                    MrktValueAmountByIdIndex.Add(result.Key, result.Sum);
                }
            }

            FaceAmountByIdIndex = new Dictionary<string, double?>(CODES.Count);
            NumberOfSecuritiesByIdIndex = new Dictionary<string, int?>(CODES.Count);

            // traitement des lignes Map
            int compteur = 0;
            double? previousFA;
            int? previousNb;
            // List of ISIN that has been holded by SuperIndex
            IDictionary<ISINIdentifier, AssetHolding> SuperIndexedSecurities = new Dictionary<ISINIdentifier, AssetHolding>((int)(dailyReturns.Capacity/1.8));
            IDictionary<ISINIdentifier, DailyReturnsMap> OrphanSecurities = new Dictionary<ISINIdentifier, DailyReturnsMap>((int)(dailyReturns.Capacity / 1.8));
            

            foreach (DailyReturnsMap indexLine in dailyReturns)
            {
                BarclaysIndexConfiguration config = CODES[indexLine.idIndex];
                // lookup for the index object and cache it
                Index index = LookupIndexObject(db, config.tickerId);
                Debt d = LookupDebtObject(db, indexLine.Cusip);

                // Calculation of the FaceAmount of the Index and the nb of bonds                
                if (FaceAmountByIdIndex.TryGetValue(indexLine.idIndex, out previousFA))
                {
                    FaceAmountByIdIndex[indexLine.idIndex] = previousFA + outstandingFacialAmount[d.ISIN];
                }
                else
                {
                    FaceAmountByIdIndex[indexLine.idIndex] = outstandingFacialAmount[d.ISIN];
                }
                if (NumberOfSecuritiesByIdIndex.TryGetValue(indexLine.idIndex, out previousNb))
                {
                    NumberOfSecuritiesByIdIndex[indexLine.idIndex] = previousNb + 1;
                }
                else
                {
                    NumberOfSecuritiesByIdIndex[indexLine.idIndex] = 1;
                }

                // if the map refers to an index that is a "super index" , then add the holding,
                // otherwise , a sub index constituants are found with the FamilyKey under the super-index
                if (config.superIndexTickerId == null)
                {
                    // build the asset holding 
                    AssetHolding holding = this.AddAssetHolding(db, index, d, indexLine, null, dateOfData, outstandingFacialAmount, MrktValueAmountByIdIndex, familyKeys[d.ISIN]);
                    SuperIndexedSecurities[d.ISIN] = holding;
                    OrphanSecurities.Remove(d.ISIN);
                }
                else
                {
                    // just update the family key on the AssetHolding to put the correct maturityLevel
                    // get the asset Holding
                    Index superIndex = LookupIndexObject(db, config.superIndexTickerId);
                    AssetHolding holding = null;
                    SuperIndexedSecurities.TryGetValue(d.ISIN,out holding);
//                    AssetHolding holding = BusinessComponentHelper.LookupAssetHolding(db, superIndex, d, dateOfData);

                    BarclaysNominalFamilyObject f = familyKeys[d.ISIN];
                    BarclaysNominalFamilyObject indexFK = config.indexFamilyKey;
                    f.Set(indexFK);

                    if (holding == null)
                    {
                        familyKeys[d.ISIN] = f;
                        OrphanSecurities[d.ISIN] = indexLine;
                    }
                    else if (!f.Equals(holding.FamilyKey))
                    {
                        holding.FamilyKeyObject = f;
                        db.Entry(holding).State = EntityState.Modified;
                    }
                }

                if (++compteur % 500 == 0)
                {
                    db.SaveChanges();
                }
            }// fin de boucle de traitement des maps bonds-index

            // update 06/2015 : there are 2 bonds available in a sub index but not into the super-index. 
            // For Orphan securities: add a holding in its subindex
            foreach (DailyReturnsMap indexLine in OrphanSecurities.Values)
            {
                BarclaysIndexConfiguration config = CODES[indexLine.idIndex];
                // lookup for the index object and cache it
                Index index = LookupIndexObject(db, config.tickerId);
                Debt d = LookupDebtObject(db, indexLine.Cusip);
                this.AddAssetHolding(db, index, d, indexLine, null, dateOfData, outstandingFacialAmount, MrktValueAmountByIdIndex, familyKeys[d.ISIN]);
            }

            db.SaveChanges();
            // Pour chaque indice
            // faire la somme des faceAmount sur IndexValuation
            foreach (string idIndex in MrktValueAmountByIdIndex.Keys)
            {
                BarclaysIndexConfiguration i = INDEX_KEYS[idIndex];
                double? sumMarketValue = MrktValueAmountByIdIndex[idIndex];
                //double? sumMarketValue = (from ah in db.AssetHoldings
                //                       where ah.Parent.Id == i.Id && ah.Date == dateOfData
                //                       select ah.MarketValue.Value).Sum();
                double? sumFaceAmount = FaceAmountByIdIndex[idIndex];
                //double? sumFaceAmount = (from ah in db.AssetHoldings
                //                         where ah.Parent.Id == i.Id && ah.Date == dateOfData
                //                         select ah.FaceAmount.Value).Sum();
                int? nb = NumberOfSecuritiesByIdIndex[idIndex];
                // chercher avec la date, l objet Valuation                                    
                IndexValuation iv = LookupIndexValuationObject(db, i.tickerId, dateOfData);
                if (iv != null)
                {
                    iv.MarketValue = new CurrencyAndAmount(sumMarketValue * 1000, CurrencyCode.EUR);
                    iv.FaceAmount = new CurrencyAndAmount(sumFaceAmount * 1000, CurrencyCode.EUR);
                    iv.IndexNumberOfSecurities = nb;
                    iv.IndexDivisor = sumMarketValue * 1000 / iv.IndexPriceValue;

                    db.Entry<IndexValuation>(iv).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }
        #endregion

        #region DEBT LINES
        // lookup for the isin bond
        protected override Security LookupSecurityObject(FGAContext db, DailyReturnsBond bond, string ISIN = null)
        {
            Debt d = BusinessComponentHelper.LookupDebt(db, bond.ISIN);
            // lookup for the isin bond            
            if (d == null)
            {
                // not found: create the security with its static data: interest , issuer
                d = new Debt(bond.ISIN);
                db.Debts.Add(d);
            }
            else
            {// get the existing security
                // eager loading / explicit load of Identification
                db.Entry(d).Reference(i => i.Identification).Load();
            }
            CountryCode issuerCountry = (CountryCode)bond.Country;
            if (!d.isStaticDataValid() || !d.Identification.CUSIP.Equals(bond.Cusip))
            {
                d.Identification.CUSIP = new CUSIPIdentifier(bond.Cusip);
                d.Identification.TickerSymbol = new TickerIdentifier(bond.Ticker);
                d.Identification.DomesticIdentificationSource = issuerCountry;
                d.FinancialInstrumentName = issuerCountry.Label + ' ' + bond.Coupon + ' ' + bond.MaturDate.ToShortDateString();
                d.MaturityDate = bond.MaturDate;
                InterestCalculation interest = new InterestCalculation(new PercentageRate(bond.Coupon), (FrequencyCode)(bond.CouponFrq));
                d.NextInterest = interest;

                if (db.Entry<Debt>(d).State == EntityState.Added)
                    db.SaveChanges();

                //Capital amountIssued = new Capital(Date: dateOfData, Amount: new CurrencyAndAmount(value: , currency: (CurrencyCode)bond.ProductCurrency), Type: CapitalTypeCode.OUTSTANDING);
                db.Entry<SecuritiesIdentification>(d.Identification).State = EntityState.Modified;

                //d.Add(amountIssued);


                db.Entry<Debt>(d).State = EntityState.Modified;
            }
            // if there is Issuer role
            IssuerRole role = BusinessComponentHelper.LookupIssuerRole(db, d);

            if (role == null)
            {
                role = new IssuerRole(bond.Issuer, issuerCountry);
                d.Add(role);
                role.Set(d);
                db.IssuerRoles.Add(role);
            }
            else
            {
                if (role.IssuerName.Equals(bond.Issuer, StringComparison.CurrentCultureIgnoreCase))
                {
                    role.IssuerName = bond.Issuer;
                    role.Country = issuerCountry;
                    db.Entry<Role>(role).State = EntityState.Modified;
                }
            }

            return d;
        }

        protected override SecuritiesPricing AddSecurityValuation(FGAContext db, Security debt, DailyReturnsBond bond, DateTime dateOfData, string ISIN = null)
        {
            Debt d = (Debt)debt;
            // chercher le prix precedent , afin de recalculer une perf Daily avec la MTD
            DateTime previousDate;
            Yield previousYield = null;
            List<SecuritiesPricing> prices;
            Yield yield;

            previousDate = GetPreviousDay(dateOfData);

            // si le jour est le 1er du mois
            if (previousDate.Month != dateOfData.Month)
            {
                //Performance (la perf Daily correspond à la MTD)
                yield = new Yield(ChangePrice_1D: bond.RetTotal, ChangePrice_MTD: bond.RetTotal);
            }
            else
            {
                // calcul de la perf 1D avec la perf precedent et la perf MTD actuelle
                // le prix precedent
                SecuritiesPricing previousPrice = BusinessComponentHelper.LookupSecurityPricingObject(db, bond.ISIN, previousDate, "BARCLAYS");
                if (previousPrice != null)
                {
                    previousYield = previousPrice.Yield;
                }

                //Performance
                yield = new Yield(ChangePrice_MTD: bond.RetTotal);
                if (previousYield != null)
                {
                    yield.ChangePrice_1D = new PercentageRate(100 * ((1 + yield.ChangePrice_MTD.Value / 100) / (1 + previousYield.ChangePrice_MTD.Value / 100) - 1));
                }
            }

            //Prix et performance
            CurrencyAndAmount price = new CurrencyAndAmount(bond.PriceE, (CurrencyCode)bond.Currency);
            DebtYield dy = new DebtYield(YieldToMaturityRate: bond.ISMA_Yld, YieldToWorstRate: bond.YldWorstE);
            DebtSpread ds = new DebtSpread(OptionAdjustedSpread: bond.OAS_bp);
            // Calcul de la Duration:
            // sensi * ( 1 + YTM / frequence)
            double? duration = bond.ISMA_MDur * (1 + bond.ISMA_Yld / (int)d.NextInterest.CalculationFrequency);
            double? durationSemiAnnual = bond.DurAdjMod * (1 + bond.ISMA_Yld / (int)d.NextInterest.CalculationFrequency);
            DebtDataCalculation debtDataCalculation = new DebtDataCalculation(
                // Modified Duration based on ISMA convention, which is annual coupon convention. European bonds typically are based on an annual
                                                                    ModifiedDuration: bond.ISMA_MDur,
                                                                    MacaulayDuration: duration,
                // Based on semi-annual coupon convention           
                                                                    MacaulayDurationSemiAnnual: durationSemiAnnual,
                                                                    ModifiedDurationSemiAnnual: bond.DurAdjMod,
                                                                    TimeToMaturity: bond.Maturity);


            DebtPriceCalculation debtPriceCalculation = new DebtPriceCalculation(CleanPrice: bond.PriceE,
                                                AccruedInterest: bond.AccrIntE);

            // chercher avec la date, l objet Price
            SecuritiesPricing p = BusinessComponentHelper.LookupSecurityPricingObject(db, bond.ISIN, dateOfData, "BARCLAYS");

            if (p == null)
            {
                //PriceFactType priceFactType = new PriceFactType();
                p = new SecuritiesPricing(price, dateOfData, (TypeOfPriceCode)"MARKET", Yield: yield,
                DebtDataCalculation: debtDataCalculation, DebtPriceCalculation: debtPriceCalculation,
                DebtYield: dy, DebtSpread: ds);
                p.PriceSource = "BARCLAYS";
                p.Set(d);
                // do not load the Pricing List. too large structure
                //d.Pricing.Add(p);
                db.SecuritiesPricings.Add(p);
                db.SaveChanges();
            }
            else if (INDEX_UNIVERSE_RETURN.Equals(CurrentIndexUniverse))
            {
                p.Yield = yield;
                p.DebtDataCalculation = debtDataCalculation;
                p.DebtPriceCalculation = debtPriceCalculation;
                p.DebtYield = dy;
                p.DebtSpread = ds;
                p.PriceSource = "BARCLAYS";
                db.Entry(p).Property(pr => pr.Yield).IsModified = true;
                db.Entry<SecuritiesPricing>(p).State = EntityState.Modified;
            }
            return p;
        }

        protected override Rating AddSecurityRating(FGAContext db, Security security, DailyReturnsBond bond, DateTime dateOfData, string ISIN = null)
        {

            Debt d = (Debt)security;
            // chercher le rating
            Rating r = new Rating(Value: bond.QualityE, ValueDate: dateOfData, RatingScheme: "BARCLAYS");
            r.Fitch = bond.Qual_Ftch;
            r.SnP = bond.Qual_SaP;
            r.Moody = bond.Qual_Mood;

            Rating existingR = BusinessComponentHelper.LookupValidRating(db, bond.ISIN, dateOfData, "BARCLAYS");

            // If the Rating is not the same
            if ((existingR != null) && (!existingR.Equals(r)))
            {
                existingR = null;
            }
            // if no rating or not the same
            if (existingR == null)
            {
                d.SetRating(r);
                db.Ratings.Add(r);
                db.SaveChanges();
            }
            return r;
        }

        protected override AssetClassification AddAssetClassification(FGAContext db, Asset security, DailyReturnsBond bondLine, string ISIN = null)
        {
            Debt d = (Debt)security;
            // chercher avec la classif
            AssetClassification classif = BusinessComponentHelper.LookupAssetClassification(db, bondLine.ISIN, "BARCLAYS");
            if (classif == null)
            {
                classif = new AssetClassification("BARCLAYS");
                d.Add(classif);
                db.AssetClassifications.Add(classif);
                db.SaveChanges();
            }

            classif.Classification1 = bondLine.IssrClsL1;
            classif.Classification2 = bondLine.IssrClsL2;
            classif.Classification3 = bondLine.IssrClsL3;
            classif.Classification4 = bondLine.IssrClsL4;
            classif.Classification5 = null;
            classif.Classification6 = null;
            classif.Classification7 = null;
            db.Entry<AssetClassification>(classif).State = EntityState.Modified;

            return classif;
        }
        #endregion

        #region HOLDING MNGT

        protected override AssetHolding AddAssetHolding(FGAContext db, Index index, Asset security, BarclaysIndexesFile indexL, DailyReturnsBond securityLine, DateTime dateOfData, params Object[] additionalparameters)
        {
            DailyReturnsMap indexLine = (DailyReturnsMap)indexL;
            Debt d = (Debt)security;
            IDictionary<ISINIdentifier, double> outstandingFacialAmount = (IDictionary<ISINIdentifier, double>)additionalparameters[0];
            IDictionary<string, double?> MrktValueAmountByIdIndex = (IDictionary<string, double?>)additionalparameters[1];
            BarclaysNominalFamilyObject bondFamilyKey = (BarclaysNominalFamilyObject)additionalparameters[2];

            AssetHolding holding = BusinessComponentHelper.LookupAssetHolding(db, index, security, dateOfData);
            double faceAmount = outstandingFacialAmount[d.ISIN];
            if (holding == null)
            {
                holding = new AssetHolding(Date: dateOfData, ISIN: d.ISIN.ISINCode,
                HoldAsset: d,
                Holder: index,
                    //Notional // Principal // Par Value * quantity
                    FaceAmount: new CurrencyAndAmount(faceAmount * 1000, CurrencyCode.EUR),
                    // calcul de la quantité
                    MarketValue: new CurrencyAndAmount(indexLine.MrktValue * 1000, CurrencyCode.EUR),
                    Weight: new PercentageRate(100* indexLine.MrktValue / MrktValueAmountByIdIndex[indexLine.idIndex]),
                    Quantity: (float)indexLine.OutstandE * 1000000 / d.MinimumIncrement.Unit);

                holding.FamilyKeyObject = bondFamilyKey;
                db.AssetHoldings.Add(holding);
                db.SaveChanges();
            }
            else
            {
                holding.FaceAmount = new CurrencyAndAmount(faceAmount * 1000, CurrencyCode.EUR);
                holding.MarketValue = new CurrencyAndAmount(indexLine.MrktValue * 1000, CurrencyCode.EUR);
                holding.Weight = new PercentageRate(100 * indexLine.MrktValue / MrktValueAmountByIdIndex[indexLine.idIndex]);
                holding.Quantity = (float)indexLine.OutstandE * 1000000 / d.MinimumIncrement.Unit;
                holding.FamilyKeyObject = bondFamilyKey;
                db.Entry(holding).State = EntityState.Modified;
            }

            return holding;
        }
        #endregion

        #region FOREX MNGT
        protected override CurrencyExchange LookupForexRateObject(FGAContext db, Object currencyExchangeLine, DateTime date, string ISIN = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region INDEX MNGT
        /// <summary>
        /// Cache for the Index Objects
        /// </summary>
        private Dictionary<string, Index> INDEX_CACHE; // key is the barclays Id or the ISIN Id

        private Index GetCachedIndex(string id, string universe = INDEX_UNIVERSE_RETURN)
        {
            if (INDEX_CACHE == null)
            {
                INDEX_CACHE = new Dictionary<string, Index>();
            }

            if (INDEX_UNIVERSE_STAT.Equals(universe, StringComparison.CurrentCultureIgnoreCase))
            {
                if (INDEX_CACHE.ContainsKey(id + INDEX_UNIVERSE_STAT))
                {
                    return INDEX_CACHE[id + INDEX_UNIVERSE_STAT];
                }
                else
                {
                    return null;
                }
            }
            else if (INDEX_CACHE.ContainsKey(id))
            {
                return INDEX_CACHE[id];
            }
            else
            {
                return null;
            }
        }

        private void SetCachedIndex(string id, Index index, string universe = INDEX_UNIVERSE_RETURN)
        {
            if (INDEX_UNIVERSE_STAT.Equals(universe, StringComparison.CurrentCultureIgnoreCase))
            {
                INDEX_CACHE[id + INDEX_UNIVERSE_STAT] = index;
            }
            else
            {
                INDEX_CACHE[id] = index;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="indexLine"></param>
        /// <param name="ISIN">l identifiant de l index, dans notre cas: le ticker Bloomberg</param>
        /// <returns></returns>
        protected override Index LookupIndexObject(FGAContext db, BarclaysIndexesFile indexL, string ISIN)
        {
            DailyReturnsIndex indexLine = (DailyReturnsIndex)indexL;
            Index index;
            index = GetCachedIndex(ISIN, CurrentIndexUniverse);
            if (index == null)
            {
                // lookup for the indice object
                string code = ISIN;
                if (CurrentIndexUniverse == INDEX_UNIVERSE_STAT)
                {
                    code += "STAT";
                }
                List<Index> results = db.Indexes.Where<Index>(t => t.Identification.OtherIdentification == indexLine.idIndex && t.Identification.SecurityIdentification.ISINCode == code).ToList();
                if (results == null || results.Count == 0)
                {
                    //TRANSCO code Barclays avec un tickr Bloom en id principal
                    string label;
                    string superTickerId;

                    BarclaysIndexConfiguration config = INDEX_KEYS[indexLine.idIndex];
                    if (CurrentIndexUniverse == INDEX_UNIVERSE_STAT)
                    {
                        label = "BARCLAYS STAT " + config.label;
                        superTickerId = config.superIndexTickerId + "STAT";
                    }
                    else
                    {
                        label = "BARCLAYS " + config.label;
                        superTickerId = config.superIndexTickerId;
                    }

                    index = new Index(Name: label, ISIN: code, IndexCurrency: (CurrencyCode)indexLine.Currency);
                    index.IndexFrequency = FrequencyCode.getFrequencyByLabel("DAILY");
                    index.Identification.OtherIdentification = indexLine.idIndex;
                    index.Identification.Bloomberg = (BloombergIdentifier)ISIN;

                    // Add if need , this index into its super-index
                    if (config.superIndexTickerId != null)
                    {
                        BarclaysIndexConfiguration configSuperIndex = INDEX_KEYS[config.superIndexTickerId];
                        DailyReturnsIndex superIndexLine = new DailyReturnsIndex();
                        superIndexLine.FullName = configSuperIndex.label;
                        superIndexLine.idIndex = configSuperIndex.internalId;

                        Index superIndex = this.LookupIndexObject(db, superIndexLine, config.superIndexTickerId);
                        if (superIndex != null && (index.Parent == null || !index.Parent.Equals(superIndex)))
                        {
                            index.Parent = superIndex;
                        }
                    }
                    db.Indexes.Add(index);
                    db.SaveChanges();
                }
                else
                {
                    index = results.First<Index>();
                }
                SetCachedIndex(ISIN, index, CurrentIndexUniverse);
            }
            return index;
        }

        protected Index LookupIndexObject(FGAContext db, string ISIN)
        {
            Index index = null;
            index = GetCachedIndex(ISIN, CurrentIndexUniverse);
            if (index == null)
            {
                string code = ISIN;
                if (CurrentIndexUniverse == INDEX_UNIVERSE_STAT)
                {
                    code += "STAT";
                }
                // lookup for the indice object
                index = BusinessComponentHelper.LookupIndexObjectById(db, code);

                if (index != null)
                {
                    SetCachedIndex(ISIN, index, CurrentIndexUniverse);
                }
            }
            return index;
        }

        protected Debt LookupDebtObject(FGAContext db, string Cusip)
        {
            List<Debt> obligs = db.Debts.Where<Debt>(t => t.Identification.CUSIP.CUSIPCode.Equals(Cusip)).ToList();
            if (obligs == null || obligs.Count == 0)
            {
                throw new Exception("Debt inconnu " + Cusip);
            }
            else
            {
                return obligs.First<Debt>();
            }
        }
        private IndexValuation LookupIndexValuationObject(FGAContext db, string ISIN, DateTime dateOfData)
        {
            IndexValuation indexV = null;
            string code = ISIN;
            if (CurrentIndexUniverse == INDEX_UNIVERSE_STAT)
            {
                code += "STAT";
            }
            indexV = BusinessComponentHelper.LookupIndexValuationObject(db, code, dateOfData, "BARCLAYS_" + CurrentIndexUniverse);
            return indexV;
        }

        protected override IndexValuation AddIndexValuation(FGAContext db, Index index, BarclaysIndexesFile indexL, DateTime dateOfData, String isin)
        {
            DailyReturnsIndex indexLine = (DailyReturnsIndex)indexL;
            // chercher avec la date, l objet Valuation
            IndexValuation valuation = LookupIndexValuationObject(db, isin, dateOfData);

            //Valuation du sous indice                    
            Yield yield = new Yield(ChangePrice_1D: indexLine.RetDaily, ChangePrice_MTD: indexLine.RetMTD, ChangePrice_YTD: indexLine.RetYTD);
            DebtDataCalculation debtCalculation = new DebtDataCalculation(TimeToMaturity: indexLine.Maturity, ModifiedDuration: indexLine.DurationModF ?? indexLine.DurationModB);
            DebtSpread ds = new DebtSpread(OptionAdjustedSpread: indexLine.AvgOAS);
            DebtYield dy = new DebtYield(YieldToWorstRate: indexLine.YieldWorst);

            if (valuation == null)
            {
                valuation = new IndexValuation(dateOfData,
                    Valuated: index,
                    // market Value a prendre dans le fichier bond et recalcul pour l index
                    // face amount à prendre dans le fichier Bond et recalcul pour l index
                    IndexPriceValue: indexLine.IndexValue,
                    DebtYield: dy,
                    DebtSpread: ds,
                    DebtDataCalculation: debtCalculation);
                valuation.ValuationSource = "BARCLAYS_" + CurrentIndexUniverse;
                db.Valuations.Add(valuation);
                db.SaveChanges();
                // do not load the Pricing List. too large structure
                //index.Add(valuation);
            }
            else
            {
                // face amount à prendre dans le fichier Bond
                valuation.MarketValue = new CurrencyAndAmount(indexLine.MarketValue, CurrencyCode.EUR);
                valuation.IndexPriceValue = indexLine.IndexValue;
                valuation.ValuationSource = "BARCLAYS_" + CurrentIndexUniverse;
                valuation.DebtYield = dy;
                db.Entry(valuation).Property(pr => pr.DebtYield).IsModified = true;
                valuation.DebtSpread = ds;
                db.Entry(valuation).Property(pr => pr.DebtSpread).IsModified = true;
                valuation.DebtDataCalculation = debtCalculation;
                db.Entry(valuation).Property(pr => pr.DebtDataCalculation).IsModified = true;
                db.Entry(valuation).State = EntityState.Modified;
            }
            return valuation;
        }
        #endregion

        /// <summary>
        /// Construit la liste des objets de donnees avec la reflection
        /// </summary>
        /// <param name="o"></param>
        /// <param name="recordClass"></param>
        /// <returns></returns>
        private static List<T> analyse<T>(EtlFullResult o, Type recordClass)
        {
            IEnumerable<Row> all = o.Data;
            List<T> objects = new List<T>();

            foreach (Row items in all)
            {
                string member = null;
                object instance = Activator.CreateInstance(recordClass);

                foreach (PropertyInfo info in GetProperties(instance))
                {
                    member = info.Name;
                    if (items.Contains(info.Name) && info.CanWrite)
                        info.SetValue(instance, items[info.Name], null);
                }
                foreach (FieldInfo info in GetFields(instance))
                {
                    member = info.Name;
                    if (items.Contains(info.Name))
                        info.SetValue(instance, items[info.Name]);
                }

                objects.Add((T)instance);
            }
            return objects;

        }

        #region util Reflection

        static readonly Dictionary<Type, List<PropertyInfo>> propertiesCache = new Dictionary<Type, List<PropertyInfo>>();
        static readonly Dictionary<Type, List<FieldInfo>> fieldsCache = new Dictionary<Type, List<FieldInfo>>();
        /// <summary>
        /// retourne la liste 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static List<PropertyInfo> GetProperties(object obj)
        {
            List<PropertyInfo> properties;
            if (propertiesCache.TryGetValue(obj.GetType(), out properties))
                return properties;

            properties = new List<PropertyInfo>();
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                    continue;
                properties.Add(property);
            }
            propertiesCache[obj.GetType()] = properties;
            return properties;
        }

        private static List<FieldInfo> GetFields(object obj)
        {
            List<FieldInfo> fields;
            if (fieldsCache.TryGetValue(obj.GetType(), out fields))
                return fields;

            fields = new List<FieldInfo>();
            foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                fields.Add(fieldInfo);
            }
            fieldsCache[obj.GetType()] = fields;
            return fields;
        }
        #endregion

    }


    /// <summary>
    /// Format du fichier Barclays Nominal (returns et stat)
    /// ( voir Smadar Shulman chez Barclays )
    /// 
    /// En returns (Nominal):
    /// La market Value de chaque titre (fichiers Map) reste constante dans le mois, jusqu'au rebalancement de fin de mois
    /// Par contre, dans le fichier Bonds , la Market Value est calculé par l'univers Stats (en daily)
    /// Seuls les prix varient en daily
    /// 
    /// E veut dire Ending (-> univers statistics)
    /// B Beginning (-> univers returns)
    /// 
    /// Identique entre returns et Stats
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class DailyReturnsBond
    {
        public string Cusip;
        public string ISIN;
        public string Ticker;
        public string Issuer;
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime MaturDate;
        public string Country;
        public string Currency;
        public int OutstandB;
        public int OutstandE;
        public double Coupon;
        public int CouponFrq;
        public double? PriceE;
        public double? AccrIntE;
        public double? MrktValue; // market value calculé sur l univers Stats (variant en Daily)
        public string IssrClsL1;
        public string IssrClsL2;
        public string IssrClsL3;
        public string IssrClsL4;
        public string Qual_Mood;
        public string Qual_SaP;
        public string Qual_Ftch;
        public string QualityE;
        public double? RetTotal;
        public double? Maturity;
        [FieldConverter(typeof(NULLConverter))]
        public double? YldWorstE;
        [FieldConverter(typeof(NULLConverter))]
        public double? ISMA_Yld;
        public double? OAS_bp;
        [FieldConverter(typeof(NULLConverter))]
        public double? ISMA_MDur; // Modified duration based on the ISMA convention which is annual coupon convention. european bonds typically are based on an annual
        [FieldConverter(typeof(NULLConverter))]
        public double? DurAdjMod; // modified duration but it based on semi-annual coupon convention
        public string ProductCurrency;
        [FieldOptional, FieldNullValue(null)]
        public double? DurAdjMdB;
    }

    public interface BarclaysIndexesFile
    { }

    /// <summary>
    /// Identique entre returns et Stats
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class DailyReturnsIndex : BarclaysIndexesFile
    {
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime dtValue;
        public string idIndex;
        public int RetType;
        public string FullName;
        public string Currency;
        public double? RetDaily;
        public double? RetMTD;
        public double? RetYTD;
        public double? IndexValue;
        public double? AvgOAS;
        public double? DurationModF;
        public double? YieldWorst;
        public double? MarketValue;// market value calculé sur l univers Stats (variant en Daily)
        public double? Maturity;
        public double? OutstandingE;
        [FieldOptional, FieldNullValue(null)]
        public double? DurationModB;
    }
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class DailyReturnsMap : BarclaysIndexesFile
    {
        public string idIndex;
        public string Cusip;
        public double? MrktValue;// market value calculé sur l univers Stats (variant en Daily)
        public int OutstandE;
        public string Currency;
    }

    /// <summary>
    /// converter to manage the null string in file
    /// </summary>
    internal class NULLConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            if ("NULL".Equals(from, StringComparison.CurrentCultureIgnoreCase))
            {
                return null;
            }
            else
            {
                Double d = Double.Parse(FGA.Automate.Helpers.StringHelper.RemoveBlanks(from), NumberStyles.Number | NumberStyles.AllowExponent);
                return d;
            }
        }
    }
}
