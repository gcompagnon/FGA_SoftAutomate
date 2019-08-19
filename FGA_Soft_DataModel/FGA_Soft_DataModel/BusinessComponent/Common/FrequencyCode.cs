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
    public sealed class FrequencyCode
    {
        private FrequencyCodeAdaptee InternalObject;

        public FrequencyCode()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">code as A for annual, D for daily ...</param>
        public FrequencyCode(string c)
        {
            this.IndexFrequency = c;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">frequency as 1 for annual, 2 for semi-annual ...</param>
        public FrequencyCode(int c)
        { 
            FrequencyCodeAdaptee result;
            FrequencyCodeAdaptee.Codes.TryGetValue(c, out result);
            if( result != null)
                this.InternalObject = result;
            else
                throw new InvalidCastException();
        }

        private FrequencyCode(FrequencyCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        [MaxLength(1),Column(TypeName="char")]
        public string IndexFrequency
        {
            get { return (InternalObject!=null?InternalObject.Value:null); }
            set
            {
                if (value == null)
                    return;
                FrequencyCodeAdaptee result;
                // recherche d un caractere correspondant à l objet FrequencyCode
                if (FrequencyCodeAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    // recherche d une constante correspondant à l objet FrequencyCode
                    if (FrequencyCodeAdaptee.Constants.TryGetValue(value, out result))
                        this.InternalObject = result;
                    else
                        throw new InvalidCastException("FrequencyCode inexistant :" + value);   // code qui n existe pas                  
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator FrequencyCode(string str)
        {
            if (str == null) return null;
            FrequencyCodeAdaptee result;
            if (FrequencyCodeAdaptee.Instances.TryGetValue(str, out result))
                return new FrequencyCode(result);
            else
                throw new InvalidCastException();
        }

        public static explicit operator FrequencyCode(int i)
        {
            FrequencyCodeAdaptee result;
            if (FrequencyCodeAdaptee.Codes.TryGetValue(i, out result))
                return new FrequencyCode(result);
            else
                throw new InvalidCastException();
        }

        public static explicit operator int(FrequencyCode f)
        {
            int code = f.InternalObject.Code;
            if( code == 0)
                throw new InvalidCastException();
            return code;                
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public override bool Equals(object obj)
        {
            FrequencyCode code = obj as FrequencyCode;
            return this.Equals(code);
        }

        public bool Equals(FrequencyCode code)
        {
            if (code == null || code.InternalObject == null || this.InternalObject == null)
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }

        public override int GetHashCode()
        {
            if (this.InternalObject == null)
                return 0;
            else
                return this.InternalObject.Value.GetHashCode();
        }

        public static FrequencyCode getFrequencyByLabel(string label)
        {
            return new FrequencyCode(FrequencyCodeAdaptee.Constants[label]);
        }
       
        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static FrequencyCode()
        {
            new FrequencyCodeAdaptee("DAILY", "D",365);
            new FrequencyCodeAdaptee("WEEKLY", "W",52);
            new FrequencyCodeAdaptee("QUARTERLY", "Q",4);
            new FrequencyCodeAdaptee("ANNUAL", "A",1);
            new FrequencyCodeAdaptee("MONTHLY", "M",12);
            new FrequencyCodeAdaptee("SEMI ANNUAL", "S",2);
            new FrequencyCodeAdaptee("ADHOC", "H");
            new FrequencyCodeAdaptee("INTRADAY", "I");
            new FrequencyCodeAdaptee("OVERNIGHT", "O");
            new FrequencyCodeAdaptee("TEN DAYS", "T");
        }
        #endregion


    }
    /// <summary>
    /// Represente une fréquence: DAILY/quotidien, WEEKLY/hebdomadaire, MONTHLY/mensuel, YEARLY/annuel ...
    /// </summary>
    [NotMapped]
    internal sealed class FrequencyCodeAdaptee
    {
        internal readonly string Label;
        internal readonly string Value;
        internal readonly int Code;

        /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static FrequencyCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }

        internal FrequencyCodeAdaptee(string Label, string Value, int Code = 0)
        {
            this.Label = Label;
            this.Code = Code;
            this.Value = Value;
            Instances[Value] = this;
            Constants[Label] = this;
            Codes[Code] = this;

        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to FrequencyCodeAdaptee return false.
            FrequencyCodeAdaptee p = obj as FrequencyCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }

        #region Singleton partie statique
        internal static readonly Dictionary<string, FrequencyCodeAdaptee> Instances = new Dictionary<string, FrequencyCodeAdaptee>();
        internal static readonly Dictionary<string, FrequencyCodeAdaptee> Constants = new Dictionary<string, FrequencyCodeAdaptee>();
        internal static readonly Dictionary<int, FrequencyCodeAdaptee> Codes = new Dictionary<int, FrequencyCodeAdaptee>();
        #endregion
    }
}
