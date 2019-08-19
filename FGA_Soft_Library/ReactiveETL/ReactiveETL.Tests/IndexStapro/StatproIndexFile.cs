using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileHelpers;
using System.IO;

namespace ReactiveETL.Tests.IndexStapro
{
    /// <summary>
    /// Le traitement des fichiers indices provenant de Statpro (EMTS et STOXX)
    /// </summary>
    [DelimitedRecord(";"), IgnoreFirst(2), IgnoreEmptyLines(true)]
    public class CN2FileReadPoco
    {
        public string IndexProvider;        //0
        public string IndexCode;          //1
        public string IndexLabel;         //2
        public string IndexCompoISIN;     //3
        public string IndexCompoCusip;    //4
        public string IndexCompoSedol;   //5
        public string IndexCompoTicker;   //6
        public string IndexCompoProviderId;   //7
        public string IndexCompoStatproISIN;  //8
        public string IndexCompoStatproCusip;  //9
        public string IndexCompoStatproSedol;  //10
        public string IndexCompoName;          //11
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime IndexCompoEndDate;          //12
        public double? IndexCompoMarketCap1;          //13
        public double? IndexCompoMarketCap2;          //14
        public double? IndexCompoMarketCap3;          //15
        public double? IndexCompoMarketCap4;          //16
        public double? IndexCompoMarketCap5;          //17
        public double? IndexCompoWeight1;          //18
        public double? IndexCompoWeight2;          //19
        public double? IndexCompoWeight3;          //20
        public double? IndexCompoWeight4;          //21
        public double? IndexCompoReturn1;          //22
        public double? IndexCompoReturn2;          //23
        public double? IndexCompoReturn3;          //24
        public double? IndexCompoReturn4;          //25
        public double? IndexCompoReturn5;          //26
        public double? IndexCompoPrice;          //27
        public string IndexCompoCurrency;          //28
        [FieldOptional()]
        public string IndexCode2;          //29
    }

    [DelimitedRecord(";"), IgnoreFirst(2), IgnoreEmptyLines(true)]
    public class RNSFileReadPoco
    {
        public string IndexProvider;        //0
        public string IndexCompoISIN;     //1
        public string IndexCompoCusip;    //2
        public string IndexCompoSedol;   //3
        public string IndexCompoTicker;   //4
        public string IndexCompoProviderId;   //5
        public string IndexCompoStatproISIN;  //6
        public string IndexCompoStatproCusip;  //7
        public string IndexCompoStatproSedol;  //8
        public string IndexCompoName;          //9
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime IndexCompoEndDate;     //10
        public double? IndexCompoEffectiveYield;     //11
        public double? IndexCompoEffectiveModifiedDuration;     //12
        public double? IndexCompoTimeToMaturity;     //13
        public double? IndexCompoRiskNumber1;     //14
        public double? IndexCompoRiskNumber2;     //15
        public double? IndexCompoRiskNumber3;     //16
        public double? IndexCompoRiskNumber4;     //17
        public double? IndexCompoRiskNumber5;     //18
        public double? IndexCompoRiskNumber6;     //19
        public double? IndexCompoRiskNumber7;     //20
        public double? IndexCompoRiskNumber8;     //21
        public double? IndexCompoRiskNumber9;     //22
        public double? IndexCompoRiskNumber10;     //23
    }

