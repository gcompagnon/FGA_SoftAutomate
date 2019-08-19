using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Core.Composite;
using FGABusinessComponent.BusinessComponent.Security;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using FGABusinessComponent.BusinessComponent.Util;

namespace FGABusinessComponent.BusinessComponent.Holding.IndexComp
{
    /// <summary>
    /// Classe représentant un Indice:
    /// composition d un ensemble d actifs financiers, ou d autres indices
    /// indice de type Taux
    /// </summary>
    public class Index:Composite
    {
        
        /// <summary>
        /// constructeur par défaut (obligatoire pour EF)
        /// </summary>
        public Index()
        {
            this.IndexFrequency = new FrequencyCode();            
            this.IndexCurrency = new CurrencyCode();
        }

        /// <summary>
        /// constructeur par défaut (obligatoire pour EF)
        /// </summary>
        /// ///
        /// 

        public Index(string ISIN, DateTime? Date =null, string Name = null, CurrencyCode IndexCurrency = null)
            : base(ISIN, Date)
        {
            this.Name = Name;
            this.Identification = new SecuritiesIdentification(Name: Name, Isin: ISIN);

            this.IndexCurrency = IndexCurrency ?? new CurrencyCode();
            this.IndexFrequency = new FrequencyCode();
        }

        public override Boolean isSecurityHolding()
        {
            return false;
        }

        /// <summary>
        /// Description ou commentaire
        /// </summary>
        [MaxLength(350)]
        public string Name { get; set; }

        /// <summary>
        /// Surcharge de la cle primaire afin d avoir un type non string
        /// </summary>
        [NotMapped]
        public virtual ISINIdentifier ISIN
        {
            get
            {
                if (Identification == null || Identification.SecurityIdentification.Equals(ISINIdentifier.DEFAULT))
                    Identification = new SecuritiesIdentification(Isin: this.ISINId);

                return Identification.SecurityIdentification;
            }
            set
            {
                if (Identification == null || Identification.SecurityIdentification.Equals(ISINIdentifier.DEFAULT))
                    Identification = new SecuritiesIdentification(value);
                else
                    Identification.SecurityIdentification = value;
                this.ISINId = value.ISINCode;
            }
        }

        /// <summary>
        /// Identification du composant (un actif financier, ou un portefeuille ou un indice ou ...)
        /// </summary>
        [ForeignKey("Identification")]
        public Int64 IdentificationId { get; set; }

        [Required]
        public virtual SecuritiesIdentification Identification { get; set; }

        /// <summary>
        /// date et heure de fixing de l'indice (cut off de l indice)
        /// </summary>
//        [Column(TypeName="BigInt")]
        public TimeSpan? IndexFixingDate { get; set; }


        /// <summary>
        /// Frequence de publication de l indice: quotidien pour les indices marché, par ex.
        /// </summary>
        [Column("IndexFrequency")]
        public FrequencyCode IndexFrequency { get; set; }

        /// <summary>
        /// Devise de l indice
        /// </summary>
        [Column("IndexCurrency")]
        public CurrencyCode IndexCurrency { get; set; }

        /// <summary>
        /// Family Key contains several tags, or flag , to be able to identify the family whom the equity is part of.
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


        ///// <summary>
        ///// Information on the "price" of the index
        ///// </summary>
        //#region Association Pricing
        //public virtual ICollection<SecuritiesPricing> Pricing { get; set; }
        //public void Add(SecuritiesPricing r)
        //{
        //    this.Pricing.Add(r);
        //}

        //public void Remove(ref SecuritiesPricing r)
        //{
        //    this.Pricing.Remove(r);
        //}
        //#endregion

        #region Utilitaires/ methodes d instances
        /// <summary>
        /// Give if the static data are valid or not
        /// </summary>
        /// <returns></returns>
        public bool isStaticDataValid()
        {
            if (this.Name  == null ||
                this.IndexFrequency == null || this.IndexFrequency.IndexFrequency == null ||
                this.ISIN == null || ISINIdentifier.DEFAULT.Equals(this.ISIN))
                return false;
            return true;
        }
        #endregion


    }


    #region CONFIGURATION MAPPING
    /// <summary>
    /// configuration Fluent API 
    /// En complément de la configuration par Attributs: DataAnnotations
    /// </summary>
    public class IndexModelConfiguration : EntityTypeConfiguration<Index>
    {
        public IndexModelConfiguration()
        {

            // specifier une colonne timestamp pour gerer la concurrence => OptimisticConcurrencyException
            //Property(d => d.RowVersion).IsRowVersion();

            // config pour le TPT / Table Per Type 
            Map<Index>(m =>
                {
                    Property(d => d.IndexFrequency.IndexFrequency).HasColumnName("IndexFrequency");
                    Property(d => d.IndexCurrency.Currency).HasColumnName("IndexCurrency");
                    m.ToTable(tableName: "INDEX", schemaName: Component.HOLDING_SHEMA_NAME);
                });
        }
    }
    #endregion
}
