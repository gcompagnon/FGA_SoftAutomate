using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Core.Composite;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Common;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Security;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Util;

namespace FGABusinessComponent.BusinessComponent.Holding
{
    /// <summary>
    /// Specifies in terms of value and quantity the assets.
    /// </summary>
    public class AssetHolding : Component
    {
        #region constructors
        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        public AssetHolding()
        {
            this.FaceAmount = new CurrencyAndAmount();
            this.BookValue = new CurrencyAndAmount();
            this.MarketValue = new CurrencyAndAmount();
            this.Weight = new PercentageRate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="Holder"></param>
        /// <param name="HolderISIN"></param>
        /// <param name="HoldAsset"></param>
        /// <param name="Quantity"></param>
        /// <param name="MarketValue"></param>
        /// <param name="BookValue"></param>
        /// <param name="FaceAmount"></param>
        public AssetHolding(DateTime Date, string ISIN, Asset HoldAsset, 
            Composite Holder = null,
            float? Quantity = null,
            CurrencyAndAmount MarketValue = null,
            CurrencyAndAmount BookValue = null,
            CurrencyAndAmount FaceAmount = null,
            PercentageRate Weight = null)
            :base(ISIN,Date)
        {
            // le code ISIN est celui du conteneur (portfolio , index)
            if (Holder != null)
            {
                this.Parent = Holder;
            }
            // lien sur l actif detenu
            this.Asset = HoldAsset;
            if (ISIN == null)
            {
                this.ISINId = HoldAsset.ISINId;
            }
            this.Quantity = Quantity;
            this.FaceAmount = FaceAmount ?? new CurrencyAndAmount();
            this.BookValue = BookValue ?? new CurrencyAndAmount();
            this.MarketValue = MarketValue ?? new CurrencyAndAmount();
            this.Weight = Weight ?? new PercentageRate();
        }
        #endregion

        [NotMapped]
        public ISINIdentifier ISIN
        {
            get
            {
                return (ISINIdentifier)this.ISINId;
            }
            set
            {
                base.ISINId = value.ISINCode;
            }
        }

        ///<summary>
        ///Specifies the asset included in the holding.
        ///</summary>
        #region lien avec le Composite contenant l asset holding, en doublon evc
        public ISINIdentifier ParentIdentification
        {
            get
            {
                if (base.Parent == null)
                    return new ISINIdentifier();
                else
                    return (ISINIdentifier)base.Parent.ISINId;
            }
            set
            {
            }
        }
        #endregion

        //[Column("Parent", Order = 2, TypeName = "nchar"), MaxLength(12), Required]
        //internal string ParentISINId { get; set; }

        #region Fonctions communes
        public override Boolean isSecurityHolding()
        {
            return true;
        }
        #endregion


        /// <summary>
        /// Quantity expressed as an amount representing the face amount, ie, the principal, of a debt instrument.
        /// </summary>
        public CurrencyAndAmount FaceAmount { get; set; }

        /// <summary>
        /// Value of the asset holding based on current market prices.
        /// </summary>
        public CurrencyAndAmount MarketValue { get; set; }

        /// <summary>
        /// Value of a security, as booked/acquired in an account. Book value is often different from the current market value of the security.
        /// </summary>
        public CurrencyAndAmount BookValue { get; set; }

        /// <summary>
        /// Quantity of Asset hold 
        /// </summary>
        public float? Quantity { get; set; }

        /// <summary>
        /// Weight of this holdings in the Parent Composite
        /// </summary>
        public PercentageRate Weight { get; set; }

        /// <summary>
        /// Specifies the asset included in the holding.
        /// </summary>
        #region lien avec Asset
        public virtual Asset Asset { get; set; }
        public void Set(Asset a)
        {
            this.Asset = a;
        }
        #endregion

        #region FamilyKey
        /// <summary>
        /// Family Key contains several tags, or flag , to be able to identify the family whom the security is part of.
        /// </summary>
        [Column("FamilyKey")]
        public String FamilyKey { get; set; }

        [NotMapped]
        public Stringeable FamilyKeyObject
        {
            get
            {
                return SerializationHelpers.StringToObject<Stringeable>(this.FamilyKey);
            }
            set
            {
                this.FamilyKey = SerializationHelpers.ObjectToString<Stringeable>(value);
            }
        }
        #endregion

    }

    #region CONFIGURATION MAPPING

    public class AssetHoldingModelConfiguration : EntityTypeConfiguration<AssetHolding>
    {
        public AssetHoldingModelConfiguration()
        {
            Map<AssetHolding>(m =>
            {

                Property(x => x.ParentIdentification.ISINCode).HasColumnName("ParentISIN").IsOptional().HasColumnOrder(3);
                
                Property(x => x.FaceAmount.Currency.Currency).HasColumnName("FaceAmount_Cur");
                Property(x => x.FaceAmount.Value).HasColumnName("FaceAmount");
                Property(x => x.MarketValue.Currency.Currency).HasColumnName("MarketValue_Cur");
                Property(x => x.MarketValue.Value).HasColumnName("MarketValue");
                Property(x => x.BookValue.Currency.Currency).HasColumnName("BookValue_Cur");
                Property(x => x.BookValue.Value).HasColumnName("BookValue");
                Property(x => x.Weight.Value).HasColumnName("Weight");

                m.ToTable("ASSET_HOLDING", Component.HOLDING_SHEMA_NAME);
            });

            //// la foreign key : relation AssetHolding 1-*->1 Asset
            //this.HasRequired(h => h.Asset).WithMany(a => a.AssetValue).Map(d => d.MapKey("AssetId", "AssetISIN"));
            this.HasRequired(h => h.Asset).WithMany(a => a.AssetValue).Map(d => d.MapKey("AssetId", "AssetISIN"));
        }
    }
    #endregion

}
