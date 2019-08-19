using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGA.SQLCopy;
using SQLCopy.Dbms;
using LumenWorks.Framework.IO.Csv;
using System.IO;

namespace FGA.Automate.Dataconverter.Producer
{
    class CSVtoSQL
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceConnexion"></param>
        /// <param name="destConnexion"></param>
        /// <param name="mode">Exclude, Include or Aucun</param>
        /// <param name="liste">Liste des tables lié à l'option Mode</param>
        public static void DataBaseIntegrate
            (string CsvFileName, string destConnexion, string destSchema, string destTable, string csvFileEncoding = "iso-8859-1")
        {

            Encoding enc = null;
            switch (csvFileEncoding)
            {
                case "UTF8": enc = Encoding.UTF8; break;
                case "ASCII": enc = Encoding.ASCII; break;
                case "BigEndianUnicode": enc = Encoding.BigEndianUnicode; break;
                case "Unicode": enc = Encoding.Unicode; break;
                case "UTF32": enc = Encoding.UTF32; break;
                case "UTF7": enc = Encoding.UTF7; break;
                case "iso-8859-1": enc = Encoding.GetEncoding(28591); break; 
                default: enc = Encoding.Default; break;
            };


            DatabaseTable destDataTable = new DatabaseTable(destSchema, destTable);
            /*
            MSDBIntegration sql = new MSDBIntegration(destinationConnection: destConnexion);           
            sql.BulkUpsertCsv(CsvFileName,destDataTable);
            */

            SQLCopy.MSSQL2005_DBConnection DestinationConnection = new SQLCopy.MSSQL2005_DBConnection(destConnexion);

            using (var reader = new CsvReader(new StreamReader(CsvFileName, enc), true, ';'))
            {
                try
                {
                    DestinationConnection.Open();

                    CsvDataAdapter adapter = new CsvDataAdapter(reader);
                    int nbTobeMergedRows = adapter.Fill(DestinationConnection, destDataTable, "yyyyMMdd");

                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    DestinationConnection.Close();
                }

            }



        }
    }
}