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
    public sealed class DistributionPolicyCode
    {
        private DistributionPolicyCodeAdaptee InternalObject;

        public DistributionPolicyCode()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">code as A for annual, D for daily ...</param>
        public DistributionPolicyCode(string c)
        {
            this.DistributionPolicy = c;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">frequency as 1 for annual, 2 for semi-annual ...</param>
        public DistributionPolicyCode(int c)
        {
            DistributionPolicyCodeAdaptee result;
            DistributionPolicyCodeAdaptee.Codes.TryGetValue(c, out result);
            if( result != null)
                this.InternalObject = result;
            else
                throw new InvalidCastException();
        }

        private DistributionPolicyCode(DistributionPolicyCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        [MaxLength(1),Column(TypeName="char")]
        public string DistributionPolicy
        {
            get { return (InternalObject!=null?InternalObject.Value:null); }
            set
            {
                if (value == null)
                    return;
                DistributionPolicyCodeAdaptee result;
                // recherche d un caractere correspondant à l objet FrequencyCode
                if (DistributionPolicyCodeAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    // recherche d une constante correspondant à l objet FrequencyCode
                    if (DistributionPolicyCodeAdaptee.Constants.TryGetValue(value, out result))
                        this.InternalObject = result;
                    else
                        throw new InvalidCastException("DistributionPolicyCode inexistant :" + value);   // code qui n existe pas                  
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator DistributionPolicyCode(string str)
        {
            if (str == null) return null;
            DistributionPolicyCodeAdaptee result;
            if (DistributionPolicyCodeAdaptee.Instances.TryGetValue(str, out result))
                return new DistributionPolicyCode(result);
            else
                throw new InvalidCastException();
        }

        public static explicit operator DistributionPolicyCode(int i)
        {
            DistributionPolicyCodeAdaptee result;
            if (DistributionPolicyCodeAdaptee.Codes.TryGetValue(i, out result))
                return new DistributionPolicyCode(result);
            else
                throw new InvalidCastException();
        }


        public override String ToString()
        {
            return InternalObject.Label;
        }

        public static DistributionPolicyCode getFrequencyByLabel(string label)
        {
            return new DistributionPolicyCode(DistributionPolicyCodeAdaptee.Constants[label]);
        }

        public override bool Equals(object obj)
        {
            DistributionPolicyCode code = obj as DistributionPolicyCode;
            return this.Equals(code);
        }

        public bool Equals(DistributionPolicyCode code)
        {
            if (code == null || code.InternalObject ==null || this.InternalObject  == null)
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
        static DistributionPolicyCode()
        {
            new DistributionPolicyCodeAdaptee("DISTRIBUTION","D", 0);
            new DistributionPolicyCodeAdaptee("ACCUMULATION","A", 1);
        }
        #endregion

    }

    /// <summary>
    /// Represente une devise
    /// </summary>
    [NotMapped]
    internal sealed class DistributionPolicyCodeAdaptee
    {
        internal readonly string Label;
        internal readonly string Value;
        internal readonly int Code;

            /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static DistributionPolicyCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }
        internal DistributionPolicyCodeAdaptee(string Label, string Value, int Code = 0)
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

            // If parameter cannot be cast to DistributionPolicyCodeAdaptee return false.
            DistributionPolicyCodeAdaptee p = obj as DistributionPolicyCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }

     
        #region Singleton partie statique
        internal static readonly Dictionary<string, DistributionPolicyCodeAdaptee> Instances = new Dictionary<string, DistributionPolicyCodeAdaptee>();
        internal static readonly Dictionary<string, DistributionPolicyCodeAdaptee> Constants = new Dictionary<string, DistributionPolicyCodeAdaptee>();
        internal static readonly Dictionary<int, DistributionPolicyCodeAdaptee> Codes = new Dictionary<int, DistributionPolicyCodeAdaptee>();
        #endregion



    }
}
