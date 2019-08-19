using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace FGABusinessComponent.BusinessComponent.Security
{

    /// <summary>
    ///  Amount of money targeted to be raised through the issuance of a security.
    /// </summary>
    public class Capital
    {
        public Capital()
        {
        }

        public Capital(DateTime? Date = null, double? Unit = null, CurrencyAndAmount Amount = null, CapitalTypeCode Type = null)
        {
            this.Date = Date;
            this.Unit = Unit;
            this.Amount = Amount;
            if (Type == null)
                this.Type = CapitalTypeCode.OUTSTANDING;
            else
                this.Type = Type;
        }
        #region Clé Primaire
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        #endregion

        /// <summary>
        /// Date/time at which capital amount was recorded.
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Capital expressed as a number of units.
        /// </summary>
        public double? Unit { get; set; }

        /// <summary>
        /// Capital expressed as a currency and amount.
        /// </summary>
        public CurrencyAndAmount Amount { get; set; }

        /// <summary>
        /// Specifies the type of capital.
        /// </summary>
        public CapitalTypeCode Type { get; set; }

        #region foreign key PricedSecurity
        [Column(Order = 1)]
        public Nullable<Int64> SecurityId { get; set; }
        public string ISINId { get; set; }
        #endregion

        /// <summary>
        /// Issued security.
        /// </summary>
        public Security SecuritiesIssuance { get; set; }
        public void Set(Security IssuedSecurity)
        {
            this.SecuritiesIssuance = IssuedSecurity;
        }

    }
    #region CONFIGURATION MAPPING

    public class CapitalModelConfiguration : EntityTypeConfiguration<Capital>
    {
        public CapitalModelConfiguration()
        {

            ToTable("SECURITIES_ISSUANCE", Asset.SHEMA_NAME);
            //reference to Security
            HasOptional(p => p.SecuritiesIssuance).WithMany(s => s.CapitalRaised).HasForeignKey(p => new { p.SecurityId, p.ISINId });

        }
    }
    #endregion
}
