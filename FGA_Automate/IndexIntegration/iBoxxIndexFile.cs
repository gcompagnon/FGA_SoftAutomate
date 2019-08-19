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
using FGABusinessComponent.BusinessComponent.Util;
using System.Text;
using log4net.Config;
using log4net;
using System.Text.RegularExpressions;
using System.Collections;
using FGABusinessComponent.BusinessComponent;
using ReactiveETL;
using FGABusinessComponent.BusinessComponent.Security.Fx;

// lecture du fichier de configuration pour les LOG
//[assembly: XmlConfigurator(ConfigFile = "Resources/Config/log4net.config", Watch = false)]

namespace FGA.Automate.IndexIntegration
{
    #region Utility Classes
    [Serializable()]
    internal class IBOXXFamilyObject : Stringeable
    {

        private string FK_REGEX = @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*" + SEPARATOR + @"\S*";

        public IBOXXFamilyObject()
        {
        }
        /// <summary>
        /// Read a sec line to extract the family Key
        /// </summary>
        /// <param name="sec"></param>
        public IBOXXFamilyObject(EnDOfDayUnderlyings sec)
        {
            this.Level0 = (sec.Level0.Equals("*") ? null : sec.Level0);
            this.Level1 = (sec.Level1.Equals("*") ? null : sec.Level1);
            this.Level2 = (sec.Level2.Equals("*") ? null : sec.Level2);
            this.Level3 = (sec.Level3.Equals("*") ? null : sec.Level3);
            this.Level4 = (sec.Level4.Equals("*") ? null : sec.Level4);
            this.MarkitRating = (sec.MarkitiBoxxRating.Equals("*") ? null : sec.MarkitiBoxxRating);
            this.SeniorityLevel1 = (sec.SeniorityLevel1.Equals("*") ? null : sec.SeniorityLevel1);
            this.SeniorityLevel2 = (sec.SeniorityLevel2.Equals("*") ? null : sec.SeniorityLevel2);
            this.SeniorityLevel3 = (sec.SeniorityLevel3.Equals("*") ? null : sec.SeniorityLevel3);

            string matLevel = null;
            // for an corp bond, it could be several maturity level possible
            // keep only one amonsgt the finest level:
            ReturnMaturityLevelOrNull(sec._1_3Years, "1-3Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._3_5Years, "3-5Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._5_7Years, "5-7Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._7_10Years, "7-10Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._10_15Years, "10-15Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._15_20Years, "15-20Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._20_25Years, "20-25Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._25_30Years, "25-30Y", ref matLevel);
            ReturnMaturityLevelOrNull(sec._30nPYears, "+30Y", ref matLevel);

            if (matLevel == null)
            {
                iBoxxIndexFile.ExceptionLogger.Error("No maturity Level for " + sec.ISIN + " on date: " + sec.Date + " => other maturity level put");
                ReturnMaturityLevelOrNull(sec._1_10Years, "1-10Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._1_15Years, "1-15Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._1_20Years, "1-20Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._1_5Years, "1-5Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._10nPYears, "+10Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._15_25Years, "15-25Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._15nPYears, "+15Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._25nPYears, "+25Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._5_10Years, "5-10Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._5_15Years, "5-15Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._5nPYears, "+5Y", ref matLevel);
                ReturnMaturityLevelOrNull(sec._7nPYears, "+7Y", ref matLevel);
            }
            this.MaturityLevel = matLevel;

        }

        private bool ReturnMaturityLevelOrNull(bool? flag, string level, ref string variable)
        {
            if (flag.HasValue && flag.Value)
            {
                variable = level;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Read a Index Line to extract a pattern , that will be use for searching sec into that index
        /// </summary>
        /// <param name="i"></param>
        public IBOXXFamilyObject(EnDOfDayIndices i)
        {
            this.Level0 = (i.Level0.Equals("*") ? null : i.Level0);
            this.Level1 = (i.Level1.Equals("*") ? null : i.Level1);
            this.Level2 = (i.Level2.Equals("*") ? null : i.Level2);
            this.Level3 = (i.Level3.Equals("*") ? null : i.Level3);
            this.Level4 = (i.Level4.Equals("*") ? null : i.Level4);
            this.MarkitRating = (i.MarkitIBoxxRating.Equals("*") ? null : i.MarkitIBoxxRating);
            this.SeniorityLevel1 = (i.Seniority_Level1.Equals("*") ? null : i.Seniority_Level1);
            this.SeniorityLevel2 = (i.Seniority_Level2.Equals("*") ? null : i.Seniority_Level2);
            this.SeniorityLevel3 = (i.Seniority_Level3.Equals("*") ? null : i.Seniority_Level3);

            string matLevel = null;
            // for an index , only one MaturityLevel possible
            ReturnMaturityLevelOrNull(i._1_3Years, "1-3Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._3_5Years, "3-5Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._5_7Years, "5-7Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._7_10Years, "7-10Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._1_10Years, "1-10Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._1_15Years, "1-15Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._1_20Years, "1-20Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._1_5Years, "1-5Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._10_15Years, "10-15Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._10nPlusYears, "+10Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._15_20Years, "15-20Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._15_25Years, "15-25Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._15nPlusYears, "+15Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._20_25Years, "20-25Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._25_30Years, "25-30Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._25nPlusYears, "+25Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._30nPlusYears, "+30Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._5_10Years, "5-10Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._5_15Years, "5-15Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._5nPlusYears, "+5Y", ref matLevel);
            ReturnMaturityLevelOrNull(i._7nPlusYears, "+7Y", ref matLevel);

            this.MaturityLevel = matLevel;

        }
        public string Level0 { get; set; }
        public string Level1 { get; set; }
        public string Level2 { get; set; }
        public string Level3 { get; set; }
        public string Level4 { get; set; }
        public string MarkitRating { get; set; }

        public string SeniorityLevel1 { get; set; }
        public string SeniorityLevel2 { get; set; }
        public string SeniorityLevel3 { get; set; }

        public string MaturityLevel { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append(Level0 == null ? "%" : Level0).Append(SEPARATOR);
            sb.Append(Level1 == null ? "%" : Level1).Append(SEPARATOR);
            sb.Append(Level2 == null ? "%" : Level2).Append(SEPARATOR);
            sb.Append(Level3 == null ? "%" : Level3).Append(SEPARATOR);
            sb.Append(Level4 == null ? "%" : Level4).Append(SEPARATOR);

            sb.Append(MarkitRating == null ? "%" : MarkitRating).Append(SEPARATOR);

            sb.Append(SeniorityLevel1 == null ? "%" : SeniorityLevel1).Append(SEPARATOR);
            sb.Append(SeniorityLevel2 == null ? "%" : SeniorityLevel2).Append(SEPARATOR);
            sb.Append(SeniorityLevel3 == null ? "%" : SeniorityLevel3).Append(SEPARATOR);

            sb.Append(MaturityLevel == null ? "%" : MaturityLevel);

            return sb.ToString();
        }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        public override bool Equals(System.Object obj)
        {
            String s = obj as String;
            IBOXXFamilyObject f = obj as IBOXXFamilyObject;
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

        public override void FromString(string s)
        {
            Match match = Regex.Match(s, @"content/([%A-Za-z0-9\-]+)\.aspx$",
        RegexOptions.IgnoreCase);

            // Here we check the Match instance.
            if (match.Success)
            {
                // Finally, we get the Group value and display it.
                string key = match.Groups[1].Value;
            }

            MatchCollection mc = Regex.Matches(s, FK_REGEX);

            IEnumerator e = mc.GetEnumerator();
            Match m = (Match)e.Current;
            Level0 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level1 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level2 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level3 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            Level4 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;

            MarkitRating = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            SeniorityLevel1 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            SeniorityLevel2 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            SeniorityLevel3 = (m.Value.Equals("%") ? null : m.Value); e.MoveNext(); m = (Match)e.Current;
            MaturityLevel = (m.Value.Equals("%") ? null : m.Value);
        }

    }
    #endregion

    /// <summary>
    /// Description résumée pour iBoxxIndexFile:
    /// 
    /// Integration des indices en Daily :
    /// fichiers IBOXX_EUR_EOD_INDICES (l ensemble des sous indices)
    /// et IBOXX_EUR_EOD_UNDERLYINGS (les composants/obligations  et les markets values)
    /// 
    /// en Monthly:
    ///  IBOXX_EOM_COMPONENTS
    ///  IBOXX_EOM_XREF  (poids)
    /// </summary>
    public class iBoxxIndexFile : IndexFileIntegration<EnDOfDayIndices, EnDOfDayUnderlyings, Object>
    {
        #region Logger
        public static ILog InfoLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_MARKIT_EXTRACTION_LOG"); }
        }

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_MARKIT_EXTRACTION"); }
        }
        #endregion

        public const string INDEX_PATH = @"\\vill1\Partage\,FGA MarketData\INDICES\MARKIT\IBOXX\";
        private static string ENVIRONNEMENT = "DEV";
        public iBoxxIndexFile(string env = "PREPROD")
        {
            iBoxxIndexFile.ENVIRONNEMENT = env;
        }

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

        /// <summary>
        /// Pas de forex dans les fichiers Markit
        /// </summary>
        /// <param name="db"></param>
        /// <param name="currencyExchangeLine"></param>
        /// <returns></returns>
        protected override CurrencyExchange LookupForexRateObject(FGAContext db, object currencyExchangeLine, DateTime date, string ISIN = null)
        {
            throw new NotImplementedException();
        }

        #region API

        /// <summary>
        /// les 3 fichiers pour 1 mois en entier
        /// </summary>
        public void TestMarkitIboxxDailyAllMonthETL()
        {
            string iboxxFilesPath = @"G:\,FGA MarketData\INDICES\MARKIT\IBOXX\";
            DateTime dateStart = DateTime.ParseExact("04/04/2014", "d", new CultureInfo("fr-FR"));
            DateTime dateEnd = DateTime.ParseExact("04/04/2014", "d", new CultureInfo("fr-FR"));
            this.ExecuteIndexFileIntegration(dateStart, dateEnd, iboxxFilesPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateStart">au format "jj/mm/aaaa"</param>
        /// <param name="dateEnd"></param>
        /// <param name="rootPath">constante INDEX_PATH par defaut</param>
        /// 
        public override void ExecuteIndexFileIntegration(DateTime dateStart, DateTime dateEnd, params object[] argumentFilesPath)
        {
            string iboxxFilesPath;
            if (argumentFilesPath != null && argumentFilesPath.Length > 1 && argumentFilesPath[0] is string)
            {
                iboxxFilesPath = argumentFilesPath[0] as string;
            }
            else
            {
                iboxxFilesPath = INDEX_PATH;
            }


            for (DateTime dateOfData = dateStart; dateOfData <= dateEnd; dateOfData = dateOfData.AddDays(1))
            {
                //DateTime dateOfData = DateTime.ParseExact("21/09/2012", "d", new CultureInfo("fr-FR"));

                InfoLogger.Info("Donnees du " + dateOfData + " en cours: " + DateTime.Now.ToString());

                if (dateOfData.DayOfWeek == DayOfWeek.Saturday || dateOfData.DayOfWeek == DayOfWeek.Sunday)
                {
                    InfoLogger.Error("La date est un WE. Pas d integration " + dateOfData);
                    continue;
                }

                string d1 = dateOfData.ToString("yyMMdd");
                string d2 = dateOfData.ToString("yyyyMMdd");

                string filePath = iboxxFilesPath + d1 + @"\iboxx_eur_eod_indices_" + d2 + ".csv";

                try
                {
                    this.IntegrationMarkitIBoxxIndicesDailyRerunETL(dateOfData, filePath);
                    InfoLogger.Info("IntegrationMarkitIBoxxIndicesDailyRerunETL: OK");

                    filePath = iboxxFilesPath + d1 + @"\iboxx_eur_eod_underlyings_" + d2 + ".csv";
                    this.IntegrationMarkitIBoxxUnderlyingsDailyRerunETL(dateOfData, filePath);
                    InfoLogger.Info("IntegrationMarkitIBoxxUnderlyingsDailyRerunETL: OK");
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
                InfoLogger.Info("Donnees du " + dateOfData + " integrees: " + DateTime.Now.ToString());


            }
        }

        #endregion

        #region INDEX DAILY

        protected override Index LookupIndexObject(FGAContext db, EnDOfDayIndices indexLine, string ISIN = null)
        {
            Index index;
            string name = indexLine.Name;
            name = name.Replace("€", "EUR");
            // lookup for the indice object
            index = BusinessComponentHelper.LookupIndexObjectById(db, indexLine.ISIN_Tri);

            if (index == null)
            {
                //Index to create
                index = new Index(Name: name, ISIN: indexLine.ISIN_Tri, IndexCurrency: (CurrencyCode)indexLine.Level0);
                db.Indexes.Add(index);
                db.SaveChanges();
            }

            IBOXXFamilyObject f = new IBOXXFamilyObject(indexLine);
            //// specify modification
            if (!f.Equals(index.FamilyKey))
            {
                index.FamilyKeyObject = f;
                db.Entry(index).Property(i => i.FamilyKey).IsModified = true;
            }

            if (!index.isStaticDataValid())
            {
                index.Name = name;
                index.IndexFrequency = FrequencyCode.getFrequencyByLabel("DAILY");
                index.Identification.Bloomberg = (BloombergIdentifier)indexLine.Code_Tri;
                db.Entry<SecuritiesIdentification>(index.Identification).State = EntityState.Modified;
                db.Entry(index).State = EntityState.Modified;
            }

            //// eager loading / explicit load of Identification
            db.Entry(index).Reference(i => i.Identification).Load();

            return index;
        }


        protected override IndexValuation AddIndexValuation(FGAContext db, Index index, EnDOfDayIndices indexLine, DateTime dateOfData, string ISIN = null)
        {
            IndexValuation valuation = BusinessComponentHelper.LookupIndexValuationObject(db, indexLine.ISIN_Tri, dateOfData, "IBOXX_EUR");

            if (valuation == null)
            {
                valuation = new IndexValuation(dateOfData,
                    Valuated: index,
                    BaseValue: indexLine.BaseMarketValue,
                    BaseDate: new DateTime(dateOfData.Year, dateOfData.Month, 1),
                    FaceAmount: new CurrencyAndAmount(indexLine.NominalValue, CurrencyCode.EUR),
                    BookValue: new CurrencyAndAmount(indexLine.BaseMarketValue, CurrencyCode.EUR),
                    MarketValue: new CurrencyAndAmount(indexLine.MarketValue, CurrencyCode.EUR),
                    IndexPriceValue: indexLine.Cpi_Today,
                    IndexGrossValue: indexLine.GrossPriceIndex,
                    IndexNetValue: indexLine.Tri_Today);
                valuation.IndexNumberOfSecurities = indexLine.NumberOfBonds;
                valuation.ValuationSource = "IBOXX_EUR";
                db.Valuations.Add(valuation);
                db.SaveChanges();
                //index.Add(valuation);
            }
            else
            {
                valuation.FaceAmount = new CurrencyAndAmount(indexLine.NominalValue, CurrencyCode.EUR);
                // valeur au rebalancement
                valuation.IndexBaseValue = indexLine.BaseMarketValue;
                valuation.IndexBaseDate = new DateTime(dateOfData.Year, dateOfData.Month, 1);
                valuation.BookValue = new CurrencyAndAmount(indexLine.BaseMarketValue, CurrencyCode.EUR);
                valuation.MarketValue = new CurrencyAndAmount(indexLine.MarketValue, CurrencyCode.EUR);
                valuation.IndexPriceValue = indexLine.Cpi_Today;
                valuation.IndexGrossValue = indexLine.GrossPriceIndex;
                valuation.IndexNetValue = indexLine.Tri_Today;
                valuation.IndexNumberOfSecurities = indexLine.NumberOfBonds;
                valuation.ValuationSource = "IBOXX_EUR";
                db.Entry(valuation).State = EntityState.Modified;
            }
            valuation.Yield.ChangePrice_MTD = new PercentageRate(100 * indexLine.MonthToDateReturn);
            valuation.Yield.ChangePrice_1D = new PercentageRate(100 * indexLine.DailyReturn);
            valuation.Yield.ChangePrice_YTD = new PercentageRate(100 * indexLine.Annual_Yield);
            db.Entry(valuation).Property(pr => pr.Yield).IsModified = true;

            valuation.DebtSpread.OptionAdjustedSpread = indexLine.OAS;
            valuation.DebtSpread.ZeroVolatilitySpread = indexLine.ZSpread;
            db.Entry(valuation).Property(pr => pr.DebtSpread).IsModified = true;

            valuation.DebtDataCalculation.Convexity = indexLine.Annual_Convexity;
            valuation.DebtDataCalculation.ConvexitySemiAnnual = indexLine.SemiAnnual_Convexity;
            valuation.DebtDataCalculation.MacaulayDuration = indexLine.Duration;
            valuation.DebtDataCalculation.ModifiedDuration = indexLine.Annual_ModifiedDuration;
            valuation.DebtDataCalculation.ModifiedDurationSemiAnnual = indexLine.SemiAnnual_ModifiedDuration;
            valuation.DebtDataCalculation.TimeToMaturity = indexLine.ExpectedRemainingLife;
            db.Entry(valuation).Property(pr => pr.DebtDataCalculation).IsModified = true;
            return valuation;
        }
        /// <summary>
        /// lookup (create) the main indices
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, Index> lookupMainIndexes(FGAContext db, String[] mainIndexes = null)
        {
            if (mainIndexes == null)
            {// IBOXX EUR ALL , IBOXX EUR CORPORATE
                String[] s = { "DE0009682716" };
                mainIndexes = s;
            }

            Dictionary<string, Index> r = new Dictionary<string, Index>();
            Index index;
            foreach (string id in mainIndexes)
            {
                index = BusinessComponentHelper.LookupIndexObjectById(db, id);
                r.Add(id, index);
            }
            return r;
        }
        private IDictionary<string, IndexValuation> lookupMainIndexesValuations(FGAContext db, DateTime date, String[] mainIndexes = null)
        {
            if (mainIndexes == null)
            {// IBOXX EUR ALL , IBOXX EUR CORPORATE
                String[] s = { "DE0009682716" };
                mainIndexes = s;
            }

            Dictionary<string, IndexValuation> r = new Dictionary<string, IndexValuation>(capacity: mainIndexes.Length);
            IndexValuation indexV;
            foreach (string id in mainIndexes)
            {
                indexV = BusinessComponentHelper.LookupIndexValuationObject(db, id, date, "IBOXX_EUR");
                r.Add(id, indexV);
            }
            return r;
        }


        /// <summary>
        ///  Integration des fichiers Barclays Nominal Returns Bond: INDEX
        ///  sans recreer les donnees en statiques
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <param name="filePath"></param>
        /// 
        private void IntegrationMarkitIBoxxIndicesDailyRerunETL(DateTime dateOfData, string filePath)
        {

            List<EnDOfDayIndices> dailyReturns = null;
            using (System.IO.StreamReader file = new StreamReader(filePath, Encoding.GetEncoding(1252)))
            {
                string content = file.ReadToEnd();
                file.BaseStream.Position = 0;
                EtlFullResult o = Input
                    .ReadFile<EnDOfDayIndices>(file)
                    .Execute();
                Type t = typeof(EnDOfDayIndices);
                // recuperation des objets dans une liste
                dailyReturns = analyse<EnDOfDayIndices>(o, t);
            }

            FGAContext db = getFGAContext;

            int compteur = 0;
            foreach (EnDOfDayIndices indexLine in dailyReturns)
            {
                Index index = LookupIndexObject(db, indexLine);
                IndexValuation iv = AddIndexValuation(db, index, indexLine, dateOfData);
                if (++compteur % 500 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
        }
        #endregion INDEX

        #region DEBT LINES
        // lookup for the isin bond
        protected override Security LookupSecurityObject(FGAContext db, EnDOfDayUnderlyings bond, string ISIN = null)
        {
            Debt d = null;
            bool changes = false;
            d = BusinessComponentHelper.LookupDebt(db, bond.ISIN);

            if (d == null)
            {
                d = new Debt(bond.ISIN);
                db.Debts.Add(d);
                // les donnees statiques sont remplies plus tard                
            }

            // si les donnees statiques ne sont pas remplies
            if (!d.isStaticDataValid())
            {
                DateTime mat = bond.FinalMaturity ?? DateTime.Now;
                d.FinancialInstrumentName = bond.Issuer + ' ' + bond.Coupon + ' ' + mat.ToShortDateString();
                d.MaturityDate = bond.FinalMaturity;
                InterestCalculation interest = new InterestCalculation(new PercentageRate(bond.Coupon), (FrequencyCode)(bond.CouponFrequency));
                d.NextInterest = interest;
                if (db.Entry<Debt>(d).State == EntityState.Added)
                    db.SaveChanges();
            }

            // if there is Issuer role
            IssuerRole role = BusinessComponentHelper.LookupIssuerRole(db, d);

            if (role == null)
            {
                role = new IssuerRole(bond.Issuer, CountryCode.getCountryByLabel(bond.IssuerCountry));
                d.Add(role);
                role.Set(d);
                db.IssuerRoles.Add(role);
                changes = true;
            }
            else if (CountryCode.DEFAULT.Equals(role.Country))
            {
                role.Country = CountryCode.getCountryByLabel(bond.IssuerCountry);
                db.Entry(role).State = EntityState.Modified;
                changes = true;
            }

            // Ticker has changed
            if (d.Identification.TickerSymbol == null || d.Identification.TickerSymbol.TickerCode == null || d.Identification.TickerSymbol.TickerCode.Trim().Length == 0
                || !d.Identification.TickerSymbol.Equals(bond.Ticker))
            {
                d.Identification.TickerSymbol = new TickerIdentifier(bond.Ticker);
                d.Identification.Name = d.FinancialInstrumentName;
                db.Entry<SecuritiesIdentification>(d.Identification).State = EntityState.Modified;
                changes = true;
            }

            if (changes)
            {
                db.Entry(d).State = EntityState.Modified;
                db.SaveChanges();
            }

            return d;
        }

        protected override AssetClassification AddAssetClassification(FGAContext db, Asset security, EnDOfDayUnderlyings bondLine, string ISIN = null)
        {
            Debt bond = (Debt)security;

            // chercher avec la classif
            AssetClassification classif = BusinessComponentHelper.LookupAssetClassification(db, bondLine.ISIN, "IBOXX_EUR");
            if (classif == null)
            {
                classif = new AssetClassification("IBOXX_EUR");
                bond.Add(classif);
                classif.Asset = bond;
                db.AssetClassifications.Add(classif);
                db.SaveChanges();
            }
            string l0 = bondLine.Level0.Equals("*") ? null : bondLine.Level0;
            string l1 = bondLine.Level1.Equals("*") ? null : bondLine.Level1;
            string l2 = bondLine.Level2.Equals("*") ? null : bondLine.Level2;
            string l3 = bondLine.Level3.Equals("*") ? null : bondLine.Level3;
            string l4 = bondLine.Level4.Equals("*") ? null : bondLine.Level4;
            string l5 = bondLine.Level4.Equals("*") ? null : bondLine.Level5;
            string l6 = bondLine.Level4.Equals("*") ? null : bondLine.Level6;

            // comparison du Tuple classif.Classification1 à 5 avec bondLine.  
            if (!(StringComparer.OrdinalIgnoreCase.Equals(classif.Classification1, l0) &&
                    StringComparer.OrdinalIgnoreCase.Equals(classif.Classification2, l1) &&
                    StringComparer.OrdinalIgnoreCase.Equals(classif.Classification3, l2) &&
                    StringComparer.OrdinalIgnoreCase.Equals(classif.Classification4, l3) &&
                    StringComparer.OrdinalIgnoreCase.Equals(classif.Classification5, l4)&&
                    StringComparer.OrdinalIgnoreCase.Equals(classif.Classification6, l5)&&
                    StringComparer.OrdinalIgnoreCase.Equals(classif.Classification7, l6)
                )) 
            {
                classif.Classification1 = l0;
                classif.Classification2 = l1;
                classif.Classification3 = l2;
                classif.Classification4 = l3;
                classif.Classification5 = l4;
                classif.Classification6 = l5;
                classif.Classification7 = l6;
                db.Entry<AssetClassification>(classif).State = EntityState.Modified;
            }
            // niveau de seniorité: prendre le niveau le plus détaillé en premier
            SeniorityLevelCode senLevel = null;
            if (bondLine.SeniorityLevel3 == null || bondLine.SeniorityLevel3.Equals("*"))
            {
                if (bondLine.SeniorityLevel2 == null || bondLine.SeniorityLevel2.Equals("*"))
                {
                    if (bondLine.SeniorityLevel1 == null || bondLine.SeniorityLevel1.Equals("*") || bondLine.SeniorityLevel1.Equals("NW"))
                    {
                        // Rien a faire. pas de seniority ou alors en NeW
                    }
                    else
                    {
                        senLevel = new SeniorityLevelCode(bondLine.SeniorityLevel1);
                    }
                }
                else
                {
                    senLevel = new SeniorityLevelCode(bondLine.SeniorityLevel2);
                }
            }
            else
            {
                senLevel = new SeniorityLevelCode(bondLine.SeniorityLevel3);
            }

            if (senLevel != null && !bond.FinancialInfos.Seniority.Equals(senLevel))
            {
                bond.FinancialInfos.Seniority = senLevel;
                bond.FinancialInfos.HybridCapital = bondLine.IsHybridCapital;
                db.Entry(bond).State = EntityState.Modified;
            }
            if (bond.PerpetualIndicator != bondLine.IsPerpetual || bond.FixedToVariableIndicator != bondLine.IsFixedtoFloat)
            {
                bond.PerpetualIndicator = bondLine.IsPerpetual;
                bond.FixedToVariableIndicator = bondLine.IsFixedtoFloat;
                db.Entry(bond).State = EntityState.Modified;
            }
            return classif;
        }

        protected override Rating AddSecurityRating(FGAContext db, Security security, EnDOfDayUnderlyings bond, DateTime dateOfData, string ISIN = null)
        {
            Debt d = (Debt)security;
            // chercher le rating
            Rating r = new Rating(Value: bond.MarkitiBoxxRating, ValueDate: dateOfData, RatingScheme: "IBOXX_EUR");

            Rating existingR = BusinessComponentHelper.LookupValidRating(db, bond.ISIN, dateOfData, "IBOXX_EUR");

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


        protected override SecuritiesPricing AddSecurityValuation(FGAContext db, Security debt, EnDOfDayUnderlyings bond, DateTime dateOfData, string ISIN = null)
        {
            //Prix et performance            
            CurrencyAndAmount price = new CurrencyAndAmount(bond.DirtyPrice - bond.AccruedInterest, CurrencyCode.EUR);
            DebtYield dy = new DebtYield(YieldToMaturityRate: bond.AnnualYield);
            DebtSpread ds = new DebtSpread(GovSpread: bond.AnnualBenchmarkSpreadToBMCurve, ZeroVolatilitySpread: bond.ZSpread, OptionAdjustedSpread: bond.OAS, AssetSwapSpread: bond.AssetSwapMargin);

            Yield yield = new Yield(ChangePrice_1D: 100 * bond.DailyReturn, ChangePrice_MTD: 100 * bond.Month_To_DateReturn);

            // Calcul de la Duration:
            DebtDataCalculation debtDataCalculation = new DebtDataCalculation(
                // Modified Duration based on ISMA convention, which is annual coupon convention. European bonds typically are based on an annual
                                                                    ModifiedDuration: bond.AnnualModifiedDuration,
                                                                    MacaulayDuration: bond.Duration,
                                                                    OADuration: bond.EffectiveOAduration,
                // Based on semi-annual coupon convention           
                                                                    ModifiedDurationSemiAnnual: bond.Semi_AnnualModifiedDuration,
                                                                    TimeToMaturity: bond.ExpectedRemainingLife,

                                                                    Convexity: bond.AnnualConvexity,
                                                                    ConvexitySemiAnnual: bond.Semi_AnnualConvexity,
                                                                    OAConvexity: bond.OAConvexity
                                                                    );
            DebtPriceCalculation debtPriceCalculation = new DebtPriceCalculation(DirtyPrice: bond.DirtyPrice, CleanPrice: bond.DirtyPrice - bond.AccruedInterest,
                                                AccruedInterest: bond.AccruedInterest);

            // chercher avec la date, l objet Price
            SecuritiesPricing p = BusinessComponentHelper.LookupSecurityPricingObject(db, bond.ISIN, dateOfData, "IBOXX_EUR");
            if (p == null)
            {
                //PriceFactType priceFactType = new PriceFactType();
                p = new SecuritiesPricing(price, dateOfData, (TypeOfPriceCode)"MARKET", Yield: yield,
                DebtDataCalculation: debtDataCalculation, DebtPriceCalculation: debtPriceCalculation,
                DebtYield: dy, DebtSpread: ds);
                p.PriceSource = "IBOXX_EUR";
                p.Set(debt);
                // do not load the Pricing List. too large structure
                //debt.Pricing.Add(p);
                db.SecuritiesPricings.Add(p);
                db.SaveChanges();
            }
            else
            {
                p.Yield = yield;
                p.DebtDataCalculation = debtDataCalculation;
                p.DebtPriceCalculation = debtPriceCalculation;
                p.DebtYield = dy;
                p.DebtSpread = ds;
                p.PriceSource = "IBOXX_EUR";

                db.Entry<SecuritiesPricing>(p).State = EntityState.Modified;
            }
            return p;
        }


        /// <summary>
        ///  Integration des fichiers Barclays Nominal Returns Index: DEBT
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <param name="filePath"></param>
        private void IntegrationMarkitIBoxxUnderlyingsDailyRerunETL(DateTime dateOfData, string filePath)
        {

            List<EnDOfDayUnderlyings> dailyReturns = null;
            using (System.IO.StreamReader file = new StreamReader(filePath, Encoding.GetEncoding(1252)))
            {
                string content = file.ReadToEnd();
                file.BaseStream.Position = 0;
                EtlFullResult o = Input
                    .ReadFile<EnDOfDayUnderlyings>(file)
                    .Execute();
                Type t = typeof(EnDOfDayUnderlyings);
                // recuperation des objets dans une liste
                dailyReturns = analyse<EnDOfDayUnderlyings>(o, t);
            }
            FGAContext db = getFGAContext;
            IDictionary<string, Index> mainIndexes = lookupMainIndexes(db);
            IDictionary<string, IndexValuation> mainIndexesValuation = lookupMainIndexesValuations(db, dateOfData);

            int compteur = 0;
            foreach (EnDOfDayUnderlyings bond in dailyReturns)
            {
                Debt d = (Debt)this.LookupSecurityObject(db, bond);
                SecuritiesPricing p = this.AddSecurityValuation(db, d, bond, dateOfData);
                AssetClassification ac = this.AddAssetClassification(db, d, bond);
                Rating r = this.AddSecurityRating(db, d, bond, dateOfData);

                this.AddAssetHolding(db, mainIndexes["DE0009682716"], d, null, bond, dateOfData, mainIndexesValuation["DE0009682716"]);
                if (++compteur % 500 == 0)
                {
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
        }

        protected override AssetHolding AddAssetHolding(FGAContext db, Index index, Asset security, EnDOfDayIndices indexLine, EnDOfDayUnderlyings securityLine, DateTime dateOfData, params Object[] additionalparameters)
        {
            IndexValuation indexValuation = null;
            if (additionalparameters != null && additionalparameters.Length > 0)
            {
                indexValuation = additionalparameters[0] as IndexValuation;
            }

            AssetHolding holding = BusinessComponentHelper.LookupAssetHolding(db, index, security, dateOfData);

            double? indexMV = null;
            if (indexValuation != null && indexValuation.MarketValue != null && indexValuation.MarketValue.Value != null)
            {
                indexMV = indexValuation.MarketValue.Value;
                if (indexValuation.MarketValue.Currency == CurrencyCode.MUSD ||
                    indexValuation.MarketValue.Currency == CurrencyCode.MEUR)
                {
                    indexMV = indexMV * 1E6;
                }
            }
            else if (indexLine != null)
            {
                indexMV = indexLine.MarketValue;
            }

            IBOXXFamilyObject f = new IBOXXFamilyObject(securityLine);
            if (holding == null)
            {
                holding = new AssetHolding(Date: dateOfData, ISIN: securityLine.ISIN,
                HoldAsset: security,
                Holder: index,
                    //Notional // Principal // Par Value * quantity
                    FaceAmount: new CurrencyAndAmount(securityLine.NotionalAmount, CurrencyCode.EUR),
                    MarketValue: new CurrencyAndAmount(securityLine.MarketValue, CurrencyCode.EUR),
                    Weight: indexMV == null ? new PercentageRate() : new PercentageRate(100 * securityLine.MarketValue / indexMV),
                    Quantity: 1);
                holding.FamilyKeyObject = f;
                db.AssetHoldings.Add(holding);
                db.SaveChanges();
            }
            else
            {
                holding.ISIN = (ISINIdentifier)securityLine.ISIN;
                holding.MarketValue = new CurrencyAndAmount(securityLine.MarketValue, CurrencyCode.EUR);
                holding.FaceAmount = new CurrencyAndAmount(securityLine.NotionalAmount, CurrencyCode.EUR);
                holding.Weight = indexMV == null ? new PercentageRate() : new PercentageRate(100 * securityLine.MarketValue / indexMV);
                holding.Quantity = 1;
                holding.FamilyKeyObject = f;
                db.Entry<AssetHolding>(holding).State = EntityState.Modified;
            }

            return holding;
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

        #region util lookup & cache Indexes
        internal static readonly Dictionary<string, Index> IndexesCache = new Dictionary<string, Index>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Utilitaire qui recherche l object Index
        /// </summary>
        /// <param name="db">object d acces à la source de données</param>
        /// <param name="ISIN">optionel : le code ISIN</param>
        /// <param name="BBTicker">optionel: le code bloomberg </param>
        private static Index lookupIndex(FGABusinessComponent.BusinessComponent.FGAContext db, String ISIN = null, String BBTicker = null)
        {
            Index foundIndex = null;
            if (ISIN != null && IndexesCache.ContainsKey(ISIN))
            {
                foundIndex = IndexesCache[ISIN];
            }
            else if (BBTicker != null && IndexesCache.ContainsKey(BBTicker))
            {
                foundIndex = IndexesCache[BBTicker];
            }
            else if (ISIN != null)
            {
                foundIndex = BusinessComponentHelper.LookupIndexObjectById(db, ISIN);
                if (foundIndex != null)
                {
                    IndexesCache.Add(ISIN, foundIndex);
                }
            }
            else if (BBTicker != null)
            {
                List<Index> resultsIndexes = db.Indexes.Include("Identification").Where<Index>(t => t.Identification.Bloomberg.BBCode == BBTicker).ToList();
                if (resultsIndexes != null && resultsIndexes.Count > 0)
                {
                    foundIndex = resultsIndexes.First<Index>();
                    IndexesCache.Add(BBTicker, foundIndex);
                }
            }
            return foundIndex;
        }
        #endregion

    }
    #region FileHelpers
    /// <summary>
    /// Format du fichier Markit Iboxx EOD_Indices
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class EnDOfDayIndices_OLD
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public string ISIN_Cpi;
        public string ISIN_Tri;
        public string Code_Cpi;
        public string Code_Tri;
        public string Name;
        public double? Cpi_Today;
        public double? Tri_Today;
        public double? Cpi_Yesterday;
        public double? Tri_Yesterday;
        public double? Duration;
        public double? Portfolio_Duration;
        public double? Annual_Yield;
        public double? Annual_ModifiedDuration;
        public double? Annual_Convexity;
        public double? Annual_Portfolio_Yield;
        public double? Annual_Portfolio_ModifiedDuration;
        public double? Annual_Portfolio_Convexity;
        public double? SemiAnnual_Yield;
        public double? SemiAnnual_ModifiedDuration;
        public double? SemiAnnual_Convexity;
        public double? SemiAnnual_Portfolio_Yield;
        public double? SemiAnnual_Portfolio_ModifiedDuration;
        public double? SemiAnnual_Portfolio_Convexity;
        public double? ExpectedRemainingLife;
        public double? Coupon;
        public double? NominalValue;
        public double? MarketValue;
        public double? BaseMarketValue; // market value de debut de mois
        public string Level0;
        public string Level1;
        public string Level2;
        public string Level3;
        public string Level4;
        public string MarkitIBoxxRating;
        public string Seniority_Level1;
        public string Seniority_Level2;
        public string Seniority_Level3;
        public double? PaidCash;
        public double? AnnualIndexBenchmarkSpread;
        public double? SemiAnnualIndexBenchmarkSpread;
        public double? DailyReturn;
        public double? MonthToDateReturn;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_3Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _3_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_7Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _7_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _10_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _20_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _25_30Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _7nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _10nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _25nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _30nPlusYears;
        public double? GrossPriceIndex;
        public double? CouponIncomeIndex;
        public double? RedemptionIncomeIndex;
        public double? IncomeIndex;
        public int NumberOfBonds;
        public double? AssetSwapMargin;
        public double? OAS;
        public double? OAConvexity;
        public double? ZSpread;
        public double? DV_01;
        public double? EffectiveDuration;
        public double? DailyExcessReturnOverSovereigns;
        public double? DailyExcessReturnOverLIBOR;
        public double? Month_to_dateExcessReturnOverSovereigns;
        public double? Month_to_dateExcessReturnOverLIBOR;
        public double? DurationWeightedExposure;
        public double? AnnualBenchmarkSpreadToBM_Curve;
        public double? Semi_AnnualBenchmarkSpreadToBM_Curve;
    }

    /// <summary>
    /// Format du fichier Markit Iboxx EOD_Underlying
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class EnDOfDayUnderlyings_OLD
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public string ISIN;
        public string Ticker;
        [FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
        public string Issuer;
        public double? Coupon;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? FinalMaturity;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime WorkoutDate;
        public double? ExpectedRemainingLife;
        public int? CouponFrequency;
        public Int64? NotionalAmount;
        public double? BidPrice;
        public double? AskPrice;
        public double? AccruedInterest;
        public double? CouponPayment;
        public double? CouponAdjustment;
        public double? DirtyPrice;
        public double? MarketValue;
        public double? StreetYield;
        public double? AnnualYield;
        public double? Semi_AnnualYield;
        public double? Duration;
        public double? StreetModifiedDuration;
        public double? AnnualModifiedDuration;
        public double? Semi_AnnualModifiedDuration;
        public double? StreetConvexity;
        public double? AnnualConvexity;
        public double? Semi_AnnualConvexity;
        public string BenchmarkISIN;
        public double? AnnualBenchmarkSpread;
        public double? Semi_AnnualBenchmarkSpread;
        public double? AssetSwapMargin;
        public string Level0;
        public string Level1;
        public string Level2;
        public string Level3;
        public string Level4;
        public string MarkitiBoxxRating;
        public string SeniorityLevel1;
        public string SeniorityLevel2;
        public string SeniorityLevel3;
        public double? CashPayment;
        public double? DailyReturn;
        public double? Month_To_DateReturn;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? Ex_Dividend; // 1 si le bond est entré ds l indice dans l ex dividende date : eligible pour le paiement du coupon
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? IsFixedtoFloat;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? IsPerpetual;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? IsHybridCapital;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _1_3Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _1_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _1_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _1_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _1_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _3_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _5_7Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _5_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _5_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _7_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _10_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _15_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _15_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _20_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _25_30Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _5nPYears; //5+
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _7nPYears; //7+
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _10nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _15nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _25nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? _30nPYears;
        public string PriceType;
        public int? NumberofContributors;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? NextCallDate;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? NextCouponDate;
        public double? OAS;
        public double? OAConvexity;
        public double? ZSpread;
        public double? DV01; //credit spread sensitivity
        public double? EffectiveDuration;
        public double? DailyExcessReturnOverSovereigns;
        public double? DailyExcessReturnOverLIBOR;
        public double? MonthtodateExcessReturnOverSovereigns;
        public double? MonthtodateExcessReturnOverLIBOR;
        public double? DurationWeightedExposure;
        public double? AnnualBenchmarkSpreadToBMCurve;
        public double? SemiAnnualBenchmarkSpreadtoBMCurve;
    }
    #endregion

    #region FileHelpers_UPDATED // evolution
    /// <summary>
    /// Format du fichier Markit Iboxx EOD_Indices
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class EnDOfDayIndices
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public string Fixing;
        public string PriceType;
        public string ISIN_Cpi;
        public string ISIN_Tri;
        public string Code_Cpi; // BBG_Ticker_CPi;
        public string Code_Tri; //BBG_Ticker_TRi;
        public string Name;
        public double? Cpi_Today;
        public double? Tri_Today;
        public double? Cpi_previous_EOM;
        public double? Tri_previous_EOM;
        public double? CostFactor_TRi;
        public double? Cash;
        public double? CostFactor_CPi;
        public double? InterestonCash;
        public double? SimpleMargin;
        public double? DiscountMargin;
        public double? Duration;
        public double? Duration_to_Maturity;
        public double? Portfolio_Duration;
        public double? PortfolioDuration_to_Maturity;
        public double? Annual_Yield;
        public double? Annual_Yield_to_Maturity;
        public double? Annual_ModifiedDuration;
        public double? Annual_ModifiedDuration_to_Maturity;
        public double? Annual_Convexity;
        public double? Annual_Convexity_to_Maturity;
        public double? Annual_Portfolio_Yield;
        public double? Annual_Portfolio_Yield_to_Maturity;
        public double? Annual_Portfolio_ModifiedDuration;
        public double? Annual_Portfolio_ModifiedDuration_to_Maturity;
        public double? Annual_Portfolio_Convexity;
        public double? Annual_Portfolio_Convexity_to_Maturity;
        public double? SemiAnnual_Yield;
        public double? SemiAnnual_YieldtoMaturity;
        public double? SemiAnnual_ModifiedDuration;
        public double? SemiAnnual_ModifiedDurationToMaturity;
        public double? SemiAnnual_Convexity;
        public double? SemiAnnual_ConvexityToMaturity;
        public double? SemiAnnual_PortfolioYield;
        public double? SemiAnnual_PortfolioYieldToMaturity;
        public double? SemiAnnual_PortfolioModifiedDuration;
        public double? SemiAnnual_PortfolioModifiedDurationToMaturity;
        public double? SemiAnnual_PortfolioConvexity;
        public double? SemiAnnual_PortfolioConvexityToMaturity;
        public double? OAS;
        public double? EffectiveDuration; //EffectiveOAduration;
        public double? OAConvexity;
        public double? ZSpread;
        public double? ZSpreadOverLibor;
        public double? ExpectedRemainingLife;
        public double? Coupon;
        public double? NominalValue;
        public double? MarketValue;
        public double? BaseMarketValue;
        public string Level0;
        public string Level1;
        public string Level2;
        public string Level3;
        public string Level4;
        public string Level5;
        public string Level6;
        public string Level7;
        public string Level8;
        public string MarkitIBoxxRating;
        public string Seniority_Level1;
        public string Seniority_Level2;
        public string Seniority_Level3;
        public double? PaidCash;
        public double? AnnualIndexBenchmarkSpread;
        public double? Semi_annualIndexBenchmarkSpread;
        public double? AnnualBenchmarkSpreadtoBM_Curve;
        public double? Semi_AnnualBenchmarkSpreadtoBM_Curve;
        public double? AssetSwapMargin;
        public double? DV01;
        public string FXVersion;
        public string IndexCurrency;
        public string TaxConsideration;
        public double? DailyReturn;
        public double? MonthToDateReturn;
        public double? QuarterToDateReturn;
        public double? YearToDateReturn;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_3Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _3_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_7Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _7_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _10_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _20_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _25_30Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _7nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _10nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _25nPlusYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _30nPlusYears;
        public double? GrossPriceIndex;
        public double? CouponIncomeIndex;
        public double? RedemptionIncomeIndex;
        public double? IncomeIndex;
        public int NumberOfBonds;
        public double? DailySovereignCurveSwapReturn;
        public double? DailyLiborSwapReturn;
        public double? Month_to_dateSovereignCurveSwapReturn;
        public double? Month_to_dateLiborSwapReturn;
        public double? Durationweightedexposure;
    }

    /// <summary>
    /// Format du fichier Markit Iboxx EOD_Underlying
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class EnDOfDayUnderlyings
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public string Fixing;
        public string PriceType;
        public string FXVersion;
        public string IndexISIN_CPi;
        public string IndexISIN_TRi;
        public string IndexName;
        public string ISIN;
        public string CUSIP;
        public string Identifier;
        public string Local1;
        public string Local2;
        public string Ticker;
        [FieldQuoted('"', QuoteMode.OptionalForBoth, MultilineMode.NotAllow)]
        public string Issuer;
        public string IssuerCountry;
        public double? Coupon;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Workoutdate;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? FinalMaturity;
        public double? ExpectedRemainingLife;
        public double? TimeToMaturity;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? NextCallDate;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? NextCouponDate;
        public double? CouponFrequency;
        public double? BidPrice;
        public double? AskPrice;
        public double? Bid_Ask_Spread;
        public double? IndexPrice;
        public double? AccruedInterest;
        public double? DirtyPrice; //DirtyIndexPrice;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? Ex_Dividend; // 1 si le bond est entré ds l indice dans l ex dividende date : eligible pour le paiement du coupon
        public double? CouponPayment;
        public double? CouponAdjustment;
        public double? CurrentRedemptionPayment;
        public double? redemptionFactor;
        public double? PIKFactor;
        public double? NotionalAmount;
        public double? CappedNotionalAmount;
        public double? MarketValue;
        public double? CappedMarketValue;
        public double? CashPayment;
        public double? CappedCashPayment;
        public double? StreetYield;
        public double? AnnualYield;
        public double? Semi_AnnualYield;
        public double? StreetYieldtoMaturity;
        public double? AnnualYieldtoMaturity;
        public double? Semi_AnnualYieldtoMaturity;
        public double? SimpleMargin;
        public double? DiscountMargin;
        public double? Duration;
        public double? StreetModifiedDuration;
        public double? AnnualModifiedDuration;
        public double? Semi_AnnualModifiedDuration;
        public double? StreetModifiedDurationtoMaturity;
        public double? AnnualModifiedDurationtoMaturity;
        public double? Semi_AnnualModifiedDurationtoMaturity;
        public double? EffectiveOAduration;
        public double? StreetConvexity;
        public double? AnnualConvexity;
        public double? Semi_AnnualConvexity;
        public double? StreetConvexitytoMaturity;
        public double? AnnualConvexitytoMaturity;
        public double? Semi_AnnualConvexitytoMaturity;
        public double? OAConvexity;
        public string BenchmarkISIN;
        public double? AnnualBenchmarkSpread;
        public double? Semi_AnnualBenchmarkSpread;
        public double? AnnualBenchmarkSpreadToBMCurve;
        public double? Semi_AnnualBenchmarkSpreadToBMCurve;
        public double? AssetSwapMargin;
        public double? OAS;
        public double? ZSpread;
        public double? ZSpreadOverLibor;
        public double? DV01;
        public double? IndexRatio;
        public double? AssumedInflation;
        public string Level0;
        public string Level1;
        public string Level2;
        public string Level3;
        public string Level4;
        public string Level5;
        public string Level6;
        public string Level7;
        public string Level8;
        public string MarkitiBoxxRating;
        public string SeniorityLevel1;
        public string SeniorityLevel2;
        public string SeniorityLevel3;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsFixedtoFloat;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsPerpetual;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsHybridCapital;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool isCallable;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsCoreindex;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsCrossover;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsFRN;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsPIK;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsZeroCoupon;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool IsSinking;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_3Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _1_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _3_5Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_7Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _7_10Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _10_15Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15_20Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _20_25Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _25_30Years;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _5nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _7nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _10nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _15nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _25nPYears;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool _30nPYears;
        public double? DailyReturn;
        public double? Month_To_DateReturn;
        public double? Quarter_To_DateReturn;
        public double? Year_To_DateReturn;
        public double? DailySovereignCurveSwapReturn;
        public double? DailyLiborSwapReturn;
        public double? MonthToDateSovereignCurveSwapReturn;
        public double? MonthToDateLiborSwapReturn;
        public double? Durationweightedexposure;
    }
    #endregion

}
