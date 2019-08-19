using FGA.SQLCopy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data.Common;
using SQLCopy.Helpers.DataReader;
using System.Xml;
using SQLCopy.Dbms;
using log4net; 

namespace SQLCopy_Test
{       
    /// <summary>
    ///Classe de test pour MSSQL2005_DBConnectionTest, destinée à contenir tous
    ///les tests unitaires MSSQL2005_DBConnectionTest
    ///</summary>
    [TestClass()]
    public class MSSQL2005_DBConnectionTest
    {

        public const string DATABASE_TEST_1 = "AdventureWorks2008R2";
        public const string DATABASE_TEST_2 = "Data Source=FX023179M\\SQLExpress;Initial Catalog=AdventureWorks2008R2;Integrated Security=True;Connection Timeout=60";

        public const string DATABASE_TEST_1_DATASOURCE = "FX023179M\\SQLExpress";
        public const string DATABASE_TEST_1_DB_NAME = "AdventureWorks2008R2";


        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        // 
        //Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilisez ClassCleanup pour exécuter du code après que tous les tests ont été exécutés dans une classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Constructor could build a connection to a DB with:
        ///<list type="bullet">
        ///<listheader><term>ConnectionStringName</term><description>name of the connection string , available in the App.config file</description></listheader>
        ///<listheader><term>ConnectionString</term><description>A connection string with all the characteristics as Data Source=FX023179M\SQLExpress;Initial Catalog=FGA_JMOINS1;Persist Security Info=True;User ID=E2FGATP;Password=E2FGATP25</description></listheader>
        ///or 
        ///</list>
        /// 
        /// 
        ///</summary>
        [TestMethod()]
        public void MSSQL2005_DBConnectionConstructorTest()
        {
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(DATABASE_TEST_1 );
            Assert.AreEqual(target.Connection.DataSource.ToString(), DATABASE_TEST_1_DATASOURCE);
            Assert.AreEqual(target.Connection.Database.ToString(), DATABASE_TEST_1_DB_NAME);

            target = new MSSQL2005_DBConnection(DATABASE_TEST_2);
            Assert.AreEqual(target.Connection.DataSource.ToString(), DATABASE_TEST_1_DATASOURCE);
            Assert.AreEqual(target.Connection.Database.ToString(), DATABASE_TEST_1_DB_NAME );
        }

        /// <summary>
        ///Test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {            
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(DATABASE_TEST_1);
            target.Close();
            Assert.AreEqual(target.Connection.State, ConnectionState.Closed);
        }


       


        /// <summary>
        ///Test pour Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(DATABASE_TEST_1);
            string targetSchemaName = "dbo"; 
            string targetTableName = "S2_DEFAUT";
            DataTable dt = new DataTable();

            int expected = 1; 
            int actual;
            actual = target.Delete(new DatabaseTable(targetSchemaName, targetTableName),dt);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour DeleteUsingCsv
        ///</summary>
        [TestMethod()]
        public void DeleteUsingCsvTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string CsvFileName = string.Empty; // TODO: initialisez à une valeur appropriée
            string targetSchemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            string targetTableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string csvFileEncoding = string.Empty; // TODO: initialisez à une valeur appropriée
            string dateFormat = string.Empty; // TODO: initialisez à une valeur appropriée
            int expected = 0; // TODO: initialisez à une valeur appropriée
            int actual;
            actual = target.DeleteCsvFile (CsvFileName,new DatabaseTable(targetSchemaName, targetTableName));
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            IDbCommand command = null; // TODO: initialisez à une valeur appropriée
            SqlCommand commandExpected = null; // TODO: initialisez à une valeur appropriée
            IDataParameter[] parameters = null; // TODO: initialisez à une valeur appropriée
            int connection_timeout = 0; // TODO: initialisez à une valeur appropriée

