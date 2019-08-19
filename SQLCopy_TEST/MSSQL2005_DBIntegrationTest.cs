using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;


using System.Configuration;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using SQLCopy.Helpers.DataReader;
using System.Data.Common;
using SQLCopy.Helpers.DataAdapter;
using SQLCopy.Dbms;
using System.Text.RegularExpressions;
using FGA.Automate.Consumer;

namespace FGA.SQLCopy
{
    [TestClass]
    public class MSSQL_2005_DBIntegrationTest
    {
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

        /// <summary>
        /// Executer une requete SQL sur la base Source pour l insertion dans la base Destination
        /// </summary>
        [TestMethod]
        public void TestBulkCopyDataSourceSQLFile()
        {
            string dest = @"Data Source=FX026132M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration("OMEGA", dest);
            // Read the file as one string.
            System.IO.StreamReader myFile =
               new System.IO.StreamReader("SourceRequestOmega.sql");
            string request = myFile.ReadToEnd();

            myFile.Close();

            s.bulkcopySourceRequest(request, new DatabaseTable("STRAT_VALEUR_INDICE"));

        }

        [TestMethod]
        public void TestBulkCopyDataSourceRequestSQLFile_OMEGA()
        {
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration("OMEGA", "FGA_JMOINS1");

            string filePath = @"C:\FGA_SOFT\DEVELOPPEMENT\PROJET\FGA_Soft_Front\Front\SQL_SCRIPTS\OMEGA\PTF_FGA.sql";

            // Read the file as one string.
            System.IO.StreamReader myFile =
               new System.IO.StreamReader(filePath);
            string request = myFile.ReadToEnd();
            myFile.Close();

            request = request.Replace("'***'", "'09/01/2014'");
            
            s.bulkcopySourceRequest(request, new DatabaseTable("PTF_FGA"));

        }


        [TestMethod]
        public void TestBulkCopyDataCriteria()
        {

            string dest = @"Data Source=FX026132M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration("ConnectionAdmin", dest);

            s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() { new DatabaseTable("PTF_RAPPORT") }, ColumnCriteria: "Date", CriteriaValue: "MAX");


        }

