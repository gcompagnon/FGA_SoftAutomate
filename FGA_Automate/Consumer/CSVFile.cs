using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using FGA.Automate.Helpers;

namespace FGA.Automate.Consumer
{
    public class CSVFile
    {
        /// <summary>
        /// Sortie d un Dataset , et de la premiere table incluse, dans 1 fichier 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="filePath">le chemin de sauvegarde</param>
        /// <param name="indexTable">index of the Table of DS to be written</param>
        public static void WriteToCSV(DataSet ds, string filePath, int indexTable = 0)
        {
            DataTable dt = ds.Tables[indexTable];
            CreateCSVFile(dt, filePath);
        }
        /// <summary>
        /// Sortie d un Dataset , et pour chaque table incluse, dans 1 fichier differents
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="filePath">le chemin de sauvegarde</param>
        /// <param name="fileNames">les noms des fichiers separes par des ';' </param>
        public static void WriteToCSV(DataSet ds, string filePath, string fileNames)
        {
            //Obtention de tous les noms de fichiers et verification de l'extension .CSV
            List<string> names = fileNames.Split(';').ToList();
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i].Length == 0)
                    names.RemoveAt(i);
                else if (!names[i].EndsWith(".csv"))
                    names[i] = names[i] + ".csv";
            }

            //Ajout de noms par défault si il en manque
            if (names.Count != ds.Tables.Count)
            {
                for (int i = names.Count; i < ds.Tables.Count; i++)
                {
                    names.Add(ds.Tables[i].TableName+"_" + i + ".csv");
                }
            }

            //Verification de la fin du chemin par un \
            if (!filePath.EndsWith(@"\"))
                filePath = filePath + @"\";

            int j = 0;
            foreach (DataTable dt in ds.Tables)
            {                
                //FileHelpers.CommonEngine.DataTableToCsv(dt, filePath + names[j],options);
                CreateCSVFile(dt, filePath + names[j]);
                j++;
            }
        }


        /// <summary>
        /// INUTILISE 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strFilePath"></param>
        private static void CreateCSVFile(DataTable dt, string strFilePath)
        {
            // Crée le fichier csv dans lequel le datatable sera exporté            
            using (StreamWriter fs = new StreamWriter(strFilePath, false, Encoding.Default))
            {
                // D'abord écriture des titres de colonnes
                int iColCount = dt.Columns.Count;
                for (int i = 0; i < iColCount; i++)
                {
                    if (i > 0)
                        fs.Write(';');
                    fs.Write(dt.Columns[i]);
                }
                fs.Write(Environment.NewLine);

                //Ensuite écriture des lignes
                foreach (DataRow dr in dt.Rows)
                {
                    object[] fields = dr.ItemArray;

                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (i > 0)
                            fs.Write(';');

                        fs.Write(Helper.ValueToString(fields[i]));
                    }
                    fs.Write(Environment.NewLine);
                }
                fs.Close();
            }
        }

      
    }
}
