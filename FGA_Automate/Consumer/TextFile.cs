using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Specialized;
using System.Data;

namespace FGA.Automate.Consumer
{
    /// <summary>
    /// classe produisant un fichier texte avec le contenu d une dataset: 
    /// 1 fichier par datable
    /// </summary>
    public class TextFile
    {
        /// <summary>
        /// Sortie d un Dataset , et pour chaque table incluse, pour chaque colonne, dans 1 fichier different
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="filePath">le chemin de sauvegarde</param>
        /// <param name="fileName">le nom de fichier sachant que l on ajoute la databaseNa&me et la columns</param>
        public static void WriteTo(DataSet ds, string filePath, string fileName, string fileExtension ="txt")
        {
            foreach (DataTable dt in ds.Tables)
            {
                WriteTo(dt, filePath, fileName + "_" + dt.TableName.ToString()+ "."+fileExtension );

            }
        }

        private static void WriteTo(DataTable dt, string filePath, string fileName)
        {
            foreach (DataColumn c in dt.Columns)
            {
                WriteTo(dt, c, filePath, fileName + "_" + c.ColumnName.ToString());

            }
        }

        private static void WriteTo(DataTable dt, DataColumn c, string filePath, string fileName)
        {
            StreamWriter myWriter = new StreamWriter(filePath + "\\" + fileName);
            try
            {

                foreach (DataRow dtRow in dt.Rows)
                {
                    // on all table's columns
                    var field1 = dtRow[c].ToString();
                    myWriter.WriteLine(field1.ToString());
                }
            }

            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Fatal("Impossible de créer le fichier :" + filePath + "\\" + fileName, e);
                throw e;
            }
            finally
            {
                myWriter.Close();
            }



        }


    }
}
