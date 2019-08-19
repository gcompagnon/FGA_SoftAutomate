using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using System.IO;
using System.Data;


using Spreadsheet = DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Charts;
using Charts = DocumentFormat.OpenXml.Drawing.Charts;


namespace Reporting_Excel
{
    class Test
    {

        public string template;
        public string source;
        public string copie;


        public Test(string template, string source, string copie)
        {
            this.copie = copie;
            this.source = source;
            this.template = template;
        }

        public PieChartSeries GeneratePieChartSeries(string[] labels,double[] data)
        {
            PieChartSeries pieChartSeries1 = new PieChartSeries();
            Index index1 = new Index() { Val = (UInt32Value)0U };
            Order order1 = new Order() { Val = (UInt32Value)0U };

            SeriesText seriesText1 = new SeriesText();
            NumericValue numericValue1 = new NumericValue();
            numericValue1.Text = "sreie 1";

            seriesText1.Append(numericValue1);

            CategoryAxisData categoryAxisData1 = new CategoryAxisData();

            StringLiteral stringLiteral1 = new StringLiteral();
            PointCount pointCount1 = new PointCount() { Val = (uint) labels.Length };

            //StringPoint stringPoint1 = new StringPoint() { Index = (UInt32Value)0U };
            //NumericValue numericValue2 = new NumericValue();
            //numericValue2.Text = "a";

            //stringPoint1.Append(numericValue2);

            //StringPoint stringPoint2 = new StringPoint() { Index = (UInt32Value)1U };
            //NumericValue numericValue3 = new NumericValue();
            //numericValue3.Text = "n";

            //stringPoint2.Append(numericValue3);

            //StringPoint stringPoint3 = new StringPoint() { Index = (UInt32Value)2U };
            //NumericValue numericValue4 = new NumericValue();
            //numericValue4.Text = "c";

            //stringPoint3.Append(numericValue4);

            //StringPoint stringPoint4 = new StringPoint() { Index = (UInt32Value)3U };
            //NumericValue numericValue5 = new NumericValue();
            //numericValue5.Text = "d";

            //stringPoint4.Append(numericValue5);

            //Ajout des etiquette de legendes
            for(int i = 0 ; i < labels.Length;i++)
            {
                StringPoint stringPoint = new StringPoint() { Index = (uint) i};
                NumericValue numericValue = new NumericValue();
                numericValue.Text =labels[i];

                stringPoint.Append(numericValue);
                stringLiteral1.Append(stringPoint);
            }

            stringLiteral1.Append(pointCount1);
            //stringLiteral1.Append(stringPoint1);
            //stringLiteral1.Append(stringPoint2);
            //stringLiteral1.Append(stringPoint3);
            //stringLiteral1.Append(stringPoint4);

            categoryAxisData1.Append(stringLiteral1);

            DocumentFormat.OpenXml.Drawing.Charts.Values values1 = new DocumentFormat.OpenXml.Drawing.Charts.Values();

            NumberLiteral numberLiteral1 = new NumberLiteral();
            FormatCode formatCode1 = new FormatCode();
            formatCode1.Text = "General";
            PointCount pointCount2 = new PointCount() { Val = (uint) data.Length };

            //NumericPoint numericPoint1 = new NumericPoint() { Index = (UInt32Value)0U };
            //NumericValue numericValue6 = new NumericValue();
            //numericValue6.Text = "1";

            //numericPoint1.Append(numericValue6);

            //NumericPoint numericPoint2 = new NumericPoint() { Index = (UInt32Value)1U };
            //NumericValue numericValue7 = new NumericValue();
            //numericValue7.Text = "2";

            //numericPoint2.Append(numericValue7);

            //NumericPoint numericPoint3 = new NumericPoint() { Index = (UInt32Value)2U };
            //NumericValue numericValue8 = new NumericValue();
            //numericValue8.Text = "3";

            //numericPoint3.Append(numericValue8);

            //NumericPoint numericPoint4 = new NumericPoint() { Index = (UInt32Value)3U };
            //NumericValue numericValue9 = new NumericValue();
            //numericValue9.Text = "5";

            //numericPoint4.Append(numericValue9);
            
            
            for (int i = 0; i < data.Length; i++)
            {
                NumericPoint numericPoint = new NumericPoint() { Index = (uint) i };
                NumericValue numericValue = new NumericValue();
                numericValue.Text = data[i].ToString();

                numericPoint.Append(numericValue);
                numberLiteral1.Append(numericPoint);
            }

            

            numberLiteral1.Append(formatCode1);
            numberLiteral1.Append(pointCount2);
            //numberLiteral1.Append(numericPoint1);
            //numberLiteral1.Append(numericPoint2);
            //numberLiteral1.Append(numericPoint3);
            //numberLiteral1.Append(numericPoint4);

            values1.Append(numberLiteral1);

            pieChartSeries1.Append(index1);
            pieChartSeries1.Append(order1);
            pieChartSeries1.Append(seriesText1);
            pieChartSeries1.Append(categoryAxisData1);
            pieChartSeries1.Append(values1);
            return pieChartSeries1;
        }

