using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Core.Composite;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.Core;
using FGABusinessComponent.BusinessComponent.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Holding.PortfolioComp
{
    /// <summary>
    /// Classe représentant un Portefeuille:
    /// composition d un ensemble d actifs financiers, ou d autres portefeuilles
    /// </summary>
    public class Portfolio:Composite
    {
        public Portfolio()
        {
        }
          /// <summary>
        /// constructeur par défaut (obligatoire pour EF)
        /// </summary>
        public Portfolio(string ISIN, DateTime Date,string Name = null, SecuritiesIdentification Identification =null)
            :base(ISIN,Date)
        {
            this.Name = Name;
            this.Identification = Identification;
        }

        /// <summary>
        /// Test pour savoir si cette ligne est un actif vif
        /// </summary>
        /// <returns>true si c'est une position en titres , false si c'est un portefeuille ou un indice</returns>
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
                if (Identification == null)
                    Identification = new SecuritiesIdentification(Isin: this.ISINId);

                return Identification.SecurityIdentification;
            }
            set
            {
                if (Identification == null)
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
        /// Specifies the asset with that classif
        /// </summary>
        public virtual AssetPortfolioAssociation Asset { get; set; }

    }

    #region CONFIGURATION MAPPING
    /// <summary>
    /// configuration Fluent API 
    /// En complément de la configuration par Attributs: DataAnnotations
    /// </summary>
    public class PortfolioModelConfiguration : EntityTypeConfiguration<Portfolio>
    {
        public PortfolioModelConfiguration()
        {
            // config pour le TPT / Table Per Type 
            Map<Portfolio>(m =>
            {
                m.ToTable(tableName: "PORTFOLIO", schemaName: Component.HOLDING_SHEMA_NAME);
            });
        }
    }
    #endregion
}
