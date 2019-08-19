using log4net.Config;
using log4net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer;

using System.Data.SqlClient;

using System.Data;
using System.Collections.Specialized;

using System.Configuration;

using System.IO;
using log4net.Repository.Hierarchy;
using log4net.Repository;
using log4net.Core;
using LumenWorks.Framework.IO.Csv;
using SQLCopy.Dbms;

namespace FGA.SQLCopy
{
    #region Enumeration And Struct
    public enum ListeMode { Aucune, Exclude, Include };
    #endregion


    /// <summary>
    /// Utilitaires pour faire des copies entre 2 bases: source vers destination
    /// </summary>
    public class MSDBIntegration
    {

        #region REQUEST CONST STRING
        private const string SET_IDENTITY_ONOFF = "IF OBJECTPROPERTY(OBJECT_ID('{0}.{1}'), 'TableHasIdentity') = 1 \n  set identity_insert [{0}].[{1}] {2}";
        #endregion


        #region logguer
        internal sealed class DummyLogger : Logger
        {
            // Methods
            internal DummyLogger(string name)
                : base(name)
            {
            }
        }

        private static ILog getDefaultLogger(String name)
        {
            ILog r = LogManager.GetCurrentLoggers().SingleOrDefault(x => x.Logger.Name == name);
            if (r != null)
            {
                return r;
            }

            Hierarchy hierarchy = (Hierarchy)r;

            log4net.Appender.ColoredConsoleAppender appender = new
        log4net.Appender.ColoredConsoleAppender();
            appender.Name = name;

            log4net.Layout.PatternLayout layout = new log4net.Layout.PatternLayout();
            layout.ConversionPattern = "%-4timestamp [%thread] %-5level %logger %ndc - %message%newline";
            layout.ActivateOptions();

            appender.Layout = layout;

            log4net.Filter.LevelRangeFilter filter = new log4net.Filter.LevelRangeFilter();
            filter.LevelMax = log4net.Core.Level.Fatal;
            filter.LevelMin = log4net.Core.Level.Debug;

            appender.AddFilter(filter);
            appender.ActivateOptions();

            hierarchy.Root.AddAppender(appender);

            hierarchy.Root.Level = log4net.Core.Level.All;
            hierarchy.Configured = true;

            DummyLogger dummyILogger = new DummyLogger(name);
            dummyILogger.Hierarchy = hierarchy;
            dummyILogger.Level = log4net.Core.Level.All;

            dummyILogger.AddAppender(appender);

            return new log4net.Core.LogImpl(dummyILogger); ;
        }

        private static ILog pInfoLogger;
        public static ILog InfoLogger
        {
            get
            {
                if (pInfoLogger == null)
                {
                    pInfoLogger = getDefaultLogger("Info");
                }

                return pInfoLogger;
            }
            set
            {
                pInfoLogger = value;
            }
        }

        private static ILog pExceptionLogger;
        public static ILog ExceptionLogger
        {
            get
            {
                if (pExceptionLogger == null)
                {
                    pExceptionLogger = getDefaultLogger("Exception");
                }
                return pExceptionLogger;
            }
            set
            {
                pExceptionLogger = value;
            }
        }

        #endregion

        #region constructeurs
        private DBConnectionDelegate SourceConnection;
        private DBConnectionDelegate DestinationConnection;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceConnection"></param>
        /// <param name="destinationConnection"></param>
        public MSDBIntegration(String sourceConnection = null, String destinationConnection = null)
        {
            if (sourceConnection != null)
            {
                SourceConnection = new MSSQL2005_DBConnection(sourceConnection);
            }

            if (destinationConnection != null)
            {
                DestinationConnection = new MSSQL2005_DBConnection(destinationConnection);
            }
        }
        public MSDBIntegration(DBConnectionDelegate sourceConnection = null, DBConnectionDelegate destinationConnection = null)
        {
            if (sourceConnection != null)
            {
                SourceConnection = sourceConnection;
            }

            if (destinationConnection != null)
            {
                DestinationConnection = destinationConnection;
            }
        }

