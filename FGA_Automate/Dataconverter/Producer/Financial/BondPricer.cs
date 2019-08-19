using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGA.Automate.Config;
using System.Text.RegularExpressions;
using System.Data;
using FGA.SQLCopy;
using System.Data.SqlClient;

namespace FGA.Automate.Dataconverter.Producer.Financial
{
    /// <summary>
    /// Classe rassemblant les algo de calcul de prix ( basé sur BusinessComponent)
    /// Charge des données depuis une BDD et calcul pour sortir un DataSet
    /// Producer
    /// </summary>
    /// 

/*
    -datastore=FGA_PROD -bondPricer=yes -nogui -sql="G:\TQA\Taux\DurationCalc\dataInputs.sql" -@date='31/12/2014' -xls="C:\DURATION.xls"
    */

    class BondPricer
    {
        public enum OPTION { ALL, CLEANPRICE, DURATION, DIRTYPRICE, ACCRUEDINTEREST };


        // les differentes bases de donnees 
        public enum DATASTORE { OMEGA, OMEGA_RECETTE, FGA_PROD, FGA_RECETTE };
        public static string[] DATASTORES = { "OMEGA", "OMEGA_RECETTE", "FGA_PROD", "FGA_RECETTE" };
        private string request = null;
        private string connection = null;
        private int connection_timeout = -1;

        // pour la configuration, les clés possibles sont parameterKey[i]+'_'+DATASTORE[j]: par exemple SERVEUR_BDD_FGA_PROD
        private enum keys { user = 0, password = 1, server = 2, name = 3, authentification = 4, timeout = 5, connection_string = 6 };
        private static string[] parameterKey = { "USER", "PASSWORD", "SERVEUR_BDD", "NAME_BDD", "WINDOWS_AUTHENTIFICATION", "CONNECT_TIMEOUT", "CONNECTION_STRING" };

        public string getConnectionString()
        {
            return connection;
        }

        /// <summary>
        /// Constructeur prenant le nom de la BDD en paramètre 
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// pour construire la chaine de connection
        /// </summary>
        /// <param name="dataStore">par défaut ce sera la base OMEGA voir SQLrequest.DATASTORE</param>
        public BondPricer(string dataStore = "OMEGA")
        {
            string[] Keys = new string[parameterKey.Length];

            if (dataStore != null)
            {
                for (int i = 0; i < parameterKey.Length; i++)
                {
                    Keys[i] = parameterKey[i] + '_' + dataStore;
                }
            }

            // lecture du fichier de configuration
            IDictionary<string, string> m = InitFile.ReadConfigFile();

            string windows_auth = Keys[(int)keys.authentification];
            string server = Keys[(int)keys.server];
            string database = Keys[(int)keys.name];
            string uid = Keys[(int)keys.user];
            string pwd = Keys[(int)keys.password];
            string timeout = Keys[(int)keys.timeout];
            string connection_string = Keys[(int)keys.connection_string];

            if (m.ContainsKey(windows_auth) && m[windows_auth].Equals("TRUE"))
            {
                connection = "server = " + m[server] +
                            " ; Integrated Security = true" + // l authentification est lié au user connecté
                            " ; database = " + m[database];
            }
            else
            {
                connection = "server = " + m[server] +
                            " ; uid = " + m[uid] +
                            " ; pwd = " + m[pwd] +
                            " ; database = " + m[database];

            }
            if (m.ContainsKey(timeout))
            {
                try
                {
                    connection_timeout = Convert.ToInt32(m[timeout]);
                }
                catch (Exception e)
                {
                    IntegratorBatch.ExceptionLogger.Error("Impossible de convertir la valeur du Timeout: " + timeout + "=" + m[timeout], e);
                }

                connection += " ; Connect Timeout = " + m[timeout];
            }

            // utilisation tel quel de la connection string
            if (m.ContainsKey(connection_string))
            {
                connection = m[connection_string];
            }
            IntegratorBatch.InfoLogger.Debug("La connection sur la base utilisée est " + connection);
        }
        /// <summary>
        /// Constructeur prenant le chemin du fichier et le nom de la BDD en paramètres 
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// pour construire la chaine de connection
        /// </summary>
        /// <param name="fileName">le chemin du fichier contenant la requete</param>
        /// <param name="dataStore">par défaut ce sera la base OMEGA</param>
        public BondPricer(string fileName, string dataStore = "OMEGA")
            : this(dataStore)
        {
            // lecture du fichier de requete
            request = InitFile.ReadFile(fileName);
        }

