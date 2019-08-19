using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{
    /// <summary>
    /// Les codes ISO 3166 des pays
    /// http://www.iso.org/iso/country_codes/iso_3166_code_lists.htm
    /// http://www.nationsonline.org/oneworld/country_code_list.htm
    /// Le code suit le pattern Adapter
    /// </summary>
    [ComplexType]
    public sealed class CountryCode
    {
        public const string DEFAULT_VALUE = "XX";
        internal static CountryCode _DEFAULT = null;
        [NotMapped]
        public static CountryCode DEFAULT
        {
            get
            {
                if (_DEFAULT == null) { _DEFAULT = new CountryCode(CountryCodeAdaptee.Instances[DEFAULT_VALUE]); }
                return _DEFAULT;
            }
        }


        private CountryCodeAdaptee InternalObject;

        public CountryCode()
        { }

        internal CountryCode(CountryCodeAdaptee Instance)
        {
            if (Instance.Value != null)
            {
                this.InternalObject = Instance;
            }
        }

        [Column("Country",TypeName="nchar"), MaxLength(2)]
        public string Country
        {
            get { return (InternalObject != null ? InternalObject.Value : null); }
            set
            {
                if (value == null || value == string.Empty) value = "XX";

                CountryCodeAdaptee result;
                if (CountryCodeAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    throw new InvalidCastException("The Code " + value + " does not refer to a country code");// code qui n existe pas
            }
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator CountryCode(string str)
        {
            if (str == null || str == string.Empty) str = "XX";

            CountryCodeAdaptee result;
            if (CountryCodeAdaptee.Instances.TryGetValue(str, out result))
                return new CountryCode(result);
            else
                throw new InvalidCastException();
        }
        public override String ToString()
        {
            return InternalObject.ToString();
        }

        public override bool Equals(object obj)
        {
            CountryCode code = obj as CountryCode;
            return this.Equals(code);
        }

        public bool Equals(CountryCode code)
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
                return this.InternalObject.Value_Numerical;
        }

        public bool isDefaultValue()
        {
            if (this.InternalObject  == null || this.InternalObject.Value.Equals(CountryCode.DEFAULT_VALUE) )
                return true;
            return false;
        }

        public static CountryCode getCountryByLabel(string label)
        {
            try
            {
            return new CountryCode(CountryCodeAdaptee.Constants[label]);
            }
            catch (KeyNotFoundException knfe)
            {
                throw new InvalidCastException("The Code " + label + " does not refer to a country code");
            }
        }


        public string LabelEnglish { get { return this.InternalObject.LabelEnglish; } }
        public string Label { get { return this.InternalObject.Label; } }
        public string Code2chars { get { return this.InternalObject.Value; } }
        public string Code3chars { get { return this.InternalObject.Value_3Chars; } }

        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static CountryCode()
        {
            new CountryCodeAdaptee("AF", "AFG", 4, "Afghanistan", "AFGHANISTAN");
            new CountryCodeAdaptee("AX", "ALA", 248, "Aland Islands", "ÅLAND, ÎLES");
            new CountryCodeAdaptee("AL", "ALB", 8, "Albania", "ALBANIE");
            new CountryCodeAdaptee("DZ", "DZA", 12, "Algeria", "ALGÉRIE");
            new CountryCodeAdaptee("AS", "ASM", 16, "American Samoa", "SAMOA AMÉRICAINES");
            new CountryCodeAdaptee("AD", "AND", 20, "Andorra", "ANDORRE");
            new CountryCodeAdaptee("AO", "AGO", 24, "Angola", "ANGOLA");
            new CountryCodeAdaptee("AI", "AIA", 660, "Anguilla", "ANGUILLA");
            new CountryCodeAdaptee("AQ", "ATA", 10, "Antarctica", "ANTARCTIQUE");
            new CountryCodeAdaptee("AG", "ATG", 28, "Antigua and Barbuda", "ANTIGUA-ET-BARBUDA");
            new CountryCodeAdaptee("AR", "ARG", 32, "Argentina", "ARGENTINE");
            new CountryCodeAdaptee("AM", "ARM", 51, "Armenia", "ARMÉNIE");
            new CountryCodeAdaptee("AW", "ABW", 533, "Aruba", "ARUBA");
            new CountryCodeAdaptee("AU", "AUS", 36, "Australia", "AUSTRALIE");
            new CountryCodeAdaptee("AT", "AUT", 40, "Austria", "AUTRICHE");
            new CountryCodeAdaptee("AZ", "AZE", 31, "Azerbaijan", "AZERBAÏDJAN");
            new CountryCodeAdaptee("BS", "BHS", 44, "Bahamas", "BAHAMAS");
            new CountryCodeAdaptee("BH", "BHR", 48, "Bahrain", "BAHREÏN");
            new CountryCodeAdaptee("BD", "BGD", 50, "Bangladesh", "BANGLADESH");
            new CountryCodeAdaptee("BB", "BRB", 52, "Barbados", "BARBADE");
            new CountryCodeAdaptee("BY", "BLR", 112, "Belarus", "BÉLARUS");
            new CountryCodeAdaptee("BE", "BEL", 56, "Belgium", "BELGIQUE");
            new CountryCodeAdaptee("BZ", "BLZ", 84, "Belize", "BELIZE");
            new CountryCodeAdaptee("BJ", "BEN", 204, "Benin", "BÉNIN");
            new CountryCodeAdaptee("BM", "BMU", 60, "Bermuda", "BERMUDES");
            new CountryCodeAdaptee("BT", "BTN", 64, "Bhutan", "BHOUTAN");
            new CountryCodeAdaptee("BO", "BOL", 68, "Bolivia, Plurinational State of", "BOLIVIE, l'ÉTAT PLURINATIONAL DE");
            new CountryCodeAdaptee("BQ", "BES", 535, "Bonaire, Sint Eustatius and Saba", "BONAIRE, SAINT-EUSTACHE ET SABA","NETHERLANDS ANTILLES");
            new CountryCodeAdaptee("BA", "BIH", 70, "Bosnia and Herzegovina", "BOSNIE-HERZÉGOVINE");
            new CountryCodeAdaptee("BW", "BWA", 72, "Botswana", "BOTSWANA");
            new CountryCodeAdaptee("BV", "BVT", 74, "Bouvet Island", "BOUVET, ÎLE");
            new CountryCodeAdaptee("BR", "BRA", 76, "Brazil", "BRÉSIL");
            new CountryCodeAdaptee("IO", "IOT", 86, "British Indian Ocean Territory", "OCÉAN INDIEN, TERRITOIRE BRITANNIQUE DE L'", "BRITISH VIRGIN ISLANDS");
            new CountryCodeAdaptee("BN", "BRN", 96, "Brunei Darussalam", "BRUNEI DARUSSALAM");
            new CountryCodeAdaptee("BG", "BGR", 100, "Bulgaria", "BULGARIE");
            new CountryCodeAdaptee("BF", "BFA", 854, "Burkina Faso", "BURKINA FASO");
            new CountryCodeAdaptee("BI", "BDI", 108, "Burundi", "BURUNDI");
            new CountryCodeAdaptee("KH", "KHM", 116, "Cambodia", "CAMBODGE");
            new CountryCodeAdaptee("CM", "CMR", 120, "Cameroon", "CAMEROUN");
            new CountryCodeAdaptee("CA", "CAN", 124, "Canada", "CANADA");
            new CountryCodeAdaptee("CV", "CPV", 132, "Cape Verde", "CAP-VERT");
            new CountryCodeAdaptee("KY", "CYM", 136, "Cayman Islands", "CAÏMANS, ÎLES");
            new CountryCodeAdaptee("CF", "CAF", 140, "Central African Republic", "CENTRAFRICAINE, RÉPUBLIQUE");
            new CountryCodeAdaptee("TD", "TCD", 148, "Chad", "TCHAD");
            new CountryCodeAdaptee("CL", "CHL", 152, "Chile", "CHILI");
            new CountryCodeAdaptee("CN", "CHN", 156, "China", "CHINE");
            new CountryCodeAdaptee("CX", "CXR", 162, "Christmas Island", "CHRISTMAS, ÎLE");
            new CountryCodeAdaptee("CC", "CCK", 166, "Cocos (Keeling) Islands", "COCOS (KEELING), ÎLES");
            new CountryCodeAdaptee("CO", "COL", 170, "Colombia", "COLOMBIE");
            new CountryCodeAdaptee("KM", "COM", 174, "Comoros", "COMORES");
            new CountryCodeAdaptee("CG", "COG", 178, "Congo", "CONGO");
            new CountryCodeAdaptee("CD", "COD", 180, "Congo, the Democratic Republic of the", "CONGO, LA RÉPUBLIQUE DÉMOCRATIQUE DU");
            new CountryCodeAdaptee("CK", "COK", 184, "Cook Islands", "COOK, ÎLES");
            new CountryCodeAdaptee("CR", "CRI", 188, "Costa Rica", "COSTA RICA");
            new CountryCodeAdaptee("CI", "CIV", 384, "Cote d'Ivoire", "CÔTE D'IVOIRE");
            new CountryCodeAdaptee("HR", "HRV", 191, "Croatia", "CROATIE");
            new CountryCodeAdaptee("CU", "CUB", 192, "Cuba", "CUBA");
            new CountryCodeAdaptee("CW", "CUW", 531, "Curaçao", "CURAÇAO", "CURACAO");
            new CountryCodeAdaptee("CY", "CYP", 196, "Cyprus", "CHYPRE");
            new CountryCodeAdaptee("CZ", "CZE", 203, "Czech Republic", "TCHÈQUE, RÉPUBLIQUE");
            new CountryCodeAdaptee("DK", "DNK", 208, "Denmark", "DANEMARK");
            new CountryCodeAdaptee("DJ", "DJI", 262, "Djibouti", "DJIBOUTI");
            new CountryCodeAdaptee("DM", "DMA", 212, "Dominica", "DOMINIQUE");
            new CountryCodeAdaptee("DO", "DOM", 214, "Dominican Republic", "DOMINICAINE, RÉPUBLIQUE");
            new CountryCodeAdaptee("EC", "ECU", 218, "Ecuador", "ÉQUATEUR");
            new CountryCodeAdaptee("EG", "EGY", 818, "Egypt", "ÉGYPTE");
            new CountryCodeAdaptee("SV", "SLV", 222, "El Salvador", "EL SALVADOR");
            new CountryCodeAdaptee("GQ", "GNQ", 226, "Equatorial Guinea", "GUINÉE ÉQUATORIALE");
            new CountryCodeAdaptee("ER", "ERI", 232, "Eritrea", "ÉRYTHRÉE");
            new CountryCodeAdaptee("EE", "EST", 233, "Estonia", "ESTONIE");
            new CountryCodeAdaptee("ET", "ETH", 231, "Ethiopia", "ÉTHIOPIE");
            new CountryCodeAdaptee("FK", "FLK", 238, "Falkland Islands (Malvinas)", "FALKLAND, ÎLES (MALVINAS)");
            new CountryCodeAdaptee("FO", "FRO", 234, "Faroe Islands", "FÉROÉ, ÎLES");
            new CountryCodeAdaptee("FJ", "FJI", 242, "Fiji", "FIDJI");
            new CountryCodeAdaptee("FI", "FIN", 246, "Finland", "FINLANDE");
            new CountryCodeAdaptee("FR", "FRA", 250, "France", "FRANCE");
            new CountryCodeAdaptee("GF", "GUF", 254, "French Guiana", "GUYANE FRANÇAISE");
            new CountryCodeAdaptee("PF", "PYF", 258, "French Polynesia", "POLYNÉSIE FRANÇAISE");
            new CountryCodeAdaptee("TF", "ATF", 260, "French Southern Territories", "TERRES AUSTRALES FRANÇAISES");
            new CountryCodeAdaptee("GA", "GAB", 266, "Gabon", "GABON");
            new CountryCodeAdaptee("GM", "GMB", 270, "Gambia", "GAMBIE");
            new CountryCodeAdaptee("GE", "GEO", 268, "Georgia", "GÉORGIE");
            new CountryCodeAdaptee("DE", "DEU", 276, "Germany", "ALLEMAGNE");
            new CountryCodeAdaptee("GH", "GHA", 288, "Ghana", "GHANA");
            new CountryCodeAdaptee("GI", "GIB", 292, "Gibraltar", "GIBRALTAR");
            new CountryCodeAdaptee("GR", "GRC", 300, "Greece", "GRÈCE");
            new CountryCodeAdaptee("GL", "GRL", 304, "Greenland", "GROENLAND");
            new CountryCodeAdaptee("GD", "GRD", 308, "Grenada", "GRENADE");
            new CountryCodeAdaptee("GP", "GLP", 312, "Guadeloupe", "GUADELOUPE");
            new CountryCodeAdaptee("GU", "GUM", 316, "Guam", "GUAM");
            new CountryCodeAdaptee("GT", "GTM", 320, "Guatemala", "GUATEMALA");
            new CountryCodeAdaptee("GG", "GGY", 831, "Guernsey", "GUERNESEY");
            new CountryCodeAdaptee("GN", "GIN", 324, "Guinea", "GUINÉE");
            new CountryCodeAdaptee("GW", "GNB", 624, "Guinea-Bissau", "GUINÉE-BISSAU");
            new CountryCodeAdaptee("GY", "GUY", 328, "Guyana", "GUYANA");
            new CountryCodeAdaptee("HT", "HTI", 332, "Haiti", "HAÏTI");
            new CountryCodeAdaptee("HM", "HMD", 334, "Heard Island and McDonald Islands", "HEARD-ET-ÎLES MACDONALD, ÎLE");
            new CountryCodeAdaptee("VA", "VAT", 336, "Holy See (Vatican City State)", "SAINT-SIÈGE (ÉTAT DE LA CITÉ DU VATICAN)");
            new CountryCodeAdaptee("HN", "HND", 340, "Honduras", "HONDURAS");
            new CountryCodeAdaptee("HK", "HKG", 344, "Hong Kong", "HONG KONG");
            new CountryCodeAdaptee("HU", "HUN", 348, "Hungary", "HONGRIE");
            new CountryCodeAdaptee("IS", "ISL", 352, "Iceland", "ISLANDE");
            new CountryCodeAdaptee("IN", "IND", 356, "India", "INDE");
            new CountryCodeAdaptee("ID", "IDN", 360, "Indonesia", "INDONÉSIE");
            new CountryCodeAdaptee("IR", "IRN", 364, "Iran, Islamic Republic of", "IRAN, RÉPUBLIQUE ISLAMIQUE D'");
            new CountryCodeAdaptee("IQ", "IRQ", 368, "Iraq", "IRAQ");
            new CountryCodeAdaptee("IE", "IRL", 372, "Ireland", "IRLANDE");
            new CountryCodeAdaptee("IM", "IMN", 833, "Isle of Man", "ÎLE DE MAN");
            new CountryCodeAdaptee("IL", "ISR", 376, "Israel", "ISRAËL");
            new CountryCodeAdaptee("IT", "ITA", 380, "Italy", "ITALIE");
            new CountryCodeAdaptee("JM", "JAM", 388, "Jamaica", "JAMAÏQUE");
            new CountryCodeAdaptee("JP", "JPN", 392, "Japan", "JAPON");
            new CountryCodeAdaptee("JE", "JEY", 832, "Jersey", "JERSEY");
            new CountryCodeAdaptee("JO", "JOR", 400, "Jordan", "JORDANIE");
            new CountryCodeAdaptee("KZ", "KAZ", 398, "Kazakhstan", "KAZAKHSTAN");
            new CountryCodeAdaptee("KE", "KEN", 404, "Kenya", "KENYA");
            new CountryCodeAdaptee("KI", "KIR", 296, "Kiribati", "KIRIBATI");
            new CountryCodeAdaptee("KP", "PRK", 408, "Korea, Democratic People's Republic of", "CORÉE, RÉPUBLIQUE POPULAIRE DÉMOCRATIQUE DE");
            new CountryCodeAdaptee("KR", "KOR", 410, "Korea, Republic of", "CORÉE, RÉPUBLIQUE DE");
            new CountryCodeAdaptee("KW", "KWT", 414, "Kuwait", "KOWEÏT");
            new CountryCodeAdaptee("KG", "KGZ", 417, "Kyrgyzstan", "KIRGHIZISTAN");
            new CountryCodeAdaptee("LA", "LAO", 418, "Lao People's Democratic Republic", "LAO, RÉPUBLIQUE DÉMOCRATIQUE POPULAIRE");
            new CountryCodeAdaptee("LV", "LVA", 428, "Latvia", "LETTONIE");
            new CountryCodeAdaptee("LB", "LBN", 422, "Lebanon", "LIBAN");
            new CountryCodeAdaptee("LS", "LSO", 426, "Lesotho", "LESOTHO");
            new CountryCodeAdaptee("LR", "LBR", 430, "Liberia", "LIBÉRIA");
            new CountryCodeAdaptee("LY", "LBY", 434, "Libyan Arab Jamahiriya", "LIBYE");
            new CountryCodeAdaptee("LI", "LIE", 438, "Liechtenstein", "LIECHTENSTEIN");
            new CountryCodeAdaptee("LT", "LTU", 440, "Lithuania", "LITUANIE");
            new CountryCodeAdaptee("LU", "LUX", 442, "Luxembourg", "LUXEMBOURG");
            new CountryCodeAdaptee("MO", "MAC", 446, "Macao", "MACAO");
            new CountryCodeAdaptee("MK", "MKD", 807, "Macedonia, the former Yugoslav Republic of", "MACÉDOINE, L'EX-RÉPUBLIQUE YOUGOSLAVE DE");
            new CountryCodeAdaptee("MG", "MDG", 450, "Madagascar", "MADAGASCAR");
            new CountryCodeAdaptee("MW", "MWI", 454, "Malawi", "MALAWI");
            new CountryCodeAdaptee("MY", "MYS", 458, "Malaysia", "MALAISIE");
            new CountryCodeAdaptee("MV", "MDV", 462, "Maldives", "MALDIVES");
            new CountryCodeAdaptee("ML", "MLI", 466, "Mali", "MALI");
            new CountryCodeAdaptee("MT", "MLT", 470, "Malta", "MALTE");
            new CountryCodeAdaptee("MH", "MHL", 584, "Marshall Islands", "MARSHALL, ÎLES");
            new CountryCodeAdaptee("MQ", "MTQ", 474, "Martinique", "MARTINIQUE");
            new CountryCodeAdaptee("MR", "MRT", 478, "Mauritania", "MAURITANIE");
            new CountryCodeAdaptee("MU", "MUS", 480, "Mauritius", "MAURICE");
            new CountryCodeAdaptee("YT", "MYT", 175, "Mayotte", "MAYOTTE");
            new CountryCodeAdaptee("MX", "MEX", 484, "Mexico", "MEXIQUE");
            new CountryCodeAdaptee("FM", "FSM", 583, "Micronesia, Federated States of", "MICRONÉSIE, ÉTATS FÉDÉRÉS DE");
            new CountryCodeAdaptee("MD", "MDA", 498, "Moldova, Republic of", "MOLDOVA, RÉPUBLIQUE DE");
            new CountryCodeAdaptee("MC", "MCO", 492, "Monaco", "MONACO");
            new CountryCodeAdaptee("MN", "MNG", 496, "Mongolia", "MONGOLIE");
            new CountryCodeAdaptee("ME", "MNE", 499, "Montenegro", "MONTÉNÉGRO");
            new CountryCodeAdaptee("MS", "MSR", 500, "Montserrat", "MONTSERRAT");
            new CountryCodeAdaptee("MA", "MAR", 504, "Morocco", "MAROC");
            new CountryCodeAdaptee("MZ", "MOZ", 508, "Mozambique", "MOZAMBIQUE");
            new CountryCodeAdaptee("MM", "MMR", 104, "Myanmar", "MYANMAR");
            new CountryCodeAdaptee("NA", "NAM", 516, "Namibia", "NAMIBIE");
            new CountryCodeAdaptee("NR", "NRU", 520, "Nauru", "NAURU");
            new CountryCodeAdaptee("NP", "NPL", 524, "Nepal", "NÉPAL");
            new CountryCodeAdaptee("NL", "NLD", 528, "Netherlands", "PAYS-BAS");
            new CountryCodeAdaptee("NC", "NCL", 540, "New Caledonia", "NOUVELLE-CALÉDONIE");
            new CountryCodeAdaptee("NZ", "NZL", 554, "New Zealand", "NOUVELLE-ZÉLANDE");
            new CountryCodeAdaptee("NI", "NIC", 558, "Nicaragua", "NICARAGUA");
            new CountryCodeAdaptee("NE", "NER", 562, "Niger", "NIGER");
            new CountryCodeAdaptee("NG", "NGA", 566, "Nigeria", "NIGÉRIA");
            new CountryCodeAdaptee("NU", "NIU", 570, "Niue", "NIUÉ");
            new CountryCodeAdaptee("NF", "NFK", 574, "Norfolk Island", "NORFOLK, ÎLE");
            new CountryCodeAdaptee("MP", "MNP", 580, "Northern Mariana Islands", "MARIANNES DU NORD, ÎLES");
            new CountryCodeAdaptee("NO", "NOR", 578, "Norway", "NORVÈGE");
            new CountryCodeAdaptee("OM", "OMN", 512, "Oman", "OMAN");
            new CountryCodeAdaptee("PK", "PAK", 586, "Pakistan", "PAKISTAN");
            new CountryCodeAdaptee("PW", "PLW", 585, "Palau", "PALAOS");
            new CountryCodeAdaptee("PS", "PSE", 275, "Palestinian Territory, Occupied", "PALESTINIEN OCCUPÉ, TERRITOIRE");
            new CountryCodeAdaptee("PA", "PAN", 591, "Panama", "PANAMA");
            new CountryCodeAdaptee("PG", "PNG", 598, "Papua New Guinea", "PAPOUASIE-NOUVELLE-GUINÉE");
            new CountryCodeAdaptee("PY", "PRY", 600, "Paraguay", "PARAGUAY");
            new CountryCodeAdaptee("PE", "PER", 604, "Peru", "PÉROU");
            new CountryCodeAdaptee("PH", "PHL", 608, "Philippines", "PHILIPPINES");
            new CountryCodeAdaptee("PN", "PCN", 612, "Pitcairn", "PITCAIRN");
            new CountryCodeAdaptee("PL", "POL", 616, "Poland", "POLOGNE");
            new CountryCodeAdaptee("PT", "PRT", 620, "Portugal", "PORTUGAL");
            new CountryCodeAdaptee("PR", "PRI", 630, "Puerto Rico", "PORTO RICO");
            new CountryCodeAdaptee("QA", "QAT", 634, "Qatar", "QATAR");
            new CountryCodeAdaptee("RE", "REU", 638, "Reunion ! Réunion", "RÉUNION");
            new CountryCodeAdaptee("RO", "ROU", 642, "Romania", "ROUMANIE");
            new CountryCodeAdaptee("RU", "RUS", 643, "Russian Federation", "RUSSIE, FÉDÉRATION DE");
            new CountryCodeAdaptee("RW", "RWA", 646, "Rwanda", "RWANDA");
            new CountryCodeAdaptee("BL", "BLM", 652, "Saint Barthélemy", "SAINT-BARTHÉLEMY");
            new CountryCodeAdaptee("SH", "SHN", 654, "Saint Helena, Ascension and Tristan da Cunha", "SAINTE-HÉLÈNE, ASCENSION ET TRISTAN DA CUNHA");
            new CountryCodeAdaptee("KN", "KNA", 659, "Saint Kitts and Nevis", "SAINT-KITTS-ET-NEVIS");
            new CountryCodeAdaptee("LC", "LCA", 662, "Saint Lucia", "SAINTE-LUCIE");
            new CountryCodeAdaptee("MF", "MAF", 663, "Saint Martin (French part)", "SAINT-MARTIN(PARTIE FRANÇAISE)");
            new CountryCodeAdaptee("PM", "SPM", 666, "Saint Pierre and Miquelon", "SAINT-PIERRE-ET-MIQUELON");
            new CountryCodeAdaptee("VC", "VCT", 670, "Saint Vincent and the Grenadines", "SAINT-VINCENT-ET-LES GRENADINES");
            new CountryCodeAdaptee("WS", "WSM", 882, "Samoa", "SAMOA");
            new CountryCodeAdaptee("SM", "SMR", 674, "San Marino", "SAINT-MARIN");
            new CountryCodeAdaptee("ST", "STP", 678, "Sao Tome and Principe", "SAO TOMÉ-ET-PRINCIPE");
            new CountryCodeAdaptee("SA", "SAU", 682, "Saudi Arabia", "ARABIE SAOUDITE");
            new CountryCodeAdaptee("SN", "SEN", 686, "Senegal", "SÉNÉGAL");
            new CountryCodeAdaptee("RS", "SRB", 688, "Serbia", "SERBIE");
            new CountryCodeAdaptee("SC", "SYC", 690, "Seychelles", "SEYCHELLES");
            new CountryCodeAdaptee("SL", "SLE", 694, "Sierra Leone", "SIERRA LEONE");
            new CountryCodeAdaptee("SG", "SGP", 702, "Singapore", "SINGAPOUR");
            new CountryCodeAdaptee("SX", "SXM", 534, "Sint Maarten (Dutch part)", "SAINT-MARTIN (PARTIE NÉERLANDAISE)");
            new CountryCodeAdaptee("SK", "SVK", 703, "Slovakia", "SLOVAQUIE");
            new CountryCodeAdaptee("SI", "SVN", 705, "Slovenia", "SLOVÉNIE");
            new CountryCodeAdaptee("SB", "SLB", 90, "Solomon Islands", "SALOMON, ÎLES");
            new CountryCodeAdaptee("SO", "SOM", 706, "Somalia", "SOMALIE");
            new CountryCodeAdaptee("ZA", "ZAF", 710, "South Africa", "AFRIQUE DU SUD");
            new CountryCodeAdaptee("GS", "SGS", 239, "South Georgia and the South Sandwich Islands", "GÉORGIE DU SUD-ET-LES ÎLES SANDWICH DU SUD");
            new CountryCodeAdaptee("SS", "SSD", 728, "South Sudan", "SOUDAN DU SUD");
            new CountryCodeAdaptee("ES", "ESP", 724, "Spain", "ESPAGNE");
            new CountryCodeAdaptee("LK", "LKA", 144, "Sri Lanka", "SRI LANKA");
            new CountryCodeAdaptee("SD", "SDN", 729, "Sudan", "SOUDAN");
            new CountryCodeAdaptee("SR", "SUR", 740, "Suriname", "SURINAME");
            new CountryCodeAdaptee("SJ", "SJM", 744, "Svalbard and Jan Mayen", "SVALBARD ET ÎLE JAN MAYEN");
            new CountryCodeAdaptee("SZ", "SWZ", 748, "Swaziland", "SWAZILAND");
            new CountryCodeAdaptee("SE", "SWE", 752, "Sweden", "SUÈDE");
            new CountryCodeAdaptee("CH", "CHE", 756, "Switzerland", "SUISSE");
            new CountryCodeAdaptee("SY", "SYR", 760, "Syrian Arab Republic", "SYRIENNE, RÉPUBLIQUE ARABE");
            new CountryCodeAdaptee("TW", "TWN", 158, "Taiwan, Province of China", "TAÏWAN, PROVINCE DE CHINE");
            new CountryCodeAdaptee("TJ", "TJK", 762, "Tajikistan", "TADJIKISTAN");
            new CountryCodeAdaptee("TZ", "TZA", 834, "Tanzania, United Republic of", "TANZANIE, RÉPUBLIQUE-UNIE DE");
            new CountryCodeAdaptee("TH", "THA", 764, "Thailand", "THAÏLANDE");
            new CountryCodeAdaptee("TL", "TLS", 626, "Timor-Leste", "TIMOR-LESTE");
            new CountryCodeAdaptee("TG", "TGO", 768, "Togo", "TOGO");
            new CountryCodeAdaptee("TK", "TKL", 772, "Tokelau", "TOKELAU");
            new CountryCodeAdaptee("TO", "TON", 776, "Tonga", "TONGA");
            new CountryCodeAdaptee("TT", "TTO", 780, "Trinidad and Tobago", "TRINITÉ-ET-TOBAGO");
            new CountryCodeAdaptee("TN", "TUN", 788, "Tunisia", "TUNISIE");
            new CountryCodeAdaptee("TR", "TUR", 792, "Turkey", "TURQUIE");
            new CountryCodeAdaptee("TM", "TKM", 795, "Turkmenistan", "TURKMÉNISTAN");
            new CountryCodeAdaptee("TC", "TCA", 796, "Turks and Caicos Islands", "TURKS-ET-CAÏCOS, ÎLES");
            new CountryCodeAdaptee("TV", "TUV", 798, "Tuvalu", "TUVALU");
            new CountryCodeAdaptee("UG", "UGA", 800, "Uganda", "OUGANDA");
            new CountryCodeAdaptee("UA", "UKR", 804, "Ukraine", "UKRAINE");
            new CountryCodeAdaptee("AE", "ARE", 784, "United Arab Emirates", "ÉMIRATS ARABES UNIS");
            new CountryCodeAdaptee("GB", "GBR", 826, "United Kingdom", "ROYAUME-UNI");
            new CountryCodeAdaptee("US", "USA", 840, "United States", "ÉTATS-UNIS", "USA");
            new CountryCodeAdaptee("UM", "UMI", 581, "United States Minor Outlying Islands", "ÎLES MINEURES ÉLOIGNÉES DES ÉTATS-UNIS");
            new CountryCodeAdaptee("UY", "URY", 858, "Uruguay", "URUGUAY");
            new CountryCodeAdaptee("UZ", "UZB", 860, "Uzbekistan", "OUZBÉKISTAN");
            new CountryCodeAdaptee("VU", "VUT", 548, "Vanuatu", "VANUATU");
            new CountryCodeAdaptee("VE", "VEN", 862, "Venezuela, Bolivarian Republic of", "VENEZUELA, RÉPUBLIQUE BOLIVARIENNE DU");
            new CountryCodeAdaptee("VN", "VNM", 704, "Viet Nam", "VIET NAM");
            new CountryCodeAdaptee("VG", "VGB", 92, "Virgin Islands, British", "ÎLES VIERGES BRITANNIQUES");
            new CountryCodeAdaptee("VI", "VIR", 850, "Virgin Islands, U.S.", "ÎLES VIERGES DES ÉTATS-UNIS");
            new CountryCodeAdaptee("WF", "WLF", 876, "Wallis and Futuna", "WALLIS ET FUTUNA");
            new CountryCodeAdaptee("EH", "ESH", 732, "Western Sahara", "SAHARA OCCIDENTAL");
            new CountryCodeAdaptee("YE", "YEM", 887, "Yemen", "YÉMEN");
            new CountryCodeAdaptee("ZM", "ZMB", 894, "Zambia", "ZAMBIE");
            new CountryCodeAdaptee("ZW", "ZWE", 716, "Zimbabwe", "ZIMBABWE");

            new CountryCodeAdaptee("XX", "XXX", 0, "Unknown", "INCONNU");
            new CountryCodeAdaptee("SU", "SUP", 1, "SUPRA NATIONAL", "SUPRA NATIONAL");
            new CountryCodeAdaptee("EU", "EUR", 1, "Europe", "EUROPE");
        }
        #endregion

    }

    /// <summary>
    /// Represente les pays avec leurs codes ISO 2 caractères, 3 caractères , numéri et libellés
    /// </summary>
    [NotMapped]
    internal class CountryCodeAdaptee
    {
        internal readonly string Label;
        internal readonly string LabelEnglish;
        internal readonly string Value;
        internal readonly string Value_3Chars;
        internal readonly int Value_Numerical;

        internal CountryCodeAdaptee(string Value, string Value_3Chars, int Value_Numerical, params string[] Labels)
        {
            for (int i=0; i<Labels.Length;i++)
            {
               if( i==0)
                   this.Label = Labels[0].ToUpper();;
               if (i == 1)
                   this.LabelEnglish = Labels[1].ToUpper();
               Constants[Labels[i]] = this;
            }
            
            this.Value = Value;
            this.Value_3Chars = Value_3Chars;
            this.Value_Numerical = Value_Numerical;
            Instances[Value] = this;
            Instances[Value_3Chars] = this;
        }

        public override String ToString()
        {
            return Label;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            CountryCodeAdaptee p = obj as CountryCodeAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }
        public override int GetHashCode()
        {
            return this.Value_Numerical;
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        internal static readonly Dictionary<string, CountryCodeAdaptee> Instances = new Dictionary<string, CountryCodeAdaptee>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Recherche par libellé
        /// </summary>
        internal static readonly Dictionary<string, CountryCodeAdaptee> Constants = new Dictionary<string, CountryCodeAdaptee>(StringComparer.OrdinalIgnoreCase);
    }
}
