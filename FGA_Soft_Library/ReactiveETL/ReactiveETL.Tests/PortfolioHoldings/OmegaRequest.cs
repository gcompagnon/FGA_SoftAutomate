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

    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class OmegaRequest
    {
        #region Attributs de tests supplémentaires
        public OmegaRequest()
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
        public void ConnectionStringsTest()
        {
            System.Configuration.ConnectionStringSettings settings =
          System.Configuration.ConfigurationManager.ConnectionStrings["OMEGA_PROD"];

            if (null != settings)
            {
                // Retrieve the partial connection string.
                string connectString = settings.ConnectionString;
                Console.WriteLine("Original: {0}", connectString);

                // Create a new SqlConnectionStringBuilder based on the
                // partial connection string retrieved from the config file.
                System.Data.SqlClient.SqlConnectionStringBuilder builder =
                    new System.Data.SqlClient.SqlConnectionStringBuilder(connectString);

                builder.ConnectTimeout = 60;

                SqlConnection con = new SqlConnection(builder.ConnectionString);
                SqlCommand cmd = new SqlCommand("select top 10 * from com.produit", con);
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                int i = 0;
                while (rdr.Read())
                {
                    Console.WriteLine("Line {0}: {1}", i, rdr[i]);
                    i++;
                }

                //DataSet ds = new DataSet();
                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //da.Fill(ds);
                con.Close();
            }

        }

        [TestMethod]
        public void PortfolioOmegaHoldingETLTest()
        {
            string request = SQLRequest.SelectPortfolio.Replace("***", "18/04/2012").Replace("%%%", "'MN'");

            // classe dynamique generee par un fichier xml
            ClassBuilder cd = ClassBuilder.LoadFromXml(ROOT + "PortfolioOmega.fhw");
            Type recordClass = cd.CreateRecordClass();

            EtlFullResult result =
                Input.Query("OMEGA_PROD", request)
                 //               .Transform(UsersToPeopleActions.SplitUserName)
                .WriteFile("resultfileOMEGA_ETL.txt", recordClass)                
                //                .DbCommand("test", UsersToPeopleActions.WritePeople)
                //.Record()
                //.ConsoleCount("Omega Load")
                .Execute();

            analyse(result, recordClass);
        }


        /// <summary>
        /// Construit la liste des objets de donnees avec la reflection
        /// </summary>
        /// <param name="o"></param>
        /// <param name="recordClass"></param>
        /// <returns></returns>
        private static List<Object> analyse(EtlFullResult o, Type recordClass)
        {
            IEnumerable<Row> all = o.Data;
            List<Object> objects = new List<Object>();

            foreach (Row items in all)
            {
                string member = null;
                object instance = Activator.CreateInstance(recordClass);

                foreach (PropertyInfo info in GetProperties(instance))
                {
                    member = info.Name;
                    if (items.Contains(info.Name) && info.CanWrite)
                        info.SetValue(instance, items[info.Name], null);
                }
                foreach (FieldInfo info in GetFields(instance))
                {
                    member = info.Name;
                    if (items.Contains(info.Name))
                        info.SetValue(instance, items[info.Name]);
                }

                objects.Add(instance);
            }
            return objects;

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