    [DelimitedRecord(";"), IgnoreFirst(2), IgnoreEmptyLines(true)]
    public class SERFileReadPoco
    {
        public string IndexProvider;        //0
        public string IndexCode;          //1
        public string IndexCode2;          //2
        public string IndexCode3;          //3
        public string IndexCode4;          //4
        public string IndexCode5;          //5
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime IndexCompoEndDate;     //6
        public double? IndexValue;     //7
        public double? IndexValue2;     //8
        public double? IndexValue3;     //9
        public double? IndexValue4;     //10
        public double? IndexValue5;     //11
        public double? IndexValue6;     //12
        public double? IndexValue7;     //13
        public double? IndexValue8;     //14
        public string IndexLabel;         //15
        [FieldOptional()]
        public string IndexLabel2;         //16
    }
    [DelimitedRecord(";"), IgnoreFirst(2), IgnoreEmptyLines(true)]
    public class SECFileReadPoco
    {
        public string IndexProvider;        //0
        public string IndexCompoISIN;     //1
        public string IndexCompoCusip;    //2
        public string IndexCompoSedol;   //3
        public string IndexCompoTicker;   //4
        public string IndexCompoProviderId;   //5
        public string IndexCompoStatproISIN;  //6
        public string IndexCompoStatproCusip;  //7
        public string IndexCompoStatproSedol;  //8
        public string IndexCompoName;          //9
        [FieldConverter(ConverterKind.Date, "yyyyMMdd")]
        public DateTime IndexCompoEndDate;     //10
        public string IndexCompoSector;          //11
        public string IndexCompoSectorDescription;          //12
        public string IndexCompoCountry;          //13
        public string IndexCompoCountryDescription;          //14
        public string IndexCompoCurrency;          //15
        public string IndexCompoCurrencyName;          //16
        public string IndexCompoRatingCode;          //17
        public string IndexCompoRatingName;          //18
        public string IndexCompoAssetType;          //19
        public string IndexCompoAssetTypeName;          //20
        public string IndexCompoBondCategory;          //21
        public string IndexCompoUserField1;          //22
        public string IndexCompoUserFieldDesc1;      //23
        public string IndexCompoUserField2;          //24
        public string IndexCompoUserFieldDesc2;          //25
        public string IndexCompoUserField3;          //26
        public string IndexCompoUserFieldDesc3;          //27
        public string IndexCompoUserField4;          //28
        public string IndexCompoUserFieldDesc4;          //29
        public string IndexCompoUserField5;          //30
        public string IndexCompoUserFieldDesc5;          //31
        public string IndexCompoUserField6;          //32
        public string IndexCompoUserFieldDesc6;          //33
        public string IndexCompoUserField7;          //34
        public string IndexCompoUserFieldDesc7;          //35
        public string IndexCompoUserField8;          //36
        public string IndexCompoUserFieldDesc8;          //37
        public string IndexCompoUserField9;          //38
        public string IndexCompoUserFieldDesc9;          //39
        public string IndexCompoUserField10;          //40
        public string IndexCompoUserFieldDesc10;          //41
        public string IndexCompoUserField11;          //42
        public string IndexCompoUserFieldDesc11;          //43
        public string IndexCompoUserField12;          //44
        public string IndexCompoUserFieldDesc12;          //45
        public string IndexCompoUserAssetType;          //46
        public string IndexCompoUserAssetTypeDesc;          //47
        public string IndexCompoUserCountry;          //48
        public string IndexCompoUserCountryDesc;          //49
        public string IndexCompoUserCurrency;          //50
        public string IndexCompoUserCurrencyDesc;          //51
        public string IndexCompoUserGICSector;          //52
        public string IndexCompoUserGICSectorDesc;          //53
        public string IndexCompoUserGICIndustryGrp;          //54
        public string IndexCompoUserGICIndustryGrpDesc;          //55
        public string IndexCompoUserGICIndustry;          //56
        public string IndexCompoUserGICIndustryDesc;          //57
        public string IndexCompoUserGICSubIndustry;          //58
        public string IndexCompoUserGICSubIndustryDesc;          //59
        public string IndexCompoUserICBIndustry;          //60
        public string IndexCompoUserICBIndustryDesc;          //61
        public string IndexCompoUserICBSuperSector;          //62
        public string IndexCompoUserICBSuperSectorDesc;          //63
        public string IndexCompoUserICBSector;          //64
        public string IndexCompoUserICBSectorDesc;          //65
        public string IndexCompoUserICBSubSector;          //66
        public string IndexCompoUserICBSubSectorDesc;          //67
        public string IndexCompoUserSize;          //68
        public string IndexCompoUserSizeDesc;          //69
        public string IndexCompoUserMaturitySector;          //70
        public string IndexCompoUserMaturitySectorDesc;          //71
        public string IndexCompoUserMoodysRating;          //72
        public string IndexCompoUserMoodysRatingDesc;          //73
        public string IndexCompoUserRating;          //74
        public string IndexCompoUserRatingDesc;          //75
    }


    /// <summary>
    /// Description résumée pour StatproIndexFile
    /// </summary>
    [TestClass]
    public class StatproIndexFile
    {
        #region Attributs de tests supplémentaires
        public StatproIndexFile()
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

