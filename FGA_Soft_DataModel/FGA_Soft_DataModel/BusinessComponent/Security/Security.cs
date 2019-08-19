using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Security.Pricing;

namespace FGABusinessComponent.BusinessComponent.Security
{
    /// <summary>
    /// Financial instruments representing a sum of rights of the investor vis-a-vis the issuer.
    /// </summary>
    public class Security : Asset
    {
        public Security()
        {
            this.Pricing = new List<SecuritiesPricing>();
            this.CapitalRaised = new List<Capital>();
        }
        public Security(String ISIN = null, string FinancialInstrumentName = null, DateTime? MaturityDate = null, FinancialAssetTypeCategoryCode FinancialAssetCategory = null)
            : base(ISIN, FinancialInstrumentName, MaturityDate, FinancialAssetCategory)
        {
            if (ISIN != null)
            {
                this.ISIN = (ISINIdentifier)ISIN;
                Identification = new SecuritiesIdentification(Isin: ISIN, Name: FinancialInstrumentName);
            }
            else
            {
                this.ISIN = ISINIdentifier.DEFAULT;
                Identification = new SecuritiesIdentification(Isin: ISINIdentifier.DEFAULT_VALUE, Name: FinancialInstrumentName);
            }
            

            this.Pricing = new List<SecuritiesPricing>();
            this.CapitalRaised = new List<Capital>();
        }

        /// <summary>
        /// Surcharge de la cle primaire afin d avoir un type non string
        /// </summary>
        [NotMapped]
        public ISINIdentifier ISIN
        {
            get
            {
                if (Identification == null || Identification.SecurityIdentification.Equals(ISINIdentifier.DEFAULT))
                    return new ISINIdentifier(base.ISINId);

                return Identification.SecurityIdentification;
            }
            set
            {
                if (Identification != null)
                    Identification.SecurityIdentification = value;
                base.ISINId = value.ISINCode;
            }
        }

        /// <summary>
        /// Identification du composant (un actif financier, ou un portefeuille ou un indice ou ...)
        /// </summary>
        [ForeignKey("Identification")]
        public Int64 IdentificationId { get; set; }

        [Required]
        public virtual SecuritiesIdentification Identification { get; set; }


        #region Association Rating
        [ForeignKey("Rating")]
        public Int64? RatingId { get; set; }

        /// <summary>
        /// Rating(s) of the security.
        /// </summary>
        public virtual Rating Rating { get; set; }

        public void SetRating(Rating r)
        {
            this.Rating = r;
            r.RatedSecurity = this;
        }
        #endregion

        /// <summary>
        /// Information on the price of the security.
        /// </summary>
        #region Association Pricing
        public virtual ICollection<SecuritiesPricing> Pricing { get; set; }
        public void Add(SecuritiesPricing r)
        {
            this.Pricing.Add(r);
        }

        public void Remove(ref SecuritiesPricing r)
        {
            this.Pricing.Remove(r);
        }
        #endregion

        /// <summary>
        /// Information on capital amount associated with the sec (outstanding, redeemed ...)
        /// </summary>
        #region Association Capital
        public virtual ICollection<Capital> CapitalRaised { get; set; }
        public void Add(Capital r)
        {
            this.CapitalRaised.Add(r);
        }

        public void Remove(ref Capital r)
        {
            this.CapitalRaised.Remove(r);
        }
        #endregion


    }

    #region CONFIGURATION MAPPING

    public class SecurityModelConfiguration : EntityTypeConfiguration<Security>
    {
        public SecurityModelConfiguration()
        {
            //ToTable("ASSET", Asset.SHEMA_NAME);


            ////// la foreign key : relation  1->* SecuritiesPricing                        
            //HasMany(s => s.Pricing).WithOptional(l => l.PricedSecurity).HasForeignKey(s => s.SecurityId);

            ////// la foreign key : relation  1->0-1 rating
            //HasOptional(s => s.Ratings).WithRequired(r => r.RatedSecurity).Map(s => s.MapKey("RatingId"));

            ToTable(Asset.TABLE_NAME, Asset.SHEMA_NAME);

            // foreign key entre Rating et Security
            HasOptional(s => s.Rating).WithMany().HasForeignKey(s => s.RatingId);


            //Map<Security>(m =>
            //{

            //    m.MapInheritedProperties(); // TPC 
            //    m.ToTable("ASSET", Asset.SHEMA_NAME);
            //    //    m.ToTable("SECURITY", "bc_core");

            //});


            // la cle etrangere avec SecuritiesIdentification est obligatoire et est placee sur la table Security
            //HasRequired<SecuritiesIdentification>(i => i.Identification).WithRequiredPrincipal();

            // seulement valable pour un composant de complex type:
            //Property(x => x.Identification.Id).HasColumnName("IdentificationId");




        }
    }
    #endregion

}
