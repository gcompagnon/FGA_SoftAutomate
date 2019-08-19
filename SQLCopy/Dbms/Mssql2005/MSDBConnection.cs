using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net.Repository.Hierarchy;
using log4net.Repository;
using log4net;
using log4net.Core;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using SQLCopy.Helpers;
using Helpers.DataReader;
using System.Data.Common;
using SQLCopy.Dbms;

namespace FGA.SQLCopy
{
    /// <summary>
    /// Implementation For MS SQL 2005
    /// </summary>
    public class MSSQL2005_DBConnection : DBConnectionDelegate
    {
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

            log4net.Appender.ColoredConsoleAppender appender = new log4net.Appender.ColoredConsoleAppender();
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
        /// <summary>
        ///Constructor could build a connection to a DB with:
        ///<list type="bullet">
        ///<listheader><term>ConnectionStringName</term><description>name of the connection string , available in the App.config file</description></listheader>
        ///<listheader><term>ConnectionString</term><description>A connection string with all the characteristics as Data Source=FX023179M\SQLExpress;Initial Catalog=FGA_JMOINS1;Persist Security Info=True;User ID=E2FGATP;Password=E2FGATP25</description></listheader>
        ///or 
        ///</list>
        /// <param name="connection">Name of one of the ConnectionStrings in the app.config file Or a connectionString that will be used directly</param>
        /// <param name="connectionStringName">Given name for the ConnectionString if not known</param>
        /// 
        /// 
        ///</summary>

        public MSSQL2005_DBConnection(String connection = null)
        {
            try
            {
                if (connection != null)
                {
                    //Get the DataBase from the AppConfig with the parameter name
                    ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[connection];
                    if (connectionString == null)
                        Connection = new SqlConnection(connection);
                    else
                        Connection = new SqlConnection(connectionString.ToString());

                    Connection.Open();
                    Connection.Close();
                }
                else
                    throw new ArgumentNullException("No given connection");

            }
            catch (SqlException e1)
            {
                ExceptionLogger.Fatal(e1);
                throw e1;
            }
            catch (InvalidOperationException e2)
            {
                ExceptionLogger.Fatal(e2);
                throw e2;
            }
            catch (Exception e3)
            {
                ExceptionLogger.Fatal(e3);
                throw e3;
            }
        }
        #endregion

