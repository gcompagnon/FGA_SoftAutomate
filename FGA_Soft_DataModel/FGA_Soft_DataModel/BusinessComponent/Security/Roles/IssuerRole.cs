using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;

namespace FGABusinessComponent.BusinessComponent.Security.Roles
{
    /// <summary>
    /// Legal entity that has the right to issue securities.
    /// </summary>
    public class IssuerRole:Role
    {
        public IssuerRole()
        {
        }

        public IssuerRole(string issuer=null, CountryCode country=null)
        {
            if (issuer!=null && issuer.Length > 60)
                issuer = issuer.Substring(0, 59);
            this.IssuerName = issuer;
            this.Country = country??new CountryCode();
        }

        public CountryCode Country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [MaxLength(60)]
        public string IssuerName { get; set; }

    }
}
