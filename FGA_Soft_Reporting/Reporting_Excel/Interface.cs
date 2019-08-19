using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Configuration;

using System.Text.RegularExpressions;

namespace Reporting_Excel
{
    using System.Data;
    using System.Data.SqlClient;


    public class Interface
    {
        public List<uint[]> lineStyles = new List<uint[]>();


        //Partie lecture de fichier----------------------------------------------------
        public void lectureFichier(Stylesheet ss, string filePath)
        {
            string line;
            List<uint> tmp = new List<uint>();

            StreamReader file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                //Si il s'agit d'une style de cellule, on l'ajoute en retenant le numero de ce style
                if (line.StartsWith("#cellule"))
                {
                    tmp.Add(toStyle(ss, line.Replace("#cellule ", "")));
                }
                //Quand on a un style de ligne, on l'ajoute avec son numero, styleNB
                if (line.StartsWith("#ligne"))
                {
                    //Ajoute la ligne de style
                    uint[] l = toLine(tmp, line.Replace("#ligne ", ""));
                    lineStyles.Add(l);
                    tmp = new List<uint>();
                }
            }
            file.Close();
        }

        public static uint toStyle(Stylesheet ss, string style)
        {
            string[] arg = style.Split(';');
            return XcelWin.creerStyle(ss, arg[0], Convert.ToInt32(arg[1]), Convert.ToBoolean(arg[2]), rgbToHexa(arg[3]), rgbToHexa(arg[4]), arg[5], arg[6], arg[7], arg[8], Convert.ToUInt32(arg[9]));
        }
        
        public static uint[] toLine(List<uint> style, string scheme)
        {
            string[] tmp = scheme.Split(';');
            uint[] res = new uint[tmp.Length];
            for (int i = 0; i < tmp.Length; i++)
            {
                int a = Convert.ToInt32(tmp[i]);
                res[i] = style[a - 1];
            }
            return res;
        }

