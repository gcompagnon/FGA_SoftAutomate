using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FGABusinessComponent.BusinessComponent.Common
{
    /// <summary>
    /// Seniority Levels for debt
    /// Classe de persistance qui correspond à un Adapteur (pattern Adapter)
    /// Ce qui permet d utiliter l entity framework avec des données représentants des CODES-LIBELLES
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class SeniorityLevelCode
    {
        private SeniorityLevelAdaptee InternalObject;

        public SeniorityLevelCode()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">seniority code</param>
        public SeniorityLevelCode(string c)
        {
            this.SeniorityLevel = c;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">seniority code</param>
        public SeniorityLevelCode(int c)
        {
            SeniorityLevelAdaptee result;
            SeniorityLevelAdaptee.Codes.TryGetValue(c, out result);
            if (result != null)
                this.InternalObject = result;
            else
                throw new InvalidCastException();
        }

        private SeniorityLevelCode(SeniorityLevelAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        [MaxLength(13), Column(TypeName = "char")]
        public string SeniorityLevel
        {
            get { return (InternalObject != null ? InternalObject.Value : null); }
            set
            {
                if (value == null)
                    return;
                value = value.TrimEnd(' ');
                SeniorityLevelAdaptee result;
                // recherche d un caractere correspondant à l objet 
                if (SeniorityLevelAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    // recherche d une constante correspondant à l objet 
                    if (SeniorityLevelAdaptee.Constants.TryGetValue(value, out result))
                        this.InternalObject = result;
                    else
                        throw new InvalidCastException("SeniorityLevel inexistant :" + value);   // code qui n existe pas                  
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator SeniorityLevelCode(string str)
        {
            if (str == null) return null;
            SeniorityLevelAdaptee result;
            if (SeniorityLevelAdaptee.Instances.TryGetValue(str, out result))
                return new SeniorityLevelCode(result);
            else
                throw new InvalidCastException();
        }

        public static explicit operator SeniorityLevelCode(int i)
        {
            SeniorityLevelAdaptee result;
            if (SeniorityLevelAdaptee.Codes.TryGetValue(i, out result))
                return new SeniorityLevelCode(result);
            else
                throw new InvalidCastException();
        }

        public override bool Equals(object obj)
        {
            SeniorityLevelCode code = obj as SeniorityLevelCode;
            return this.Equals(code);
        }

        public bool Equals(SeniorityLevelCode code)
        {
            if (code == null || code.InternalObject == null || this.InternalObject == null)
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }

        public override int GetHashCode()
        {
            if (this.InternalObject == null)
                return 0;
            return this.InternalObject.Code;
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public static SeniorityLevelCode getSeniorityLevelByLabel(string label)
        {
            try
            {
                return new SeniorityLevelCode(SeniorityLevelAdaptee.Constants[label]);
            }
            catch (KeyNotFoundException knfe)
            {
                throw new InvalidCastException("The Code " + label + " does not refer to a SeniorityLevel code");
            }
        }
        #region methodes fonctionnelles
        public bool isSubordinated()
        {
            return this.InternalObject.Category == "SUB";
        }
        public bool isSenior()
        {
            return this.InternalObject.Category == "SEN";
        }
        #endregion

        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static SeniorityLevelCode()
        {
            SeniorityLevelAdaptee adapteeA = new SeniorityLevelAdaptee("SENIOR", "SEN", 0, "SEN");
            SENIOR = new SeniorityLevelCode(adapteeA);
            SeniorityLevelAdaptee adapteeO = new SeniorityLevelAdaptee("Subordinated Tiers 1", "SUB_T1", 1, "SUB", "T1","T1");
            SUB_T1 = new SeniorityLevelCode(adapteeO);
            SeniorityLevelAdaptee adapteeE = new SeniorityLevelAdaptee("Subordinated Upper Tiers 2", "SUB_UT2", 2, "SUB", "UT2","UT2");
            SUB_UT2 = new SeniorityLevelCode(adapteeE);
            SeniorityLevelAdaptee adapteeD = new SeniorityLevelAdaptee("Subordinated Lower Tiers 2", "SUB_LT2", 3, "SUB", "LT2","LT2");
            SUB_LT2 = new SeniorityLevelCode(adapteeD);
            SeniorityLevelAdaptee adapteeM = new SeniorityLevelAdaptee("Subordinated others", "SUB_OTHER", 4, "SUB", "OTHER","OTHER");
            SUB_OTHER = new SeniorityLevelCode(adapteeM);

            new SeniorityLevelAdaptee("Insurance Sub Tiers 1", "ISUB_T1", 5, "SUB", "T1", "iT1");
            new SeniorityLevelAdaptee("Insurance Sub Upper Tiers 2", "ISUB_UT2", 6, "SUB", "UT2", "iUT2");
            new SeniorityLevelAdaptee("Insurance Sub Lower Tiers 2", "ISUB_LT2", 7, "SUB", "LT2", "iLT2");

            new SeniorityLevelAdaptee("Subordinated Tiers 1 Step", "SUB_T1_STEP", 8, "SUB", "T1", "T1 step");
            new SeniorityLevelAdaptee("Subordinated Tiers 1 Non Step", "SUB_T1_NSTEP", 9, "SUB", "T1", "T1 non-step");
            new SeniorityLevelAdaptee("Subordinated LT2 Callable", "SUB_LT2_CALL", 10, "SUB", "LT2", "LT2 callable");
            
            new SeniorityLevelAdaptee("Subordinated LT2 Non Callable", "SUB_LT2_NCALL", 11, "SUB", "LT2", "LT2 non-callable");
            new SeniorityLevelAdaptee("Insurance Sub Lower Tiers 2 callable", "ISUB_LT2_CALL", 12, "SUB", "LT2", "iLT2 callable");
            new SeniorityLevelAdaptee("Insurance Sub Lower Tiers 2 non callable", "ISUB_LT2_NCAL", 13, "SUB", "LT2", "iLT2 non-callable");
            new SeniorityLevelAdaptee("T2 perpetual", "SUB_T2_PERP", 14, "SUB", "T2", "T2 perpetual");
            new SeniorityLevelAdaptee("T1 perpetual", "SUB_T1_PERP", 15, "SUB", "T1", "T1 perpetual");
            new SeniorityLevelAdaptee("LT2 perpetual", "SUB_LT2_PERP", 16, "SUB", "LT2", "LT2 perpetual");
            new SeniorityLevelAdaptee("LT1 perpetual", "SUB_LT1_PERP", 17, "SUB", "LT1", "LT1 perpetual");
            new SeniorityLevelAdaptee("T2 dated callable", "SUB_T2_CALL", 18, "SUB", "T2", "T2 dated callable");
            new SeniorityLevelAdaptee("T2 dated non-callable", "SUB_T2_NCALL", 18, "SUB", "T2", "T2 dated non-callable");
            
        }
        public static SeniorityLevelCode SENIOR;
        public static SeniorityLevelCode SUB_T1;
        public static SeniorityLevelCode SUB_UT2;
        public static SeniorityLevelCode SUB_LT2;
        public static SeniorityLevelCode SUB_OTHER;
        #endregion

    }
    /// <summary>
    /// Represente une code pour le Financial AssetType
    /// </summary>
    [NotMapped]
    internal sealed class SeniorityLevelAdaptee
    {
        internal readonly string Label;
        internal readonly string Value;
        internal readonly int Code;
        internal readonly string Category;
        internal readonly string SubCategory;


        /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static SeniorityLevelAdaptee()
        {
            //Console.WriteLine("Chargement de SeniorityLevelAdaptee");
        }

        internal SeniorityLevelAdaptee(string Label, string Value, int Code = 0, string Category = null, string SubCategory = null, params string[] keywords)
        {
            this.Category = Category;
            this.SubCategory = SubCategory;
            this.Label = Label;
            this.Code = Code;
            this.Value = Value;
            Instances[Value] = this;
            Constants[Label] = this;
            Codes[Code] = this;

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

            // If parameter cannot be cast to FinancialAssetTypeCategoryCodeAdaptee return false.
            SeniorityLevelAdaptee p = obj as SeniorityLevelAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }
        public override int GetHashCode()
        {
            return this.Code;
        }

        #region Singleton partie statique
        internal static readonly Dictionary<string, SeniorityLevelAdaptee> Instances = new Dictionary<string, SeniorityLevelAdaptee>();
        internal static readonly Dictionary<string, SeniorityLevelAdaptee> Constants = new Dictionary<string, SeniorityLevelAdaptee>(StringComparer.OrdinalIgnoreCase);
        internal static readonly Dictionary<int, SeniorityLevelAdaptee> Codes = new Dictionary<int, SeniorityLevelAdaptee>();
        #endregion
    }
}
