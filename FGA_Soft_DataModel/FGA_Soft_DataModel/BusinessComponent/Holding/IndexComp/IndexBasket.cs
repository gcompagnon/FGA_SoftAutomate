using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Core;
using System.Data.Entity.ModelConfiguration;

namespace FGABusinessComponent.BusinessComponent.Holding.IndexComp
{
    /// <summary>
    /// Représentation d un indice composé d'actifs (représentation d un secteur ou d'un marché)
    /// Le prix est market-weighted (Weighted Average Market Capitalization) 
    /// ou price-weighted
    /// ou ...
    /// </summary>
    //public class IndexBasket : Index
    //{

    //    public string ReferenceSource { get; set; }

    //    public virtual IList<Asset> Compo { get; set; }

    //}

    //public class IndexBasketDataModelConfiguration : EntityTypeConfiguration<IndexBasket>
    //{
    //    public IndexBasketDataModelConfiguration()
    //    {
    //        Map<IndexBasket>(m =>
    //        {
    //            m.Requires("IndexType").HasValue("IndexCompo");
    //        });
    //        // la foreign key : relation Index 1->* IndexAsset            
    //        HasMany(s => s.Compo).WithRequired(l => l.IndexBasket).HasForeignKey(s => s.IndexBasketId);
    //    }
    //}

}
