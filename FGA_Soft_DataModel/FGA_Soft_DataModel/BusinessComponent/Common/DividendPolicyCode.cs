using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{  
    /// <summary>
    /// Classe de persistance qui correspond à un Adapteur (pattern Adapter)
    /// Ce qui permet d utiliter l entity framework avec des données représentants des CODES-LIBELLES   
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class DividendPolicyCode
    {
        private DividendPolicyCodeAdaptee InternalObject;

        public DividendPolicyCode()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">code as A for annual, D for daily ...</param>
        public DividendPolicyCode(string c)
        {
            this.DividendPolicy = c;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">frequency as 1 for annual, 2 for semi-annual ...</param>
        public DividendPolicyCode(int c)
        {
            DividendPolicyCodeAdaptee result;
            DividendPolicyCodeAdaptee.Codes.TryGetValue(c, out result);
            if( result != null)
                this.InternalObject = result;
            else
                throw new InvalidCastException();
        }

        private DividendPolicyCode(DividendPolicyCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        [MaxLength(1),Column(TypeName="char")]
        public string DividendPolicy
        {
            get { return (InternalObject!=null?InternalObject.Value:null); }
            set
            {
                if (value == null)
                    return;
                DividendPolicyCodeAdaptee result;
                // recherche d un caractere correspondant à l objet FrequencyCode
                if (DividendPolicyCodeAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    // recherche d une constante correspondant à l objet FrequencyCode
                    if (DividendPolicyCodeAdaptee.Constants.TryGetValue(value, out result))
                        this.InternalObject = result;
                    else
                        throw new InvalidCastException("DividendPolicyCode inexistant :" + value);   // code qui n existe pas                  
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator DividendPolicyCode(string str)
        {
            if (str == null) return null;
            DividendPolicyCodeAdaptee result;
            if (DividendPolicyCodeAdaptee.Instances.TryGetValue(str, out result))
                return new DividendPolicyCode(result);
            else
                throw new InvalidCastException();
        }

        public static explicit operator DividendPolicyCode(int i)
        {
            DividendPolicyCodeAdaptee result;
            if (DividendPolicyCodeAdaptee.Codes.TryGetValue(i, out result))
                return new DividendPolicyCode(result);
            else
                throw new InvalidCastException();
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public static DividendPolicyCode getFrequencyByLabel(string label)
        {
            return new DividendPolicyCode(DividendPolicyCodeAdaptee.Constants[label]);
        }

        public override bool Equals(object obj)
        {
            DividendPolicyCode code = obj as DividendPolicyCode;
            return this.Equals(code);
        }

        public bool Equals(DividendPolicyCode code)
        {
            if (code == null || code.InternalObject ==null || this.InternalObject ==null )
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }
        public override int GetHashCode()
        {
            return this.InternalObject.Code;
        }
        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static DividendPolicyCode()
        {
            //Dividend is paid daily and can be accrued.
            new DividendPolicyCodeAdaptee("DAILY_ACCRUING_DIVIDEND", "A", 0);
            new DividendPolicyCodeAdaptee("CASH", "C", 1);
            new DividendPolicyCodeAdaptee("UNITS", "U", 2);
            //Dividend is paid in both Cash and Units.
            new DividendPolicyCodeAdaptee("BOTH", "B", 3);
        }
        #endregion

    }

    /// <summary>
    /// Represente une devise
    /// </summary>
    [NotMapped]
    internal sealed class DividendPolicyCodeAdaptee
    {
        internal readonly string Label;
        internal readonly string Value;
        internal readonly int Code;

            /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static DividendPolicyCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }
        internal DividendPolicyCodeAdaptee(string Label, string Value, int Code = 0)
        {
            this.Label = Label;
            this.Code = Code;
            this.Value = Value;
            Instances[Value] = this;
            Constants[Label] = this;
            Codes[Code] = this;

        }
        public override String ToString()
        {
            return Value;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to DividendPolicyCodeAdaptee return false.
            DividendPolicyCodeAdaptee p = obj as DividendPolicyCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }
     
        #region Singleton partie statique
        internal static readonly Dictionary<string, DividendPolicyCodeAdaptee> Instances = new Dictionary<string, DividendPolicyCodeAdaptee>();
        internal static readonly Dictionary<string, DividendPolicyCodeAdaptee> Constants = new Dictionary<string, DividendPolicyCodeAdaptee>();
        internal static readonly Dictionary<int, DividendPolicyCodeAdaptee> Codes = new Dictionary<int, DividendPolicyCodeAdaptee>();
        #endregion



    }
}
