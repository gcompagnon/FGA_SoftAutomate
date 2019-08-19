using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using FGABusinessComponent.BusinessComponent.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace FGABusinessComponent.BusinessComponent.Security
{
    /// <summary>
    /// Les données d indentification / codes / ISIN / tickers pour les actifs
    /// </summary>
    public class SecuritiesIdentification
    {

        #region CONSTRUCTORS
        public SecuritiesIdentification()
///            :this(null)
        {
            this.Name = "DEFAULT";
        }
        public SecuritiesIdentification(ISINIdentifier IsinId)
        {
            this.DomesticIdentificationSource = CountryCode.DEFAULT;
            this.SecurityIdentification = IsinId ?? ISINIdentifier.DEFAULT;
            this.OtherIdentification = "";
            this.ProprietaryIdentificationSource = "";
            this.Bloomberg = BloombergIdentifier.DEFAULT;
            this.RIC = RICIdentifier.DEFAULT;
            this.SEDOL = SEDOLIdentifier.DEFAULT;
            this.CUSIP = CUSIPIdentifier.DEFAULT;
            this.TickerSymbol = TickerIdentifier.DEFAULT;
        }

        /// <summary>
        ///  Init de toutes les identifications
        /// </summary>
        public SecuritiesIdentification(string DomesticIdentificationSource = null,
                                        string Isin = null,
                                        string Name = null,
                                        string OtherIdentification = null,
                                        string ProprietaryIdentificationSource = null,
                                        string Bloomberg = null,
                                        string RIC = null,
                                        string SEDOL = null,
                                        string CUSIP = null,
                                        string Ticker = null)
        {
            if (Name != null && Name.Length > 350)
                Name = Name.Substring(0, 349);
            this.Name = Name;
            this.DomesticIdentificationSource = (CountryCode)DomesticIdentificationSource;
            this.SecurityIdentification = new ISINIdentifier(Isin ?? ISINIdentifier.DEFAULT_VALUE);
            this.OtherIdentification = OtherIdentification ?? "";
            this.ProprietaryIdentificationSource = ProprietaryIdentificationSource ?? "";
            this.Bloomberg = new BloombergIdentifier(Bloomberg ?? BloombergIdentifier.DEFAULT_VALUE);
            this.RIC = new RICIdentifier(RIC ?? RICIdentifier.DEFAULT_VALUE);
            this.SEDOL = new SEDOLIdentifier(SEDOL ?? SEDOLIdentifier.DEFAULT_VALUE);
            this.CUSIP = new CUSIPIdentifier(CUSIP ?? CUSIPIdentifier.DEFAULT_VALUE);
            this.TickerSymbol = new TickerIdentifier(Ticker ?? TickerIdentifier.DEFAULT_VALUE);

        }
        #endregion

        #region Clé Primaire
        [Key, Column(Order=0)]
        public Int64 Id { get; set; }
        #endregion

        /// <summary>
        /// Description ou commentaire
        /// </summary>
        [MaxLength(350)]
        public string Name { get; set; }

        public ISINIdentifier SecurityIdentification { get; set; }

        public CountryCode DomesticIdentificationSource { get; set; }

        [MaxLength(35)]
        public string OtherIdentification { get; set; }

        [MaxLength(35)]
        public string ProprietaryIdentificationSource { get; set; }

        public BloombergIdentifier Bloomberg { get; set; }
        public RICIdentifier RIC { get; set; }
        public SEDOLIdentifier SEDOL { get; set; }


        public CUSIPIdentifier CUSIP { get; set; }
        public TickerIdentifier TickerSymbol { get; set; }
        
        [MaxLength(70)]
        public string TradingIdentification { get; set; }

        //EList<Market> getApplicableTradingMarket();
        //EList<Security> getTradedSecurity();
    }

    [ComplexType]
    public class ISINIdentifier
    {
        public const String DEFAULT_VALUE = "XXXXXXXXXXXX";
        public static ISINIdentifier DEFAULT = new ISINIdentifier(DEFAULT_VALUE);

        public ISINIdentifier()
        {
            this.ISINCode = DEFAULT_VALUE;
        }
        public ISINIdentifier(string Isin)
        {
            this.ISINCode = Isin;
        }

        [Column("ISIN", TypeName = "nchar"), MaxLength(12)]
        public string ISINCode { get; set; }

        public bool validate()
        {
            return ISINCode.Length == 12;
        }

        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator ISINIdentifier(string str)
        {
            return new ISINIdentifier(str);
        }


        /// <summary>
        /// Operateur d equalite
        /// </summary>
        public static bool operator ==(ISINIdentifier id, string str)
        {
            return id.ISINCode.TrimEnd().Equals(str, StringComparison.OrdinalIgnoreCase);
        }
        public static bool operator !=(ISINIdentifier id, string str)
        {
            return !id.ISINCode.TrimEnd().Equals(str, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (this.ISINCode == null)
                return false;
            if (obj is ISINIdentifier)
            {
                ISINIdentifier id = (ISINIdentifier)obj;
                return this.ISINCode.TrimEnd().Equals(id.ISINCode, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is String)
            {
                String id = (String)obj;
                return this.ISINCode.TrimEnd().Equals(id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public override string ToString()
        {
            if (ISINCode != null)
                return "ISIN:" + ISINCode.ToString();         
            return null;
        }
        public override int GetHashCode()
        {
            return ISINCode.GetHashCode();
        }
    }

    [ComplexType]
    public sealed class BloombergIdentifier
    {
        public const string DEFAULT_VALUE = "";
        public static BloombergIdentifier DEFAULT = new BloombergIdentifier(DEFAULT_VALUE);

        public BloombergIdentifier()
        {
        }
        public BloombergIdentifier(string code)
        {
            this.BBCode = code;
        }

        [Column("Bloomberg", TypeName = "nchar"), MaxLength(20)]
        public string BBCode { get; set; }

        public bool validate()
        {
            return BBCode.Length == 20;
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator BloombergIdentifier(string str)
        {
            return new BloombergIdentifier(str);
        }

        /// <summary>
        /// Operateur d equalite
        /// </summary>
        public static bool operator ==(BloombergIdentifier id, string str)
        {
            return id.BBCode.TrimEnd().Equals(str, StringComparison.OrdinalIgnoreCase);
        }
        public static bool operator !=(BloombergIdentifier id, string str)
        {
            return !id.BBCode.TrimEnd().Equals(str, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (this.BBCode == null)
                return false;
            if (obj is BloombergIdentifier)
            {
                BloombergIdentifier id = (BloombergIdentifier)obj;
                return this.BBCode.TrimEnd().Equals(id.BBCode, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is String)
            {
                String id = (String)obj;
                return this.BBCode.TrimEnd().Equals(id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return BBCode.GetHashCode();
        }


    }
    /// <summary>
    /// Identifiant de type Reuters
    /// </summary>
    [ComplexType]
    public class RICIdentifier
    {
        public const string DEFAULT_VALUE = "";
        public static RICIdentifier DEFAULT = new RICIdentifier(DEFAULT_VALUE);

        public RICIdentifier()
        {
        }
        public RICIdentifier(string code)
        {
            this.RICCode = code;
        }

        [Column("Reuters", TypeName = "nchar"), MaxLength(20)]
        public string RICCode { get; set; }

        public bool validate()
        {
            return true;
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator RICIdentifier(string str)
        {
            return new RICIdentifier(str);
        }
        /// <summary>
        /// Operateur d equalite
        /// </summary>
        public static bool operator ==(RICIdentifier id, string str)
        {
            return id.RICCode.TrimEnd().Equals(str, StringComparison.OrdinalIgnoreCase);
        }
        public static bool operator !=(RICIdentifier id, string str)
        {
            return !id.RICCode.TrimEnd().Equals(str, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (this.RICCode == null)
                return false;
            if (obj is RICIdentifier)
            {
                RICIdentifier id = (RICIdentifier)obj;
                return this.RICCode.TrimEnd().Equals(id.RICCode, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is String)
            {
                String id = (String)obj;
                return this.RICCode.TrimEnd().Equals(id, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        public override int GetHashCode()
        {
            return RICCode.GetHashCode();
        }

    }
    /// <summary>
    /// SEDOL Identifier
    /// </summary>    
    [ComplexType]
    public class SEDOLIdentifier
    {
        public const string DEFAULT_VALUE = "";
        public static SEDOLIdentifier DEFAULT = new SEDOLIdentifier(DEFAULT_VALUE);

        public SEDOLIdentifier()
        {
        }
        public SEDOLIdentifier(string code)
        {
            this.SEDOL = code;
        }

        [Column("SEDOL", TypeName = "nchar"), MaxLength(20)]
        public string SEDOL { get; set; }

        public bool validate()
        {
            return true;
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator SEDOLIdentifier(string str)
        {
            return new SEDOLIdentifier(str);
        }
        public override bool Equals(object obj)
        {
            if (this.SEDOL == null)
                return false;

            if (obj is SEDOLIdentifier)
            {
                SEDOLIdentifier id = (SEDOLIdentifier)obj;
                return this.SEDOL.TrimEnd().Equals(id.SEDOL, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is String)
            {
                String id = (String)obj;
                return this.SEDOL.TrimEnd().Equals(id, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        public override string ToString()
        {
            if (SEDOL != null)
                return "SEDOL:" + SEDOL.ToString();
            return null;
        }
        public override int GetHashCode()
        {
            return SEDOL.GetHashCode();
        }
    }
    /// <summary>
    /// Identifiant de type Reuters
    /// </summary>
    [ComplexType]
    public class CUSIPIdentifier
    {
        public const string DEFAULT_VALUE = "";
        public static CUSIPIdentifier DEFAULT = new CUSIPIdentifier(DEFAULT_VALUE);

        public CUSIPIdentifier()
        {
        }
        public CUSIPIdentifier(string code)
        {
            this.CUSIPCode = code;
        }

        [Column("CUSIP", TypeName = "nchar"), MaxLength(20)]
        public string CUSIPCode { get; set; }

        public bool validate()
        {
            return true;
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator CUSIPIdentifier(string str)
        {
            return new CUSIPIdentifier(str);
        }
        public override bool Equals(object obj)
        {
            if (this.CUSIPCode == null)
                return false;

            if (obj is CUSIPIdentifier)
            {
                CUSIPIdentifier id = (CUSIPIdentifier)obj;
                return this.CUSIPCode.TrimEnd().Equals(id.CUSIPCode, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is String)
            {
                String id = (String)obj;
                return this.CUSIPCode.TrimEnd().Equals(id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public override string ToString()
        {
            if (CUSIPCode != null)
                return "CUSIP:" + CUSIPCode.ToString();
            return null;
        }
        public override int GetHashCode()
        {
            return CUSIPCode.GetHashCode();
        }
    }

    /// <summary>
    /// Identifiant de type Reuters
    /// </summary>
    [ComplexType]
    public class TickerIdentifier
    {
        public const string DEFAULT_VALUE = "";
        public static TickerIdentifier DEFAULT = new TickerIdentifier(DEFAULT_VALUE);

        public TickerIdentifier()
        {
        }
        public TickerIdentifier(string code)
        {
            this.TickerCode = code;
        }

        [Column("Ticker", TypeName = "nchar"), MaxLength(20)]
        public string TickerCode { get; set; }

        public bool validate()
        {
            return true;
        }
        /// <summary>
        /// Gestion du cast entre le code et le type
        /// </summary>
        public static explicit operator TickerIdentifier(string str)
        {
            return new TickerIdentifier(str);
        }
        public override bool Equals(object obj)
        {
            if (this.TickerCode == null)
                return false;

            if (obj is TickerIdentifier)
            {
                TickerIdentifier id = (TickerIdentifier)obj;
                return this.TickerCode.TrimEnd().Equals(id.TickerCode, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is String)
            {
                String id = (String)obj;
                return this.TickerCode.TrimEnd().Equals(id, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
        public override string ToString()
        {
            if (TickerCode != null)
                return "Ticker:" + TickerCode.ToString();
            return null;
        }
        public override int GetHashCode()
        {
            return TickerCode.GetHashCode();
        }
    }

    /// <summary>
    /// configuration Fluent API 
    /// En complément de la configuration par Attributs: DataAnnotations
    /// </summary>
    public class SecuritiesIdentificationModelConfiguration : EntityTypeConfiguration<SecuritiesIdentification>
    {
        public SecuritiesIdentificationModelConfiguration()
        {
            ToTable(tableName: "IDENTIFICATION", schemaName: "ref_common");            
            // faire en sorte que la clé soit généré par la base (type identity)
            Property(d => d.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
