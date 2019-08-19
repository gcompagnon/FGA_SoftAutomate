using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using FGA.Automate.Consumer.Excel;

namespace FGA.Automate.Consumer
{
    /// <summary>
    /// Classe utilitaire / Helper pour generer un fichier excel format XML 2003
    /// </summary>
    class ExcelFileProducer
    {

        public static void CreateWorkbook(DataSet ds, String path)
        {
            try
            {
                if (ds.Tables.Count > 0)
                {
                    WorkbookEngine we = new WorkbookEngine();
                    we.CreateWorkbook(ds);
                    we.SaveWorkbook(path);
                }
            }
            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Fatal("Impossible de créer le fichier Excel:" + path, e);
                throw e;
            }

          }
     }
}
