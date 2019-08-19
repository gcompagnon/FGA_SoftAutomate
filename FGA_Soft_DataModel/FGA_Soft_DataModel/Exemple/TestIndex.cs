using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.IndexComp;
using FGABusinessComponent.BusinessComponent.Asset;

namespace FGABusinessComponent.Exemple
{

    /// <summary>
    /// TEST INDEX
    /// </summary>
    class ProgramTestIndex
    {

        static void Main(string[] args)
        {
            using (var db = new FGABusinessComponent.BusinessComponent.FGAContext())
            {
#if DEBUG
                FGABusinessComponent.BusinessComponent.Util.EFCodeFirstMethods.DumpDbCreationScriptToFile(db);
#endif
                IndexBasket ex = new IndexBasket();
                ex.Identification = new SecuritiesIdentification(Isin: "FR1234567890");
                ex.IndexFixingDate = new DateTime(2000, 1, 1, 14, 30, 0); // 14h30
                ex.IndexFrequency = FrequencyCode.getFrequencyByLabel("MONTHLY");
                ex.IndexCurrency = CurrencyCode.getCurrencyByLabel("Euro");
                IndexAsset ia = new IndexAsset();
                ia.MarketCapitalization = new CurrencyAndAmount();
                ia.MarketCapitalization.Value = 12;
                ia.MarketCapitalization.Currency = (CurrencyCode)"EUR";
                ia.InvestmentRate = new PercentageRate();
                ia.InvestmentRate.Value = 10;
                ia.EffectiveDate = DateTime.Now;
                ia.HeldNumber = new SecuritiesQuantity();
                ia.HeldNumber.SecurityIdentification = new ISINIdentifier();
                ia.HeldNumber.SecurityIdentification.ISINCode = "XX0000000000";
                
                ia.HeldNumber.Unit = 2;
                ia.HeldNumber.Rate = new PercentageRate();
                ia.HeldNumber.Amount = new CurrencyAndAmount();
                ia.HeldNumber.Amount.Currency = (CurrencyCode)"USD";
                ia.HeldNumber.Amount.Value = 66;
                //ia.HeldNumber = new SecuritiesQuantity();
                //ia.HeldNumber.SecurityIdentification = "10 EUR";
                ex.Compo = new List<IndexAsset>();

                ex.Compo.Add(ia);

                db.Indexes.Add(ex);

                ex = new IndexBasket();
                ex.Identification = new SecuritiesIdentification(Isin: "FR0000000000");
                ex.IndexFixingDate = new DateTime(2000, 1, 1, 14, 30, 0); // 14h30
                ex.IndexFrequency = FrequencyCode.getFrequencyByLabel("MONTHLY");
                ex.IndexCurrency = (CurrencyCode)"USD";

                db.Indexes.Add(ex);
                db.SaveChanges();

                Index ex1 = db.Indexes.FirstOrDefault<Index>();
                Index ex2 = db.Indexes.Where<Index>(t => t.Identification.SecurityIdentification == "FR1114567890").FirstOrDefault<Index>();

                List<Index> results2 = db.Indexes.Where<Index>(t => t.Identification.SecurityIdentification == "FR0000000000").ToList();


                var q = from c in db.Indexes
                        where c.Identification.SecurityIdentification == "FR0000000000"
                        select c;
                List<Index> results = q.ToList();

                Console.WriteLine(results);





            }
        }
    }
}
