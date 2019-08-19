using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.Holding.PortfolioComp;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Security
{
    /// <summary>
    /// Association optionnel entre Portfolio et Asset :
    /// un fond est un actif
    /// </summary>
    #region Association: Portfolio
    public class AssetPortfolioAssociation
    {
        public AssetPortfolioAssociation()
        {
            this.InvestmentAmount = new CurrencyAndAmount();
            this.InvestmentRate = new PercentageRate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="Asset"></param>
        public AssetPortfolioAssociation(Portfolio p, Asset PtfAsAsset)
        {            
            this.InvestmentAmount = new CurrencyAndAmount();
            this.InvestmentRate = new PercentageRate();
            this.Portfolio = p;
            p.Asset = this;
            this.Asset = PtfAsAsset;
            PtfAsAsset.UnderlyingPortfolio = this;
        }


        #region Clé Primaire
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        #endregion

        /// <summary>
        /// return the value of the '<em>Portfolio</em>' reference.
        /// </summary>    
        #region foreign key
        public virtual Portfolio Portfolio { get; set; }
        public virtual Asset Asset { get; set; }
        #endregion

        /// <summary>
        /// Invested amount of the portfolio asset.
        /// </summary>
        public CurrencyAndAmount InvestmentAmount { get; set; }
        /// <summary>
        /// Invested percentage of the portfolio asset.
        /// </summary>
        public PercentageRate InvestmentRate { get; set; }
        /// <summary>
        /// Cut off date/time for the information of the specified portfolio asset. 
        /// </summary>
        public DateTime? EffectiveDate { get; set; }
    }

    #endregion

    #region CONFIGURATION MAPPING

    public class AssetPortfolioAssociationModelConfiguration : EntityTypeConfiguration<AssetPortfolioAssociation>
    {
        public AssetPortfolioAssociationModelConfiguration()
        {
            Map<AssetPortfolioAssociation>(m =>
            {
                Property(x => x.InvestmentAmount.Currency.Currency).HasColumnName("InvestmentAmount_Cur");
                Property(x => x.InvestmentAmount.Value).HasColumnName("InvestmentAmount");

                Property(x => x.InvestmentRate.Value).HasColumnName("InvestmentRate");
                m.ToTable("ASSET_PORTFOLIO", Asset.SHEMA_NAME);
            });
            //// la foreign key : relation AssetHolding 1->* Asset
            this.HasOptional(ap => ap.Asset).WithOptionalDependent(a => a.UnderlyingPortfolio);

            // relation for [0-1]AssetPortfolioAssociation - > Portfolio
            this.HasOptional(ap => ap.Portfolio).WithOptionalDependent(a => a.Asset);

        }
    }
    #endregion

}
