using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Security.Pricing
{
    /// <summary>
    /// Characteristics related to the price of the security.
    /// </summary>
    public class SecuritiesPricing
    {
        public SecuritiesPricing()
        {
            this.PriceType = new TypeOfPriceCode();
            this.DebtPriceCalculation = new DebtPriceCalculation();
            this.DebtDataCalculation = new DebtDataCalculation();
            this.PriceFactType = new PriceFactType();
        }
        public SecuritiesPricing(CurrencyAndAmount Price, DateTime Date, TypeOfPriceCode PriceType)
        {
            this.Price = Price;
            this.PriceType = PriceType ?? (TypeOfPriceCode)"MARKET";
            this.Date = Date;
            this.DebtPriceCalculation = new DebtPriceCalculation();
            this.DebtDataCalculation = new DebtDataCalculation();

            this.PriceFactType = new PriceFactType();

            this.Yield = new Yield();
            this.DebtSpread = new DebtSpread();
            this.DebtYield = new DebtYield();

        }

        public SecuritiesPricing(CurrencyAndAmount Price, DateTime Date, TypeOfPriceCode PriceType,
            double? AskPrice = null, double? BidPrice = null, double? MidPrice = null,
            PriceFactType PriceFactType = null, Yield Yield = null,
            DebtPriceCalculation DebtPriceCalculation = null,
            DebtDataCalculation DebtDataCalculation = null, DebtYield DebtYield = null, DebtSpread DebtSpread = null)
        {
            this.Price = Price;
            this.PriceType = PriceType ?? (TypeOfPriceCode)"MARKET";
            this.Date = Date;
            this.AskPrice = AskPrice;
            this.BidPrice = BidPrice;
            this.MidPrice = MidPrice;

            this.PriceFactType = PriceFactType ?? new PriceFactType();
            this.Yield = Yield ?? new Yield();

            this.DebtPriceCalculation = DebtPriceCalculation ?? new DebtPriceCalculation();
            this.DebtDataCalculation = DebtDataCalculation ?? new DebtDataCalculation();

            this.DebtSpread = DebtSpread ?? new DebtSpread();
            this.DebtYield = DebtYield ?? new DebtYield();
        }

        #region Clé Primaire
        [Key, Column(Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Int64 Id { get; set; }
        /// <summary>
        /// Date/time of the price. For CIV, this is the NAV date.
        /// </summary>
        [Key, Column(Order = 1)]
        public DateTime Date { get; set; }
        #endregion


        #region Main Price
        /// <summary>
        /// Value of the price. (last main/market or mid )
        /// </summary>
        public CurrencyAndAmount Price { get; set; }

        /// <summary>
        /// Type and information about a price.
        /// </summary>
        public TypeOfPriceCode PriceType { get; set; }
      
        /// <summary>
        /// Identifies the Benchmark source price (eg. BB Generic, BB Fairvalue, Brokertec..).
        /// </summary>
        public string PriceSource { get; set; }
        #endregion


        public double? AskPrice { get; set; }
        public double? BidPrice { get; set; }
        public double? MidPrice { get; set; }


        //<summary>
        //Specifies observed limits or specific price condition.
        //</summary>
        public PriceFactType PriceFactType { get; set; }

        /// <summary>
        /// All the data concerning Yields and Performance
        /// </summary>
        public Yield Yield { get; set; }

        /// <summary>
        /// Price concerning Debt Security
        /// </summary>
        public DebtPriceCalculation DebtPriceCalculation { get; set; }

        /// <summary>
        /// Data concerning Debt Security (duration, sensitivity...)
        /// </summary>
        public DebtDataCalculation DebtDataCalculation { get; set; }

        /// <summary>
        /// All the data concerning Yields and Performance
        /// </summary>
        public DebtYield DebtYield { get; set; }

        /// <summary>
        /// All the data concerning Spread with a reference debt security
        /// </summary>
        public DebtSpread DebtSpread { get; set; }

        #region foreign key PricedSecurity
        public Nullable<Int64> SecurityId { get; set; }
        public string ISINId { get; set; }
        #endregion

        /// <summary>
        /// Specifies the Priced security 
        /// </summary>
        public virtual Security PricedSecurity { get; set; }

        public void Set(Security PricedSecurity)
        {
            this.PricedSecurity = PricedSecurity;
        }
    }
    #region CONFIGURATION MAPPING

    public class SecuritiesPricingModelConfiguration : EntityTypeConfiguration<SecuritiesPricing>
    {
        public SecuritiesPricingModelConfiguration()
        {

            Map<SecuritiesPricing>(m =>
            {
                Property(x => x.Date).IsRequired();
                Property(x => x.Price.Value).HasColumnName("Price");
                Property(x => x.AskPrice).HasColumnName("Price_Ask");
                Property(x => x.BidPrice).HasColumnName("Price_Bid");
                Property(x => x.MidPrice).HasColumnName("Price_Mid");
                Property(x => x.PriceType.TypeOfPrice).HasColumnName("Price_Type");
                Property(x => x.PriceSource).HasColumnName("Price_Source");
                Property(x => x.Price.Currency.Currency).HasColumnName("Price_Cur");
                Property(x => x.DebtPriceCalculation.AccruedInterest).HasColumnName("Debt_AI");
                Property(x => x.DebtPriceCalculation.CleanPrice).HasColumnName("Debt_CleanP");
                Property(x => x.DebtPriceCalculation.DirtyPrice).HasColumnName("Debt_DirtyP");
                Property(x => x.DebtDataCalculation.MacaulayDuration).HasColumnName("Debt_Duration");
                Property(x => x.DebtDataCalculation.ModifiedDuration).HasColumnName("Debt_Sensitivity");
                Property(x => x.DebtDataCalculation.TimeToMaturity).HasColumnName("Debt_TTM");
                Property(x => x.DebtDataCalculation.Convexity).HasColumnName("Debt_Convexity");

                Property(x => x.DebtYield.YieldToMaturityRate.Value).HasColumnName("Debt_YTM_Rate");

                m.ToTable("PRICE", Asset.SHEMA_NAME);
            });
            //reference to Security
            HasOptional(p => p.PricedSecurity).WithMany(s => s.Pricing).HasForeignKey(p => new { p.SecurityId, p.ISINId });

        }
    }
    #endregion

    /// <summary>
    /// Calculation of the price of a fixed-income security
    /// </summary>
    [ComplexType]
    public class DebtPriceCalculation
    {
        public DebtPriceCalculation()
        {
        }
        public DebtPriceCalculation(double? CleanPrice = null, double? DirtyPrice = null, double? AccruedInterest = null)
        {
            this.CleanPrice = CleanPrice;
            this.DirtyPrice = DirtyPrice;
            this.AccruedInterest = AccruedInterest;
        }
        /// <summary>
        /// Price of a debt security without the accrued interest
        /// </summary>
        public double? CleanPrice { get; set; }
        /// <summary>
        /// Price of a debt security with the accrued interest
        /// </summary>
        public double? DirtyPrice { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double? AccruedInterest { get; set; }
    }
    /// <summary>
    /// Calculation of the price sensitivity of a fixed-income security to a change in interest rates.
    /// </summary>
    [ComplexType]
    public class DebtDataCalculation
    {
        public DebtDataCalculation()
        {
        }
        public DebtDataCalculation(double? MacaulayDuration = null, double? ModifiedDuration = null, double? OADuration = null,
                                double? TimeToMaturity = null, double? Convexity = null, double? OAConvexity = null,
                                double? MacaulayDurationSemiAnnual = null, double? ModifiedDurationSemiAnnual = null, double? ConvexitySemiAnnual = null)
        {
            this.MacaulayDuration = MacaulayDuration;
            this.ModifiedDuration = ModifiedDuration;
            this.TimeToMaturity = TimeToMaturity;
            this.Convexity = Convexity;

            this.MacaulayDurationSemiAnnual = MacaulayDurationSemiAnnual;
            this.ModifiedDurationSemiAnnual = ModifiedDurationSemiAnnual;           
            this.ConvexitySemiAnnual = ConvexitySemiAnnual;

            this.OADuration = OADuration;
            this.OAConvexity = OAConvexity;
        }


        /// <summary>
        /// Weighted-average term to maturity of the cash flows from a bond. The weight of each cash flow is determined by dividing the present value of the cash flow by the price.
        /// based on an annual coupounding frequency / ISMA convention
        /// </summary>
        public double? MacaulayDuration { get; set; }

        /// <summary>
        /// Formula that expresses the measurable change in the value of a security in response to a change in interest rates.
        /// Sensitivite ou Adjusted Duration
        /// </summary>
        public double? ModifiedDuration { get; set; }

        /// <summary>
        /// Option Adjusted
        /// Sensitivite ou Adjusted Duration
        /// </summary>
        public double? OADuration { get; set; }

        /// <summary>
        /// time in years until the maturity
        /// </summary>
        public double? TimeToMaturity { get; set; }

        /// <summary>
        ///  Measure of change in duration (linear measure of changes in bond price in reaction to interest rate changes) of a bond as interest rate changes over time.
        /// </summary>
        public double? Convexity { get; set; }

        /// <summary>
        ///  Option Adjusted Convexity
        /// </summary>
        public double? OAConvexity { get; set; }

        /// <summary>
        /// Weighted-average term to maturity of the cash flows from a bond. The weight of each cash flow is determined by dividing the present value of the cash flow by the price.
        /// based on semi annual coupon / SIA convention ( units is in nb of years)
        /// </summary>
        public double? MacaulayDurationSemiAnnual { get; set; }

        /// <summary>
        /// Formula that expresses the measurable change in the value of a security in response to a change in interest rates.
        /// Based on a Semi annual Coupon
        /// Sensitivite ou Adjusted Duration
        /// </summary>
        public double? ModifiedDurationSemiAnnual { get; set; }

        /// <summary>
        ///  Measure of change in duration (linear measure of changes in bond price in reaction to interest rate changes) of a bond as interest rate changes over time.
        ///  
        /// </summary>
        public double? ConvexitySemiAnnual { get; set; }

        /// <summary>
        /// Contains the weighted average life of the fixed income instrument (expressed in months).
        /// </summary>
        public double? WeightedAverageLife { get; set; }

    }


    /// <summary>
    /// Specifies observed limits or specific price condition.
    /// </summary>
    [ComplexType]
    public class PriceFactType
    {
        public PriceFactType()
        {
        }

        public PriceFactType(double? BestPrice = null, double? ClosePrice = null, double? OpenPrice = null,
                                double? LastPrice = null, double? LowPrice = null, double? HighPrice = null)
        {
            this.BestPrice = BestPrice;
            this.ClosePrice = ClosePrice;
            this.OpenPrice = OpenPrice;
            this.LastPrice = LastPrice;
            this.LowPrice = LowPrice;
            this.HighPrice = HighPrice;
        }
        /// <summary>
        /// The last price provided.
        /// </summary>
        public double? LastPrice { get; set; }
        /// <summary>
        /// For bid/ask, top of orderbook.
        /// </summary>
        public double? BestPrice { get; set; }
        /// <summary>
        /// The closing price for the day.
        /// </summary>
        public double? ClosePrice { get; set; }
        /// <summary>
        /// The opening price for the day.
        /// </summary>
        public double? OpenPrice { get; set; }
        /// <summary>
        /// Lowest price during the period.
        /// </summary>
        public double? LowPrice { get; set; }
        /// <summary>
        /// Highest price over the period
        /// </summary>
        public double? HighPrice { get; set; }
    }


    /// <summary>
    /// Different spreads on Debt Yields
    /// </summary>
    [ComplexType]
    public class DebtSpread
    {
        public DebtSpread()
        {
        }
        public DebtSpread(double? GovSpread = null, double? InterpolatedSwapSpread = null,
             double? CDSBondBasisSpread = null,
             double? ZeroVolatilitySpread = null,
             double? AssetSwapSpread = null,
             double? OptionAdjustedSpread = null)
        {
            this.GovSpread = GovSpread;
            this.InterpolatedSwapSpread = InterpolatedSwapSpread;
            this.CDSBondBasisSpread = CDSBondBasisSpread;
            this.ZeroVolatilitySpread = ZeroVolatilitySpread;
            this.OptionAdjustedSpread = OptionAdjustedSpread;
            this.AssetSwapSpread = AssetSwapSpread;
        }

        /// <summary>
        /// Spread in bp (0.01%) with a similar Govies Bonds/Treasuries
        /// </summary>
        public double? GovSpread { get; set; }

        /// <summary>
        /// I-Spread in bp (0.01%) / yield-on-yield spread / the spread of the sec relative to the swap curve,
        /// calculated by taking the yield to maturity of a bond less the interpolated yield on the swap curve
        /// </summary>
        public double? InterpolatedSwapSpread { get; set; }

        /// <summary>
        /// CDS-Bond Basis Spread in bp (0.01%) / difference between  CDS Premium and a selected spread measure measure of a bond.
        /// ( Z-Spread on Bloomberg)
        /// A positive basis implies that the prevailing default swap premium is greater than the spread on the bond
        /// (exemple du "Raw Basis" , où le spread est la difference entre le 5y CDS et le Z-Spread)
        /// </summary>
        public double? CDSBondBasisSpread { get; set; }

        /// <summary>
        /// Z-spread.A constant spread over the Libor zero curve that equates the present value of a bond's cash flows to its market price
        /// 
        /// </summary>
        public double? ZeroVolatilitySpread { get; set; }

        /// <summary>
        /// Asset Swap Margin in bp (0.01%) / the spread of a corporate security relative to Treasures or Libor
        /// represents the credit risk
        /// </summary>
        public double? AssetSwapSpread { get; set; }

        /// <summary>
        /// Option Adjusted Spread / OAS in bp (0.01%) / the spread of a corporate security reelative to Treasures or Libor
        /// ,adjusted for embedded options
        /// </summary>
        public double? OptionAdjustedSpread { get; set; }



    }
    /// <summary>
    /// Yields and Performance datas on debt securities
    /// </summary>
    [ComplexType]
    public class DebtYield
    {
        public DebtYield()
        {
            this.YieldToMaturityRate = new PercentageRate();
            this.YieldToWorstRate = new PercentageRate();
            this.YieldToNextCallRate = new PercentageRate();
        }
        public DebtYield(double? YieldToMaturityRate = null, double? YieldToWorstRate = null,
            double? YieldToNextCallRate = null)
        {
            this.YieldToMaturityRate = new PercentageRate(YieldToMaturityRate);
            this.YieldToWorstRate = new PercentageRate(YieldToWorstRate);
            this.YieldToNextCallRate = new PercentageRate(YieldToNextCallRate);
        }
        /// <summary>
        /// Rate of return anticipated on a bond when held until maturity date.
        /// </summary>
        public PercentageRate YieldToMaturityRate { get; set; }

        /// <summary>
        /// Worst Rate of return possible on a bond among all redemption scenariis.
        /// </summary>
        public PercentageRate YieldToWorstRate { get; set; }

        /// <summary>
        /// Rate of return if the bond is called at the next call date
        /// </summary>
        public PercentageRate YieldToNextCallRate { get; set; }
    }


    /// <summary>
    /// Yields and Performance datas
    /// </summary>
    [ComplexType]
    public class Yield
    {
        public Yield()
        {
            this.ChangePrice_1D = new PercentageRate();
            this.ChangePrice_MTD = new PercentageRate();
            this.ChangePrice_YTD = new PercentageRate();
        }

        public Yield(double? ChangePrice_1D = null,
            double? ChangePrice_MTD = null,
            double? ChangePrice_YTD = null)
        {
            this.ChangePrice_1D = new PercentageRate(ChangePrice_1D);
            this.ChangePrice_MTD = new PercentageRate(ChangePrice_MTD);
            this.ChangePrice_YTD = new PercentageRate(ChangePrice_YTD);
        }

        /// <summary>
        /// CHG_PCT_1D
        /// Change of the price (PX_LAST -PX_CLOSE_1D)/PX_CLOSE_1D * 100
        /// </summary>
        public PercentageRate ChangePrice_1D { get; set; }

        /// <summary>
        /// CHG_PCT_MTD
        /// Change of the price (CHG_NET_MTD)/PX_CLOSE_MTD * 100
        /// </summary>
        public PercentageRate ChangePrice_MTD { get; set; }

        /// <summary>
        /// CHG_PCT_YTD
        /// Change of the price (CHG_NET_YTD)/PX_CLOSE_YTD * 100
        /// </summary>
        public PercentageRate ChangePrice_YTD { get; set; }

    }
}
