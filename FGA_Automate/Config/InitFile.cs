using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FGA.Automate.Config
{
    class InitFile
    {
        public static string loginIni = "login.ini";
        static string defautPath = "G:\\,FGA Soft\\";

        /// <summary>
        /// Recherche le nom de fichier passé en parametre dans les arborescences classiques
        /// </summary>
        /// <param name=fileName> nom du fichier sans chemin</param>
        ///<returns> Chemin complet vers le fichier</returns>
        public static string SearchForFile(string fileName)
        {
            string basePath1 = Directory.GetCurrentDirectory() + "\\";
            string basePath2 = Directory.GetCurrentDirectory() + "\\Resources\\";
            string basePath3 = System.AppDomain.CurrentDomain.BaseDirectory + "\\";
            string basePath4 = System.AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\";
            string path = fileName;
            if (File.Exists(path)) { return path; }
            // recherche parmi les repertoires suivants
            foreach (string p in new string[] { basePath1, basePath2, basePath3, basePath4, defautPath })
            {
                path = p + fileName;
                if (File.Exists(path)) { return path; }

                path = p + "CONFIGURATION\\" + fileName;
                if (File.Exists(path)) { return path; }

                path = p + "CONFIG\\" + fileName;
                if (File.Exists(path)) { return path; }

                path = p + "INPUT\\CONFIGURATION\\" + fileName;
                if (File.Exists(path)) { return path; }
            }
            //log error : fichier de conf introuvable
            throw new FileNotFoundException(fileName + " fichier introuvable dans les répertoires " + basePath1 + ", " + basePath3 + ", " + defautPath);
        }

        /// <summary>
        /// Charge la configuration, paramètres stockées dans un fichier login.ini
        /// </summary>
        ///<returns>la hashtable contenant les couples nom = valeur</returns>
        public static IDictionary<string, string> ReadConfigFile()
        {
            return ReadConfigFile(loginIni);
        }
        /// <summary>
        /// Charge la configuration, paramètres stockées dans un fichier
        /// </summary>
        /// <param name=file> nom du fichier sans chemin</param>
        ///<returns>la hashtable contenant les couples nom = valeur</returns>
        public static IDictionary<string, string> ReadConfigFile(string file)
        {
            // si null prendre la valeur par defaut
            file = file ?? loginIni;

            IDictionary<string, string> loginConfig = new Dictionary<string, string>();

            try
            {
                string path = SearchForFile(file);
                using (StreamReader sr = File.OpenText(path))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if ((s.IndexOf('#') == -1) || ((s = s.Substring(0, s.IndexOf('#'))) != string.Empty))
                        {
                            int i = s.IndexOf('=');
                            if (i >= 0)
                            {
                                string key = s.Substring(0, i);
                                string value = s.Substring(i + 1);
                                loginConfig.Add(key.Trim(), value.Trim());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Error("Erreur: \nFichier texte: " + file + "introuvable", e);
            }
            return loginConfig;
        }

        ///<summary>
        ///Lecture du contenu du fichier texte
        ///</summary>
        ///<param name=fileName> nom du fichier sans chemin</param>
        ///<returns>contenu du fichier</returns>
        public static string ReadFile(string file)
        {
            try
            {
                string path = SearchForFile(file);
                using (StreamReader sr = File.OpenText(path))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Error("Erreur: \nFichier texte: " + file + "introuvable", e);
            }
            return null;
        }

    }
}
