using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Data;
using System.Data.SqlClient;



using System.Configuration;
using System.Data.SqlClient;


using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;


namespace SQLCopy
{
    class Program
    {
        static void affDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows) // Loop over the rows.
            {
                Console.WriteLine("--- Row ---"); // Print separator.
                foreach (var item in row.ItemArray) // Loop over the items.
                {
                    Console.Write("Item: "); // Print label.
                    Console.WriteLine(item); // Invokes ToString abstract method.
                }
            }
        }


        //Copie les métata data d'une BDD d'un serveur vers un autre
        public static void copyMetaData(string connexion1, string connexion2, List<string> include, List<string> exclude)
        {
            //Obtenion de la DataBase de la premiere Connection---------------------------------------------------------------
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[connexion1];
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
            ConnectionStringSettings connectionString2 = ConfigurationManager.ConnectionStrings[connexion2];
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
            /*ScriptingOptions scrOpt = new ScriptingOptions();
            scrOpt.SchemaQualifyForeignKeysReferences = true;
            scrOpt.DriForeignKeys = true;
            */

            bool includeMode = exclude.Count == 0 || exclude == null;
            bool excludeMode = include.Count == 0 || include == null;

            Console.WriteLine("Obtention metadonnees tables et clefs");
            List<string> schemaCollection = new List<string>();
            List<string> tableCol = new List<string>();
            //List<string> foreignKeyCol = new List<string>();
            foreach (Table myTable in db.Tables)
            {
                if ((includeMode && include.Contains(myTable.Name)) || (excludeMode && !exclude.Contains(myTable.Name)))
                {
                    //Si c'est un nouveau schéma on retient son nom
                    if (!schemaCollection.Contains("[" + myTable.Schema + "]"))
                        schemaCollection.Add("[" + myTable.Schema + "]");

                    //On joute le script de la table à tableCol
                    foreach (string s in myTable.Script(scriptOptions))
                    {
                        tableCol.Add(s);
                    }

                    #region foreign keys, inutile
                    /*//On ajoute le script des foreign keys à foreignKeyCol
                ForeignKeyCollection fk = myTable.ForeignKeys;
                foreach (ForeignKey myFk in fk)
                {
                    StringCollection stmp = myFk.Script(scrOpt);
                    string[] tmp2 = new string[stmp.Count];
                    stmp.CopyTo(tmp2, 0);
                    foreignKeyCol.AddRange(tmp2);
                }*/
                    #endregion
                }
            }
            //Eneleve le schéma par défault
            schemaCollection.Remove("[dbo]");

            #region Procedures stockées, inutile
            /*Console.WriteLine("Obtention des Procédures stockées");
            ScriptingOptions scrOpt2 = new ScriptingOptions() { IncludeIfNotExists = true };
            StringCollection prostoCol = new StringCollection();
            foreach (StoredProcedure sp in db.StoredProcedures)
            {
                if (!sp.Schema.Equals("sys"))
                {
                    StringCollection scsp = sp.Script();
                    string[] tmp2 = new string[scsp.Count];
                    scsp.CopyTo(tmp2, 0);
                    prostoCol.AddRange(tmp2);
                }

            }*/
            #endregion


            Console.WriteLine("Obtention Metadonees schemas");
            SchemaCollection sc = db.Schemas;
            List<string> schemaCol = new List<string>();
            foreach (Schema schem in sc)
            {
                if (schemaCollection.Contains(schem.ToString()))
                {
                    string[] strtmp = { "AUTHORIZATION" };
                    schemaCol.Add(schem.Script(new ScriptingOptions() { IncludeIfNotExists = true })[0].Split(strtmp, new StringSplitOptions())[0] + "AUTHORIZATION [dbo]'");

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
            }
            Connection2.Close();
        }
        
        /// <summary>
        ///Copie les données d'une BDD à l'autre 
        /// </summary>
        /// <param name="connexion1">Source</param>
        /// <param name="connexion2">Destination</param>
        /// <param name="include"></param>
        /// <param name="exclude"></param>
        public static void copyData(string connexion1, string connexion2, List<string> include, List<string> exclude)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connexion1].ToString();
            string connectionString2 = ConfigurationManager.ConnectionStrings[connexion2].ToString();

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

                //Liste des tables à traiter
                bool includeMode = exclude.Count == 0 || exclude == null;
                bool excludeMode = include.Count == 0 || include == null;
                List<Table> lt = new List<Table>();
                foreach (Table myTable in db.Tables)
                {
                    if ((includeMode && include.Contains(myTable.Name)) || (excludeMode && !exclude.Contains(myTable.Name)))
                    { lt.Add(myTable); }
                }


                foreach (Table table in lt)
                {

                    destinationConnection.Open();
                    Console.Write("Obtention des données table : " + table.Name);
                    // Get data from the source table as a SqlDataReader.
                    SqlCommand commandSourceData = new SqlCommand("SELECT * FROM " + table.Schema + "." + table.Name, sourceConnection);
                    SqlDataReader reader = commandSourceData.ExecuteReader();
                    Console.WriteLine(" -- OK");

                    bulkCopy.DestinationTableName = table.Schema + "." + table.Name;
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


        public static void tralala(string connexion1,List<string> include)
        {
            //Obtenion de la DataBase de la premiere Connection---------------------------------------------------------------
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[connexion1];
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
            //----------------------------------------------------------------------------------------------------------


            // Define a Scripter object and set the required scripting options. 
            Scripter scrp = new Scripter(server);
            scrp.Options.ScriptDrops = false;
            scrp.Options.WithDependencies = true;
            scrp.Options.Indexes = true;   // To include indexes
            scrp.Options.DriAllConstraints = true;   // to include referential constraints in the script
            scrp.Options.SchemaQualify = true;



            // Iterate through the tables in database and script each one. Display the script.   
            foreach (Schema tb in db.Schemas)
            {
                // check if the table is not a system table
                if (tb.IsSystemObject == false)
                {
                    if (include.Contains(tb.Name))
                    {
                        Console.WriteLine("-- Scripting for table " + tb.Name);

                        // Generating script for table tb
                        System.Collections.Specialized.StringCollection sc = scrp.Script(new Urn[] { tb.Urn });
                      
                        foreach (string st in sc)
                        {
                            Console.WriteLine(st);
                        }
                        Console.WriteLine("--");
                    }
                }
            } 



        }

        static void Main(string[] args)
        {
            List<string> tables = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h" };
            List<string> include = new List<string>();
            List<string> exclude = new List<string>();

            include.Add("x");
            exclude.Add("w");


            bool includeMode = exclude.Count == 0 || exclude == null;
            bool excludeMode = include.Count == 0 || include == null;

            foreach (string myTable in tables)
            {
                if ((includeMode && include.Contains(myTable)) || (excludeMode && !exclude.Contains(myTable)))
                {
                    Console.WriteLine(myTable);
                }
            }



//            tralala("ConnectionAdmin", new List<string>() { "TX_EMETTEUR_FICHIER", "TX_SIGNATURE", "TX_FICHIER" });

//            copyMetaData("FGA_DEV_BAK", "FGA_DEV_1", new List<string>() { "TX_EMETTEUR_FICHIER" }, new List<string>() { });


            //copyData("FGA_DEV_BAK", "ConnectionAdmin", new List<string>() { "TX_RATING", "SOUS_SECTEUR", "TX_GROUPE", "TX_RECOMMANDATION", "TX_RECOMMANDATION_PAYS", "TX_RECOMMANDATION_SOUS_SECTEUR" }, new List<string>() { });
//            copyData("FGA_DEV_BAK", "ConnectionAdmin", new List<string>() { "TX_HISTO_RATING", "TX_HISTO_RECOMMANDATION" }, new List<string>() { });

//            copyData("FGA_DEV_BAK", "ConnectionAdmin", new List<string>() { "TMP_SIGNATURE_OMEGA", "TMP_PAYS_OMEGA", "TMP_SECTEUR_OMEGA", "TMP_SOUS_SECTEUR_OMEGA" }, new List<string>() { });


//            copyData("FGA_DEV_BAK", "ConnectionAdmin", new List<string>() { "TX_SIGNATURE" }, new List<string>() { });
            bool bddVide = SQL.targetBDDEmpty("FGA_JMOINS1");
            if (bddVide)
            {

                SQL.copyMetaData("ConnectionAdmin", "FGA_JMOINS1");
                
                //copyData("ConnectionLec", "FGA_BAK", new List<string>() { "TX_RACINE" }, new List<string>() { });
            }
            else
                Console.WriteLine("La base de donnée de destinaion n'est pas vide, opération annulée");

            //tralala("ConnectionAdmin", new List<string>() { "INDEX_ASSET" });

            /*
            bool bddVide = SQL.targetBDDEmpty();
            if (bddVide)
            {
                SQL.copyMetaData();
                SQL.copyData();
            }
            else
                Console.WriteLine("La base de donnée de destinaion n'est pas vide, opération annulée");


            */


        }
    }
}
