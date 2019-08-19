using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Common
{

    enum MarketTypeCode
    {
        PrimaryMarket = 0,
        SecondaryMarket = 1,
        ThirdMarket = 2,
        FourthMarket = 3,
        OverTheCounter = 4,
        Various = 5,
        StockExchange = 6,
        Fund = 7,
        LocalMarket = 8,
        Theoretical = 9,
        Vendor = 10
    }

/// <summary>
    /// Les codes ISO 10383 MIC (Market Indentifier Code)
    /// http://www.iso15022.org/MIC/homepageMIC.htm
    /// </summary>
    [ComplexType]
    public sealed class Market
    {
        private MarketAdaptee InternalObject;

        public Market()
        {
        }

        internal Market(MarketAdaptee Instance)
        {
            if (Instance.Value != null)
            {
                this.InternalObject = Instance;
            }
        }

        [Column("Market"), MaxLength(4)]
        public string MarketId
        {
            get { return (InternalObject != null ? InternalObject.Value : null); }
            set
            {
                if (value == null) value = "XXXX";

                MarketAdaptee result;
                if (MarketAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    throw new InvalidCastException("Market inexistant :" + value);   // code qui n existe pas                  
            }
        }

        public bool validate()
        {
            return InternalObject.Value.Length == 4;
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator Market(string str)
        {
            if (str == null) str = "XXXX";

            MarketAdaptee result;
            if (MarketAdaptee.Instances.TryGetValue(str, out result))
                return new Market(result);
            else
                throw new InvalidCastException();
        }
        public override String ToString()
        {
            return InternalObject.ToString();
        }

        public override bool Equals(object obj)
        {
            Market code = obj as Market;
            return this.Equals(code);
        }

        public bool Equals(Market code)
        {
            if (code == null || code.InternalObject ==null || this.InternalObject ==null)
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

        public static Market getMarketByLabel(string label)
        {
            return new Market(MarketAdaptee.Constants[label]);
        }

        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static Market()
        {
            new MarketAdaptee("360T", "360T", "DE", "FRANKFURT", 0);
            new MarketAdaptee("AATS", "ASSENT ATS", "US", "JERSEY CITY", 0);
            new MarketAdaptee("ACEX", "ACE DERIVATIVES & COMMODITY EXCHANGE LTD", "IN", "MUMBAI", 0);
            new MarketAdaptee("AFET", "AGRICULTURAL FUTURES EXCHANGE OF THAILAND", "TH", "BANGKOK", 0);
            new MarketAdaptee("ALDP", "NYSE ALTERNEXT DARK", "US", "NEW YORK", 0, "AMEXDARK");
            new MarketAdaptee("ALTX", "ALTERNATIVE EXCHANGE", "ZA", "JOHANNESBURG", 0, "ALTX");
            new MarketAdaptee("ALXA", "NYSE EURONEXT - ALTERNEXT AMSTERDAM", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("ALXB", "NYSE EURONEXT - ALTERNEXT BRUSSELS", "BE", "BRUSSELS", 0);
            new MarketAdaptee("ALXL", "NYSE EURONEXT - ALTERNEXT LISBON", "PT", "LISBON", 0);
            new MarketAdaptee("ALXP", "NYSE EURONEXT - ALTERNEXT PARIS", "FR", "PARIS", 0);
            new MarketAdaptee("AMTS", "MTS NETHERLANDS", "GB", "LONDON", 0);
            new MarketAdaptee("AMXO", "NYSE AMEX OPTIONS", "US", "NEW YORK", 0, "NYSE");
            new MarketAdaptee("APXL", "ASIA PACIFIC EXCHANGE LIMITED", "AU", "SYDNEY", 0);
            new MarketAdaptee("AQUA", "AQUA EQUITIES L.P.", "US", "NEW YORK", 0);
            new MarketAdaptee("ARCD", "ARCA DARK", "US", "NEW YORK", 0, "ARCADARK");
            new MarketAdaptee("ARCO", "NYSE ARCA OPTIONS", "US", "NEW YORK", 0);
            new MarketAdaptee("ARCX", "NYSE ARCA", "US", "NEW YORK", 0, "NYSE");
            new MarketAdaptee("ASXC", "ASX - CENTER POINT", "AU", "SYDNEY", 0);
            new MarketAdaptee("ASXP", "ASX - PUREMATCH", "AU", "SYDNEY", 0);
            new MarketAdaptee("ASXV", "ASX - VOLUMEMATCH", "AU", "SYDNEY", 0);
            new MarketAdaptee("AWBX", "AUSTRALIAN WHEAT BOARD", "AU", "MELBOURNE", 0, "AWB");
            new MarketAdaptee("AWEX", "AUSTRALIAN WOOL EXCHANGE", "AU", "LANE COVE", 0, "AWEX");
            new MarketAdaptee("BACE", "BOLSA DE CEREALES DE BUENOS AIRES", "AR", "BUENOS AIRES", 0);
            new MarketAdaptee("BALT", "THE BALTIC EXCHANGE", "GB", "LONDON", 0);
            new MarketAdaptee("BAML", "BANK OF AMERICA - MERRILL LYNCH ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("BAPX", "BALTPOOL", "LT", "VILNIUS", 0);
            new MarketAdaptee("BARD", "BARCLAYS LX", "US", "NEW YORK", 0, "BCAP LX");
            new MarketAdaptee("BARX", "BARCLAY'S ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("BATE", "BATS EUROPE", "GB", "LONDON", 0);
            new MarketAdaptee("BATO", "BATS EXCHANGE OPTIONS MARKET", "US", "NEW YORK", 0, "BATS");
            new MarketAdaptee("BATS", "BATS EXCHANGE", "US", "NEW YORK", 0, "BATS");
            new MarketAdaptee("BATY", "BATS Y-EXCHANGE, INC.", "US", "NEW YORK", 0, "BATS");
            new MarketAdaptee("BCFS", "BOLSA DE COMERCIO DE SANTA FE", "AR", "SANTA FE", 0);
            new MarketAdaptee("BCMM", "BOLSA DE CEREAIS A MERCADORIAS DE MARINGA", "BR", "MARINGA", 0);
            new MarketAdaptee("BCSE", "BELARUS CURRENCY AND STOCK EXCHANGE", "BY", "MINSK", 0, "BCSE");
            new MarketAdaptee("BEEX", "BOND ELECTRONIC EXCHANGE", "TH", "BANGKOK", 0);
            new MarketAdaptee("BERA", "BOERSE BERLIN - REGULIERTER MARKT", "DE", "BERLIN", 0);
            new MarketAdaptee("BERB", "BOERSE BERLIN - FREIVERKEHR", "DE", "BERLIN", 0);
            new MarketAdaptee("BERC", "BOERSE BERLIN - BERLIN SECOND REGULATED MARKET", "DE", "BERLIN", 0);
            new MarketAdaptee("BFEX", "BAHRAIN FINANCIAL EXCHANGE", "BH", "MANAMA", 0);
            new MarketAdaptee("BGCF", "BGC FINANCIAL INC", "US", "NEW YORK", 0);
            new MarketAdaptee("BGCI", "BGC BROKERS LP", "GB", "LONDON", 0);
            new MarketAdaptee("BIDS", "BIDS TRADING L.P.", "US", "NEW YORK", 0, "BIDS");
            new MarketAdaptee("BLKX", "BLOCKCROSS", "GB", "LONDON", 0);
            new MarketAdaptee("BLNK", "BLINK", "GB", "LONDON", 0);
            new MarketAdaptee("BLOX", "BLOCKMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("BLPX", "BELGIAN POWER EXCHANGE", "BE", "BRUSSELS", 0, "BLPX");
            new MarketAdaptee("BLTD", "BLOOMBERG TRADEBOOK LLC", "US", "NEW YORK", 0);
            new MarketAdaptee("BMFA", "BMFMS-ATS", "RO", "SIBIU", 0);
            new MarketAdaptee("BMFM", "DERIVATIVES REGULATED MARKET - BMFMS", "RO", "SIBIU", 0, "BMFMS");
            new MarketAdaptee("BMFX", "SIBIU MONETARY- FINANCIAL AND COMMODITIES EXCHANGE", "RO", "SIBIU", 0);
            new MarketAdaptee("BMTS", "MTS BELGIUM", "BE", "BRUSSELS", 0);
            new MarketAdaptee("BNDD", "BONDDESK", "US", "MILL VALLEY", 0);
            new MarketAdaptee("BOAT", "MARKIT BOAT", "GB", "LONDON", 0);
            new MarketAdaptee("BOND", "BONDVISION", "IT", "ROMA", 0);
            new MarketAdaptee("BOSC", "BONDSCAPE", "GB", "LONDON", 0);
            new MarketAdaptee("BOSD", "NASDAQ OMX BX DARK", "US", "BOSTON", 0, "NQBXDARK");
            new MarketAdaptee("BOSP", "BONDSPOT MTF", "PL", "WARSZAWA", 0);
            new MarketAdaptee("BOVA", "BOLSA DE CORREDORES - BOLSA DE VALORES", "CL", "VALPARAISO", 0);
            new MarketAdaptee("BOVM", "BOLSA DE VALORES MINAS", "BR", "ESPIRITO SANTO", 0);
            new MarketAdaptee("BRIX", "BRAZILIAN ENERGY EXCHANGE", "BR", "SAO PAULO", 0);
            new MarketAdaptee("BSEX", "BAKU STOCK EXCHANGE", "AZ", "BAKU", 0);
            new MarketAdaptee("BTEC", "ICAP ELECTRONIC BROKING (US)", "US", "JERSEY CITY", 0);
            new MarketAdaptee("BTEE", "ICAP ELECTRONIC BROKING (EUROPE)", "GB", "LONDON", 0);
            new MarketAdaptee("BURG", "BURGUNDY NORDIC MTF", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("BURM", "BURGUNDY REGULATED MARKET", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("BVCA", "BOLSA ELECTRONICA DE VALORES DE CARACAS", "VE", "CARACAS", 0);
            new MarketAdaptee("BVMF", "BM&FBOVESPA S.A. - BOLSA DE VALORES, MERCADORIAS E FUTUROS", "BR", "SAO PAULO", 0);
            new MarketAdaptee("BVUR", "BOLSA ELECTRONICA DE VALORES DEL URUGUAY", "UY", "MONTEVIDEO", 0);
            new MarketAdaptee("C2OX", "C2 OPTIONS EXCHANGE INC.", "US", "CHICAGO", 0);
            new MarketAdaptee("CAES", "CREDIT SUISSE AES CROSSFINDER", "US", "NEW YORK", 0);
            new MarketAdaptee("CANX", "CANNEX FINANCIAL EXCHANGE LTS", "CA", "TORONTO", 0, "CANNEX");
            new MarketAdaptee("CATS", "CATS", "DE", "FRANKFURT", 0);
            new MarketAdaptee("CAZE", "THE CAZENOVE MTF", "GB", "LONDON", 0);
            new MarketAdaptee("CBSX", "CBOE STOCK EXCHANGE", "US", "CHICAGO", 0, "CBSX");
            new MarketAdaptee("CCFE", "CHICAGO CLIMATE FUTURES EXCHANGE", "US", "CHICAGO", 0);
            new MarketAdaptee("CCFX", "CHINA FINANCIAL FUTURES EXCHANGE ", "CN", "SHANGHAI", 0);
            new MarketAdaptee("CCLX", "FINESTI S.A.", "LU", "LUXEMBOURG", 0, "CCLux");
            new MarketAdaptee("CCO2", "CANTORCO2E.COM LIMITED", "GB", "LONDON", 0);
            new MarketAdaptee("CDED", "CITADEL DARK", "US", "NEW YORK", 0);
            new MarketAdaptee("CDEL", "CITADEL SECURITIES ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("CETI", "CETIP SA - BALCAO ORGANIZADO DE ATIVOS E DERIVATIVOS", "BR", "RIO DE JANEIRO", 0);
            new MarketAdaptee("CHIA", "CHI-X AUSTRALIA", "AU", "SYDNEY", 0);
            new MarketAdaptee("CHIC", "CHI-X CANADA ATS", "CA", "TORONTO", 0);
            new MarketAdaptee("CHIE", "CHI-EAST", "SG", "SINGAPORE", 0);
            new MarketAdaptee("CHIJ", "CHI-X JAPAN", "JP", "TOKYO", 0);
            new MarketAdaptee("CHIX", "CHI-X EUROPE LIMITED.", "GB", "LONDON", 0, "CHI-X");
            new MarketAdaptee("CHIY", "CHI-X EUROPE LIMITED - CHI-CLEAR", "GB", "LONDON", 0);
            new MarketAdaptee("CITD", "CITI DARK", "JP", "TOKYO", 0);
            new MarketAdaptee("CITX", "CITI MATCH", "JP", "TOKYO", 0);
            new MarketAdaptee("CLMX", "CLIMEX", "NL", "UTRECHT", 0);
            new MarketAdaptee("CMTS", "EUROCREDIT MTS", "GB", "LONDON", 0);
            new MarketAdaptee("COAL", "LA COTE ALPHA", "FR", "PARIS", 0);
            new MarketAdaptee("CSLP", "CREDIT SUISSE LIGHT POOL", "US", "NEW YORK", 0);
            new MarketAdaptee("CXRT", "CREDITEX REALTIME", "GB", "LONDON", 0);
            new MarketAdaptee("DAMP", "GXG MARKETS A/S", "DK", "COPENHAGEN", 0, "Dansk AMP");
            new MarketAdaptee("DBHK", "DEUTSCHE BANK HONG KONG ATS", "HK", "HONG KONG", 0);
            new MarketAdaptee("DBOX", "DEUTSCHE BANK OFF EXCHANGE TRADING", "DE", "FRANKFURT", 0);
            new MarketAdaptee("DBSX", "DEUTSCHE BANK ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("DEAL", "DEAL-X", "US", "NEW YORK", 0);
            new MarketAdaptee("DGCX", "DUBAI GOLD & COMMODITIES EXCHANGE DMCC", "AE", "DUBAI", 0, "DGCX");
            new MarketAdaptee("DIFX", "NASDAQ DUBAI", "AE", "DUBAI", 0, "DIFX");
            new MarketAdaptee("DKTC", "DANSK OTC", "DK", "HORSENS", 0);
            new MarketAdaptee("DSMD", "QATAR EXCHANGE", "QA", "DOHA", 0, "DSM");
            new MarketAdaptee("DUMX", "DUBAI MERCANTILE EXCHANGE", "AE", "DUBAI", 0);
            new MarketAdaptee("DUSA", "BOERSE DUESSELDORF - REGULIERTER MARKT", "DE", "DUESSELDORF", 0);
            new MarketAdaptee("DUSB", "BOERSE DUESSELDORF - FREIVERKEHR", "DE", "DUESSELDORF", 0);
            new MarketAdaptee("DUSC", "BOERSE DUESSELDORF - QUOTRIX - REGULIERTER MARKT", "DE", "DUESSELDORF", 0);
            new MarketAdaptee("DUSD", "BOERSE DUESSELDORF - QUOTRIX MTF", "DE", "DUESSELDORF", 0);
            new MarketAdaptee("ECAG", "EUREX CLEARING AG", "DE", "FRANKFURT", 0);
            new MarketAdaptee("ECXE", "EUROPEAN CLIMATE EXCHANGE", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("EDDP", "DIRECT EDGE X DARK", "US", "JERSEY CITY", 0, "EDGXDARK");
            new MarketAdaptee("EDGA", "EDGA EXCHANGE", "US", "JERSEY CITY", 0);
            new MarketAdaptee("EDGD", "DIRECT EDGE A DARK", "US", "JERSEY CITY", 0, "EDGADARK");
            new MarketAdaptee("EDGX", "EDGX EXCHANGE", "US", "JERSEY CITY", 0);
            new MarketAdaptee("EESE", "EAST EUROPEAN STOCK EXCHANGE", "UA", "KIEV", 0);
            new MarketAdaptee("EMDR", "E-MID - E-MIDER MARKET", "IT", "MILANO", 0);
            new MarketAdaptee("EMID", "E-MID", "IT", "MILANO", 0);
            new MarketAdaptee("EMTF", "EURO MTF", "LU", "LUXEMBOURG", 0);
            new MarketAdaptee("EMTS", "EUROMTS", "GB", "LONDON", 0, "EMTS");
            new MarketAdaptee("ENAX", "ATHENS EXCHANGE ALTERNATIVE MARKET", "GR", "ATHENS", 0, "ENAX");
            new MarketAdaptee("ENXB", "NYSE EURONEXT - EASY NEXT", "BE", "BRUSSELS", 0);
            new MarketAdaptee("ENXL", "NYSE EURONEXT - EASYNEXT LISBON", "PT", "LISBOA", 0);
            new MarketAdaptee("EOTC", "E-OTC", "HK", "HONG KONG", 0);
            new MarketAdaptee("EPEX", "EPEX SPOT SE", "FR", "PARIS", 0);
            new MarketAdaptee("EQTA", "BOERSE BERLIN EQUIDUCT TRADING - REGULIERTER MARKT", "DE", "BERLIN", 0);
            new MarketAdaptee("EQTB", "BOERSE BERLIN EQUIDUCT TRADING - BERLIN SECOND REGULATED MARKET", "DE", "BERLIN", 0);
            new MarketAdaptee("EQTC", "BOERSE BERLIN EQUIDUCT TRADING - FREIVERKEHR", "DE", "BERLIN", 0);
            new MarketAdaptee("EQTD", "BOERSE BERLIN EQUIDUCT TRADING - OTC", "DE", "BERLIN", 0);
            new MarketAdaptee("ETFP", "ELECTRONIC OPEN-END FUNDS AND ETC MARKET", "IT", "MILANO", 0, "ETFplus");
            new MarketAdaptee("ETLX", "EUROTLX", "IT", "MILANO", 0);
            new MarketAdaptee("ETSC", "ETS EURASIAN TRADING SYSTEM COMMODITY EXCHANGE", "KZ", "ALMATY", 0);
            new MarketAdaptee("EUAX", "ATHENS EXCHANGE EUAS MARKET", "GR", "ATHENS", 0);
            new MarketAdaptee("EUWX", "EUWAX", "DE", "STUTTGART", 0, "EUWAX");
            new MarketAdaptee("EXAA", "AUSTRIAN ENERGY EXCHANGE", "AT", "VIENNA", 0, "EXAA");
            new MarketAdaptee("FAIR", "CANTOR SPREADFAIR", "GB", "LONDON", 0);
            new MarketAdaptee("FCBT", "CHICAGO BOARD OF TRADE (FLOOR)", "US", "CHICAGO", 0, "CBOT (FLOOR)");
            new MarketAdaptee("FCME", "CHICAGO MERCANTILE EXCHANGE (FLOOR)", "US", "CHICAGO", 0, "CME (FLOOR)");
            new MarketAdaptee("FINN", "FINRA/NASDAQ TRF (TRADE REPORTING FACILITY)", "US", "WASHINGTON", 0);
            new MarketAdaptee("FINO", "FINRA ORF (TRADE REPORTING FACILITY)", "US", "WASHINGTON", 0);
            new MarketAdaptee("FINR", "FINRA ADF (TRADE REPORTING FACILITY)", "US", "WASHINGTON", 0);
            new MarketAdaptee("FINY", "FINRA/NYSE TRF (TRADE REPORTING FACILITY)", "US", "WASHINGTON", 0);
            new MarketAdaptee("FISH", "FISH POOL ASA", "NO", "BERGEN", 0);
            new MarketAdaptee("FMTS", "MTS FRANCE SAS", "FR", "PARIS", 0);
            new MarketAdaptee("FNEE", "FIRST NORTH ESTONIA", "EE", "TALLINN", 0);
            new MarketAdaptee("FNFI", "FIRST NORTH FINLAND", "FI", "HELSINKI", 0);
            new MarketAdaptee("FNLT", "FIRST NORTH LITHUANIA", "LT", "VILNIUS", 0);
            new MarketAdaptee("FNLV", "FIRST NORTH LATVIA", "LV", "RIGA", 0);
            new MarketAdaptee("FNSE", "FIRST NORTH STOCKHOLM", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("FRAA", "BOERSE FRANKFURT - REGULIERTER MARKT", "DE", "FRANKFURT", 0);
            new MarketAdaptee("FRAB", "BOERSE FRANKFURT - FREIVERKEHR", "DE", "FRANKFURT", 0);
            new MarketAdaptee("FRAD", "DEUTSCHE BOERSE MID-POINT CROSS", "DE", "FRANKFURT", 0, "DBMID");
            new MarketAdaptee("FRRF", "FONDS DES RENTES / RENTENFONDS", "BE", "BRUSSELS", 0);
            new MarketAdaptee("FSHX", "FISHEX", "NO", "TROMSO", 0);
            new MarketAdaptee("FXAL", "FXALL", "US", "NEW YORK", 0);
            new MarketAdaptee("FXCM", "FXCM", "US", "NEW YORK", 0);
            new MarketAdaptee("GBOT", "GLOBAL BOARD OF TRADE LTD", "MU", "EBENE", 0);
            new MarketAdaptee("GEMX", "GEMMA (GILT EDGED MARKET MAKERS’ASSOCIATION)", "GB", "LONDON", 0, "GEMMA");
            new MarketAdaptee("GFIA", "GFI AUCTIONMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("GFIC", "GFI CREDITMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("GFIF", "GFI FOREXMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("GFIM", "GFI MARKETMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("GFIN", "GFI ENERGYMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("GFIR", "GFI RATESMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("GLBX", "CME GLOBEX", "US", "CHICAGO", 0);
            new MarketAdaptee("GLLC", "GATE US LLC", "US", "NEW YORK", 0);
            new MarketAdaptee("GMEX", "GREENMARKET EXCHANGE", "DE", "MUENCHEN", 0);
            new MarketAdaptee("GMTF", "GALAXY", "FR", "PARIS", 0);
            new MarketAdaptee("GMTS", "MTS GERMANY", "GB", "LONDON", 0);
            new MarketAdaptee("GREE", "THE GREEN EXCHANGE", "US", "CHICAGO", 0);
            new MarketAdaptee("GSCI", "THE GUYANA ASSOCIATION OF SECURITIES COMPANIES AND INTERMEDIARIES INC.", "GY", "GEORGETOWN", 0);
            new MarketAdaptee("GTCO", "GETCO EXECUTION SERVICES, LLC", "US", "NEW YORK", 0);
            new MarketAdaptee("GXGM", "GXG MTF", "DK", "HORSENS", 0);
            new MarketAdaptee("GXMA", "GX MARKETCENTER", "VU", "VILA", 0);
            new MarketAdaptee("HAMA", "BOERSE HAMBURG - REGULIERTER MARKT", "DE", "HAMBURG", 0);
            new MarketAdaptee("HAMB", "BOERSE HAMBURG - FREIVERKEHR", "DE", "HAMBURG", 0);
            new MarketAdaptee("HANA", "BOERSE HANNOVER - REGULIERTER MARKT", "DE", "HANNOVER", 0);
            new MarketAdaptee("HANB", "BOERSE HANNOVER - FREIVERKEHR", "DE", "HANNOVER", 0);
            new MarketAdaptee("HDAT", "ELECTRONIC SECONDARY SECURITIES MARKET (HDAT)", "GR", "ATHENS", 0, "HDAT");
            new MarketAdaptee("HEGX", "NADEX", "US", "CHICAGO", 0);
            new MarketAdaptee("HKME", "HONG KONG MERCANTILE EXCHANGE", "HK", "HONG KONG", 0, "HKMEX");
            new MarketAdaptee("HMOD", "HI-MTF ORDER DRIVEN", "IT", "MILANO", 0);
            new MarketAdaptee("HMTF", "HI-MTF", "IT", "MILANO", 0);
            new MarketAdaptee("HOTC", "HELLENIC EXCHANGE OTC MARKET", "GR", "ATHENS", 0);
            new MarketAdaptee("HSFX", "HOTSPOT FX", "US", "JERSEY CITY", 0);
            new MarketAdaptee("HSTC", "HANOI STOCK EXCHANGE", "VN", "HANOI", 0, "HNX");
            new MarketAdaptee("HSXA", "HSBC-X HONG KONG", "HK", "HONG KONG", 0);
            new MarketAdaptee("HUNG", "MTS HUNGARY", "GB", "LONDON", 0);
            new MarketAdaptee("HUPX", "HUNGARIAN POWER EXCHANGE", "HU", "BUDAPEST", 0, "HUPX");
            new MarketAdaptee("IBLX", "INSTINET BLX", "US", "NEW YORK", 0);
            new MarketAdaptee("ICAH", "ICAP SHIPPING TANKER DERIVATIVES LIMITED", "GB", "LONDON", 0);
            new MarketAdaptee("ICAP", "ICAP EUROPE", "GB", "LONDON", 0);
            new MarketAdaptee("ICAS", "ICAP ENERGY AS", "NO", "BERGEN", 0);
            new MarketAdaptee("ICBX", "INSTINET CBX (US)", "US", "NEW YORK", 0);
            new MarketAdaptee("ICDX", "INDONESIA COMMODITY AND DERIVATIVES EXCHANGE", "ID", "JAKARTA", 0);
            new MarketAdaptee("ICEL", "ISLAND ECN LTD, THE ", "US", "NEW YORK", 0);
            new MarketAdaptee("ICEN", "ICAP ENERGY", "GB", "LONDON", 0);
            new MarketAdaptee("ICRO", "INSTINET VWAP CROSS", "US", "NEW YORK", 0);
            new MarketAdaptee("ICSE", "ICAP SECURITIES", "GB", "LONDON", 0);
            new MarketAdaptee("ICTQ", "ICAP TRUEQUOTE", "GB", "LONDON", 0);
            new MarketAdaptee("ICXL", "INDIAN COMMODITY EXCHANGE LTD.", "IN", "MUMBAI", 0);
            new MarketAdaptee("IEPA", "INTERCONTINENTAL EXCHANGE", "US", "ATLANTA", 0, "ICE");
            new MarketAdaptee("IFCA", "ICE FUTURES CANADA", "CA", "WINNIPEG", 0);
            new MarketAdaptee("IFEU", "INTERCONTINENTAL EXCHANGE - ICE FUTURES LIMITED", "GB", "LONDON", 0);
            new MarketAdaptee("IFUS", "ICE FUTURES U.S.", "US", "NEW YORK", 0);
            new MarketAdaptee("IIDX", "INSTINET IDX", "US", "NEW YORK", 0);
            new MarketAdaptee("IMAG", "ICE MARKETS AGRICULTURE", "US", "ATLANTA", 0);
            new MarketAdaptee("IMBD", "ICE MARKETS BONDS", "US", "ATLANTA", 0);
            new MarketAdaptee("IMCO", "ICE MARKETS COMMODITIES", "US", "ATLANTA", 0);
            new MarketAdaptee("IMCR", "ICE MARKETS CREDIT", "US", "ATLANTA", 0);
            new MarketAdaptee("IMEN", "ICE MARKETS ENERGY", "US", "ATLANTA", 0);
            new MarketAdaptee("IMEQ", "ICE MARKETS EQUITY", "US", "ATLANTA", 0);
            new MarketAdaptee("IMEX", "IRAN MERCANTILE EXCHANGE", "IR", "TEHRAN", 0);
            new MarketAdaptee("IMFX", "ICE MARKETS FOREIGN EXCHANGE", "US", "ATLANTA", 0);
            new MarketAdaptee("IMIR", "ICE MARKETS RATES", "US", "ATLANTA", 0);
            new MarketAdaptee("IMTS", "MTS IRELAND", "GB", "LONDON", 0);
            new MarketAdaptee("ISEC", "FIRST NORTH ICELAND", "IS", "REYKJAVIK", 0);
            new MarketAdaptee("ISEX", "INTER-CONNECTED STOCK EXCHANGE OF INDIA LTD", "IN", "MUMBAI", 0, "ISE");
            new MarketAdaptee("ITGI", "ITG - POSIT EXCHANGE", "US", "NEW YORK", 0);
            new MarketAdaptee("IXSP", "INTERNATIONAL STOCK EXCHANGE SAINT-PETERSBOURG", "RU", "SAINT-PETERSBURG", 0, "IXSP");
            new MarketAdaptee("JADX", "JOINT ASIAN DERIVATIVES EXCHANGE", "SG", "SINGAPORE", 0);
            new MarketAdaptee("JASR", "JAPANCROSSING", "JP", "TOKYO", 0);
            new MarketAdaptee("KABU", "KABU.COM PTS", "JP", "TOKYO", 0);
            new MarketAdaptee("KLEU", "KNIGHT LINK EUROPE", "GB", "LONDON", 0);
            new MarketAdaptee("KNCM", "KNIGHT CAPITAL MARKETS LLC", "US", "JERSEY CITY", 0);
            new MarketAdaptee("KNEM", "KNIGHT EQUITY MARKETS LP", "US", "JERSEY CITY", 0);
            new MarketAdaptee("KNLI", "KNIGHT LINK", "US", "JERSEY CITY", 0);
            new MarketAdaptee("KNMX", "KNIGHT MATCH ATS", "US", "JERSEY CITY", 0);
            new MarketAdaptee("LAFD", "FLOW DARK", "US", "NEW YORK", 0, "FLOWDARK");
            new MarketAdaptee("LAFL", "LAVAFLOW ECN", "US", "NEW YORK", 0);
            new MarketAdaptee("LAFX", "LAVAFX", "US", "NEW YORK", 0);
            new MarketAdaptee("LEVL", "LEVEL ATS", "US", "BOSTON", 0, "LEVEL");
            new MarketAdaptee("LICA", "LIQUIDNET CANADA ATS", "CA", "TORONTO", 0);
            new MarketAdaptee("LIQH", "LIQUIDNET H20", "GB", "LONDON", 0, "LQNT H20");
            new MarketAdaptee("LIQU", "LIQUIDNET SYSTEMS", "GB", "LONDON", 0);
            new MarketAdaptee("LMAD", "LMAX - DERIVATIVES", "GB", "LONDON", 0);
            new MarketAdaptee("LMAE", "LMAX - EQUITIES", "GB", "LONDON", 0);
            new MarketAdaptee("LMAF", "LMAX - FX", "GB", "LONDON", 0);
            new MarketAdaptee("LMAO", "LMAX - INDICES/RATES/COMMODITIES", "GB", "LONDON", 0);
            new MarketAdaptee("LMAX", "LMAX", "GB", "LONDON", 0);
            new MarketAdaptee("LMTS", "EUROGLOBALMTS", "GB", "LONDON", 0);
            new MarketAdaptee("LPPM", "LONDON PLATINUM AND PALLADIUM MARKET", "GB", "LONDON", 0, "LPPM");
            new MarketAdaptee("MABX", "MERCADO ALTERNATIVO BURSATIL", "ES", "MADRID", 0, "MAB");
            new MarketAdaptee("MACX", "MERCATO ALTERNATIVO DEL CAPITALE", "IT", "MILANO", 0, "MAC");
            new MarketAdaptee("MAEL", "MARKETAXESS EUROPE LIMITED", "GB", "LONDON", 0);
            new MarketAdaptee("MALX", "MALDIVES STOCK EXCHANGE", "MV", "MALE", 0);
            new MarketAdaptee("MATN", "MATCH NOW", "CA", "TORONTO", 0);
            new MarketAdaptee("MCXX", "MCX STOCK EXCHANGE LTD", "IN", "MUMBAI", 0);
            new MarketAdaptee("MCZK", "MTS CZECH REPUBLIC", "GB", "LONDON", 0);
            new MarketAdaptee("MDIP", "MEDIP  (MTS PORTUGAL SGMR, SA)", "PT", "LISBOA", 0, "MEDIP");
            new MarketAdaptee("MEAU", "MACQUARIE EXECUTION (AU)", "AU", "SYDNEY", 0);
            new MarketAdaptee("MEHK", "MACQUARIE EXECUTION (HK)", "HK", "HONG KONG", 0);
            new MarketAdaptee("MESQ", "ACE MARKET", "MY", "KUALA LUMPUR", 0);
            new MarketAdaptee("MFGL", "MF GLOBAL ENERGY MTF", "GB", "LONDON", 0);
            new MarketAdaptee("MFOX", "NYSE EURONEXT - MERCADO DE FUTUROS E OPÇÕES", "PT", "LISBOA", 0);
            new MarketAdaptee("MISX", "MICEX STOCK EXCHANGE", "RU", "MOSCOW", 0, "MICEX SE");
            new MarketAdaptee("MIVX", "MARKET FOR INVESTMENT VEHICULES", "IT", "MILANO", 0, "MIV");
            new MarketAdaptee("MLXB", "NYSE EURONEXT - MARCHE LIBRE BRUSSELS", "BE", "BRUSSELS", 0);
            new MarketAdaptee("MOTX", "ELECTRONIC BOND MARKET", "IT", "MILANO", 0, "MOT");
            new MarketAdaptee("MSPL", "MS POOL ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("MSTC", "MS TRAJECTORY CROSSING ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("MTAA", "ELECTRONIC SHARE MARKET", "IT", "MILANO", 0, "MTA");
            new MarketAdaptee("MTCH", "NYSE BONDMATCH", "FR", "PARIS", 0);
            new MarketAdaptee("MTSA", "MTS AUSTRIA", "GB", "LONDON", 0);
            new MarketAdaptee("MTSC", "MTS S.P.A.", "IT", "ROMA", 0, "MTS Italy");
            new MarketAdaptee("MTSD", "MTS DENMARK", "BE", "BRUSSELS", 0);
            new MarketAdaptee("MTSF", "MTS FINLAND", "BE", "BRUSSELS", 0);
            new MarketAdaptee("MTSG", "MTS GREECE", "GB", "LONDON", 0);
            new MarketAdaptee("MTSM", "MTS CORPORATE MARKET", "IT", "ROMA", 0, "MTS Italy");
            new MarketAdaptee("MTSP", "MTS POLAND", "PL", "WARSZAWA", 0);
            new MarketAdaptee("MTSS", "MTS SWAP MARKET", "GB", "LONDON", 0);
            new MarketAdaptee("MUNA", "BOERSE MUENCHEN - REGULIERTER MARKT", "DE", "MUENCHEN", 0);
            new MarketAdaptee("MUNB", "BOERSE MUENCHEN - FREIVERKEHR", "DE", "MUENCHEN", 0);
            new MarketAdaptee("MVCX", "MERCADO DE VALORES DE CORDOBA S.A.", "AR", "CORDOBA", 0);
            new MarketAdaptee("MYTR", "MYTREASURY", "GB", "LONDON", 0);
            new MarketAdaptee("N2EX", "N2EX", "GB", "LONDON", 0);
            new MarketAdaptee("NAMX", "NATIONAL MERCANTILE EXCHANGE", "RU", "MOSCOW", 0, "NAMEX");
            new MarketAdaptee("NASB", "NASDAQ OMX BALTIC", "LT", "VILNIUS", 0);
            new MarketAdaptee("NASD", "NSDQ DARK", "US", "NEW YORK", 0, "NSDQDARK");
            new MarketAdaptee("NBOT", "NATIONAL BOARD OF TRADE LIMITED", "IN", "INDORE MADHYA PRADESH", 0);
            new MarketAdaptee("NCEL", "NATIONAL COMMODITY EXCHANGE LIMITED", "PK", "KARACHI", 0, "NCEL");
            new MarketAdaptee("NDEX", "EUROPEAN ENERGY DERIVATIVES EXCHANGE N.V.", "NL", "AMSTERDAM", 0, "ENDEX");
            new MarketAdaptee("NFSC", "NATIONAL FINANCIAL SERVICES, LLC", "US", "BOSTON", 0);
            new MarketAdaptee("NFSD", "FIDELITY DARK", "US", "BOSTON", 0);
            new MarketAdaptee("NGXC", "NATURAL GAS EXCHANGE", "CA", "CALGARY", 0, "NGX");
            new MarketAdaptee("NILX", "NILE STOCK EXCHANGE", "EG", "CAIRO", 0);
            new MarketAdaptee("NLPX", "APX POWER NL", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("NMCE", "NATIONAL MULTI-COMMODITY EXCHANGE OF INDIA", "IN", "AHMEDABAD", 0);
            new MarketAdaptee("NMTF", "NORDIC MTF", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("NMTS", "NEW EUROMTS", "GB", "LONDON", 0);
            new MarketAdaptee("NNCS", "REGIONAL EXCHANGE CENTRE - MICEX VOLGA REGION", "RU", "NIZHNIY NOVGOROD ", 0, "NCSE");
            new MarketAdaptee("NODX", "NODAL EXCHANGE", "US", "VIENNA", 0);
            new MarketAdaptee("NOPS", "NORD POOL SPOT AS", "NO", "TRONDHEIM", 0);
            new MarketAdaptee("NORX", "NASDAQ OMX COMMODITIES", "NO", "OSLO", 0);
            new MarketAdaptee("NOTC", "NORWEGIAN OVER THE COUNTER MARKET", "NO", "OSLO", 0, "NOTC");
            new MarketAdaptee("NPGA", "NORD POOL GAS A/S", "DK", "COPENHAGEN", 0);
            new MarketAdaptee("NSXB", "BENDIGO STOCK EXCHANGE LIMITED", "AU", "MELBOURNE", 0, "BSX");
            new MarketAdaptee("NURD", "NASDAQ EUROPE (NURO) DARK", "GB", "LONDON", 0, "NURODARK");
            new MarketAdaptee("NURO", "NASDAQ OMX EUROPE", "GB", "LONDON", 0);
            new MarketAdaptee("NXEU", "NX", "GB", "LONDON", 0);
            new MarketAdaptee("NXUS", "NX ATS - CROSSING PLATFORM", "US", "NEW YORK", 0);
            new MarketAdaptee("NYFX", "MILLENNIUM", "US", "NEW YORK", 0);
            new MarketAdaptee("NYSD", "NYSE DARK", "US", "NEW YORK", 0, "NYSEDARK");
            new MarketAdaptee("NZFX", "NEW ZEALAND FUTURES & OPTIONS", "NZ", "AUCKLAND", 0, "NZFOX");
            new MarketAdaptee("OILX", "OILX", "GB", "LONDON", 0);
            new MarketAdaptee("OMEL", "OPERADOR DE MERCADO IBERICO DE ENERGIA - SPAIN", "ES", "MADRID", 0, "OMEL");
            new MarketAdaptee("OMGA", "OMEGA ATS", "CA", "TORONTO", 0);
            new MarketAdaptee("OMIP", "OPERADOR DE MERCADO IBERICO DE ENERGIA - PORTUGAL", "PT", "LISBOA", 0, "OMIP");
            new MarketAdaptee("OOTC", "OTC BULLETIN BOARD - OTHER OTC", "US", "WASHINGTON", 0, "OTCBB");
            new MarketAdaptee("OPEX", "PEX-PRIVATE EXCHANGE", "PT", "LISBOA", 0, "OPEX");
            new MarketAdaptee("OTCB", "OTCQB MARKETPLACE", "US", "NEW YORK", 0);
            new MarketAdaptee("OTCQ", "OTCQX", "US", "NEW YORK", 0);
            new MarketAdaptee("OTCX", "OTC EXCHANGE OF INDIA", "IN", "MUMBAI", 0, "OTCEI");
            new MarketAdaptee("PDEX", "PHILIPPINE DEALING AND EXCHANGE CORP", "PH", "MAKATI CITY", 0);
            new MarketAdaptee("PDQD", "PDQ ATS DARK", "US", "ILLINOIS", 0);
            new MarketAdaptee("PDQX", "PDQ ATS", "US", "ILLINOIS", 0);
            new MarketAdaptee("PFTQ", "PFTS QUOTE DRIVEN", "UA", "KIEV", 0);
            new MarketAdaptee("PFTS", "PFTS STOCK EXCHANGE", "UA", "KIEV", 0, "PFTS");
            new MarketAdaptee("PIEU", "PIPELINE - MTF", "GB", "LONDON", 0);
            new MarketAdaptee("PINX", "PINK OTC MARKETS INC. (NQB)", "US", "NEW YORK", 0);
            new MarketAdaptee("PIPE", "PIPELINE", "US", "NEW YORK", 0);
            new MarketAdaptee("PIRM", "PIRUM", "GB", "LONDON", 0);
            new MarketAdaptee("PLDX", "PLUS DERIVATIVES EXCHANGE", "GB", "LONDON", 0);
            new MarketAdaptee("PLPX", "TOWAROWA GIELDA ENERGII S.A. (POLISH POWER EXCHANGE)", "PL", "WARSZAWA", 0);
            new MarketAdaptee("PLUS", "BOERSE MUENCHEN - FREIVERKHER - PLUS - EUROPE", "DE", "MUENCHEN", 0);
            new MarketAdaptee("POEE", "POEE WARSAW STOCK EXCHANGE ENERGY MARKET", "PL", "WARSZAWA", 0);
            new MarketAdaptee("PRME", "MTS PRIME", "GB", "LONDON", 0);
            new MarketAdaptee("PRSE", "PRAGMA ATS", "US", "NEW YORK", 0);
            new MarketAdaptee("PSGM", "PINK SHEETS GREY MARKET", "US", "NEW YORK", 0);
            new MarketAdaptee("PULX", "BLOCKCROSS ATS", "US", "BOSTON", 0);
            new MarketAdaptee("PURE", "PURE TRADING", "CA", "TORONTO", 0);
            new MarketAdaptee("PXIL", "POWER EXCHANGE INDIA LTD.", "IN", "MUMBAI", 0);
            new MarketAdaptee("QMTF", "QUOTE MTF", "HU", "BUDAPEST", 0);
            new MarketAdaptee("QWIX", "Q-WIXX PLATFORM", "GB", "LONDON", 0);
            new MarketAdaptee("RBSX", "RBS CROSS", "GB", "LONDON", 0);
            new MarketAdaptee("RICD", "RIVERCROSS DARK", "US", "NARBERTH", 0, "RIVERX");
            new MarketAdaptee("RICX", "RIVERCROSS", "US", "NARBERTH", 0);
            new MarketAdaptee("RMTS", "MTS ISRAEL", "GB", "LONDON", 0);
            new MarketAdaptee("ROCO", "GRETAI SECURITIES MARKET", "TW", "TAIPEI", 0);
            new MarketAdaptee("ROFX", "ROSARIO FUTURE EXCHANGE", "AR", "ROSARIO", 0, "ROFEX");
            new MarketAdaptee("ROTC", "RWANDA OTC MARKET", "RW", "KIGALI", 0);
            new MarketAdaptee("RPDX", "MOSCOW ENERGY EXCHANGE", "RU", "MOSCOW", 0, "MEEX");
            new MarketAdaptee("RPWC", "BONDSPOT REGULATED MARKET", "PL", "WARSZAWA", 0);
            new MarketAdaptee("RSEX", "RWANDA STOCK EXCHANGE", "RW", "KIGALI", 0);
            new MarketAdaptee("RTSL", "REUTERS TRANSACTION SERVICES LIMITED", "GB", "LONDON", 0, "RTSL");
            new MarketAdaptee("RTSX", "RUSSIAN TRADING SYSTEM STOCK EXCHANGE", "RU", "MOSCOW", 0, "RTS");
            new MarketAdaptee("SBIJ", "JAPANNEXT PTS", "JP", "TOKYO", 0);
            new MarketAdaptee("SBMF", "SPOT REGULATED MARKET - BMFMS", "RO", "SIBIU", 0);
            new MarketAdaptee("SECF", "SECFINEX", "GB", "LONDON", 0);
            new MarketAdaptee("SEDX", "SECURITISED DERIVATIVES MARKET", "IT", "MILANO", 0, "SeDeX");
            new MarketAdaptee("SELC", "SISTEMA ESPECIAL DE LIQUIDACAO E CUSTODIA DE TITULOS PUBLICOS", "BR", "RIO DE JANEIRO", 0);
            new MarketAdaptee("SEND", "SEND - SISTEMA ELECTRONICO DE NEGOCIACION DE DEUDA", "ES", "MADRID", 0);
            new MarketAdaptee("SEPE", "STOCK EXCHANGE PERSPECTIVA", "UA", "DNIPROPETROVSK", 0);
            new MarketAdaptee("SGEX", "SHANGHAI GOLD EXCHANGE", "CN", "SHANGHAI", 0, "SGE");
            new MarketAdaptee("SGMA", "GOLDMAN SACH MTF", "US", "NEW YORK", 0);
            new MarketAdaptee("SGMX", "SIGMA X MTF", "GB", "LONDON", 0);
            new MarketAdaptee("SHAD", "D.E. SHAW DARK", "US", "NEW YORK", 0);
            new MarketAdaptee("SHAR", "SHAREMARK", "GB", "AYLESBURY", 0);
            new MarketAdaptee("SHAW", "D.E. SHAW", "US", "NEW YORK", 0);
            new MarketAdaptee("SIGX", "SIGMA X CANADA", "US", "BOSTON", 0);
            new MarketAdaptee("SMEX", "SINGAPORE MERCANTILE EXCHANGE PTE LTD", "SG", "SINGAPORE", 0);
            new MarketAdaptee("SMTS", "MTS SPAIN", "GB", "LONDON", 0);
            new MarketAdaptee("SPAD", "SPAD TRADING", "CZ", "PRAGUE", 0);
            new MarketAdaptee("SPEC", "SPECTRONLIVE", "GB", "LONDON", 0);
            new MarketAdaptee("SPIM", "ST. PETERSBURG INTERNATIONAL MERCANTILE EXCHANGE", "RU", "SAINT-PETERSBURG", 0, "SPIMEX");
            new MarketAdaptee("SPRZ", "SPREADZERO", "GB", "LONDON", 0);
            new MarketAdaptee("SSOB", "BOND VISION CORPORATE", "IT", "ROMA", 0);
            new MarketAdaptee("STUA", "BOERSE STUTTGART - REGULIERTER MARKT", "DE", "STUTTGART", 0);
            new MarketAdaptee("STUB", "BOERSE STUTTGART - FREIVERKEHR", "DE", "STUTTGART", 0);
            new MarketAdaptee("SWAP", "SWAPSTREAM", "GB", "LONDON", 0);
            new MarketAdaptee("TBEN", "TULLETT PREBON PLC - TULLET PREBON ENERGY", "GB", "LONDON", 0);
            new MarketAdaptee("TBLA", "TULLETT PREBON PLC - TP TRADEBLADE", "GB", "LONDON", 0);
            new MarketAdaptee("TBSP", "TREASURY BONDSPOT POLAND", "PL", "WARSZAWA", 0);
            new MarketAdaptee("TCDS", "TRADITION CDS", "GB", "LONDON", 0);
            new MarketAdaptee("TFEX", "THAILAND FUTURES EXCHANGE", "TH", "BANGKOK", 0, "TFEX");
            new MarketAdaptee("TFSG", "TFS GREEN SCREEN", "GB", "LONDON", 0);
            new MarketAdaptee("TFSS", "TFS VARIANCE SWAPS SYSTEM", "GB", "LONDON", 0);
            new MarketAdaptee("TFSV", "VOLBROKER", "GB", "LONDON", 0);
            new MarketAdaptee("THRD", "THIRD MARKET CORPORATION", "US", "CHICAGO", 0, "eTHRD");
            new MarketAdaptee("TMTS", "EUROBENCHMARK TRES. BILLS", "GB", "LONDON", 0);
            new MarketAdaptee("TMXS", "TMX SELECT", "CA", "TORONTO", 0);
            new MarketAdaptee("TNLA", "NYSE EURONEXT - TRADED BUT NOT LISTED AMSTERDAM", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("TNLB", "NYSE EURONEXT - TRADING FACILITY BRUSSELS", "BE", "BRUSSELS", 0);
            new MarketAdaptee("TOCP", "TORA CROSSPOINT", "HK", "HONG KONG", 0);
            new MarketAdaptee("TOMD", "TOM MTF DERIVATIVES MARKET", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("TOMX", "TOM MTF CASH MARKETS", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("TPCD", "TULLETT PREBON PLC - TP CREDITDEAL", "GB", "LONDON", 0);
            new MarketAdaptee("TPIE", "THE PROPERTY INVESTMENT EXCHANGE", "GB", "LONDON", 0);
            new MarketAdaptee("TPIM", "THE PROPERTY INVESTMENT MARKET", "GB", "LONDON", 0);
            new MarketAdaptee("TPRE", "TULLETT PREBON PLC - TP REPO", "GB", "LONDON", 0);
            new MarketAdaptee("TPSD", "TULLETT PREBON PLC - TP SWAPDEAL", "GB", "LONDON", 0);
            new MarketAdaptee("TRCK", "TRACK ECN", "US", "NEW YORK", 0);
            new MarketAdaptee("TRDE", "TRADITION ELECTRONIC TRADING PLATFORM", "GB", "LONDON", 0);
            new MarketAdaptee("TREU", "TRADEWEB EUROPE LIMITED ", "GB", "LONDON", 0);
            new MarketAdaptee("TRQD", "TURQUOISE DERIVATIVES MARKET", "GB", "LONDON", 0);
            new MarketAdaptee("TRQM", "TURQUOISE DARK", "GB", "LONDON", 0, "TQDARK");
            new MarketAdaptee("TRQX", "TURQUOISE", "GB", "LONDON", 0);
            new MarketAdaptee("TRWB", "TRADEWEB LLC", "US", "JERSEY CITY", 0);
            new MarketAdaptee("TSXV", "ALPHA ATS - TSX-V", "CA", "TORONTO", 0);
            new MarketAdaptee("UKEX", "UKRANIAN EXCHANGE", "UA", "KIEV", 0);
            new MarketAdaptee("UKGD", "UK GILTS MARKET", "GB", "LONDON", 0);
            new MarketAdaptee("UKPX", "APX POWER UK", "GB", "LONDON", 0);
            new MarketAdaptee("VEGA", "VEGA-CHI", "GB", "LONDON", 0);
            new MarketAdaptee("VKAB", "KABU.COMPTS-VWAP", "JP", "TOKYO", 0);
            new MarketAdaptee("VMFX", "THE FAROESE SECURITIES MARKET", "FO", "TORSHAVN", 0);
            new MarketAdaptee("VMTS", "MTS SLOVENIA", "GB", "LONDON", 0);
            new MarketAdaptee("VPXB", "NYSE EURONEXT - VENTES PUBLIQUES BRUSSELS", "BE", "BRUSSELS", 0);
            new MarketAdaptee("VTEX", "VORTEX", "US", "NEW YORK", 0);
            new MarketAdaptee("WBAH", "WIENER BOERSE AG AMTLICHER HANDEL (OFFICIAL MARKET) ", "AT", "VIENNA", 0);
            new MarketAdaptee("WBDM", "WIENER BOERSE AG DRITTER MARKT (THIRD MARKET)", "AT", "VIENNA", 0);
            new MarketAdaptee("WBGF", "WIENER BOERSE AG GEREGELTER FREIVERKEHR (SEMI-OFFICIAL MARKET)", "AT", "VIENNA", 0);
            new MarketAdaptee("WCLK", "ICAP WCLK", "GB", "LONDON", 0);
            new MarketAdaptee("WQXL", "NYSE EURONEXT - MARKET WITHOUT QUOTATIONS LISBON", "PT", "LISBOA", 0);
            new MarketAdaptee("XADE", "ATHENS EXCHANGE S.A. DERIVATIVES MARKET", "GR", "ATHENS", 0, "ATHEXD");
            new MarketAdaptee("XADF", "FINRA ALTERNATIVE DISPLAY FACILITY", "US", "WASHINGTON/NEW YORK", 0);
            new MarketAdaptee("XADS", "ABU DHABI SECURITIES EXCHANGE", "AE", "ABU DHABI", 0, "ADSM");
            new MarketAdaptee("XAFR", "ALTERNATIVA FRANCE", "FR", "PARIS", 0);
            new MarketAdaptee("XAIM", "AIM ITALIA", "IT", "MILANO", 0);
            new MarketAdaptee("XALG", "ALGIERS STOCK EXCHANGE", "DZ", "ALGIERS", 0);
            new MarketAdaptee("XALT", "ALTEX-ATS", "GB", "LONDON", 0);
            new MarketAdaptee("XAMM", "AMMAN STOCK EXCHANGE", "JO", "AMMAN", 0, "ASE");
            new MarketAdaptee("XAMS", "NYSE EURONEXT - EURONEXT AMSTERDAM", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("XAPI", "REGIONAL EXCHANGE CENTRE - MICEX FAR EAST", "RU", "VLADIVOSTOK", 0, "MICEX FAR EAST");
            new MarketAdaptee("XAQS", "AUTOMATED EQUITY FINANCE MARKETS", "US", "NEW YORK", 0, "AQS");
            new MarketAdaptee("XARM", "NASDAQ OMX ARMENIA", "AM", "YEREVAN", 0, "ARMEX");
            new MarketAdaptee("XASE", "NYSE AMEX EQUITIES", "US", "NEW YORK", 0, "AMEX");
            new MarketAdaptee("XASX", "ASX TRADEMATCH", "AU", "SYDNEY", 0, "ASX");
            new MarketAdaptee("XATH", "ATHENS EXCHANGE S.A. CASH MARKET", "GR", "ATHENS", 0, "ATHEXC");
            new MarketAdaptee("XATS", "ALPHA ATS", "CA", "TORONTO", 0);
            new MarketAdaptee("XBAA", "BAHAMAS INTERNATIONAL SECURITIES EXCHANGE", "BS", "NASAU", 0, "BISX");
            new MarketAdaptee("XBAB", "BARBADOS STOCK EXCHANGE", "BB", "BRIDGETOWN", 0, "BSE");
            new MarketAdaptee("XBAH", "BAHRAIN BOURSE", "BH", "MANAMA", 0, "BSE");
            new MarketAdaptee("XBAN", "BANGALORE STOCK EXCHANGE LTD", "IN", "BANGALORE", 0);
            new MarketAdaptee("XBAR", "BOLSA DE BARCELONA", "ES", "BARCELONA", 0);
            new MarketAdaptee("XBBJ", "JAKARTA FUTURES EXCHANGE (BURSA BERJANGKA JAKARTA)", "ID", "JAKARTA", 0, "BBJ");
            new MarketAdaptee("XBBK", "PERIMETER FINANCIAL CORP. - BLOCKBOOK ATS", "CA", "TORONTO", 0);
            new MarketAdaptee("XBCC", "BOLSA DE COMERCIO DE CORDOBA", "AR", "CORDOBA", 0);
            new MarketAdaptee("XBCL", "LA BOLSA ELECTRONICA DE CHILE", "CL", "SANTIAGO", 0, "BOLCHILE");
            new MarketAdaptee("XBCM", "BOLSA DE COMERCIO DE MENDOZA S.A.", "AR", "MENDOZA", 0);
            new MarketAdaptee("XBCV", "BOLSA CENTROAMERICANA DE VALORES S.A.", "HN", "TEGUCIGALPA", 0, "BCV");
            new MarketAdaptee("XBCX", "MERCADO DE VALORES DE MENDOZA S.A.", "AR", "MENDOZA", 0);
            new MarketAdaptee("XBDA", "BERMUDA STOCK EXCHANGE LTD", "BM", "HAMILTON", 0, "BSX");
            new MarketAdaptee("XBEL", "BELGRADE STOCK EXCHANGE", "RS", "BELGRADE", 0);
            new MarketAdaptee("XBER", "BOERSE BERLIN", "DE", "BERLIN", 1);
            new MarketAdaptee("XBES", "JSE - FORMER BESA", "ZA", "JOHANNESBURG", 0, "BESA");
            new MarketAdaptee("XBEY", "BOURSE DE BEYROUTH - BEIRUT STOCK EXCHANGE", "LB", "BEIRUT", 0, "BSE");
            new MarketAdaptee("XBIL", "BOLSA DE VALORES DE BILBAO", "ES", "BILBAO", 0);
            new MarketAdaptee("XBKF", "STOCK EXCHANGE OF THAILAND - FOREIGN BOARD", "TH", "BANGKOK", 0, "SET");
            new MarketAdaptee("XBKK", "STOCK EXCHANGE OF THAILAND", "TH", "BANGKOK", 0, "SET");
            new MarketAdaptee("XBLB", "BANJA LUKA STOCK EXCHANGE", "BA", "BANJA LUKA", 0);
            new MarketAdaptee("XBLN", "BLUENEXT", "FR", "PARIS", 0);
            new MarketAdaptee("XBNV", "BOLSA NACIONAL DE VALORES, S.A.", "CR", "SAN JOSE", 0, "BNV");
            new MarketAdaptee("XBOG", "BOLSA DE VALORES DE COLOMBIA", "CO", "BOGOTA", 0, "BVC");
            new MarketAdaptee("XBOL", "BOLSA BOLIVIANA DE VALORES S.A.", "BO", "LA PAZ", 0);
            new MarketAdaptee("XBOM", "BSE LTD.", "IN", "MUMBAI", 0, "MSE");
            new MarketAdaptee("XBOS", "NASDAQ OMX BX", "US", "BOSTON", 0, "BSE");
            new MarketAdaptee("XBOT", "BOTSWANA STOCK EXCHANGE", "BW", "GABORONE", 0);
            new MarketAdaptee("XBOX", "BOSTON OPTIONS EXCHANGE", "US", "BOSTON", 0, "BOX");
            new MarketAdaptee("XBRA", "BRATISLAVA STOCK EXCHANGE", "SK", "BRATISLAVA", 0, "BSSE");
            new MarketAdaptee("XBRD", "NYSE EURONEXT - EURONEXT BRUSSELS - DERIVATIVES", "BE", "BRUSSELS", 0);
            new MarketAdaptee("XBRM", "ROMANIAN  COMMODITIES EXCHANGE", "RO", "BUCHAREST", 0, "BRM");
            new MarketAdaptee("XBRN", "BERNE STOCK EXCHANGE", "CH", "BERNE", 0);
            new MarketAdaptee("XBRT", "BRUT ECN", "US", "NEW YORK", 0, "BRUT");
            new MarketAdaptee("XBRU", "NYSE EURONEXT - EURONEXT BRUSSELS", "BE", "BRUSSELS", 0);
            new MarketAdaptee("XBRV", "BOURSE REGIONALE DES VALEURS MOBILIERES", "CI", "ABIDJAN", 0, "BRVM");
            new MarketAdaptee("XBSD", "DERIVATIVES REGULATED MARKET - BVB", "RO", "BUCHAREST", 0, "REGF");
            new MarketAdaptee("XBSE", "SPOT REGULATED MARKET - BVB", "RO", "BUCHAREST", 0, "REGS");
            new MarketAdaptee("XBUD", "BUDAPEST STOCK EXCHANGE", "HU", "BUDAPEST", 0);
            new MarketAdaptee("XBUE", "BOLSA DE COMERCIO DE BUENOS AIRES", "AR", "BUENOS AIRES", 0, "BCBA");
            new MarketAdaptee("XBUL", "BULGARIAN STOCK EXCHANGE", "BG", "SOFIA", 0, "BSE");
            new MarketAdaptee("XBVC", "CAPE VERDE STOCK EXCHANGE", "CV", "PRAIA", 0, "BVC");
            new MarketAdaptee("XBVR", "BOLSA DE VALORES DE LA REPUBLICA DOMINICANA SA.", "DO", "SANTO DOMINGO", 0, "BVRD");
            new MarketAdaptee("XCAI", "EGYPTIAN EXCHANGE", "EG", "CAIRO", 0, "CASE");
            new MarketAdaptee("XCAL", "CALCUTTA STOCK EXCHANGE", "IN", "CALCUTTA", 0);
            new MarketAdaptee("XCAN", "CAN-ATS", "RO", "BUCHAREST", 0);
            new MarketAdaptee("XCAR", "BOLSA DE VALORES DE CARACAS", "VE", "CARACAS", 0);
            new MarketAdaptee("XCAS", "CASABLANCA STOCK EXCHANGE", "MA", "CASABLANCA", 0);
            new MarketAdaptee("XCAY", "CAYMAN ISLANDS STOCK EXCHANGE", "KY", "GEORGETOWN", 0);
            new MarketAdaptee("XCBF", "CBOE FUTURES EXCHANGE", "US", "CHICAGO", 0, "CFE");
            new MarketAdaptee("XCBO", "CHICAGO BOARD OPTIONS EXCHANGE", "US", "CHICAGO", 0, "CBOE");
            new MarketAdaptee("XCBT", "CHICAGO BOARD OF TRADE", "US", "CHICAGO", 0, "CBOT");
            new MarketAdaptee("XCCX", "CHICAGO CLIMATE EXCHANGE, INC", "US", "CHICAGO", 0, "CCX");
            new MarketAdaptee("XCDE", "BAXTER FINANCIAL SERVICES", "IE", "DUBLIN", 0);
            new MarketAdaptee("XCEC", "COMMODITIES EXCHANGE CENTER", "US", "NEW YORK", 0, "COMEX");
            new MarketAdaptee("XCEG", "WIENER BOERSE AG, CEGH GAS EXCHANGE", "AT", "VIENNA", 0);
            new MarketAdaptee("XCET", "UZBEK COMMODITY EXCHANGE", "UZ", "TASHKENT", 0);
            new MarketAdaptee("XCFE", "CHINA FOREIGN EXCHANGE TRADE SYSTEM ", "CN", "SHANGHAI", 0, "CFETS");
            new MarketAdaptee("XCFF", "CANTOR FINANCIAL FUTURES EXCHANGE", "US", "NEW YORK", 0, "CANTOR");
            new MarketAdaptee("XCGS", "CHINESE GOLD & SILVER EXCHANGE SOCIETY", "HK", "HONG KONG", 0);
            new MarketAdaptee("XCHG", "CHITTAGONG STOCK EXCHANGE LTD.", "BD", "CHITTAGONG", 0, "CSE");
            new MarketAdaptee("XCHI", "CHICAGO STOCK EXCHANGE, INC", "US", "CHICAGO", 0, "CHX");
            new MarketAdaptee("XCIE", "CHANNEL ISLANDS STOCK EXCHANGE", "GG", "ST.  PETER PORT", 0, "CISX");
            new MarketAdaptee("XCIS", "NATIONAL STOCK EXCHANGE", "US", "CHICAGO", 0);
            new MarketAdaptee("XCME", "CHICAGO MERCANTILE EXCHANGE", "US", "CHICAGO", 1, "CME");
            new MarketAdaptee("XCNF", "BOLSA DE COMERCIO CONFEDERADA S.A.", "AR", "CORRIENTES", 0, "BCC");
            new MarketAdaptee("XCNQ", "CANADIAN NATIONAL STOCK EXCHANGE", "CA", "TORONTO", 0, "CNSX");
            new MarketAdaptee("XCOL", "COLOMBO STOCK EXCHANGE", "LK", "COLOMBO", 0);
            new MarketAdaptee("XCOR", "ICMA", "GB", "LONDON", 0);
            new MarketAdaptee("XCSE", "OMX NORDIC EXCHANGE COPENHAGEN A/S", "DK", "COPENHAGEN", 0);
            new MarketAdaptee("XCUE", "UZBEKISTAN REPUBLICAN CURRENCY EXCHANGE", "UZ", "TASHKENT", 0);
            new MarketAdaptee("XCUR", "CURRENEX", "US", "NEW YORK", 0);
            new MarketAdaptee("XCYO", "CYPRUS STOCK EXCHANGE - OTC", "CY", "NICOSIA (LEFKOSIA)", 0);
            new MarketAdaptee("XCYS", "CYPRUS STOCK EXCHANGE", "CY", "NICOSIA (LEFKOSIA)", 0, "CSE");
            new MarketAdaptee("XDAR", "DAR ES  SALAAM STOCK EXCHANGE", "TZ", "DAR ES SALAAM", 0);
            new MarketAdaptee("XDBC", "DEUTSCHE BOERSE AG - CUSTOMIZED INDICES", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XDBV", "DEUTSCHE BOERSE AG - VOLATILITY INDICES", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XDBX", "DEUTSCHE BOERSE AG - INDICES", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XDCE", "DALIAN COMMODITY EXCHANGE", "CN", "DALIAN", 0, "DCE");
            new MarketAdaptee("XDES", "DELHI STOCK EXCHANGE", "IN", "DELHI", 0);
            new MarketAdaptee("XDFB", "JOINT-STOCK COMPANY “STOCK EXCHANGE INNEX”", "UA", "KIEV", 0);
            new MarketAdaptee("XDFM", "DUBAI FINANCIAL MARKET", "AE", "DUBAI", 0, "DFM");
            new MarketAdaptee("XDHA", "DHAKA STOCK EXCHANGE LTD", "BD", "DHAKA", 0, "DSE");
            new MarketAdaptee("XDMI", "ITALIAN DERIVATIVES MARKET", "IT", "MILANO", 0, "IDEM");
            new MarketAdaptee("XDPA", "CADE - MERCADO DE DEUDA PUBLICA ANOTADA", "ES", "MADRID", 0);
            new MarketAdaptee("XDRF", "AIAF - MERCADO DE RENTA FIJA", "ES", "MADRID", 0);
            new MarketAdaptee("XDSE", "DAMASCUS SECURITIES EXCHANGE", "SY", "DAMASCUS", 0);
            new MarketAdaptee("XDSX", "DOUALA STOCK EXCHANGE", "CM", "DOUALA", 0);
            new MarketAdaptee("XDUB", "IRISH STOCK EXCHANGE - ALL MARKET", "IE", "DUBLIN", 0, "ISE");
            new MarketAdaptee("XDUS", "BOERSE DUESSELDORF", "DE", "DUESSELDORF", 0);
            new MarketAdaptee("XECM", "MTF - CYPRUS EXCHANGE", "CY", "NICOSIA", 0);
            new MarketAdaptee("XECS", "EASTERN CARIBBEAN SECURITIES EXCHANGE", "KN", "BASSETERRE", 0, "ECSE");
            new MarketAdaptee("XEEE", "EUROPEAN ENERGY EXCHANGE AG", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XELX", "ELX", "US", "NEW YORK", 0);
            new MarketAdaptee("XEMD", "MERCADO MEXICANO DE DERIVADOS", "MX", "MEXICO", 0, "MEXDER");
            new MarketAdaptee("XEQT", "BOERSE BERLIN EQUIDUCT TRADING", "DE", "BERLIN", 0);
            new MarketAdaptee("XETA", "XETRA - REGULIERTER MARKT", "DE", "FRANKFURT", 0, "XETRA");
            new MarketAdaptee("XETB", "XETRA - FREIVERKEHR", "DE", "FRANKFURT", 0, "XETRA");
            new MarketAdaptee("XETC", "XETRA INTERNATIONAL MARKET - REGULATED MARKET", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XETD", "XETRA INTERNATIONAL MARKET - OPEN MARKET", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XETI", "XETRA INTERNATIONAL MARKET", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XETR", "XETRA", "DE", "FRANKFURT", 0, "XETRA");
            new MarketAdaptee("XEUB", "EUREX BONDS", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XEUC", "EURONEXT COM, COMMODITIES FUTURES AND OPTIONS", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("XEUE", "EURONEXT EQF, EQUITIES AND INDICES DERIVATIVES", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("XEUI", "EURONEXT IRF, INTEREST RATE FUTURE AND OPTIONS", "NL", "AMSTERDAM", 0);
            new MarketAdaptee("XEUP", "EUREX REPO GMBH", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XEUR", "EUREX DEUTSCHLAND", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XFCI", "FINANCIALCONTENT INDEXES", "US", "FOSTER CITY", 0);
            new MarketAdaptee("XFKA", "FUKUOKA STOCK EXCHANGE", "JP", "FUKUOKA", 0);
            new MarketAdaptee("XFND", "FIRST NORTH DENMARK", "DK", "COPENHAGEN", 0);
            new MarketAdaptee("XFRA", "DEUTSCHE BOERSE AG", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XGAT", "TRADEGATE EXCHANGE - FREIVERKEHR", "DE", "BERLIN", 0);
            new MarketAdaptee("XGEM", "HONG KONG GROWTH ENTERPRISES MARKET", "HK", "HONG KONG", 0, "HK GEM");
            new MarketAdaptee("XGFI", "GFI BASISMATCH", "GB", "LONDON", 0);
            new MarketAdaptee("XGHA", "GHANA STOCK EXCHANGE", "GH", "ACCRA", 0);
            new MarketAdaptee("XGME", "GESTORE MERCATO ELETTRICO - ITALIAN POWER EXCHANGE", "IT", "ROMA", 0, "IPEX/GME");
            new MarketAdaptee("XGRM", "TRADEGATE EXCHANGE - REGULIERTER MARKT", "DE", "BERLIN", 0);
            new MarketAdaptee("XGSE", "GEORGIA STOCK EXCHANGE", "GE", "TBILISI", 0, "GSE");
            new MarketAdaptee("XGTG", "BOLSA DE VALORES NACIONAL SA", "GT", "GUATEMALA", 0);
            new MarketAdaptee("XGUA", "BOLSA DE VALORES DE GUAYAQUIL", "EC", "GUAYAQUIL", 0);
            new MarketAdaptee("XHAM", "HANSEATISCHE WERTPAPIERBOERSE HAMBURG", "DE", "HAMBURG", 0);
            new MarketAdaptee("XHAN", "NIEDERSAECHSISCHE BOERSE ZU HANNOVER", "DE", "HANNOVER", 0);
            new MarketAdaptee("XHEL", "NASDAQ OMX HELSINKI LTD.", "FI", "HELSINKI", 0);
            new MarketAdaptee("XHFT", "NYSE ARCA EUROPE", "NL", "AMSTERDAM", 0, "NYSE");
            new MarketAdaptee("XHKF", "HONG KONG FUTURES EXCHANGE LTD.", "HK", "HONG KONG", 0, "HKFE");
            new MarketAdaptee("XHKG", "HONG KONG EXCHANGES AND CLEARING LTD", "HK", "HONG KONG", 0, "HKEX");
            new MarketAdaptee("XHNX", "HANOI STOCK EXCHANGE (UNLISTED PUBLIC COMPANY TRADING PLATFORM)", "VN", "HANOI", 0, "UPCOM");
            new MarketAdaptee("XIAB", "ISTANBUL GOLD EXCHANGE", "TR", "ISTANBUL", 0, "IAB");
            new MarketAdaptee("XIBE", "BAKU INTERBANK CURRENCY EXCHANGE", "AZ", "BAKU", 0);
            new MarketAdaptee("XICE", "NASDAQ OMX ICELAND", "IS", "REYKJAVIK", 0, "ICEX");
            new MarketAdaptee("XICX", "INSTINET CANADA CROSS", "CA", "TORONTO", 0);
            new MarketAdaptee("XIDX", "INDONESIA STOCK EXCHANGE", "ID", "JAKARTA", 0, "IDX");
            new MarketAdaptee("XIHK", "INSTINET PACIFIC LTD", "HK", "HONG KONG", 0);
            new MarketAdaptee("XIJP", "INSTINET JAPAN", "JP", "TOKYO", 0);
            new MarketAdaptee("XIMA", "INTERNATIONAL MARTIME EXCHANGE", "NO", "OSLO", 0, "IMAREX");
            new MarketAdaptee("XIMC", "MULTI COMMODITY EXCHANGE OF INDIA LTD.", "IN", "MUMBAI", 0, "MCX");
            new MarketAdaptee("XIMM", "INTERNATIONAL MONETARY MARKET", "US", "CHICAGO", 0);
            new MarketAdaptee("XINV", "INVESTRO", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XIOM", "INDEX AND OPTIONS MARKET", "US", "CHICAGO", 0, "IOM");
            new MarketAdaptee("XIQS", "IRAQ STOCK EXCHANGE", "IQ", "BAGHDAD", 0, "ISX");
            new MarketAdaptee("XISA", "INTERNATIONAL SECURITIES EXCHANGE, LLC -  ALTERNATIVE MARKETS", "US", "NEW YORK", 0, "ISE");
            new MarketAdaptee("XISE", "INTERNATIONAL SECURITIES EXCHANGE, LLC - EQUITIES", "US", "NEW YORK", 0, "ISE");
            new MarketAdaptee("XISL", "ISLAMABAD STOCK EXCHANGE", "PK", "ISLAMABAD", 0, "ISE");
            new MarketAdaptee("XIST", "ISTANBUL STOCK EXCHANGE", "TR", "ISTANBUL", 0, "ISE");
            new MarketAdaptee("XISX", "INTERNATIONAL SECURITIES EXCHANGE, LLC", "US", "NEW YORK", 0, "ISE");
            new MarketAdaptee("XJAM", "JAMAICA STOCK EXCHANGE", "JM", "KINGSTON", 0);
            new MarketAdaptee("XJAS", "OSAKA SECURITIES EXCHANGE JASDAQ", "JP", "OSAKA", 0, "JASDAQ");
            new MarketAdaptee("XJNB", "JAKARTA NEGOTIATED BOARD", "ID", "JAKARTA", 0);
            new MarketAdaptee("XJSE", "JOHANNESBURG STOCK EXCHANGE", "ZA", "JOHANNESBURG", 0, "JSE");
            new MarketAdaptee("XKAC", "KANSAI COMMODITIES EXCHANGE", "JP", "OSAKA", 0, "KANEX");
            new MarketAdaptee("XKAR", "KARACHI STOCK EXCHANGE (GUARANTEE) LIMITED", "PK", "KARACHI", 0, "KSE");
            new MarketAdaptee("XKAZ", "KAZAKHSTAN STOCK EXCHANGE", "KZ", "ALMA-ATA", 0, "KAZE");
            new MarketAdaptee("XKBT", "KANSAS CITY BOARD OF TRADE", "US", "KANSAS CITY", 0, "KCBT");
            new MarketAdaptee("XKCE", "KHOREZM INTERREGION COMMODITY EXCHANGE", "UZ", "TASHKENT", 0);
            new MarketAdaptee("XKFB", "KOREA FREEBOARD MARKET", "KR", "SEOUL", 0);
            new MarketAdaptee("XKFE", "KOREA EXCHANGE (FUTURES MARKET)", "KR", "SEOUL", 0, "KRX FM");
            new MarketAdaptee("XKHA", "KHARTOUM STOCK EXCHANGE", "SD", "KHARTOUM", 0, "KSE");
            new MarketAdaptee("XKHR", "KHARKOV COMMODITY EXCHANGE", "UA", "KHARKOV", 0);
            new MarketAdaptee("XKIE", "KIEV UNIVERSAL EXCHANGE", "UA", "KIEV", 0);
            new MarketAdaptee("XKIS", "KIEV INTERNATIONAL STOCK EXCHANGE", "UA", "KIEV", 0, "KISE");
            new MarketAdaptee("XKLS", "BURSA MALAYSIA", "MY", "KUALA LUMPUR", 0);
            new MarketAdaptee("XKOS", "KOREA EXCHANGE (KOSDAQ)", "KR", "SEOUL", 0, "KOSDAQ");
            new MarketAdaptee("XKRX", "KOREA EXCHANGE (STOCK MARKET)", "KR", "SEOUL", 0, "KRX SM");
            new MarketAdaptee("XKSE", "KYRGYZ STOCK EXCHANGE", "KG", "BISHKEK", 0, "KSE");
            new MarketAdaptee("XKUW", "KUWAIT STOCK EXCHANGE", "KW", "KUWAIT", 0);
            new MarketAdaptee("XLAH", "LAHORE STOCK EXCHANGE", "PK", "LAHORE", 0, "LSE");
            new MarketAdaptee("XLAO", "LAO SECURITIES EXCHANGE", "LA", "LAO", 0, "LSX");
            new MarketAdaptee("XLAT", "LATIBEX", "ES", "MADRID", 0);
            new MarketAdaptee("XLBM", "LONDON BULLION MARKET", "GB", "LONDON", 0);
            new MarketAdaptee("XLDN", "NYSE EURONEXT - EURONEXT LONDON", "GB", "LONDON", 0);
            new MarketAdaptee("XLFX", "LABUAN INTERNATIONAL FINANCIAL EXCHANGE", "MY", "LABUAN", 0, "LFX");
            new MarketAdaptee("XLIF", "NYSE EURONEXT LIFFE", "GB", "LONDON", 0);
            new MarketAdaptee("XLIM", "BOLSA DE VALORES DE LIMA", "PE", "LIMA", 0, "BVL");
            new MarketAdaptee("XLIS", "NYSE EURONEXT - EURONEXT LISBON", "PT", "LISBOA", 0);
            new MarketAdaptee("XLIT", "NASDAQ OMX VILNIUS", "LT", "VILNIUS", 0);
            new MarketAdaptee("XLJU", "LJUBLJANA STOCK EXCHANGE (OFFICIAL MARKET)", "SI", "LJUBLJANA", 0);
            new MarketAdaptee("XLME", "LONDON METAL EXCHANGE", "GB", "LONDON", 0, "LME");
            new MarketAdaptee("XLON", "LONDON STOCK EXCHANGE", "GB", "LONDON", 1, "LSE");
            new MarketAdaptee("XLSM", "LIBYAN STOCK MARKET", "LY", "TRIPOLI", 0);
            new MarketAdaptee("XLUS", "LUSAKA STOCK EXCHANGE", "ZM", "LUSAKA", 0);
            new MarketAdaptee("XLUX", "LUXEMBOURG STOCK EXCHANGE", "LU", "LUXEMBOURG", 0);
            new MarketAdaptee("XMAB", "MERCADO ABIERTO ELECTRONICO S.A.", "AR", "BUENOS AIRES", 0, "MAE");
            new MarketAdaptee("XMAD", "BOLSA DE MADRID", "ES", "MADRID", 0);
            new MarketAdaptee("XMAE", "MACEDONIAN STOCK EXCHANGE", "MK", "SKOPJE", 0);
            new MarketAdaptee("XMAI", "MARKET FOR ALTERNATIVE INVESTMENT", "TH", "BANGKOK", 0);
            new MarketAdaptee("XMAL", "MALTA STOCK EXCHANGE", "MT", "VALLETTA", 0);
            new MarketAdaptee("XMAN", "BOLSA DE VALORES DE NICARAGUA", "NI", "MANAGUA", 0);
            new MarketAdaptee("XMAP", "MAPUTO STOCK  EXCHANGE", "MZ", "MAPUTO", 0);
            new MarketAdaptee("XMAT", "EURONEXT PARIS MATIF", "FR", "PARIS", 0);
            new MarketAdaptee("XMAU", "STOCK EXCHANGE OF MAURITIUS LTD", "MU", "PORT LOUIS", 0);
            new MarketAdaptee("XMCE", "MERCADO CONTINUO ESPANOL", "ES", "MADRID", 0, "SIBE");
            new MarketAdaptee("XMDG", "MARCHE INTERBANCAIRE DES DEVISES M.I.D.", "MG", "ANTANANARIVO", 0);
            new MarketAdaptee("XMDS", "MADRAS STOCK EXCHANGE", "IN", "MADRAS", 0);
            new MarketAdaptee("XMEF", "MEFF RENTA FIJA", "ES", "BARCELONA", 0, "MEFF");
            new MarketAdaptee("XMER", "MERCHANTS' EXCHANGE", "US", "CHICAGO", 0, "ME");
            new MarketAdaptee("XMEV", "MERCADO DE VALORES DE BUENOS AIRES S.A.", "AR", "BUENOS AIRES", 0, "MERVAL");
            new MarketAdaptee("XMEX", "BOLSA MEXICANA DE VALORES (MEXICAN STOCK EXCHANGE)", "MX", "MEXICO", 0);
            new MarketAdaptee("XMGE", "MINNEAPOLIS GRAIN EXCHANGE", "US", "MINNEAPOLIS", 0, "MGE");
            new MarketAdaptee("XMIC", "MOSCOW INTERBANK CURRENCY EXCHANGE", "RU", "MOSCOW", 0, "MICEX");
            new MarketAdaptee("XMLI", "NYSE EURONEXT - MARCHE LIBRE PARIS", "FR", "PARIS", 0);
            new MarketAdaptee("XMNT", "BOLSA DE VALORES DE MONTEVIDEO", "UY", "MONTEVIDEO", 0, "BVMT");
            new MarketAdaptee("XMNX", "MONTENEGRO STOCK EXCHANGE", "ME", "MONTENEGRO", 0);
            new MarketAdaptee("XMOC", "MONTREAL CLIMATE EXCHANGE", "CA", "MONTREAL", 0);
            new MarketAdaptee("XMOD", "THE MONTREAL EXCHANGE / BOURSE DE MONTREAL", "CA", "MONTREAL", 0, "CDE");
            new MarketAdaptee("XMOL", "MOLDOVA STOCK EXCHANGE", "MD", "CHISINAU", 0);
            new MarketAdaptee("XMON", "EURONEXT PARIS MONEP", "FR", "PARIS", 0);
            new MarketAdaptee("XMOS", "MOSCOW STOCK EXCHANGE", "RU", "MOSCOW", 0, "MSE");
            new MarketAdaptee("XMOT", "EXTRAMOT", "IT", "MILANO", 0);
            new MarketAdaptee("XMRV", "MEFF RENTA VARIABLE", "ES", "BARCELONA", 0, "MEFF");
            new MarketAdaptee("XMSW", "MALAWI STOCK EXCHANGE", "MW", "BLANTYRE", 0);
            new MarketAdaptee("XMTB", "MERCADO A TERMINO DE BUENOS AIRES S.A.", "AR", "BUENOS AIRES", 0, "MATBA");
            new MarketAdaptee("XMUN", "BOERSE MUENCHEN", "DE", "MUENCHEN", 0);
            new MarketAdaptee("XMUS", "MUSCAT SECURITIES MARKET", "OM", "MUSCAT", 0, "MSM");
            new MarketAdaptee("XMVL", "MERCADO DE VALORES DEL LITORAL S.A.", "AR", "SANTA FE", 0);
            new MarketAdaptee("XNAF", "SISTEMA ESPANOL DE NEGOCIACION DE ACTIVOS FINANCIEROS", "ES", "MADRID", 0, "SENAF");
            new MarketAdaptee("XNAI", "NAIROBI STOCK EXCHANGE", "KE", "NAIROBI", 0);
            new MarketAdaptee("XNAM", "NAMIBIAN STOCK EXCHANGE", "NA", "WINDHOEK", 0);
            new MarketAdaptee("XNAS", "NASDAQ", "US", "NEW YORK", 0, "NASDAQ");
            new MarketAdaptee("XNCD", "NATIONAL COMMODITY & DERIVATIVES EXCHANGE LTD", "IN", "MUMBAI", 0, "NCDEX");
            new MarketAdaptee("XNCM", "NASDAQ CAPITAL MARKET", "US", "NEW YORK", 0);
            new MarketAdaptee("XNCO", "NEW CONNECT", "PL", "WARSZAWA", 0);
            new MarketAdaptee("XNDQ", "NASDAQ OPTIONS MARKET", "US", "NEW YORK", 0);
            new MarketAdaptee("XNDX", "NORDIC DERIVATIVES EXCHANGE", "SE", "STOCKHOLM", 0, "NDX");
            new MarketAdaptee("XNEC", "NATIONAL STOCK EXCHANGE OF AUSTRALIA LIMITED", "AU", "NEWCASTLE", 0, "NSXA");
            new MarketAdaptee("XNEP", "NEPAL STOCK EXCHANGE", "NP", "KATHMANDU", 0);
            new MarketAdaptee("XNEW", "NEWEX", "DE", "FRANKFURT", 0, "NEWEX");
            new MarketAdaptee("XNGM", "NORDIC GROWTH MARKET", "SE", "STOCKHOLM", 0, "NGM");
            new MarketAdaptee("XNGO", "NAGOYA STOCK EXCHANGE", "JP", "NAGOYA", 0, "NSE");
            new MarketAdaptee("XNGS", "NASDAQ/NGS (GLOBAL SELECT MARKET)", "US", "NEW YORK", 0, "NGS");
            new MarketAdaptee("XNIM", "NASDAQ INTERMARKET", "US", "NEW YORK", 0);
            new MarketAdaptee("XNKS", "CENTRAL JAPAN COMMODITIES EXCHANGE", "JP", "NAGOYA", 0, "C-COM");
            new MarketAdaptee("XNLI", "NYSE LIFFE", "US", "NEW YORK", 0);
            new MarketAdaptee("XNMR", "NORDIC MTF REPORTING", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("XNMS", "NASDAQ/NMS (GLOBAL MARKET)", "US", "NEW YORK", 0);
            new MarketAdaptee("XNSA", "NIGERIAN STOCK EXCHANGE", "NG", "LAGOS", 0);
            new MarketAdaptee("XNSE", "NATIONAL STOCK EXCHANGE OF INDIA", "IN", "MUMBAI", 0, "NSE");
            new MarketAdaptee("XNYE", "NEW YORK MERCANTILE EXCHANGE - OTC MARKETS", "US", "NEW YORK", 0, "NYMEX ECM");
            new MarketAdaptee("XNYL", "NEW YORK MERCANTILE EXCHANGE - ENERGY MARKETS", "US", "NEW YORK", 0, "NYMEX MTF LIMITED");
            new MarketAdaptee("XNYM", "NEW YORK MERCANTILE EXCHANGE", "US", "NEW YORK", 0, "NYMEX");
            new MarketAdaptee("XNYS", "NEW YORK STOCK EXCHANGE, INC.", "US", "NEW YORK", 1, "NYSE");
            new MarketAdaptee("XNZE", "NEW ZEALAND EXCHANGE LTD", "NZ", "WELLINGTON", 0, "NZX");
            new MarketAdaptee("XOAM", "OSLO BORS ALTERNATIVE BOND MARKET", "NO", "OSLO", 0, "ABM");
            new MarketAdaptee("XOAS", "OSLO AXESS", "NO", "OSLO", 0);
            new MarketAdaptee("XOCH", "ONECHICAGO, LLC", "US", "CHICAGO", 0);
            new MarketAdaptee("XODE", "ODESSA COMMODITY EXCHANGE", "UA", "ODESSA", 0);
            new MarketAdaptee("XOPV", "OMX OTC PUBLICATION VENUE", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("XOSE", "OSAKA SECURITIES EXCHANGE", "JP", "OSAKA", 0, "OSE");
            new MarketAdaptee("XOSJ", "OSAKA SECURITIES EXCHANGE J-NET", "JP", "OSAKA", 0, "J-NET");
            new MarketAdaptee("XOSL", "OSLO BORS ASA", "NO", "OSLO", 0);
            new MarketAdaptee("XOTC", "OTC BULLETIN BOARD", "US", "WASHINGTON", 0, "OTCBB");
            new MarketAdaptee("XPAE", "PALESTINE EXCHANGE", "PS", "NABLUS", 0, "PEX");
            new MarketAdaptee("XPAR", "NYSE EURONEXT - EURONEXT PARIS", "FR", "PARIS", 1);
            new MarketAdaptee("XPBT", "NASDAQ OMX FUTURES EXCHANGE", "US", "PHILADELPHIA", 0, "NFX");
            new MarketAdaptee("XPET", "SAINT PETERSBURG EXCHANGE", "RU", "SAINT-PETERSBURG", 0, "SPEX");
            new MarketAdaptee("XPHL", "NASDAQ OMX PHLX", "US", "PHILADELPHIA", 0, "PHLX");
            new MarketAdaptee("XPHO", "PHILADELPHIA OPTIONS EXCHANGE", "US", "PHILADELPHIA", 0);
            new MarketAdaptee("XPHS", "PHILIPPINE STOCK EXCHANGE, INC.", "PH", "PASIG CITY", 0, "PSE");
            new MarketAdaptee("XPIC", "SAINT-PETERSBURG CURRENCY EXCHANGE", "RU", "SAINT-PETERSBURG", 0, "SPCEX");
            new MarketAdaptee("XPIN", "UBS ATS", "US", "STAMFORD", 0);
            new MarketAdaptee("XPLU", "PLUS MARKETS", "GB", "LONDON", 0);
            new MarketAdaptee("XPOM", "PORT MORESBY STOCK EXCHANGE", "PG", "PORT MORESBY", 0);
            new MarketAdaptee("XPOR", "PORTAL", "US", "WASHINGTON", 0);
            new MarketAdaptee("XPOS", "POSIT", "IE", "DUBLIN", 0, "POSIT");
            new MarketAdaptee("XPOW", "POWERNEXT", "FR", "PARIS", 0);
            new MarketAdaptee("XPRA", "PRAGUE STOCK EXCHANGE", "CZ", "PRAGUE", 0, "PSE");
            new MarketAdaptee("XPRI", "PRIDNEPROVSK COMMODITY EXCHANGE", "UA", "DNIPROPETROVSK", 0);
            new MarketAdaptee("XPST", "POSIT - ASIA PACIFIC", "HK", "HONG KONG", 0);
            new MarketAdaptee("XPTY", "BOLSA DE VALORES DE PANAMA, S.A.", "PA", "PANAMA", 0, "BVP");
            new MarketAdaptee("XPXE", "POWER EXCHANGE CENTRAL EUROPE", "CZ", "PRAGUE", 0, "PXE");
            new MarketAdaptee("XQMH", "SCOACH SWITZERLAND", "CH", "ZURICH", 0);
            new MarketAdaptee("XQUI", "BOLSA DE VALORES DE QUITO", "EC", "QUITO", 0);
            new MarketAdaptee("XRAS", "RASDAQ", "RO", "BUCHAREST", 0, "RASDAQ");
            new MarketAdaptee("XRBM", "RINGGIT BOND MARKET", "MY", "KUALA LUMPUR", 0, "RBM");
            new MarketAdaptee("XRIS", "NASDAQ OMX RIGA", "LV", "RIGA", 0);
            new MarketAdaptee("XRMO", "RM-SYSTEM CZECH STOCK EXCHANGE (MTF)", "CZ", "PRAGUE", 0, "RMS CZ");
            new MarketAdaptee("XRMZ", "RM-SYSTEM CZECH STOCK EXCHANGE", "CZ", "PRAGUE", 0, "RMS CZ");
            new MarketAdaptee("XROS", "BOLSA DE COMERCIO ROSARIO", "AR", "ROSARIO", 0, "BCR");
            new MarketAdaptee("XROX", "MERCADO DE VALORES DE ROSARIO S.A.", "AR", "ROSARIO", 0, "MERVAROS");
            new MarketAdaptee("XRPM", "ROMANIAN POWER MARKET", "RO", "BUCHAREST", 0);
            new MarketAdaptee("XRUS", "INTERNET DIRECT-ACCESS EXCHANGE", "RU", "MOSCOW", 0, "INDX");
            new MarketAdaptee("XSAF", "SOUTH AFRICAN FUTURES EXCHANGE", "ZA", "JOHANNESBURG", 0, "SAFEX");
            new MarketAdaptee("XSAM", "SAMARA CURRENCY INTERBANK EXCHANGE", "RU", "SAMARA", 0, "SCIEX");
            new MarketAdaptee("XSAP", "SAPPORO SECURITIES EXCHANGE", "JP", "SAPPORO", 0);
            new MarketAdaptee("XSAT", "AKTIETORGET", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("XSAU", "SAUDI STOCK EXCHANGE", "SA", "RIYADH", 0);
            new MarketAdaptee("XSC1", "SCOACH EUROPA INTERNATIONAL MARKET 1", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XSC2", "SCOACH EUROPA INTERNATIONAL MARKET 2", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XSC3", "SCOACH EUROPA INTERNATIONAL MARKET 3", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XSCA", "SINGAPORE CATALIST MARKET", "SG", "SINGAPORE", 0);
            new MarketAdaptee("XSCE", "SINGAPORE COMMODITY EXCHANGE", "SG", "SINGAPORE", 0, "SICOM");
            new MarketAdaptee("XSCL", "SINGAPORE CENTRAL LIMIT ORDER BOOK INTERNATIONAL", "SG", "SINGAPORE", 0);
            new MarketAdaptee("XSCU", "STOXX LIMITED - CUSTOMIZED INDICES", "CH", "ZURICH", 0);
            new MarketAdaptee("XSES", "SINGAPORE EXCHANGE", "SG", "SINGAPORE", 0, "SGX");
            new MarketAdaptee("XSFA", "SOUTH AFRICAN FUTURES EXCHANGE - AGRICULTURAL MARKET DIVISION", "ZA", "JOHANNESBURG", 0, "SAFEX");
            new MarketAdaptee("XSFE", "ASX 24", "AU", "SYDNEY", 0, "SFE");
            new MarketAdaptee("XSGE", "SHANGHAI FUTURES EXCHANGE", "CN", "SHANGHAI", 0, "SHFE");
            new MarketAdaptee("XSGO", "SANTIAGO STOCK EXCHANGE", "CL", "SANTIAGO", 0);
            new MarketAdaptee("XSHE", "SHENZHEN STOCK EXCHANGE", "CN", "SHENZHEN", 0);
            new MarketAdaptee("XSHG", "SHANGHAI STOCK EXCHANGE", "CN", "SHANGHAI", 0);
            new MarketAdaptee("XSIB", "SIBERIAN EXCHANGE", "RU", "NOVOSIBIRSK", 0, "SIMEX");
            new MarketAdaptee("XSIM", "SINGAPORE EXCHANGE DERIVATIVES CLEARING LIMITED", "SG", "SINGAPORE", 0, "SGX-DT");
            new MarketAdaptee("XSMP", "SMARTPOOL", "GB", "LONDON", 0);
            new MarketAdaptee("XSOP", "BSP REGIONAL ENERGY EXCHANGE - SOUTH POOL", "SI", "LJUBLJANA", 0);
            new MarketAdaptee("XSPS", "SOUTH PACIFIC STOCK EXCHANGE", "FJ", "SUVA", 0, "SPSE");
            new MarketAdaptee("XSRM", "MERCADO DE FUTUROS DE ACEITE DE OLIVA, S.A.", "ES", "JAEN", 0, "MFAO");
            new MarketAdaptee("XSSE", "SARAJEVO STOCK EXCHANGE", "BA", "SARAJEVO", 0, "SASE");
            new MarketAdaptee("XSTC", "HOCHIMINH STOCK EXCHANGE", "VN", "HO CHI MINH CITY", 0, " HOSE");
            new MarketAdaptee("XSTE", "REPUBLICAN STOCK EXCHANGE", "UZ", "TASHKENT", 0, "UZSE");
            new MarketAdaptee("XSTO", "NASDAQ OMX NORDIC", "SE", "STOCKHOLM", 0);
            new MarketAdaptee("XSTU", "BOERSE STUTTGART", "DE", "STUTTGART", 0);
            new MarketAdaptee("XSTV", "STOXX LIMITED - VOLATILITY INDICES", "CH", "ZURICH", 0);
            new MarketAdaptee("XSTX", "STOXX LIMITED - INDICES", "CH", "ZURICH", 0);
            new MarketAdaptee("XSVA", "EL SALVADOR STOCK EXCHANGE", "SV", "EL SALVADOR", 0);
            new MarketAdaptee("XSWA", "SWAZILAND STOCK EXCHANGE", "SZ", "MBABANE", 0, "SSX");
            new MarketAdaptee("XSWB", "SWX SWISS BLOCK", "GB", "LONDON", 0);
            new MarketAdaptee("XSWX", "SWISS EXCHANGE", "CH", "ZURICH", 0, "SWX");
            new MarketAdaptee("XTAE", "TEL AVIV STOCK EXCHANGE", "IL", "TEL AVIV", 0, "TASE");
            new MarketAdaptee("XTAF", "TAIWAN FUTURES EXCHANGE", "TW", "TAIPEI", 0, "TAIFEX");
            new MarketAdaptee("XTAI", "TAIWAN STOCK EXCHANGE", "TW", "TAIPEI", 0, "TWSE");
            new MarketAdaptee("XTAL", "NASDAQ OMX TALLINN", "EE", "TALLINN", 0);
            new MarketAdaptee("XTAM", "TOKYO AIM", "JP", "TOKYO", 0);
            new MarketAdaptee("XTEH", "TEHRAN STOCK EXCHANGE", "IR", "TEHRAN", 0, "TSE");
            new MarketAdaptee("XTFF", "TOKYO FINANCIAL  EXCHANGE", "JP", "TOKYO", 0, "TFX");
            new MarketAdaptee("XTIR", "TIRANA STOCK EXCHANGE", "AL", "TIRANA", 0);
            new MarketAdaptee("XTK1", "TOKYO STOCK EXCHANGE - TOSTNET-1", "JP", "TOKYO", 0);
            new MarketAdaptee("XTK2", "TOKYO STOCK EXCHANGE - TOSTNET-2", "JP", "TOKYO", 0);
            new MarketAdaptee("XTK3", "TOKYO STOCK EXCHANGE - TOSTNET-3", "JP", "TOKYO", 0);
            new MarketAdaptee("XTKO", "TOKYO GRAIN EXCHANGE", "JP", "TOKYO", 0, "TGE");
            new MarketAdaptee("XTKS", "TOKYO STOCK EXCHANGE", "JP", "TOKYO", 0, "TSE");
            new MarketAdaptee("XTKT", "TOKYO COMMODITY EXCHANGE", "JP", "TOKYO", 0, "TOCOM");
            new MarketAdaptee("XTNX", "TSX VENTURE EXCHANGE - NEX", "CA", "VANCOUVER", 0, "NEX");
            new MarketAdaptee("XTPE", "TULLETT PREBON ENERGYTRADE", "GB", "LONDON", 0);
            new MarketAdaptee("XTRN", "TRINIDAD AND TOBAGO STOCK EXCHANGE", "TT", "PORT OF SPAIN", 0, "TTSE");
            new MarketAdaptee("XTRZ", "ZAGREB MONEY AND SHORT TERM SECURITIES MARKET INC", "HR", "ZAGREB", 0);
            new MarketAdaptee("XTSE", "TORONTO STOCK EXCHANGE", "CA", "TORONTO", 0, "TSX");
            new MarketAdaptee("XTSX", "TSX VENTURE EXCHANGE", "CA", "TORONTO", 0, "TSX-V");
            new MarketAdaptee("XTUC", "NUEVA BOLSA DE COMERCIO DE TUCUMAN S.A.", "AR", "TUCUMAN", 0, "NBCT");
            new MarketAdaptee("XTUN", "BOURSE DE TUNIS", "TN", "TUNIS", 0, "BVMT");
            new MarketAdaptee("XTUR", "TURKISH DERIVATIVES EXCHANGE", "TR", "IZMIR", 0, "TURKDEX");
            new MarketAdaptee("XUAX", "UKRAINIAN STOCK EXCHANGE", "UA", "KIEV", 0, "UKRSE");
            new MarketAdaptee("XUBS", "UBS MTF", "GB", "LONDON", 0);
            new MarketAdaptee("XUGA", "UGANDA SECURITIES EXCHANGE", "UG", "KAMPALA", 0, "USE");
            new MarketAdaptee("XUKR", "UKRAINIAN UNIVERSAL COMMODITY EXCHANGE", "UA", "KIEV", 0);
            new MarketAdaptee("XULA", "MONGOLIAN STOCK EXCHANGE", "MN", "ULAAN BAATAR", 0);
            new MarketAdaptee("XUNI", "UNIVERSAL BROKER'S EXCHANGE 'TASHKENT'", "UZ", "TASHKENT", 0);
            new MarketAdaptee("XUSE", "UNITED STOCK EXCHANGE", "IN", "MUMBAI", 0);
            new MarketAdaptee("XVAL", "BOLSA DE VALENCIA", "ES", "VALENCIA", 0);
            new MarketAdaptee("XVES", "VESTIMA+", "LU", "LUXEMBOURG", 0);
            new MarketAdaptee("XVIE", "WIENER BOERSE AG, WERTPAPIERBOERSE (SECURITIES EXCHANGE)", "AT", "VIENNA", 0);
            new MarketAdaptee("XVPA", "BOLSA DE VALORES Y PRODUCTOS DE ASUNCION SA", "PY", "ASUNCION", 0, "BVPASA");
            new MarketAdaptee("XVTX", "SIX SWISS EXCHANGE AG", "CH", "ZURICH", 0);
            new MarketAdaptee("XWAR", "WARSAW STOCK EXCHANGE", "PL", "WARSZAWA", 0, "WSE");
            new MarketAdaptee("XWBO", "WIENER BOERSE AG", "AT", "VIENNA", 0);
            new MarketAdaptee("XWEE", "WEEDEN ATS", "US", "GREENWICH", 0);
            new MarketAdaptee("XXSC", "FRANKFURT CEF SC", "DE", "FRANKFURT", 0);
            new MarketAdaptee("XXXX", "NO MARKET (EG, UNLISTED)", null,null, 0);
            new MarketAdaptee("XYIE", "YIELDBROKER PTY LTD", "AU", "SYDNEY", 0);
            new MarketAdaptee("XZAG", "ZAGREB STOCK EXCHANGE", "HR", "ZAGREB", 0);
            new MarketAdaptee("XZAM", "THE ZAGREB STOCK EXCHANGE MTF", "HR", "ZAGREB", 0);
            new MarketAdaptee("XZCE", "ZHENGZHOU COMMODITY EXCHANGE", "CN", "ZHENGZHOU", 0, "ZCE");
            new MarketAdaptee("XZIM", "ZIMBABWE STOCK EXCHANGE", "ZW", "HARARE", 0);
            new MarketAdaptee("YLDX", "JSE YIELD-X", "ZA", "JOHANNESBURG", 0);
            new MarketAdaptee("ZKBX", "ZURCHER KANTONALBANK SECURITIES EXCHANGE", "CH", "ZURICH", 0);
            new MarketAdaptee("ZOBX", "ZOBEX", "DE", "BERLIN", 0, "ZOBEX");
        }
        #endregion
    }

    [NotMapped]
    internal class MarketAdaptee
    {

        internal readonly string Label;
        /// <summary>
        /// MIC Identifier
        /// </summary>
        internal readonly string Value;
        internal readonly string CountryCode;
        internal readonly string City;
        internal readonly string ShortName;
        internal MarketTypeCode Type;
        internal readonly int TopUsed;

        internal MarketAdaptee(string Value, string Label, string CountryCode, string City, int TopUsed)
        {
            this.Value = Value;
            this.Label = Label;
            this.CountryCode = CountryCode;
            this.City = City;
            this.ShortName = null;
            this.Type = MarketTypeCode.PrimaryMarket;
            this.TopUsed = TopUsed;
            Instances[Value] = this;
            Constants[Label] = this;
        }
        internal MarketAdaptee(string Value, string Label, string CountryCode, string City, int TopUsed,string ShortName)
        {
            this.Value = Value;
            this.Label = Label;
            this.CountryCode = CountryCode;
            this.City = City;
            this.ShortName = ShortName;
            this.Type = MarketTypeCode.PrimaryMarket;
            this.TopUsed = TopUsed;
            Instances[Value] = this;
            Constants[Label] = this;
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

            // If parameter cannot be cast to InterestComputationMethodCodeAdaptee return false.
            MarketAdaptee p = obj as MarketAdaptee;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Value == p.Value) && (Label == p.Label);
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        internal static readonly Dictionary<string, MarketAdaptee> Instances = new Dictionary<string, MarketAdaptee>();
        /// <summary>
        /// Recherche par libellé
        /// </summary>
        internal static readonly Dictionary<string, MarketAdaptee> Constants = new Dictionary<string, MarketAdaptee>();
    }

}