        #region Execute Scripts
        /// <summary>
        /// prendre dans le Dataset , les dataTables où il y a une colonne "sql"
        /// Il n y a pas de retour
        /// </summary>
        /// <param name="scripts">une table avec au moins une colonne nommée "sql" et contenant sur chaque ligne des requetes à executer</param>
        /// <param name="RequestColumnName">le nom de la colonne par defaut : "sql" </param>
        /// <param name="useDBcmd">par defaut, ajoute une commande USE avant la requete </param>
        public override void ExecuteScripts(DataTable scripts, string RequestColumnName = "sql", bool useDBcmd = true)
        {
            if (scripts.Columns.Contains(RequestColumnName))
            {
                using (SqlCommand cmd = ((SqlConnection)Connection).CreateCommand())
                {
                    try
                    {
                        // nom de la base de destination
                        string dbNameDestination = Connection.Database.ToString();

                        Connection.Open();

                        foreach (DataRow row in scripts.Rows)
                        {
                            if (useDBcmd)
                                cmd.CommandText = "USE " + dbNameDestination + " " + row[RequestColumnName].ToString();
                            else
                                cmd.CommandText = row[RequestColumnName].ToString();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.Fatal(e);
                    }
                    finally
                    {
                        Connection.Close();
                    }
                }

            }
        }

        /// <summary>
        ///Remplit la dataset donnée en paramètre avec le résultat de la requete
        /// sur la Connection Destination 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="connection_timeout"></param>
        /// <returns></returns>
        public override DataSet Execute(String request, int connection_timeout = -1)
        {
            DataSet DS = new DataSet();
            InfoLogger.Debug("La requete utilisée est \n" + request);
            try
            {
                if (Connection != null)
                {
                    SqlDataAdapter DA = new SqlDataAdapter(request, (SqlConnection)Connection);
                    if (connection_timeout > 0)
                    {
                        DA.SelectCommand.CommandTimeout = connection_timeout;
                    }
                    int nbLignes = DA.Fill(DS);
                    InfoLogger.Debug("La requete retourne :" + nbLignes + " Lignes");
                }
                return DS;
            }
            catch (Exception e)
            {
                ExceptionLogger.Fatal("Impossible d executer la requete: " + request, e);
                throw e;
            }
        }

        /// <summary>
        /// Execute a prepare statement for performance reason, keep and reuse the SqlCommand object 
        /// </summary>
        /// <param name="command">by reference , an objet SqlCommand</param>
        /// <param name="parameters">Request parameters</param>
        /// <param name="connection_timeout"></param>
        public override void Execute(ref IDbCommand command, IDataParameter[] parameters = null, int connection_timeout = -1)
        {
            InfoLogger.Debug("La requete utilisée est \n" + command.CommandText);
            try
            {
                if (Connection != null)
                {
                    if (command.Connection == null)
                    {
                        command.Connection = (SqlConnection)Connection;
                    }
                    else
                    {
                        command.Parameters.Clear();
                    }

                    if (connection_timeout > 0)
                    {
                        command.CommandTimeout = connection_timeout;
                    }
                    if (parameters != null)
                    {
                        foreach (IDataParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    int nbLignes = command.ExecuteNonQuery();
                    InfoLogger.Debug("La requete affecte :" + nbLignes + " Lignes");
                }

            }
            catch (Exception e)
            {
                ExceptionLogger.Fatal("Impossible d executer la requete: " + command, e);
                throw e;
            }
        }

        /// <summary>
        /// Execute a sql command, with our without Parameters (@xxx)
        /// using a delegate for reading the results,
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parametersDelegate">Delegate defined with lambda expression, used to give a value to the command parameters</param>
        /// <param name="dataReaderDelegate">Delegate defined with lambda expression, gives the results row by row, and on column Field</param>
        /// <param name="commandType">Type of the SQL command: Text if SQL  StoredProcedure if commandText is the name of a stored procedure or TableDirect if the commandtext is the name of a table to get all row and all column </param>
        /// <param name="connection_timeout"></param>
        /// <param name="max_retry"></param>
        public void Execute(string commandText, Action<DbParameterCollectionDelegate> parametersDelegate, Action<DbDataReader> dataReaderDelegate, CommandType commandType = CommandType.Text, int connection_timeout = -1, int max_retry = 3)
        {
            DbCommand dbCommand = this.Connection.CreateCommand();
            dbCommand.CommandType = commandType;
            dbCommand.CommandText = commandText;

            if (connection_timeout > 0)
                dbCommand.CommandTimeout = connection_timeout;

            if (parametersDelegate != null)
                parametersDelegate(new DbParameterCollectionDelegate(dbCommand));

            this.Open();
            DbDataReader reader;

            int nbRetry = 0;
            while (true)
            {
                try
                {
                    reader = dbCommand.ExecuteReader();
                    break;
                }
                catch (Exception e)
                {

                    this.Reconnect();

                    if (nbRetry > max_retry)
                        throw e;
                }
                nbRetry++;
            }

            using (reader)
            {
                if (dataReaderDelegate != null)
                    while (reader.Read())
                        dataReaderDelegate(reader);
            }
        }


        #endregion


        public override bool isTableExist(DatabaseTable tableName)
        {
            if( tableName.schema == null)
                return isTableExist("dbo", tableName.table);
            return isTableExist(tableName.schema, tableName.table);
        }
        private bool isTableExist(string schemaName, string tableName)
        {
            String sqlRequest = "select s.name,t.name from {3}sys.tables as t left outer join {3}sys.schemas as s on s.schema_id = t.schema_id where s.name='{0}' and t.name {1}'{2}'";

            System.Diagnostics.Debug.Assert(Connection != null, "Connection must be setted");
            if (Connection.State != ConnectionState.Open)
            {
                this.Open();
            }
            SqlCommand s = new SqlCommand(
            String.Format(sqlRequest, schemaName, "=", tableName, ""), (SqlConnection)Connection
            );
            var obj = s.ExecuteScalar();
            if (obj != null)
                return true;
            // test if table is on tempdb
            s = new SqlCommand(
            String.Format(sqlRequest, schemaName, "like ", tableName + "%", "tempdb."), (SqlConnection)Connection
            );
            obj = s.ExecuteScalar();
            return obj != null;
        }
        /// <summary>
        /// Booleen si la base ne possède aucune tables
        /// </summary>
        /// <returns></returns>
        public override bool isDBEmpty()
        {
            bool empty = true;
            //------------------------------------
            //SMO Server object setup with SQLConnection.
            Server server2 = new Server(new ServerConnection((SqlConnection)Connection));
            string dbName2 = Connection.Database.ToString();

            //Set Database to the newly created database
            Database db2 = server2.Databases[dbName2];
            try
            {
                if (db2.Tables.Count != 0)
                    empty = false;
            }
            catch (NullReferenceException)
            {
                InfoLogger.Fatal("La base de destination n'existe pas!");
                empty = false;
            }

            //------------------------------------
            Connection.Close();

            return empty;
        }

        /// <summary>
        /// Insertion des elements de la table With dans la table Target
        ///  la liste des PK à utiliser est donné en parametre
        /// </summary>                   
        public override int Insert(DatabaseTable targetTableName, DatabaseTable withTableName, string excluded_PKs_Table = null, string included_PKs_Table = null, string date_format = "DMY", int connection_timeout = -1)
        {
            // ASSERTION
            if (!isTableExist(targetTableName))
            {
                ExceptionLogger.Fatal("Le nom de la table est inconnu :" + targetTableName);
                return 0;
            }
            if (!isTableExist(withTableName))
            {
                ExceptionLogger.Fatal("Le nom de la table est inconnu :" + withTableName);
                return 0;
            }

            // PRE TRAITEMENT : reperer les colonnes existantes ou non , et celles qui sont Primary Keys
            Dictionary<string, ColumnSpec> updated_table_columnsSpec = this.GetColumnsSpec(targetTableName);
            IEnumerable<string> with_table_columnsName;
            with_table_columnsName = GetColumnsName(withTableName);
            List<ColumnSpec> _columns = new List<ColumnSpec>(with_table_columnsName.Count<string>());
            List<string> _primaryKeys = new List<string>(with_table_columnsName.Count<string>());

            // PRE TRAITEMENT : reperer les colonnes existantes ou non , et celles qui sont Primary Keys
            foreach (String colName in with_table_columnsName)
            {
                // verifier que la colonne existe pour la table à updater

                if (updated_table_columnsSpec.Keys.Contains<string>(colName))
                {
                    ColumnSpec col = updated_table_columnsSpec[colName];
                    _columns.Add(col);
                    if (col.isPK)
                    {
                        _primaryKeys.Add(col.Column);
                    }
                }
            }
            // test : l ensemble des PKs sont presentes
            foreach (ColumnSpec col in updated_table_columnsSpec.Values)
            {
                if (col.isPK)
                {
                    if (!_primaryKeys.Contains(col.Column))
                    {
                        ExceptionLogger.Fatal("la colonne  :" + col.Column + " ne fait pas parti de la table " + withTableName);
                        throw new Exception("Insert impossible because the " + col.Column + " of " + targetTableName + " is not present in " + withTableName);
                    }
                }
            }

            //Construction de la requete            
            // modele de requete pour faire un update
            StringBuilder _1insertTable = null;
            StringBuilder _2whereClauseTable = null;
            StringBuilder _3columns = null;
            String _4clauseIsNull = null;

            foreach (ColumnSpec i in _columns)
            {
                if (_1insertTable == null)
                {
                    _1insertTable = new StringBuilder("w." + i.Column);
                }
                else
                {
                    _1insertTable.Append(", w.").Append(i.Column);
                }
                if (_3columns == null)
                {
                    _3columns = new StringBuilder(i.Column);
                }
                else
                {
                    _3columns.Append(", ").Append(i.Column);
                }

                if (i.isPK)
                {
                    if (_2whereClauseTable == null)
                    {
                        _2whereClauseTable = new StringBuilder("pk." + i.Column + " = w." + i.Column);
                        _4clauseIsNull = "pk." + i.Column + " is null ";
                    }
                    else
                    {
                        _2whereClauseTable.Append(" AND pk.").Append(i.Column).Append(" = w.").Append(i.Column);
                    }

                }
            }

            string _insertTable = String.Format("INSERT INTO  [{0}].[{1}]  ({2}) SELECT {3} FROM  [{4}].[{5}] as w ",
                targetTableName.schema, targetTableName.table,
                _3columns, _1insertTable.ToString(),
                withTableName.schema, withTableName.table );

            string _whereClauseTable;
            if (excluded_PKs_Table != null)
            {
                _whereClauseTable = " LEFT JOIN " + excluded_PKs_Table + " AS pk ON " + _2whereClauseTable + " WHERE " + _4clauseIsNull;
            }
            else if (included_PKs_Table != null)
            {
                _whereClauseTable = " INNER JOIN " + excluded_PKs_Table + " AS pk ON " + _2whereClauseTable;

            }
            else
            {
                _whereClauseTable = "";
            }

            int nbLines = 0;
            try
            {
                SqlConnection c = (SqlConnection)Connection;
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandText = _insertTable + _whereClauseTable;
                nbLines = cmd.ExecuteNonQuery();
            }
            catch (SqlException e1)
            {
                ExceptionLogger.Fatal("SQL Request failed: " + _insertTable + _whereClauseTable, e1);
                throw e1;
            }
            return nbLines;
        }

        /*
        /// <summary>
        /// Insertion des elements de la table With dans la table Target
        ///  la liste des PK à utiliser est donné en parametre
        /// </summary>
        /// <param name="targetSchemaName"></param>
        /// <param name="targetTableName"></param>
        /// <param name="withSchemaName"></param>
        /// <param name="withTableName"></param>
        /// <param name="excluded_PKs_Table"></param>
        /// <param name="included_PKs_Table"></param>
        /// <param name="date_format"></param>
        /// <param name="connection_timeout"></param>
        /// <returns></returns>
        /// 
        */
        public override int BulkCopydataCsvFile(string sourceCsvFile, DatabaseTable targetTableName, string excluded_PKs_Table = null, string included_PKs_Table = null, string csvFileEncoding = "UTF8", string date_format = "DMY", int connection_timeout = -1)
        {
            Encoding enc = csvFileEncoding.GetEncoding();

            List<string> columns = new List<string>();
            Stream s = new FileStream(sourceCsvFile, FileMode.Open);

            using (var reader = new FileDataReader(s, null, enc))
            {
                try
                {
                    //TODO 
                    Connection.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    Connection.Close();
                }
            }
            return 0;

        }



        /// <summary>
        /// Delete  table datas  avec les valeurs de la table With
        /// avec la liste des PK qui ont été mis a jour dans une table temporaire
        /// </summary>        
        public override int Delete(DatabaseTable targetTableName, DataTable withPKs,
            string date_format = "DMY", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS", int connection_timeout = -1)
        {
            SqlTransaction transaction = null;
            int _deletedRows = 0;
            // ASSERTION
            if (!isTableExist(targetTableName))
            {
                ExceptionLogger.Fatal("Le nom de la table est inconnu :" + targetTableName);
                return 0;
            }

            try
            {

                Dictionary<string, ColumnSpec> _columns = this.GetColumnsSpec(targetTableName);


                StringBuilder sqlScript = new StringBuilder();

                // build of the SQL request
                List<String> columnsName = new List<string>(withPKs.Columns.Count);
                foreach (DataColumn c in withPKs.Columns)
                {
                    if (_columns.Keys.Contains<string>(c.ColumnName, StringComparer.CurrentCultureIgnoreCase))
                    {
                        columnsName.Add(c.ColumnName);
                        if (sqlScript.Length == 0)
                        {
                            sqlScript.Append("DELETE FROM [" + targetTableName.schema + "].[" + targetTableName.table + "] WHERE " + c.ColumnName + "= @" + c.ColumnName);
                        }
                        else
                        {
                            sqlScript.Append(" AND " + c.ColumnName + "= @" + c.ColumnName);
                        }
                    }
                }

                SqlCommand cmd = ((SqlConnection)Connection).CreateCommand();
                // Start a local transaction.
                transaction = ((SqlConnection)this.Connection).BeginTransaction("Delete");

                cmd.Connection = (SqlConnection)this.Connection;
                cmd.Transaction = transaction;
                cmd.CommandText = sqlScript.ToString();
                if (connection_timeout > 0)
                    cmd.CommandTimeout = connection_timeout;

                // execute the sql request with parameters
                foreach (DataRow row in withPKs.Rows)
                {
                    foreach (string cn in columnsName)
                    {
                        cmd.Parameters.AddWithValue("@" + cn, row[cn]);
                    }
                    _deletedRows += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
            }
            catch (SqlException ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                ExceptionLogger.Error(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();
                ExceptionLogger.Error(ex);
                throw ex;
            }
            finally
            {
                transaction.Commit();
            }
            return _deletedRows;

        }

        public override int DeleteCsvFile(string CsvFileName, DatabaseTable targetTableName, string csvFileEncoding = "UTF8", string dateFormat = "DMY", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS", int connectionTimeout = -1)
        {
            Encoding enc = csvFileEncoding.GetEncoding();

            string usingTableName = "#" + targetTableName.table;
            string usingSchemaName = targetTableName.schema;

            List<string> columns = new List<string>();
            DataSet ds = new DataSet();
            using (var reader = new CsvReader(new StreamReader(CsvFileName, enc), true, ';'))
            {
                    Connection.Open();

                    CsvDataAdapter adapter = new CsvDataAdapter(reader);
                    adapter.Fill(ds,"TO_DELETE");
                    DataTable dt = ds.Tables["TO_DELETE"];
                    return this.Delete(targetTableName, dt, dateFormat, tableCollation, connectionTimeout);
            }
        }


        /// <summary>
        /// MAJ de la table Updated avec les valeurs de la table With
        /// avec la liste des PK qui ont été mis a jour dans une table temporaire
        /// </summary>        
        public override int Update(DatabaseTable targetTableName, DatabaseTable withTableName, string updated_PKs_Table = "@updatedPK", string date_format = "DMY", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS", int connection_timeout = -1)
        {

            // ASSERTION
            if (!isTableExist(targetTableName))
            {
                ExceptionLogger.Fatal("Le nom de la table est inconnu :" + targetTableName);
                return 0;
            }
            if (!isTableExist(withTableName))
            {
                ExceptionLogger.Fatal("Le nom de la table est inconnu :" + withTableName);
                return 0;
            }
            // PRE TRAITEMENT : reperer les colonnes existantes ou non , et celles qui sont Primary Keys
            Dictionary<string, ColumnSpec> updated_table_columnsSpec = this.GetColumnsSpec(targetTableName);
            IEnumerable<string> with_table_columnsName;
            with_table_columnsName = GetColumnsName(withTableName);
            List<ColumnSpec> _columns = new List<ColumnSpec>(with_table_columnsName.Count<string>());
            List<string> _primaryKeys = new List<string>(with_table_columnsName.Count<string>());

            // PRE TRAITEMENT : reperer les colonnes existantes ou non , et celles qui sont Primary Keys
            foreach (String colName in with_table_columnsName)
            {
                // verifier que la colonne existe pour la table à updater

                if (updated_table_columnsSpec.Keys.Contains<string>(colName))
                {
                    ColumnSpec col = updated_table_columnsSpec[colName];
                    _columns.Add(col);
                    if (col.isPK)
                    {
                        _primaryKeys.Add(col.Column);
                    }
                }
            }

            // test : l ensemble des PKs sont presentes
            foreach (ColumnSpec col in updated_table_columnsSpec.Values)
            {
                if (col.isPK)
                {
                    if (!_primaryKeys.Contains(col.Column))
                    {
                        ExceptionLogger.Fatal("la colonne  :" + col.Column + " ne fait pas parti de la table " + withTableName);
                        throw new Exception("Update Impossible because the " + col.Column + " of " + targetTableName + " is not present in " + withTableName);
                    }
                }
            }

            //Construction de la requete            
            // rmodele de requete pour faire un update
            StringBuilder _1createUpdatedPKTable = null;
            StringBuilder _2setClauseTable = null;
            StringBuilder _3outputCLause = null;
            StringBuilder _4joinClauseTable = null;

            foreach (ColumnSpec i in _columns)
            {
                if (i.isPK)
                {
                    if (_4joinClauseTable == null)
                    {
                        _4joinClauseTable = new StringBuilder("w.").Append(i.Column).Append(" = u.").Append(i.Column);
                    }
                    else
                    {
                        _4joinClauseTable.Append(" AND w.").Append(i.Column).Append(" = u.").Append(i.Column);
                    }

                    if (_1createUpdatedPKTable == null)
                    {
                        _1createUpdatedPKTable = new StringBuilder(i.Column).Append(" ").Append(i.SQLType).Append((i.isSQLCharType ? " " + tableCollation : ""));
                    }
                    else
                    {
                        _1createUpdatedPKTable.Append(", ").Append(i.Column).Append(" ").Append(i.SQLType).Append((i.isSQLCharType ? " " + tableCollation : ""));
                    }

                    if (_3outputCLause == null)
                    {
                        _3outputCLause = new StringBuilder("inserted.").Append(i.Column);
                    }
                    else
                    {
                        _3outputCLause.Append(", inserted.").Append(i.Column);
                    }

                }
                else
                {
                    if (_2setClauseTable == null)
                        _2setClauseTable = new StringBuilder(i.Column).Append(" =  w.").Append(i.Column);
                    else
                        _2setClauseTable.Append(", ").Append(i.Column).Append(" = w.").Append(i.Column);
                }
            }

            string _setClauseTable = String.Format("UPDATE  [{0}].[{1}] SET {2} OUTPUT {3} INTO {4} FROM [{5}].[{6}] as u INNER JOIN [{7}].[{8}] as w ON {9}",
                    targetTableName.schema,//0
                    targetTableName.table,//1
                    _2setClauseTable.ToString(),//2
                    _3outputCLause.ToString(),//3
                    updated_PKs_Table,//4
                    targetTableName.schema,//5
                    targetTableName.table,//6
                    withTableName.schema,//7
                    withTableName.table,//8
                    _4joinClauseTable.ToString() //9
                    );

            string _createUpdatedPKTable;
            // La table contenant les PKs qui ont ete mis à jour, est une variable
            if (updated_PKs_Table[1] == '@')
            {
                _createUpdatedPKTable = "SET DATEFORMAT " + date_format + "; DECLARE " + updated_PKs_Table + " TABLE (" + _1createUpdatedPKTable.ToString() + "); ";

            }
            else
            {
                _createUpdatedPKTable = "SET DATEFORMAT " + date_format + "; CREATE TABLE " + updated_PKs_Table + " (" + _1createUpdatedPKTable.ToString() + "); ";
            }

            int nbLines = 0;
            try
            {

                SqlConnection c = (SqlConnection)Connection;
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandText = _createUpdatedPKTable + _setClauseTable;
                nbLines = cmd.ExecuteNonQuery();
            }
            catch (SqlException e1)
            {
                ExceptionLogger.Fatal("SQL Request failed: " + _createUpdatedPKTable + _setClauseTable, e1);
                throw e1;
            }
            return nbLines;
        }

        /// <summary>
        /// Copie les métata data (schema, tables, PK et FK, ProcStock d'une BDD dans un tableau à 4 entrées: 
        /// <list type="dd">TABLE</list>
        /// <list type="dd">SCHEMA</list>
        /// <list type="dd">FK</list>
        /// <list type="dd">PROCSTOC</list>
        /// </summary>
        /// <param name="ListMode">rien ou vide pour ne pas tenir compte de la liste , toutes les tables de la source sont concernées / "exclude" pour donner une liste de tables à exclure / "include" pour donner le nom des tables à inclure</param>
        /// <param name="ListTableName">la liste des noms des tables à considerer dns un mode exclude ou include </param>
        public override DataSet GetMetatDataDBScripts(ListeMode mode = ListeMode.Aucune, List<DatabaseTable> ListTableName = null)
        {
            // nom de la base de source
            string dbName = Connection.Database.ToString();

            // structure de retour contenant les scripts
            DataSet ds = new DataSet("SCRIPTS");

            DataTable tableDT = new DataTable("TABLE");
            tableDT.Columns.Add("sql");
            ds.Tables.Add(tableDT);

            DataTable fkDT = new DataTable("FK");
            fkDT.Columns.Add("sql");
            ds.Tables.Add(fkDT);

            DataTable procstocDT = new DataTable("PROCSTOC");
            procstocDT.Columns.Add("sql");
            ds.Tables.Add(procstocDT);

            DataTable schemaDT = new DataTable("SCHEMA");
            schemaDT.Columns.Add("sql");
            ds.Tables.Add(schemaDT);

            //SMO Server object setup with SQLConnection.
            Server server = new Server(new ServerConnection((SqlConnection)Connection));

            //Set Database to the database
            Database db = server.Databases[dbName];
            //----------------------------------------------------------------------------------------------------------------

            /*Option pour la creation des tables
             * inclus les cles primaires, les contraintes nonclustered et
             * if not exist pour ne pas creer une table qui existe deja*/
            ScriptingOptions tablesScriptOptions = new ScriptingOptions();
            tablesScriptOptions.DriPrimaryKey = true;
            tablesScriptOptions.IncludeIfNotExists = true;
            tablesScriptOptions.DriNonClustered = true;

            /*Option pour les foreign key de chaque table, 
             * préposé de leur schéma*/
            ScriptingOptions fkScriptOptions = new ScriptingOptions();
            fkScriptOptions.SchemaQualifyForeignKeysReferences = true;
            fkScriptOptions.DriForeignKeys = true;

            InfoLogger.Debug("Obtention metadonnees tables et clefs");

            StringCollection schemaCollection = new StringCollection();

            foreach (Table myTable in db.Tables)
            {
                // si il y a une liste
                if (mode.Equals(ListeMode.Include))
                {
                    if (!ListTableName.Contains(myTable.Name,myTable.Schema))
                        continue;
                }
                else if (mode.Equals(ListeMode.Exclude))
                {
                    if (ListTableName.Contains(myTable.Name,myTable.Schema))
                        continue;
                }

                //Si c'est un nouveau schéma on retient son nom
                if (!schemaCollection.Contains("[" + myTable.Schema + "]"))
                    schemaCollection.Add("[" + myTable.Schema + "]");

                //On ajoute le script de la table à tableCol
                StringCollection tableScripts = myTable.Script(tablesScriptOptions);
                // maj de la Dataset
                DataRow dr = tableDT.NewRow();
                foreach (string scriptLine in tableScripts)
                {
                    dr["sql"] += scriptLine + System.Environment.NewLine;
                }
                tableDT.Rows.Add(dr);


                //On ajoute le script des foreign keys à foreignKeyCol
                ForeignKeyCollection fk = myTable.ForeignKeys;
                foreach (ForeignKey myFk in fk)
                {
                    StringCollection stmp = myFk.Script(fkScriptOptions);
                    // maj de la Dataset
                    DataRow fkDr = fkDT.NewRow();
                    foreach (string scriptLine in stmp)
                    {
                        fkDr["sql"] += scriptLine + System.Environment.NewLine;
                    }
                    fkDT.Rows.Add(fkDr);

                }
            }
            //Enleve le schéma par défault
            schemaCollection.Remove("[dbo]");


            InfoLogger.Debug("Obtention des Procédures stockées");
            ScriptingOptions scrOptProcStoc = new ScriptingOptions() { NoCommandTerminator = false, ScriptBatchTerminator = true, IncludeIfNotExists = true };
            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                if (!sp.Schema.Equals("sys") && !sp.IsSystemObject)
                {
                    StringCollection scsp = sp.Script(scrOptProcStoc);
                    // maj de la Dataset
                    DataRow pcDr = procstocDT.NewRow();
                    foreach (string scriptLine in scsp)
                    {
                        pcDr["sql"] += scriptLine + System.Environment.NewLine;
                    }
                    procstocDT.Rows.Add(pcDr);
                }
            }

            InfoLogger.Debug("Obtention Metadonnees schemas");
            SchemaCollection sc = db.Schemas;
            ScriptingOptions scrOpt_Schema = new ScriptingOptions() { IncludeIfNotExists = true };

            foreach (Schema schem in sc)
            {
                if (schemaCollection.Contains(schem.ToString()))
                {
                    StringCollection schemaScripts = schem.Script(scrOpt_Schema);
                    // maj de la Dataset
                    DataRow schemaDr = schemaDT.NewRow();
                    foreach (string scriptLine in schemaScripts)
                    {
                        schemaDr["sql"] += scriptLine + System.Environment.NewLine;
                    }
                    schemaDT.Rows.Add(schemaDr);

                }
            }
            Connection.Close();

            return ds;

        }

        /// <summary>
        /// Open connection if necessary
        /// </summary>
        public override void Open()
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        /// <summary>
        /// Close if connection is open
        /// </summary>
        public override void Close()
        {
            if (Connection.State != ConnectionState.Closed)
            {

                Connection.Close();
            }
        }

        /// <summary>
        /// Close and Open if connection is open
        /// </summary>
        public override void Reconnect()
        {

            if (Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
                Connection.Open();
            }
        }



        /// <summary>
        /// List of columns of a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaName">by default dbo, could be null, and it will return the first available</param>
        /// <returns>a enumerable of string</returns>
        public override IEnumerable<string> GetColumnsName(DatabaseTable tableName)
        {
            String sqlRequest = "SELECT c.name FROM {3}sys.tables t JOIN {3}sys.schemas s on t.schema_id = s.schema_id JOIN {3}sys.columns c on t.object_id = c.object_id WHERE t.name {1} '{2}'and s.name = '{0}'";
            String sqlRequest_wo_schema = "SELECT c.name FROM {2}sys.tables t JOIN {2}sys.schemas s on t.schema_id = s.schema_id JOIN {2}sys.columns c on t.object_id = c.object_id WHERE t.name {0} '{1}'";

            System.Diagnostics.Debug.Assert(Connection != null, "Connection must be set");
            this.Open();


            SqlCommand s = ((SqlConnection)Connection).CreateCommand();
            if (tableName.schema == null)
                s.CommandText = String.Format(sqlRequest_wo_schema, "=", tableName.table, "");
            else
                s.CommandText = String.Format(sqlRequest, tableName.schema, "=", tableName.table, "");

            // An alternative : sp_columns
                //s.CommandText = "sp_Columns";
                //s.CommandType = CommandType.StoredProcedure;
                //s.Parameters.Add("@table_name", SqlDbType.NVarChar, 384).Value = tableName;

            // look if it is a temp table
            if (s.ExecuteScalar() == null)
            {
                if (tableName.schema == null)
                    s.CommandText = String.Format(sqlRequest_wo_schema, " like ", tableName.table  + "%", "tempdb.");
                else
                    s.CommandText = String.Format(sqlRequest, tableName.schema, " like ", tableName.table + "%", "tempdb.");
            }
            List<string> returns = new List<string>();
            using (SqlDataReader reader = s.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return reader.GetString(0);
                }
            }
        }


        public override Dictionary<string, ColumnSpec> GetColumnsSpec(DatabaseTable tableName)
        {

            String sqlRequest = "SELECT c.name 'ColumnName', t.Name 'DataType',c.max_length 'Max Length',c.precision ,c.scale ,c.is_nullable,ISNULL(i.is_primary_key, 0) 'Primary Key' FROM {3}sys.tables tab JOIN {3}sys.schemas s on tab.schema_id = s.schema_id JOIN {3}sys.columns c on tab.object_id = c.object_id INNER JOIN {3}sys.types t ON c.system_type_id = t.system_type_id LEFT OUTER JOIN {3}sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN {3}sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE tab.name {1} '{2}'and s.name = '{0}'";

            System.Diagnostics.Debug.Assert(Connection != null, "Connection must be set");
            this.Open();


            SqlCommand s = ((SqlConnection)Connection).CreateCommand();
            s.CommandText = String.Format(sqlRequest, tableName.schema, "=", tableName.table, "");

            // look if it is a temp table
            if (s.ExecuteScalar() == null)
            {
                s.CommandText = String.Format(sqlRequest, tableName.schema, " like ", tableName.table + "%", "tempdb.");
            }
            Dictionary<string, ColumnSpec> returns = new Dictionary<string, ColumnSpec>(StringComparer.CurrentCultureIgnoreCase);
            using (SqlDataReader reader = s.ExecuteReader())
            {
                while (reader.Read())
                {
                    returns.Add(reader.GetString(0), new ColumnSpec(reader.GetString(0), reader.GetString(1), reader.GetBoolean(6), reader.GetBoolean(5), reader.GetInt16(2)));
                }
            }
            return returns;
        }
    }
}