        [ClassInitialize()]
        public static void ConfigInitialize(TestContext testContext)
        {
            config = new Dictionary<string, IDictionary<string, string>>();
            // Test CN2
            IDictionary<string, string> configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120501");
            configI.Add("indexFamily", "EMTS");
            configI.Add("file", "EMTS.cn2");
            configI.Add("type", "CN2FileReadPoco");
            config.Add("CN2_EMTS", configI);

            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120501");
            configI.Add("indexFamily", "EMTS");
            configI.Add("file", "EMTS.rns");
            configI.Add("type", "RNSFileReadPoco");
            config.Add("RNS_EMTS", configI);

            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120501");
            configI.Add("indexFamily", "EMTS");
            configI.Add("file", "EMTS.ser");
            configI.Add("type", "SERFileReadPoco");
            config.Add("SER_EMTS", configI);

            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120501");
            configI.Add("indexFamily", "EMTS");
            configI.Add("file", "SECURITY.sec");
            configI.Add("type", "SECFileReadPoco");
            config.Add("SEC_EMTS", configI);
            //--------------------------------------------
            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120502");
            configI.Add("indexFamily", "STOX");
            configI.Add("file", "STOX.cn2");
            configI.Add("type", "CN2FileReadPoco");
            config.Add("CN2_STOX", configI);

            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120502");
            configI.Add("indexFamily", "STOX");
            configI.Add("file", "STOX.rns");
            configI.Add("type", "RNSFileReadPoco");
            config.Add("RNS_STOX", configI);

            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "04302012");
            configI.Add("dateImportation", "20120502");
            configI.Add("indexFamily", "STOX");
            configI.Add("file", "STOX.ser");
            configI.Add("type", "SERFileReadPoco");
            config.Add("SER_STOX", configI);

            configI = new Dictionary<string, string>();
            configI.Add("dateMD", "SECURITY");
            configI.Add("dateImportation", "20120502");
            configI.Add("indexFamily", "STOX");
            configI.Add("file", "SECURITY.sec");
            configI.Add("type", "SECFileReadPoco");
            config.Add("SEC_STOX", configI);
        }

        const string ROOT = @"C:\FGA_SOFT\DEVELOPPEMENT\DATA\STATPRO\IMPORT201201\";

        public static IDictionary<string, IDictionary<string, string>> config = null;


        private string FilePath(string cle)
        {
            IDictionary<string, string> c = config[cle];
            string relativePath;
            if( cle.Substring(0, 3).Equals("SEC"))
                relativePath = c["dateImportation"] + "\\Renamed\\" + c["indexFamily"] + "\\SECURITY\\" + c["file"];
            else
                relativePath = c["dateImportation"] + "\\Renamed\\" + c["indexFamily"] + "\\" + c["dateMD"] + "\\" + c["file"];
            return ROOT + relativePath;
        }

        /// <summary>
        /// retourne le type de la classe objet 
        /// </summary>
        /// <param name="cle"></param>
        /// <returns></returns>
        private Type ETLPoco(string cle)
        {
            IDictionary<string, string> c = config[cle];
            return System.Reflection.Emit.TypeBuilder.GetType(c["type"]);
        }


