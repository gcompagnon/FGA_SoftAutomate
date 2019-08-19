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

namespace SQLCopy
{
    class SQL
    {
        /*
        public static string getCommande(string filename)
        {
            string resultat = "";
            try
            {
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier 
                StreamReader monStreamReader = new StreamReader(filename);
                string ligne = monStreamReader.ReadLine();

                // Lecture de toutes les lignes et affichage de chacune sur la page 
                while (ligne != null)
                {
                    resultat += ligne + "\n";
                    ligne = monStreamReader.ReadLine();
                }
                // Fermeture du StreamReader
                monStreamReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur est survenue au cours de la lecture du fichier contenant la requette SQL ! Filename: " + filename);
                Console.WriteLine(ex.Message);
            }
            return resultat;
        }
        */

        /*
        public static DataTable requeteSQL(string sCommande)
        {
            //Chaine de connexion à la BDD
            ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["Connection1"];


            //Objet de connection à la BDD:
            SqlConnection cn = new SqlConnection(cs.ToString());

            // Adapateur pour comprendre les SQL databases:
            SqlDataAdapter da = new SqlDataAdapter(sCommande, cn);

            // DataTable pour récupérer le résultat de ma requette:
            DataTable dataTable = new DataTable();

            try
            {
                cn.Open();

                // Fill the data table with select statement's query results:
                int recordsAffected = da.Fill(dataTable);

                if (recordsAffected > 0)
                {
                    Console.WriteLine("Commande récupérée avec succes");
                }
                else if (recordsAffected == 0)
                {
                    Console.WriteLine("Table vide");
                }
            }
            catch (SqlException e)
            {
                string msg = "";
                for (int i = 0; i < e.Errors.Count; i++)
                {
                    msg += "Error #" + i + " Message: " + e.Errors[i].Message + "\n";
                }
                System.Console.WriteLine(msg);
            }
            finally
            {
                if (cn.State != ConnectionState.Closed)
                {
                    cn.Close();
                }
            }
            return dataTable;
        }
        */

        
        public static DataType GetDataType(string dataType)
        {
            DataType DTTemp = null;

            switch (dataType)
            {
                case ("System.Decimal"):
                    DTTemp = DataType.Decimal(2, 18);
                    break;
                case ("System.String"):
                    DTTemp = DataType.VarChar(50);
                    break;
                case ("System.Int32"):
                    DTTemp = DataType.Int;
                    break;
                case ("System.Double"):
                    DTTemp = DataType.Float;
                    break;
                case ("System.DateTime"):
                    DTTemp = DataType.DateTime;
                    break;
                case ("System.Single"):
                    DTTemp = DataType.Real;
                    break;
            }
            return DTTemp;
        }
        

        public static void copie( DataTable dt, string tableName,int ind)
        {
            //--------------------------------------------------------------------------------
            //Set destination connection string
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Connection1"];
            SqlConnection Connection = new SqlConnection(connectionString.ToString());

            //SMO Server object setup with SQLConnection.
            Server server = new Server(new ServerConnection(Connection));

            //Create a new SMO Database giving server object and database name
            //Database db = new Database(server, "TestSMODatabase");
            //db.Create();


            //--------------------------------------------------------------------------------
            string[] opt = connectionString.ToString().Split(';');
            string dbName = "";
            for (int i = 0; i < opt.Length; i++)
            {
                if(opt[i].Contains("Initial Catalog"))
                    dbName+= opt[i].Split('=')[1];
            }
            //Set Database to the newly created database
            Database db = server.Databases[dbName];

            //Create a new SMO table
            Table TestTable = new Table(db, tableName);
            //--------------------------------------------------------------------------------

            //SMO Column object referring to destination table.
            Column tempC = new Column();

            //Add the column names and types from the datatable into the new table
            //Using the columns name and type property
            foreach (DataColumn dc in dt.Columns)
            {
                //Create columns from datatable column schema
                tempC = new Column(TestTable, dc.ColumnName);
                tempC.DataType = GetDataType(dc.DataType.ToString());
                TestTable.Columns.Add(tempC);



                Microsoft.SqlServer.Management.Smo.Column tmp = new Column();
                //tmp = dc;

            }
            //Create the Destination Table
            try
            {
                TestTable.Create();
            }
            catch (FailedOperationException)
            {
                Console.WriteLine("Erreur : Plantage lors de la création de la table, tentative avec un autre nom");
                copie(dt, tableName + (ind + 1).ToString(), ind + 1);
            }
            //---------------------------------------------------------------------------------------------------
            //First create a connection string to destination database
            //Open a connection with destination database;
            using (SqlConnection connection = 
                   new SqlConnection(connectionString.ToString()))
            {
               connection.Open();

               //Open bulkcopy connection.
               using (SqlBulkCopy bulkcopy = new SqlBulkCopy(connection))
               {
   	            //Set destination table name
	            //to table previously created.
	            bulkcopy.DestinationTableName = tableName;

	            try
	            {
	               bulkcopy.WriteToServer(dt);
	            }
	            catch (Exception ex)
	            {
	               Console.WriteLine(ex.Message);
	            }
	
	            connection.Close();
               }
            }
            
        }

