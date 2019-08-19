using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.IO;

using System.Xml.Linq;

namespace Reporting_Excel
{
    class Program
    {
        /*
      ////Obtention des parametres 
            string[] parametres = new string[(args.Length - 3) * 2];
            string date = "";
            string saveDir = args[0];
            string requeteDir = args[1];
            string styleDir = args[2];
            for (int i = 3; i < args.Length; i++)
            {
                string tmp = args[i].Replace("-", "");
                parametres[2 * (i - 3)] = tmp.Split('=')[0];
                parametres[2 * (i - 3) + 1] = tmp.Split('=')[1];
                if (parametres[2 * (i - 3)] == "@date")
                    date = parametres[2 * (i - 3) + 1].Replace("'", "");
            }
         * */

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

        public static void sousmain(string[] args)
        {
            ////Obtention des parametres 
            List<string> parametres = new List<string>();
            string[] sheetNames = null;
            string date = "";
            string saveDir = args[0];
            string requeteDir = args[1];
            string styleDir = args[2];
            string graphDir = args[3];
            for (int i = 4; i < args.Length; i++)
            {
                string tmp = args[i].Replace("-", "");
                if (tmp.Contains('@'))
                {
                    parametres.Add(tmp.Split('=')[0]);
                    parametres.Add(tmp.Split('=')[1]);
                    if (tmp.Contains("date"))
                        date = tmp.Split('=')[1].Replace("'", "");
                }
                else
                {
                    sheetNames = tmp.Split('=')[1].Split(';');
                }
            }


            if (parametres.Count % 2 != 0)
            {
                Console.WriteLine("Le nombre d'argument est incorect, passez le chemin de svgrde et les couple clef/valeur des cmdes SQL");
                Console.WriteLine();
            }
            else
            {
                Interface bdd = new Interface();


                string commande = bdd.getCommande(requeteDir);
                commande = bdd.replaceArguments(commande, parametres);
                DataSet ds = bdd.multiRequeteSQL(commande,"");

                Ultimate g2 = new Ultimate(@"Resources\Reporting\TemplateDemo.xlsx", saveDir, styleDir,graphDir);
                
                g2.execution(ds,date,sheetNames);



            }
        }

        public static void mainMandats()
        {
            Interface bdd = new Interface();

            string requeteDir = @"test\sortie_Base_Mandats.sql";
            List<string> parametres = new List<string>() { "@date", "'31/05/2012'" };


            string commande = bdd.getCommande(requeteDir);
            //commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande, "omega");
            string[] sheetNames = {"Mandats" };
            string date = "31/05/2012";
            string tmplt = @"test\tplt.xlsx";
            string saveDir = @"test\BaseMandats.xlsx";
            string styleDir = null;
            string graphDir = @"test\graphMandats.txt"; ;

            Ultimate g2 = new Ultimate(tmplt, saveDir, styleDir, graphDir);

            g2.execution(ds, date, sheetNames);
        }

        public static void mainIRCEM()
        {

            Interface bdd = new Interface();
            List<string> parametres = new List<string>(){ "@date", "'31/05/2012'" };


            string commande = bdd.getCommande(@"IRCEM\IRCEM.sql");
            
            commande = bdd.replaceArguments(commande, parametres);
            DataSet ds = bdd.multiRequeteSQL(commande,"");


            IRCEM ircem = new IRCEM(@"IRCEM\TemplateIRCEM.xlsx", @"IRCEM\Gen.xlsx");
            ircem.execution2(ds);
        }

        public static void mainTest() 
        {
            Test t = new Test(@"test\tplt.xlsx",@"test\source.xlsx",@"test\genere.xlsx");
            //t.exec();

            Interface bdd = new Interface();
            string commande = bdd.getCommande(@"test\sortie_Base_Mandats.sql");
            //commande = bdd.replaceArguments(commande, );
            DataSet ds = bdd.multiRequeteSQL(commande,"omega");



            t.exec(ds.Tables[0]);
        }

        public static void mainGraph()
        {
            Test t = new Test(@"Classeur1.xlsx", @"", @"GeNeRe.xlsx");
            t.exec(@"Feuil1!$F$7:$F$9", @"Feuil1!$D$7:$D$9");
        }


        public struct Data
        {
            public string nom;
            public int val;
            public int val2;
            public int val3;

            public Data(string nom2, int v1,int v2,int v3)
            {
                nom = nom2;
                val = v1;
                val2 = v2;
                val3 = v3;

            }
        }

        static void Main(string[] args)
        {


            //mainGraph();

            //sousmain(args);

            mainIRCEM();

            //mainTest();

            //mainMandats();





            //string col = "C";
            //int li = 3;
            //string formule = "=A2&\" \"&B2";
            //Console.WriteLine(formule);

            ////BLP(A2&\" \"&B2;C$1)


            //Ultimate.bloom(col, li, "BLP(A2&\" \"&B2,C$1)");
            
 

        }

    }
}
