using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Xml.Xsl;
using System.Resources;
using System.Reflection;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using FGA.Automate.Properties;

namespace FGA.Automate.Consumer.Excel
{
    /// <summary>
    /// Utilisation de XSL T (Transformation de fichiers XML)
    /// </summary>
    class WorkbookEngine
    {
        private XslCompiledTransform xslt;
        private XmlDataDocument xmlDataDoc;
        private DataSet dataSet;
        private XmlWriterSettings settings;

        /// <summary>
        /// Constructeur unique
        /// </summary>
        public WorkbookEngine()
        {
            xslt = new XslCompiledTransform();

            StringReader reader = new StringReader(Resources.Excel_type);
            XmlTextReader xRdr = new XmlTextReader(reader);
            xslt.Load(xRdr);

            // Configuration de la sortie XML
            settings = new XmlWriterSettings();
            //settings.Indent = true;
            //settings.IndentChars = ("    ");
            //settings.OmitXmlDeclaration = false;
            //settings.NewLineHandling = NewLineHandling.Entitize;
            //settings.Encoding = new UnicodeEncoding();
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;
            settings.NewLineHandling = NewLineHandling.None;
            settings.Encoding = ASCIIEncoding.ASCII;

        }
        ///<summary>
        ///retourne la donnée XML représentant un classeur Excel (Base64FormattingOptions sur Xslt)
        ///</summary>
        ///<param name="ds">donnee a sauver</param>
        public void CreateWorkbook(DataSet ds)
        {
            dataSet = ds;
            xmlDataDoc = new XmlDataDocument(ds);
        }

        /// <summary>
        /// Ecriture du fichier
        /// </summary>
        /// <param name="fileName">nom pour le fichier</param>
        public void SaveWorkbook(string fileName)
        {
            XmlWriter xw = XmlWriter.Create(fileName, settings);
            xslt.Transform(xmlDataDoc, xw);
            xw.Flush();
            xw.Close();
        }
        /// <summary>
        /// Ecriture du fichier au format DataSet
        /// </summary>
        /// <param name="fileName">nom pour le fichier</param>
        public void SaveXmlData(string fileName)
        {
            XmlWriter xw = XmlWriter.Create(fileName, settings);
            xmlDataDoc.WriteTo(xw);
            xw.Flush();
            xw.Close();

            
            StringWriter sw = new StringWriter();
            sw.Write(dataSet.GetXml());            
            sw.Flush(); sw.Close();
            StreamWriter outfile = new StreamWriter(fileName + ".2");
            outfile.Write(sw);
            outfile.Flush(); outfile.Close();

            StringWriter sw2 = new StringWriter();
            sw2.Write(dataSet.GetXmlSchema());
            sw2.Flush(); sw2.Close();
            StreamWriter outfile2 = new StreamWriter(fileName + ".xsd");
            outfile2.Write(sw2);
            outfile2.Flush(); outfile2.Close();

        }


           /// <summary>
        /// Sortie en String
        /// </summary>
        public override String ToString()
        {
            StringWriter sw = new StringWriter();
            XmlWriter xw = XmlWriter.Create(sw, settings);
            xslt.Transform(xmlDataDoc, xw);
            return sw.ToString();
        }


        /// <summary>
        /// Creation d un fichier open XML (excel 2007 )
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveWorkbook2007(string fileName)
        {
            //Make a copy of the template file.
            //File.Copy("template.xlsx", "generated.xlsx", true);
            //Open the copied template workbook. 
            SpreadsheetDocument myWorkbook = SpreadsheetDocument.Create(fileName,SpreadsheetDocumentType.Workbook,true);

            //Access the main Workbook part, which contains all references.
            WorkbookPart workbookPart = myWorkbook.WorkbookPart;
            //Get the first worksheet. 
            WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
            // The SheetData object will contain all the data.
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            //Row contentRow = CreateContentRow(index, territoryName, salesLastYear, salesThisYear);
            //index++;
            ////Append new row to sheet data.
            //sheetData.AppendChild(contentRow);

            //XmlWriter xw = XmlWriter.Create(fileName, settings);
            //xslt.Transform(xmlDataDoc, xw);
            //xw.Flush();
            //xw.Close();
        }


    }
}
