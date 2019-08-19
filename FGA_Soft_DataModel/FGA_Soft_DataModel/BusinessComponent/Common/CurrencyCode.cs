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
    /// La liste des codes des devises ISO 4217 
    /// http://www.currency-iso.org/iso_index/iso_tables/iso_tables_a1.htm
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class CurrencyCode
    {
        private CurrencyCodeAdaptee InternalObject;

        public CurrencyCode()
        {
        }


        internal CurrencyCode(CurrencyCodeAdaptee Instance)
        {
            if (Instance.Value != null)
            {
                this.InternalObject = Instance;
            }
        }

        [MaxLength(4),Column(TypeName="nchar")]
        public string Currency
        {
            get { return (InternalObject == null ? null : InternalObject.Value); }
            set
            {
                if (value == null)
                    return;

                CurrencyCodeAdaptee result;
                if (CurrencyCodeAdaptee.Instances.TryGetValue(value.TrimEnd(), out result))
                    this.InternalObject = result;
                else
                    throw new InvalidCastException("CurrencyCode inexistant :" + value);   // code qui n existe pas                  
            }

        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator CurrencyCode(string str)
        {
            if (str == null) return null;
            CurrencyCodeAdaptee result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue(str.TrimEnd(), out result))
                return new CurrencyCode(result);
            else
                throw new InvalidCastException("The Code "+str+" does not refer to a currency code");
        }
        public override String ToString()
        {
            return InternalObject.ToString();
        }


        public static CurrencyCode getCurrencyByLabel(string label)
        {
            try
            {
                return new CurrencyCode(CurrencyCodeAdaptee.Constants[label]);
            }
            catch (KeyNotFoundException knfe)
            {
                throw new InvalidCastException("The Code " + label + " does not refer to a currency code");
            }
        }

        public override bool Equals(object obj)
        {
            CurrencyCode code = obj as CurrencyCode;
            return this.Equals(code);
        }

        public bool Equals(CurrencyCode code)
        {
            if (code == null || code.InternalObject ==null || this.InternalObject == null)
                return false;
            return this.InternalObject.Equals(code.InternalObject);
        }

        public override int GetHashCode()
        {
            if (this.InternalObject == null)
                return 0;
            else
                return this.InternalObject.NumericCode;
        }

        public bool HasValue
        {
            get
            {
                return this.InternalObject != null;
            }
        }

        #region utilities to get constant, most used currencies
        public static CurrencyCode EUR
        {
            get
            {
                return new CurrencyCode(mEUR);
            }
        }
        public static CurrencyCode MEUR
        {
            get
            {
                return new CurrencyCode(mMillionsEUR);
            }
        }
        public static CurrencyCode KEUR
        {
            get
            {
                return new CurrencyCode(mKiloEUR);
            }
        }
        public static CurrencyCode USD
        {
            get
            {
                return new CurrencyCode(mUSD);
            }
        }
        public static CurrencyCode MUSD
        {
            get
            {
                return new CurrencyCode(mMillionsUSD);
            }
        }
        public static CurrencyCode KUSD
        {
            get
            {
                return new CurrencyCode(mKiloUSD);
            }
        }
        #endregion

        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static CurrencyCode()
        {
            new CurrencyCodeAdaptee("Afghani", "AFGHANISTAN", "AFN", 971, 2);
            new CurrencyCodeAdaptee("Euro", "ÅLAND ISLANDS", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Lek", "ALBANIA", "ALL", 008, 2);
            new CurrencyCodeAdaptee("Algerian Dinar", "ALGERIA", "DZD", 012, 2);
            new CurrencyCodeAdaptee("US Dollar", "AMERICAN SAMOA", "USD", 840, 2);
            new CurrencyCodeAdaptee("Euro", "ANDORRA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Kwanza", "ANGOLA", "AOA", 973, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "ANGUILLA", "XCD", 951, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "ANTIGUA AND BARBUDA", "XCD", 951, 2);
            new CurrencyCodeAdaptee("Argentine Peso", "ARGENTINA", "ARS", 032, 2);
            new CurrencyCodeAdaptee("Armenian Dram", "ARMENIA", "AMD", 051, 2);
            new CurrencyCodeAdaptee("Aruban Florin", "ARUBA", "AWG", 533, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "AUSTRALIA", "AUD", 036, 2);
            new CurrencyCodeAdaptee("Euro", "AUSTRIA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Azerbaijanian Manat", "AZERBAIJAN", "AZN", 944, 2);
            new CurrencyCodeAdaptee("Bahamian Dollar", "BAHAMAS", "BSD", 044, 2);
            new CurrencyCodeAdaptee("Bahraini Dinar", "BAHRAIN", "BHD", 048, 3);
            new CurrencyCodeAdaptee("Taka", "BANGLADESH", "BDT", 050, 2);
            new CurrencyCodeAdaptee("Barbados Dollar", "BARBADOS", "BBD", 052, 2);
            new CurrencyCodeAdaptee("Belarussian Ruble", "BELARUS", "BYR", 974, 0);
            new CurrencyCodeAdaptee("Euro", "BELGIUM", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Belize Dollar", "BELIZE", "BZD", 084, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "BENIN", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Bermudian Dollar", "BERMUDA", "BMD", 060, 2);
            new CurrencyCodeAdaptee("Indian Rupee", "BHUTAN", "INR", 356, 2);
            new CurrencyCodeAdaptee("Ngultrum", "BHUTAN", "BTN", 064, 2);
            new CurrencyCodeAdaptee("Boliviano", "BOLIVIA, PLURINATIONAL STATE OF", "BOB", 068, 2);
            new CurrencyCodeAdaptee("Mvdol", "BOLIVIA, PLURINATIONAL STATE OF", "BOV", 984, 2);
            new CurrencyCodeAdaptee("US Dollar", "BONAIRE, SINT EUSTATIUS AND SABA", "USD", 840, 2);
            new CurrencyCodeAdaptee("Convertible Mark", "BOSNIA & HERZEGOVINA", "BAM", 977, 2);
            new CurrencyCodeAdaptee("Pula", "BOTSWANA", "BWP", 072, 2);
            new CurrencyCodeAdaptee("Norwegian Krone", "BOUVET ISLAND", "NOK", 578, 2);
            new CurrencyCodeAdaptee("Brazilian Real", "BRAZIL", "BRL", 986, 2);
            new CurrencyCodeAdaptee("US Dollar", "BRITISH INDIAN OCEAN TERRITORY", "USD", 840, 2);
            new CurrencyCodeAdaptee("Brunei Dollar", "BRUNEI DARUSSALAM", "BND", 096, 2);
            new CurrencyCodeAdaptee("Bulgarian Lev", "BULGARIA", "BGN", 975, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "BURKINA FASO", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Burundi Franc", "BURUNDI", "BIF", 108, 0);
            new CurrencyCodeAdaptee("Riel", "CAMBODIA", "KHR", 116, 2);
            new CurrencyCodeAdaptee("CFA Franc BEAC", "CAMEROON", "XAF", 950, 0);
            new CurrencyCodeAdaptee("Canadian Dollar", "CANADA", "CAD", 124, 2);
            new CurrencyCodeAdaptee("Cape Verde Escudo", "CAPE VERDE", "CVE", 132, 2);
            new CurrencyCodeAdaptee("Cayman Islands Dollar", "CAYMAN ISLANDS", "KYD", 136, 2);
            new CurrencyCodeAdaptee("CFA Franc BEAC", "CENTRAL AFRICAN REPUBLIC", "XAF", 950, 0);
            new CurrencyCodeAdaptee("CFA Franc BEAC", "CHAD", "XAF", 950, 0);
            new CurrencyCodeAdaptee("Chilean Peso", "CHILE", "CLP", 152, 0);
            new CurrencyCodeAdaptee("Unidades de fomento", "CHILE", "CLF", 990, 0);
            new CurrencyCodeAdaptee("Yuan Renminbi", "CHINA", "CNY", 156, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "CHRISTMAS ISLAND", "AUD", 036, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "COCOS (KEELING) ISLANDS", "AUD", 036, 2);
            new CurrencyCodeAdaptee("Colombian Peso", "COLOMBIA", "COP", 170, 2);
            new CurrencyCodeAdaptee("Unidad de Valor Real", "COLOMBIA", "COU", 970, 2);
            new CurrencyCodeAdaptee("Comoro Franc", "COMOROS", "KMF", 174, 0);
            new CurrencyCodeAdaptee("CFA Franc BEAC", "CONGO", "XAF", 950, 0);
            new CurrencyCodeAdaptee("Congolese Franc", "CONGO, THE DEMOCRATIC REPUBLIC OF", "CDF", 976, 2);
            new CurrencyCodeAdaptee("New Zealand Dollar", "COOK ISLANDS", "NZD", 554, 2);
            new CurrencyCodeAdaptee("Costa Rican Colon", "COSTA RICA", "CRC", 188, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "CÔTE D'IVOIRE", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Croatian Kuna", "CROATIA", "HRK", 191, 2);
            new CurrencyCodeAdaptee("Cuban Peso", "CUBA", "CUP", 192, 2);
            new CurrencyCodeAdaptee("Peso Convertible", "CUBA", "CUC", 931, 2);
            new CurrencyCodeAdaptee("Netherlands Antillean Guilder", "CURACAO", "ANG", 532, 2);
            new CurrencyCodeAdaptee("Euro", "CYPRUS", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Czech Koruna", "CZECH REPUBLIC", "CZK", 203, 2);
            new CurrencyCodeAdaptee("Danish Krone", "DENMARK", "DKK", 208, 2);
            new CurrencyCodeAdaptee("Djibouti Franc", "DJIBOUTI", "DJF", 262, 0);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "DOMINICA", "XCD", 951, 2);
            new CurrencyCodeAdaptee("Dominican Peso", "DOMINICAN REPUBLIC", "DOP", 214, 2);
            new CurrencyCodeAdaptee("US Dollar", "ECUADOR", "USD", 840, 2);
            new CurrencyCodeAdaptee("Egyptian Pound", "EGYPT", "EGP", 818, 2);
            new CurrencyCodeAdaptee("El Salvador Colon", "EL SALVADOR", "SVC", 222, 2);
            new CurrencyCodeAdaptee("US Dollar", "EL SALVADOR", "USD", 840, 2);
            new CurrencyCodeAdaptee("CFA Franc BEAC", "EQUATORIAL GUINEA", "XAF", 950, 0);
            new CurrencyCodeAdaptee("Nakfa", "ERITREA", "ERN", 232, 2);
            new CurrencyCodeAdaptee("Euro", "ESTONIA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Ethiopian Birr", "ETHIOPIA", "ETB", 230, 2);
            new CurrencyCodeAdaptee("Euro", "EUROPEAN UNION", "EUR", 978, 2);            
            new CurrencyCodeAdaptee("Falkland Islands Pound", "FALKLAND ISLANDS (MALVINAS)", "FKP", 238, 2);
            new CurrencyCodeAdaptee("Danish Krone", "FAROE ISLANDS", "DKK", 208, 2);
            new CurrencyCodeAdaptee("Fiji Dollar", "FIJI", "FJD", 242, 2);
            new CurrencyCodeAdaptee("Euro", "FINLAND", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Euro", "FRANCE", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Euro", "FRENCH GUIANA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("CFP Franc", "FRENCH POLYNESIA", "XPF", 953, 0);
            new CurrencyCodeAdaptee("Euro", "FRENCH SOUTHERN TERRITORIES", "EUR", 978, 2);
            new CurrencyCodeAdaptee("CFA Franc BEAC", "GABON", "XAF", 950, 0);
            new CurrencyCodeAdaptee("Dalasi", "GAMBIA", "GMD", 270, 2);
            new CurrencyCodeAdaptee("Lari", "GEORGIA", "GEL", 981, 2);
            new CurrencyCodeAdaptee("Euro", "GERMANY", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Cedi", "GHANA", "GHS", 936, 2);
            new CurrencyCodeAdaptee("Gibraltar Pound", "GIBRALTAR", "GIP", 292, 2);
            new CurrencyCodeAdaptee("Euro", "GREECE", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Danish Krone", "GREENLAND", "DKK", 208, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "GRENADA", "XCD", 951, 2);
            new CurrencyCodeAdaptee("Euro", "GUADELOUPE", "EUR", 978, 2);
            new CurrencyCodeAdaptee("US Dollar", "GUAM", "USD", 840, 2);
            new CurrencyCodeAdaptee("Quetzal", "GUATEMALA", "GTQ", 320, 2);
            new CurrencyCodeAdaptee("Pound Sterling", "GUERNSEY", "GBP", 826, 2);
            new CurrencyCodeAdaptee("Guinea Franc", "GUINEA", "GNF", 324, 0);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "GUINEA-BISSAU", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Guyana Dollar", "GUYANA", "GYD", 328, 2);
            new CurrencyCodeAdaptee("Gourde", "HAITI", "HTG", 332, 2);
            new CurrencyCodeAdaptee("US Dollar", "HAITI", "USD", 840, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "HEARD ISLAND AND McDONALD ISLANDS", "AUD", 036, 2);
            new CurrencyCodeAdaptee("Euro", "HOLY SEE (VATICAN CITY STATE)", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Lempira", "HONDURAS", "HNL", 340, 2);
            new CurrencyCodeAdaptee("Hong Kong Dollar", "HONG KONG", "HKD", 344, 2);
            new CurrencyCodeAdaptee("Forint", "HUNGARY", "HUF", 348, 2);
            new CurrencyCodeAdaptee("Iceland Krona", "ICELAND", "ISK", 352, 0);
            new CurrencyCodeAdaptee("Indian Rupee", "INDIA", "INR", 356, 2);
            new CurrencyCodeAdaptee("Rupiah", "INDONESIA", "IDR", 360, 2);
            new CurrencyCodeAdaptee("SDR (Special Drawing Right)", "INTERNATIONAL MONETARY FUND (IMF) ", "XDR", 960, -1);
            new CurrencyCodeAdaptee("Iranian Rial", "IRAN, ISLAMIC REPUBLIC OF", "IRR", 364, 2);
            new CurrencyCodeAdaptee("Iraqi Dinar", "IRAQ", "IQD", 368, 3);
            new CurrencyCodeAdaptee("Euro", "IRELAND", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Pound Sterling", "ISLE OF MAN", "GBP", 826, 2);
            new CurrencyCodeAdaptee("New Israeli Sheqel", "ISRAEL", "ILS", 376, 2);
            new CurrencyCodeAdaptee("Euro", "ITALY", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Jamaican Dollar", "JAMAICA", "JMD", 388, 2);
            new CurrencyCodeAdaptee("Yen", "JAPAN", "JPY", 392, 0);
            new CurrencyCodeAdaptee("Pound Sterling", "JERSEY", "GBP", 826, 2);
            new CurrencyCodeAdaptee("Jordanian Dinar", "JORDAN", "JOD", 400, 3);
            new CurrencyCodeAdaptee("Tenge", "KAZAKHSTAN", "KZT", 398, 2);
            new CurrencyCodeAdaptee("Kenyan Shilling", "KENYA", "KES", 404, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "KIRIBATI", "AUD", 036, 2);
            new CurrencyCodeAdaptee("North Korean Won", "KOREA, DEMOCRATIC PEOPLE’S REPUBLIC OF", "KPW", 408, 2);
            new CurrencyCodeAdaptee("Won", "KOREA, REPUBLIC OF", "KRW", 410, 0);
            new CurrencyCodeAdaptee("Kuwaiti Dinar", "KUWAIT", "KWD", 414, 3);
            new CurrencyCodeAdaptee("Som", "KYRGYZSTAN", "KGS", 417, 2);
            new CurrencyCodeAdaptee("Kip", "LAO PEOPLE’S DEMOCRATIC REPUBLIC", "LAK", 418, 2);
            new CurrencyCodeAdaptee("Latvian Lats", "LATVIA", "LVL", 428, 2);
            new CurrencyCodeAdaptee("Lebanese Pound", "LEBANON", "LBP", 422, 2);
            new CurrencyCodeAdaptee("Loti", "LESOTHO", "LSL", 426, 2);
            new CurrencyCodeAdaptee("Rand", "LESOTHO", "ZAR", 710, 2);
            new CurrencyCodeAdaptee("Liberian Dollar", "LIBERIA", "LRD", 430, 2);
            new CurrencyCodeAdaptee("Libyan Dinar", "LIBYAN ARAB JAMAHIRIYA", "LYD", 434, 3);
            new CurrencyCodeAdaptee("Swiss Franc", "LIECHTENSTEIN", "CHF", 756, 2);
            new CurrencyCodeAdaptee("Lithuanian Litas", "LITHUANIA", "LTL", 440, 2);
            new CurrencyCodeAdaptee("Euro", "LUXEMBOURG", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Pataca", "MACAO", "MOP", 446, 2);
            new CurrencyCodeAdaptee("Denar", "MACEDONIA, THE FORMER YUGOSLAV REPUBLIC OF", "MKD", 807, 2);
            new CurrencyCodeAdaptee("Malagasy Ariary", "MADAGASCAR", "MGA", 969, 2);
            new CurrencyCodeAdaptee("Kwacha", "MALAWI", "MWK", 454, 2);
            new CurrencyCodeAdaptee("Malaysian Ringgit", "MALAYSIA", "MYR", 458, 2);
            new CurrencyCodeAdaptee("Rufiyaa", "MALDIVES", "MVR", 462, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "MALI", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Euro", "MALTA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("US Dollar", "MARSHALL ISLANDS", "USD", 840, 2);
            new CurrencyCodeAdaptee("Euro", "MARTINIQUE", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Ouguiya", "MAURITANIA", "MRO", 478, 2);
            new CurrencyCodeAdaptee("Mauritius Rupee", "MAURITIUS", "MUR", 480, 2);
            new CurrencyCodeAdaptee("Euro", "MAYOTTE", "EUR", 978, 2);
            new CurrencyCodeAdaptee("ADB Unit of Account", "MEMBER COUNTRIES OF THE AFRICAN DEVELOPMENT BANK GROUP", "XUA", 965, -1);
            new CurrencyCodeAdaptee("Mexican Peso", "MEXICO", "MXN", 484, 2);
            new CurrencyCodeAdaptee("Mexican Unidad de Inversion (UDI)", "MEXICO", "MXV", 979, 2);
            new CurrencyCodeAdaptee("US Dollar", "MICRONESIA, FEDERATED STATES OF", "USD", 840, 2);
            new CurrencyCodeAdaptee("Moldovan Leu", "MOLDOVA, REPUBLIC OF", "MDL", 498, 2);
            new CurrencyCodeAdaptee("Euro", "MONACO", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Tugrik", "MONGOLIA", "MNT", 496, 2);
            new CurrencyCodeAdaptee("Euro", "MONTENEGRO", "EUR", 978, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "MONTSERRAT", "XCD", 951, 2);
            new CurrencyCodeAdaptee("Moroccan Dirham", "MOROCCO", "MAD", 504, 2);
            new CurrencyCodeAdaptee("Metical", "MOZAMBIQUE", "MZN", 943, 2);
            new CurrencyCodeAdaptee("Kyat", "MYANMAR", "MMK", 104, 2);
            new CurrencyCodeAdaptee("Namibia Dollar", "NAMIBIA", "NAD", 516, 2);
            new CurrencyCodeAdaptee("Rand", "NAMIBIA", "ZAR", 710, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "NAURU", "AUD", 036, 2);
            new CurrencyCodeAdaptee("Nepalese Rupee", "NEPAL", "NPR", 524, 2);
            new CurrencyCodeAdaptee("Euro", "NETHERLANDS", "EUR", 978, 2);
            new CurrencyCodeAdaptee("CFP Franc", "NEW CALEDONIA", "XPF", 953, 0);
            new CurrencyCodeAdaptee("New Zealand Dollar", "NEW ZEALAND", "NZD", 554, 2);
            new CurrencyCodeAdaptee("Cordoba Oro", "NICARAGUA", "NIO", 558, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "NIGER", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Naira", "NIGERIA", "NGN", 566, 2);
            new CurrencyCodeAdaptee("New Zealand Dollar", "NIUE", "NZD", 554, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "NORFOLK ISLAND", "AUD", 036, 2);
            new CurrencyCodeAdaptee("US Dollar", "NORTHERN MARIANA ISLANDS", "USD", 840, 2);
            new CurrencyCodeAdaptee("Norwegian Krone", "NORWAY", "NOK", 578, 2);
            new CurrencyCodeAdaptee("Rial Omani", "OMAN", "OMR", 512, 3);
            new CurrencyCodeAdaptee("Pakistan Rupee", "PAKISTAN", "PKR", 586, 2);
            new CurrencyCodeAdaptee("US Dollar", "PALAU", "USD", 840, 2);
            new CurrencyCodeAdaptee("Balboa", "PANAMA", "PAB", 590, 2);
            new CurrencyCodeAdaptee("US Dollar", "PANAMA", "USD", 840, 2);
            new CurrencyCodeAdaptee("Kina", "PAPUA NEW GUINEA", "PGK", 598, 2);
            new CurrencyCodeAdaptee("Guarani", "PARAGUAY", "PYG", 600, 0);
            new CurrencyCodeAdaptee("Nuevo Sol", "PERU", "PEN", 604, 2);
            new CurrencyCodeAdaptee("Philippine Peso", "PHILIPPINES", "PHP", 608, 2);
            new CurrencyCodeAdaptee("New Zealand Dollar", "PITCAIRN", "NZD", 554, 2);
            new CurrencyCodeAdaptee("Zloty", "POLAND", "PLN", 985, 2);
            new CurrencyCodeAdaptee("Euro", "PORTUGAL", "EUR", 978, 2);
            new CurrencyCodeAdaptee("US Dollar", "PUERTO RICO", "USD", 840, 2);
            new CurrencyCodeAdaptee("Qatari Rial", "QATAR", "QAR", 634, 2);
            new CurrencyCodeAdaptee("Euro", "RÉUNION", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Leu", "ROMANIA", "RON", 946, 2);
            new CurrencyCodeAdaptee("Russian Ruble", "RUSSIAN FEDERATION", "RUB", 643, 2);
            new CurrencyCodeAdaptee("Rwanda Franc", "RWANDA", "RWF", 646, 0);
            new CurrencyCodeAdaptee("Saint Helena Pound", "SAINT HELENA, ASCENSION AND TRISTAN DA CUNHA", "SHP", 654, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "SAINT KITTS AND NEVIS", "XCD", 951, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "SAINT LUCIA", "XCD", 951, 2);
            new CurrencyCodeAdaptee("Euro", "SAINT MARTIN", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Euro", "SAINT PIERRE AND MIQUELON", "EUR", 978, 2);
            new CurrencyCodeAdaptee("East Caribbean Dollar", "SAINT VINCENT AND THE GRENADINES", "XCD", 951, 2);
            new CurrencyCodeAdaptee("Euro", "SAINT-BARTHÉLEMY", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Tala", "SAMOA", "WST", 882, 2);
            new CurrencyCodeAdaptee("Euro", "SAN MARINO", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Dobra", "SÃO TOME AND PRINCIPE", "STD", 678, 2);
            new CurrencyCodeAdaptee("Saudi Riyal", "SAUDI ARABIA", "SAR", 682, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "SENEGAL", "XOF", 952, 0);
            new CurrencyCodeAdaptee("Serbian Dinar", "SERBIA ", "RSD", 941, 2);
            new CurrencyCodeAdaptee("Seychelles Rupee", "SEYCHELLES", "SCR", 690, 2);
            new CurrencyCodeAdaptee("Leone", "SIERRA LEONE", "SLL", 694, 2);
            new CurrencyCodeAdaptee("Singapore Dollar", "SINGAPORE", "SGD", 702, 2);
            new CurrencyCodeAdaptee("Netherlands Antillean Guilder", "SINT MAARTEN (DUTCH PART)", "ANG", 532, 2);
            new CurrencyCodeAdaptee("Sucre", "SISTEMA UNITARIO DE COMPENSACION REGIONAL DE PAGOS \"SUCRE\" ", "XSU", 994, -1);
            new CurrencyCodeAdaptee("Euro", "SLOVAKIA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Euro", "SLOVENIA", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Solomon Islands Dollar", "SOLOMON ISLANDS", "SBD", 090, 2);
            new CurrencyCodeAdaptee("Somali Shilling", "SOMALIA", "SOS", 706, 2);
            new CurrencyCodeAdaptee("Rand", "SOUTH AFRICA", "ZAR", 710, 2);
            new CurrencyCodeAdaptee("South Sudanese Pound", "SOUTH SUDAN", "SSP", 728, 2);
            new CurrencyCodeAdaptee("Euro", "SPAIN", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Sri Lanka Rupee", "SRI LANKA", "LKR", 144, 2);
            new CurrencyCodeAdaptee("Sudanese Pound", "SUDAN", "SDG", 938, 2);
            new CurrencyCodeAdaptee("Surinam Dollar", "SURINAME", "SRD", 968, 2);
            new CurrencyCodeAdaptee("Norwegian Krone", "SVALBARD AND JAN MAYEN", "NOK", 578, 2);
            new CurrencyCodeAdaptee("Lilangeni", "SWAZILAND", "SZL", 748, 2);
            new CurrencyCodeAdaptee("Swedish Krona", "SWEDEN", "SEK", 752, 2);
            new CurrencyCodeAdaptee("Swiss Franc", "SWITZERLAND", "CHF", 756, 2);
            new CurrencyCodeAdaptee("WIR Euro", "SWITZERLAND", "CHE", 947, 2);
            new CurrencyCodeAdaptee("WIR Franc", "SWITZERLAND", "CHW", 948, 2);
            new CurrencyCodeAdaptee("Syrian Pound", "SYRIAN ARAB REPUBLIC", "SYP", 760, 2);
            new CurrencyCodeAdaptee("New Taiwan Dollar", "TAIWAN, PROVINCE OF CHINA", "TWD", 901, 2);
            new CurrencyCodeAdaptee("Somoni", "TAJIKISTAN", "TJS", 972, 2);
            new CurrencyCodeAdaptee("Tanzanian Shilling", "TANZANIA, UNITED REPUBLIC OF", "TZS", 834, 2);
            new CurrencyCodeAdaptee("Baht", "THAILAND", "THB", 764, 2);
            new CurrencyCodeAdaptee("US Dollar", "TIMOR-LESTE", "USD", 840, 2);
            new CurrencyCodeAdaptee("CFA Franc BCEAO", "TOGO", "XOF", 952, 0);
            new CurrencyCodeAdaptee("New Zealand Dollar", "TOKELAU", "NZD", 554, 2);
            new CurrencyCodeAdaptee("Pa’anga", "TONGA", "TOP", 776, 2);
            new CurrencyCodeAdaptee("Trinidad and Tobago Dollar", "TRINIDAD AND TOBAGO", "TTD", 780, 2);
            new CurrencyCodeAdaptee("Tunisian Dinar", "TUNISIA", "TND", 788, 3);
            new CurrencyCodeAdaptee("Turkish Lira", "TURKEY", "TRY", 949, 2);
            new CurrencyCodeAdaptee("New Manat", "TURKMENISTAN", "TMT", 934, 2);
            new CurrencyCodeAdaptee("US Dollar", "TURKS AND CAICOS ISLANDS", "USD", 840, 2);
            new CurrencyCodeAdaptee("Australian Dollar", "TUVALU", "AUD", 036, 2);
            new CurrencyCodeAdaptee("Uganda Shilling", "UGANDA", "UGX", 800, 2);
            new CurrencyCodeAdaptee("Hryvnia", "UKRAINE", "UAH", 980, 2);
            new CurrencyCodeAdaptee("UAE Dirham", "UNITED ARAB EMIRATES", "AED", 784, 2);
            new CurrencyCodeAdaptee("Pound Sterling", "UNITED KINGDOM", "GBP", 826, 2);
            new CurrencyCodeAdaptee("US Dollar", "UNITED STATES", "USD", 840, 2);            
            new CurrencyCodeAdaptee("US Dollar (Next day)", "UNITED STATES", "USN", 997, 2);
            new CurrencyCodeAdaptee("US Dollar (Same day)", "UNITED STATES", "USS", 998, 2);
            new CurrencyCodeAdaptee("US Dollar", "UNITED STATES MINOR OUTLYING ISLANDS", "USD", 840, 2);
            new CurrencyCodeAdaptee("Peso Uruguayo", "URUGUAY", "UYU", 858, 2);
            new CurrencyCodeAdaptee("Uruguay Peso en Unidades Indexadas (URUIURUI)", "URUGUAY", "UYI", 940, 0);
            new CurrencyCodeAdaptee("Uzbekistan Sum", "UZBEKISTAN", "UZS", 860, 2);
            new CurrencyCodeAdaptee("Vatu", "VANUATU", "VUV", 548, 0);
            new CurrencyCodeAdaptee("Euro", "Vatican City State (HOLY SEE)", "EUR", 978, 2);
            new CurrencyCodeAdaptee("Bolivar Fuerte", "VENEZUELA, BOLIVARIAN REPUBLIC OF", "VEF", 937, 2);
            new CurrencyCodeAdaptee("Dong", "VIET NAM", "VND", 704, 0);
            new CurrencyCodeAdaptee("US Dollar", "VIRGIN ISLANDS (BRITISH)", "USD", 840, 2);
            new CurrencyCodeAdaptee("US Dollar", "VIRGIN ISLANDS (US)", "USD", 840, 2);
            new CurrencyCodeAdaptee("CFP Franc", "WALLIS AND FUTUNA", "XPF", 953, 0);
            new CurrencyCodeAdaptee("Moroccan Dirham", "WESTERN SAHARA", "MAD", 504, 2);
            new CurrencyCodeAdaptee("Yemeni Rial", "YEMEN", "YER", 886, 2);
            new CurrencyCodeAdaptee("Zambian Kwacha", "ZAMBIA", "ZMK", 894, 2);
            new CurrencyCodeAdaptee("Zimbabwe Dollar", "ZIMBABWE", "ZWL", 932, 2);
            new CurrencyCodeAdaptee("Bond Markets Unit European Composite Unit (EURCO)", "ZZ01_Bond Markets Unit European_EURCO", "XBA", 955, -1);
            new CurrencyCodeAdaptee("Bond Markets Unit European Monetary Unit (E.M.U.-6)", "ZZ02_Bond Markets Unit European_EMU-6", "XBB", 956, -1);
            new CurrencyCodeAdaptee("Bond Markets Unit European Unit of Account 9 (E.U.A.-9)", "ZZ03_Bond Markets Unit European_EUA-9", "XBC", 957, -1);
            new CurrencyCodeAdaptee("Bond Markets Unit European Unit of Account 17 (E.U.A.-17)", "ZZ04_Bond Markets Unit European_EUA-17", "XBD", 958, -1);
            new CurrencyCodeAdaptee("UIC-Franc", "ZZ05_UIC-Franc", "XFU", -1, -1);
            new CurrencyCodeAdaptee("Codes specifically reserved for testing purposes", "ZZ06_Testing_Code", "XTS", 963, -1);
            new CurrencyCodeAdaptee("The codes assigned for transactions where no currency is involved", "ZZ07_No_Currency", "XXX", 999, -1);
            new CurrencyCodeAdaptee("Gold", "ZZ08_Gold", "XAU", 959, -1);
            new CurrencyCodeAdaptee("Palladium", "ZZ09_Palladium", "XPD", 964, -1);
            new CurrencyCodeAdaptee("Platinum", "ZZ10_Platinum", "XPT", 962, -1);
            new CurrencyCodeAdaptee("Silver", "ZZ11_Silver", "XAG", 961, -1);

            new CurrencyCodeAdaptee("Millions Euros", "EUROPEAN UNION", "MEUR", 1978, -1);
            new CurrencyCodeAdaptee("Kilo Euros", "EUROPEAN UNION", "KEUR", 1978, -1);
            new CurrencyCodeAdaptee("Millions US Dollar", "UNITED STATES", "MUSD", 1840, -1);
            new CurrencyCodeAdaptee("Kilo US Dollar", "UNITED STATES", "KUSD", 1840, -1);

            CurrencyCodeAdaptee result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue("EUR", out result))
                mEUR = result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue("USD", out result))
                mUSD = result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue("MEUR", out result))
                mMillionsEUR = result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue("KEUR", out result))
                mKiloEUR = result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue("MUSD", out result))
                mMillionsUSD = result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue("KUSD", out result))
                mKiloUSD = result;

        }

        internal static CurrencyCodeAdaptee mEUR;
        internal static CurrencyCodeAdaptee mUSD;
        internal static CurrencyCodeAdaptee mMillionsEUR;
        internal static CurrencyCodeAdaptee mKiloEUR;
        internal static CurrencyCodeAdaptee mMillionsUSD;
        internal static CurrencyCodeAdaptee mKiloUSD;

        #endregion

    }

    /// <summary>
    /// Represente une devise
    /// </summary>
    [NotMapped]
    internal sealed class CurrencyCodeAdaptee
    {
        internal readonly string Label;
        internal readonly List<string> Countries;
        internal readonly string Value;
        internal readonly int NumericCode;
        internal readonly int MinorUnit;


        internal CurrencyCodeAdaptee(string Label, string Country, string Value, int NumericCode, int MinorUnit)
        {
            CurrencyCodeAdaptee result;
            if (CurrencyCodeAdaptee.Instances.TryGetValue(Value, out result))
            {
                result.addCountry(Country);
            }
            else
            {
                this.Label = Label;
                this.Countries = new List<string>();
                this.Countries.Add(Country);
                this.Value = Value;
                this.NumericCode = NumericCode;
                this.MinorUnit = MinorUnit;
                Instances[Value] = this;
                Constants[Label] = this;
            }

        }
        public override String ToString()
        {
            return Value;
        }

        internal void addCountry(string Country)
        {
            this.Countries.Add(Country);
        }
        
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to CurrencyCodeAdaptee return false.
            CurrencyCodeAdaptee p = obj as CurrencyCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }

        public override int GetHashCode()
        {
                return this.NumericCode;
        }

        #region Singleton partie statique
        internal static readonly Dictionary<string, CurrencyCodeAdaptee> Instances = new Dictionary<string, CurrencyCodeAdaptee>(StringComparer.OrdinalIgnoreCase);
        internal static readonly Dictionary<string, CurrencyCodeAdaptee> Constants = new Dictionary<string, CurrencyCodeAdaptee>(StringComparer.OrdinalIgnoreCase);

        #endregion



    }

}
