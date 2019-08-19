using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Common;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Security;

namespace FGABusinessComponent.BusinessComponent.Core.Composite
{
    /// <summary>
    /// Classe de base pour les lignes de positions en titres vifs ou en tant que sous-indices(indice comoposite) ou même fonds de fonds ( 1 portefeuille contenu dans un autre portefeuille comme une poche de gestion)
    /// CF pattern Composite 
    /// </summary>
    public abstract class Component
    {
        public const string HOLDING_SHEMA_NAME = "ref_holding";
        internal static readonly DateTime DATETIME_DEFAULT = new DateTime(9999, 12, 31);

        #region Clé Primaire
        [Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        [Column("ISIN", Order = 1, TypeName = "nchar"), MaxLength(12)]
        internal string ISINId { get; set; }

        [Column(Order = 2)]
        public DateTime Date { get; set; }
        #endregion

        /// <summary>
        /// constructeur par défaut (obligatoire pour EF)
        /// </summary>
        public Component()
        {            
        }

        /// <summary>
        /// constructeur
        /// </summary>
        public Component(string ISINId, DateTime? Date=null)
        {
            this.ISINId = ISINId;
            this.Date = Date ?? DATETIME_DEFAULT;
        }

        #region Lien sur le Contenant
        /// <summary>
        /// 1 seul contenant pour un composant / ou Null car une racine (indice racine ou 
        /// </summary>
        public virtual Composite Parent { get; set; }

        #endregion


        #region Fonctions communes
        /// <summary>
        /// Test pour savoir si cette ligne est un actif vif
        /// </summary>
        /// <returns>true si c'est une position en titres , false si c'est un portefeuille ou un indice</returns>
        public abstract Boolean isSecurityHolding();
        #endregion



    }
    #region CONFIGURATION MAPPING
    /// <summary>
    /// configuration Fluent API 
    /// En complément de la configuration par Attributs: DataAnnotations
    /// </summary>
    public class ComponentModelConfiguration : EntityTypeConfiguration<Component>
    {
        public ComponentModelConfiguration()
        {

            HasKey(s => new { s.Id, s.ISINId, s.Date });

            Map<Component>(m =>
            {
                m.ToTable("COMPONENT", Component.HOLDING_SHEMA_NAME);
            });

            // relation componennt - composite:  component 1 -> * composite
            HasOptional(c => c.Parent).WithMany(c => c.Items).Map(d => d.MapKey("ParentId", "ParentISIN","ParentDate"));

        }
    }
    #endregion
}