        /*
        public static void CD()
        {


            //Seconde connection---------------------------------------------------------------
            ConnectionStringSettings connectionString2 = ConfigurationManager.ConnectionStrings["Connection2"];
            SqlConnection Connection2 = new SqlConnection(connectionString2.ToString());

            //Premiere connection
            ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["Connection1"];
            SqlConnection cn = new SqlConnection(cs.ToString());


            string[] opt = cs.ToString().Split(';');
            string dbName = "";
            for (int i = 0; i < opt.Length; i++)
            {
                if (opt[i].Contains("Initial Catalog"))
                    dbName += opt[i].Split('=')[1];
            }

            cn.Open();
            Connection2.Open();

            Server server = new Server(new ServerConnection(cn));
            Database db = server.Databases[dbName];

            foreach (Table t in db.Tables)
            {

                //try
                //{
                    DataTable dataTable = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT *FROM " + t.Name, cn);
                    da.Fill(dataTable);

                    //SqlCommand command = new SqlCommand(@"DELETE FROM "+t.Name, Connection2);
                    //command.ExecuteNonQuery();

                    using (SqlBulkCopy bulkcopy = new SqlBulkCopy(Connection2))
                    {
                        //Set destination table name
                        //to table previously created.
                        bulkcopy.DestinationTableName = t.Name;
                        bulkcopy.WriteToServer(dataTable);
                    }
                //}
                //catch (Exception)
                //{
                //    Console.WriteLine("Echec de remplissage de la table : " + t.Name);
                //}


            }

            Connection2.Close();
            cn.Close();

        }
        */

        //Booleen si la table de destination est vides
        public static bool targetBDDEmpty( String destinationConnection = "Connection2")
        {
            bool empty = true;
            string connectionString2 = ConfigurationManager.ConnectionStrings[destinationConnection].ToString();
            SqlConnection Connection2 = new SqlConnection(connectionString2.ToString());
            //------------------------------------
            //SMO Server object setup with SQLConnection.
            Server server2 = new Server(new ServerConnection(Connection2));

            string[] opt2 = connectionString2.ToString().Split(';');
            string dbName2 = "";
            for (int i = 0; i < opt2.Length; i++)
            {
                if (opt2[i].Contains("Initial Catalog"))
                    dbName2 += opt2[i].Split('=')[1];
            }
            //Set Database to the newly created database
            Database db2 = server2.Databases[dbName2];
            try
            {
                if (db2.Tables.Count != 0)
                    empty = false;
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("La base de destination n'existe pas!");
                empty = false;
            }


            //------------------------------------
            Connection2.Close();


            return empty;
        }

