using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Holding.IndexComp;
using FGABusinessComponent.BusinessComponent;
using FGABusinessComponent.BusinessComponent.Security;
using FGABusinessComponent.BusinessComponent.Common;
using log4net;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Holding;
using FGABusinessComponent.BusinessComponent.Security.Roles;
using FGABusinessComponent.BusinessComponent.Security.Fx;

namespace FGA.Automate.IndexIntegration
{
    class BusinessComponentHelper
    {

        public static Index LookupIndexObject(FGAContext db, string Name)
        {
            Index index = db.Indexes.Where<Index>(t => t.Name.Contains(Name)).FirstOrDefault<Index>();
            return index;
        }

        public static Index LookupIndexObjectById(FGAContext db, string Id)
        {
            Index index = null;
            index = db.Indexes.Include("Identification").Where<Index>(t => t.Identification.SecurityIdentification.ISINCode.Equals(Id)).FirstOrDefault<Index>();
            return index;
        }

        public static IndexValuation LookupIndexValuationObject(FGAContext db, string Isin, DateTime dateOfData, string ValuationSource)
        {
            IndexValuation indexV = db.Valuations.OfType<IndexValuation>().Where<IndexValuation>(t => (t.ISINId == Isin && t.Date == dateOfData && t.ValuationSource == ValuationSource)).FirstOrDefault<IndexValuation>();
            return indexV;
        }

        public static CurrencyExchange LookupCurrencyExchange(FGAContext db, CurrencyCode Unit, CurrencyCode Quoted, DateTime dateOfData)
        {
            CurrencyExchange forex;
            forex = db.CurrencyExchanges.Where<CurrencyExchange>(t => (t.Date == dateOfData && t.UnitCurrency.Currency == Unit.Currency && t.QuotedCurrency.Currency == Unit.Currency)).FirstOrDefault<CurrencyExchange>();
            return forex;
        }

        public static SecuritiesPricing LookupSecurityPricingObject(FGAContext db, string Isin, DateTime dateOfData, string ValuationSource)
        {
            SecuritiesPricing secV = db.SecuritiesPricings.Where<SecuritiesPricing>(t => (t.ISINId == Isin && t.Date == dateOfData && t.PriceSource == ValuationSource)).FirstOrDefault<SecuritiesPricing>();
            return secV;
        }

        public static SecuritiesPricing LookupSecurityPricingObject(FGAContext db, Security sec, DateTime dateOfData, string ValuationSource)
        {
            SecuritiesPricing secV = db.SecuritiesPricings.Where<SecuritiesPricing>(t => (t.SecurityId == sec.Id && t.Date == dateOfData && t.PriceSource == ValuationSource)).FirstOrDefault<SecuritiesPricing>();
            return secV;
        }

        public static Rating LookupValidRating(FGAContext db, string Isin, DateTime dateOfData, string RatingScheme)
        {
            Rating R = db.Ratings.Where<Rating>(t => (t.ISINId == Isin) && (t.RatingScheme == RatingScheme) && t.ValueDate <= dateOfData).OrderByDescending<Rating, DateTime?>(c => c.ValueDate).FirstOrDefault<Rating>();
            return R;
        }


        public static AssetClassification LookupAssetClassification(FGAContext db, Asset asset, string Classification)
        {
            AssetClassification classif = db.AssetClassifications.Where<AssetClassification>(t => (t.AssetId == asset.Id && t.Source == Classification)).FirstOrDefault<AssetClassification>();
            return classif;
        }


        public static AssetClassification LookupAssetClassification(FGAContext db, string Isin, string Classification)
        {
            AssetClassification classif = db.AssetClassifications.Where<AssetClassification>(t => (t.ISINId == Isin && t.Source == Classification)).FirstOrDefault<AssetClassification>();
            return classif;
        }

        public static AssetHolding LookupAssetHolding(FGAContext db, Index index, Asset security, DateTime dateOfData)
        {
            AssetHolding holding = db.AssetHoldings.Where<AssetHolding>(t => t.Asset.Id == security.Id && t.Parent.Id == index.Id && t.Date == dateOfData).FirstOrDefault<AssetHolding>();
            return holding;
        }

        public static Debt LookupDebt(FGAContext db, String Isin)
        {
            Debt d = db.Debts.Include("Identification").Where<Debt>(t => t.Identification.SecurityIdentification.ISINCode == Isin).FirstOrDefault<Debt>();
            return d;
        }

        public static Equity LookupEquity(FGAContext db, String Isin, CountryCode exchange)
        {
            Equity eq = db.Equities.Include("Identification").Where<Equity>(t => t.Identification.SecurityIdentification.ISINCode == Isin && t.Identification.DomesticIdentificationSource.Code2chars.Equals(exchange.Code2chars)).FirstOrDefault<Equity>();
            return eq;
        }

        public static IssuerRole LookupIssuerRole(FGAContext db, Asset asset)
        {
            IssuerRole issuerRole = db.IssuerRoles.Where<IssuerRole>(r => r.AssetId == asset.Id).FirstOrDefault<IssuerRole>();
            return issuerRole;
        }

        public static Index CreateIndexObject(FGAContext db, IDictionary<string, string> data, ILog ExceptionLogger)
        {
            Index index;
            string isin = data["isin"];
            CurrencyCode c = data["currency"] == null ? CurrencyCode.EUR : (CurrencyCode)data["currency"];

            index = new Index(Name: data["name"], ISIN: isin, IndexCurrency: c);
            index.IndexFrequency = FrequencyCode.getFrequencyByLabel("DAILY");
            index.Identification.OtherIdentification = data["id"];
            index.Identification.RIC = (RICIdentifier)data["ric"];
            index.Identification.Bloomberg = (BloombergIdentifier)data["bloomberg"];
            index.FamilyKeyObject = new MSCIFamilyObject();
            if (data["country"] != null)
            {
                try
                {
                    index.Identification.DomesticIdentificationSource = (CountryCode)data["country"];
                }
                catch (Exception e)
                {
                    ExceptionLogger.Info("Country code :" + data["country"] + " Not recognized");
                }
            }
            db.Indexes.Add(index);
            return index;
        }

    }
}
