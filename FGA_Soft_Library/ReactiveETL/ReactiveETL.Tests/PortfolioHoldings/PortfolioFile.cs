using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileHelpers;
using System.IO;
using FileHelpers.RunTime;
using System.Data;
using System.Reflection;
using FileHelpers.DataLink;
using System.Data.SqlClient;

namespace ReactiveETL.Tests.PortfolioHoldings
{
    //[DelimitedRecord(";"), IgnoreFirst(1), IgnoreEmptyLines(true)]
    //public class PortfolioHoldingFileReadPoco
    //{
    //    public string PortfolioHoldingsProvider;        //0 
    //    public string PortfolioCode;          //1
    //    [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
    //    public DateTime PortfolioCompoEndDate;          //12
    //    public string PortfolioCompoISIN;     //3
    //    public double PortfolioCompoWeight1;          //18
    //}



    /// <summary>
    /// Description résumée pour StatproIndexFile
    /// </summary>
    [TestClass]
    public class PortfolioHoldingsFile
    {
        #region Attributs de tests supplémentaires
        public PortfolioHoldingsFile()
        {
            //
            // TODO: ajoutez ici la logique du constructeur
            //
        }

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


        //
        // Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        // Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test de la classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilisez ClassCleanup pour exécuter du code une fois que tous les tests d'une classe ont été exécutés
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        const string ROOT = @"C:\FGA_SOFT\DEVELOPPEMENT\DATA\PORTFOLIO\";

        public static IDictionary<string, IDictionary<string, string>> config = null;




        [TestMethod]
        public void PortfolioHoldingFileTest()
        {
            Console.WriteLine("Portfolio file...");
            Console.WriteLine();

            string fileName = "PortfolioHolding_CLICHY_P.csv";

            // classe dynamique generee par un fichier xml
            ClassBuilder cd = ClassBuilder.LoadFromXml(ROOT + "PortfolioHolding.fhw");
            Type recordClass = cd.CreateRecordClass();

            FileHelperEngine engine = new FileHelperEngine(recordClass);
            // lecture du contenu du fichier
            Object[] o = engine.ReadFile(ROOT + fileName);

            analyse(o, recordClass);

            Console.WriteLine("Data successful written !!!");
            Console.ReadLine();
        }

        [TestMethod]
        public void PortfolioHoldingETLTest()
        {
            string fileName = "PortfolioHolding_CLICHY_P.csv";

            // classe dynamique generee par un fichier xml
            ClassBuilder cd = ClassBuilder.LoadFromXml(ROOT + "PortfolioHolding.fhw");
            Type recordClass = cd.CreateRecordClass();

            using (System.IO.StreamReader file = new StreamReader(ROOT + fileName))
            {
                // string content = file.ReadToEnd();
                file.BaseStream.Position = 1;
                var filecontent = Input
                    .ReadFile(file, recordClass)
                    .WriteFile("resultfilePH_ETL.txt", recordClass)
                    .Execute();
            }
        }


        [TestMethod]
        public void PortfolioHoldingExcelTest()
        {
            Console.WriteLine("Portfolio excelfile...");
            Console.WriteLine();
            string fileName = "PortfolioHolding_CLICHY_P.xlsx";

            // classe dynamique generee par un fichier xml
            ClassBuilder cd = ClassBuilder.LoadFromXml(ROOT + "PortfolioHolding.fhw");
            Type recordClass = cd.CreateRecordClass();

            ExcelStorage provider = new ExcelStorage(recordClass);

            provider.StartRow = 2;
            provider.StartColumn = 1;

            provider.FileName = ROOT + fileName;

            Object[] o = provider.ExtractRecords();
            analyse(o, recordClass);

            Console.WriteLine("Data successful written !!!");
            Console.ReadLine();

        }
        #region classes de données DTO
        public class Indextest
        {
            public string CompoISIN { get; set; }
            public decimal CompoWeight { get; set; }
        }

        static List<Indextest> allIndex = new List<Indextest>();
        #endregion


        /// <summary>
        /// test de la méthode utilisant Transform de 
        /// </summary>
        [TestMethod]
        public void TransformOperation()
        {

            string fileName = "PortfolioHolding_CLICHY_P.csv";

            // classe dynamique generee par un fichier xml
            ClassBuilder cd = ClassBuilder.LoadFromXml(ROOT + "PortfolioHolding.fhw");
            Type recordClass = cd.CreateRecordClass();


            using (System.IO.StreamReader file = new StreamReader(ROOT + fileName))
            {
                // string content = file.ReadToEnd();
                file.BaseStream.Position = 1;
                var filecontent = Input
                    .ReadFile(file, recordClass)
                    .Transform(

                    row =>
                    {
                        Indextest value = new Indextest();
                        value.CompoISIN = (string)row["PortfolioCompoISIN"];
                        value.CompoWeight = (Decimal)row["PortfolioCompoWeight1"];
                        allIndex.Add(value);
                        return row;
                    }

                    )
                    //                    .Mapper(recordClass, typeof(Indextest))
                    .Execute();
            }
        }