        //Copie les métata data d'une BDD sur un serveur vers un autre
        public static void copyMetaData(String sourceConnection = "Connection1", String destinationConnection = "Connection2" )
        {
            //Obtenion de la DataBase de la premiere Connection---------------------------------------------------------------
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[sourceConnection];
            SqlConnection Connection = new SqlConnection(connectionString.ToString());

            //SMO Server object setup with SQLConnection.
            Server server = new Server(new ServerConnection(Connection));

            string[] opt = connectionString.ToString().Split(';');
            string dbName = "";
            for (int i = 0; i < opt.Length; i++)
            {
                if (opt[i].Contains("Initial Catalog"))
                    dbName += opt[i].Split('=')[1];
            }
            //Set Database to the database
            Database db = server.Databases[dbName];
            //----------------------------------------------------------------------------------------------------------------


            //Obtenion de la DataBase de la seconde Connection---------------------------------------------------------------
            ConnectionStringSettings connectionString2 = ConfigurationManager.ConnectionStrings[destinationConnection];
            SqlConnection Connection2 = new SqlConnection(connectionString2.ToString());
            //----------------------------------------------------------------------------------------------------------------
            
            /*Option pour la creation des tables
             * inclus les cles primaires, les contraintes nonclustered et
             * if not exist pour ne pas creer une table qui existe deja*/
            ScriptingOptions scriptOptions = new ScriptingOptions();
            scriptOptions.DriPrimaryKey = true;
            scriptOptions.IncludeIfNotExists = true;
            scriptOptions.DriNonClustered = true;

            /*Option pour les foreign key de chaque table, 
             * préposé de leur schéma*/
            ScriptingOptions scrOpt = new ScriptingOptions();
            scrOpt.SchemaQualifyForeignKeysReferences = true;
            scrOpt.DriForeignKeys = true;

            Console.WriteLine("Obtention metadonnees tables et clefs");
            StringCollection schemaCollection = new StringCollection();
            StringCollection tableCol = new StringCollection();
            StringCollection foreignKeyCol = new StringCollection();
            foreach (Table myTable in db.Tables)
            {
                //Si c'est un nouveau schéma on retient son nom
                if(! schemaCollection.Contains("["+myTable.Schema+"]"))
                    schemaCollection.Add("["+myTable.Schema+"]");

                //On joute le script de la table à tableCol
                StringCollection tableScripts = myTable.Script(scriptOptions);
                string[] tmp = new string[tableScripts.Count];
                tableScripts.CopyTo(tmp, 0);
                tableCol.AddRange(tmp);

                //On ajoute le script des foreign keys à foreignKeyCol
                ForeignKeyCollection fk = myTable.ForeignKeys;
                foreach (ForeignKey myFk in fk)
                {
                    StringCollection stmp = myFk.Script(scrOpt);
                    string[] tmp2 = new string[stmp.Count];
                    stmp.CopyTo(tmp2, 0);
                    foreignKeyCol.AddRange(tmp2);
                }
            }
            //Eneleve le schéma par défault
            schemaCollection.Remove("[dbo]");

            
            Console.WriteLine("Obtention des Procédures stockées");
            ScriptingOptions scrOpt2 = new ScriptingOptions() { IncludeIfNotExists = true};
            StringCollection prostoCol = new StringCollection();
            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                if(!sp.Schema.Equals("sys") && ! sp.IsSystemObject )
                {   
                    StringCollection scsp = sp.Script();
                    string[] tmp2 = new string[scsp.Count];
                    scsp.CopyTo(tmp2, 0);
                    prostoCol.AddRange(tmp2);
                }
                
            }


            Console.WriteLine("Obtention Metadonees schemas");
            SchemaCollection sc = db.Schemas;
            StringCollection schemaCol = new StringCollection();
            foreach (Schema schem in sc)
            { 
                if(schemaCollection.Contains(schem.ToString()))
                {
                    StringCollection tableScripts = schem.Script(new ScriptingOptions() {IncludeIfNotExists=true });
                    string[] tmp = new string[tableScripts.Count];
                    tableScripts.CopyTo(tmp, 0);
                    string[] strtmp={"AUTHORIZATION"};
                    schemaCol.Add(tmp[0].Split(strtmp, new StringSplitOptions())[0]+"AUTHORIZATION [dbo]'");

                }
            }
            Connection.Close();

