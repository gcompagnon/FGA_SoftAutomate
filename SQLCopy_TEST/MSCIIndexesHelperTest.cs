using MSCIBarra_EquityIndex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Serialization;
using System.IO;
using SQLCopy.Helpers;
using System.Data;
using FGA.Automate.Consumer;

namespace SQLCopy_Test
{


    /// <summary>
    ///Classe de test pour MSCIIndexesHelperTest, destinée à contenir tous
    ///les tests unitaires MSCIIndexesHelperTest
    ///</summary>
    [TestClass()]
    public class MSCIIndexesHelperTest
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

        #region Attributs de tests supplémentaires


        // 
        //Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {




        }


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

        // -- pour les tests MSCI: parametrage des fichiers


        const string USED_DATE = "20131121";
        const string rootPath = @"G:\,FGA Systèmes\PRODUCTION\FTP\INDICES\MSCI\" + USED_DATE + @"\";

        const string PREFIX_DATE = USED_DATE + "_" + USED_DATE + "_";
        // core_dm_daily
        const string rootPath_DM = rootPath + USED_DATE + @"core_dm_daily_d";
        const string indexType_DM = "core_dm_daily";
        // core_eafe_daily
        const string rootPath_EAFE = rootPath + USED_DATE + @"core_eafe_daily_d";
        const string indexType_EAFE = "core_eafe_daily";
        // core_amer_daily
        string rootPath_AMER = rootPath + USED_DATE + @"core_amer_daily_d";
        string indexType_AMER = "core_amer_daily";

        // core_amer_daily
        const string rootPath_DM_ACE = rootPath + USED_DATE + @"core_dm_ace";
        const string indexType_DM_ACE = "core_dm_ace";

        /// <summary>
        /// List of constituents : with ISIN , prices ...
        /// </summary>
        [TestMethod]
        public void Test_MSCI_Indexes_Daily_Security_ZIPFile()
        {
            var x = new
            {
                indexType = indexType_DM,
                xmlPath = rootPath,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
                xmlDataFile = "CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml",
                zipDataFile = USED_DATE + @"core_dm_daily_d.zip"
            };


            MSCIBarra_EquityIndex.package_D15D p = (MSCIBarra_EquityIndex.package_D15D)this.Test_packageZIP(x.xmlPath, x.xsdClass, x.zipDataFile, PREFIX_DATE + x.xmlDataFile);
            DataSet DS = new DataSet();

            DS.Tables.Add(this.read(p));

            String excel = @"C:\YYYYMMDDMSCI_Sec.xls";
            DateTime now = DateTime.Now;
            excel = excel.Replace("YYYYMMDD", now.ToString("yyyyMMdd"));

            ExcelFile.CreateWorkbook(DS, excel);

        }

        private DataTable read(MSCIBarra_EquityIndex.package_D15D p)
        {
            DataTable DT = new DataTable("MSCI_D15D");



            string columnsDef = "DATE;ISIN;SEDOL;CODE_RIC;CODE_BLOOM;NOM;PAYS;DEVISE;SECTEUR;SOUSSECTEUR;COURS Ouverture;COURS Cloture;FLOTTANT;ACTIF_NET;ACTIF_NET_USD;PERF_PRICE;PERF_PRICE_USD;PERF_GROSS;PERF_GROSS_USD;PERF_NET;PERF_NET_USD;FACTEUR_AJUSTEMENT_PRIX";
            foreach (string c in columnsDef.Split(';'))
            {
                DT.Columns.Add(c);
            }

            //Console.WriteLine("ALL:  Nb of Securities: {0}", p.dataset_D15D.Count);
            foreach (MSCIBarra_EquityIndex.package_D15DEntry e in p.dataset_D15D)
            {
                DT.Rows.Add(String.Format("{0:dd/MM/yyyy}", e.calc_date), e.isin, e.sedol, e.RIC, e.bb_ticker,
                    e.security_name, e.ISO_country_symbol, e.price_ISO_currency_symbol, e.sector, e.sub_industry, e.price, e.price, e.closing_number_of_shares, e.initial_mkt_cap_loc_next_day,
                    e.initial_mkt_cap_usd_next_day,
                    e.price_return_loc, e.price_return_usd,
                    e.gross_return_loc, e.gross_return_usd,
                    e.net_return_intl_loc, e.net_return_intl_usd,
                    e.price_adjustment_factor);
                //Console.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", e.calc_date, e.security_name, e.isin, e.price, e.price_ISO_currency_symbol, e.price_return_loc, e.bb_ticker, e.industry, e.ISO_country_symbol, e.sector);                
            }
            return DT;
        }

        /// <summary>
        /// List of constituents : with ISIN , prices ...
        /// </summary>
        [TestMethod]
        public void Test_MSCI_Indexes_Daily_Security()
        {

            var x = new
           {
               indexType = indexType_DM,
               xmlPath = rootPath_DM,
               xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
               xmlDataFile = "CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml"
           };


            MSCIBarra_EquityIndex.package_D15D p = (MSCIBarra_EquityIndex.package_D15D)this.Test_package(x.xmlPath, x.xsdClass, PREFIX_DATE + x.xmlDataFile);

            Console.WriteLine("DATE;ISIN;SEDOL;CODE_RIC;CODE_BLOOM;NOM;PAYS;DEVISE;SECTEUR;SOUSSECTEUR;COURS Ouverture;COURS Cloture;FLOTTANT;ACTIF_NET;ACTIF_NET_USD;PERF_PRICE;PERF_PRICE_USD;PERF_GROSS;PERF_GROSS_USD;PERF_NET;PERF_NET_USD;FACTEUR_AJUSTEMENT_PRIX");
            //Console.WriteLine("ALL:  Nb of Securities: {0}", p.dataset_D15D.Count);
            foreach (MSCIBarra_EquityIndex.package_D15DEntry e in p.dataset_D15D)
            {
                //Console.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", e.calc_date, e.security_name, e.isin, e.price, e.price_ISO_currency_symbol, e.price_return_loc, e.bb_ticker, e.industry, e.ISO_country_symbol, e.sector);
                Console.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21}",
                    String.Format("{0:dd/MM/yyyy}", e.calc_date), e.isin, e.sedol, e.RIC, e.bb_ticker,
                    e.security_name, e.ISO_country_symbol, e.price_ISO_currency_symbol, e.sector, e.sub_industry, e.price, e.price, e.closing_number_of_shares, e.initial_mkt_cap_loc_next_day,
                    e.initial_mkt_cap_usd_next_day,

                    e.price_return_loc, e.price_return_usd,
                    e.gross_return_loc, e.gross_return_usd,
                    e.net_return_intl_loc, e.net_return_intl_usd,
                    e.price_adjustment_factor);

            }

        }
        [TestMethod]
        public void Test_MSCI_Indexes_Daily_Country_Weight()
        {
            var x = new
                        {
                            indexType = indexType_DM,
                            xmlPath = rootPath_DM,
                            xsdClass = typeof(MSCIBarra_EquityIndex.package_D80DCTY),
                            xmlDataFile = "CORE_DM_ALL_INDEX_WEIGHT_DAILY_D.xml"
                        };

            MSCIBarra_EquityIndex.package_D80DCTY p = (MSCIBarra_EquityIndex.package_D80DCTY)this.Test_package(x.xmlPath, x.xsdClass, PREFIX_DATE + x.xmlDataFile);

            Console.WriteLine("ALL: dataset_D80DCTYC . Nb of Lines: {0}", p.dataset_D80DCTYC.Count);
            foreach (MSCIBarra_EquityIndex.package_D80DCTYEntry e in p.dataset_D80DCTYC)
            {
                Console.WriteLine("{0};{1};{2};{3}", e.calc_date, e.country_name, e.ISO_country_symbol, e.bond_yield_name);
            }

            Console.WriteLine("ALL: dataset_D80DCTYI . Nb of Lines: {0}", p.dataset_D80DCTYI.Count);
            foreach (MSCIBarra_EquityIndex.package_D80DCTYEntry1 e in p.dataset_D80DCTYI)
            {
                Console.WriteLine("{0};{1};{2};{3}", e.calc_date, e.index_name, e.msci_index_code, e.region_code);
            }
            Console.WriteLine("ALL: dataset_D80DCTYW : Nb of Lines: {0}", p.dataset_D80DCTYW.Count);

            foreach (MSCIBarra_EquityIndex.package_D80DCTYEntry2 e in p.dataset_D80DCTYW)
            {
                Console.WriteLine("{0};{1};{2};{3}", e.calc_date, e.country_weight, e.msci_index_code, e.ISO_country_symbol);
            }

        }

        [TestMethod]
        public void Test_MSCI_Indexes_Daily_Index_Main()
        {


            var x = new
        {
            indexType = indexType_DM,
            xmlPath = rootPath_DM,
            xsdClass = typeof(MSCIBarra_EquityIndex.package_D51D),
            xmlDataFile = "CORE_DM_ALL_INDEX_MAIN_DAILY_D.xml"
        };

            MSCIBarra_EquityIndex.package_D51D p = (MSCIBarra_EquityIndex.package_D51D)this.Test_package(x.xmlPath, x.xsdClass, PREFIX_DATE + x.xmlDataFile);

            Console.WriteLine("ALL: Nb of Indexes: {0}", p.dataset_D51D.Count);
            foreach (MSCIBarra_EquityIndex.package_D51DEntry e in p.dataset_D51D)
            {
                Console.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7}", e.calc_date, e.msci_index_code, e.index_name, e.real_time_ric, e.real_time_ticker, e.ISO_country_symbol, e.region_code, e.industry);
            }
            //-----------------------------------------------------------------
            x = new
          {
              indexType = indexType_EAFE,
              xmlPath = rootPath_EAFE,
              xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D51F),
              xmlDataFile = "CORE_DM_EAFE_INDEX_MAIN_DAILY_D.xml"
          };
            MSCIBarra_EquityIndex.core_eafe_daily.package_D51F p1 = (MSCIBarra_EquityIndex.core_eafe_daily.package_D51F)this.Test_package(x.xmlPath, x.xsdClass, PREFIX_DATE + x.xmlDataFile);

            Console.WriteLine("EAFE : Nb of Indexes: {0}", p1.dataset_D51F.Count);
            foreach (MSCIBarra_EquityIndex.core_eafe_daily.package_D51FEntry e in p1.dataset_D51F)
            {
                Console.WriteLine("{0};{1};{2};{3};{4};{5};{6}", e.calc_date, e.msci_index_code, e.index_name, e.real_time_ric, e.real_time_ticker, e.ISO_country_symbol, e.region_code);
            }
            //-----------------------------------------------------------------
            x = new
            {
                indexType = indexType_AMER,
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D51A),
                xmlDataFile = "CORE_DM_AMER_INDEX_MAIN_DAILY_D.xml"
            };
            MSCIBarra_EquityIndex.core_amer_daily.package_D51A p2 = (MSCIBarra_EquityIndex.core_amer_daily.package_D51A)this.Test_package(x.xmlPath, x.xsdClass, PREFIX_DATE + x.xmlDataFile);

            Console.WriteLine("AMER: Nb of Indexes: {0}", p2.dataset_D51A.Count);
            foreach (MSCIBarra_EquityIndex.core_amer_daily.package_D51AEntry e in p2.dataset_D51A)
            {
                Console.WriteLine("{0};{1};{2};{3};{4};{5};{6}", e.calc_date, e.msci_index_code, e.index_name, e.real_time_ric, e.real_time_ticker, e.ISO_country_symbol, e.region_code);
            }


        }


        [TestMethod]
        public void Test_MSCI_Indexes_packages()
        {

            const string PREFIX_DATE = "20130918_20130918_";

            var xsdClasses = new[]
                {
            new
            {
                indexType = indexType_DM, 
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D_5D),
                xmlDataFile = "CORE_DM_ALL_COUNTRY_FXRATE_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_DM, 
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D51D), 
                xmlDataFile = "CORE_DM_ALL_INDEX_MAIN_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_DM,
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D80DCTY),
                xmlDataFile = "CORE_DM_ALL_INDEX_WEIGHT_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_DM,
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D60DDVD),
                xmlDataFile = "CORE_DM_ALL_SECURITY_ADVD_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_DM,
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D98D),
                xmlDataFile = "CORE_DM_ALL_SECURITY_CODE_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_DM,
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D16D),
                xmlDataFile = "CORE_DM_ALL_SECURITY_DTR_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_DM,
                xmlPath = rootPath_DM,
                xsdClass = typeof(MSCIBarra_EquityIndex.package_D15D),
                xmlDataFile = "CORE_DM_ALL_SECURITY_MAIN_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE, 
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D_5F),
                xmlDataFile = "CORE_DM_EAFE_COUNTRY_FXRATE_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE, 
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D51F), 
                xmlDataFile = "CORE_DM_EAFE_INDEX_MAIN_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE,
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D80FCTY),
                xmlDataFile = "CORE_DM_EAFE_INDEX_WEIGHT_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE,
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D60FDVD),
                xmlDataFile = "CORE_DM_EAFE_SECURITY_ADVD_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE,
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D98F),
                xmlDataFile = "CORE_DM_EAFE_SECURITY_CODE_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE,
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D16F),
                xmlDataFile = "CORE_DM_EAFE_SECURITY_DTR_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_EAFE,
                xmlPath = rootPath_EAFE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_eafe_daily.package_D15F),
                xmlDataFile = "CORE_DM_EAFE_SECURITY_MAIN_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER, 
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D_5A),
                xmlDataFile = "CORE_DM_AMER_COUNTRY_FXRATE_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER, 
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D51A), 
                xmlDataFile = "CORE_DM_AMER_INDEX_MAIN_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER,
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D80ACTY),
                xmlDataFile = "CORE_DM_AMER_INDEX_WEIGHT_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER,
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D60ADVD),
                xmlDataFile = "CORE_DM_AMER_SECURITY_ADVD_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER,
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D98A),
                xmlDataFile = "CORE_DM_AMER_SECURITY_CODE_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER,
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D16A),
                xmlDataFile = "CORE_DM_AMER_SECURITY_DTR_DAILY_D.xml"
            },
            new
            {
                indexType = indexType_AMER,
                xmlPath = rootPath_AMER,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_amer_daily.package_D15A),
                xmlDataFile = "CORE_DM_AMER_SECURITY_MAIN_DAILY_D.xml"
            },
                        new
            {
                indexType = indexType_DM_ACE ,
                xmlPath = rootPath_DM_ACE,
                xsdClass = typeof(MSCIBarra_EquityIndex.core_dm_ace.package_DM_ACE ),
                xmlDataFile = "CORE_DM_SECURITY_ACE_DAILY.xml"
            }
                };


            foreach (var x in xsdClasses)
            {
                this.Test_package(x.xmlPath, x.xsdClass, PREFIX_DATE + x.xmlDataFile);
            }
        }
        /// <summary>
        /// Methode pour lire l object dans le fichier XML 
        /// </summary>
        public Object Test_package(string rootPath, Type xsdClass, string xmlFile)
        {

            XmlSerializer x = new XmlSerializer(xsdClass);
            StreamReader reader = new StreamReader(rootPath + @"\" + xmlFile);
            // the class object has only one field : a List object
            Object datas = x.Deserialize(reader);
            Assert.AreEqual(datas.GetType(), xsdClass);
            return datas;
        }

        /// <summary>
        /// Methode pour lire l object dans le répertoire XML
        /// Impossible d utilisr WindowsBase System.IO.Packaging car ZIP n'est pas implémenté dans toutes ses versions
        /// </summary>
        public Object Test_packageZIP(string rootPath, Type xsdClass, string zipFile, string fileName)
        {
            XmlSerializer x = new XmlSerializer(xsdClass);

            ZipArchive package = new ZipArchive(rootPath + @"\" + zipFile, FileAccess.Read);


            foreach (ZipArchiveFile part in package.Files)
            {
                if (fileName.Equals(part.Name))
                {
                    StreamReader reader = part.OpenText();
                    // the class object has only one field : a List object
                    Object datas = x.Deserialize(reader);
                    Assert.AreEqual(datas.GetType(), xsdClass);
                    return datas;


                }
                Console.WriteLine("Extracted {0}", part.Name);

            }
            return null;

        }

        /// <summary>
        ///Test pour Serialize
        ///</summary>
        public void SerializeTestHelper<T>()
        {
            MSCIIndexesHelper<T> target = new MSCIIndexesHelper<T>(); // TODO: initialisez à une valeur appropriée
            string expected = string.Empty; // TODO: initialisez à une valeur appropriée
            string actual;
            actual = target.Serialize();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        [TestMethod()]
        public void SerializeTest()
        {
            SerializeTestHelper<GenericParameterHelper>();
        }
    }
}
