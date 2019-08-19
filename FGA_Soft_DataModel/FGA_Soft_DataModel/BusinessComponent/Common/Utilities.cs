using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{

    /// <summary>
    /// Représente l ensemble d un taux et d'un montant dans sa devise
    /// A savoir les complex type: possedent une limitation : il ne peuvent pas etre Null
    /// </summary>
    [ComplexType]
    public class RateAndAmount
    {
        public RateAndAmount()
        {
            this.Rate = new PercentageRate();
            this.Amount = new CurrencyAndAmount();
        }
        public RateAndAmount(PercentageRate rate = null, CurrencyAndAmount amount = null)
        {
            if (rate != null)
                this.Rate = rate;
            else
                this.Rate = new PercentageRate();
            if (amount != null)
                this.Amount = amount;
            else
                this.Amount = new CurrencyAndAmount();
        }
        PercentageRate Rate { get; set; }
        CurrencyAndAmount Amount { get; set; }

        public override string ToString()
        {
            if (Rate != null && Amount != null)
                return Amount.ToString() + " " + Rate.ToString();
            else if( Rate !=null)
                return Rate.ToString();
            else if (Amount != null)
                return Amount.ToString();
            return null;
        }
        public bool HasValue
        {
            get
            {
                return (Rate != null || Amount != null) ;
            }
        }
    }

    /// <summary>
    /// Un montant dans sa devise
    /// </summary>
    [ComplexType]
    public class CurrencyAndAmount
    {
        public CurrencyAndAmount()
        {
            this.Currency = new CurrencyCode();
        }
        public CurrencyAndAmount(double? value = null, CurrencyCode currency = null)
        {
            this.Value = value;
            if (currency != null)
                this.Currency = currency;
            else
                this.Currency = new CurrencyCode();
        }
        public double? Value { get; set; }
        public CurrencyCode Currency { get; set; }
        public override string ToString()
        {
            if (Value != null && Currency != null)
                return Value.ToString() + " " + Currency.ToString();
            else if (Value != null)
                return Value.ToString();
            return "";
        }
        public bool HasValue
        {
            get
            {
                return (Value != null || Currency.HasValue );
            }
        }
    }

    /// <summary>
    /// Un taux
    /// </summary>
    [ComplexType]
    public class PercentageRate
    {
        public PercentageRate()
        {
        }
        public PercentageRate(double? value)
        {
            this.Value = value;
        }
        public double? Value { get; set; }
        public override string ToString()
        {
            if (this.HasValue)
                return Value.ToString() + " %";
            else return "";
        }
        public bool HasValue
        {
            get
            {
                return (Value != null );
            }
        }
    }

    /// <summary>
    /// La quantité est exprimée en % , ou unité, ou montant
    /// </summary>
    [ComplexType]
    public class SecuritiesQuantity
    {
        public SecuritiesQuantity()
        {
            this.Rate = new PercentageRate();
            this.Amount = new CurrencyAndAmount();
        }
        public SecuritiesQuantity(int unit, PercentageRate rate = null, CurrencyAndAmount amount = null)
        {
            if (rate != null)
                this.Rate = rate;
            else
                this.Rate = new PercentageRate();
            
            this.Unit = unit;

            if(amount !=null)
                this.Amount = amount;
            else
                this.Amount = new CurrencyAndAmount();
        }
        // le code Open, UnknownQuantity ou AllSecurities
        //public QuantityCode Code { get; set; }
        public PercentageRate Rate { get; set; }
        public int? Unit { get; set; }
        public CurrencyAndAmount Amount { get; set; }
        public override string ToString()
        {
            if (this.Unit !=null && Rate != null && Amount != null)
                return Unit.ToString() + " "+ Amount.ToString() + " "+ Rate.ToString();
            else if( this.Unit !=null && Rate != null )
                return Unit.ToString() + " "+ Rate.ToString();
            else if( this.Unit !=null && Amount != null )
                return Unit.ToString() + " "+ Amount.ToString();
            else if( this.Rate !=null && Amount != null )
                return Amount.ToString() + " " + Rate.ToString();
            else if (this.Unit != null)
                return Unit.ToString();
            else if (this.Rate != null)
                return Rate.ToString();
            else if (Amount != null)
                return Amount.ToString();
            else return null;
        }
        public bool HasValue
        {
            get
            {
                return (Rate != null || Amount != null || Unit != null);
            }
        }

    }

    
}