            //Execution sur la nouvelle base des scripts récupérés
            Connection2.Open();
            using (SqlCommand cmd = Connection2.CreateCommand())
            {
                Console.WriteLine("Création Schemas");
                foreach (string str in schemaCol)
                {
                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Création Tables");
                foreach (string str in tableCol)
                {
                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Création Clefs etrangeres");
                foreach (string str in foreignKeyCol)
                {
                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Création Procedures Stockées");
                foreach (string str in prostoCol)
                {
                    try
                    {
                        cmd.CommandText = str;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Suite à une erreur la procédure suivante n'a pas pu etre copiée");
                        Console.WriteLine(str);
                        Console.WriteLine(e.Message);
                    }
                    
                }
            }
            Connection2.Close();      
        }


        /*
        public static void CMRB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection1"].ToString();
            string connectionString2 = ConfigurationManager.ConnectionStrings["Connection2"].ToString();

            // Open a sourceConnection to the AdventureWorks database.
            using (SqlConnection sourceConnection =
                       new SqlConnection(connectionString))
            {
                sourceConnection.Open();

                //// Perform an initial count on the destination table.
                //SqlCommand commandRowCount = new SqlCommand(
                //    "SELECT COUNT(*) FROM " +
                //    "dbo.BulkCopyDemoMatchingColumns;",
                //    sourceConnection);
                //long countStart = System.Convert.ToInt32(
                //    commandRowCount.ExecuteScalar());
                //Console.WriteLine("Starting row count = {0}", countStart);

                //SMO Server object setup with SQLConnection.
                Server server = new Server(new ServerConnection(sourceConnection));

                string[] opt = connectionString.ToString().Split(';');
                string dbName = "";
                for (int i = 0; i < opt.Length; i++)
                {
                    if (opt[i].Contains("Initial Catalog"))
                        dbName += opt[i].Split('=')[1];
                }
                //Set Database to the newly created database
                Database db = server.Databases[dbName];

                foreach (Table table in db.Tables)
                {
                    Console.WriteLine("Obtention des données table : "+table.Name);
                    // Get data from the source table as a SqlDataReader.
                    SqlCommand commandSourceData = new SqlCommand("SELECT * FROM "+table.Name, sourceConnection);
                    SqlDataReader reader = commandSourceData.ExecuteReader();

                    // Open the destination connection. In the real world you would 
                    // not use SqlBulkCopy to move data from one table to the other 
                    // in the same database. This is for demonstration purposes only.
                    using (SqlConnection destinationConnection = new SqlConnection(connectionString2))
                    {
                        destinationConnection.Open();

                        // Set up the bulk copy object. 
                        // Note that the column positions in the source
                        // data reader match the column positions in 
                        // the destination table so there is no need to
                        // map columns.
                        using (SqlBulkCopy bulkCopy =
                                    new SqlBulkCopy(destinationConnection))
                        {
                            bulkCopy.DestinationTableName =table.Name;
                            Console.WriteLine("Ecriture des données nouvelle BDD");
                            try
                            {
                                // Write from the source to the destination.
                                bulkCopy.WriteToServer(reader);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            finally
                            {
                                // Close the SqlDataReader. The SqlBulkCopy
                                // object is automatically closed at the end
                                // of the using block.
                                reader.Close();
                            }
                        }

                }//Fin du using destinationConnnection
            }//Fin du foreach
        }//fin du using sourceConnection
    }
        */

        //Copie les données d'une bdd à l'autre
        public static void copyData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Connection1"].ToString();
            string connectionString2 = ConfigurationManager.ConnectionStrings["Connection2"].ToString();

            // Open a sourceConnection to the first database
            using (SqlConnection sourceConnection =
                       new SqlConnection(connectionString))
            {
                sourceConnection.Open();

                //SMO Server object setup with SQLConnection.
                Server server = new Server(new ServerConnection(sourceConnection));
                string[] opt = connectionString.ToString().Split(';');
                string dbName = "";
                for (int i = 0; i < opt.Length; i++)
                {
                    if (opt[i].Contains("Initial Catalog"))
                        dbName += opt[i].Split('=')[1];
                }
                //Set Database to the database
                Database db = server.Databases[dbName];

                //Seconde connection
                SqlConnection destinationConnection = new SqlConnection(connectionString2);
                SqlBulkCopy bulkCopy = new SqlBulkCopy(destinationConnection);
                bulkCopy.BulkCopyTimeout = 120;
                bulkCopy.BatchSize = 1500;

                //Liste des tables à exclure
                List<Table> lt = new List<Table>();
                StringCollection exclude = new StringCollection();
                exclude.Add("PTF_TRANSPARISE");
                exclude.Add("PTF_TRANSPARISE_2011");
                foreach (Table t in db.Tables)
                {//!exclude.Contains(t.Name)
                    if (!exclude.Contains(t.Name)) { lt.Add(t); }
                }
                
                
                foreach (Table table in lt)
                {   
                    
                    destinationConnection.Open();
                    Console.Write("Obtention des données table : " + table.Name);
                    // Get data from the source table as a SqlDataReader.
                    SqlCommand commandSourceData = new SqlCommand("SELECT * FROM " + table.Schema+"."+table.Name, sourceConnection);
                    SqlDataReader reader = commandSourceData.ExecuteReader();
                    Console.WriteLine(" -- OK");
                       
                    bulkCopy.DestinationTableName = table.Schema+"."+table.Name;
                    Console.Write("Ecriture des données nouvelle BDD");
                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(reader);
                        Console.WriteLine(" -- OK");    
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        reader.Close();
                        destinationConnection.Close();
                    }
                }//Fin foreach
                sourceConnection.Close();
            }//Fin du using sourceCponnection
        }

