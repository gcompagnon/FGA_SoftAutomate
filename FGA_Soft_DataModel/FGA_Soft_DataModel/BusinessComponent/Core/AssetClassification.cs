using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Core
{
    /// <summary>
    /// Representation des classifications marché
    /// </summary>
    public class AssetClassification
    {
        public const string SHEMA_NAME = "ref_security";
        #region Clé Primaire
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        public string Source { get; set; }
        #endregion

        public AssetClassification()
        {
        }
        public AssetClassification(string Source)
        {
            this.Source = Source;
        }

        public string Classification1 { get; set; }
        public string Classification2 { get; set; }
        public string Classification3 { get; set; }
        public string Classification4 { get; set; }
        public string Classification5 { get; set; }
        public string Classification6 { get; set; }
        public string Classification7 { get; set; }

        #region foreign key Asset classified
        public Nullable<Int64> AssetId { get; set; }
        public string ISINId { get; set; }
        #endregion

        /// <summary>
        /// Specifies the asset with that classif
        /// </summary>
        public virtual Asset Asset { get; set; }

    }
    #region CONFIGURATION MAPPING

    public class AssetClassificationModelConfiguration : EntityTypeConfiguration<AssetClassification>
    {
        public AssetClassificationModelConfiguration()
        {
            // definition de la cle primaire composite: composée avec l isin d une cle autogeneree
            HasKey(s => new { s.Id, s.Source });

            Map<AssetClassification>(m =>
            {
                m.ToTable("ASSET_CLASSIFICATION", AssetClassification.SHEMA_NAME);
            });

            this.HasRequired(c => c.Asset).WithMany(a => a.AssetClassification).HasForeignKey(c => new { c.AssetId, c.ISINId });
        }
    }
    #endregion


}
