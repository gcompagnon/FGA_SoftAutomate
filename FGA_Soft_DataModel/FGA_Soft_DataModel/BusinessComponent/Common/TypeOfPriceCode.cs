using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{
    /// <summary>
    /// Specifies the type of price and information about the price.
    /// Classe de persistance qui correspond à un Adapteur (pattern Adapter)
    /// Ce qui permet d utiliter l entity framework avec des données représentants des CODES-LIBELLES
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class TypeOfPriceCode
    {
        
        private TypeOfPriceCodeAdaptee InternalObject;

        public TypeOfPriceCode()
        {
        }
        public TypeOfPriceCode(string c)
        {
            this.TypeOfPrice = c;
        }
        private TypeOfPriceCode(TypeOfPriceCodeAdaptee Instance)
        {
            this.InternalObject = Instance;
        }

        public string TypeOfPrice
        {
            get { return (InternalObject!=null?InternalObject.Label:""); }
            set
            {
                if (value == null)
                    return;
                TypeOfPriceCodeAdaptee result;
                // recherche d un caractere correspondant à l objet TypeOfPriceCode
                if (TypeOfPriceCodeAdaptee.Constants.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                {
                    // recherche d une constante correspondant à l objet TypeOfPriceCode
                    int codeInt = 0;
                    if( int.TryParse(value,out codeInt) )
                    {
                        if (TypeOfPriceCodeAdaptee.Instances.TryGetValue(codeInt, out result))
                            this.InternalObject = result;
                        else
                            throw new InvalidCastException("TypeOfPriceCode inexistant :" + value);   // code qui n existe pas                  
                    }
                }
            }
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator TypeOfPriceCode(string str)
        {
            if (str == null) return null;
            TypeOfPriceCodeAdaptee result;
            if (TypeOfPriceCodeAdaptee.Constants.TryGetValue(str, out result))
                return new TypeOfPriceCode(result);
            else
                throw new InvalidCastException();
        }

        public override String ToString()
        {
            return InternalObject.Label;
        }

        public override bool Equals(object obj)
        {
            TypeOfPriceCode code = obj as TypeOfPriceCode;
            return this.Equals(code);
        }

        public bool Equals(TypeOfPriceCode code)
        {
            if (code == null || code.InternalObject ==null || this.InternalObject == null )
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

        public static TypeOfPriceCode getTypeOfPriceCodeByLabel(string label)
        {
            return new TypeOfPriceCode(TypeOfPriceCodeAdaptee.Constants[label]);
        }
        
        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static TypeOfPriceCode()
        {
            new TypeOfPriceCodeAdaptee("AVERAGE", 0, "Price is an average execution price.");
            new TypeOfPriceCodeAdaptee("AVERAGE_OVERRIDE", 1, "Price is an override of the average price.");
            new TypeOfPriceCodeAdaptee("COMBINED", 2, "Price is composed of the combined expenses (used in the UK market).");
            new TypeOfPriceCodeAdaptee("GROSS_OF_ALL", 3, "Price is a gross execution price. The price is an all inclusive price, ie, including all charges, fees, and taxes.");
            new TypeOfPriceCodeAdaptee("LIMIT", 4, "Price is the limit price of a limit order, eg, a customer might put in a limit order to sell financial instruments at 67 or to buy at 60.");
            new TypeOfPriceCodeAdaptee("NET", 5, "Price is a net price, ie, net only of local broker's commission, local fees and local taxes.");
            new TypeOfPriceCodeAdaptee("NET_DISCLOSED", 6, "Price is net to the disclosed client.");
            new TypeOfPriceCodeAdaptee("NET_OF_ALL", 7, "Price is a net price, ie, net of all charges, fees and taxes.");
            new TypeOfPriceCodeAdaptee("NET_UNDISCLOSED", 8, "Price is net to the client undisclosed (used in the UK market).");
            //..
            new TypeOfPriceCodeAdaptee("BID", 13, "Price is the calculated bid price of a dual-priced fund (offer-bid prices), ie, the selling price of the units for the investor.");

            new TypeOfPriceCodeAdaptee("NET_ASSET_VALUE",15 , "Price is the net asset value per unit that is used either as a transacting price for a single-priced investment fund class, or as a notional price for the calculation of other prices.");
            new TypeOfPriceCodeAdaptee("MID",21 , "Price is the average price between the bid and offer prices.");
            //..
            new TypeOfPriceCodeAdaptee("MARKET", 25, "Price is the current market price.");

            new TypeOfPriceCodeAdaptee("ESTIMATED_NAV", 30, "Price is an estimated net asset value per unit");
            
            new TypeOfPriceCodeAdaptee("ASK", 34, "Offer.  Price for which seller is willing to sell item.");
            new TypeOfPriceCodeAdaptee("CLEAN", 37, "Paid without accumulated interest.");
            new TypeOfPriceCodeAdaptee("DIRTY", 38, "Paid with accumulated interest.");
            
            new TypeOfPriceCodeAdaptee("CONSOLIDATED", 39, "Price is made using several contributed prices and processed");
        }
        #endregion


    }
    /// <summary>
    /// Represente une fréquence: DAILY/quotidien, WEEKLY/hebdomadaire, MONTHLY/mensuel, YEARLY/annuel ...
    /// </summary>
    [NotMapped]
    internal sealed class TypeOfPriceCodeAdaptee
    {
        internal readonly string Label;
        internal readonly int Value;
        internal readonly string Description;

        /// <summary>
        /// Initialisation: TODO : charger une table de codification 
        /// </summary>
        static TypeOfPriceCodeAdaptee()
        {
            //Console.WriteLine("Chargement de FrequencyCodeAdaptee");
        }

        internal TypeOfPriceCodeAdaptee(string Label, int Value, string Description)
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

            // If parameter cannot be cast to InterestComputationMethodCodeAdaptee return false.
            TypeOfPriceCodeAdaptee p = obj as TypeOfPriceCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }

        #region Singleton partie statique
        internal static readonly Dictionary<int, TypeOfPriceCodeAdaptee> Instances = new Dictionary<int, TypeOfPriceCodeAdaptee>();
        internal static readonly Dictionary<string, TypeOfPriceCodeAdaptee> Constants = new Dictionary<string, TypeOfPriceCodeAdaptee>();
        #endregion
    
    }
}