        [TestMethod]
        public void StatproIndexFileTest()
        {
            Console.WriteLine("CN2 Statpro file...");
            Console.WriteLine();

            string[] cle = { "CN2_EMTS", "SER_EMTS", "RNS_EMTS", "SEC_EMTS", "CN2_STOX", "SER_STOX", "RNS_STOX", "SEC_STOX" };

            foreach (string k in cle)
            {
                string filePath = FilePath(k);
//                Assert.IsTrue(File.Exists(filePath));

               
                Object[] compo = null;

                if (k.Substring(0, 3).Equals("CN2"))
                {
                    FileHelperEngine engine = new FileHelperEngine(typeof(CN2FileReadPoco));
                    compo = (CN2FileReadPoco[])engine.ReadFile(filePath);                    

                    foreach (CN2FileReadPoco cli in compo)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Indice: " + cli.IndexCompoName.ToString() + " - " + cli.IndexCompoISIN);
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------");
                    }
                }
                else if (k.Substring(0, 3).Equals("RNS"))
                {
                    FileHelperEngine engine = new FileHelperEngine(typeof(RNSFileReadPoco));
                    compo = (RNSFileReadPoco[])engine.ReadFile(filePath);

                    foreach (RNSFileReadPoco cli in compo)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Indice: " + cli.IndexCompoName.ToString() + " - " + cli.IndexCompoISIN);
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------");
                    }
                }
                else if (k.Substring(0, 3).Equals("SEC"))
                {
                    FileHelperEngine engine = new FileHelperEngine(typeof(SECFileReadPoco));
                    compo = (SECFileReadPoco[])engine.ReadFile(filePath);

                    foreach (SECFileReadPoco cli in compo)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Indice: " + cli.IndexCompoName.ToString() + " - " + cli.IndexCompoISIN);
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------");
                    }
                }
                else if (k.Substring(0, 3).Equals("SER"))
                {
                    FileHelperEngine engine = new FileHelperEngine(typeof(SERFileReadPoco));
                    compo = (SERFileReadPoco[])engine.ReadFile(filePath);

                    foreach (SERFileReadPoco cli in compo)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Indice: " + cli.IndexCode.ToString() + " - " + cli.IndexCompoEndDate);
                        Console.WriteLine();
                        Console.WriteLine("-----------------------------");
                    }
                }

                Console.ReadLine();
                Console.WriteLine("Writing data to a delimited file...");
                Console.WriteLine();

                //if (File.Exists("tempCN2.txt")) File.Delete("tempCN2.txt");
                // write the data to a file
                //engine.WriteFile(k+"_temp.txt", compo);
            }

            Console.WriteLine("Data successful written !!!");
            Console.ReadLine();


        }

        [TestMethod]
        public void StatproIndexFileETLTest()
        {
            string[] cle = { "CN2_EMTS", "SER_EMTS", "RNS_EMTS", "SEC_EMTS", "CN2_STOX", "SER_STOX", "RNS_STOX", "SEC_STOX" };

            foreach (string k in cle)
            {
                string filePath = FilePath(k);
//                Assert.IsTrue(File.Exists(filePath));

                if (k.Substring(0, 3).Equals("CN2"))
                {
                    using (System.IO.StreamReader file = new StreamReader(filePath))
                    {
                        string content = file.ReadToEnd();
                        file.BaseStream.Position = 0;
                        var filecontent = Input
                            .ReadFile<CN2FileReadPoco>(file)
                            .WriteFile<CN2FileReadPoco>("resultfileCN2.txt")
                            .Execute();
                        //Assert.IsTrue(File.Exists("resultfileCN2.txt"));
                        //Assert.IsTrue(filecontent.Count == 2);
                        //File.Delete("resultfileCN2.txt");
                    }
                }
                else if (k.Substring(0, 3).Equals("RNS"))
                {
                    using (System.IO.StreamReader file = new StreamReader(filePath))
                    {
                        string content = file.ReadToEnd();
                        file.BaseStream.Position = 0;
                        var filecontent = Input
                            .ReadFile<RNSFileReadPoco>(file)
                            .WriteFile<RNSFileReadPoco>("resultfileRNS.txt")
                            .Execute();
                        //Assert.IsTrue(File.Exists("resultfileRNS.txt"));
                        //Assert.IsTrue(filecontent.Count == 2);
                        //File.Delete("resultfileCN2.txt");
                    }
                }
                else if (k.Substring(0, 3).Equals("SEC"))
                {
                    using (System.IO.StreamReader file = new StreamReader(filePath))
                    {
                        string content = file.ReadToEnd();
                        file.BaseStream.Position = 0;
                        var filecontent = Input
                            .ReadFile<SECFileReadPoco>(file)
                            .WriteFile<SECFileReadPoco>("resultfileSEC.txt")
                            .Execute();
                        //Assert.IsTrue(File.Exists("resultfileSEC.txt"));
                        //Assert.IsTrue(filecontent.Count == 2);
                        //File.Delete("resultfileCN2.txt");
                    }
                }
                else if (k.Substring(0, 3).Equals("SER"))
                {
                    using (System.IO.StreamReader file = new StreamReader(filePath))
                    {
                        string content = file.ReadToEnd();
                        file.BaseStream.Position = 0;
                        var filecontent = Input
                            .ReadFile<SERFileReadPoco>(file)
                            .WriteFile<SERFileReadPoco>("resultfileSER.txt")
                            .Execute();
                        //Assert.IsTrue(File.Exists("resultfileSER.txt"));
                        //Assert.IsTrue(filecontent.Count == 2);
                        //File.Delete("resultfileCN2.txt");
                    }
                }

            }
        }
    }
}
