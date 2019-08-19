using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Security;
using System.Data.Entity.ModelConfiguration;

namespace FGABusinessComponent.BusinessComponent.Security.Roles
{
    /// <summary>
    /// Specifies roles played by a party that are linked to the handling of assets but not related to a specific process.
    /// </summary>
    public class Role
    {
        public const string SHEMA_NAME = "ref_issuer";

        public Role()
        {
        }
        #region Clé Primaire
        [Key]
        public Int64 Id { get; set; }
        #endregion


        #region foreign key Asset hold
        public Nullable<Int64> AssetId { get; set; }
        public string ISINId { get; set; }
        #endregion

        /// <summary>
        /// Specifies the asset included in the holding.
        /// </summary>
        public virtual Asset Asset { get; set; }

        public void Set(Asset Roler)
        {
            this.Asset = Roler;
        }

    }

    #region CONFIGURATION MAPPING

    public class RoleModelConfiguration : EntityTypeConfiguration<Role>
    {
        public RoleModelConfiguration()
        {
            // config pour le TPT / Table Per Type 
            Map<Role>(m =>
            {
                m.ToTable("ROLE", Role.SHEMA_NAME);
            });
            //// la foreign key : relation Asset 1->* Role
            this.HasRequired(r => r.Asset).WithMany(a => a.AssetRoles).HasForeignKey(r => new { r.AssetId, r.ISINId });

        }
    }
    #endregion
}