        #endregion

        ///// <summary>
        ///// Operateur pour convertir un type de données DataSet en type SQL Server
        ///// </summary>
        ///// <param name="dataType"></param>
        ///// <returns></returns>
        //public static DataType getDataType(string DT)
        //{
        //    DataType DTTemp = null;

        //    switch (DT)
        //    {
        //        case ("System.Decimal"):
        //            DTTemp = DataType.Decimal(2, 18);
        //            break;
        //        case ("System.String"):
        //            DTTemp = DataType.VarChar(50);
        //            break;
        //        case ("System.Int32"):
        //            DTTemp = DataType.Int;
        //            break;
        //        case ("System.Double"):
        //            DTTemp = DataType.Float;
        //            break;
        //        case ("System.DateTime"):
        //            DTTemp = DataType.DateTime;
        //            break;
        //        case ("System.Single"):
        //            DTTemp = DataType.Real;
        //            break;
        //        default:
        //            ExceptionLogger.Error("Convertion of " + DT + " to SqlServer.management.Smo.DataType UNKNOWN");
        //            break;
        //    }
        //    return DTTemp;
        //}





        /// <summary>
        ///Copie les données de la bdd Source vers la Destination pour toutes les tables, ou pour celles incluent dans la liste (en mode Include) ou toutes sauf celles de la liste (en mode Exclude)
        /// </summary>
        /// <param name="mode">Aucune pour ne pas prendre en compte la liste. Include si la liste est une liste de table à inclure et exclude  si la copie est sur toute la base sauf les tables de la liste</param>
        /// <param name="ListTableName">La liste des noms de tables</param>        
        public void bulkcopyData(ListeMode mode = ListeMode.Aucune, List<DatabaseTable> ListTableName = null)
        {
            string requestSelect = "SELECT * FROM {0}.{1}";
            this.bulkcopyData(mode, ListTableName, requestSelect);
        }

        /// <summary>
        ///Copie les données de la bdd Source vers la Destination pour les n dernieres lignes 
        /// </summary>
        /// <param name="mode">Aucune pour ne pas prendre en compte la liste. Include si la liste est une liste de table à inclure et exclude  si la copie est sur toute la base sauf les tables de la liste</param>
        /// <param name="ListTableName">La liste des noms de tables</param>        
        /// <param name="BOTTOMSize">nombre de lignes à ajouter ou null si toutes les lignes des tables sont prises </param>
        /// <param name="columnOrder">critere pour l ordre des n lignes à prendre si TOPsize est non null </param>        
        public void bulkcopyData(ListeMode mode = ListeMode.Aucune, List<DatabaseTable> ListTableName = null,
            int? BOTTOMSize = null, string columnOrder = null)
        {
            string requestSelect;
            if (BOTTOMSize == null)
            {
                requestSelect = "SELECT * FROM {0}.{1}";
            }
            else if (columnOrder == null)
            {
                requestSelect = "SELECT TOP " + BOTTOMSize + " * FROM {0}.{1} ";
            }
            else
            {
                requestSelect = "SELECT * FROM ( SELECT TOP " + BOTTOMSize + " * FROM {0}.{1} ORDER BY " + columnOrder + " desc ) A ORDER BY " + columnOrder;
            }
            this.bulkcopyData(mode, ListTableName, requestSelect);
        }

