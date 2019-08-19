using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FGABusinessComponent.BusinessComponent.Security;
using FGABusinessComponent.BusinessComponent.Core.Composite;
using FGABusinessComponent.BusinessComponent.Holding.IndexComp;
using FGABusinessComponent.BusinessComponent.Holding;
using FGABusinessComponent.BusinessComponent.Holding.PortfolioComp;
using FGABusinessComponent.BusinessComponent.Security.Roles;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

namespace FGABusinessComponent.Exemple
{

    /// <summary>
    /// TEST INDEX
    /// </summary>
    [TestClass()]
    public class ProgramTestIndex
    {
        private DbCompiledModel compiledModel;
        

        public ProgramTestIndex()
        {
            //
            // TODO: ajoutez ici la logique du constructeur
            //
            DbModelBuilder modelBuilder = new DbModelBuilder();
            compiledModel = modelBuilder.Build(new DbProviderInfo("System.Data.SqlClient", "2005")).Compile();
        }

        DateTime dateOfData = new DateTime(2012,12,10);

        [TestMethod()]
        public void SecurityPricingSimpleContructionTest()
        {
            Debt s;
            SecuritiesPricing price;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("TEST",compiledModel))
                {

                    s = new Debt(ISIN: "PR0000000001", FinancialInstrumentName: "Pricing Security 1", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(2.365), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    CurrencyAndAmount p = new CurrencyAndAmount(1.25, (CurrencyCode)"EUR");
                    price = new SecuritiesPricing(p, DateTime.Parse("31/07/2012"), (TypeOfPriceCode)"MARKET");
                    price.Set(s);
                    db.SecuritiesPricings.Add(price);

                    db.SaveChanges();

                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw e;
            }
        }