        public Chart getChartByTitle(WorksheetPart wsp, string title)
        {
            DrawingsPart dp = wsp.DrawingsPart;
            IEnumerable<ChartPart> cps = dp.ChartParts;

            ChartPart c = cps.ElementAt<ChartPart>(0);
            //c.ChartSpace.Descendants<Chart>().First().Descendants<Title>().First();
            //c.ChartSpace.Descendants<Chart>().First().Descendants<Title>().First().Des

            Chart res = null;
            foreach (ChartPart cp in cps)
            {
                if (cp.ChartSpace.Descendants<Chart>().First().Descendants<Title>().First().Descendants<DocumentFormat.OpenXml.Drawing.Run>().First().Text.Text == title)
                {
                    res = cp.ChartSpace.Descendants<Chart>().First();
                }
            }
            return res;
        }

        public void exec()
        {
            System.IO.File.Copy(template, copie, true);
            SpreadsheetDocument myWorkbookCopie = SpreadsheetDocument.Open(copie, true);
            SpreadsheetDocument myWorkbookSource = SpreadsheetDocument.Open(source, true);

            ////Obtention du graph 
            ////WorkbookPart workbookPart = myWorkbookSource.WorkbookPart;
            ////WorksheetPart worksheetPart = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(2);
            //WorksheetPart worksheetPart = XcelWin.GetWorksheetPartByName(myWorkbookSource, "REPORTING");
            //DrawingsPart a = worksheetPart.DrawingsPart;
            //ChartPart b = a.ChartParts.ElementAt(0);


            ////Creation drawingpart et chartpart
            //WorkbookPart workbookPart2 = myWorkbookCopie.WorkbookPart;
            //WorksheetPart worksheetPart2 = workbookPart2.WorksheetParts.ElementAt<WorksheetPart>(0);
            //DrawingsPart dp = worksheetPart2.AddNewPart<DrawingsPart>();
            //ChartPart x = dp.AddNewPart<ChartPart>();

            //x.FeedData(b.GetStream());

            WorkbookPart workbookPart2 = myWorkbookCopie.WorkbookPart;
            WorksheetPart worksheetPart = XcelWin.getWorksheetPartByName(myWorkbookCopie, "REPORTING");

            string[] labels = {"a","b","c"};
            double[] data = { 2.2, 3.3, 5.5};

            //Permet de mettre un chart avec des données
            PieChartSeries pcs = GeneratePieChartSeries(labels,data);
            Chart x = getChartByTitle(worksheetPart, "Ressource");
            PieChart px = x.Descendants<PieChart>().First();
            px.RemoveAllChildren<PieChartSeries>();
            px.Append(pcs);


            workbookPart2.Workbook.Save();
            myWorkbookCopie.Close();

        }



        //-----------------------------------------------------------------------------------
        //Tests sur les charts 

