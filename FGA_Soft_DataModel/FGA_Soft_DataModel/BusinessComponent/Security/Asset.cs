using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;

using System.Data.Entity.ModelConfiguration;

using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Holding;
using Role = FGABusinessComponent.BusinessComponent.Security.Roles.Role;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Security
{
    /// <summary>
    /// Représente 1 actif
    /// Tangible items of value to a business.
    /// </summary>
    public abstract class Asset
    {
        public const string SHEMA_NAME = "ref_security";
        public const string TABLE_NAME = "ASSET";

        public Asset(string ISIN = null, string FinancialInstrumentName = null, DateTime? MaturityDate = null, FinancialAssetTypeCategoryCode FinancialAssetCategory = null)
        {
            this.ISINId = ISIN;

            this.FinancialInstrumentName = FinancialInstrumentName;
            this.MaturityDate = MaturityDate;
            if (FinancialAssetCategory == null)
                this.FinancialAssetCategory = new FinancialAssetTypeCategoryCode();
            else
                this.FinancialAssetCategory = FinancialAssetCategory;

            this.AssetValue = new List<AssetHolding>();
            this.AssetClassification = new List<AssetClassification>();
            this.AssetRoles = new List<Role>();

        }


        #region Clé Primaire
        [Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        /// <summary>
        /// La cle primaire est constitué du code ISIN si possible, en type string obligatoirement
        /// </summary>
        [Column("ISIN", Order = 1, TypeName = "nchar"), MaxLength(12), Required]
        internal string ISINId { get; set; }
        #endregion

        /// <summary>
        /// Description ou commentaire
        /// </summary>
        [MaxLength(350)]
        public string FinancialInstrumentName { get; set; }

        /// <summary>
        /// Returns the value of the 'Maturity Date' containment reference. Planned date, at the time of issuance, on which an interest bearing financial instrument becomes due and principal is repaid by the issuer to the investor.
        /// </summary>
        public DateTime? MaturityDate { get; set; }


        /// <summary>
        /// Categorization of financial asset type.
        /// </summary>
        public FinancialAssetTypeCategoryCode FinancialAssetCategory { get; set; }

        #region Association: AssetValue
        /// <summary>
        /// Specifies the different values of an asset.
        /// </summary>
        public virtual ICollection<AssetHolding> AssetValue { get; set; }


        // different holdings of the asset
        public void Add(AssetHolding c)
        {
            this.AssetValue.Add(c);
        }

        public void Remove(ref AssetHolding c)
        {
            this.AssetValue.Remove(c);
        }
        #endregion

        #region Association: AssetRole
        /// <summary>
        /// Party which plays a role for a specific asset.
        /// </summary>
        public virtual ICollection<Role> AssetRoles { get; set; }
        public void Add(Role r)
        {
            this.AssetRoles.Add(r);
        }

        public void Remove(ref Role r)
        {
            this.AssetRoles.Remove(r);
        }

        #endregion

        #region Association: AssetClassification
        /// <summary>
        /// Classifications pour la ligne
        /// </summary>
        public List<AssetClassification> AssetClassification { get; set; }

        // different classification of the asset
        public void Add(AssetClassification c)
        {
            c.AssetId = this.Id;
            c.ISINId = this.ISINId;
            this.AssetClassification.Add(c);
        }

        public void Remove(ref AssetClassification c)
        {
            this.AssetClassification.Remove(c);
        }

        #endregion

        public virtual AssetPortfolioAssociation UnderlyingPortfolio { get; set; }

    }

    #region CONFIGURATION MAPPING

    public class AssetModelConfiguration : EntityTypeConfiguration<Asset>
    {
        public AssetModelConfiguration()
        {
            // definition de la cle primaire composite: composée avec l isin d une cle autogeneree
            HasKey(s => new { s.Id, s.ISINId });

            //// config pour le TPT / Table Per Type 
            //Map<Asset>(m =>
            //{
            //    //    //m.MapInheritedProperties(); // TPC : ne fonctionne pas avec le polymorphisme
            //    Property(x => x.InvestmentAmount.Value).HasColumnName("InvestmentAmount");
            //    Property(x => x.InvestmentAmount.Currency.Currency).HasColumnName("InvestmentAmount_Cur");
            //    m.ToTable("ASSET", Asset.SHEMA_NAME);
            //});
            ToTable(Asset.TABLE_NAME, Asset.SHEMA_NAME);


            //Property(x => x.InvestmentAmount.Value).HasColumnName("InvestmentAmount");;
            //Property(x => x.InvestmentAmount.Currency.Currency).HasColumnName("InvestmentAmount_Cur");
        }
    }
    #endregion

    public static class AssetExtensions
    {
        public static AssetClassification GetAssetClassification(this List<AssetClassification> target, string source)
        {
            foreach (AssetClassification ac in target)
            {
                if (source == null || ac.Source == source)
                {
                    return ac;
                }
            }
            return null;
        }     
    }
}
