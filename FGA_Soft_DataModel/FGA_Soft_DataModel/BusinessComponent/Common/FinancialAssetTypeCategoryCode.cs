using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{
    /// <summary>
    /// Categorization of financial asset type.
    /// Classe de persistance qui correspond à un Adapteur (pattern Adapter)
    /// Ce qui permet d utiliter l entity framework avec des données représentants des CODES-LIBELLES
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class FinancialAssetTypeCategoryCode
    {
         private FinancialAssetTypeCategoryCodeAdaptee InternalObject;

        public FinancialAssetTypeCategoryCode()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">code as A for annual, D for daily ...</param>
        public FinancialAssetTypeCategoryCode(string c)
        {
            this.FinancialAssetTypeCategory = c;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">frequency as 1 for annual, 2 for semi-annual ...</param>
        public FinancialAssetTypeCategoryCode(int c)
        {
            FinancialAssetTypeCategoryCodeAdaptee result;
            FinancialAssetTypeCategoryCodeAdaptee.Codes.TryGetValue(c, out result);
            if (result != null)
                this.InternalObject = result;
            else
                throw new InvalidCastException();
        }

        private FinancialAssetTypeCategoryCode(FinancialAssetTypeCategoryCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        [MaxLength(1), Column(TypeName = "char")]
        public string FinancialAssetTypeCategory
        {
            get { return (InternalObject != null ? InternalObject.Value : null); }
            set
            {
                if (value == null)
                    return;
                FinancialAssetTypeCategoryCodeAdaptee result;
                // recherche d un caractere correspondant à l objet FrequencyCode
                if (FinancialAssetTypeCategoryCodeAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    // recherche d une constante correspondant à l objet FrequencyCode
                    if (FinancialAssetTypeCategoryCodeAdaptee.Constants.TryGetValue(value, out result))
                        this.InternalObject = result;
                    else
                        throw new InvalidCastException("FinancialAssetTypeCategoryCode inexistant :" + value);   // code qui n existe pas                  
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator FinancialAssetTypeCategoryCode(string str)
        {
            if (str == null) return null;
            FinancialAssetTypeCategoryCodeAdaptee result;
            if (FinancialAssetTypeCategoryCodeAdaptee.Instances.TryGetValue(str, out result))
                return new FinancialAssetTypeCategoryCode(result);
            else
                throw new InvalidCastException();
        }

        public static explicit operator FinancialAssetTypeCategoryCode(int i)
        {
            FinancialAssetTypeCategoryCodeAdaptee result;
            if (FinancialAssetTypeCategoryCodeAdaptee.Codes.TryGetValue(i, out result))
                return new FinancialAssetTypeCategoryCode(result);
            else
                throw new InvalidCastException();
        }

        public override bool Equals(object obj)
        {
            FinancialAssetTypeCategoryCode code = obj as FinancialAssetTypeCategoryCode;
            return this.Equals(code);
        }

        public bool Equals(FinancialAssetTypeCategoryCode code)
        {
            if ( (code == null) || this.InternalObject == null )
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }
        public override int GetHashCode()
        {
            return this.InternalObject.Code;
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public static FinancialAssetTypeCategoryCode getFATByLabel(string label)
        {
            return new FinancialAssetTypeCategoryCode(FinancialAssetTypeCategoryCodeAdaptee.Constants[label]);
        }


        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static FinancialAssetTypeCategoryCode()
        {
            FinancialAssetTypeCategoryCodeAdaptee adapteeA= new FinancialAssetTypeCategoryCodeAdaptee("EQUITIES", "A", 0);
            EQUITIES = new FinancialAssetTypeCategoryCode(adapteeA);
            FinancialAssetTypeCategoryCodeAdaptee adapteeO=new FinancialAssetTypeCategoryCodeAdaptee("DEBT INSTRUMENTS", "O", 1);
            DEBT = new FinancialAssetTypeCategoryCode(adapteeO);
            FinancialAssetTypeCategoryCodeAdaptee adapteeE=new FinancialAssetTypeCategoryCodeAdaptee("ENTITLEMENTS", "E", 2);
            ENTITLEMENTS = new FinancialAssetTypeCategoryCode(adapteeE);
            FinancialAssetTypeCategoryCodeAdaptee adapteeD=new FinancialAssetTypeCategoryCodeAdaptee("DERIVATIVES", "D", 3);
            DERIVATIVES = new FinancialAssetTypeCategoryCode(adapteeD);
            FinancialAssetTypeCategoryCodeAdaptee adapteeM=new FinancialAssetTypeCategoryCodeAdaptee("MONEY_MARKET", "M", 4);
            MONEY_MARKET = new FinancialAssetTypeCategoryCode(adapteeM);
            FinancialAssetTypeCategoryCodeAdaptee adapteeZ=new FinancialAssetTypeCategoryCodeAdaptee("OTHERS", "Z", 5);
            OTHERS = new FinancialAssetTypeCategoryCode(adapteeZ);
        }
        public static FinancialAssetTypeCategoryCode EQUITIES;
        public static FinancialAssetTypeCategoryCode DEBT;
        public static FinancialAssetTypeCategoryCode ENTITLEMENTS;
        public static FinancialAssetTypeCategoryCode DERIVATIVES;
        public static FinancialAssetTypeCategoryCode MONEY_MARKET;
        public static FinancialAssetTypeCategoryCode OTHERS;

        #endregion

    }
    /// <summary>
    /// Represente une code pour le Financial AssetType
    /// </summary>
    [NotMapped]
    internal sealed class FinancialAssetTypeCategoryCodeAdaptee
    {
        internal readonly string Label;
        internal readonly string Value;
        internal readonly int Code;

        /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static FinancialAssetTypeCategoryCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }

        internal FinancialAssetTypeCategoryCodeAdaptee(string Label, string Value, int Code = 0)
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

            // If parameter cannot be cast to FinancialAssetTypeCategoryCodeAdaptee return false.
            FinancialAssetTypeCategoryCodeAdaptee p = obj as FinancialAssetTypeCategoryCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }

        #region Singleton partie statique
        internal static readonly Dictionary<string, FinancialAssetTypeCategoryCodeAdaptee> Instances = new Dictionary<string, FinancialAssetTypeCategoryCodeAdaptee>();
        internal static readonly Dictionary<string, FinancialAssetTypeCategoryCodeAdaptee> Constants = new Dictionary<string, FinancialAssetTypeCategoryCodeAdaptee>();
        internal static readonly Dictionary<int, FinancialAssetTypeCategoryCodeAdaptee> Codes = new Dictionary<int, FinancialAssetTypeCategoryCodeAdaptee>();
        #endregion
    }
}