        public void exec(string formuleVal, string formuleLegende)
        {
            string label = "Feuil1!D5:J5";
            string serie = "Feuil1!C12";
            string val = "Feuil1!D12:J12";
            //euro	mmp	mut2m	cmav	sapem	quatrem	auxia

             System.IO.File.Copy(template, copie, true);
            SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(copie, true);

            ChartPart cc = XcelWin.cloneChart(XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1"), "Tests", 0, 0, 10, 10);



            BarChart bc = cc.ChartSpace.Descendants<BarChart>().First();

            for (int i = 0; i < 6; i++)
            {
                BarChartSeries newSerie = (BarChartSeries)bc.Elements<BarChartSeries>().First().CloneNode(true);

                string form = val.Replace("12", (6+i).ToString());

                newSerie.SeriesText.StringReference.Formula.Text = serie;
                //newSerie.Descendants<CategoryAxisData>().First().Remove();
                
                newSerie.Descendants<CategoryAxisData>().First().NumberReference.Formula.Text = label;
                newSerie.Descendants<Charts.Values>().First().NumberReference.Formula.Text = form;


                newSerie.Index.Val = (uint)i;
                newSerie.Order.Val = (uint)i;

                bc.Append(newSerie);
            }

            bc.Elements<BarChartSeries>().First().Remove();

            myWorkbook.WorkbookPart.Workbook.Save();
            myWorkbook.Close();
        }

        public static void fixChartData2(ChartPart cc, string nomSheet, List<int> elems, string colValeur, string colLegende)
        {


            //ChartPart cc = wsp.GetPartsOfType<DrawingsPart>().First().ChartParts.First<ChartPart>();
            Charts.Values v = cc.ChartSpace.Descendants<Charts.Values>().First();

            //Formule de mon piechart
            Charts.Formula f = v.Descendants<Charts.Formula>().First();

            //string id = wbp.GetIdOfPart(wsp);
            //string nom = "'" + wbp.Workbook.Descendants<Sheet>().Where(s => s.Id.Value.Equals(id)).First().Name + "'!";

            string formulaBis = "\'" + nomSheet + "\'!$" + colValeur + "$" + elems.ElementAt(0);
            foreach (int i in elems.Skip(1))
            {
                formulaBis += ",\'" + nomSheet + "\'!$" + colValeur + "$" + i;
            }

            f.Text = formulaBis;

            //Formule pour les legendes
            Charts.CategoryAxisData cad = cc.ChartSpace.Descendants<Charts.CategoryAxisData>().First();

            //Formule des legendes de mon piechart
            Charts.Formula f2 = cad.Descendants<Charts.Formula>().First();

            string formula2Bis = "\'" + nomSheet + "\'!$" + colLegende + "$" + elems.ElementAt(0);
            foreach (int i in elems.Skip(1))
            {
                formula2Bis += ",\'" + nomSheet + "\'!$" + colLegende + "$" + i;
            }
            f2.Text = formula2Bis;


            Charts.SeriesText st = cc.ChartSpace.Descendants<Charts.SeriesText>().First();

            st.StringReference.Formula.Text = "";
        }


        //------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------
        //Tableau
        public static void AddTableDefinitionPart(WorksheetPart part, List<string> titres, int nbLi, int nbCol)
        {
            TableDefinitionPart tableDefinitionPart1 = part.AddNewPart<TableDefinitionPart>("vId1");
            GenerateTableDefinitionPart1Content(tableDefinitionPart1, titres, nbLi,nbCol);
        }

        // Generates content of tableDefinitionPart1.
        private static void GenerateTableDefinitionPart1Content(TableDefinitionPart tableDefinitionPart1, List<string> titres, int nbLi, int nbCol)
        {
            string refe = "A1:"+Ultimate.headerColumns[nbCol-1]+nbLi;
            
            Table table1 = new Table() { Id = (UInt32Value)1U, Name = "Table1", DisplayName = "Table1", Reference = refe, TotalsRowShown = false };
            AutoFilter autoFilter1 = new AutoFilter() { Reference = refe };

            //TableColumns tableColumns1 = new TableColumns() { Count = (UInt32Value)3U };
            //TableColumn tableColumn1 = new TableColumn() { Id = (UInt32Value)1U, Name = "Make" };
            //TableColumn tableColumn2 = new TableColumn() { Id = (UInt32Value)2U, Name = "Miles" };
            //TableColumn tableColumn3 = new TableColumn() { Id = (UInt32Value)3U, Name = "Cost", DataFormatId = (UInt32Value)0U };
            //tableColumns1.Append(tableColumn1);
            //tableColumns1.Append(tableColumn2);
            //tableColumns1.Append(tableColumn3);
            
            TableColumns tableColumns1 = new TableColumns() { Count = (uint)titres.Count };
            int i = 1;
            foreach (string t in titres)
            {
                TableColumn tableColumn1 = new TableColumn() { Id = (uint)i, Name = t };
                tableColumns1.Append(tableColumn1);
                i++;
            }

           
            TableStyleInfo tableStyleInfo1 = new TableStyleInfo() { Name = "TableStyleMedium2", ShowFirstColumn = false, ShowLastColumn = false, ShowRowStripes = true, ShowColumnStripes = false };

            table1.Append(autoFilter1);
            //table1.Append(tableColumns1);
            table1.Append(tableStyleInfo1);

            tableDefinitionPart1.Table = table1;
        }


        public void exec(System.Data.DataTable dt)
        {
            System.IO.File.Copy(template, copie, true);
            SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(copie, true);
            WorkbookPart workbookPart = myWorkbook.WorkbookPart;

            WorksheetPart wsPart = XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1");

            //DocumentFormat.OpenXml.Spreadsheet.Columns columns = new Columns();
            //for(int i=0;i<size.Length;i++)
            //{
            //    DocumentFormat.OpenXml.Spreadsheet.Column c = new Column();
            //    c.CustomWidth = true;
            //    c.Min = (uint) i+1;
            //    c.Max = (uint) i+1;
            //    c.Width = size[i];
                
            //    columns.Append(c);
            //}

            //DocumentFormat.OpenXml.Spreadsheet.Columns cc = new Columns();
            //cc.Append(new DocumentFormat.OpenXml.Spreadsheet.Column() { Min = 1, Max = 3, CustomWidth = true, Width = 5 });
            ////wsPart.Worksheet.Append(cc);
            //Worksheet ws = wsPart.Worksheet;
            //ws.Append(cc);
            

            

            DrawingsPart drawingsPart1 = wsPart.AddNewPart<DrawingsPart>("rId1");
            XcelWin.GenerateDrawingsPart1Content(drawingsPart1);

            ChartPart chartPart1 = drawingsPart1.AddNewPart<ChartPart>("rId1");
            XcelWin.GenerateChartPart1Content(chartPart1);

            Drawing drawing1 = new Drawing() { Id = "rId1" };
            wsPart.Worksheet.Append(drawing1);







            workbookPart.Workbook.Save();
            myWorkbook.Close();
        }


        public int[] testColumnWidth(System.Data.DataTable dt)
        {
            int[] size = new int[dt.Columns.Count];
            foreach (DataRow r in dt.Rows)
            {
                int i=0;
                foreach (var a in r.ItemArray)
                {
                    if (a.ToString().Length > size[i])
                        size[i] = a.ToString().Length;
                    i++;
                }
            }

            return size;
        }
        //------------------------------------------------------------------------------------

    }
}
