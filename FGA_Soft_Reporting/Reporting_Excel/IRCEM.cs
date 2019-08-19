using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using System.Data;

namespace Reporting_Excel
{
    public class IRCEM
    {
        public string template;
        public string copie;
        public SpreadsheetDocument myWorkbook;
        public string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ","AR","AS","AT","AU","AV","AW","AX","AY","AZ" };

        public IRCEM(string tmplt, string cp)
        {
            template = tmplt;
            copie = cp;
        }

        public Row creerTitres(DataTable dt, int indiceLigne)
        {
            int i = 0;
            Row r = new Row();
            r.RowIndex = (uint) indiceLigne;

            foreach (DataColumn DC in dt.Columns)
            {
                if (DC.ColumnName != "style")
                {
                    r.AppendChild(XcelWin.createTextCell(headerColumns[i], indiceLigne, DC.ColumnName, 0));
                    i++;
                }
            }
            return r;
        }

        public Row creerLigne(DataRow dr, int index, uint[] style)
        {
            Row r = new Row() { RowIndex = (uint) index};
            Cell c = new Cell();
            int i = 0;
            //Skip 1 pour ne pas écrire la colonne de style
            foreach (var att in dr.ItemArray)
            {
                c = new Cell();

                try { c = XcelWin.createCellDouble(headerColumns[i], index, Convert.ToDouble(att), style[0]); }
                catch (Exception) { string tmp; if (att == DBNull.Value) { tmp = ""; } else { tmp = att.ToString(); } c = XcelWin.createTextCell(headerColumns[i], index, tmp, style[0]); }

                r.AppendChild(c);
                i++;
            }

            return r;
        }

        /*
        public SheetData creerFeuille(WorksheetPart worksheetPart, DataTable dt)
        {

            SheetData sd = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            Row r = creerTitres(dt,1);
            sd.AppendChild(r);

            int index = 2;

            r = new Row();
            foreach (DataRow item in dt.Rows)
            {
                uint[] a = { 0,0,0,0,0,0};// lineStyles[tmp];
                r = creerLigne(item, index, a);
                sd.AppendChild(r);
                index++;
            }

            return sd;
        }
        */

        public SheetData creerFeuille2(WorksheetPart worksheetPart, DataSet ds)
        {
            SheetData sd = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            int index = 1;
            foreach (DataTable dt in ds.Tables)
            {
                Row r = new Row();
                if (!dt.Columns[0].ColumnName.Contains("Column"))
                {
                    r = creerTitres(dt, index);
                    sd.AppendChild(r);
                    index++;
                }

                r = new Row();
                foreach (DataRow item in dt.Rows)
                {
                    uint[] a = { 0, 0, 0, 0, 0, 0 };
                    r = creerLigne(item, index, a);
                    sd.AppendChild(r);
                    index++;
                }
                index++;
            }
            return sd;
        }

        /*
        public void execution(DataTable[] dt)
        {

            //Copie du template et ouverture du fichier
            System.IO.File.Copy(template, copie, true);
            myWorkbook = SpreadsheetDocument.Open(copie, true);
            

            //Access the main Workbook part, which contains all references.
            WorkbookPart workbookPart = myWorkbook.WorkbookPart;

            //0 : feuille 1
            //4 : feuille 2
            //3 : feuille 3

        
            //WorksheetPart worksheetPart = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(2);
            //SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            ////Ajout de la date sur la page de garde
            //Cell cell = XcelWin.CreateTextCell("A", 1, "IRCEM", 0);
            //Row r = new Row();
            //r.RowIndex = 1;
            //r.AppendChild(cell);
            //sheetData.AppendChild(r);


            WorksheetPart wp = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(0);
            creerFeuille(wp, dt[0]);

            wp = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(5);
            creerFeuille(wp, dt[1]);

            wp = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(4);
            creerFeuille(wp, dt[2]);

            wp = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(3);
            creerFeuille(wp, dt[3]);

            myWorkbook.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            myWorkbook.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;

            //Sauvegarde du workbook et fermeture de l'objet fichier
            workbookPart.Workbook.Save();
            myWorkbook.Close();
        }
        */

        public void execution2(DataSet ds)
        {
            //Copie du template et ouverture du fichier
            System.IO.File.Copy(template, copie, true);
            myWorkbook = SpreadsheetDocument.Open(copie, true);


            //Access the main Workbook part, which contains all references.
            WorkbookPart workbookPart = myWorkbook.WorkbookPart;


            WorksheetPart wp = XcelWin.getWorksheetPartByName(myWorkbook,"Feuil1");
            creerFeuille2(wp, ds);


            myWorkbook.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            myWorkbook.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;

            //Sauvegarde du workbook et fermeture de l'objet fichier
            workbookPart.Workbook.Save();
            myWorkbook.Close();
        }

    }
}
