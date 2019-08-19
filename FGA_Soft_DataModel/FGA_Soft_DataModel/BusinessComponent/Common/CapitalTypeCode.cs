using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{
    /// <summary>
    /// Specifies the type of capital.
    /// Classe de persistance qui correspond à un Adapteur (pattern Adapter)
    /// Ce qui permet d utiliter l entity framework avec des données représentants des CODES-LIBELLES
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class CapitalTypeCode
    {

        private CapitalTypeCodeAdaptee InternalObject;

        public CapitalTypeCode()
        {
        }
        public CapitalTypeCode(string c)
        {
            this.CapitalType = c;
        }
        private CapitalTypeCode(CapitalTypeCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        public string CapitalType
        {
            get { return (InternalObject != null ? InternalObject.Label : ""); }
            set
            {
                if (value == null)
                    return;
                CapitalTypeCodeAdaptee result;
                // recherche d un caractere correspondant à l objet CapitalTypeCode
                if (CapitalTypeCodeAdaptee.Constants.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                {
                    // recherche d une constante correspondant à l objet CapitalTypeCode
                    int codeInt = 0;
                    if (int.TryParse(value, out codeInt))
                    {
                        if (CapitalTypeCodeAdaptee.Instances.TryGetValue(codeInt, out result))
                            this.InternalObject = result;
                        else
                            throw new InvalidCastException("CapitalTypeCode inexistant :" + value);    // code qui n existe pas
                    }
                }
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator CapitalTypeCode(string str)
        {
            if (str == null) return null;
            CapitalTypeCodeAdaptee result;
            if (CapitalTypeCodeAdaptee.Constants.TryGetValue(str, out result))
                return new CapitalTypeCode(result);
            else
                throw new InvalidCastException();
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public override bool Equals(object obj)
        {
            CapitalTypeCode code = obj as CapitalTypeCode;
            return this.Equals(code);
        }

        public override int GetHashCode()
        {
            if (this.InternalObject == null)
                return 0;
            else
                return this.InternalObject.Value;
        }

        public bool Equals(CapitalTypeCode code)
        {
            if (code == null || code.InternalObject ==null || this.InternalObject == null)
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }


        public static CapitalTypeCode getCapitalTypeCodeByLabel(string label)
        {
            return new CapitalTypeCode(CapitalTypeCodeAdaptee.Constants[label]);
        }


        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static CapitalTypeCode()
        {
            APPROVED = new CapitalTypeCode(
                new CapitalTypeCodeAdaptee("APPROVED", 0, "Approved capital."));
            ISSUED_VOTING_RIGHTS = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("ISSUED_VOTING_RIGHTS", 1, "Issued voting rights."));
            MAXIMUM_INCREASE = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("MAXIMUM_INCREASE", 2, "Maximum amount of increase."));
            OUTSTANDING = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("OUTSTANDING", 3, "Outstanding capital."));
            REDEEMED = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("REDEEMED", 4, "Redeemed capital."));
            STATED_CAPITAL = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("STATED_CAPITAL", 5, "Stated capital."));
            TREASURY_STOCK = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("TREASURY_STOCK", 6, "Issued reserve capital."));
            UNISSUED = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("UNISSUED", 7, "Contingent capital."));
            WITHDRAWN = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("WITHDRAWN", 8, " Withdrawn capital."));
            AUTHORISED = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("AUTHORISED", 9, "Authorised capital."));
            IN_CIRCULATION = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("IN_CIRCULATION", 10, "In circulation."));
            ISSUED = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("ISSUED", 11, "Issued capital."));
            ISSUED_RESERVE_CAPITAL = new CapitalTypeCode(
            new CapitalTypeCodeAdaptee("ISSUED_RESERVE_CAPITAL", 12, "Issued reserve capital."));
        }
        #endregion

        public static CapitalTypeCode APPROVED;
        public static CapitalTypeCode ISSUED_VOTING_RIGHTS;
        public static CapitalTypeCode MAXIMUM_INCREASE;
        public static CapitalTypeCode OUTSTANDING;
        public static CapitalTypeCode REDEEMED;
        public static CapitalTypeCode STATED_CAPITAL;
        public static CapitalTypeCode TREASURY_STOCK;
        public static CapitalTypeCode UNISSUED;
        public static CapitalTypeCode WITHDRAWN;
        public static CapitalTypeCode AUTHORISED;
        public static CapitalTypeCode IN_CIRCULATION;
        public static CapitalTypeCode ISSUED;
        public static CapitalTypeCode ISSUED_RESERVE_CAPITAL;
    }
    /// <summary>
    /// Represente une fréquence: DAILY/quotidien, WEEKLY/hebdomadaire, MONTHLY/mensuel, YEARLY/annuel ...
    /// </summary>
    [NotMapped]
    internal sealed class CapitalTypeCodeAdaptee
    {
        internal readonly string Label;
        internal readonly int Value;
        internal readonly string Description;

        /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static CapitalTypeCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }

        internal CapitalTypeCodeAdaptee(string Label, int Value, string Description)
        {
            this.Label = Label;
            this.Value = Value;
            this.Description = Description;
            Instances[Value] = this;
            Constants[Label] = this;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to CapitalTypeCodeAdaptee return false.
            CapitalTypeCodeAdaptee p = obj as CapitalTypeCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }


        public override int GetHashCode()
        {
            return this.Value;
        }

        #region Singleton partie statique
        internal static readonly Dictionary<int, CapitalTypeCodeAdaptee> Instances = new Dictionary<int, CapitalTypeCodeAdaptee>();
        internal static readonly Dictionary<string, CapitalTypeCodeAdaptee> Constants = new Dictionary<string, CapitalTypeCodeAdaptee>();
        #endregion

    }
}
