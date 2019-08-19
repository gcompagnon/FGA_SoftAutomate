using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;

namespace FGABusinessComponent.BusinessComponent.Security
{
    public class InvestmentFund:Security
    {
        public InvestmentFund()
            :base(null,null,null,FinancialAssetTypeCategoryCode.OTHERS) 
        {
        }

        public InvestmentFund(String ISIN = null, string FinancialInstrumentName = null, FinancialAssetTypeCategoryCode FinancialAssetTypeCategoryCode =null)
            : base(ISIN, FinancialInstrumentName, MaturityDate: null, FinancialAssetCategory: FinancialAssetTypeCategoryCode)
        {
            DistributionPolicy = new DistributionPolicyCode();
            DividendPolicy = new DividendPolicyCode();
        }

        /// <summary>
        /// Income policy relating to a class type, ie, if income is paid out or retained in the fund.
        /// </summary>
        public DistributionPolicyCode DistributionPolicy { get; set; }

        /// <summary>
        /// Dividend policy of the fund, eg, cash, units.
        /// </summary>
        public DividendPolicyCode DividendPolicy { get; set; }



    }
    #region CONFIGURATION MAPPING

    public class InvestmentFundClassModelConfiguration : EntityTypeConfiguration<InvestmentFund>
    {
        public InvestmentFundClassModelConfiguration()
        {
            ToTable("FUND", Asset.SHEMA_NAME);
        }
    }
    #endregion
}
