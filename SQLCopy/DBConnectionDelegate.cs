using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using log4net.Repository.Hierarchy;
using log4net.Repository;
using log4net;
using log4net.Core;
using System.Configuration;
using System.Collections.Specialized;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.Common;
using SQLCopy.Dbms;

namespace FGA.SQLCopy
{

    public abstract class DBConnectionDelegate : IDisposable
    {

        #region accesseurs
        public DbConnection Connection
        {
            get;

            protected set;
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
        public abstract void ExecuteScripts(DataTable scripts, string RequestColumnName = "sql", bool useDBcmd = true);
        /// <summary>
        ///Remplit la dataset donnée en paramètre avec le résultat de la requete
        /// sur la Connection Destination 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="connection_timeout"></param>
        /// <returns></returns>
        public abstract DataSet Execute(String request, int connection_timeout = -1);

        /// <summary>
        /// Execute a prepare statement for performance reason, keep and reuse the DbCommand object 
        /// </summary>
        /// <param name="command">by reference , an objet DbCommand</param>
        /// <param name="parameters">Request parameters</param>
        /// <param name="connection_timeout"></param>
        public abstract void Execute(ref IDbCommand command, IDataParameter[] parameters = null, int connection_timeout = -1);
        #endregion

        /// <summary>
        /// Booleen si la base ne possède aucune tables
        /// </summary>
        /// <returns></returns>
        public abstract bool isTableExist(DatabaseTable tableName);
        /// <summary>
        /// Booleen si la base ne possède aucune tables
        /// </summary>
        /// <returns></returns>
        public abstract bool isDBEmpty();
        /// <summary>
        /// Insertion des elements de la table With dans la table Target
        ///  la liste des PK à utiliser est donné en parametre
        /// </summary>        
        public abstract int Insert(DatabaseTable targetTableName, DatabaseTable withTableName, string excluded_PKs_Table = null, string included_PKs_Table = null, string date_format = "DMY", int connection_timeout = -1);

        public abstract int BulkCopydataCsvFile(string sourceCsvFile, DatabaseTable targetTableName, string excluded_PKs_Table = null, string included_PKs_Table = null, string csvFileEncoding = "UTF8", string date_format = "DMY", int connection_timeout = -1);
        /// <summary>
        /// Delete  table datas  avec les valeurs de la table With
        /// avec la liste des PK qui ont été mis a jour dans une table temporaire
        /// </summary>        
        public abstract int Delete(DatabaseTable targetTableName, DataTable dt,
            string date_format = "DMY", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS", int connection_timeout = -1);

        public abstract int DeleteCsvFile(string CsvFileName, DatabaseTable targetTableName, string csvFileEncoding = "UTF8", string dateFormat = "DMY", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS", int connectionTimeout = -1);


        /// <summary>
        /// MAJ de la table Updated avec les valeurs de la table With
        /// avec la liste des PK qui ont été mis a jour dans une table temporaire
        /// </summary>        
        public abstract int Update(DatabaseTable targetTableName, DatabaseTable withTableName, string updated_PKs_Table = "@updatedPK", string date_format = "DMY", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS", int connection_timeout = -1);
        /// <summary>
        /// Copie les métata data (schema, tables, PK et FK, ProcStock d'une BDD dans un tableau à 4 entrées: 
        /// <list type="dd">TABLE</list>
        /// <list type="dd">SCHEMA</list>
        /// <list type="dd">FK</list>
        /// <list type="dd">PROCSTOC</list>
        /// </summary>
        /// <param name="ListMode">rien ou vide pour ne pas tenir compte de la liste , toutes les tables de la source sont concernées / "exclude" pour donner une liste de tables à exclure / "include" pour donner le nom des tables à inclure</param>
        /// <param name="ListTableName">la liste des noms des tables à considerer dns un mode exclude ou include </param>
        public abstract DataSet GetMetatDataDBScripts(ListeMode mode = ListeMode.Aucune, List<DatabaseTable> ListTableName = null);

        /// <summary>
        /// Open connection if necessary
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Close if connection is open
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Close and Open if connection is open
        /// </summary>
        public abstract void Reconnect();

        /// <summary>
        /// List of columns of a table
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaName">by default dbo</param>
        /// <returns>a enumerable of string</returns>
        public abstract IEnumerable<string> GetColumnsName(DatabaseTable tableName);

        public abstract Dictionary<string, ColumnSpec> GetColumnsSpec(DatabaseTable tableName);


        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Close();
                Connection.Dispose();
            }
            
        }
    }

}
