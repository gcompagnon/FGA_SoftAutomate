using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using FGA.Automate.Config;
using FGA.SQLCopy;

namespace FGA.Automate.Producer
{
    
    /// <summary>
    /// Classe representant une requete SQL
    /// Producer
    /// </summary>
    class SQLrequest
    {
        // les differentes bases de donnees 
        public enum DATASTORE { OMEGA, OMEGA_RECETTE, FGA_PROD, FGA_RECETTE };
        public static string[] DATASTORES = { "OMEGA", "OMEGA_RECETTE", "FGA_PROD", "FGA_RECETTE" };
        private string request = null;
        private string connection = null;
        private int connection_timeout = -1;

        // pour la configuration, les clés possibles sont parameterKey[i]+'_'+DATASTORE[j]: par exemple SERVEUR_BDD_FGA_PROD
        private enum keys { user = 0, password = 1, server = 2, name = 3, authentification = 4, timeout = 5, connection_string = 6 };
        private static string[] parameterKey = { "USER", "PASSWORD", "SERVEUR_BDD", "NAME_BDD", "WINDOWS_AUTHENTIFICATION", "CONNECT_TIMEOUT", "CONNECTION_STRING" };

        public string getConnectionString()
        {
            return connection;
        }


        /// <summary>
        /// Constructeur prenant le nom de la BDD en paramètre 
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// pour construire la chaine de connection
        /// </summary>
        /// <param name="dataStore">par défaut ce sera la base OMEGA voir SQLrequest.DATASTORE</param>
        private SQLrequest(string dataStore = "OMEGA")
        {
            string[] Keys = new string[parameterKey.Length];

            if (dataStore != null)
            {
                for (int i = 0; i < parameterKey.Length; i++)
                {
                    Keys[i] = parameterKey[i] + '_' + dataStore;
                }
            }

            // lecture du fichier de configuration
            IDictionary<string, string> m = InitFile.ReadConfigFile();

            string windows_auth = Keys[(int)keys.authentification];
            string server = Keys[(int)keys.server];
            string database = Keys[(int)keys.name];
            string uid = Keys[(int)keys.user];
            string pwd = Keys[(int)keys.password];
            string timeout = Keys[(int)keys.timeout];
            string connection_string = Keys[(int)keys.connection_string];

            if (m.ContainsKey(windows_auth) && m[windows_auth].Equals("TRUE"))
            {
                connection = "server = " + m[server] +
                            " ; Integrated Security = true" + // l authentification est lié au user connecté
                            " ; database = " + m[database];
            }
            else
            {
                connection = "server = " + m[server] +
                            " ; uid = " + m[uid] +
                            " ; pwd = " + m[pwd] +
                            " ; database = " + m[database];

            }
            if (m.ContainsKey(timeout))
            {
                try
                {
                    connection_timeout = Convert.ToInt32(m[timeout]);
                }
                catch (Exception e)
                {
                    IntegratorBatch.ExceptionLogger.Error("Impossible de convertir la valeur du Timeout: " + timeout + "=" + m[timeout], e);
                }
                
                connection += " ; Connect Timeout = " + m[timeout];
            }

            // utilisation tel quel de la connection string
            if (m.ContainsKey(connection_string))
            {
                connection = m[connection_string];
            }
            IntegratorBatch.InfoLogger.Debug("La connection sur la base utilisée est " + connection);
        }

        /// <summary>
        /// Constructeur prenant le chemin du fichier et le nom de la BDD en paramètres 
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// pour construire la chaine de connection
        /// </summary>
        /// <param name="fileName">le chemin du fichier contenant la requete</param>
        /// <param name="dataStore">par défaut ce sera la base OMEGA</param>
        public SQLrequest(string fileName, string dataStore = "OMEGA")
            : this(dataStore)
        {
            // lecture du fichier de requete
            request = InitFile.ReadFile(fileName);
        }

        /// <summary>
        /// Constructeur prenant le chemin du fichier en paramètre
        /// On appelle une initialisation des paramètres , lues dans un fichier -ini= ou login.ini
        /// </summary>
        /// <param name="fileName">le chemin du fichier contenant la requete</param>
        /// <param name="dataStore">par défaut ce sera la base OMEGA</param>
        /// <param name="parameters">liste des parametres @xxx=yyy</param>
        public SQLrequest(string fileName, string dataStore, string[] parameters, string[] values)
            : this(fileName, dataStore)
        {
            // on remplace le set @xxx=zzz de request par set @xxx=yyy passée en parametre
            for (int i = 0; i < parameters.Length; i++)
            {
                string pattern = @"set\s+" + parameters[i] + @"\s*=.*";                
                
                if(values[i] !=null && values[i].Trim().Length >0 )
                {
                    // gestion de la constante NOW 
                    if(values[i].Contains("NOW"))
                    {
                    values[i] = values[i].Replace("NOW","GetDate()");
                    }
                    string replacement = "set " + parameters[i] + "=" + values[i];
                    Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                    request = rgx.Replace(request, replacement);
                }
            }
        }


        /// <summary>
        /// Remplit la dataset donnée en paramètre avec le résultat de la requete
        /// sur la base source
        /// </summary>
        /// <param name="DS"></param>
        public void Execute(out DataSet DS)
        {
            IntegratorBatch.InfoLogger.Debug("La requete utilisée est \n" + request);
            DBConnectionDelegate mssql = new MSSQL2005_DBConnection(connection);

                SqlDataAdapter DA = new SqlDataAdapter(request, connection);
                if (connection_timeout > 0)
                {
                    DS = mssql.Execute(request, connection_timeout);
                }                
                else
                {
                     DS = mssql.Execute(request);
                }
            }

        }

}
