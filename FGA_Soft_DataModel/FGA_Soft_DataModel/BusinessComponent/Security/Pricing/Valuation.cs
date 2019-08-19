using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Core.Composite;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Common;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Security.Pricing
{
    /// <summary>
    /// Valuation information of the portfolio / indexes.
    /// </summary>
    public class Valuation
    {
        #region constructors
        /// <summary>
        /// Constructeur par defaut
        /// </summary>
        internal Valuation()
        {
            this.FaceAmount = new CurrencyAndAmount();
            this.BookValue = new CurrencyAndAmount();
            this.MarketValue = new CurrencyAndAmount();
            this.DebtYield = new DebtYield();
            this.DebtSpread = new DebtSpread();
            this.DebtDataCalculation = new DebtDataCalculation();
            this.Yield = new Yield();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ISINHolder"></param>
        /// <param name="HoldAsset"></param>
        /// <param name="Quantity"></param>
        /// <param name="MarketValue"></param>
        /// <param name="BookValue"></param>
        /// <param name="FaceAmount"></param>
        internal Valuation(DateTime Date, Composite Valuated = null, CurrencyAndAmount MarketValue = null,
            CurrencyAndAmount BookValue = null, CurrencyAndAmount FaceAmount = null,
            Yield Yield = null,
            DebtDataCalculation DebtDataCalculation = null,
            DebtYield DebtYield = null, DebtSpread DebtSpread = null)
        {
            if (Valuated != null)
            {
                Valuated.Add(this);
                this.Valuated = Valuated;
            }
            this.Date = Date;
            this.FaceAmount = FaceAmount ?? new CurrencyAndAmount();
            this.BookValue = BookValue ?? new CurrencyAndAmount();
            this.MarketValue = MarketValue ?? new CurrencyAndAmount();
            this.Yield = Yield ?? new Yield();
            this.DebtYield = DebtYield ?? new DebtYield();
            this.DebtSpread = DebtSpread ?? new DebtSpread();
            this.DebtDataCalculation = DebtDataCalculation ?? new DebtDataCalculation();
        }
        #endregion

        #region Clé Primaire
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }

        /// <summary>
        /// Date/time of Valuation
        /// </summary>
        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }
        #endregion

        /// <summary>
        /// Quantity expressed as an amount representing the face amount, ie, the principal, of a debt instrument.
        /// </summary>
        public CurrencyAndAmount FaceAmount { get; set; }

        /// <summary>
        /// Value of the asset holding based on current market prices.
        /// </summary>
        public CurrencyAndAmount MarketValue { get; set; }

        /// <summary>
        /// Value of a security, as booked/acquired in an account. Book value is often different from the current market value of the security.
        /// </summary>
        public CurrencyAndAmount BookValue { get; set; }

        /// <summary>
        /// Data concerning Debt Security (duration, sensitivity...)
        /// </summary>
        public DebtDataCalculation DebtDataCalculation { get; set; }

        /// <summary>
        /// All the data concerning Yields and Performance
        /// </summary>
        public Yield Yield { get; set; }

        /// <summary>
        /// All the data concerning Yields and Performance
        /// </summary>
        public DebtYield DebtYield { get; set; }

        /// <summary>
        /// All the data concerning Yields and Performance
        /// </summary>
        public DebtSpread DebtSpread { get; set; }

        /// <summary>
        /// Identifies the source valuation
        /// </summary>
        public string ValuationSource { get; set; }


        /// <summary>
        /// Specifies the Container (Portfolio or Index) included in the holding.
        /// </summary>        
        #region foreign key Valuated
        public Nullable<Int64> ContainerId { get; set; }
        public string ISINId { get; set; }
        public DateTime ContainerDate { get; set; }
        #endregion

        /// <summary>
        /// Specifies the Valuation Item 
        /// </summary>
        public virtual Composite Valuated { get; set; }

        public void Set(Composite Valuated)
        {
            this.Valuated = Valuated;
        }
    }

    /// <summary>
    /// sub class for a valuation of Index
    /// </summary>
    public class IndexValuation : Valuation
    {
        public IndexValuation()
            : base()
        { }

        public IndexValuation(DateTime Date, double? IndexPriceValue = null, double? IndexGrossValue = null, double? IndexNetValue = null, DateTime? BaseDate = null, double? BaseValue = null,
             Composite Valuated = null, CurrencyAndAmount MarketValue = null,
            CurrencyAndAmount BookValue = null, CurrencyAndAmount FaceAmount = null, Yield Yield = null,
            DebtDataCalculation DebtDataCalculation = null,
            DebtYield DebtYield = null, DebtSpread DebtSpread = null)
            : base(Date, Valuated, MarketValue, BookValue, FaceAmount,Yield, DebtDataCalculation, DebtYield, DebtSpread)
        {
            this.IndexPriceValue = IndexPriceValue;
            this.IndexGrossValue = IndexGrossValue;
            this.IndexNetValue = IndexNetValue;
            this.IndexBaseDate = BaseDate;
            this.IndexBaseValue = BaseValue;
        }
        /// <summary>
        /// Divisor, is the Market Value (Market Cap) / Index Value
        /// Same currency that MarketValue
        /// </summary>
        public double? IndexDivisor { get; set; }
        /// <summary>
        /// Level Value of the index/ Only Price Change
        /// Same currency that MarketValue
        /// </summary>
        public double? IndexPriceValue { get; set; }

        /// <summary>
        /// Level Value of the index with gross dividends. Index reinvests dividend distributions. 
        /// Same currency that MarketValue
        /// </summary>
        public double? IndexGrossValue { get; set; }

        /// <summary>
        /// Level Value of the index with gross dividends. Index reinvests dividend distributions after deduction of withholding taxes.
        /// </summary>
        public double? IndexNetValue { get; set; }

        /// <summary>
        /// Size of the Index, if securities composition: the number of securities in the index
        /// </summary>
        [Column("IndexNumberOfSecurities")]
        public int? IndexNumberOfSecurities { get; set; }

        /// <summary>
        /// Date of the first value
        /// </summary>
        public DateTime? IndexBaseDate { get; set; }
        /// <summary>
        /// First value (100 for example)
        /// </summary>
        public double? IndexBaseValue { get; set; }
    }

    /// <summary>
    /// Sub class for Portfolio valorisation
    /// </summary>
    public class PortfolioValuation : Valuation
    {
        public PortfolioValuation()
            : base()
        { }

        public PortfolioValuation(DateTime Date, float? NbParts = null,
             Composite Valuated = null, CurrencyAndAmount MarketValue = null,
            CurrencyAndAmount BookValue = null, CurrencyAndAmount FaceAmount = null, Yield Yield = null,
            DebtDataCalculation DebtDataCalculation = null,
            DebtYield DebtYield = null, DebtSpread DebtSpread = null)
            : base(Date, Valuated, MarketValue, BookValue, FaceAmount, Yield, DebtDataCalculation, DebtYield, DebtSpread)
        {
            this.NumberOfParts = NbParts;
        }
        public float? NumberOfParts { get; set; }
    }


    #region CONFIGURATION MAPPING

    public class ValuationModelConfiguration : EntityTypeConfiguration<Valuation>
    {
        public ValuationModelConfiguration()
        {

            Map<Valuation>(m =>
            {

                Property(x => x.FaceAmount.Currency.Currency).HasColumnName("FaceAmount_Cur");
                Property(x => x.FaceAmount.Value).HasColumnName("FaceAmount");
                Property(x => x.MarketValue.Currency.Currency).HasColumnName("MarketValue_Cur");
                Property(x => x.MarketValue.Value).HasColumnName("MarketValue");
                Property(x => x.BookValue.Currency.Currency).HasColumnName("BookValue_Cur");
                Property(x => x.BookValue.Value).HasColumnName("BookValue");
                Property(x => x.DebtDataCalculation.MacaulayDuration).HasColumnName("Debt_Duration");
                Property(x => x.DebtDataCalculation.ModifiedDuration).HasColumnName("Debt_Sensitivity");
                Property(x => x.DebtDataCalculation.TimeToMaturity).HasColumnName("Debt_TTM");
                Property(x => x.DebtDataCalculation.Convexity).HasColumnName("Debt_Convexity");

                Property(x => x.DebtYield.YieldToMaturityRate.Value).HasColumnName("Debt_YTM_Rate");

                m.ToTable("VALUATION", Component.HOLDING_SHEMA_NAME);
            });

            Map<IndexValuation>(m => m.Requires("ValuationType").HasValue("INDEX"));
            Map<PortfolioValuation>(m => m.Requires("ValuationType").HasValue("PTF"));

            //reference to Security
            HasOptional(p => p.Valuated).WithMany(s => s.Valuation).HasForeignKey(p => new { p.ContainerId, p.ISINId, p.ContainerDate });

        }
    }
    #endregion

}
