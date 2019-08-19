using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace SQLLauncher.producer
{
    /// <summary>
    /// Classe representant une requete SQL
    /// Producer
    /// </summary>
    class SQLrequest
    {
        private string request = null;
        private string connection = null;

        private SQLrequest()
        {
            // lecture du fichier de configuration
            IDictionary<string, string> m = config.InitFile.readConfigFile();
            if( m.ContainsKey("WINDOWS_AUTHENTIFICATION") && m["WINDOWS_AUTHENTIFICATION"].Equals("TRUE") ){
                connection = "server = " + m["SERVEUR_BDD_OMEGA"] +
                            " ; Integrated Security = true" +
                            " ; database = " + m["NAME_BDD_OMEGA"];
            }else {
                connection = "server = " + m["SERVEUR_BDD_OMEGA"] +
                            " ; uid = " + m["USER_OMEGA"] +
                            " ; pwd = " + m["PASSWORD_OMEGA"] +
                            " ; database = " + m["NAME_BDD_OMEGA"];
            }
            SQLextract.InfoLogger.Debug("La connection sur la base utilisée est " + connection);
        }

        /// <summary>
        /// Constructeur prenant le chemin du fichier en paramètre
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// </summary>
        /// <param name="name">le chemin du fichier contenant la requete</param>
        public SQLrequest(string name):this()
        {
            request = config.InitFile.readFile(name);            
        }

        /// <summary>
        /// Remplit la dataset donnée en paramètre avec le résultat de la requete
        /// </summary>
        /// <param name="DS"></param>
        public void execute(DataSet DS)
        {
            SQLextract.InfoLogger.Debug("La requete utilisée est " + request);
            try
            {
                IDbDataAdapter DA = new SqlDataAdapter(request, connection);
                DA.Fill(DS);
            }
            catch (Exception e)
            {
                SQLextract.ExceptionLogger.Fatal("Impossible d executer la requete: "+request, e);
                throw e;
            }
        }
    }
}
