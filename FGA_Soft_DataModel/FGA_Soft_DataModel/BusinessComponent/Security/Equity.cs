using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Util;

namespace FGABusinessComponent.BusinessComponent.Security
{
    /// <summary>
    /// Financial instrument which represents a title of ownership  in a company, ie,  the shareholder is entitled to a part of the company's profit - usually by payment of a dividend - and to voting rights, if any. Each company issues generally different classes of shares, eg, ordinary or common shares, which have no guaranteed amount of dividend but carry voting rights, or preferred shares, which receive dividends before ordinary shares but have no voting right.
    /// </summary>
    public class Equity:Security
    {
        public Equity()
            : base(null, null, null, FinancialAssetTypeCategoryCode.EQUITIES)
        {
        }

        public Equity(String ISIN = null, string FinancialInstrumentName = null, DateTime? MaturityDate = null)
            :base(ISIN,FinancialInstrumentName,MaturityDate,FinancialAssetTypeCategoryCode.EQUITIES)
        {            
        }
        // /// <summary>
        ///// Indicates whether the investor or the issuer has a conversion option.
        ///// </summary>
        //public Boolean ConvertibleIndicator { get; set; }

    }
    #region CONFIGURATION MAPPING

    public class EquityModelConfiguration : EntityTypeConfiguration<Equity>
    {
        public EquityModelConfiguration()
        {
            Map<Equity>(m =>
            {
                m.ToTable("EQUITY", Asset.SHEMA_NAME);
            });            
        }
    }
    #endregion


}
