using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Common;

namespace FGABusinessComponent.BusinessComponent.Security.Fx
{
    public class CurrencyExchange
    {
        public CurrencyExchange()            
        {
        }

        #region Clé Primaire
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        /// <summary>
        /// Date/time
        /// </summary>
        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }
        #endregion
        #region Main data
        /// <summary>
        /// Currency in which the rate of exchange is expressed in a currency exchange. In the example 1USD = xxx EUR, the unit currency is USD
        /// </summary>
         [Column("UnitCurrency")]
        public CurrencyCode UnitCurrency { get; set; }
        /// <summary>
        /// Currency in which the base currency is converted in a  in a currency exchange. In the example 1USD = xxx EUR, the unit currency is EUR
        /// </summary>
        [Column("QuotedCurrency")]
        public CurrencyCode QuotedCurrency { get; set; }

        [Column("FX")]
        public double? ExchangeRate { get; set; }

        /// <summary>
        /// Identifies the source valuation
        /// </summary>
        public string ValuationSource { get; set; }

        #endregion

    }
    #region CONFIGURATION MAPPING

    public class CurrencyExchangeModelConfiguration : EntityTypeConfiguration<CurrencyExchange>
    {
        public CurrencyExchangeModelConfiguration()
        {
            Map<CurrencyExchange>(fx =>
            {
                Property(x => x.UnitCurrency.Currency).HasColumnName("UnitCurrency");
                Property(x => x.QuotedCurrency.Currency).HasColumnName("QuotedCurrency");
                fx.ToTable("FX_RATE", Asset.SHEMA_NAME);
            });
        }
    }
    #endregion

}