        public static Dictionary<string, Object> lectureConfGraph(string filePath)
        {
            string line;
            Dictionary<string, Object> res = new Dictionary<string, Object>();

            StreamReader file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                //Si il s'agit d'une ligne de conf graph
                if (line.StartsWith("#graph"))
                {
                    string[] tmp = line.Split(':');
                    res.Add(tmp[0].Replace("#", ""), new ChartData(tmp[1].Split(';')[1], tmp[1].Split(';')[0], new List<int>(), tmp[1].Split(';')[3], tmp[1].Split(';')[2]));
                }
                if (line.StartsWith("#serie"))
                {
                    string[] tmp = line.Split(':');
                    res.Add(tmp[0].Replace("#", ""), new ChartData(tmp[1].Split(';')[1], tmp[1].Split(';')[0], new List<int>(), tmp[1].Split(';')[3], tmp[1].Split(';')[2]));
                }
                if (line.StartsWith("#meta"))
                {
                    string[] tmp = line.Split(':');
                    if(tmp[1].Split(';').Count()==4)
                        res.Add(tmp[0].Replace("#", ""), new MetaChartData(tmp[1].Split(';')[1], tmp[1].Split(';')[0], -1, tmp[1].Split(';')[3].Split(',').ToList(), Convert.ToBoolean(tmp[1].Split(';')[2])));
                    else
                        res.Add(tmp[0].Replace("#", ""), new MetaChartData(tmp[1].Split(';')[1], tmp[1].Split(';')[0], -1, tmp[1].Split(';')[2].Split(',').ToList()));
                }
                if (line.StartsWith("#tableau"))
                {
                    string[] tmp = line.Split(':');
                    res.Add(tmp[0].Replace("#", ""), new TableauData(Convert.ToInt32(tmp[1])-1));//-1 pour commencer l'indexation à 0 (et non à 1 pour les humains)
                }
            }
            file.Close();
            return res;
        }

        public static Dictionary<string, Object> razConfGraph(Dictionary<string, Object> d)
        {
            Dictionary<string, Object> res = new Dictionary<string, Object>();
            foreach (string key in d.Keys)
            {
                if (key.Contains("graph"))
                {
                    ChartData tmp = (ChartData)d[key];
                    res.Add(key, new ChartData(tmp.titre, tmp.nomModele, new List<int>(), tmp.colValeurs, tmp.colTitres));
                }
                else
                    res.Add(key, d[key]);
            }
            return res;
        }

        public static string rgbToHexa(string rgb)
        {
            if (rgb == "")
            {
                return "";
            }
            else
            {
                Match match = Regex.Match(rgb, @"R([0-9]{1,})G([0-9]{1,})B([0-9]{1,})");

                if (match.Success)
                {
                    string str = Convert.ToInt32(match.Groups[1].Value).ToString("X");
                    string str1 = Convert.ToInt32(match.Groups[2].Value).ToString("X");
                    string str2 = Convert.ToInt32(match.Groups[3].Value).ToString("X");
                    return str + str1 + str2;
                }
                else
                {
                    Console.WriteLine("Matching RGB impossible, couleur non prise en compte : " + rgb);
                    return "";
                }
            }
        }

        //Partie SQL------------------------------------------------------------------
        public string getCommande(string filename)
        {
            string resultat = "";
            //try
            //{
                // Création d'une instance de StreamReader pour permettre la lecture de notre fichier 
                StreamReader monStreamReader = new StreamReader(filename);
                string ligne = monStreamReader.ReadLine();

                // Lecture de toutes les lignes et affichage de chacune sur la page 
                while (ligne != null)
                {
                    resultat += ligne+"\n";
                    ligne = monStreamReader.ReadLine();
                }
                // Fermeture du StreamReader
                monStreamReader.Close();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Une erreur est survenue au cours de la lecture du fichier contenant la requette SQL ! Filename: "+filename);
            //    Console.WriteLine(ex.Message);
            //}
            return resultat;
        }

        public string setArgument(string commande, string variable, string valeur)
        {
            commande = commande.Replace( variable, valeur);
            return commande;
        }

        public string replaceArguments(string commande, List<string> args)
        {
            for (int i = 0; i < args.Count / 2; i++)
            {
                commande = setArgument(commande, args[2 * i], args[2 * i + 1]);
            }
            return commande;
        }

        //Pas utilisé
        public DataTable[] requeteSQL(string[] commande)
        {
            //Chaine de connexion à la BDD
            //string connectString = "Data Source=MEPAPP042_R;Initial Catalog=E2DBFGA01;Persist Security Info=True;User ID=E2FGATP;Password=E2FGATP25";
            ConnectionStringSettings connectString = ConfigurationManager.ConnectionStrings["Connection"];

            //Objet de connection à la BDD:
            SqlConnection cn = new SqlConnection(connectString.ToString());

            // Adapateur pour comprendre les SQL databases:
            SqlDataAdapter da ;//= new SqlDataAdapter(sCommande, cn);

            // DataTable pour récupérer le résultat de ma requette:
            DataTable[] dataTable =new DataTable[commande.Length];//= new DataTable();

            try
            {
                cn.Open();

                for (int i = 0; i < commande.Length; i++)
                {
                    try
                    {
                        string sCommande = commande[i];
                        dataTable[i] = new DataTable();

                        da = new SqlDataAdapter(sCommande, cn);
                       
                        // Fill the data table with select statement's query results:
                        int recordsAffected = da.Fill(dataTable[i]);

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
                        for (int j = 0; j < e.Errors.Count; j++)
                        {
                            msg += "Error #" + j + " Message: " + e.Errors[j].Message + "\n";
                        }
                        System.Console.WriteLine(msg);
                    }
                }
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
        
        public DataSet multiRequeteSQL(string requete,string bdd)
        {
            //Chaine de connexion à la BDD
            ConnectionStringSettings cs;
            if(bdd=="omega")
                cs= ConfigurationManager.ConnectionStrings["omega"];
            else
                cs = ConfigurationManager.ConnectionStrings["Connection"];

            //Objet de connection à la BDD:
            SqlConnection sqlConnection = new SqlConnection(cs.ToString());

            SqlDataAdapter sqlDa = new SqlDataAdapter();
            SqlCommand selectCmd = new SqlCommand();
            selectCmd.CommandTimeout = 90;
            selectCmd.CommandText = requete;
            selectCmd.CommandType = CommandType.Text;
            selectCmd.Connection = sqlConnection;
            sqlDa.SelectCommand = selectCmd;

            // DataSet1 is a strongly typed DataSet
            DataSet ds = new DataSet();

            sqlConnection.Open();

            int recordsAffected = sqlDa.Fill(ds);
            if (recordsAffected > 0)
            {
                Console.WriteLine("Commande récupérée avec succes");
            }
            else if (recordsAffected == 0)
            {
                Console.WriteLine("Table vide");
            }


            sqlConnection.Close();

            return ds;
        }

    }
}