        [TestMethod()]
        public void SecurityPricingCompleteContructionTest()
        {
            Debt s;
            SecuritiesPricing price;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD",compiledModel))
                {

                    s = new Debt(ISIN: "PR0000000002", FinancialInstrumentName: "Pricing Security 2", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(2.365), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    CurrencyAndAmount p = new CurrencyAndAmount(1.25, (CurrencyCode)"EUR");
                    Yield yield = new Yield(ChangePrice_MTD: 2.1);
                    PriceFactType priceFact = new PriceFactType(101.2, 100.1, 100.1, 100.3, 100.4, 100.6);
                    DebtYield dy = new DebtYield(YieldToMaturityRate: 6.2, YieldToWorstRate: 3.5);
                    DebtSpread ds = new DebtSpread(OptionAdjustedSpread: 3.32);
                    DebtPriceCalculation debtPriceCalculation = new DebtPriceCalculation(CleanPrice: 100.1,
                                                                            AccruedInterest: 2.36);

                    DebtDataCalculation debtDataCalculation = new DebtDataCalculation(MacaulayDuration: 9.5,
                                                                            ModifiedDuration: 9.6,
                                                                            TimeToMaturity: 9.3);

                    price = new SecuritiesPricing(p, DateTime.Parse("31/07/2012"), (TypeOfPriceCode)"MARKET",
                        100.3, 100.4, 100.35, priceFact, yield, debtPriceCalculation, debtDataCalculation, dy, ds);
                    price.Set(s);
                    db.SecuritiesPricings.Add(price);

                    db.SaveChanges();

                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw e;
            }
        }
        [TestMethod()]
        public void AssetClassificationContructionTest()
        {
            Debt s;
            AssetClassification ac;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD",compiledModel))
                {

                    s = new Debt(ISIN: "AC0000000001", FinancialInstrumentName: "ClassificationSecurity 1", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(2.365), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    db.SaveChanges();


                    ac = new AssetClassification("EXEMPLE");
                    ac.Classification1 = "CLASSIF1";
                    ac.Classification2 = "CLASSIF2";
                    ac.Classification3 = "CLASSIF3";
                    ac.Classification4 = "CLASSIF4";

                    s.Add(ac);

                    db.SaveChanges();


                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw e;
            }

        }
        [TestMethod()]
        public void IssuerRoleContructionTest()
        {
            Debt s;
            IssuerRole role;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD", compiledModel))
                {

                    s = new Debt(ISIN: "AC0000000001", FinancialInstrumentName: "ClassificationSecurity 1", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(2.365), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    db.SaveChanges();
                    role = new IssuerRole("TOTO COMPANY", (CountryCode)"FR");
                    s.Add(role);

                    db.SaveChanges();


                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw e;
            }

        }

        [TestMethod()]
        public void SecurityLookupTest()
        {
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("TEST",performanceOverValidationFlag: true ))
                {
                    Equity result = db.Equities.Where<Equity>(t => t.Identification.SecurityIdentification.ISINCode == "AT0000730007").Include(b => b.Identification).First<Equity>();
                    // Load 
                    db.Entry<Security>(result).Reference(p => p.Identification).Load();

                    Security s = db.Securities.Find(1, "AT0000730007");
                    db.Entry<Security>(s).Reference(p => p.Identification).Load();
                    db.Entry<Security>(s).Reference("Identification").Load();

                    Assert.AreEqual("AT0000730007", result.Identification.SecurityIdentification.ISINCode);

                    Assert.AreEqual("AT0000730007", s.Identification.SecurityIdentification.ISINCode);
                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw e;
            }

        }
        
        [TestMethod()]
        public void SecurityContructionTest()
        {
            Debt s;
            Rating r;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD", compiledModel))
                {

#if DEBUG
                    // xml seeder: le fichier xml est lu et est chargé comme valeurs par défaut
                    db.SaveChanges();
                    // ecriture du fichier pour permettre d avoir un fichier
                    FGABusinessComponent.BusinessComponent.Util.EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
#endif

                    s = new Debt(ISIN: "XX0000000000", FinancialInstrumentName: "Security 1", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(2.365), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    db.SaveChanges();


                    r = new Rating();
                    r.Fitch = "AAA";
                    r.SnP = "AAA";
                    r.Moody = "AAA";
                    s.Rating = r;
                    r.RatedSecurity = s;

                    db.SaveChanges();

                    s = new Debt(ISIN: "DD1100000000", interestCoupon: new InterestCalculation(new PercentageRate(2.365), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    db.SaveChanges();


                }

            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw e;
            }

        }

        [TestMethod()]
        public void HoldingContructionTest()
        {
            Debt s;
            Equity e;
            Rating r;
            AssetHolding ia;
            Portfolio p;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD", compiledModel))
                {

#if DEBUG
                    // xml seeder: le fichier xml est lu et est chargé comme valeurs par défaut
                    db.SaveChanges();
                    // ecriture du fichier pour permettre d avoir un fichier
                    FGABusinessComponent.BusinessComponent.Util.EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
#endif

                    s = new Debt(ISIN: "XX11110000", FinancialInstrumentName: "Debt 1-Holding test", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(1.666), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    e = new Equity(ISIN: "ZZ11110000", FinancialInstrumentName: "Equity 1-Holding test");
                    db.Equities.Add(e);
                    s = new Debt(ISIN: "ZZ22220000", FinancialInstrumentName: "Debt 2-Holding test", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(1.666), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    db.SaveChanges();


                    db.Debts.Add(s);
                    db.SaveChanges();
                

                    ia = new AssetHolding(Date: dateOfData, ISIN: e.ISINId, HoldAsset: e, Quantity: 1000);
                    db.AssetHoldings.Add(ia);
                    db.SaveChanges();

                    ia = new AssetHolding(Date:dateOfData,ISIN: s.ISINId , HoldAsset: s, Quantity: 1000);
                    db.AssetHoldings.Add(ia);
                    db.SaveChanges();


                }

            }
            catch (Exception exc)
            {
                System.Console.WriteLine(exc);
                throw exc;
            }

        }

        [TestMethod()]
        public void PortfolioContructionTest()
        {
            Equity e;
            Debt s;
            InvestmentFund f;
            Rating r;
            AssetHolding ia;
            Portfolio p;
            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD", compiledModel))
                {

#if DEBUG
                    // xml seeder: le fichier xml est lu et est chargé comme valeurs par défaut
                    db.SaveChanges();
                    // ecriture du fichier pour permettre d avoir un fichier
                    FGABusinessComponent.BusinessComponent.Util.EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
#endif

                    f = new InvestmentFund(ISIN: "ZZ11110000", FinancialInstrumentName: "Equity 1-Holding test", FinancialAssetTypeCategoryCode: FinancialAssetTypeCategoryCode.DEBT);
                    db.InvestmentFunds.Add(f);
                    p = new Portfolio(ISIN:"ZZ11110000",Date: new DateTime(9999,12,31),Name:"ExempleFonds1");
                    db.Portfolios.Add(p);
                    db.SaveChanges();

                    AssetPortfolioAssociation assoc = new AssetPortfolioAssociation(p,f);

                    f.UnderlyingPortfolio = assoc;
                    assoc.InvestmentAmount = new CurrencyAndAmount();
                    assoc.InvestmentAmount.Value = 5;
                    assoc.InvestmentAmount.Currency = (CurrencyCode)"EUR";
                    db.AssetPortfolioAssociations.Add(assoc);
                    db.SaveChanges();

                    s = new Debt(ISIN: "PP11110000", FinancialInstrumentName: "Debt 1-Portfolio test", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(1.666), new FrequencyCode(1)));
                    db.Debts.Add(s);

                    ia = new AssetHolding(Date: this.dateOfData, ISIN: s.ISINId, HoldAsset: s, Holder: p, Quantity: 100);
                    db.AssetHoldings.Add(ia);
                    p.Add(ia);

                    e = new Equity(ISIN: "PP11110000", FinancialInstrumentName: "Equity 1-Portfolio test");
                    db.Equities.Add(e);
                    ia = new AssetHolding(Date:this.dateOfData,ISIN:e.ISINId, Holder: p, HoldAsset: e, Quantity: 200);
                    db.AssetHoldings.Add(ia);
                    p.Add(ia);

                    s = new Debt(ISIN: "PP22220000", FinancialInstrumentName: "Debt 2-Portfolio test", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(1.666), new FrequencyCode(1)));
                    db.Debts.Add(s);
                    ia = new AssetHolding(Date:this.dateOfData,ISIN:s.ISINId, Holder: p, HoldAsset: s, Quantity: 300);
                    db.AssetHoldings.Add(ia);
                    p.Add(ia);
                    
                    db.SaveChanges();



                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                throw ex;
            }

        }
        [TestMethod()]
        public void IndexContructionTest()
        {
            Equity e;
            Debt s;
            InvestmentFund f;
            Rating r;
            AssetHolding ia;
            Index ex;

            try
            {
                using (var db = new FGABusinessComponent.BusinessComponent.FGAContext("PREPROD", compiledModel))
                {

#if DEBUG
                    // xml seeder: le fichier xml est lu et est chargé comme valeurs par défaut
                    db.SaveChanges();
                    // ecriture du fichier pour permettre d avoir un fichier
                    FGABusinessComponent.BusinessComponent.Util.EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
#endif


                    //Security_TEST stest = new Security_TEST();
                    //stest.ISIN = (ISINIdentifier)"TEST234567890";
                    //System.Console.WriteLine(stest.ISIN);
                    ex = new Index(Name: "Exemple d indice 1", ISIN: "BTS1TREU", IndexCurrency: (CurrencyCode)"EUR");

                    ex.IndexFixingDate =  new TimeSpan(15, 30, 0); // 15h30
                    ex.IndexFrequency = FrequencyCode.getFrequencyByLabel("MONTHLY");

                    s = new Debt(ISIN: "PP11110000", FinancialInstrumentName: "Debt 1-Index test", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(1.666), new FrequencyCode(1)));
                    s.Identification = new SecuritiesIdentification(Isin: "PP11110000");
                    s.FinancialInstrumentName = "Security 1";
                    s.MaturityDate = new DateTime(2013, 1, 1);


                    ia = new AssetHolding(Date:this.dateOfData,ISIN:s.ISINId, HoldAsset: s,Holder: ex);
                    ia.MarketValue = new CurrencyAndAmount();
                    ia.MarketValue.Value = 12;
                    ia.MarketValue.Currency = (CurrencyCode)"EUR";
                    ia.FaceAmount = new CurrencyAndAmount();
                    ia.FaceAmount.Value = 5;
                    ia.FaceAmount.Currency = (CurrencyCode)"EUR";
                    ia.BookValue = new CurrencyAndAmount();
                    ia.BookValue.Value = 5;
                    ia.BookValue.Currency = (CurrencyCode)"EUR";

                    ia.Quantity = 30;


                    //SecuritiesQuantity held = new SecuritiesQuantity();

                    //held.Unit = 2;
                    //held.Rate = new PercentageRate();
                    //held.Rate.Value = 2;
                    //held.Amount = new CurrencyAndAmount();
                    //held.Amount.Currency = (CurrencyCode)"USD";
                    //held.Amount.Value = 666;
                    db.AssetHoldings.Add(ia);
                    db.Indexes.Add(ex);
                    db.SaveChanges();


                    ex = new Index(ISIN: "FR0000000000",Date: new DateTime(9999,12,31));
                    ex.IndexFixingDate = new TimeSpan(16, 30, 0); // 16h30
                    ex.IndexFrequency = FrequencyCode.getFrequencyByLabel("MONTHLY");
                    ex.IndexCurrency = (CurrencyCode)"USD";

                    db.Indexes.Add(ex);
                    db.SaveChanges();
                    // test sans ISIN (identification) 
                    s = new Debt(ISIN: "PP11110000", FinancialInstrumentName: "Debt 2-Index test", MaturityDate: new DateTime(2013, 1, 1), interestCoupon: new InterestCalculation(new PercentageRate(1.666), new FrequencyCode(1)));

                    ia = new AssetHolding(Date: this.dateOfData, ISIN:s.ISINId, Holder: ex, HoldAsset: s);

                    db.AssetHoldings.Add(ia);

                    db.SaveChanges();

                    Index ex1 = db.Indexes.FirstOrDefault<Index>();

                    Index ex2 = db.Indexes.Where<Index>(t => t.Identification.SecurityIdentification.ISINCode == "BTS1TREU").FirstOrDefault<Index>();

                    Index ex3 = db.Indexes.FirstOrDefault<Index>();

                    foreach (Component a in ex3.Items)
                    {
                        Console.Out.WriteLine(a.ToString());
                    }

                    Index ex4 = (Index)ex3;

                    List<Index> results2 = db.Indexes.Where<Index>(t => t.Identification.SecurityIdentification.ISINCode == "FR0000000000").ToList();


                    var q = from c in db.Indexes
                            where c.Identification.SecurityIdentification.ISINCode == "FR0000000000"
                            select c;
                    List<Index> results = q.ToList();

                    Console.WriteLine(results);
                    Index ex5 = db.Indexes.Find(new object[] { 3, "FR0000000000", new DateTime(9999,12,31) });

                }

            }
            catch (Exception exc)
            {
                System.Console.WriteLine(exc);
                throw exc;
            }

        }
    }
}