        /// <summary>
        ///Copie les données de la bdd Source vers la Destination pour les lignes respectants le critere en parametre
        /// </summary>
        /// <param name="mode">Aucune pour ne pas prendre en compte la liste. Include si la liste est une liste de table à inclure et exclude  si la copie est sur toute la base sauf les tables de la liste</param>
        /// <param name="ListTableName">La liste des noms de tables</param>        
        /// <param name="ColumnCriteria">nom de la colonne sur laquelle </param>
        /// <param name="CriteriaValue">critere pour l ordre des n lignes à prendre si TOPsize est non null </param>        
        public void bulkcopyData(ListeMode mode = ListeMode.Aucune, List<DatabaseTable> ListTableName = null,
            string ColumnCriteria = null, string CriteriaValue = "MAX")
        {
            string requestSelect;
            if (ColumnCriteria == null)
            {
                requestSelect = "SELECT * FROM {0}.{1}";
            }
            else if (CriteriaValue == "MAX")
            {
                requestSelect = "SELECT * FROM {0}.{1} where " + ColumnCriteria + " = (select MAX(" + ColumnCriteria + ") from  {0}.{1} )";
            }
            else if (CriteriaValue == "MIN")
            {
                requestSelect = "SELECT * FROM {0}.{1} where " + ColumnCriteria + " = (select MIN(" + ColumnCriteria + ") from  {0}.{1} )";
            }
            else
            {
                requestSelect = "SELECT * FROM {0}.{1} where " + ColumnCriteria + " = " + CriteriaValue;
            }
            this.bulkcopyData(mode, ListTableName, requestSelect);
        }

        /// <summary>
        ///Copie les données de la bdd Source vers la Destination
        ///note1: Identity column: copy as it from source to destination, using KeepIdentity option
        ///note2: Null values: copy as it even if column has default value, using KeepNulls option
        /// </summary>
        /// <param name="mode">Aucune pour ne pas prendre en compte la liste. Include si la liste est une liste de table à inclure et exclude  si la copie est sur toute la base sauf les tables de la liste</param>
        /// <param name="ListTableName">La liste des noms de tables</param>        
        /// <param name="requestSelect">la requete utilisée pour recupérer les données sur chaque table, paramètres {0} pour le schema et {1} pour le nom de table</param>
        public void bulkcopyData(ListeMode mode, List<DatabaseTable> ListTableName, string RequestSelect)
        {

            // Open a sourceConnection to the first database
            SourceConnection.Open();

            //SMO Server object setup with SQLConnection.

            Server server = new Server(new ServerConnection((SqlConnection)SourceConnection.Connection));
            string dbName = SourceConnection.Connection.Database.ToString();

            //Set Database to the database
            Database db = server.Databases[dbName];

            //connection en destination
            DestinationConnection.Open();
            SqlBulkCopyOptions Options = SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.KeepIdentity;
            SqlTransaction Transaction = ((SqlConnection)DestinationConnection.Connection).BeginTransaction();

            SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)DestinationConnection.Connection, Options, Transaction);
            bulkCopy.BulkCopyTimeout = 120;
            bulkCopy.BatchSize = 1500;