        /// <summary>
        /// Constructeur prenant le chemin du fichier en paramètre
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// </summary>
        /// <param name="fileName">le chemin du fichier contenant la requete</param>
        /// <param name="dataStore">par défaut ce sera la base OMEGA</param>
        /// <param name="parameters">liste des parametres @xxx=yyy</param>
        public BondPricer(string fileName, string dataStore, string[] parameters, string[] values)
            : this(fileName, dataStore)
        {
            // on remplace le set @xxx=zzz de request par set @xxx=yyy passée en parametre
            for (int i = 0; i < parameters.Length; i++)
            {
                string pattern = @"set\s+" + parameters[i] + @"\s*=.*";

                if (values[i] != null && values[i].Trim().Length > 0)
                {
                    // gestion de la constante NOW 
                    if (values[i].Contains("NOW"))
                    {
                        values[i] = values[i].Replace("NOW", "GetDate()");
                    }
                    string replacement = "set " + parameters[i] + "=" + values[i];
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    request = rgx.Replace(request, replacement);
                }
            }
        }

        /// <summary>
        /// Init the DataTable structure for the returned DataSet
        /// </summary>
        static BondPricer() {
            _columns = new Dictionary<OPTION, DataColumn>(4);

            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;
            // Create new DataColumn, set DataType,  
            // ColumnName and add to DataTable.    
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "ISIN";
            column.AutoIncrement = false;
            column.Caption = "ISIN";
            column.ReadOnly = false;
            column.Unique = false;
            _ISINColumn = column;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.DateTime");
            column.ColumnName = "DATE";
            column.AutoIncrement = false;
            column.Caption = "Calc Date";
            column.ReadOnly = false;
            column.Unique = false;
            _DATEColumn = column;

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "DURATION";
            column.AutoIncrement = false;
            column.Caption = "Calc Duration";
            column.ReadOnly = false;
            column.Unique = false;
            _columns.Add(OPTION.DURATION, column); 

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "CLEANPRICE";
            column.AutoIncrement = false;
            column.Caption = "Calc clean Price wo accrued interests";
            column.ReadOnly = false;
            column.Unique = false;
            _columns.Add(OPTION.CLEANPRICE, column); 
            
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "DIRTYPRICE";
            column.AutoIncrement = false;
            column.Caption = "Calc dirty Price price and accrued interests";
            column.ReadOnly = false;
            column.Unique = false;
            _columns.Add(OPTION.DIRTYPRICE, column); 

            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Double");
            column.ColumnName = "ACCRUEDINTEREST";
            column.AutoIncrement = false;
            column.Caption = "accrued interests";
            column.ReadOnly = false;
            column.Unique = false;
            _columns.Add(OPTION.ACCRUEDINTEREST, column);
        }
        private DataTable _returnedTable;
        private static IDictionary<OPTION,DataColumn> _columns;

        private static DataColumn _ISINColumn;
        private static DataColumn _DATEColumn;