        private static void analyse(Object[] o, Type recordClass)
        {

            FieldInfo PortfolioHoldingsProviderFieldInfo = recordClass.GetField("PortfolioHoldingsProvider");
            FieldInfo PortfolioCompoEndDateFieldInfo = recordClass.GetField("PortfolioCompoEndDate");
            FieldInfo PortfolioCodeFieldInfo = recordClass.GetField("PortfolioCode");
            FieldInfo PortfolioLabelFieldInfo = recordClass.GetField("PortfolioLabel");
            FieldInfo PortfolioCompoISINFieldInfo = recordClass.GetField("PortfolioCompoISIN");
            FieldInfo PortfolioCompoLabelFieldInfo = recordClass.GetField("PortfolioCompoLabel");
            FieldInfo PortfolioCompoMaturityDateFieldInfo = recordClass.GetField("PortfolioCompoMaturityDate");
            FieldInfo PortfolioCompoIssuerFieldInfo = recordClass.GetField("PortfolioCompoIssuer");
            FieldInfo PortfolioCompoCurrencyFieldInfo = recordClass.GetField("PortfolioCompoCurrency");
            FieldInfo PortfolioCompoWeight1FieldInfo = recordClass.GetField("PortfolioCompoWeight1");
            FieldInfo PortfolioCompoWeight2FieldInfo = recordClass.GetField("PortfolioCompoWeight2");

            foreach (Object cli in o)
            {
                String PortfolioHoldingsProvider = (String)PortfolioHoldingsProviderFieldInfo.GetValue(cli);
                DateTime PortfolioCompoEndDate = (DateTime)PortfolioCompoEndDateFieldInfo.GetValue(cli);
                String PortfolioCode = (String)PortfolioCodeFieldInfo.GetValue(cli);
                String PortfolioLabel = (String)PortfolioLabelFieldInfo.GetValue(cli);
                String PortfolioCompoISIN = (String)PortfolioCompoISINFieldInfo.GetValue(cli);
                String PortfolioCompoLabel = (String)PortfolioCompoLabelFieldInfo.GetValue(cli);
                DateTime? PortfolioCompoMaturityDate = (DateTime?)PortfolioCompoMaturityDateFieldInfo.GetValue(cli);
                String PortfolioCompoIssuer = (String)PortfolioCompoIssuerFieldInfo.GetValue(cli);
                String PortfolioCompoCurrency = (String)PortfolioCompoCurrencyFieldInfo.GetValue(cli);
                Double PortfolioCompoWeight1 = (Double)PortfolioCompoWeight1FieldInfo.GetValue(cli);
                Double? PortfolioCompoWeight2 = (Double?)PortfolioCompoWeight2FieldInfo.GetValue(cli);
            }

            //DataTable dt = engine.ReadFileAsDT(ROOT+fileName);
            //    foreach (DataRowCollection cli in dt.Rows)
            //    {
            //        Console.WriteLine();
            //        Console.WriteLine("Portfolio: " + cli);
            //        Console.WriteLine();
            //        Console.WriteLine("-----------------------------");
            //    }


            Console.WriteLine("Writing data to a delimited file...");
            Console.WriteLine();

            //if (File.Exists("tempCN2.txt")) File.Delete("tempCN2.txt");
            // write the data to a file
            //engine.WriteFile(k+"_temp.txt", compo);
        }

        #region util Reflection

        static readonly Dictionary<Type, List<PropertyInfo>> propertiesCache = new Dictionary<Type, List<PropertyInfo>>();
        static readonly Dictionary<Type, List<FieldInfo>> fieldsCache = new Dictionary<Type, List<FieldInfo>>();
        /// <summary>
        /// retourne la liste 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static List<PropertyInfo> GetProperties(object obj)
        {
            List<PropertyInfo> properties;
            if (propertiesCache.TryGetValue(obj.GetType(), out properties))
                return properties;

            properties = new List<PropertyInfo>();
            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (property.CanRead == false || property.GetIndexParameters().Length > 0)
                    continue;
                properties.Add(property);
            }
            propertiesCache[obj.GetType()] = properties;
            return properties;
        }

        private static List<FieldInfo> GetFields(object obj)
        {
            List<FieldInfo> fields;
            if (fieldsCache.TryGetValue(obj.GetType(), out fields))
                return fields;

            fields = new List<FieldInfo>();
            foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                fields.Add(fieldInfo);
            }
            fieldsCache[obj.GetType()] = fields;
            return fields;
        }
        #endregion
    }

}
