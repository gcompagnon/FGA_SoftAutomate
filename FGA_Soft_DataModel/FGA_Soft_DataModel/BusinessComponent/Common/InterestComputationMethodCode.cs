using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{
    /// <summary>
    /// Specifies the method used to compute accruing interest of a financial instrument.
    /// Classe de persistance qui correspond à un Adapteur (pattern Adapter)
    /// Ce qui permet d utiliter l entity framework avec des données représentants des CODES-LIBELLES
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class InterestComputationMethodCode
    {
        private InterestComputationMethodCodeAdaptee InternalObject;

        public InterestComputationMethodCode()
        {
        }
        public InterestComputationMethodCode(string c)
        {
            this.InterestComputationMethod = c;
        }
        private InterestComputationMethodCode(InterestComputationMethodCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        public string InterestComputationMethod
        {
            get { return (InternalObject != null ? InternalObject.Label : ""); }
            set
            {
                if (value == null)
                    return;
                InterestComputationMethodCodeAdaptee result;
                // recherche d un caractere correspondant à l objet InterestComputationMethodCode
                if (InterestComputationMethodCodeAdaptee.Constants.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                {
                    // recherche d une constante correspondant à l objet InterestComputationMethodCode
                    int codeInt = 0;
                    if (int.TryParse(value, out codeInt))
                    {
                        if (InterestComputationMethodCodeAdaptee.Instances.TryGetValue(codeInt, out result))
                            this.InternalObject = result;
                        else
                            throw new InvalidCastException("InterestComputationMethodCode inexistant :" + value);    // code qui n existe pas
                    }
                }
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator InterestComputationMethodCode(string str)
        {
            if (str == null) return null;
            InterestComputationMethodCodeAdaptee result;
            if (InterestComputationMethodCodeAdaptee.Constants.TryGetValue(str, out result))
                return new InterestComputationMethodCode(result);
            else
                throw new InvalidCastException();
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public override bool Equals(object obj)
        {
            InterestComputationMethodCode code = obj as InterestComputationMethodCode;
            return this.Equals(code);
        }

        public bool Equals(InterestComputationMethodCode code)
        {
            if (code == null || code.InternalObject  ==null || this.InternalObject  == null)
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }

        public override int GetHashCode()
        {
            if (this.InternalObject == null)
                return 0;
            else
                return this.InternalObject.Value;
        }
        public static InterestComputationMethodCode getCapitalTypeCodeByLabel(string label)
        {
            return new InterestComputationMethodCode(InterestComputationMethodCodeAdaptee.Constants[label]);
        }



        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static InterestComputationMethodCode()
        {
            new InterestComputationMethodCodeAdaptee("IC30360ISD_AOR30360_AMERICAN_BASIC_RULE", 0, "Method whereby interest is calculated based on a 30-day month and a 360-day year. Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month, except for February, and provided that the interest period started on a 30th or a 31st. This means that a 31st is assumed to be a 30th if the period started on a 30th or a 31st and the 28 Feb (or 29 Feb for a leap year) is assumed to be a 28th (or 29th). It is the most commonly used 30/360 method for US straight and convertible bonds.");
            new InterestComputationMethodCodeAdaptee("IC30365", 1, "Method whereby interest is calculated based on a 30-day month in a way similar to the 30/360 (basic rule) and a 365-day year. Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month, except for February. This means that a 31st is assumed to be a 30th and the 28 Feb (or 29 Feb for a leap year) is assumed to be a 28th (or 29th).",
                "30/365");
            new InterestComputationMethodCodeAdaptee("IC30_ACTUAL", 2, "Method whereby interest is calculated based on a 30-day month in a way similar to the 30/360 (basic rule) and the assumed number of days in a year in a way similar to the Actual/Actual (ICMA). Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month, except for February. This means that a 31st is assumed to be a 30th and the 28 Feb (or 29 Feb for a leap year) is assumed to be a 28th (or 29th). The assumed number of days in a year is computed as the actual number of days in the coupon period multiplied by the number of interest payments in the year.",
                "30/ACT");
            new InterestComputationMethodCodeAdaptee("ACTUAL360", 3, "Method whereby interest is calculated based on the actual number of accrued days in the interest period and a 360-day year.",
                "ACT/360");
            new InterestComputationMethodCodeAdaptee("ACTUAL365_FIXED", 4, "Method whereby interest is calculated based on the actual number of accrued days in the interest period and a 365-day year.",
                "ACT/365");
            new InterestComputationMethodCodeAdaptee("ACTUAL_ACTUAL_ICMA", 5, "Method whereby interest is calculated based on the actual number of accrued days and the assumed number of days in a year, ie, the actual number of days in the coupon period multiplied by the number of interest payments in the year. If the coupon period is irregular (first or last coupon), it is extended or split into quasi interest periods that have the length of a regular coupon period and the computation is operated separately on each quasi interest period and the intermediate results are summed up.",
                "ICMA ACT/ACT");
            new InterestComputationMethodCodeAdaptee("30E_360", 6, "Method whereby interest is calculated based on a 30-day month and a 360-day year. Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month. This means that a 31st is assumed to be a 30th and the 28 Feb (or 29 Feb for a leap year) is assumed to be equivalent to a 30 Feb. However, if the last day of the maturity coupon period is the last day of February, it will not be assumed to be a 30th. It is a variation of the 30/360 (ICMA) method commonly used for eurobonds. The usage of this variation is only relevant when the coupon periods are scheduled to end on the last day of the month.",
                "IC30E36_0OR_EURO_BOND_BASISMODEL1");

            new InterestComputationMethodCodeAdaptee("ACTUAL_ACTUAL_ISDA", 7, "Method whereby interest is calculated based on the actual number of accrued days of the interest period that fall (falling on a normal year, year) divided by 365, added to the actual number of days of the interest period that fall (falling on a leap year, year) divided by 366.",
                "ISDA ACT/ACT");
            new InterestComputationMethodCodeAdaptee("ACTUAL365_LOR_ACTU_ACTUBASIS_RULE", 8, "Method whereby interest is calculated based on the actual number of accrued days and a 365-day year (if the coupon payment date is NOT in a leap year) or a 366-day year (if the coupon payment date is in a leap year).");
            new InterestComputationMethodCodeAdaptee("ACTUAL_ACTUAL_AFB", 9, "Method whereby interest is calculated based on the actual number of accrued days and a 366-day year (if 29 Feb falls in the coupon period) or a 365-day year (if 29 Feb does not fall in the coupon period). If a coupon period is longer than one year, it is split by repetitively separating full year sub-periods counting backwards from the end of the coupon period (a year backwards from a 28 Feb being 29 Feb, if it exists). The first of the sub-periods starts on the start date of the accrued interest period and thus is possibly shorter than a year. Then the interest computation is operated separately on each sub-period and the intermediate results are summed up.",
                "AFB ACT/ACT");
            new InterestComputationMethodCodeAdaptee("IC30360ICM_AOR30360BASICRULE", 10, "Method whereby interest is calculated based on a 30-day month and a 360-day year. Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month, except for February. This means that a 31st is assumed to be a 30th and the 28 Feb (or 29 Feb for a leap year) is assumed to be a 28th (or 29th). It is the most commonly used 30/360 method for non-US straight and convertible bonds issued before 01/01/1999.");
            new InterestComputationMethodCodeAdaptee("IC30E236_0OR_EUROBONDBASISMODEL2", 11, "Method whereby interest is calculated based on a 30-day month and a 360-day year. Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month, except for the last day of February whose day of the month value shall be adapted to the value of the first day of the interest period if the latter is higher and if the period is one of a regular schedule. This means that a 31st is assumed to be a 30th and the 28th Feb of a non-leap year is assumed to be equivalent to a 29th Feb when the first day of the interest period is a 29th, or to a 30th Feb when the first day of the interest period is a 30th or a 31st. The 29th Feb of a leap year is assumed to be equivalent to a 30th Feb when the first day of the interest period is a 30th or a 31st. Similarly, if the coupon period starts on the last day of February, it is assumed to produce only one day of interest in February as if it was starting on a 30th Feb when the end of the period is a 30th or a 31st, or two days of interest in February when the end of the period is a 29th, or 3 days of interest in February when it is the 28th Feb of a non-leap year and the end of the period is before the 29th.");
            new InterestComputationMethodCodeAdaptee("IC30E336_0OR_EUROBONDBASISMODEL3", 12, "Method whereby interest is calculated based on a 30-day month and a 360-day year. Accrued interest to a value date on the last day of a month shall be the same as to the 30th calendar day of the same month. This means that a 31st is assumed to be a 30th and the 28 Feb (or 29 Feb for a leap year) is assumed to be equivalent to a 30 Feb. It is a variation of the 30E/360 (or Eurobond basis) method where the last day of February is always assumed to be a 30th, even if it is the last day of the maturity coupon period.");
            new InterestComputationMethodCodeAdaptee("ACTUAL365_NL", 13, "Method whereby interest is calculated based on the actual number of accrued days in the interest period, excluding any leap day from the count, and a 365-day year.");
            new InterestComputationMethodCodeAdaptee("NARRATIVE", 14, "Other method than A001-A014. See Narrative.");

            new InterestComputationMethodCodeAdaptee("ACTUAL_ACTUAL", 50, "Actual/Actual",
                "ACT/ACT");
            new InterestComputationMethodCodeAdaptee("ACTUAL_ACTUAL_NE", 51, "Actual / Actual Non End of Month. This indicates that a bond pays a cash flow not on the last day of the month but on a date 6 months after the 1st coupon date. For example if a bond paying semi-annually has a 1st coupon date of 09/30, the 2nd coupon date would be 03/30. If this bond was just ACT/ACT the 2nd coupon payment would be 03/31.",
                "ACT/ACT NE");
            new InterestComputationMethodCodeAdaptee("30_360_ISMA", 52, "ISMA 30/360",
                "ISMA 30/360");

        }
        #endregion

    }
    /// <summary>
    /// Represente une fréquence: DAILY/quotidien, WEEKLY/hebdomadaire, MONTHLY/mensuel, YEARLY/annuel ...
    /// </summary>
    [NotMapped]
    internal sealed class InterestComputationMethodCodeAdaptee
    {
        internal readonly string Label;
        internal readonly int Value;
        internal readonly string Description;

        /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static InterestComputationMethodCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }

        internal InterestComputationMethodCodeAdaptee(string Label, int Value, string Description, params string[] keywords)
        {
            this.Label = Label;
            this.Value = Value;
            this.Description = Description;
            Instances[Value] = this;
            Constants[Label] = this;

            foreach (string kw in keywords)
            {
                Constants[kw] = this;
            }
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to InterestComputationMethodCodeAdaptee return false.
            InterestComputationMethodCodeAdaptee p = obj as InterestComputationMethodCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }


        #region Singleton partie statique
        internal static readonly Dictionary<int, InterestComputationMethodCodeAdaptee> Instances = new Dictionary<int, InterestComputationMethodCodeAdaptee>();
        internal static readonly Dictionary<string, InterestComputationMethodCodeAdaptee> Constants = new Dictionary<string, InterestComputationMethodCodeAdaptee>();
        #endregion

    }
}