        private DataSet prepareDataSet(OPTION outputOption)
        {
            DataSet ret = new DataSet();
            // Create a new DataTable.
            _returnedTable = new DataTable("BondPricer");

            _returnedTable.Columns.Add(_ISINColumn);
            _returnedTable.Columns.Add(_DATEColumn);
            if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.DURATION))
            {
                _returnedTable.Columns.Add(_columns[OPTION.DURATION]);
            }
            if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.ACCRUEDINTEREST))
            {
                _returnedTable.Columns.Add(_columns[OPTION.ACCRUEDINTEREST]);
            }
            if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.CLEANPRICE))
            {
                _returnedTable.Columns.Add(_columns[OPTION.CLEANPRICE]);
            }
            if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.DIRTYPRICE))
            {
                _returnedTable.Columns.Add(_columns[OPTION.DIRTYPRICE]);
            }

            ret.Tables.Add(_returnedTable); 
            return ret;
        }

        /// <summary>
        /// Remplit la dataset donnée en paramètre avec le resultat du calcul de :
        /// OPTION.DURATION , ou ou CLEANPRICE ou DIRTYPRICE ou ACCRUEDPRICE
        /// sur la base source
        /// </summary>
        /// <param name="DS"></param>
        public DataSet Execute(DataSet INPUTS,OPTION outputOption = OPTION.ALL )
        {
            DataRow row;
            DataSet ret = prepareDataSet(outputOption);

            // lecture input
            foreach (DataTable dt in INPUTS.Tables)
            {                
                IntegratorBatch.InfoLogger.Debug("Nb de bonds à pricer :" + dt.Rows.Count);
                foreach (DataRow dr in dt.Rows)
                {
                    // calcul pour une oblig
                    string ISIN = dr.Field<string>("ISIN");
                    DateTime DATE = dr.Field<DateTime>("DATE");
                    DateTime maturity = dr.Field<DateTime>("MATURITY");

                    double ttm = (dr.Field<double?>("TTM") ?? getDays_30E360(DATE, maturity));
                    double c = dr.Field<double>("COUPON");
                    double r = dr.Field<double>("RATE");
                    
                    int nbPeriods;
                    if (ttm != null)
                    {
                        nbPeriods = (int)Math.Truncate((double)ttm);
                    }
                    else
                    {
                        // Calcul impossible
                        continue;
                    }

                    double? cp = null,ai = null;

                    row = _returnedTable.NewRow();
                    row["ISIN"] = ISIN;
                    row["DATE"] = DATE;

                    if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.CLEANPRICE))
                    {
                        cp = CleanPrice(c, nbPeriods, r);
                        row["CLEANPRICE"] = cp;
                    }

                    if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.DURATION))
                    {
                        if (c == 0 || nbPeriods <= 1)
                        {
                            row["DURATION"] = ttm;
                        }
                        else
                        {
                            cp = cp ?? CleanPrice(c, nbPeriods, r);
                            double duration = Duration((double)cp, c, nbPeriods, r);
                            row["DURATION"] = duration;
                        }
                    }
                    if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.ACCRUEDINTEREST))
                    {
                        ai = AccruedInterest(c, nbPeriods, ttm);
                        row["ACCRUEDINTEREST"] = ai;
                    }
                    if (outputOption.Equals(OPTION.ALL) || outputOption.Equals(OPTION.DIRTYPRICE))
                    {
                        cp = cp ?? CleanPrice(c, nbPeriods, r);
                        ai = ai??AccruedInterest(c, nbPeriods, ttm);
                        row["DIRTYPRICE"] = cp + ai;
                    }
                    _returnedTable.Rows.Add(row);
                }
            }
            return ret;

        }


        public double getDays_30E360(DateTime refDate,DateTime maturity)
        {            
            return Days360(refDate,maturity,true) / 360.00;
        }
        public double getDays_30360(DateTime refDate, DateTime maturity)
        {
            return Days360(refDate, maturity) / 360.00;
        }

        /// <summary>
        /// DAYS360 from Excel.
        /// The European Method:
        /// If either date A or B falls on the 31st of the month, that date will be changed to the 30th;
        ///  Where date B falls on the last day of February, the actual date B will be used.
        /// The US/NASD Method:
        /// If both date A and B fall on the last day of February, then date B will be changed to the 30th.
        /// If date A falls on the 31st of a month or last day of February, then date A will be changed to the 30th.
        /// If date A falls on the 30th of a month after applying (2) above and date B falls on the 31st of a month, then date B will be changed to the 30th.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="method"> European Method (true) or  US/NASD Method (false)</param>
        /// <returns></returns>
        public static double Days360(DateTime dtStartDate, DateTime dtEndDate, Boolean method = false)
        {
            int startMonthDays = 0;
            int endMonthDays = 0;
            double diff = 0;
            if (method)
            {

                if (dtStartDate.Day < 30)
                {
                    startMonthDays = (30 - dtStartDate.Day);
                }
                else
                {
                    startMonthDays = 0;
                }

                if (dtEndDate.Day < 30)
                {
                    endMonthDays = dtEndDate.Day;
                }
                else
                {
                    endMonthDays = 30;
                }

                diff = (dtEndDate.Year - dtStartDate.Year) * 360 +
                                (dtEndDate.Month - dtStartDate.Month - 1) * 30 +
                                startMonthDays + endMonthDays;
            }
            else
            {
                if (daysInMonth(dtStartDate.Year, dtStartDate.Month) == dtStartDate.Day)
                {
                    startMonthDays = 0;
                }
                else
                {
                    startMonthDays = (30 - dtStartDate.Day);
                }

                if (daysInMonth(dtEndDate.Year, dtEndDate.Month) == dtEndDate.Day)
                {
                    if (dtStartDate.Day < daysInMonth(dtStartDate.Year, dtStartDate.Month) - 1)
                    {
                        if (daysInMonth(dtEndDate.Year, dtEndDate.Month) > 30)
                        {
                            endMonthDays = daysInMonth(dtEndDate.Year, dtEndDate.Month);
                        }
                        else
                        {
                            endMonthDays = dtEndDate.Day;
                        }
                    }
                    else
                    {
                        if (daysInMonth(dtEndDate.Year, dtEndDate.Month) > 30)
                        {
                            endMonthDays = daysInMonth(dtEndDate.Year, dtEndDate.Month) - 1;
                        }
                        else
                        {
                            endMonthDays = dtEndDate.Day;
                        }
                    }
                }
                else
                {
                    endMonthDays = dtEndDate.Day;

                }
                diff = (dtEndDate.Year - dtStartDate.Year) * 360 +
                            (dtEndDate.Month - dtStartDate.Month - 1) * 30 +
                            startMonthDays + endMonthDays;
            }
            return diff;
        }
        private static readonly int[] table = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        public static int daysInMonth(int year, int month)
        {
            if (DateTime.IsLeapYear(year) && month == 2)
            {
                return 29;
            }
            else
            {
                return table[month - 1];
            }
        }
        public double getDays_ACT_365360(DateTime refDate, DateTime maturity)
        {
            TimeSpan time = maturity.Subtract(refDate);
            return time.Days / 365.00;
        }

        public double Duration(double coupon, int nPeriods, double rate, double redemptionValue = 100.0)
        {
            double price = CleanPrice(coupon, nPeriods, rate, redemptionValue);
            double numerator = PresentValue(redemptionValue * nPeriods, nPeriods, rate) + PresentValueWeightCoupon(coupon, nPeriods, rate);
            return numerator / price;
        }

        public double Duration(double price, double coupon, int nPeriods, double rate, double redemptionValue = 100.0)
        {
            double numerator = PresentValue(redemptionValue * nPeriods, nPeriods, rate) + PresentValueWeightCoupon(redemptionValue*coupon, nPeriods, rate);
            return numerator / price;
        }

        public double AccruedInterest(double coupon, int nPeriods, double TTM, double redemptionValue = 100.0)
        {
            return (TTM - nPeriods) * coupon * redemptionValue;
        }


        public double DirtyPrice(double coupon, int nPeriods, double rate, double TTM, double redemptionValue = 100.0)
        {
            return PresentValue(redemptionValue, nPeriods, rate) + PresentValueCoupon(redemptionValue*coupon, nPeriods, rate) + AccruedInterest(coupon, nPeriods, TTM, redemptionValue);
        }

        public double CleanPrice(double coupon, int nPeriods, double rate, double redemptionValue = 100.0)
        {
            return PresentValue(redemptionValue, nPeriods, rate) + PresentValueCoupon(redemptionValue*coupon, nPeriods, rate);
        }

        public double PresentValueCoupon(double coupon, int nPeriods, double rate)
        {
            double factor = 1 + rate;
            double PV = 0.0;
            for (int t = 0; t < nPeriods; t++)
            {
                PV += 1.0 / Math.Pow(factor, t + 1);
            }
            return PV * coupon;
        }


        public double PresentValueWeightCoupon(double coupon, int nPeriods, double rate)
        {
            double factor = 1 + rate;
            double PV = 0.0;
            for (int t = 0; t < nPeriods; t++)
            {
                PV += (t + 1) / Math.Pow(factor, t + 1);
            }
            return PV * coupon;
        }



        public double FutureValue(double P0, int nPeriods, double rate)
        {
            return P0 * Math.Pow( 1 + rate , nPeriods );
        }
        public double PresentValue(double Pn, int nPeriods, double rate)
        {
            return Pn * ( 1.0 / Math.Pow(1 + rate, nPeriods));
        }



    }


}