            string requestSelectToExecute = null;
            Table table = null;
            SqlDataReader reader = null;
            try
            {
                foreach (Table myTable in db.Tables)
                {
                    // si il y a une liste
                    if (mode.Equals(ListeMode.Include))// if this table t is in our parameter list => include it
                    {
                        if (!ListTableName.Contains(myTable.Name, myTable.Schema))
                            continue;
                    }
                    else if (mode.Equals(ListeMode.Exclude)) // if this table t is in our parameter list => do not include it
                    {
                        if (ListTableName.Contains(myTable.Name, myTable.Schema))
                            continue;

                    }
                    table = myTable;

                    //InfoLogger.Debug("Desactivation des cles identity sur la table : " + table.Name);
                    string requestSet = String.Format(SET_IDENTITY_ONOFF, table.Schema, table.Name, "ON");
                    SqlCommand SqlCmdSetIdentity = new SqlCommand(requestSet, (SqlConnection)DestinationConnection.Connection, Transaction);
                    int nb = SqlCmdSetIdentity.ExecuteNonQuery();
                    InfoLogger.Debug("Desactivation des cles identity sur la table : OK " + nb);

                    InfoLogger.Debug("Obtention des données table : " + table.Name);
                    // Get data from the source table as a SqlDataReader.
                    requestSelectToExecute = String.Format(RequestSelect, table.Schema, table.Name);
                    SqlCommand commandSourceData = new SqlCommand(
                        requestSelectToExecute, (SqlConnection)
                        SourceConnection.Connection);
                    reader = commandSourceData.ExecuteReader();
                    InfoLogger.Debug(" -- OK");

                    InfoLogger.Debug("Ecriture des données nouvelle BDD");

                    // Write from the source to the destination.
                    bulkCopy.DestinationTableName = table.Schema + "." + table.Name;
                    bulkCopy.WriteToServer(reader);

                    InfoLogger.Debug(" -- OK");
                    reader.Close();

                    InfoLogger.Debug("reactivation des cles identity sur la table : " + table.Name);
                    requestSet = String.Format(SET_IDENTITY_ONOFF, table.Schema, table.Name, "OFF");
                    SqlCmdSetIdentity = new SqlCommand(requestSet, (SqlConnection)DestinationConnection.Connection, Transaction);
                    nb = SqlCmdSetIdentity.ExecuteNonQuery();
                    InfoLogger.Debug("reactivation des cles identity sur la table : OK " + nb);

                }//Fin foreach

                // Transaction OK : commit all stuff: for all tables
                Transaction.Commit();

            }
            catch (Exception ex)
            {
                if (table != null)
                {
                    ExceptionLogger.Error("BULK IMPOSSIBLE " + requestSelectToExecute + " DANS " + table.Schema + "." + table.Name);
                }
                ExceptionLogger.Error(ex);

                // cancel all modif
                Transaction.Rollback();
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                Transaction.Dispose();
            }

