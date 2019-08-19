using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Core;
using System.ComponentModel.DataAnnotations.Schema;
using FGABusinessComponent.BusinessComponent.Util;

namespace FGABusinessComponent.BusinessComponent.Security
{
    public class Debt:Security
    {
        public Debt()
        {
            this.NextInterest = new InterestCalculation();
            this.FinancialInfos = new FinancialInfos();
            this.MinimumIncrement = new SecuritiesQuantity(1000);
        }
        /// <summary>
        /// Nouveau titre obligataire
        /// </summary>
        /// <param name="ISIN"></param>
        /// <param name="FinancialInstrumentName"></param>
        /// <param name="MaturityDate"></param>
        /// <param name="interestCoupon"></param>
        /// <param name="minimunIncrement">nominal/Par en unité et en devise , par défaut :1000</param>
        public Debt(String ISIN = null, string FinancialInstrumentName = null, DateTime? MaturityDate = null, InterestCalculation interestCoupon = null, SecuritiesQuantity minimunIncrement = null)
            :base(ISIN,FinancialInstrumentName,MaturityDate,FinancialAssetTypeCategoryCode.DEBT)
        {
            if( interestCoupon ==null)
                this.NextInterest = new InterestCalculation();
            else
                this.NextInterest = interestCoupon;

            this.FinancialInfos = new FinancialInfos();
            this.MinimumIncrement = minimunIncrement ?? new SecuritiesQuantity(1000);
        }

     
         /// <summary>
        /// Coupon information of the debt security.
        /// </summary>
        public InterestCalculation NextInterest { get; set; }

        /// <summary>
        /// Date/time at which the first interest payment is due to holders of the security.
        /// </summary>
        public DateTime? FirstPaymentDate { get; set; }

        /// <summary>
        /// Date at which the holder has the right to ask for redemption of the security prior to final maturity.
        /// </summary>
        public DateTime? PutableDate { get; set; }

        /// <summary>
        /// Indicates whether the holder has the right to ask for redemption of the security prior to final maturity. Also called RedeemableIndicator.
        /// </summary>
        public bool? PutableIndicator { get; set; }

        /// <summary>
        /// Indicates whether the security is a sinkung fund. A sinking fund is a bond in which part of the total principal amount is repaid according to agreed schedules of dates, amounts and prices.
        /// </summary>
        public bool? SinkableIndicator { get; set; }

        /// <summary>
        /// Indicates whether the interest rate is fixed but will be Variable
        /// </summary>        
        public bool? FixedToVariableIndicator { get; set; }

        /// <summary>
        /// Indicates whether the interest rate of an interest bearing instrument is reset periodically.
        /// </summary>        
        public bool? VariableRateIndicator { get; set; }

        /// <summary>
        /// Next date/time at which the issuer has the right to pay the securitiy prior to maturity.
        /// </summary>
        public DateTime? NextCallableDate { get; set; }

        /// <summary>
        /// Indicates whether the security has no maturity date.
        /// </summary>
        public bool? PerpetualIndicator { get; set; }
        /// <summary>
        /// Indicates the minimum denomination of a security.
        /// nominal/Par en devise
        /// </summary>
        public SecuritiesQuantity MinimumIncrement { get; set; }

        /// <summary>
        /// Gives all characteristic concerning Financial:
        /// Seniority Levels
        /// </summary>
        public FinancialInfos FinancialInfos { get; set; }

        #region Utilitaires/ methodes d instances
        /// <summary>
        /// Give if the static data are valid or not
        /// </summary>
        /// <returns></returns>
        public bool isStaticDataValid()
        {
            if (this.FinancialInstrumentName == null ||
                this.MaturityDate == null ||
                this.NextInterest.ToString() == null)
                return false;
            return true;
        }
        #endregion

    }
    #region CONFIGURATION MAPPING

