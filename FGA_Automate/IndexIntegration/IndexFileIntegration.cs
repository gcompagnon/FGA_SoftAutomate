using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Holding.IndexComp;
using FGABusinessComponent.BusinessComponent;
using FGABusinessComponent.BusinessComponent.Security;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using FGABusinessComponent.BusinessComponent.Security.Fx;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Holding;

namespace FGA.Automate.IndexIntegration
{
    /// <summary>
    /// API for Index of Securities (Debt or Equity)    
    /// </summary>
    /// <typeparam name="IndexLineType">Class of a Index into the file</typeparam>
    /// <typeparam name="SecurityLineType"></typeparam>
    /// <typeparam name="CurrencyLineType"></typeparam>
    public abstract class IndexFileIntegration<IndexLineType, SecurityLineType, CurrencyLineType>
    {
        #region API 
        public abstract void ExecuteIndexFileIntegration(DateTime dateStart, DateTime dateEnd, params Object[] additionalparameters);
        #endregion

        #region INDEX MNGT
        protected abstract Index LookupIndexObject(FGAContext db, IndexLineType indexLine, string ISIN =null);
        protected abstract IndexValuation AddIndexValuation(FGAContext db, Index index, IndexLineType indexLine, DateTime dateOfData, string ISIN = null);
        #endregion

        #region SEC MNGT
        protected abstract Security LookupSecurityObject(FGAContext db, SecurityLineType securityLine, string ISIN =null);
        protected abstract SecuritiesPricing AddSecurityValuation(FGAContext db, Security security, SecurityLineType securityLine, DateTime dateOfData, string ISIN = null);
        protected abstract AssetClassification AddAssetClassification(FGAContext db, Asset security, SecurityLineType securityLine, string ISIN = null);
        protected abstract Rating AddSecurityRating(FGAContext db, Security security, SecurityLineType securityLine, DateTime dateOfData, string ISIN = null);
        #endregion

        #region FOREX MNGT
        protected abstract CurrencyExchange LookupForexRateObject(FGAContext db, CurrencyLineType currencyExchangeLine, DateTime dateOfData, string ISIN = null);
        #endregion

        #region HOLDING MNGT
        protected abstract AssetHolding AddAssetHolding(FGAContext db, Index index, Asset security, IndexLineType indexLine, SecurityLineType securityLine, DateTime dateOfData, params Object[] additionalparameters);
        #endregion
    }
}