            target.Execute(ref command, parameters, connection_timeout);
            Assert.AreEqual(commandExpected, command);
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }

        /// <summary>
        ///Test pour Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest1()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string request = string.Empty; // TODO: initialisez à une valeur appropriée
            int connection_timeout = 0; // TODO: initialisez à une valeur appropriée
            DataSet expected = null; // TODO: initialisez à une valeur appropriée
            DataSet actual;
            actual = target.Execute(request, connection_timeout);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour ExecuteScripts
        ///</summary>
        [TestMethod()]
        public void ExecuteScriptsTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            DataTable scripts = null; // TODO: initialisez à une valeur appropriée
            string RequestColumnName = string.Empty; // TODO: initialisez à une valeur appropriée
            bool useDBcmd = false; // TODO: initialisez à une valeur appropriée
            target.ExecuteScripts(scripts, RequestColumnName, useDBcmd);
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }

        /// <summary>
        ///Test pour Execute
        ///</summary>+
        [TestMethod()]
        public void ExecuteTest2()
        {
            string connectionStringName = DATABASE_TEST_1;
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string request = "select top 100 * from PTF_Proxy where Date = @Date";
            DataSet expected = null; // TODO: initialisez à une valeur appropriée
            target.Execute(request, parameters   =>
            {
                parameters.Add("Date", "25/09/2013");                
            }, reader => {
                
                Console.WriteLine("[{0:HH:mm:ss.fff}] code proxy{1} Libelle {2} (  <titre: {3}> Libelle {4} poids: {5:f}",
                    DateTime.Now  ,
                    reader.Field<string>("Code_proxy"),
                    reader.Field<string>("Libelle_Proxy"),
                    reader.Field<string>("code_titre"),
                    reader.Field<string>("Libelle_titre"),
                    reader.Field<float>("Poids_VB"));
            });
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour GetColumnsName
        ///</summary>
        [TestMethod()]
        public void GetColumnsNameTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string tableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string schemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            IEnumerable<string> expected = null; // TODO: initialisez à une valeur appropriée
            IEnumerable<string> actual;
            actual = target.GetColumnsName(new DatabaseTable(schemaName,tableName));
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour GetColumnsSpec
        ///</summary>
        [TestMethod()]
        public void GetColumnsSpecTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string tableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string schemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            Dictionary<string, ColumnSpec> expected = null; // TODO: initialisez à une valeur appropriée
            Dictionary<string, ColumnSpec> actual;
            actual = target.GetColumnsSpec(new DatabaseTable(tableName, schemaName));
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour GetMetatDataDBScripts
        ///</summary>
        [TestMethod()]
        public void GetMetatDataDBScriptsTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            ListeMode mode = new ListeMode(); // TODO: initialisez à une valeur appropriée
            List<DatabaseTable> ListTableName = null; // TODO: initialisez à une valeur appropriée
            DataSet expected = null; // TODO: initialisez à une valeur appropriée
            DataSet actual;
            actual = target.GetMetatDataDBScripts(mode, ListTableName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour Insert
        ///</summary>
        [TestMethod()]
        public void InsertTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string targetSchemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            string targetTableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string withSchemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            string withTableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string excluded_PKs_Table = string.Empty; // TODO: initialisez à une valeur appropriée
            string included_PKs_Table = string.Empty; // TODO: initialisez à une valeur appropriée
            string date_format = string.Empty; // TODO: initialisez à une valeur appropriée
            int connection_timeout = 0; // TODO: initialisez à une valeur appropriée
            int expected = 0; // TODO: initialisez à une valeur appropriée
            int actual;
            actual = target.Insert(new DatabaseTable(targetSchemaName, targetTableName), new DatabaseTable(withSchemaName, withTableName), excluded_PKs_Table, included_PKs_Table, date_format, connection_timeout);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour Open
        ///</summary>
        [TestMethod()]
        public void OpenTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            target.Open();
            Assert.Inconclusive("Une méthode qui ne retourne pas une valeur ne peut pas être vérifiée.");
        }

        /// <summary>
        ///Test pour Update
        ///</summary>
        [TestMethod()]
        public void UpdateTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string targetSchemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            string targetTableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string withSchemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            string withTableName = string.Empty; // TODO: initialisez à une valeur appropriée
            string updated_PKs_Table = string.Empty; // TODO: initialisez à une valeur appropriée
            string date_format = string.Empty; // TODO: initialisez à une valeur appropriée
            string tableCollation = string.Empty; // TODO: initialisez à une valeur appropriée
            int connection_timeout = 0; // TODO: initialisez à une valeur appropriée
            int expected = 0; // TODO: initialisez à une valeur appropriée
            int actual;
            actual = target.Update(new DatabaseTable(targetSchemaName, targetTableName),new DatabaseTable(withSchemaName, withTableName), updated_PKs_Table, date_format, tableCollation, connection_timeout);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }


        /// <summary>
        ///Test pour isDBEmpty
        ///</summary>
        [TestMethod()]
        public void isDBEmptyTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            bool expected = false; // TODO: initialisez à une valeur appropriée
            bool actual;
            actual = target.isDBEmpty();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour isTableExist
        ///</summary>
        [TestMethod()]
        public void isTableExistTest()
        {
            string connectionStringName = "";
            MSSQL2005_DBConnection target = new MSSQL2005_DBConnection(connectionStringName);
            string schemaName = string.Empty; // TODO: initialisez à une valeur appropriée
            string tableName = string.Empty; // TODO: initialisez à une valeur appropriée
            bool expected = false; // TODO: initialisez à une valeur appropriée
            bool actual;
            actual = target.isTableExist(new DatabaseTable(schemaName, tableName));
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour ExceptionLogger
        ///</summary>
        [TestMethod()]
        public void ExceptionLoggerTest()
        {
            ILog expected = null; // TODO: initialisez à une valeur appropriée
            ILog actual;
            MSSQL2005_DBConnection.ExceptionLogger = expected;
            actual = MSSQL2005_DBConnection.ExceptionLogger;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        /// <summary>
        ///Test pour InfoLogger
        ///</summary>
        [TestMethod()]
        public void InfoLoggerTest()
        {
            ILog expected = null; // TODO: initialisez à une valeur appropriée
            ILog actual;
            MSSQL2005_DBConnection.InfoLogger = expected;
            actual = MSSQL2005_DBConnection.InfoLogger;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }



    }
}
