using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using FGABusinessComponent.BusinessComponent.Common;
using FGABusinessComponent.BusinessComponent.Security;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Holding.PortfolioComp
{
    /// <summary>
    /// Le typage des fonds : Action, Taux (Gov, Credit ...) ou Diversifie
    /// </summary>
    class PortfolioTypeCode
    {
        [MaxLength(10)]
        internal readonly string Label;
        [MaxLength(1)]
        internal readonly string Value;

        internal PortfolioTypeCode()
        {
        }

        private PortfolioTypeCode(string Label, string Value)
        {
            this.Label = Label;
            this.Value = Value;
            Instances[Value] = this;
        }

        public override String ToString()
        {
            return Label;
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator PortfolioTypeCode(string str)
        {
            if (str == null) str = "X";

            PortfolioTypeCode result;
            if (PortfolioTypeCode.Instances.TryGetValue(str, out result))
                return result;
            else
                return Instances["X"];
        }


        static PortfolioTypeCode()
        {
            new PortfolioTypeCode("Obligataire","O");
            new PortfolioTypeCode("Action","A");
            new PortfolioTypeCode("Oblig Credit","C");
            new PortfolioTypeCode("Oblig Etat","G");
            new PortfolioTypeCode("Diversifie","D");
            new PortfolioTypeCode("FCPE", "E");
            new PortfolioTypeCode("Non Determiné","X");
        }

        internal static readonly Dictionary<string, PortfolioTypeCode> Instances = new Dictionary<string, PortfolioTypeCode>();
    }

    class PortfolioCode
    {
        public const string DEFAULT = "XXXXXX";
        private PortfolioCodeAdaptee InternalObject;

        public PortfolioCode()
        {
        }

        internal PortfolioCode(PortfolioCodeAdaptee Instance)
        {
            if (Instance.Value != null)
            {
                this.InternalObject = Instance;
            }
        }

        [Column("Portfolio"), MaxLength(15)]
        public string Portfolio
        {
            get { return ( InternalObject !=null ? InternalObject.Value : null ); }
            set
            {
                if (value == null) value = "XXXXXX";

                PortfolioCodeAdaptee result;
                if (PortfolioCodeAdaptee.Instances.TryGetValue(value, out result))
                    this.InternalObject = result;
                else
                    throw new InvalidCastException();   // code qui n existe pas => porfolio code à créer                  
            }
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator PortfolioCode(string str)
        {
            if (str == null) str = "XXXXXX";

            PortfolioCodeAdaptee result;
            if (PortfolioCodeAdaptee.Instances.TryGetValue(str, out result))
                return new PortfolioCode(result);
            else
                throw new InvalidCastException();
        }
        public override String ToString()
        {
            return InternalObject.ToString();
        }
        
        #region INITIALISATION DES CONSTANTES
        /// <summary>
        /// Initialisation des constantes dans le dictionnaire
        /// </summary>
        static PortfolioCode()
        {
            new PortfolioCodeAdaptee("0001031", "AUXIA ASSISTANCE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("0001032", "AUXIA ASSISTANCE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("0002100", "AUXIA ACTIONS BAGUE", "BP2S", "A");
            new PortfolioCodeAdaptee("0002101", "AUXIA ACTIONS NON BAGUE", "BP2S", "A");
            new PortfolioCodeAdaptee("0002200", "AUXIA OBLIGATIONS BAGUE", "BP2S", "O");
            new PortfolioCodeAdaptee("0002201", "AUXIA OBLIGATIONS NON BAGUE", "BP2S", "O");
            new PortfolioCodeAdaptee("04225", "MMP ARCELOR OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("04226", "MMP ARCELOR ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("04227", "MMP ARCELOR DIVERSIFIE", "BP2S", "D");
            new PortfolioCodeAdaptee("08115", "CMAV OBLIGATIONS", "CIC", "O");
            new PortfolioCodeAdaptee("08116", "CMAV ACTIONS", "CIC", "A");
            new PortfolioCodeAdaptee("08117", "CMAV DIVERSIFIE", "CIC", "D");
            new PortfolioCodeAdaptee("08118", "CMAV Actif general (hors FGA)", "CIC", "X");
            new PortfolioCodeAdaptee("08215", "CMAV CANTONNE 1 OBLIGATIONS", "CIC", "O");
            new PortfolioCodeAdaptee("08315", "CMAV PERP OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("08316", "CMAV PERP ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("08335", "CMAV RS ARCELOR OBLIGATIONS", "CIC", "O");
            new PortfolioCodeAdaptee("08336", "CMAV RS ARCELOR ACTIONS", "CIC", "A");
            new PortfolioCodeAdaptee("08345", "CMAV RS MICHELIN OBLIGATIONS", "CIC", "O");
            new PortfolioCodeAdaptee("08346", "CMAV RS MICHELIN ACTIONS", "CIC", "A");
            new PortfolioCodeAdaptee("08355", "CMAV SOCGEN OBLIGATIONS", "CIC", "O");
            new PortfolioCodeAdaptee("08356", "CMAV SOCGEN ACTIONS", "CIC", "A");
            new PortfolioCodeAdaptee("08357", "CMAV SOCGEN TRESORERIE", "CIC", "T");
            new PortfolioCodeAdaptee("1300110", "INPR OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("1300130", "INPR ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("22005", "SAPREM OBLIGATIONS", "CIC", "O");
            new PortfolioCodeAdaptee("22006", "SAPREM ACTIONS", "CIC", "A");
            new PortfolioCodeAdaptee("22007", "SAPREM DIVERSIFIE", "CIC", "D");
            new PortfolioCodeAdaptee("3010010", "IRCEM RT OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3010011", "IRCEM RT ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3010012", "IRCEM RT DIVERSIFIE & STRUCTURES", "BP2S", "D");
            new PortfolioCodeAdaptee("3010020", "IRCEM RS OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3010021", "IRCEM RS ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3010022", "IRCEM RS DIVERSIFIE & STRUCTURES", "BP2S", "D");
            new PortfolioCodeAdaptee("3010030", "IRCEM RG OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3010031", "IRCEM RG ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3010032", "IRCEM RG DIVERSIFIE & STRUCTURES", "BP2S", "D");
            new PortfolioCodeAdaptee("3020010", "IRCEM PREVOYANCE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3020011", "IRCEM PREVOYANCE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3020012", "IRCEM PREVOYANCE DIVERSIFIE & STRUCTURES", "BP2S", "D");
            new PortfolioCodeAdaptee("3020020", "IRCEM MUTUELLE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3020021", "IRCEM MUTUELLE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3020022", "IRCEM MUTUELLE DIVERSIFIE & STRUCTURES", "BP2S", "D");
            new PortfolioCodeAdaptee("3203105", "MM ARRCO RT OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3203106", "MM ARRCO RT ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3203107", "MM ARRCO RT DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("3303105", "MM ARRCO FG OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3303106", "MM ARRCO FG ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3303107", "MM ARRCO FG DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("3403105", "MM ARRCO FS OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3403106", "MM ARRCO FS ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3403107", "MM ARRCO FS DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("3503105", "MM ARRCO CMD OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("3503106", "MM ARRCO CMD ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("3503107", "MM ARRCO CMD DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("4010015", "MP PERE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4010016", "EXPAR OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4010017", "CAA", "BP2S", "X");
            new PortfolioCodeAdaptee("4010022", "CAA ARKEMA OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4010025", "MP PERE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4010026", "EXPAR ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4010040", "MEDERIC PREVOYANCE EDITION OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4010042", "MEDERIC PREVOYANCE EDITION ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4010079", "MMP FP PARTICIPATIONS (Compte Fundquest)", "BP2S", "X");
            new PortfolioCodeAdaptee("4010081", "MMP RETRAITE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4010082", "MMP RETRAITE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4010083", "MMP RETRAITE DIVERSIFIE", "BP2S", "D");
            new PortfolioCodeAdaptee("4030001", "MMP FP OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4030002", "MMP FP ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4030003", "MMP FP DIVERSIFIE", "BP2S", "D");
            new PortfolioCodeAdaptee("4030005", "MMP PREV OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4030006", "MMP PREV ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4030007", "MMP PREV DIVERSIFIE", "BP2S", "D");
            new PortfolioCodeAdaptee("4030009", "MMP SANTE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("4030010", "MMP SANTE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("4030011", "MMP SANTE DIVERSIFIE", "BP2S", "D");
            new PortfolioCodeAdaptee("44000", "CAPREVAL Obligations", "CIC", "O");
            new PortfolioCodeAdaptee("44001", "CAPREVAL ACTIONS (hors FGA)", "CIC", "A");
            new PortfolioCodeAdaptee("5300240", "IDENTITES MUTUELLE OBLIGATIONS", "CDC", "O");
            new PortfolioCodeAdaptee("5300241", "IDENTITES MUTUELLE ACTIONS", "CDC", "A");
            new PortfolioCodeAdaptee("5300250", "U.N.M.I OBLIGATIONS", "CDC", "O");
            new PortfolioCodeAdaptee("5300251", "U.N.M.I ACTIONS", "CDC", "A");
            new PortfolioCodeAdaptee("5300252", "U.N.M.I OBLIGATIONS COURT TERME", "CDC", "O");
            new PortfolioCodeAdaptee("5300260", "MUT2M SANTE DIVERSIFIE", "CDC", "D");
            new PortfolioCodeAdaptee("5300261", "MUT2M SANTE OBLIGATIONS", "CDC", "O");
            new PortfolioCodeAdaptee("5300262", "MUT2M SANTE ACTIONS", "CDC", "A");
            new PortfolioCodeAdaptee("5300264", "MUT2M FONDS PROPRES DIVERSIFIE", "CDC", "D");
            new PortfolioCodeAdaptee("5300265", "MUT2M FONDS PROPRES ACTIONS", "CDC", "A");
            new PortfolioCodeAdaptee("5300266", "MUT2M FONDS PROPRES OBLIGATIONS", "CDC", "O");
            new PortfolioCodeAdaptee("53316", "ARCELORMITTAL France", "CIC", "D");
            new PortfolioCodeAdaptee("6100001", "FCP FEDERIS ACTIONS", "BP2S", "FR0007012182", "A");
            new PortfolioCodeAdaptee("6100002", "FCP FEDERIS CAC 40", "BP2S", "FR0007021936", "A");
            new PortfolioCodeAdaptee("6100004", "FCP FEDERIS ISR EURO", "BP2S", "FR0007045950", "X");
            new PortfolioCodeAdaptee("6100012", "FCPE NAPHTACHIMIE ACTIONS", "BP2S", "000000006079", "E");
            new PortfolioCodeAdaptee("6100015", "FCPE DYNAMIQUE AIR LIQUIDE CROISSANCE", "BP2S", "000000000411", "E");
            new PortfolioCodeAdaptee("6100016", "FCPE NAPHTACHIMIE EPARGNE", "BP2S", "000000001130", "E");
            new PortfolioCodeAdaptee("6100017", "FCPE CORA", "BP2S", "000000000410", "E");
            new PortfolioCodeAdaptee("6100018", "FCP FEDERIS OBLIGATION EURO", "BP2S", "FR0010250571", "O");
            new PortfolioCodeAdaptee("6100019", "FCP FEDERIS MONETAIRE", "BP2S", "X");
            new PortfolioCodeAdaptee("6100020", "FCP FEDERIS SERENITE", "BP2S", "FR0000989287", "X");
            new PortfolioCodeAdaptee("6100021", "FCP FEDERIS EQUILIBRE", "BP2S", "FR0000989295", "X");
            new PortfolioCodeAdaptee("6100022", "FCP FEDERIS DYNAMIQUE", "BP2S", "FR0000989279", "X");
            new PortfolioCodeAdaptee("6100023", "FCP UNIFED EPARGNE TEMPS", "CC", "FR0000989261", "X");
            new PortfolioCodeAdaptee("6100024", "FCP FEDERIS NORTH AMERICA", "BP2S", "FR0007057674", "X");
            new PortfolioCodeAdaptee("6100025", "FCP FEDERIS OBLIG EURO 1-3 ANS", "BP2S", "FR0007454889", "O");
            new PortfolioCodeAdaptee("6100026", "FCP FEDERIS EUROPE ACTIONS", "BP2S", "FR0007022801", "A");
            new PortfolioCodeAdaptee("6100027", "FCP MEDERIC MODERE", "BP2S", "FR0000984668", "X");
            new PortfolioCodeAdaptee("6100028", "FCP MEDERIC MEDIAN", "BP2S", "FR0000984643", "X");
            new PortfolioCodeAdaptee("6100029", "FCP MEDERIC OFFENSIF", "BP2S", "FR0000984650", "X");
            new PortfolioCodeAdaptee("6100030", "FCP FEDERIS EURO ACTIONS", "BP2S", "FR0007078480", "A");
            new PortfolioCodeAdaptee("6100031", "FCP FEDERIS ASIE ACTIONS", "BP2S", "FR0007079330", "A");
            new PortfolioCodeAdaptee("6100033", "FCP FEDERIS IRC ACTIONS", "BP2S", "FR0010030031", "A");
            new PortfolioCodeAdaptee("6100034", "FCP FEDERIS PRIME OBLIGATIONS", "BP2S", "FR0010027458", "O");
            new PortfolioCodeAdaptee("6100035", "FCP FEDERIS TRESORERIE", "BP2S", "FR0010250597", "X");
            new PortfolioCodeAdaptee("6100037", "FCPE SOREA OBLIGATIONS", "BP2S", "000000008506", "E");
            new PortfolioCodeAdaptee("6100043", "FCPE SOREA ACTIONS ETHIQUES ET SOLIDAIRES", "BP2S", "000000008515", "E");
            new PortfolioCodeAdaptee("6100044", "FCPE SOREA ACTIONS EURO", "BP2S", "000000008517", "E");
            new PortfolioCodeAdaptee("6100047", "FCPE SOREA COURT TERME", "BP2S", "000000008525", "E");
            new PortfolioCodeAdaptee("6100054", "FCPE FAIVELEY ACTIONS", "BP2S", "000000006111", "E");
            new PortfolioCodeAdaptee("6100059", "FCPE SOREA CROISSANCE", "BP2S", "000000008516", "E");
            new PortfolioCodeAdaptee("6100060", "FCP FEDERIS EURO QUATREM", "BP2S", "FR0007044532", "X");
            new PortfolioCodeAdaptee("6100062", "FCP FEDERIS EX EURO", "BP2S", "FR0007022793", "X");
            new PortfolioCodeAdaptee("6100063", "FCP FEDERIS CROISSANCE EURO", "BP2S", "FR0007022967", "X");
            new PortfolioCodeAdaptee("6100064", "FCP FEDERIS INFLATION", "BP2S", "FR0010140244", "X");
            new PortfolioCodeAdaptee("6100065", "FCP FEDERIS IRC ETAT", "BP2S", "FR0010140251", "X");
            new PortfolioCodeAdaptee("6100066", "FCP FEDERIS AMERIQUE ACTIONS", "BP2S", "FR0010193235", "A");
            new PortfolioCodeAdaptee("6100069", "FCP FEDERIS EPARGNE EQUILIBRE", "BP2S", "FR0010250720", "X");
            new PortfolioCodeAdaptee("6100070", "FCP FEDERIS DIVERSIFIE", "BP2S", "FR0010251082", "X");
            new PortfolioCodeAdaptee("6100073", "FCP FEDERIS DIVERSIFIE DYNAMIQUE", "BP2S", "FR0010404731", "X");
            new PortfolioCodeAdaptee("6100074", "FCPE ARCELOR 6 MONETAIRE", "CDC", "990000094039", "X");
            new PortfolioCodeAdaptee("6100075", "FCP UNIFED ÉPARGNE HORIZONS", "CC", "FR0010444836", "X");
            new PortfolioCodeAdaptee("6100076", "FCP FEDERIS OBLIGATIONS ISR", "BP2S", "FR0010622662", "O");
            new PortfolioCodeAdaptee("6100078", "FCPE SOREA ISR MONETAIRE", "BP2S", "QS0011128020", "X");
            new PortfolioCodeAdaptee("6100079", "FCPE SOREA ISR CROISSANCE", "BP2S", "QS0011128038", "X");
            new PortfolioCodeAdaptee("6100080", "FCPE SOREA ISR OBLIGATIONS", "BP2S", "QS0011128046", "O");
            new PortfolioCodeAdaptee("6100081", "FCPE SOREA ISR DYNAMIQUE ET SOLIDAIRE", "BP2S", "QS0011128053", "X");
            new PortfolioCodeAdaptee("6100082", "PRATIC IRC", "BP2S", "FR0010775437", "X");
            new PortfolioCodeAdaptee("6100083", "BOURBON 10", "CDC", "FR0010790030", "X");
            new PortfolioCodeAdaptee("6100084", "FEDERIS INTERNATIONAL QUATREM", "BP2S", "FR0010828673", "X");
            new PortfolioCodeAdaptee("6100085", "FEDERIS SELECTION INDEX", "BP2S", "FR0010827444", "X");
            new PortfolioCodeAdaptee("6100086", "FEDERIS SELECTION ISR EURO", "BP2S", "FR0010822122", "X");
            new PortfolioCodeAdaptee("6100088", "FEDERIS SELECTION EMERGENTS", "BP2S", "FR0011049709", "X");
            new PortfolioCodeAdaptee("6100089", "FEDERIS OPPORTUNITES ACTIONS", "BP2S", "FR0011105279", "A");
            new PortfolioCodeAdaptee("6100090", "FEDERIS SELECTION EURO", "BP2S", "FR0011133438", "X");
            new PortfolioCodeAdaptee("6300009", "QUATREM PERE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("6300010", "QUATREM PERE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("6300110", "QUATREM FONDS PROPRES OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("6300111", "QUATREM FONDS PROPRES ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("6300120", "QUATREM RETRAITE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("6300121", "QUATREM RETRAITE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("6300122", "QUATREM RETRAITE DIVERSIFIE", "BP2S", "D");
            new PortfolioCodeAdaptee("6300130", "QUATREM PREVOYANCE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("6300131", "QUATREM PREVOYANCE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("6300140", "QUATREM SANTE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("6300141", "QUATREM SANTE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("6400001", "MEDERIC EPARGNE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("6400002", "MEDERIC EPARGNE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("7020010", "MM HARMONIE OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("7020011", "MM HARMONIE ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("7201105", "MM AGIRC RT OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("7201106", "MM AGIRC RT ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("7201107", "MM AGIRC RT DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("7301105", "MM AGIRC FG OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("7301106", "MM AGIRC FG ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("7301107", "MM AGIRC FG DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("7401105", "MM AGIRC FS OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("7401106", "MM AGIRC FS ACTIONS", "BP2S", "A");
            new PortfolioCodeAdaptee("7401107", "MM AGIRC FS DIVERSIFIES", "BP2S", "D");
            new PortfolioCodeAdaptee("8010015", "MEDERIC ASSURANCES OBLIGATIONS", "BP2S", "O");
            new PortfolioCodeAdaptee("8020012", "AUXIA DIVERSIFIE (EX MEDERIC VIE GESTION DIV)", "BP2S", "D");
            new PortfolioCodeAdaptee("8501001", "MUTUELLE ALLASSO", "BP2S", "X");
            new PortfolioCodeAdaptee("8502001", "ASSOCIATION LA PORTE VERTE", "BP2S", "X");
            new PortfolioCodeAdaptee("8503001", "LYBERNET ASSURANCES", "BP2S", "D");
            new PortfolioCodeAdaptee("AVCAN", "FCP AVENIR CANTONNE 2", "CIC", "FR0007031646", "X");
            new PortfolioCodeAdaptee("AVEPAR", "SICAV AVENIR EPARGNE", "CIC", "FR0000098253", "X");
            new PortfolioCodeAdaptee("AVEURO", "FCP AVENIR EURO", "BP2S", "FR0007031653", "X");
            new PortfolioCodeAdaptee("AVEUROPE", "FCP FEDERIS VALUE EURO", "BP2S", "FR0007074166", "X");
            new PortfolioCodeAdaptee("FES", "FEDERIS EPARGNE SALARIALE FONDS PROPRES", "BP2S", "X");
        }
        #endregion
    }

    /// <summary>
    /// Represente un code portefeuille
    /// </summary>
    [NotMapped]
    internal sealed class PortfolioCodeAdaptee
    {
        internal readonly string Label;
        internal readonly ISINIdentifier Identification;
        internal readonly string Value;
        internal readonly string Custodian;
        internal readonly PortfolioTypeCode Type;

        internal PortfolioCodeAdaptee(string Value, string Label, string Custodian, string Type):this(Value, Label, Custodian, ISINIdentifier.DEFAULT_VALUE, Type)
        {            
        }
        internal PortfolioCodeAdaptee(string Value, string Label, string Custodian, string Indentification, string Type)
        {
                this.Label = Label;
                this.Value = Value;
                this.Custodian = Custodian;
                this.Identification = new ISINIdentifier(Indentification);
                this.Type = (PortfolioTypeCode)Type;

                Instances[Value] = this;
                Constants[Label] = this;
        }

        public override String ToString()
        {
            return Value;
        }


        #region Singleton partie statique
        internal static readonly Dictionary<string, PortfolioCodeAdaptee> Instances = new Dictionary<string, PortfolioCodeAdaptee>();
        internal static readonly Dictionary<string, PortfolioCodeAdaptee> Constants = new Dictionary<string, PortfolioCodeAdaptee>();
        #endregion


    }
}
