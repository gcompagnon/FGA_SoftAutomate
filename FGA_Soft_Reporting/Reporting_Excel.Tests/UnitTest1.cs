using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using System.Data;



namespace Reporting_Excel.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestDemo()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\Demo\Demo.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'11/07/2012'");
            parametres.Add("@rapportCle");
            parametres.Add("'MonitoringGroupe'");
            parametres.Add("@cle_Montant");
            parametres.Add("'MonitoringGroupe'");
            parametres.Add("@rubriqueEncours");
            parametres.Add("'Encours'");
            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande, "");


            string[] sheetNames = { "RETRAITE A","ASSURANCE","A ou B" };
            string date = "11/07/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\Demo\TemplateDemo.xlsx";
            string saveDir = @"Demo.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\Demo\stylesDemo.txt";
            string graphDir = @"G:\UZX\Resources\Reporting\Demo\graphDemo.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, date, sheetNames);
        }


        [TestMethod]
        public void TestBaseMandats()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\sortie_Base_Mandats.sql";
            string commande = bdd.getCommande(requeteDir);
            DataSet ds = bdd.multiRequeteSQL(commande, "omega");



            string[] sheetNames = { "Mandats" };
            string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\TemplateMandats.xlsx";
            string saveDir = @"BaseMandats.xlsx";
            string styleDir = null;
            string graphDir = @"G:\UZX\Resources\Reporting\graphMandats.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);

            g2.execution(ds, date, sheetNames);
        }

        [TestMethod]
        public void TestMonitorGroupe()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\MonitoringGroupe\MonitoringGroupe.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'27/06/2012'");
            parametres.Add("@rapportCle");
            parametres.Add("'MonitoringGroupe'");
            parametres.Add("@cle_Montant");
            parametres.Add("'MonitoringGroupe'");
            parametres.Add("@rubriqueEncours");
            parametres.Add("'Encours'");

            string commande = bdd.getCommande(requeteDir);
            commande= bdd.replaceArguments(commande,parametres);

            DataSet ds = bdd.multiRequeteSQL(commande, "");



            string[] sheetNames = null;//{ "RETRAITE" };
            string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\MonitoringGroupe\Template.xlsx";
            string saveDir = @"MonitorGroupe.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\MonitoringGroupe\stylesMonitoringGroupe.txt";
            string graphDir = @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);

            g2.execution(ds, date, sheetNames);
        }

        [TestMethod]
        public void TestBDDI1()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E1.sql";

            string commande = bdd.getCommande(requeteDir);

            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Requete exterieure" };
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 1.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

        [TestMethod]
        public void TestBDDI2()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E2.sql";

            string commande = bdd.getCommande(requeteDir);

            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Requete exterieure" };
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 2.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

        [TestMethod]
        public void TestBDDI3()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E3.sql";

            string commande = bdd.getCommande(requeteDir);
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'30/03/2012'");
            commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Requete exterieure" };
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 3.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

        [TestMethod]
        public void TestBDDI6()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E6.sql";

            string commande = bdd.getCommande(requeteDir);

            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Resultat" };
            //string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 6.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

        [TestMethod]
        public void TestBDDI7()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E7.sql";

            List<string> parametres = new List<string>();
            parametres.Add("@datedebut");
            parametres.Add("'31/12/2011'");
            parametres.Add("@datefin");
            parametres.Add("'30/03/2012'");
            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Requete Exterieur" };
            //string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 7.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

        [TestMethod]//PARAMTRER LA DATE !
        public void TestBDDI8()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E8.sql";

            string commande = bdd.getCommande(requeteDir);

            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Requete Exterieur" };
            //string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 8.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }


        [TestMethod]
        public void TestBDDI10()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\BDDI\E10.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'30/12/2011'");

            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande, "omega");

            string[] sheetNames = { "Requete Exterieur" };
            string tmplt = @"G:\UZX\Resources\Reporting\BDDI\Template.xlsx";
            string saveDir = @"FGA - Enregistrement 10.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\BDDI\style.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

        [TestMethod]
        public void TestIRCEM()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\IRCEM\IRCEM.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'31/05/2012'");

            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);

            DataSet ds = bdd.multiRequeteSQL(commande, "");



            string[] sheetNames = null;//{ "RETRAITE" };
            string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\IRCEM\TemplateIRCEM.xlsx";
            string saveDir = @"IRCEM.xlsx";
            string styleDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\stylesMonitoringGroupe.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            //IRCEM i = new IRCEM(tmplt, saveDir);
            //i.execution2(ds);

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, date, sheetNames,true);
        }


        [TestMethod]
        public void TestTDBHebdo()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\TDBHebdo\Fisrt Query.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'04/07/2012'");

            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);

            DataSet ds = bdd.multiRequeteSQL(commande, "");



            //string[] sheetNames = null;//{ "RETRAITE" };
            //string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\TDBHebdo\TemplateTDBHebdo.xlsx";
            string saveDir = @"Tableau de Bord Hebdo.xlsx";
            //string styleDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\stylesMonitoringGroupe.txt";
            //string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            IRCEM i = new IRCEM(tmplt, saveDir);
            i.execution2(ds);

            //Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            //g2.execution(ds, date, sheetNames);
        }


        [TestMethod]
        public void TestMaturite()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\Maturite\ReportMaturiteSensi.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@dateInventaire");
            parametres.Add("'27/06/2012'");

            parametres.Add("@rapporCle");
            parametres.Add("'MonitoringMaturiteSe'");

            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);

            DataSet ds = bdd.multiRequeteSQL(commande, "");



            string[] sheetNames ={ "RETRAITE","ASSURANCE","EXTERNE","GLOBAL" };
            string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\Maturite\Template.xlsx";
            string saveDir = @"ReportingMaturiteSensi.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\Maturite\stylesMaturiteSensi.txt";
            string graphDir = @"G:\UZX\Resources\Reporting\Maturite\graphMaturiteSensi.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, date, sheetNames);
        }



        [TestMethod]
        public void TestPIIGS()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"G:\UZX\Resources\Reporting\PIIGS\ExpositionPaysPIIGS.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@date");
            parametres.Add("'27/06/2012'");

            parametres.Add("@rapportCle");
            parametres.Add("'MonitorExpoPays'");

            parametres.Add("@cle_Montant");
            parametres.Add("'MonitorExpoPays'");

            parametres.Add("@rubriqueEncours");
            parametres.Add("'TOTAL'");

            parametres.Add("@sousRubriqueEncours");
            parametres.Add("'TOTAL_HOLDING'");
            
            parametres.Add("@key");
            parametres.Add("'MonitorExpoPaysDur'");
            
            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);

            DataSet ds = bdd.multiRequeteSQL(commande, "");



            string[] sheetNames = { "ASSURANCE", "ASSUANCE_Duration" };
            string date = "27/06/2012";
            string tmplt = @"G:\UZX\Resources\Reporting\PIIGS\Template.xlsx";
            string saveDir = @"ReportingPIIGS.xlsx";
            string styleDir = @"G:\UZX\Resources\Reporting\PIIGS\stylesExpositionPaysPIIGS.txt";
            string graphDir = @"G:\UZX\Resources\Reporting\PIIGs\graphExpositionPays.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, date, sheetNames);
        }
        
        
        [TestMethod]
        public void TestStephane()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"\\vill1\Partage\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\Suivi_Histo_PIIGS.sql";


            string commande = bdd.getCommande(requeteDir);
            DataSet ds = bdd.multiRequeteSQL(commande, "");



            string[] sheetNames = { "F1", "F2" };
            string date = "xx/yy/2012";
            string tmplt = @"\\vill1\Partage\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\TemplateSuiviHistoPIIGS.xlsx";
            string saveDir = @"Suivi_Histo_Expo_Gpe_Emetteur-YYYYMMDD.xlsx";
            string styleDir = null;// @"G:\UZX\Resources\Reporting\PIIGS\stylesExpositionPaysPIIGS.txt";
            string graphDir = @"\\vill1\Partage\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\graphSuiviHistoPIIGS.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, date, sheetNames);
        }


        [TestMethod]
        public void TestCompteRating()
        {
            //Obtention du dataset
            Interface bdd = new Interface();
            string requeteDir = @"\\vill1\Partage\,FGA Soft\SQL_SCRIPTS\AUTOMATE\REPORTING\MONITORING\PRODUCTION_EXCEL\CompteRating.sql";
            List<string> parametres = new List<string>();
            parametres.Add("@comptes");
            parametres.Add("'3020010,3020011,3020012'");

            string commande = bdd.getCommande(requeteDir);
            commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande, "");

            string[] sheetNames = { "IRCEM" };
            string tmplt = @"\\vill1\Partage\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\TemplateMonitoringRating.xlsx";
            string saveDir = @"FGA - IRCEM COMPTES.xlsx";
            string styleDir = @"\\vill1\Partage\,FGA Soft\FGA_Automate\FGA_Automate_Batch\Resources\Reporting\stylesMonitoringRating.txt";
            string graphDir = null;// @"G:\UZX\Resources\Reporting\MonitoringGroupe\graphMonitoringGroupe.txt";

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);
            g2.execution(ds, "", sheetNames);
        }

    }
}
