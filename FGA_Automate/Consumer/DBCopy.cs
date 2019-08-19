using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Data.SqlClient;


using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using FGA.Automate.Producer;
using System.Data;
using System.IO;
using FGA.SQLCopy;
using SQLCopy.Dbms;



namespace FGA.Automate.Consumer
{



    /// <summary>
    /// Adaptateur/Utilitaire pour declencher une copie complete de base à base
    /// </summary>
    public class DBCopy
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceConnexion"></param>
        /// <param name="destConnexion"></param>
        /// <param name="mode">Exclude, Include or Aucun</param>
        /// <param name="liste">Liste des tables lié à l'option Mode</param>
        public static void DataBaseCopy
            (string sourceConnexion, string destConnexion, ListeMode mode, List<string> liste)
        {
            MSDBIntegration sql = new MSDBIntegration(sourceConnexion, destConnexion);           
            sql.bulkcopyData(mode, liste.ToDatabaseTableList() );            
        }

                /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceConnexion"></param>
        /// <param name="destConnexion"></param>
        /// <param name="mode">Exclude, Include or Aucun</param>
        /// <param name="liste">Liste des tables lié à l'option Mode</param>
        /// <param name="ds">Contenu de la requete ou des requetes à mettre en plus dans les tables destination</param>
        /// <param name="nomTableDestination"></param>
        public static void DataBaseCopy
            (string sourceConnexion, string destConnexion, DataSet DataTables, List<string> nomTableDestination)
        {
            MSDBIntegration sql = new MSDBIntegration(sourceConnexion, destConnexion);
            if (DataTables.Tables.Count != nomTableDestination.Count)
                IntegratorBatch.ExceptionLogger.Error("Il manque des noms de table de destination, opération de copie du SELECT annulée");
            else
                for (int i = 0; i < DataTables.Tables.Count; i++)
                {
                    sql.bulkcopyData(DataTables.Tables[i], new DatabaseTable(nomTableDestination[i]));
                }    
        }
              

    }
}
