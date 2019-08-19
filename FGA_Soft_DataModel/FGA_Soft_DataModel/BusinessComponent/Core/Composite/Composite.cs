using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Security.Pricing;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Holding;

namespace FGABusinessComponent.BusinessComponent.Core.Composite
{
    /// <summary>
    /// Classe de base pour designer les conteneurs de positions (soit des lignes de positions, soit d autres conteneurs) , comme les portefeuilles ou les indices
    /// CF pattern Composite 
    /// </summary>
    public abstract class Composite:Component
    {
        public virtual ICollection<Component> Items { get; set; }

        public Composite()
        {
            this.Items = new List<Component>();
            this.Valuation = new List<Valuation>();
        }

        public Composite(string ISIN,DateTime? Date)
            :base(ISIN,Date)
        {
            this.Items = new List<Component>();
            this.Valuation = new List<Valuation>();
        }

        public void Add(Component c)
        {
            c.Parent = this;
            this.Items.Add(c);
        }

        public void Remove(ref Component c)
        {
            this.Items.Remove(c);
        }
    
        /// <summary>
        /// Information on the valuation of the contener
        /// </summary>
        #region Association Valuation
        public virtual ICollection<Valuation> Valuation { get; set; }
        public void Add(Valuation r)
        {
            this.Valuation.Add(r);
        }

        public void Remove(ref Valuation r)
        {
            this.Valuation.Remove(r);
        }
        #endregion        
    }

    public class CompositeModelConfiguration : EntityTypeConfiguration<Composite>
    {
        public CompositeModelConfiguration()
        {
            //// la foreign key : relation Composite 1->* Component            
            HasMany(s => s.Items).WithOptional(l => l.Parent).Map(d => d.MapKey("ParentId", "ParentISIN","ParentDate"));
        }
    }

}