        [TestMethod]
        public void TestBulkCopyDataBottomLimit()
        {

            string dest = @"Data Source=FX026132M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration("ConnectionAdmin", dest);

            s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() { new DatabaseTable(
                    "PTF_TRANSPARISE")}, 1000, "dateinventaire");
        }
        /// <summary>
        /// Premiere etape pour un backup : creer les tables , schema, procstock et Foreign Key 
        /// </summary>
        [TestMethod]
        public void TestSourceDataCopy()
        {

            //string dest = @"Data Source=FX026132M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            //string dest = @"Data Source=FX027471M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            //DBConnection d = new MSSQL2005_DBConnection(dest);


            //SQLCopy.SQLUtils s = new SQLCopy.SQLUtils("ConnectionAdmin", dest);
            DBConnectionDelegate s = new MSSQL2005_DBConnection("FGA_RW");
            DBConnectionDelegate d = new MSSQL2005_DBConnection("FGA_JMOINS1");
            bool bddVide = d.isDBEmpty();
            if (bddVide)
            {

                DataSet ds = s.GetMetatDataDBScripts();

                // sauvegarde des fichiers de scripts
                TextFile.WriteTo(ds, "C:", "fga_db", "sql");

                // on execute les scripts sur la base destination
                DataTable schema = ds.Tables["SCHEMA"];
                d.ExecuteScripts(schema);
                DataTable table = ds.Tables["TABLE"];
                d.ExecuteScripts(table);
                DataTable fk = ds.Tables["FK"];
                d.ExecuteScripts(fk);
                DataTable procstock = ds.Tables["PROCSTOC"];
                d.ExecuteScripts(procstock, useDBcmd: false);
            }
            else
                Console.WriteLine("La base de donnée de destinaion n'est pas vide, opération annulée");

        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void TestBulkCopyData_4FACTSET()
        {
            //string dest = @"Data Source=FX023179M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            //SQLCopy.SQLUtils s = new SQLCopy.SQLUtils("ConnectionAdmin", dest);
            string source = @"Data Source=FX027471M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";

            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(source, "FGA_RW");

            //s.bulkcopyData(ListeMode.Include, new List<string>() {
            //        "ACT_RECO_COMMENT",
            //        "ACT_RECO_SECTOR",
            //        "ACT_RECO_VALEUR",
            //        "ACT_FGA_SECTOR_RECOMMANDATION",
            //        "ACT_ICB_SECTOR_RECOMMANDATION",
            //        "ACT_RECOMMANDATION",
            //    });

            s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() { new DatabaseTable("ACT_DATA_FACTSET")
                }, "SELECT * FROM {0}.{1} where date = '27/12/2013'");

        }

        /// <summary>
        /// </summary>
        [TestMethod]
        public void TestBulkCopyData_DEV_2_PROD()
        {
            //string dest = @"Data Source=FX023179M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            //SQLCopy.SQLUtils s = new SQLCopy.SQLUtils("ConnectionAdmin", dest);
            string dev = @"Data Source=FX027471M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            string stage = @"Data Source=MEPAPP042_R;Initial Catalog=E2DBFGA01;Persist Security Info=True;User ID=E2FGATP;Password=E2FGATP25";

            string preprod = @"Data Source=VWI1BDD002;Initial Catalog=E1DBFGA01;Persist Security Info=True;User ID=e1fgatp;Password=e1fgatp02";
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(stage, preprod);

            s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() {
//                    "DATA_FACTSET"
                    //"SECTOR",
                    new DatabaseTable("IDENTIFICATION"),
                    new DatabaseTable("ASSET")
                    //"SECTOR_TRANSCO",
                    //"ASSET_TO_SECTOR"
                    //"ACT_COEF_CRITERE",
                    //"ACT_COEF_SECTEUR",
                    //"ACT_PTF",
                    //"ISR_NOTE"
                    //"ACT_AGR_FORMAT"
                });
        }


        /// <summary>
        /// </summary>
        [TestMethod]
        public void TestBulkCopyData_4DataModel()
        {
            string source = @"Data Source=FX026132M\SQLExpress;Initial Catalog=FGA_DATAMODEL;Integrated Security=True;Connection Timeout=60";
            DBConnectionDelegate s = new MSSQL2005_DBConnection(source);
            DBConnectionDelegate d = new MSSQL2005_DBConnection("FGA_JMOINS1");

            //DataSet ds = s.GetMetatDataDBScripts();
            //// sauvegarde des fichiers de scripts
            //TextFile.WriteTo(ds, "C:", "fgadatamodel_db", "sql");

            //// on execute les scripts sur la base destination
            //DataTable schema = ds.Tables["SCHEMA"];
            //d.ExecuteScripts(schema);
            //DataTable table = ds.Tables["TABLE"];
            //d.ExecuteScripts(table);
            //DataTable fk = ds.Tables["FK"];
            //d.ExecuteScripts(fk);
            //DataTable procstock = ds.Tables["PROCSTOC"];
            //d.ExecuteScripts(procstock, useDBcmd: false);


            //SQLCopy.SQLUtils s = new SQLCopy.SQLUtils("ConnectionAdmin", dest);
            SQLCopy.MSDBIntegration util = new SQLCopy.MSDBIntegration(s, d);


            util.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() {
                new DatabaseTable("ASSET_HOLDING"),
                new DatabaseTable("COMPONENT"),
                new DatabaseTable("INDEX"),
                new DatabaseTable("ref_holding","PORTFOLIO"),
                new DatabaseTable("VALUATION"),
                new DatabaseTable("ROLE"),
                new DatabaseTable("RATING"),
                new DatabaseTable("ASSET"),
                new DatabaseTable("ASSET_CLASSIFICATION"),
                new DatabaseTable("ASSET_PORTFOLIO"),
                new DatabaseTable("DEBT"),
                new DatabaseTable("EQUITY"),
                new DatabaseTable("FUND"),
                new DatabaseTable("PRICE"),
                new DatabaseTable("SECURITIES_ISSUANCE")
                //    "IDENTIFICATION"
                });


        }
        /// <summary>
        /// Deuxieme etape pour un backup : bulk copy des donnees des tables specifiees
        /// </summary>
        [TestMethod]
        public void TestBulkCopyData()
        {
            //string dest = @"Data Source=FX023179M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";
            //string dest = @"Data Source=FX007119m\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";

            string dest = @"Server=tcp:FX026896M,1433; Database=PreProLocal; User Id=super; Password=Password; Connection Timeout = 60";
            //SQLCopy.SQLUtils s = new SQLCopy.SQLUtils("ConnectionAdmin", dest);
            //string dest = @"Data Source=FX027471M\SQLExpress;Initial Catalog=FGA_JMOINS1;Integrated Security=True;Connection Timeout=60";

            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration("FGA_RW", dest);

            //s.bulkcopyData(ListeMode.Exclude, new List<DatabaseTable>() {
            //        new DatabaseTable("PTF_TRANSPARISE"),
            //        new DatabaseTable("PTF_RAPPORT"),
            //        new DatabaseTable("PTF_RAPPORT_NIV2"),
            //        new DatabaseTable("ACT_DATA_FACTSET_AGR"),
            //        new DatabaseTable("ACT_DATA_FACTSET"),
            //        new DatabaseTable("TX_IBOXX"),
            //        new DatabaseTable("TX_IBOXX_RAPPORT_EMETTEUR"),
            //        new DatabaseTable("TX_IBOXX_RAPPORT_EMETTEUR2"),
            //        new DatabaseTable("TX_IBOXX_RAPPORT"),
            //        new DatabaseTable("TX_IBOXX_RAPPORT_PRIME")
            //    });

            //s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() {
            //         new DatabaseTable("ACT_RECO_COMMENT"),
            //         new DatabaseTable("ACT_RECO_SECTOR"),
            //         new DatabaseTable("ACT_RECO_VALEUR"),
            //         new DatabaseTable("ACT_FGA_SECTOR_RECOMMANDATION"),
            //         new DatabaseTable("ACT_ICB_SECTOR_RECOMMANDATION"),
            //         new DatabaseTable("ACT_RECOMMANDATION"),
            //    });
            //s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() {
            //         new DatabaseTable("ACT_AGR_FORMAT"),
            //         new DatabaseTable("ISR_NOTE"),
            //         new DatabaseTable("SECTOR"),
            //         new DatabaseTable("SECTOR_TRANSCO"),
            //         new DatabaseTable("UTILISATEUR")
            s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() {
                     new DatabaseTable("ACT_COEF_SECTEUR"),
                     new DatabaseTable("ACT_COEF_CRITERE")                     
                });

            //s.bulkcopyData(ListeMode.Include, new List<DatabaseTable>() { new DatabaseTable("DATA_FACTSET")
            //    }, "SELECT * FROM {0}.{1} where date >= '01/11/2014'");

        }

        [TestMethod]
        public void CSVWriteValues()
        {
            using (var writer = new CsvFileWriter(@"C:\WriteTest.csv"))
            {
                // Write each row of data
                for (int row = 0; row < 100; row++)
                {
                    List<string> columns = new List<string>();
                    writer.WriteRow(columns);
                }
            }
        }
        [TestMethod]
        public void CSVReadValues()
        {
            List<string> columns = new List<string>();
            using (var reader = new CsvFileReader(@"C:\ReadTest.csv"))
            {
                while (reader.ReadRow(columns))
                {
                    Console.WriteLine(" Lecture columns " + columns);
                }
            }
        }

        #region OpenXML Input
        [TestMethod]
        public void OpenXMLReadValues_TestGetColumRowNumbers()
        {
            double[] i = OpenXMLDataAdapter.GetColumnRowXY("A255");
            Assert.AreEqual(i[0], 1);
            Assert.AreEqual(i[1], 255);
            i = OpenXMLDataAdapter.GetColumnRowXY("IV99999");
            Assert.AreEqual(i[0], 256);
            Assert.AreEqual(i[1], 99999);
            i = OpenXMLDataAdapter.GetColumnRowXY("ZZ1");
            Assert.AreEqual(i[0], 702);
            Assert.AreEqual(i[1], 1);
            i = OpenXMLDataAdapter.GetColumnRowXY("AAA1");
            Assert.AreEqual(i[0], 703);
            Assert.AreEqual(i[1], 1);
            i = OpenXMLDataAdapter.GetColumnRowXY("BAC1");
            Assert.AreEqual(i[0], 1381);
            Assert.AreEqual(i[1], 1);
            i = OpenXMLDataAdapter.GetColumnRowXY("XFD1048576");
            Assert.AreEqual(i[0], 16384);
            Assert.AreEqual(i[1], 1048576);

        }

        [TestMethod]
        public void AutoMappingHelper()
        {
            string[] label = {"C1","C2","C3","C4","C5","C6","C7"};
            DataTable dt = new DataTable ("TEST");
            foreach(string l in label)
            {
                dt.Columns.Add(l);
            }
            int i=0;
                        foreach (string c in dt.GetColumnsName())
                        {
                            Assert.AreEqual(c, label[i]);
                            i++;
                        }
        }
        /// <summary>
        /// Test de lecture du fichier XMSM du portfeuille modele pour alimenter un dataset (et une table de base de données)
        /// </summary>
        [TestMethod]
        public void OpenXMLReadValues_PortefeuilleModele()
        {
            DataSet ds = new DataSet("PTF_MODELE");
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_RW");
            /*            using (var reader = new OpenXMLDataReader(new StreamReader(@"Portefeuille modèle.xlsm"), null, true, "modele_€", "J7", "R94",null))
                        {

                            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(reader);
                        */
            string model_path1 = @"G:\,FGA Front Office\02_Gestion_Actions\01_MODELES\MODELE GARP\Portefeuille modèle.xlsm";
            string model_path2 = @"C:\DATA\Portefeuille modèle_20131125.xlsm"; 
            string model_path3 = @"C:\DATA\Portefeuille modèle.xlsm"; 
            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(model_path1 , "modele_€", "J7", "R94", true);

            //ITableMapping mapping = adapter.TableMappings.Add("Table", "PTF_FGA");
            //mapping.ColumnMappings.Add("CLOSE", "close");
            //mapping.ColumnMappings.Add("INDEX", "ponderationIndice");

            adapter.AddColumnMapping("PTF_FGA","COMPANY", "Libelle_Titre");
            adapter.AddColumnMapping("PTF_FGA", "QUANTITY", "quantite", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("PTF_FGA", "€CLOSE", "coursclose", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("PTF_FGA", "CRNCY", "Devise_Titre");
            
            adapter.FillColumnWithCell("Dateinventaire", "Q5", System.Type.GetType("System.DateTime"));
            adapter.FillColumnWithCell("Libelle_PtfSource", "G3");

            adapter.FillColumnWithValue("Groupe", "MODEL_ACT");
            adapter.FillColumnWithValue("Compte", "ACT001");
            adapter.FillColumnWithValue("ISIN_Ptf", "MODEL_ACT001");
            adapter.FillColumnWithValue("Coupon_Couru", 0d);
            adapter.FillColumnWithValue("Dateintegration", DateTime.Now);

            adapter.Fill(ds);

            DataTable dt = ds.Tables["PTF_FGA"];
            // conversion de colonne pour mettre dans la BDD
            // DataColumn.Expression pour remplir des colonnes
            //dt.Columns.Add("quantite", System.Type.GetType("System.Double"), "CONVERT( quantiteSource, System.Double )");
            //dt.Columns.Add("coursclose", System.Type.GetType("System.Double"), "CONVERT( courscloseSource, System.Double )");
            dt.Columns.Add("Libelle_Ptf", System.Type.GetType("System.String"), "TRIM(Libelle_PtfSource)");
            dt.Columns.Add("Valeur_Boursiere", System.Type.GetType("System.Double"), "quantite*coursclose"); 
            dt.Columns.Add("code_Titre", System.Type.GetType("System.String"), "TICKER + ' EQUITY'");

            //dt.Columns.Add("Dateinventaire", System.Type.GetType("System.DateTime"), "CONVERT(DateinventaireSource,System.DateTime)");

            s.bulkcopyData(dt);

        }

        /// <summary>
        /// Test de lecture du fichier XMSM du portfeuille modele pour alimenter un dataset (et une table de base de données)
        /// en mode 
        /// </summary>
        [TestMethod]
        public void OpenXMLReadValues_PortefeuilleModele2()
        {

            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_JMOINS1");
            DataTable excelDataTable = null;

            List<string> columns = new List<string>();


            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(@"C:\DATA\Portefeuille modèle.xlsm", false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = GetWorksheetPartByName(spreadsheetDocument, "modele_€");

                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                string text;
                string rowNum;
                while (reader.Read())
                {
                    if (reader.ElementType == typeof(SheetData))
                    {
                        //   SheetData sd = (SheetData)reader.LoadCurrentElement();

                        reader.ReadFirstChild();

                        do// while : Skip to the next row
                        {
                            if (reader.HasAttributes)
                            {
                                rowNum = reader.Attributes.First(a => a.LocalName == "r").Value;
                                Console.WriteLine("rowNum: " + rowNum);
                            }



                            if (reader.ElementType == typeof(Row))
                            {
                                reader.ReadFirstChild();

                                do// while: next Cell
                                {

                                    if (reader.ElementType == typeof(Cell))
                                    {
                                        Cell c = (Cell)reader.LoadCurrentElement();

                                        string cellValue;

                                        if (c.DataType != null && c.DataType == CellValues.SharedString)
                                        {
                                            SharedStringItem ssi = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(c.CellValue.InnerText));

                                            cellValue = ssi.Text.Text;
                                        }
                                        else if (c.CellValue != null)
                                        {
                                            cellValue = c.CellValue.InnerText;
                                        }
                                        else
                                        {
                                            cellValue = "Empty";
                                        }

                                        Console.WriteLine("{0}: {1} ", c.CellReference, cellValue);
                                    }
                                } while (reader.ReadNextSibling());// while: next Cell
                            }

                            Console.WriteLine("END ROW");
                        } while (reader.ReadNextSibling());// while : Skip to the next row

                        Console.WriteLine("END ROW");
                    }

                    if (reader.ElementType != typeof(Worksheet))
                        reader.Skip();
                }
                reader.Close();

            }

            //            s.bulkcopyData(dt: excelDataTable, nomTableDestination : "PTF_FGA");
        }

        private static WorksheetPart
           GetWorksheetPartByName(SpreadsheetDocument document,
           string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                // The specified worksheet does not exist.

                return null;
            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        private static Cell GetCell(Worksheet worksheet,
                  string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
                return null;

            return row.Elements<Cell>().Where(c => string.Compare
                   (c.CellReference.Value, columnName +
                   rowIndex, true) == 0).First();
        }


        // Given a worksheet and a row index, return the row.
        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }
        #endregion



        [TestMethod]
        public void LumenCSVReadValues()
        {
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_JMOINS1");
            List<string> columns = new List<string>();
            using (var reader = new CsvReader(new StreamReader(@"PortfolioHolding_PROXY1.csv"), true, ';'))
            {
                int fieldCount = reader.FieldCount;

                string[] headers = reader.GetFieldHeaders();
                while (reader.ReadNextRecord())
                {
                    for (int i = 0; i < fieldCount; i++)
                        Console.Write(string.Format("{0} = {1};",
                                      headers[i], reader[i]));
                    Console.WriteLine();
                }
            }
        }

        [TestMethod]
        public void LumenCSVReadValuesAndBulkCopyIntoDest_PROXY()
        {
            string proxyPath = @"G:\,FGA Systèmes\PRODUCTION\BASES\PROXY\20131231\portfolio\PortfolioHolding_MONNET.csv";

            DataSet ds = new DataSet("PTF_PROXY");

            SQLCopy.MSSQL2005_DBConnection dest = new SQLCopy.MSSQL2005_DBConnection("FGA_RW");
            using (var reader = new CsvReader(new StreamReader(proxyPath,Encoding.UTF8), true, ';'))
            {
                CsvDataAdapter adapter = new CsvDataAdapter(reader);
                adapter.Fill(dest, new DatabaseTable("dbo", "#PTF_PROXY"));
                dest.Insert(new DatabaseTable("dbo", "PTF_PROXY"), new DatabaseTable("dbo", "#PTF_PROXY"));
                //adapter.Fill(dest, new DatabaseTable("dbo", "PTF_PROXY"));
            }
        }
        [TestMethod]
        public void LumenCSVReadValuesAndInsertIntoDest()
        {

            TestContext.BeginTimer("LumenCSVReadValuesAndInsertIntoDest");

            DataSet ds = new DataSet("PTF_PROXY");
            DBConnectionDelegate dest = new MSSQL2005_DBConnection("FGA_JMOINS1");
            using (var reader = new CsvReader(new StreamReader(@"PortfolioHolding_PROXY2.csv"), true, ';'))
            {
                CsvDataAdapter adapter = new CsvDataAdapter(reader);
                adapter.Fill(dest, new DatabaseTable("dbo", "#PTF_PROXY"));

                dest.Insert(new DatabaseTable("dbo", "PTF_PROXY"), new DatabaseTable("dbo", "#PTF_PROXY"));
            }
            TestContext.EndTimer("LumenCSVReadValuesAndInsertIntoDest");
        }

        [TestMethod]
        public void LumenCSVReadValuesAndMergeIntoDest()
        {
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_JMOINS1");
            s.BulkUpsertCsv(@"PortfolioHolding_PROXY1.csv", new DatabaseTable("dbo", "PTF_PROXY"));
        }

        [TestMethod]
        public void LumenCSVReadValuesAndDeleteIntoDest()
        {
            DBConnectionDelegate destinationConnection = new MSSQL2005_DBConnection("FGA_JMOINS1");

            DataSet ds = new DataSet("PTF_PROXY_4DELETE");
            List<string> columns = new List<string>();
            using (var reader = new CsvReader(new StreamReader(@"Delete_PROXY1.csv"), true, ';'))
            {
                CsvDataAdapter adapter = new CsvDataAdapter(reader);
                int nbReadLines = adapter.Fill(ds, "#PTF_PROXY_CLAUSE");

                DataTable dt = ds.Tables["#PTF_PROXY_CLAUSE"];

                destinationConnection.Delete(new DatabaseTable("dbo", "PTF_PROXY"), dt);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestContext.WriteLine("{0} : {1}", TestContext.TestName, TestContext.CurrentTestOutcome);
        }

        [TestMethod]
        public void ACTION_PROCESS_BulkCopy_DATA_FACTSET_DATA_1()
        {
            string filepath = @"G:\,FGA MarketData\FACTSET\201402\base_20140210.csv";

            DataSet ds = new DataSet("DATA_FACTSET");        
            SQLCopy.MSSQL2005_DBConnection dest = new SQLCopy.MSSQL2005_DBConnection("FGA_RW");
            
            using (var reader = new CsvReader(new StreamReader(new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)), true, '|'))
            {                
                CsvDataAdapter adapter = new CsvDataAdapter(reader);
                // TODO : ajouter un objet de mapping sur le csvDataApdapter
                int nbReadLines = adapter.Fill(dest, new DatabaseTable("dbo", "DATA_FACTSET"));
                
            }
        }



        [TestMethod]
        public void ACTION_PROCESS_BulkCopy_FACTSET_MODELE_CLASSIFICATION_2()
        {
            string date = "10/02/2014";
            //string filepath1 = @"C:\FGA_SOFT\DEVELOPPEMENT\PROJET\FGA_Soft_Front\Front\INPUT\ACTION\FACTSET\Modele_Classification.xlsx";
            string filepath2 = @"G:\,FGA Front Office\02_Gestion_Actions\00_BASE\Base 2.0\Modele_Classification.xlsx";

            
            DataSet ds = new DataSet("DATA_FACTSET");
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_RW");

            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(filepath2 , "Modele Classification", "B1", "V926", true);


            adapter.AddColumnMapping("DATA_FACTSET", "MXEU", "MXEU", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXUSLC", "MXUSLC", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXEM", "MXEM", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXEUM", "MXEUM", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "MXFR", "MXFR", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100001", "6100001", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100004", "6100004", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100030", "6100030", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100033", "6100033", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100063", "6100063", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "AVEURO", "AVEURO", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "AVEUROPE", "AVEUROPE", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100026", "6100026", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100062", "6100062", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100002", "6100002", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("DATA_FACTSET", "6100024", "6100024", System.Type.GetType("System.Double"));
            
            adapter.FillColumnWithValue("DATE", date);

            adapter.Fill(ds);

            DataTable dt = ds.Tables["DATA_FACTSET"];

            s.bulkcopyData(dt);

        }

        [TestMethod]
        public void ACTION_PROCESS_BulkCopy_IMPORT_ISR_3()
        {

            string filepath2 = @"G:\,FGA ISR\Notation Fédéris\NotationISRbase.xlsx";
            string filepath1 = @"G:\,FGA Soft\INPUT\ACTION\ISR\NotationISRbase.xlsx";
            DataSet ds = new DataSet("ISR_NOTE");
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_RW");

            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(filepath2, "ISR", "A2", "F1782", true);
            adapter.AddColumnMapping("ISR_NOTE", "Note Actions", "Note Actions", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("ISR_NOTE", "Note Credit", "Note Credit", System.Type.GetType("System.Double"));
            adapter.AddColumnMapping("ISR_NOTE", "date ", "DATE", System.Type.GetType("System.DateTime"));
            adapter.AddColumnMapping("ISR_NOTE", "Name", "NAME");

            adapter.Fill(ds);

            DataTable dt = ds.Tables["ISR_NOTE"];

            s.bulkcopyData(dt);            
        }


        [TestMethod]
        public void ACTION_PROCESS_BulkCopy_Import_ACT_PTF_BaseTitreDirects_4()
        {
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration("OMEGA", "FGA_RW");            

            string date = "10/02/2014";

            string filePath = @"C:\FGA_SOFT\DEVELOPPEMENT\PROJET\FGA_Soft_Front\Front\SQL_SCRIPTS\AUTOMATE\GESTION_ACTION\FCP_Action_BaseTitresDirects.sql";
            // la requete 
            string request;

            if (date != null)
            {
                request = parameterRequest(filePath, new string[] { "@dateDemandee" }, new string[] { "'" + date + "'" });
            }
            else
            {
                request = parameterRequest(filePath, new string[] { }, new string[] { });
            }

            s.bulkcopySourceRequest(request, new DatabaseTable("ACT_PTF"));


        }
        public static string parameterRequest(string fileName, string[] parameters, string[] values)
        {
            // lecture du fichier de requete
            string request = ReadFile(fileName);

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
            return request;
        }

        public static string ReadFile(string file)
        {
            try
            {
                using (StreamReader sr = File.OpenText(file))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.Write("Erreur: \nFichier texte: " + file + "introuvable", e);
            }
            return null;
        }

        [TestMethod]
        public void ACTION_PROCESS_BulkCopy_Calculate_5()
        {
            string procStock = @"ACT_DailyImport";
            SQLCopy.DBConnectionDelegate d = new SQLCopy.MSSQL2005_DBConnection("FGA_PREPROD_RW");
            DataSet ds = d.Execute("Execute " + procStock,connection_timeout: 60*60*60 );
        }

        // TODO: A TESTER 
        [TestMethod]
        public void ACTION_PROCESS_BulkCopy_FACTSET_TICKER_CONV()
        {
            string date = "17/01/2014";            
            string filepath1 = @"G:\,FGA Front Office\02_Gestion_Actions\00_BASE\Base 2.0\TickerConversion.xlsx";
            string filepath2 = @"C:\TickerConversion.xlsx";


            DataSet ds = new DataSet("ACT_TICKER_CONVERSION");
            SQLCopy.MSDBIntegration s = new SQLCopy.MSDBIntegration(destinationConnection: "FGA_PREPROD_RW");

            OpenXMLDataAdapter adapter = new OpenXMLDataAdapter(filepath2, "Feuil1", "B3", "F51", true);

            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "ISIN", "ISIN");
            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "TICKER", "TICKER");
            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "BBG", "BBG");
            adapter.AddColumnMapping("ACT_TICKER_CONVERSION", "EXCH_F", "EXCH_B");

            adapter.Fill(ds);

            DataTable dt = ds.Tables["ACT_TICKER_CONVERSION"];

            s.bulkcopyData(dt);

        }
    }
}
