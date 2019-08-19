using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Security
{
    /// <summary>
    /// Assessment of securities credit and investment risk.
    /// </summary>
    public class Rating
    {
        public Rating()
        {
        }

        public Rating(string Value=null, DateTime? ValueDate = null, string RatingScheme =null)
        {
            this.Value = Value;
            this.ValueDate = ValueDate;
            this.RatingScheme = RatingScheme;
        }


        public const string SHEMA_NAME = "ref_rating";
        public const string TABLE_NAME = "RATING";
        
        #region Clé Primaire
        [Key ,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        #endregion

        /// <summary>
        /// Specifies the rated security 
        /// </summary>
        #region foreign key Asset rated
        public string ISINId { get; set; }
        public Int64 AssetId { get; set; }
        public virtual Security RatedSecurity { get; set; }
        public void Set(Security s)
        {
            this.RatedSecurity = s;
            s.Rating = this;
        }
        #endregion        


        /// <summary>
        /// Specifies the rating, which has been assigned to a security by a rating agency.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Information regarding the entity that assigns the rating.
        /// </summary>
        public string RatingScheme { get; set; }

        /// <summary>
        /// Date/time as from which the rating is valid.
        /// </summary>
        public DateTime? ValueDate { get; set; }

        public string Moody { get; set; }
        public DateTime? MoodyDate { get; set; }
        public string SnP { get; set; }
        public DateTime? SnPDate { get; set; }
        public string Fitch { get; set; }
        public DateTime? FitchDate { get; set; }



        public override bool Equals(object obj)
        {
            Rating code = obj as Rating;
            return this.Equals(code);
        }

        public bool Equals(Rating code)
        {
            if (code == null || code.Value ==null || this.Value == null)
                return false;
            return this.Value.Equals(code.Value);
        }

        public override int GetHashCode()
        {
            return ( this.Value.GetHashCode() + this.RatingScheme.GetHashCode() )/2;
        }

    }


    #region CONFIGURATION MAPPING

    public class RatingModelConfiguration : EntityTypeConfiguration<Rating>
    {
        public RatingModelConfiguration()
        {

            Map<Rating>(m =>
            {
                m.ToTable(Rating.TABLE_NAME, Rating.SHEMA_NAME);
            });

            // foreign key entre Rating et Security
            HasRequired(c => c.RatedSecurity).WithMany().HasForeignKey(r => new { r.AssetId, r.ISINId });
        }
    }
    #endregion

}
