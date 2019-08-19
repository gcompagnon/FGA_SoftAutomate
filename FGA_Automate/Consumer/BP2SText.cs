using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Data.SqlTypes;
using FGA.Automate.Config;
using FGA.Automate;

namespace FGA.Automate.Consumer
{
    /// <summary>
    /// classe produisant le fichier à destination de BP2S
    /// une validation de chaque champ est possible
    /// </summary>
    class BP2SText
    {

        private IDictionary<string,string> validationFields = null;
        private string validationPath;
        private NumberFormatInfo nfi ;


        public BP2SText()
        {
            nfi = new NumberFormatInfo();
            nfi.CurrencyDecimalSeparator = ".";
            nfi.NumberDecimalSeparator = ".";
        }

        /// <summary>
        /// creation d un fichier au format de transmission BP2S (instrument OTC)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="path"></param>
        /// <param name="date"></param>
        public void CreateFile(DataSet ds, String path, DateTime date)
        {
            StringBuilder dataString = new StringBuilder();
            StringBuilder lineString = new StringBuilder();
            int nbColumns = 0;
            int nbRows = 0; // nb de lignes de données
            foreach(DataTable table in ds.Tables)
            {
                //dataString.Append( table.TableName.ToString() );
                // la ligne de spec:
                foreach (DataColumn column in table.Columns)
                {
                    nbColumns++;
                    dataString.Append(column.ToString());
                    if (nbColumns < table.Columns.Count)
                    {
                        dataString.Append(',');
                    }
                }
                // ecriture des lignes sur les produits
                dataString.AppendLine();
                foreach (DataRow row in table.Rows)
                {
                    nbRows++;
                    nbColumns = 0;
                    foreach (DataColumn column in table.Columns)
                    {
                        nbColumns++;
                        string fieldString;
                        Object field = row[column];

                        if (field.GetType() == typeof(DateTime)
                            || field.GetType() == typeof(SqlDateTime))
                        {
                            fieldString = ((DateTime)field).ToString("yyyy-MM-dd");
                        }
                        else if (field.GetType() == typeof(Decimal)
                              || field.GetType() == typeof(SqlDecimal))
                        {                            
                            // mettre une , comme séparateur de decimal
                            fieldString = ((Decimal)field).ToString("F2", nfi);
                        }
                        else if (field.GetType() == typeof(SqlMoney))
                        {
                            fieldString = ((SqlMoney)field).ToString();
                        }
                        else if (field.GetType() == typeof(SqlInt32))
                        {
                            fieldString = ((SqlInt32)field).ToString();
                        }
                        else
                        {
                            fieldString = field.ToString();
                            fieldString = fieldString.TrimEnd();
                        }
                        // validation du champ
                        try
                        {
                            if (validationFields != null)
                            {
                                string pattern = validationFields[column.ToString()];
                                bool valid = Regex.IsMatch(fieldString, pattern);
                                // debug : log                                
                                if (IntegratorBatch.InfoLogger.IsDebugEnabled)
                                {
                                    IntegratorBatch.InfoLogger.Debug(nbRows + "," + nbColumns + "-> Controle du contenu de " + column.ToString() + " : " + fieldString + " sur le pattern " + pattern + " => " + valid);
                                }

                                if (!valid)
                                {
                                    // log error : le format n'est pas respecté
                                    IntegratorBatch.ExceptionLogger.Error("Contenu du champ: " + column.ToString() + ": " + fieldString + " pour la ligne n°" + nbRows + " est incorrect");
                                    IntegratorBatch.InfoLogger.Error("Annulation de la ligne : " + nbRows);
                                    nbRows--;
                                    lineString.Remove(0, lineString.Length);
                                    break;
                                }

                            }
                        }
                        catch (KeyNotFoundException knfe)
                        {
                            // log WARN car le champ spécifié n existe pas dans les codes de validation
                            IntegratorBatch.ExceptionLogger.Error("La validation du champ:" + column.ToString() + " n est pas prevue dans le fichier "+this.ValidationPath);
                            // correction pour n avoir qun seul message d erreur
                            validationFields[column.ToString()] = "^.*";
                        }

                        // ajout du champ sur la ligne
                        lineString.Append(fieldString);

                        if (nbColumns < table.Columns.Count)
                        {
                            lineString.Append('|');
                        }
                    }// fin de l'analyse de la ligne
                    if (lineString.Length > 0)
                    {
                        dataString.AppendLine(lineString.ToString());
                        lineString.Remove(0, lineString.Length);
                    }
                }
            }

            //Ligne de récapitulatif
            string recap = date.ToString("yyyyMMddHHmmss") + nbRows.ToString("0000000");
            dataString.Append(recap);

            try
            {
                StreamWriter myWriter = new StreamWriter(path);
                myWriter.Write(dataString);
                myWriter.Close();
            }
            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Fatal("Impossible de créer le fichier BP2s:" + path, e);
                throw e;
            }
            // fin de la création du fichier
            if (nbRows == 0)
            {
                IntegratorBatch.ExceptionLogger.Error("BP2S: " + recap + " Le fichier " + path + " est VIDE (nbLignes=" + nbRows + ", nbChamps=" + nbColumns + ")");                
            }
            else
            {
                IntegratorBatch.InfoLogger.Info("BP2S: " + recap + " Le fichier " + path + " est cree: OK (nbLignes=" + nbRows + ", nbChamps=" + nbColumns + ")");
            }
        }

        /// <summary>
        /// lecture des codes de validation pour les champs. Chemin vers le fichier contenant les expressions regulieres
        /// </summary>
        public string ValidationPath
        {
            set { validationFields = InitFile.ReadConfigFile(value); validationPath = value; }
            get { return validationPath; }
        }
    }
}