            DestinationConnection.Close();
            SourceConnection.Close();
        }

        /// <summary>
        /// Insert les données du dataset dans la table destination
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="nomTableDestination"></param>
        /// <param name="columnsMapping">give the mapping between the source DataTable and the destination database Table</param>
        public void bulkcopyData(DataTable dt, DatabaseTable? destinationTableName =null, IDictionary<string, string> columnsMapping = null)
        {
            DatabaseTable destTable;
            if( destinationTableName == null)
            {
                destTable = new DatabaseTable(dt.TableName);
            }
            else
            {
                destTable =(DatabaseTable)destinationTableName;
            }

            // Create the SqlBulkCopy object. 
            // Note that the column positions in the source DataTable 
            // match the column positions in the destination table so 
            // there is no need to map columns.         
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)DestinationConnection.Connection))
            {
                bulkCopy.BulkCopyTimeout = 120;
                bulkCopy.BatchSize = 1500;

                DestinationConnection.Open();                
                                        bulkCopy.DestinationTableName = destTable.ToString();
                

                if (columnsMapping != null)
                {
                    foreach (string source in columnsMapping.Keys)
                    {
                        bulkCopy.ColumnMappings.Add(source, columnsMapping[source]);
                    }
                }
                else
                {
                    foreach (string columnsName in AutoMappingHelper.GetMapping(dt, DestinationConnection, destTable))
                    {
                        bulkCopy.ColumnMappings.Add(columnsName, columnsName);
                    }
                }

                bulkCopy.WriteToServer(dt);
            }
            DestinationConnection.Close();
            
        }

        /// <summary>
        /// Insert les données resultant de la requete SQL dans la table destination
        /// </summary>
        /// <param name="RequestSelect"></param>
        /// <param name="nomTableDestination">avec ou sans le nom du schema</param>
        public void bulkcopySourceRequest(string sourceSelectRequest, DatabaseTable DestinationTableName)
        {

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy((SqlConnection)DestinationConnection.Connection))
            {
                bulkCopy.BulkCopyTimeout = 120;
                bulkCopy.BatchSize = 1500;

                DestinationConnection.Open();

                bulkCopy.DestinationTableName = DestinationTableName.ToString();

                SqlCommand command = new SqlCommand(sourceSelectRequest, (SqlConnection)SourceConnection.Connection);
                SourceConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                bulkCopy.WriteToServer(reader);
            }
            SourceConnection.Close();
            DestinationConnection.Close();

        }


        /// <summary>
        /// BulkUpsert (update and insert) ou BulkMerge
        ///Remplit la table destination en mergeant avec le contenu du fichier csv fournie
        ///
        /// Execute un UPDATE et ensuite un INSERT sur la connection Target
        /// 
        /// Voir en MS SQL 2012 , pour utiliser la nouvelle instruction MERGE 
        ///
        /// </summary>
        /// <returns></returns>
        public void BulkUpsert(DatabaseTable targetTableName, DatabaseTable usingTableName, string dateFormat = "DMY", int connection_timeout = -1)
        {
            try
            {
                DestinationConnection.Open();

                string tmpUpdatedPKs = "#updatedPK";

                int nbUpdatedRows = DestinationConnection.Update(targetTableName, usingTableName, updated_PKs_Table: tmpUpdatedPKs, date_format: dateFormat);                 
                int nbInsertedNewRow = DestinationConnection.Insert(targetTableName, usingTableName, excluded_PKs_Table: tmpUpdatedPKs);

                InfoLogger.Info(String.Format("MERGE Table: {0}, using {1} : updatedRows: {2}, new Rows:{3} ", targetTableName, usingTableName, nbUpdatedRows, nbInsertedNewRow));
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                DestinationConnection.Close();
            }
        }

        /// <summary>
        ///Remplit la table destination en mergeant avec le contenu du fichier csv fournie
        ///
        /// Voir en MS SQL 2012 , pour utiliser la nouvelle instruction MERGE
        ///
        /// </summary>
        /// <returns></returns>
        public void BulkUpsertCsv(string CsvFileName, DatabaseTable targetTableName, string csvFileEncoding = "UTF8", string dateFormat = "DMY", int connectionTimeout = -1)
        {
            Encoding enc = null;
            switch (csvFileEncoding)
            {
                case "UTF8": enc = Encoding.UTF8; break;
                case "ASCII": enc = Encoding.ASCII; break;
                case "BigEndianUnicode": enc = Encoding.BigEndianUnicode; break;
                case "Unicode": enc = Encoding.Unicode; break;
                case "UTF32": enc = Encoding.UTF32; break;
                case "UTF7": enc = Encoding.UTF7; break;
                default: enc = Encoding.Default; break;
            };

            DatabaseTable usingTableName = new DatabaseTable(targetTableName.schema, "#" + targetTableName.table);

            List<string> columns = new List<string>();
            using (var reader = new CsvReader(new StreamReader(CsvFileName, enc), true, ';'))
            {
                try
                {
                    DestinationConnection.Open();

                    CsvDataAdapter adapter = new CsvDataAdapter(reader);
                    int nbTobeMergedRows = adapter.Fill(DestinationConnection, usingTableName);

                    InfoLogger.Info(String.Format("MERGE Table: {0}, using File {1} : nb rows: {2}", targetTableName, CsvFileName, nbTobeMergedRows));

                    this.BulkUpsert(targetTableName, usingTableName, dateFormat, connectionTimeout);

                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    DestinationConnection.Close();
                }
            }
        }


        /// <summary>
        ///Remplit la table destination en mergeant avec le contenu du fichier csv fournie
        ///
        /// Voir en MS SQL 2012 , pour utiliser la nouvelle instruction MERGE
        ///
        /// </summary>
        /// <returns></returns>
        public void BulkUpsertSourceRequest(string CsvFileName, string targetSchemaName, string targetTableName, string csvFileEncoding = "UTF8", string dateFormat = "DMY", int connectionTimeout = -1)
        {
            // TODO
            throw new NotImplementedException();
        }

    }

}
