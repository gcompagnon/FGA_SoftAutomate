using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FGA.Automate;

namespace FGA_Automate_TEST
{
    /// <summary>
    /// Description résumée pour UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest_Simple
    {
        public UnitTest_Simple()
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

        #region Attributs de tests supplémentaires
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

        [TestMethod]
        public void TestSQLExcel()
        {
            
            List<String> param = new List<string>();
            param.Add("-sql=\"C:\\FGA_SOFT\\DEVELOPPEMENT\\PROJET\\FGA_Soft_Front\\Front\\SQL_SCRIPTS\\AUTOMATE\\EXTERNE\\Prix_Historique_Produits_Devises.sql\"");
            param.Add(@"-ini=Resources\Config\login.ini");
            param.Add(@"-xls=Historique_INDICE_DEVISE.xls");
            param.Add("-@date_start='01/01/2012'");
            param.Add("-@date_end='01/02/2012'");
            param.Add("-@code_produit1='SBF120NET'");
            param.Add("-@code_produit2='SP500NET'");
            param.Add("-@code_devise1='USD'");
            param.Add(@"-nogui");
            param.Add(@"-csv=c:\");

            IntegratorBatch.Main(param.ToArray<string>());

        }

        /// <summary>
        /// test qui prend des parametres constants @param=value et
        /// aussi des parametres SQL: 
        /// le parametre prend comme valeur le resultat d une requete simple
        /// -paramsql
        /// </summary>
        [TestMethod]        
        public void TestSQLParamsDimXLS()
        {

            List<String> param = new List<string>();
            param.Add("-sql=\"C:\\FGA_SOFT\\DEVELOPPEMENT\\PROJET\\FGA_Soft_Front\\Front\\SQL_SCRIPTS\\AUTOMATE\\REPORTING\\EXTRACTION_JUMP\\Inventaire_Trans_JUMP.SQL\"");
            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-ini=Resources\Config\login.ini");
            param.Add(@"-xls=Compte_extraction_JUMP_20121130.xls");
            param.Add("-@dateInventaire='30/11/2012'");            
            param.Add("-paramsql=\"C:\\FGA_SOFT\\DEVELOPPEMENT\\PROJET\\FGA_Soft_Front\\Front\\SQL_SCRIPTS\\AUTOMATE\\REPORTING\\EXTRACTION_JUMP\\Liste_Comptes_JUMP.SQL\"");
            param.Add(@"-nogui");

            IntegratorBatch.Main(param.ToArray<string>());

        }
        /// <summary>
        /// test qui prend des parametres constants @param=value et
        /// aussi des parametres SQL: 
        /// le parametre prend comme valeur le resultat d une requete simple
        /// -paramsql
        /// </summary>
        [TestMethod]        
        public void TestSQLParamsDimCSV()
        {

            List<String> param = new List<string>();
            param.Add("-sql=\"C:\\FGA_SOFT\\DEVELOPPEMENT\\PROJET\\FGA_Soft_Front\\Front\\SQL_SCRIPTS\\AUTOMATE\\REPORTING\\EXTRACTION_JUMP\\Inventaire_Trans_JUMP.SQL\"");
            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-ini=Resources\Config\login.ini");
            param.Add(@"-#names=Compte_extraction_JUMP_20121130.csv");
            param.Add("-@dateInventaire='30/11/2012'");
            param.Add("-paramsql=\"C:\\FGA_SOFT\\DEVELOPPEMENT\\PROJET\\FGA_Soft_Front\\Front\\SQL_SCRIPTS\\AUTOMATE\\REPORTING\\EXTRACTION_JUMP\\Liste_Comptes_JUMP.SQL\"");
            param.Add(@"-csv=C:\JUMP");
            param.Add(@"-nogui");


            IntegratorBatch.Main(param.ToArray<string>());

        }
        
        [TestMethod]
        public void TestOpenXML()
        {

            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=FGA_PROD");
            param.Add("-sql=\".\\Requetes\\MonitoringGroupe.sql\"");
            param.Add(@"-ini=Resources\Config\login.ini");
            param.Add(@"-oxlsx=MonitoringGroupe_20120531.xlsx");
            param.Add(@"-@date='31/05/2012'");
            param.Add(@"-@rapportCle='MonitoringGroupe'");
            param.Add(@"-#date=31/05/2012");
            param.Add(@"-#names=RETRAITE;ASSURANCE;EXTERNE;GLOBAL");
            param.Add(@"-#style=Config\stylesMonitoringGroupe.txt");
            param.Add(@"-#template=Config\Template.xlsx");
            param.Add(@"-#graph=Config\graphMonitoringGroupe.txt");

            IntegratorBatch.Main(param.ToArray<string>());
        }


        [TestMethod]
        public void TestOpenXML_Resources()
        {

            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-sql=G:\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\MONITORING\PRODUCTION_EXCEL\MonitoringGroupe.sql");
            param.Add(@"-ini=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Config\login.ini");
            param.Add(@"-oxlsx=c:\20120627_MonitoringGroupe-YYYYMMDD.xlsx");
            param.Add(@"-@date='27/06/2012'");
            param.Add(@"-@rapportCle='MonitoringGroupe'");
            param.Add(@"-#date=27/06/2012");
            param.Add(@"-#names=RETRAITE;ASSURANCE;EXTERNE;GLOBAL");
            param.Add(@"-#style=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\stylesMonitoringGroupe.txt");
            param.Add(@"-#template=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\Template.xlsx");
            param.Add(@"-#graph=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\graphMonitoringGroupe.txt");
            
            IntegratorBatch.Main(param.ToArray<string>());
        }


        [TestMethod]
        public void TestOpenXML_Resources_Maturite()
        {
            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-sql=G:\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\MONITORING\PRODUCTION_EXCEL\MonitoringMaturite.sql");
            param.Add(@"-ini=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Config\login.ini");
            param.Add(@"-oxlsx=c:\20120627_MonitoringMaturiteSensi-YYYYMMDD.xlsx");
            param.Add(@"-@date='11/07/2012'");
            param.Add(@"-@rapportCle='MonitoringMaturiteSe'");
            param.Add(@"-#date=11/07/2012");
            param.Add(@"-#names=RETRAITE;ASSURANCE;EXTERNE;GLOBAL");
            param.Add(@"-#style=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\stylesMonitoringMaturiteDuration.txt");
            param.Add(@"-#template=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\TemplateMonitoringMaturite.xlsx");
            param.Add(@"-#graph=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\graphMonitoringMaturite.txt");

            IntegratorBatch.Main(param.ToArray<string>());
        }

        [TestMethod]
        public void TestOpenXML_Resources_PIIGS()
        {
            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-sql=G:\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\MONITORING\PRODUCTION_EXCEL\ExpositionPaysPIIGS.sql");
            param.Add(@"-ini=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Config\login.ini");
            param.Add(@"-oxlsx=c:\20120627_ExpositionPaysPIIGS-YYYYMMDD.xlsx");
            param.Add(@"-@date='27/06/2012'");
            param.Add("-@rapportCle='MonitorExpoPays'");
            param.Add("-@rapportKey='MonitorExpoPaysDur'");
            param.Add("-@cle_Montant='MonitorExpoPays'");
            param.Add("-@rubriqueEncours='TOTAL'");
            param.Add("-@sousRubriqueEncours='TOTAL_HOLDING'");
 
            param.Add(@"-#date=27/06/2012");
            param.Add(@"-#names=ASSURANCE;ASSURANCE_Duration");
            param.Add(@"-#style=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\stylesExpositionPaysPIIGS.txt");
            param.Add(@"-#template=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\TemplateExpoPays.xlsx");
            param.Add(@"-#graph=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\graphExpositionPays.txt");

            IntegratorBatch.Main(param.ToArray<string>());
        }


        [TestMethod]
        public void TestOpenXML_Resources_BaseMandats()
        {

            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=OMEGA");
            param.Add(@"-sql=G:\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\sortie_Base_Mandats.sql");
            param.Add(@"-ini=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Config\login.ini");
            param.Add(@"-oxlsx=c:\20120704_MonitoringGroupe-YYYYMMDD.xlsx");
            param.Add(@"-@dateExtraction='04/07/2012'");
            param.Add(@"-#date=04/07/2012");
            param.Add(@"-#names=MANDATS");
            param.Add(@"-#template=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\TemplateBaseMandats.xlsx");
            param.Add(@"-#graph=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\graphBaseMandats.txt");

            IntegratorBatch.Main(param.ToArray<string>());
        }

        [TestMethod]
        public void TestCSV()
        {
            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-csv=c:\");

            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-sql=G:\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\MONITORING\PRODUCTION_EXCEL\MonitoringGroupe.sql");
            param.Add(@"-ini=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Config\login.ini");
            param.Add(@"-@date='27/06/2012'");
            param.Add(@"-@rapportCle='MonitoringGroupe'");
            param.Add(@"-#names=ASSURANCE.csv;RETRAITE.csv;EXTERNE.csv;GLOBAL.csv");
            

            IntegratorBatch.Main(param.ToArray<string>());
        }

        [TestMethod]
        public void TestProductionDonnees()
        {
            List<String> param = new List<string>();
            param.Add("-sql=\"..\\SQL_SCRIPTS\\AUTOMATE\\EXTERNE\\Prix_Historique_Produits_Devises.sql\"");
            param.Add(@"-ini=Resources\Config\login.ini");
            param.Add(@"-xls=..\BASE\Historique_INDICE_DEVISE.xls");
            param.Add("-@date_start='01/01/2009'");
            param.Add("-@date_end='01/02/2009'");
            param.Add(@"-nogui");

            IntegratorBatch.Main(param.ToArray<string>());

        }



        //Si include et exclude sont vide : copie la BDD en entier
        //Si include et exclude non vide : aucune table ne sera copiée
        //Si include est vide : copie tout sauf les tables comprises dans exclude
        //Si exclude est vide : copie uniquement les tables dans include
        [TestMethod]
        public void TestDBCopy()
        {
            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=OMEGA");
            param.Add("-sql=\"c:\\FGA_SOFT\\DEVELOPPEMENT\\PROJET\\FGA_Soft_Front\\Front\\SQL_SCRIPTS\\AUTOMATE\\REPORTING\\ExpoPIIGS\\Recap_operations_YTD.sql\"");
            param.Add(@"-ini=Resources\Config\login.ini");

            param.Add(@"-dbCopy=true");
            param.Add(@"-#connection1=OMEGA");
            param.Add(@"-#connection2=ConnectionAdmin");
            param.Add(@"-#include=a");
            param.Add(@"-#exclude=b");
            param.Add(@"-#nomTable=TMP_RECAP_OP");


            IntegratorBatch.Main(param.ToArray<string>());

        }


        [TestMethod]
        public void TestIRCEM()
        {
            List<String> param = new List<string>();
            param.Add(@"-nogui");
            param.Add(@"-datastore=FGA_PROD");
            param.Add(@"-sql=G:\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\IRCEM\IRCEM.sql");
            param.Add(@"-ini=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Config\login.ini");
            param.Add(@"-oxlsx=c:\IRCEMM-YYYYMMDD.xlsx");
            param.Add(@"-@date='30/04/2012'");
            //param.Add("-@rapportCle='MonitorExpoPays'");
            //param.Add("-@rapportKey='MonitorExpoPaysDur'");
            //param.Add("-@cle_Montant='MonitorExpoPays'");
            //param.Add("-@rubriqueEncours='TOTAL'");
            //param.Add("-@sousRubriqueEncours='TOTAL_HOLDING'");

            param.Add(@"-#date=30/04/2012");
            param.Add(@"-#names=AAA;BBB;CCC");
            param.Add(@"-#monoSheetFlag=True");
            //param.Add(@"-#style=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\stylesExpositionPaysPIIGS.txt");
            param.Add(@"-#template=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\TemplateIRCEM.xlsx");
            //param.Add(@"-#graph=G:\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\graphExpositionPays.txt");

            IntegratorBatch.Main(param.ToArray<string>());
        }


    }
}
