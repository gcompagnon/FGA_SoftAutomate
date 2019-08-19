using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Reporting_Excel;

namespace FGA.Automate.Consumer
{
    class OpenXMLFileProducer
    {
        /// <summary>
        /// Helper pour generer une feuille excel OpenXML
        /// </summary>
        /// <param name="ds">les donnees</param>
        /// <param name="saveFilePath">repertoire de sauvegarde et nom du fichier en sortie</param>
        /// <param name="styleFilepath">repertoire et nom du fichier de style</param>
        /// <param name="templateFilepath">repertoire et nom du fichier de style</param>
        /// <param name="graphFilepath">repertoire et nom du fichier de graphiques</param>
        /// <param name="date">constante pour la page de garde</param>
        /// <param name="sheetNames">la liste des noms des onglets</param>
        /// <param name="monoSheetFlag">Flag = TRUE: toutes les datatables de la dataset sur le même onglet</param>
        public static void CreateExcelWorkbook(DataSet ds, String saveFilepath, String styleFilepath, String templateFilepath, String graphFilepath, String date, String[] sheetNames,Boolean monoSheetFlag)
        {
            try
            {
                // Rechercher les fichiers de configuration
                string templateFilepath2 = Config.InitFile.SearchForFile(templateFilepath);
                string styleFilepath2 = null;
                string graphFilepath2 = null;
                if (styleFilepath != null)
                {
                    styleFilepath2 = Config.InitFile.SearchForFile(styleFilepath);
                }
                if (graphFilepath != null)
                {
                    graphFilepath2 = Config.InitFile.SearchForFile(graphFilepath);
                }
                Ultimate g2 = new Ultimate(templateFilepath2, saveFilepath, styleFilepath2, graphFilepath2);
                g2.execution(ds, date, sheetNames,monoSheetFlag);
            }
            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Fatal("Impossible de créer le fichier Excel:" + saveFilepath, e);
                throw e;
            }

        }
    }
}