        /*
        public static void vidange()
        {
            //Obtenion de la DataBase de la seconde Connection---------------------------------------------------------------
            ConnectionStringSettings connectionString2 = ConfigurationManager.ConnectionStrings["Connection2"];
            SqlConnection Connection2 = new SqlConnection(connectionString2.ToString());

            //SMO Server object setup with SQLConnection.
            Server server = new Server(new ServerConnection(Connection2));

            string[] opt = connectionString2.ToString().Split(';');
            string dbName = "";
            for (int i = 0; i < opt.Length; i++)
            {
                if (opt[i].Contains("Initial Catalog"))
                    dbName += opt[i].Split('=')[1];
            }
            //Set Database to the newly created database
            Database db = server.Databases[dbName];


            //Obtention de tout les noms
            StringCollection nameCol = new StringCollection();
            foreach (Table t in db.Tables)
            {
                nameCol.Add(t.Name);
            }



            //Connection2.Open();
            using (SqlCommand cmd = Connection2.CreateCommand())
            {
                Console.WriteLine("Suppression des données dans les tables");
                foreach (string str in nameCol)
                {
                    Console.WriteLine("    Vidage table : "+str);
                    cmd.CommandText = "DELETE FROM "+str;
                    cmd.ExecuteNonQuery();
                }
            }
            Connection2.Close();
        }
       */
         
         
    //Multi select dans un dataset, Fonctionne
    public static DataSet multiRequeteSQL(string requete)
    {
        //Chaine de connexion à la BDD
        ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["Connection2"];
        //Objet de connection à la BDD:
        SqlConnection sqlConnection = new SqlConnection(cs.ToString());

        SqlDataAdapter sqlDa = new SqlDataAdapter();
        SqlCommand selectCmd = new SqlCommand();
        selectCmd.CommandText = requete;
        selectCmd.CommandType = CommandType.Text;
        selectCmd.Connection = sqlConnection;
        sqlDa.SelectCommand = selectCmd;

        // Add table mappings to the SqlDataAdapter
        sqlDa.TableMappings.Add("Table", "Customers");
        sqlDa.TableMappings.Add("Table1", "Orders");

        // DataSet1 is a strongly typed DataSet
        DataSet ds = new DataSet();

        sqlConnection.Open();

        sqlDa.Fill(ds);

        sqlConnection.Close();

        return ds;
    }

    //Fonction de sauvegarde de la BDD, Fonctionne
    public static void backup(string destinationConnection = "Connection2")
    {   
        //Chaine de connexion à la BDD
        ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings[destinationConnection];

        //Objet de connection à la BDD:
        SqlConnection cn = new SqlConnection(cs.ToString());

        // Adapateur pour comprendre les SQL databases:
           
        SqlCommand command = new SqlCommand(@"Backup database FGA_SOFT to disk 'c:\bp.bak'", cn);
  
        cn.Open();
        command.ExecuteNonQuery();    
        cn.Close();
    }

    }
}