    public class DebtModelConfiguration : EntityTypeConfiguration<Debt>
    {
        public DebtModelConfiguration()
        {
            Property(x => x.NextInterest.CalculationFrequency.IndexFrequency).HasColumnName("NextInterest_CalcFreq");
            Property(x => x.NextInterest.Rate.Value).HasColumnName("NextInterest_Rate");
            Property(x => x.MinimumIncrement.Unit).HasColumnName("MinIncrement");
            ToTable("DEBT", Asset.SHEMA_NAME);
            //Map<Debt>(m =>
            //{                
            //    //m.MapInheritedProperties(); // TPC : ne fonctionne pas avec le polymorphisme
            //    //m.Properties(d => new { d.EffectiveDate });
            //    Property(x => x.NextInterest.CalculationFrequency.IndexFrequency).HasColumnName("NextInterest_CalcFreq");
            //    Property(x => x.NextInterest.Rate.Value).HasColumnName("NextInterest_Rate");
            //    m.ToTable("DEBT", Asset.SHEMA_NAME);
            //});

        }
    }
    #endregion

    /// <summary>
    /// Set of parameters used to calculate an interest amount.
    /// </summary>
    [ComplexType]
    public class InterestCalculation
    {

        public InterestCalculation()
        {
            this.CalculationFrequency = new FrequencyCode("S");
        }

            public InterestCalculation(PercentageRate rate = null, FrequencyCode CalculationFrequency = null,
                InterestComputationMethodCode DayCountBasis =null)
        {
                this.Rate = rate;
                this.CalculationFrequency = CalculationFrequency;
            if (CalculationFrequency == null)
                this.CalculationFrequency = new FrequencyCode("S");
            if (DayCountBasis == null)
                this.DayCountBasis = new InterestComputationMethodCode();
            else
                this.DayCountBasis = DayCountBasis;

        }
        /// <summary>
        /// Specifies the periodicity of the calculation of the interest.
        /// </summary>
        [Required]
        public FrequencyCode CalculationFrequency { get; set; }
        ///
        /// Percentage charged for the use of an amount of money, usually expressed at an annual rate. The interest rate is the ratio of the amount of interest paid during a certain period of time compared to the principal amount of the interest bearing financial instrument.
        [Required]
        public PercentageRate Rate { get; set; }

        /// <summary>
        /// Identifies the computation method of accrued interest of the related financial instrument.
        /// </summary>
        public InterestComputationMethodCode DayCountBasis { get; set; }

        /// <summary>
        /// Specifies whether the interest is simple or compounded.
        /// </summary>
        //public CalculationMethodCode CalculationMethod { get; set; }

        /// <summary>
        /// Indicates whether the interest will be settled in cash or rolled in the existing collateral balance.
        /// </summary>
        //public InterestMethodCode InterestMethod { get; set; }

        /// <summary>
        /// Period during which the interest rate has been applied.
        /// </summary>
        //public DateTimePeriod InterestPeriod { get; set; }

        /// <summary>
        /// Specifies the type of interest.
        /// </summary>
        //public InterestCode InterestType { get; set; }
        ///Index rate related to the interest rate of the forthcoming interest payment.
        //public PercentageRate RelatedIndex { get; set; }

        /// <summary>
        /// Indicates the calculation date of the interest amount.
        /// </summary>
        //public DateTime? CalculationDate { get; set; }


        /// <summary>
        ///Underlying reason for the interest, eg, yearly credit interest on a savings account.
        /// </summary>
        //public string Reason { get; set; }

        public override string ToString()
        {
            if( Rate !=null)
                return Rate.ToString();
            return null;
        }
    }


    /// <summary>
    /// all informations concerning the :
    /// Seniority Levels
    /// Insurance covenants ...
    /// </summary>
    [ComplexType]
    public class FinancialInfos
    {

        public FinancialInfos()
        {
            this.Seniority = new SeniorityLevelCode();
        }

        public FinancialInfos(SeniorityLevelCode seniorityLevel)
        {
            this.Seniority = seniorityLevel;
        }

        /// <summary>
        /// Level for seniority of financial corp bonds
        /// </summary>
        public SeniorityLevelCode Seniority { get; set; }

        /// <summary>
        /// Hybrid bank capital indicator
        /// </summary>
        public bool? HybridCapital { get; set; }

        /// <summary>
        /// Indicates whether the security is a subordinated security.
        /// </summary>
        [NotMapped]
        public bool SubordinatedIndicator
        {
            get
            {
                return Seniority.isSubordinated();
            }
        }
        /// <summary>
        /// Indicates whether the security is a subordinated security.
        /// </summary>
        [NotMapped]
        public bool SeniorIndicator
        {
            get
            {
                return Seniority.isSenior();
            }
        }
     
        public override string ToString()
        {
            if (Seniority != null)
                return Seniority.ToString();
            return null;
        }
    }
}
