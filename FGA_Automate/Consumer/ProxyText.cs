using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Specialized;

namespace FGA.Automate.Consumer
{
    /// <summary>
    /// classe produisant le fichier Proxy en format CSV
    /// </summary>
    class ProxyText
    {

        private IDictionary<string, string> validationFields = null;
        private string validationPath;
        private NumberFormatInfo nfi;


        public ProxyText()
        {
            nfi = new NumberFormatInfo();
            nfi.CurrencyDecimalSeparator = ".";
            nfi.NumberDecimalSeparator = ".";
        }

        /// <summary>
        /// creation d un fichier au format des proxy (instrument OTC)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="path"></param>
        /// <param name="date"></param>
        public void CreateFile(StringCollection data, String path, DateTime date)
        {
            StringBuilder dataString = new StringBuilder();
            StringBuilder lineString = new StringBuilder();
            try
            {
                StreamWriter myWriter = new StreamWriter (path, false,Encoding.Unicode);
                foreach (String ligne in data)
                {
                    myWriter.WriteLine(ligne);
                }
                myWriter.Close();
            }
            catch (Exception e)
            {
                IntegratorBatch.ExceptionLogger.Fatal("Impossible de créer le fichier Proxy:" + path, e);
                throw e;
            }
        }

    }
}
