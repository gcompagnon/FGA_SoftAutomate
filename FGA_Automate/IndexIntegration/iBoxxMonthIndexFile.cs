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
    /// <summary>
    /// Description résumée pour iBoxxMonthIndexFile:
    /// 
    /// TODO// TODO  A TESTER
    /// 
    /// Integration des indices en Monthly :
    /// en Monthly:
    ///  IBOXX_EOM_COMPONENTS
    ///  IBOXX_EOM_XREF  (poids)
    /// </summary>
    public class iBoxxMonthIndexFile : IndexFileIntegration<EnDOfDayIndices, EnDOfDayUnderlyings, Object>
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
        public iBoxxMonthIndexFile(string env = "PREPROD")
        {
            iBoxxMonthIndexFile.ENVIRONNEMENT = env;
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
        protected override CurrencyExchange LookupForexRateObject(FGAContext db, object currencyExchangeLine, DateTime date,string ISIN = null)
        {
            throw new NotImplementedException();
        }


        #region API

        /// <summary>
        /// les 3 fichiers pour 1 mois en entier
        /// </summary>
        public void TestMarkitIboxxMonthlyAllMonthETL()
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
            string filePath;
            if (argumentFilesPath != null && argumentFilesPath.Length > 1 && argumentFilesPath[0] is string)
            {
                filePath = argumentFilesPath[0] as string;
            }
            else
            {
                filePath = INDEX_PATH;
            }

                        for (DateTime dateOfData = dateStart; dateOfData <= dateEnd; dateOfData = dateOfData.AddDays(1))
                        {


                            List<EnDOfMonthComponents> dailyReturns = null;
                            using (System.IO.StreamReader file = new StreamReader(filePath))
                            {
                                string content = file.ReadToEnd();
                                file.BaseStream.Position = 0;
                                EtlFullResult o = Input
                                    .ReadFile<EnDOfMonthComponents>(file)
                                    .Execute();
                                Type t = typeof(EnDOfMonthComponents);
                                // recuperation des objets dans une liste
                                dailyReturns = analyse<EnDOfMonthComponents>(o, t);
                            }
                            FGAContext db = getFGAContext;
                            {
                                // desactiver la gestion des changes auto
                                db.Configuration.AutoDetectChangesEnabled = false;
                                db.Configuration.ValidateOnSaveEnabled = false;
                                db.Configuration.LazyLoadingEnabled = true;

                                // chercher l objet pour l indice Iboxx Overall Total return
                                // lookup for the indice object
                                Index index = lookupIndex(db, BBTicker: "QW7A");
                                if (index == null)
                                {
                                    index = new Index(Name: "iBoxx € Overall - TRI", ISIN: "DE0009682716", IndexCurrency: CurrencyCode.EUR);
                                    index.IndexFrequency = FrequencyCode.getFrequencyByLabel("DAILY");
                                    index.Identification.Bloomberg = (BloombergIdentifier)"QW7A";
                                    db.Indexes.Add(index);
                                    db.SaveChanges();
                                }
                                int compteur = 0;
                                foreach (EnDOfMonthComponents bond in dailyReturns)
                                {
                                    compteur++;
                                    Debt d;
                                    AssetHolding holding;
                                    try
                                    {
                                        // lookup for the isin bond
                                        List<Debt> results = db.Debts.Where<Debt>(t => t.Identification.SecurityIdentification.ISINCode == bond.ISIN).ToList();
                                        if (results == null || results.Count == 0)
                                        {
                                            d = new Debt(bond.ISIN);
                                            db.Debts.Add(d);
                                            // les donnees statiques sont remplies plus tard
                                        }
                                        else
                                        {
                                            d = results.First<Debt>();
                                        }

                                        // si les donnees statiques ne sont pas remplies
                                        if (!d.isStaticDataValid())
                                        {
                                            d.Identification.Bloomberg = (BloombergIdentifier)bond.Ticker;
                                            CountryCode issuerCountry = CountryCode.getCountryByLabel(bond.IssuerCountry);
                                            d.Add(new IssuerRole(bond.Issuer, issuerCountry));
                                            d.FinancialInstrumentName = bond.Ticker + ' ' + bond.Coupon + ' ' + bond.WorkoutDate.ToShortDateString();
                                            d.MaturityDate = bond.WorkoutDate;
                                            d.FirstPaymentDate = bond.FirstCouponDate;
                                            d.NextCallableDate = bond.NextCallDate;

                                            InterestCalculation interest = new InterestCalculation(new PercentageRate(bond.Coupon), (FrequencyCode)(bond.CouponFrequency), InterestComputationMethodCode.getCapitalTypeCodeByLabel(bond.DayCountMethod));
                                            d.NextInterest = interest;

                                            //Capital amountIssued = new Capital(Date: dateOfData, Amount: new CurrencyAndAmount(value: , currency: (CurrencyCode)bond.Level0), Type: CapitalTypeCode.OUTSTANDING);
                                            //d.Add(amountIssued);


                                            db.SaveChanges();
                                        }

                                        List<SecuritiesPricing> prices;


                                        // le prix à fin du mois                    
                                        CurrencyAndAmount price = new CurrencyAndAmount(bond.IndexPrice, (CurrencyCode)bond.Level0);
                                        DebtYield dy = new DebtYield(YieldToMaturityRate: bond.AnnualYield);
                                        DebtSpread ds = new DebtSpread(OptionAdjustedSpread: bond.OAS, ZeroVolatilitySpread: bond.ZSpread, GovSpread: bond.AnnualBenchmarkSpread);
                                        DebtDataCalculation debtDataCalculation = new DebtDataCalculation(MacaulayDuration: bond.Duration,
                                                                                                ModifiedDuration: bond.AnnualModifiedDuration,
                                                                                                TimeToMaturity: bond.ExpectedRemainingLife);
                                        DebtPriceCalculation debtPriceCalculation = new DebtPriceCalculation(CleanPrice: bond.IndexPrice, DirtyPrice: bond.IndexPrice + bond.AccruedInterest,
                                                                            AccruedInterest: bond.AccruedInterest);

                                        // chercher avec la date, l objet Price
                                        prices = db.SecuritiesPricings.Where<SecuritiesPricing>(t => (t.ISINId == bond.ISIN && t.Date == dateOfData)).ToList();
                                        if (prices == null || prices.Count == 0)
                                        {
                                            //PriceFactType priceFactType = new PriceFactType();
                                            SecuritiesPricing p = new SecuritiesPricing(price, dateOfData, (TypeOfPriceCode)"CONSOLIDATED",
                                                AskPrice: bond.AskPrice, BidPrice: bond.BidPrice,
                                            DebtDataCalculation: debtDataCalculation, DebtPriceCalculation: debtPriceCalculation,
                                            DebtYield: dy, DebtSpread: ds);
                                            p.Set(d);
                                            d.Pricing.Add(p);
                                            db.SecuritiesPricings.Add(p);
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            SecuritiesPricing p = prices.First<SecuritiesPricing>();
                                            p.Price = price;
                                            p.PriceType = (TypeOfPriceCode)"CONSOLIDATED";
                                            p.AskPrice = bond.AskPrice;
                                            p.BidPrice = bond.BidPrice;
                                            p.DebtDataCalculation = debtDataCalculation;
                                            p.DebtPriceCalculation = debtPriceCalculation;
                                            p.DebtYield = dy;
                                            p.DebtSpread = ds;
                                        }

                                        // chercher le rating
                                        Rating r = new Rating(Value: bond.MarkitiBoxxRating, ValueDate: dateOfData, RatingScheme: "IBOXX_EUR");

                                        List<Rating> ratings = db.Ratings.Where<Rating>(t => (t.ISINId == bond.ISIN)).ToList();

                                        if (ratings != null && ratings.Count > 0)
                                        {
                                            Rating existingR = ratings.First<Rating>();
                                            // si les ratings ont changé: supprimer le rating existant (archivage)
                                            if (!existingR.Equals(r))
                                            {
                                                // mark le rating pour suppression
                                                db.Ratings.Remove(existingR);
                                                db.SaveChanges();

                                                d.SetRating(r);
                                                db.Ratings.Add(r);
                                            }
                                        }
                                        else
                                        {
                                            d.SetRating(r);
                                            db.Ratings.Add(r);
                                        }

                                        db.SaveChanges();
                                        // chercher avec la classif
                                        List<AssetClassification> classifs = db.AssetClassifications.Where<AssetClassification>(t => (t.ISINId == bond.ISIN && t.Source == "IBOXX_EUR")).ToList();
                                        AssetClassification classif;
                                        if (classifs == null || classifs.Count == 0)
                                        {
                                            classif = new AssetClassification("IBOXX_EUR");
                                            d.Add(classif);
                                            classif.Asset = d;
                                            db.AssetClassifications.Add(classif);
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            classif = classifs.First<AssetClassification>();
                                        }

                                        classif.Classification1 = (bond.Level0.Equals("*") ? null : bond.Level0);
                                        classif.Classification2 = (bond.Level1.Equals("*") ? null : bond.Level1);
                                        classif.Classification3 = (bond.Level2.Equals("*") ? null : bond.Level2);
                                        classif.Classification4 = (bond.Level3.Equals("*") ? null : bond.Level3);
                                        classif.Classification5 = (bond.Level4.Equals("*") ? null : bond.Level4);
                                        classif.Classification6 = (bond.Level5.Equals("*") ? null : bond.Level5);
                                        classif.Classification7 = (bond.Level6.Equals("*") ? null : bond.Level6);


                                        // niveau de seniorité: prendre le niveau le plus détaillé en premier
                                        SeniorityLevelCode senLevel = null;
                                        if (bond.SeniorityLevel3 == null || bond.SeniorityLevel3.Equals("*"))
                                        {
                                            if (bond.SeniorityLevel2 == null || bond.SeniorityLevel2.Equals("*"))
                                            {
                                                if (bond.SeniorityLevel1 == null || bond.SeniorityLevel1.Equals("*") || bond.SeniorityLevel1.Equals("NW"))
                                                {
                                                    // Rien a faire. pas de seniority ou alors en NeW
                                                }
                                                else
                                                {
                                                    senLevel = new SeniorityLevelCode(bond.SeniorityLevel1);
                                                }
                                            }
                                            else
                                            {
                                                senLevel = new SeniorityLevelCode(bond.SeniorityLevel2);
                                            }
                                        }
                                        else
                                        {
                                            senLevel = new SeniorityLevelCode(bond.SeniorityLevel3);
                                        }

                                        if (senLevel != null)
                                        {
                                            d.FinancialInfos.Seniority = senLevel;
                                        }

                                        d.PerpetualIndicator = bond.IsPerpetual;
                                        d.FixedToVariableIndicator = bond.IsFixedtoFloat;
                                        d.FinancialInfos.HybridCapital = bond.IsHybridCapital;

                                        db.SaveChanges();
                                        // pondération sur l indice
                                        List<AssetHolding> holdings = db.AssetHoldings.Where<AssetHolding>(t => t.Asset.Id == d.Id && t.Parent.Id == index.Id && t.Date == dateOfData).ToList();
                                        if (holdings == null || holdings.Count == 0)
                                        {
                                            holding = new AssetHolding(Date: dateOfData, ISIN: d.ISIN.ISINCode,
                                            HoldAsset: d,
                                            Holder: index,
                                                //Notional // Principal // Par Value * quantity
                                            FaceAmount: new CurrencyAndAmount(bond.NotionalAmount, CurrencyCode.EUR),
                                                // calcul de la quantité
                                            Quantity: (float)bond.NotionalAmount / d.MinimumIncrement.Unit,
                                            MarketValue: new CurrencyAndAmount(bond.BaseMarketValue, CurrencyCode.EUR),
                                            Weight: new PercentageRate(bond.IndexWeight));
                                            db.AssetHoldings.Add(holding);
                                            db.SaveChanges();
                                            index.Add(holding);




                                        }
                                        else
                                        {
                                            holding = holdings.First<AssetHolding>();
                                            holding.FaceAmount = new CurrencyAndAmount(bond.NotionalAmount, CurrencyCode.EUR);
                                            holding.MarketValue = new CurrencyAndAmount(bond.BaseMarketValue, CurrencyCode.EUR);
                                            holding.Weight = new PercentageRate(bond.IndexWeight);
                                        }

                                        db.SaveChanges();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Probleme avec l integration du titre " + bond.ISIN);
                                        throw e;
                                    }
                                }
                            }
                        }
        }

        /// <summary>
        ///  Integration des fichiers Markit IBOXX
        ///  sans recreer les donnees en statiques
        /// </summary>
        /// <param name="dateOfData"></param>
        /// <param name="filePath"></param>
        private void IntegrationMarkitIBoxxIndicesMonthlyXrefsRerunETL(DateTime dateOfData, string filePath)
        {

            List<EnDOfMonthXref> dailyReturns = null;
            using (System.IO.StreamReader file = new StreamReader(filePath))
            {
                string content = file.ReadToEnd();
                file.BaseStream.Position = 0;
                EtlFullResult o = Input
                    .ReadFile<EnDOfMonthXref>(file)
                    .Execute();
                Type t = typeof(EnDOfMonthXref);
                // recuperation des objets dans une liste
                dailyReturns = analyse<EnDOfMonthXref>(o, t);
            }
            FGAContext db = getFGAContext;
            {
                // desactiver la gestion des changes auto
                db.Configuration.AutoDetectChangesEnabled = false;
                db.Configuration.ValidateOnSaveEnabled = false;
                // optimisation du chargement des indices (et de leurs composantes, et valorisation)
                db.Configuration.LazyLoadingEnabled = false;

                int compteur = 0;
                Debt d;
                Index index;

                foreach (EnDOfMonthXref indexLine in dailyReturns)
                {
                    try
                    {
                        compteur++;
                        bool toSave = false;

                        // chercher l objet pour l indice concerné par le holding
                        // lookup for the indice object
                        index = lookupIndex(db, ISIN: indexLine.ISIN_Tri, BBTicker: indexLine.Code_Tri);
                        if (index == null)
                        {
                            index = new Index(Name: "iBoxx € " + indexLine.ISIN_Tri + " - TRI",
                                                ISIN: indexLine.ISIN_Tri, IndexCurrency: CurrencyCode.EUR);
                            index.IndexFrequency = FrequencyCode.getFrequencyByLabel("DAILY");
                            index.Identification.Bloomberg = (BloombergIdentifier)indexLine.Code_Tri;
                            db.Indexes.Add(index);
                            toSave = true;
                        }

                        // lookup de the debt
                        List<Debt> results = db.Debts.Where<Debt>(t => t.Identification.SecurityIdentification.ISINCode == indexLine.ComponentISIN).ToList();
                        if (results == null || results.Count == 0)
                        {
                            // erreur car les composants doivent etre allimenter
                            d = new Debt(indexLine.ComponentISIN);

                            Capital amountIssued = new Capital(Date: dateOfData, Amount: new CurrencyAndAmount(value: indexLine.NotionalAmount, currency: CurrencyCode.EUR), Type: CapitalTypeCode.OUTSTANDING);
                            d.Add(amountIssued);

                            db.Debts.Add(d);
                            toSave = true;
                        }
                        else
                        {
                            d = results.First<Debt>();
                        }

                        if (toSave)
                        {
                            db.SaveChanges();
                            toSave = false;
                        }

                        // pondération sur l indice
                        AssetHolding holding;
                        List<AssetHolding> holdings = db.AssetHoldings.Where<AssetHolding>(t => t.Asset.Id == d.Id && t.Parent.Id == index.Id && t.Date == dateOfData).ToList();
                        if (holdings == null || holdings.Count == 0)
                        {
                            holding = new AssetHolding(Date: dateOfData, ISIN: d.ISIN.ISINCode,
                            HoldAsset: d,
                            Holder: index,
                                //Notional // Principal // Par Value * quantity
                                FaceAmount: new CurrencyAndAmount(indexLine.NotionalAmount, CurrencyCode.EUR),
                                // calcul de la quantité
                                Quantity: (float)indexLine.NotionalAmount / d.MinimumIncrement.Unit,
                                Weight: new PercentageRate(indexLine.IndexWeight));
                            db.AssetHoldings.Add(holding);
                            db.SaveChanges();
                            index.Add(holding);
                        }
                        else
                        {
                            holding = holdings.First<AssetHolding>();
                            holding.FaceAmount = new CurrencyAndAmount(indexLine.NotionalAmount, CurrencyCode.EUR);
                            holding.Weight = new PercentageRate(indexLine.IndexWeight);
                        }


                        if (compteur % 100000 == 0 || compteur == dailyReturns.Count)
                        {
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Probleme avec l integration du titre " + indexLine.ComponentISIN + " (ligne " + compteur + "/" + dailyReturns.Count + ") dans l indice " + indexLine.ISIN_Tri);
                        throw e;
                    }

                }
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
            List<Index> results = db.Indexes.Include("Identification").Where<Index>(t => t.Identification.SecurityIdentification.ISINCode == indexLine.ISIN_Tri).ToList();
            if (results == null || results.Count == 0)
            {
                //Indice à créer
                index = new Index(Name: name, ISIN: indexLine.ISIN_Tri, IndexCurrency: (CurrencyCode)indexLine.Level0);                
                db.Indexes.Add(index);
            }
            else
            {
                index = results.First<Index>();
                //// specify modification
                db.Entry(index).Property(i => i.FamilyKey).IsModified = true;
            }

            if (!index.isStaticDataValid())
            {
                index.Name = name;
                index.IndexFrequency = FrequencyCode.getFrequencyByLabel("DAILY");
                index.Identification.Bloomberg = (BloombergIdentifier)indexLine.Code_Tri;
            }
            IBOXXFamilyObject f = new IBOXXFamilyObject(indexLine);
            index.FamilyKeyObject = f;

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
                    BaseDate: new DateTime(dateOfData.Year,dateOfData.Month,1) ,
                    FaceAmount: new CurrencyAndAmount(indexLine.NominalValue, CurrencyCode.EUR),
                    BookValue: new CurrencyAndAmount(indexLine.BaseMarketValue, CurrencyCode.EUR),
                    MarketValue: new CurrencyAndAmount(indexLine.MarketValue, CurrencyCode.EUR),
                    IndexPriceValue: indexLine.Cpi_Today,
                    IndexGrossValue: indexLine.GrossPriceIndex,
                    IndexNetValue: indexLine.Tri_Today);
                valuation.IndexNumberOfSecurities = (int)indexLine.NumberOfBonds;
                valuation.ValuationSource = "IBOXX_EUR";
                db.Valuations.Add(valuation);
                db.SaveChanges();
                index.Add(valuation);
            }
            else
            {
                valuation.FaceAmount = new CurrencyAndAmount(indexLine.NominalValue, CurrencyCode.EUR);
                // valeur au rebalancement
                valuation.IndexBaseValue = indexLine.BaseMarketValue;
                valuation.IndexBaseDate = new DateTime(dateOfData.Year,dateOfData.Month,1) ;
                valuation.BookValue = new CurrencyAndAmount(indexLine.BaseMarketValue, CurrencyCode.EUR);
                valuation.MarketValue = new CurrencyAndAmount(indexLine.MarketValue, CurrencyCode.EUR);
                valuation.IndexPriceValue = indexLine.Cpi_Today;
                valuation.IndexGrossValue = indexLine.GrossPriceIndex;
                valuation.IndexNetValue = indexLine.Tri_Today;
                valuation.IndexNumberOfSecurities = indexLine.NumberOfBonds;
                valuation.ValuationSource = "IBOXX_EUR";
            }
            valuation.Yield.ChangePrice_MTD = new PercentageRate(indexLine.MonthToDateReturn);
            valuation.Yield.ChangePrice_1D = new PercentageRate(indexLine.DailyReturn);
            valuation.Yield.ChangePrice_YTD = new PercentageRate(indexLine.Annual_Yield);
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

            Dictionary<string, Index> r = new Dictionary<string, Index>(capacity: mainIndexes.Length);
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

            Dictionary<string, IndexValuation> r = new Dictionary<string, IndexValuation>( capacity: mainIndexes.Length);
            IndexValuation indexV;
            foreach (string id in mainIndexes)
            {                
                indexV = BusinessComponentHelper.LookupIndexValuationObject(db,id,date,"IBOXX_EUR");
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
            using (System.IO.StreamReader file = new StreamReader(filePath,Encoding.GetEncoding(1252)))
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
                if (compteur++ % 100000 == 0)
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
            List<Debt> results = db.Debts.Include("Identification").Where<Debt>(t => t.Identification.SecurityIdentification.ISINCode == bond.ISIN).ToList();
            if (results == null || results.Count == 0)
            {
                d = new Debt(bond.ISIN);
                db.Debts.Add(d);
                // les donnees statiques sont remplies plus tard                
            }
            else
            {
                d = results.First<Debt>();
            }

            // si les donnees statiques ne sont pas remplies
            if (!d.isStaticDataValid())
            {
                DateTime mat = bond.FinalMaturity ?? DateTime.Now;
                d.Identification = new SecuritiesIdentification(Isin: bond.ISIN, Ticker: bond.Ticker);
                d.FinancialInstrumentName = bond.Issuer + ' ' + bond.Coupon + ' ' + mat.ToShortDateString();
                d.MaturityDate = bond.FinalMaturity;
                InterestCalculation interest = new InterestCalculation(new PercentageRate(bond.Coupon), (FrequencyCode)(bond.CouponFrequency));
                d.NextInterest = interest;
            }

            return d;
        }

        protected override AssetClassification AddAssetClassification(FGAContext db, Asset security, EnDOfDayUnderlyings bondLine, string ISIN = null)
        {
            Debt bond = (Debt)security;

            // chercher avec la classif
            List<AssetClassification> classifs = db.AssetClassifications.Where<AssetClassification>(t => (t.ISINId == bondLine.ISIN && t.Source == "IBOXX_EUR")).ToList();
            AssetClassification classif;
            if (classifs == null || classifs.Count == 0)
            {
                classif = new AssetClassification("IBOXX_EUR");
                bond.Add(classif);
                classif.Asset = bond;
                db.AssetClassifications.Add(classif);
                db.SaveChanges();
            }
            else
            {
                classif = classifs.First<AssetClassification>();
            }

            classif.Classification1 = (bondLine.Level0.Equals("*") ? null : bondLine.Level0);
            classif.Classification2 = (bondLine.Level1.Equals("*") ? null : bondLine.Level1);
            classif.Classification3 = (bondLine.Level2.Equals("*") ? null : bondLine.Level2);
            classif.Classification4 = (bondLine.Level3.Equals("*") ? null : bondLine.Level3);
            classif.Classification5 = (bondLine.Level4.Equals("*") ? null : bondLine.Level4);
            db.Entry<AssetClassification>(classif).State = EntityState.Modified;

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

            if (senLevel != null)
            {
                bond.FinancialInfos.Seniority = senLevel;
            }

            bond.PerpetualIndicator = bondLine.IsPerpetual;
            bond.FixedToVariableIndicator = bondLine.IsFixedtoFloat;
            bond.FinancialInfos.HybridCapital = bondLine.IsHybridCapital;

            //db.Entry(bond).Property<FinancialInfos>(fi => bond.FinancialInfos).IsModified = true;
            return classif;
        }

        protected override Rating AddSecurityRating(FGAContext db, Security security, EnDOfDayUnderlyings bond, DateTime dateOfData, string ISIN = null)
        {
            Debt d = (Debt)security;
            // chercher le rating
            Rating r = new Rating(Value: bond.MarkitiBoxxRating, ValueDate: dateOfData, RatingScheme: "IBOXX_EUR");

            List<Rating> ratings = db.Ratings.Where<Rating>(t => (t.ISINId == bond.ISIN)).ToList();

            if (ratings != null && ratings.Count > 0)
            {
                Rating existingR = ratings.First<Rating>();
                // si les ratings ont changé: supprimer le rating existant (archivage)
                if ((!existingR.Equals(r))&&(existingR.ValueDate < dateOfData))
                {
                    // mark le rating pour suppression
                    db.Ratings.Remove(existingR);
                    db.SaveChanges();

                    d.SetRating(r);
                    db.Ratings.Add(r);
                    db.SaveChanges();
                }
            }
            else
            {
                d.SetRating(r);
                db.Ratings.Add(r);
                db.SaveChanges();
                //db.Entry<Rating>(r).State = EntityState.Modified;
            }
            return r;
        }


        protected override SecuritiesPricing AddSecurityValuation(FGAContext db, Security debt, EnDOfDayUnderlyings bond, DateTime dateOfData, string ISIN = null)
        {
            //Prix et performance
            List<SecuritiesPricing> prices;

            CurrencyAndAmount price = new CurrencyAndAmount(bond.DirtyPrice - bond.AccruedInterest, CurrencyCode.EUR);
            DebtYield dy = new DebtYield(YieldToMaturityRate: bond.AnnualYield);
            DebtSpread ds = new DebtSpread(GovSpread: bond.AnnualBenchmarkSpreadToBMCurve, ZeroVolatilitySpread: bond.ZSpread, OptionAdjustedSpread: bond.OAS, AssetSwapSpread: bond.AssetSwapMargin);

            Yield yield = new Yield(ChangePrice_1D: bond.DailyReturn, ChangePrice_MTD: bond.Month_To_DateReturn);

            // Calcul de la Duration:
            DebtDataCalculation debtDataCalculation = new DebtDataCalculation(
                // Modified Duration based on ISMA convention, which is annual coupon convention. European bonds typically are based on an annual
                                                                    ModifiedDuration: bond.AnnualModifiedDuration,
                                                                    MacaulayDuration: bond.Duration,
                // Based on semi-annual coupon convention           
                                                                    ModifiedDurationSemiAnnual: bond.Semi_AnnualModifiedDuration,
                                                                    TimeToMaturity: bond.ExpectedRemainingLife,

                                                                    Convexity: bond.AnnualConvexity, 
                                                                    ConvexitySemiAnnual: bond.Semi_AnnualConvexity
                                                                    );

            DebtPriceCalculation debtPriceCalculation = new DebtPriceCalculation(DirtyPrice: bond.DirtyPrice, CleanPrice: bond.DirtyPrice - bond.AccruedInterest,
                                                AccruedInterest: bond.AccruedInterest);            

            // chercher avec la date, l objet Price
            prices = db.SecuritiesPricings.Where<SecuritiesPricing>(t => (t.ISINId == bond.ISIN && t.Date == dateOfData)).ToList();
            SecuritiesPricing p;
            if (prices == null || prices.Count == 0)
            {
                //PriceFactType priceFactType = new PriceFactType();
                p = new SecuritiesPricing(price, dateOfData, (TypeOfPriceCode)"MARKET", Yield: yield,
                DebtDataCalculation: debtDataCalculation, DebtPriceCalculation: debtPriceCalculation,
                DebtYield: dy, DebtSpread: ds);
                p.PriceSource = "IBOXX_EUR";
                p.Set(debt);
                debt.Pricing.Add(p);
                db.SecuritiesPricings.Add(p);
                db.SaveChanges();
            }
            else
            {
                p = prices.First<SecuritiesPricing>();
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
            IDictionary<string, IndexValuation> mainIndexesValuation = lookupMainIndexesValuations(db,dateOfData); 
            
            foreach (EnDOfDayUnderlyings bond in dailyReturns)
            {
                Debt d = (Debt)this.LookupSecurityObject(db, bond);
                SecuritiesPricing p = this.AddSecurityValuation(db, d, bond, dateOfData);
                AssetClassification ac = this.AddAssetClassification(db, d, bond);
                Rating r = this.AddSecurityRating(db, d, bond, dateOfData);
                //db.Entry<Debt>(d).State = EntityState.Modified;                
                this.AddAssetHolding(db, mainIndexes["DE0009682716"], d, null, bond, dateOfData, mainIndexesValuation["DE0009682716"]);
            }
        }


        protected override AssetHolding AddAssetHolding(FGAContext db, Index index, Asset security, EnDOfDayIndices indexLine, EnDOfDayUnderlyings securityLine, DateTime dateOfData, params Object[] additionalparameters)
        {
            IndexValuation indexValuation = null;
            if (additionalparameters != null && additionalparameters.Length > 1 && additionalparameters[0] is string)
            {
                indexValuation = additionalparameters[0] as IndexValuation;
            }

            AssetHolding holding;
            List<AssetHolding> holdings = db.AssetHoldings.Where<AssetHolding>(t => t.Asset.Id == security.Id && t.Parent.Id == index.Id && t.Date == dateOfData).ToList();

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
            else if( indexLine !=null)
            {
                indexMV = indexLine.MarketValue;
            }

            if (holdings == null || holdings.Count == 0)
            {
                holding = new AssetHolding(Date: dateOfData, ISIN: securityLine.ISIN,
                HoldAsset: security,
                Holder: index,
                    //Notional // Principal // Par Value * quantity
                    FaceAmount: new CurrencyAndAmount(securityLine.NotionalAmount, CurrencyCode.EUR),
                    MarketValue: new CurrencyAndAmount(securityLine.MarketValue, CurrencyCode.EUR),
                    Weight: indexMV == null ? null : new PercentageRate(100 * securityLine.MarketValue / indexMV),
                    Quantity: 1);

                db.AssetHoldings.Add(holding);
                db.SaveChanges();
                index.Add(holding);
            }
            else
            {
                holding = holdings.First<AssetHolding>();
                holding.ISIN = (ISINIdentifier)securityLine.ISIN;
                holding.MarketValue = new CurrencyAndAmount(securityLine.MarketValue, CurrencyCode.EUR);                
                holding.FaceAmount= new CurrencyAndAmount(securityLine.NotionalAmount, CurrencyCode.EUR);
                holding.Weight = indexMV == null ? new PercentageRate() : new PercentageRate(100 * securityLine.MarketValue / indexMV);
                holding.Quantity = 1;
                db.Entry<AssetHolding>(holding).State = EntityState.Modified;
            }
            IBOXXFamilyObject f = new IBOXXFamilyObject(securityLine);
            if (!f.Equals(holding.FamilyKey))
            {
                holding.FamilyKeyObject = f;
                db.Entry(holding).State = EntityState.Modified;
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
                List<Index> resultsIndexes = db.Indexes.Where<Index>(t => t.Identification.SecurityIdentification.ISINCode == ISIN).ToList();
                if (resultsIndexes != null && resultsIndexes.Count > 0)
                {
                    foundIndex = resultsIndexes.First<Index>();
                    IndexesCache.Add(ISIN, foundIndex);
                }
            }
            else if (BBTicker != null)
            {
                List<Index> resultsIndexes = db.Indexes.Where<Index>(t => t.Identification.Bloomberg.BBCode == BBTicker).ToList();
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
    /// Format du fichier Markit Iboxx EOM_Components
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class EnDOfMonthComponents
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public string ISIN;
        public string Ticker;
        public string Issuer;
        public string IssuerCountry;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? FirstSettlementDate;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? InterestAccrualDate;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? FirstCouponDate;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime? FinalMaturity;
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime WorkoutDate;
        public double? ExpectedRemainingLife;
        public double? Coupon;
        public int? CouponFrequency;
        public string DayCountMethod;
        public double? NotionalAmount;
        public double? BidPrice;
        public double? AskPrice;
        public double? IndexPrice;
        public double? AccruedInterest;
        [FieldConverter(ConverterKind.Boolean, "1", "0")]
        public bool? Ex_Dividend; // 1 si le bond est entré ds l indice dans l ex dividende date : eligible pour le paiement du coupon
        public double? CouponAdjustment;
        public double? BaseMarketValue;  // Market Value au rebalancement de fin de mois
        public string Level0;
        public string Level1;
        public string Level2;
        public string Level3;
        public string Level4;
        public string Level5;
        public string Level6;
        public string MarkitiBoxxRating;
        public string SeniorityLevel1;
        public string SeniorityLevel2;
        public string SeniorityLevel3;
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
        public double? IndexWeight;
        public string BenchmarkISIN;
        public double? AnnualBenchmarkSpread;
        public double? AssetSwapMargin;
        public double? Semi_AnnualBenchmarkSpread;
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
    /// <summary>
    /// Format du fichier Markit Iboxx EOM_XREF : repartition des titres dans chacun des indices
    /// </summary>
    [DelimitedRecord(","), IgnoreFirst(1), IgnoreEmptyLines(true)]
    public class EnDOfMonthXref
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Date;
        public string ISIN_Cpi;
        public string ISIN_Tri;
        public string Code_Cpi;
        public string Code_Tri;
        public string ComponentISIN;
        public double? NotionalAmount;
        public double? IndexWeight;
    }

    #endregion


}
