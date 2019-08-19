using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;

namespace FGABusinessComponent.BusinessComponent.Holding.IndexComp
{
    /// <summary>
    /// Représentation d un indice taux, comme EONIA + 2% (par exemple)
    /// </summary>
    //public class IndexRate : Index
    //{

    //    ///<summary>
    //    ///Niveau de l'indice
    //    ///Taux ou montant 
    //    ///</summary>
    //    ///<returns></returns>
    //    public RateAndAmount IndexFactor { get; set; }

    //    public double IndexRateMultiplier { get; set; }

    //    public PercentageRate IndexRateBasis { get; set; }

    //    [Column("IndexRateCurrency")]
    //    public CurrencyCode IndexRateCurrency { get; set; }

    //    [Column("IndexRateFrequency")]
    //    public FrequencyCode IndexRateFrequency { get; set; }

    //    public string ReferenceSource { get; set; }

    //    public int IndexPoints { get; set; }
    //}

    //#region MAPPING CONFIG
    //public class IndexRateDataModelConfiguration : EntityTypeConfiguration<IndexRate>
    //{
    //    public IndexRateDataModelConfiguration()
    //    {
    //        Map<IndexRate>(m =>
    //        {
    //            m.Requires("IndexType").HasValue("IndexRate");
    //        });

    //    }

    //}
    //#endregion
}
