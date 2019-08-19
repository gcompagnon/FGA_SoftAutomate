using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using System.Data;

using System.IO;

using A = DocumentFormat.OpenXml.Drawing;
using Draw = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Chart = DocumentFormat.OpenXml.Drawing.Charts;


namespace Reporting_Excel
{
    public class XcelWin
    {
        #region Creation cellules
        public static Cell createCellFloat(string column, int index, float classementRubrique, uint style)
        {
            Cell c = new Cell();
            c.CellReference = column + index;
            CellValue v = new CellValue();
            v.Text = classementRubrique.ToString();
            c.AppendChild(v);
            c.StyleIndex = style;
            return c;
        }

        public static Cell createTextCell(string column, int index, string text, uint style)
        {
            //Create a new inline string cell.
            Cell c = new Cell();
            c.DataType = CellValues.InlineString;
            c.CellReference = column + index;
            //Add text to the text cell.
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = text;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            c.StyleIndex = style;
            return c;
        }

        public static Cell createCellDouble(string column, int index, double fga, uint style)
        {
            Cell c = new Cell();
            c.CellReference = column + index;
            CellValue v = new CellValue();
            if (fga == 0)
                v.Text = "";
            else
                v.Text = fga.ToString();
            c.StyleIndex = style;
            c.AppendChild(v);
            return c;
        }
        #endregion

        #region Stylesheet
        public static uint ajoutFont(Stylesheet ss, string name, int size, bool bold, string color)
        {
            uint indexFont;

            Fonts fts = ss.Fonts;
            indexFont = fts.Count;

            Font ft = new Font();
            FontName ftn = new FontName();
            ftn.Val = name;
            FontSize ftsz = new FontSize();
            ftsz.Val = size;
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            if(bold)
                ft.Bold = new Bold();
            if (color != "")
            {
                Color couleur = new Color() { Rgb = new HexBinaryValue() { Value = HexBinaryValue.FromString(color) } };
                ft.Append(couleur);
            }

            fts.Append(ft);

            fts.Count = (uint)fts.ChildElements.Count;
            return fts.Count-1;
        }

        public static uint ajoutFill(Stylesheet ss,string color)
        {
            Fills fills = ss.Fills;
            uint indexFill = fills.Count;

            Fill fill;
            PatternFill patternFill;

            if (color == "")
            {
                fill = new Fill();
                patternFill = new PatternFill();
                patternFill.PatternType = PatternValues.None;
                fill.PatternFill = patternFill;
                fills.Append(fill);
            }
            else
            {
                fill = new Fill();
                patternFill = new PatternFill();
                patternFill.PatternType = PatternValues.Solid;
                patternFill.ForegroundColor = new ForegroundColor();
                patternFill.ForegroundColor.Rgb = HexBinaryValue.FromString(color);
                patternFill.BackgroundColor = new BackgroundColor();
                patternFill.BackgroundColor.Rgb = patternFill.ForegroundColor.Rgb;
                fill.PatternFill = patternFill;
                fills.Append(fill);
            }

            fills.Count = (uint)fills.ChildElements.Count;
            return fills.Count-1;
        }

        public static uint ajoutBorder(Stylesheet ss, string ts, string bs, string ls, string rs)
        {
            Borders borders = ss.Borders;
            uint indexBorder = borders.Count;
            
            Border border = new Border();
            if (ts!="")
            {
                border.TopBorder = new TopBorder();
                if (ts == "Thin")
                {
                    border.TopBorder.Style = BorderStyleValues.Thin;
                } else
                {
                    border.TopBorder.Style = BorderStyleValues.Thick;
                }
            }
            if (bs != "")
            {
                border.BottomBorder = new BottomBorder();
                if (bs == "Thin")
                {
                    border.BottomBorder.Style = BorderStyleValues.Thin;
                }
                else
                {
                    border.BottomBorder.Style = BorderStyleValues.Thick;
                }
            }
            if (ls != "")
            {
                border.LeftBorder = new LeftBorder();
                if (ls == "Thin")
                {
                    border.LeftBorder.Style = BorderStyleValues.Thin;
                }
                else
                {
                    border.LeftBorder.Style = BorderStyleValues.Thick;
                }
            }
            if (rs != "")
            {
                border.RightBorder = new RightBorder();
                if (rs == "Thin")
                {
                    border.RightBorder.Style = BorderStyleValues.Thin;
                }
                else
                {
                    border.RightBorder.Style = BorderStyleValues.Thick;
                }
            }

            borders.Append(border);

            borders.Count = (uint)borders.ChildElements.Count;
            return borders.Count-1;
        }

        public static uint ajouterCellformat(Stylesheet ss,uint numberFomat,uint font, uint fill, uint border, uint format) 
        {
            CellFormats cfs = ss.CellFormats;

            CellFormat cf = new CellFormat();

            cf = new CellFormat();
            cf.NumberFormatId = numberFomat;
            cf.FontId = font;
            cf.FillId = fill;
            cf.BorderId = border;
            cf.FormatId = format;
            cfs.Append(cf);

            cfs.Count = (uint)cfs.ChildElements.Count;
            return cfs.Count-1; 
        }

        public static uint creerStyle(Stylesheet ss, string police, int taille, bool bold, string fontColor,
            string fillColor, string ts, string bs, string ls, string rs, uint numberFormat)
        {
            uint f = XcelWin.ajoutFont(ss, police, taille, bold, fontColor);
            uint f2 = XcelWin.ajoutFill(ss, fillColor);
            uint b = XcelWin.ajoutBorder(ss, ts, bs, ls, rs);
            uint c = XcelWin.ajouterCellformat(ss, numberFormat, f, f2, b, 0);
            return c;
        }

        public static Stylesheet CreateStylesheet()
        {
            Stylesheet ss = new Stylesheet();

            //Font 0, de base
            Fonts fts = new Fonts();
           
            Font ft = new Font();
            FontName ftn = new FontName();
            ftn.Val = "Calibri";
            FontSize ftsz = new FontSize();
            ftsz.Val = 11;
            ft.FontName = ftn;
            ft.FontSize = ftsz;
            fts.Append(ft);

            fts.Count = (uint)fts.ChildElements.Count;

            //Pour les couleurs de fond des cellules
            Fills fills = new Fills();

            Fill fill;
            PatternFill patternFill;

            //fill 0, de base, necessaire
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.None;
            fill.PatternFill = patternFill;
            fills.Append(fill);
            
            ////fill 1 de base, necessaire
            fill = new Fill();
            patternFill = new PatternFill();
            patternFill.PatternType = PatternValues.Gray125;
            fill.PatternFill = patternFill;
            fills.Append(fill);

             
            fills.Count = (uint)fills.ChildElements.Count;

            //Border 0, de base, necessaire
            Borders borders = new Borders() ;

            Border border = new Border();
            border.LeftBorder = new LeftBorder();
            border.RightBorder = new RightBorder();
            border.TopBorder = new TopBorder();
            border.BottomBorder = new BottomBorder();
            border.DiagonalBorder = new DiagonalBorder();
            borders.Append(border);

            borders.Count = (uint)borders.ChildElements.Count;

            //164 : nbre de cellFormat implémenté dans excel, on ajoute donc les notres à la suite
            NumberingFormats nfs = new NumberingFormats();
            CellFormats cfs = new CellFormats();

            //Cell par défault, semble ne pas compter dans l'index. Particulierement necessaire
             CellStyleFormats csfs = new CellStyleFormats();
             CellFormat cf = new CellFormat();
             cf.NumberFormatId = 0;
             cf.FontId = 0;
             cf.FillId = 0;
             cf.BorderId = 0;
             csfs.Append(cf);
             csfs.Count = (uint)csfs.ChildElements.Count;

            //CellFormat 0, Necessaire !
            cf = new CellFormat();
            cf.NumberFormatId = 0;
            cf.FontId = 0;
            cf.FillId = 0;
            cf.BorderId = 0;
            cf.FormatId = 0;
            cfs.Append(cf);

            nfs.Count = (uint)nfs.ChildElements.Count;
            cfs.Count = (uint)cfs.ChildElements.Count;

            ss.Append(nfs);
            ss.Append(fts);
            ss.Append(fills);
            ss.Append(borders);
            ss.Append(csfs);
            ss.Append(cfs);

            //A priori inutil mais a garder, peut être necessaire par la suite
            CellStyles css = new CellStyles();
            CellStyle cs = new CellStyle();
            cs.Name = "Normal";
            cs.FormatId = 0;
            cs.BuiltinId = 0;
            css.Append(cs);
            css.Count = (uint)css.ChildElements.Count;
            ss.Append(css);

            DifferentialFormats dfs = new DifferentialFormats();
            dfs.Count = 0;
            ss.Append(dfs);

            TableStyles tss = new TableStyles();
            tss.Count = 0;
            tss.DefaultTableStyle = "TableStyleMedium9";
            tss.DefaultPivotStyle = "PivotStyleLight16";
            ss.Append(tss);

            return ss;
        }
        #endregion

        public static double getRowAttribut(DataRow item, string att)
        {
            double resultat;
            if (item[att] == DBNull.Value)
                resultat = 0.0;
            else
                resultat = (double)item[att];
            
            return resultat;
        }

        public static Row creerTitres(DataTable dt, int indiceLigne)
        {
            int i = 0;
            Row r = new Row();
            r.RowIndex = (uint)indiceLigne;

            foreach (DataColumn DC in dt.Columns)
            {
                if (DC.ColumnName != "style" && DC.ColumnName != "graph")
                {
                    if(DC.ColumnName == " ")
                        r.AppendChild(XcelWin.createTextCell(Ultimate.headerColumns[i], indiceLigne, "%", 0));
                    else
                        r.AppendChild(XcelWin.createTextCell(Ultimate.headerColumns[i], indiceLigne, DC.ColumnName, 0));
                    i++;
                }
            }
            return r;
        }

        public static Row creerLigne(DataRow dr, int index, uint style, int nbColonneConfig)
        {
            Row r = new Row();
            Cell c = new Cell();
            int i = 0;
            
            foreach (var att in dr.ItemArray.Skip(nbColonneConfig))
            {
                c = new Cell();

                try {c = XcelWin.createTextCell(Ultimate.headerColumns[i], index, Convert.ToDateTime(att).ToShortDateString(), style);}
                catch (Exception) 
                { 
                    try { c = XcelWin.createCellFloat(Ultimate.headerColumns[i], index, Convert.ToSingle(att), style); }
                    catch (Exception) { string tmp; if (att == DBNull.Value) { tmp = ""; } else { tmp = att.ToString(); } c = XcelWin.createTextCell(Ultimate.headerColumns[i], index, tmp, style);}
                }
                
                r.AppendChild(c);
                i++;
            }

            return r;
        }

        #region Chart
        public static ChartPart cloneChart(WorksheetPart worksheetPart, string chartTitle, int posLi, int posCol,int largeLi,int largeCol)
        {
            //Obtention de tous les ChartParts dans une enumeration
            DrawingsPart dp = worksheetPart.DrawingsPart;
            IEnumerable<ChartPart> cps = dp.ChartParts;

            //Recherche la ChartPart qui porte le titre correspondant
            ChartPart b = null;
            foreach (ChartPart cp in cps)
            {
                if (cp.ChartSpace.Descendants<Chart.Chart>().First().Descendants<Chart.Title>().First().Descendants<DocumentFormat.OpenXml.Drawing.Run>().First().Text.Text == chartTitle)
                {b = cp;}
            }

            DrawingsPart a = worksheetPart.DrawingsPart;

            //Ajout de la nouvelle partie et copie
            ChartPart x = a.AddNewPart<ChartPart>();
            Stream stream = b.GetStream();
            x.FeedData(stream);

            string id = a.GetIdOfPart(b);

            //Copie de l'ancre associée au graph original
            Draw.TwoCellAnchor newAnchor = null;
            DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing wsd = a.WorksheetDrawing;
            foreach (Draw.TwoCellAnchor tca in wsd.Elements<Draw.TwoCellAnchor>())
            {
                string tmp = tca.Descendants<Chart.ChartReference>().First().Id;
                if (tmp == id)
                {
                    newAnchor = (Draw.TwoCellAnchor)tca.CloneNode(true);
                }
            }

            //positionnement de la nouvelle ancre
            int r = Convert.ToInt32(newAnchor.ToMarker.RowId.Text) - Convert.ToInt32(newAnchor.FromMarker.RowId.Text);
            int c = Convert.ToInt32(newAnchor.ToMarker.ColumnId.Text) - Convert.ToInt32(newAnchor.FromMarker.ColumnId.Text);

            newAnchor.FromMarker.ColumnId = new Draw.ColumnId() { Text = "" + largeCol * posCol };
            newAnchor.ToMarker.ColumnId = new Draw.ColumnId() { Text = "" + (c + largeCol * posCol) };

            newAnchor.FromMarker.RowId = new Draw.RowId() { Text = "" + largeLi * posLi };
            newAnchor.ToMarker.RowId = new Draw.RowId() { Text = "" + (r + largeLi * posLi) };
            newAnchor.Descendants<Chart.ChartReference>().First().Id = a.GetIdOfPart(x);
            wsd.Append(newAnchor);

            return x;
        }

        public static void supprChart(WorksheetPart worksheetPart, string titre)
        {
            DrawingsPart a = worksheetPart.DrawingsPart;

            DrawingsPart dp = worksheetPart.DrawingsPart;
            IEnumerable<ChartPart> cps = a.ChartParts;

            ChartPart b = null;
            foreach (ChartPart cp in cps)
            {
                if (cp.ChartSpace.Descendants<Chart.Chart>().First().Descendants<Chart.Title>().First().Descendants<DocumentFormat.OpenXml.Drawing.Run>().First().Text.Text == titre)
                {b = cp;}
            }

            //obtention id du modèle
            string id = a.GetIdOfPart(b);

            a.DeletePart(b);

            //Suppression de l'ancre associée
            DocumentFormat.OpenXml.Drawing.Spreadsheet.WorksheetDrawing wsd = a.WorksheetDrawing;
            foreach (Draw.TwoCellAnchor tca in wsd.Elements<Draw.TwoCellAnchor>())
            {
                string tmp = tca.Descendants<Chart.ChartReference>().First().Id;
                if (tmp == id) { tca.Remove(); }
            }
        }

        public static void fixChartData(ChartPart cc,string nomSheet, List<int> elems, string colValeur, string colLegende)
        {
            Chart.Values v = cc.ChartSpace.Descendants<Chart.Values>().First();

            //Formule de mon piechart
            Chart.Formula f = v.Descendants<Chart.Formula>().First();

            string formulaBis = "\'" + nomSheet + "\'!$" + colValeur + "$" + elems.ElementAt(0);
            foreach (int i in elems.Skip(1))
            {
                formulaBis += ",\'" + nomSheet + "\'!$" + colValeur + "$" + i;
            }

            f.Text = formulaBis;

            //Formule pour les legendes
            Chart.CategoryAxisData cad = cc.ChartSpace.Descendants<Chart.CategoryAxisData>().First();

            //Formule des legendes de mon piechart
            Chart.Formula f2 = cad.Descendants<Chart.Formula>().First();

            string formula2Bis = "\'" + nomSheet + "\'!$" + colLegende + "$" + elems.ElementAt(0);
            foreach (int i in elems.Skip(1))
            {
                formula2Bis += ",\'" + nomSheet + "\'!$" + colLegende + "$" + i;
            }
            f2.Text = formula2Bis;

            Chart.SeriesText st = cc.ChartSpace.Descendants<Chart.SeriesText>().First();

            st.StringReference.Formula.Text = "";
        }

        public static void fixChartTitle(ChartPart cp, string titre)
        {
            DocumentFormat.OpenXml.Drawing.Run r = cp.ChartSpace.Descendants<DocumentFormat.OpenXml.Drawing.Run>().First();
            r.Text.Text = titre;
        }
#endregion

        //calcul dans li et col les dimensions du plus grand chart
        public static void maxDimChart(WorksheetPart wsp, out int li, out int col)
        {
            Draw.WorksheetDrawing wsd = wsp.DrawingsPart.WorksheetDrawing;
            int maxLi=0, maxCol=0;
            foreach (Draw.TwoCellAnchor tca in wsd.Elements<Draw.TwoCellAnchor>())
            {
                int r = Convert.ToInt32(tca.ToMarker.RowId.Text) - Convert.ToInt32(tca.FromMarker.RowId.Text);
                int c = Convert.ToInt32(tca.ToMarker.ColumnId.Text) - Convert.ToInt32(tca.FromMarker.ColumnId.Text);
                
                if (r > maxLi)
                    maxLi = r;
                if (c > maxCol)
                    maxCol = c;
            }
            li = maxLi+1;
            col = maxCol+1;
        }

       public static WorksheetPart getWorksheetPartByName(SpreadsheetDocument document, string sheetName)
        {
           Sheet res=null;
           Sheets sh = document.WorkbookPart.Workbook.Sheets;
            foreach (Sheet s in sh)
            {
                if (s.Name == sheetName)
                    res = s;
            }

            if (res == null)
                return null;
            
            string relationshipId = res.Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(relationshipId);

            return worksheetPart;
        }

       public static WorksheetPart addWorksheetPart(SpreadsheetDocument document, string name)
        {
            // Open the document for editing.
            
            // Add a blank WorksheetPart.
            WorksheetPart newWorksheetPart = document.WorkbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());

            Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = document.WorkbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Give the new worksheet a name.
            string sheetName = name;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);

            return newWorksheetPart;
        }
       


       public static Sheet getSheetByName(WorkbookPart workbookPart, string name)
       {return workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name.Value.Equals(name)).First();}

        public static void majMetaChart(ChartPart cc, List<string> formules)
       {
            if (cc.ChartSpace.Descendants().OfType<Chart.BarChart>().Count() != 0)
                majBarChart(cc, formules);
            else if (cc.ChartSpace.Descendants().OfType<Chart.Bar3DChart>().Count() != 0)
                majBarChart(cc, formules);
            else if (cc.ChartSpace.Descendants().OfType<Chart.LineChart>().Count() != 0)
                majLineChart(cc, formules);
            else if (cc.ChartSpace.Descendants().OfType<Chart.Line3DChart>().Count() != 0)
                majLineChart(cc, formules);
            else if (cc.ChartSpace.Descendants().OfType<Chart.ScatterChart>().Count() != 0)
                majScatterChart(cc, formules);
            else
                majAreaChart(cc, formules);
       }

        public static void majBarChart(ChartPart cc, List<string> formules)
        {
           var bc = (OpenXmlElement)null;
           if (cc.ChartSpace.Descendants().OfType<Chart.BarChart>().Count() != 0)
               bc = cc.ChartSpace.Descendants<Chart.BarChart>().First();
           else
               bc = cc.ChartSpace.Descendants<Chart.Bar3DChart>().First();

           for (int j = 0; j < (formules.Count - 1) / 2; j++)
           {
                Chart.BarChartSeries newSerie = (Chart.BarChartSeries)bc.Elements<Chart.BarChartSeries>().First().CloneNode(true);

                newSerie.SeriesText.StringReference.Formula.Text = formules[2 * j + 1];
                newSerie.Index.Val = (uint)j;
                newSerie.Order.Val = (uint)j;
                newSerie.Descendants<Chart.CategoryAxisData>().First().NumberReference.Formula.Text = formules[0];
                newSerie.Descendants<Chart.Values>().First().NumberReference.Formula.Text = formules[2 * j + 2];

                bc.Append(newSerie);
            }
            bc.Elements<Chart.BarChartSeries>().First().Remove();
        }

        public static void majLineChart(ChartPart cc, List<string> formules)
        {
           var bc = (OpenXmlElement)null;
           if (cc.ChartSpace.Descendants().OfType<Chart.LineChart>().Count() != 0)
               bc = cc.ChartSpace.Descendants<Chart.LineChart>().First();
           else
               bc = cc.ChartSpace.Descendants<Chart.Line3DChart>().First();

           for (int j = 0; j < (formules.Count - 1) / 2; j++)
           {
                Chart.LineChartSeries newSerie = (Chart.LineChartSeries)bc.Elements<Chart.LineChartSeries>().First().CloneNode(true);

                newSerie.SeriesText.StringReference.Formula.Text = formules[2 * j + 1];
                newSerie.Index.Val = (uint)j;
                newSerie.Order.Val = (uint)j;
                newSerie.Descendants<Chart.CategoryAxisData>().First().NumberReference.Formula.Text = formules[0];
                newSerie.Descendants<Chart.Values>().First().NumberReference.Formula.Text = formules[2 * j + 2];

                bc.Append(newSerie);
            }
            bc.Elements<Chart.LineChartSeries>().First().Remove();
        }

        public static void majScatterChart(ChartPart cc, List<string> formules)
        {
            Chart.ScatterChart bc = cc.ChartSpace.Descendants<Chart.ScatterChart>().First();

            for (int j = 0; j < (formules.Count - 1) / 2; j++)
            {
                Chart.ScatterChartSeries newSerie = (Chart.ScatterChartSeries)bc.Elements<Chart.ScatterChartSeries>().First().CloneNode(true);

                newSerie.SeriesText.StringReference.Formula.Text = formules[2 * j + 1];
                newSerie.Index.Val = (uint)j;
                newSerie.Order.Val = (uint)j;
                newSerie.Descendants<Chart.CategoryAxisData>().First().NumberReference.Formula.Text = formules[0];
                newSerie.Descendants<Chart.Values>().First().NumberReference.Formula.Text = formules[2 * j + 2];

                bc.Append(newSerie);
            }
            bc.Elements<Chart.ScatterChartSeries>().First().Remove();
        }

        public static void majAreaChart(ChartPart cc, List<string> formules)
       {
           var bc = (OpenXmlElement)null;
           if (cc.ChartSpace.Descendants().OfType<Chart.AreaChart>().Count() != 0)
               bc = cc.ChartSpace.Descendants<Chart.AreaChart>().First();
           else
               bc = cc.ChartSpace.Descendants<Chart.Area3DChart>().First();

           for (int j = 0; j < (formules.Count - 1) / 2; j++)
           {
               Chart.AreaChartSeries newSerie = (Chart.AreaChartSeries)bc.Elements<Chart.AreaChartSeries>().First().CloneNode(true);

               newSerie.SeriesText.StringReference.Formula.Text = formules[2 * j + 1];
               newSerie.Index.Val = (uint)j;
               newSerie.Order.Val = (uint)j;
               newSerie.Descendants<Chart.CategoryAxisData>().First().NumberReference.Formula.Text = formules[0];
               newSerie.Descendants<Chart.Values>().First().NumberReference.Formula.Text = formules[2 * j + 2];

               bc.Append(newSerie);
           }
           bc.Elements<Chart.AreaChartSeries>().First().Remove();
       }

       //public static Chart.BarChartSeries genBarChartSeries()
       //{
       //     Chart.BarChartSeries b = new Chart.BarChartSeries();
       //     Chart.Values v = new Chart.Values() { NumberReference = new Chart.NumberReference() { Formula = new Chart.Formula() { Text = ""} } };
       //     Chart.CategoryAxisData c = new Chart.CategoryAxisData() { NumberReference = new Chart.NumberReference() { Formula = new Chart.Formula() { Text = "" } } };
       //     b.SeriesText = new Chart.SeriesText() { StringReference = new Chart.StringReference() { Formula = new Chart.Formula() { Text = "" } } };
       //     b.Index = new Chart.Index() { Val = 0 };
       //     b.Order = new Chart.Order() { Val = 0 };
       //    b.Append(v,c);
       //    return b;
       //}

        /*
       public static void majMetaChartGen(ChartPart cc, List<string> formules)
       {

           
           Chart.BarChart bc = cc.ChartSpace.Descendants<Chart.BarChart>().First();

           for (int j = 0; j < (formules.Count - 1) / 2; j++)
           {
               Chart.BarChartSeries newSerie = (Chart.BarChartSeries)bc.Elements<Chart.BarChartSeries>().First().CloneNode(true);

               newSerie.SeriesText.StringReference.Formula.Text = formules[2 * j + 1];
               //newSerie.Descendants<CategoryAxisData>().First().Remove();

               newSerie.Descendants<Chart.CategoryAxisData>().First().NumberReference.Formula.Text = formules[0];
               newSerie.Descendants<Chart.Values>().First().NumberReference.Formula.Text = formules[2 * j + 2];


               newSerie.Index.Val = (uint)j;
               newSerie.Order.Val = (uint)j;

               bc.Append(newSerie);
           }
           bc.Elements<Chart.BarChartSeries>().First().Remove();
       }
        */



        //Tableau
        public static void AddTableDefinitionPart(WorksheetPart part, List<string> titres, int nbLi, int nbCol, int indice)
        {
            TableDefinitionPart tableDefinitionPart1 = part.AddNewPart<TableDefinitionPart>("vId1");
            GenerateTableDefinitionPart1Content(tableDefinitionPart1, titres, nbLi, nbCol, indice);
        }

        // Generates content of tableDefinitionPart1.
        public static void GenerateTableDefinitionPart1Content(TableDefinitionPart tableDefinitionPart1, List<string> titres, int nbLi, int nbCol, int indice)
        {
            string refe = "A1:" + Ultimate.headerColumns[nbCol - 1] + nbLi;

            Table table1 = new Table() { Id = (uint) indice, Name = "Table1", DisplayName = "Table"+indice, Reference = refe, TotalsRowShown = false };
            AutoFilter autoFilter1 = new AutoFilter() { Reference = refe };

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
            table1.Append(tableColumns1);
            table1.Append(tableStyleInfo1);
            tableDefinitionPart1.Table = table1;
        }
        
        
        
        // Generates content of drawingsPart1.
        public static void GenerateDrawingsPart1Content(DrawingsPart drawingsPart1)
        {
            Draw.WorksheetDrawing worksheetDrawing1 = new Draw.WorksheetDrawing();
            worksheetDrawing1.AddNamespaceDeclaration("xdr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
            worksheetDrawing1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");

            Draw.TwoCellAnchor twoCellAnchor1 = new Draw.TwoCellAnchor();

            Draw.FromMarker fromMarker1 = new Draw.FromMarker();
            Draw.ColumnId columnId1 = new Draw.ColumnId();
            columnId1.Text = "3";
            Draw.ColumnOffset columnOffset1 = new Draw.ColumnOffset();
            columnOffset1.Text = "0";
            Draw.RowId rowId1 = new Draw.RowId();
            rowId1.Text = "11";
            Draw.RowOffset rowOffset1 = new Draw.RowOffset();
            rowOffset1.Text = "114300";

            fromMarker1.Append(columnId1);
            fromMarker1.Append(columnOffset1);
            fromMarker1.Append(rowId1);
            fromMarker1.Append(rowOffset1);

            Draw.ToMarker toMarker1 = new Draw.ToMarker();
            Draw.ColumnId columnId2 = new Draw.ColumnId();
            columnId2.Text = "9";
            Draw.ColumnOffset columnOffset2 = new Draw.ColumnOffset();
            columnOffset2.Text = "0";
            Draw.RowId rowId2 = new Draw.RowId();
            rowId2.Text = "26";
            Draw.RowOffset rowOffset2 = new Draw.RowOffset();
            rowOffset2.Text = "0";

            toMarker1.Append(columnId2);
            toMarker1.Append(columnOffset2);
            toMarker1.Append(rowId2);
            toMarker1.Append(rowOffset2);

            Draw.GraphicFrame graphicFrame1 = new Draw.GraphicFrame() { Macro = "" };

            Draw.NonVisualGraphicFrameProperties nonVisualGraphicFrameProperties1 = new Draw.NonVisualGraphicFrameProperties();
            Draw.NonVisualDrawingProperties nonVisualDrawingProperties1 = new Draw.NonVisualDrawingProperties() { Id = (UInt32Value)3U, Name = "Graphique 2" };
            Draw.NonVisualGraphicFrameDrawingProperties nonVisualGraphicFrameDrawingProperties1 = new Draw.NonVisualGraphicFrameDrawingProperties();

            nonVisualGraphicFrameProperties1.Append(nonVisualDrawingProperties1);
            nonVisualGraphicFrameProperties1.Append(nonVisualGraphicFrameDrawingProperties1);

            Draw.Transform transform1 = new Draw.Transform();
            A.Offset offset1 = new A.Offset() { X = 0L, Y = 0L };
            A.Extents extents1 = new A.Extents() { Cx = 0L, Cy = 0L };

            transform1.Append(offset1);
            transform1.Append(extents1);

            A.Graphic graphic1 = new A.Graphic();

            A.GraphicData graphicData1 = new A.GraphicData() { Uri = "http://schemas.openxmlformats.org/drawingml/2006/chart" };

            Chart.ChartReference chartReference1 = new Chart.ChartReference() { Id = "rId1" };
            chartReference1.AddNamespaceDeclaration("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
            chartReference1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            graphicData1.Append(chartReference1);

            graphic1.Append(graphicData1);

            graphicFrame1.Append(nonVisualGraphicFrameProperties1);
            graphicFrame1.Append(transform1);
            graphicFrame1.Append(graphic1);
            Draw.ClientData clientData1 = new Draw.ClientData();

            twoCellAnchor1.Append(fromMarker1);
            twoCellAnchor1.Append(toMarker1);
            twoCellAnchor1.Append(graphicFrame1);
            twoCellAnchor1.Append(clientData1);

            worksheetDrawing1.Append(twoCellAnchor1);

            drawingsPart1.WorksheetDrawing = worksheetDrawing1;
        }

        // Generates content of chartPart1.
        public static void GenerateChartPart1Content(ChartPart chartPart1)
        {
            Chart.ChartSpace chartSpace1 = new Chart.ChartSpace();
            chartSpace1.AddNamespaceDeclaration("c", "http://schemas.openxmlformats.org/drawingml/2006/chart");
            chartSpace1.AddNamespaceDeclaration("a", "http://schemas.openxmlformats.org/drawingml/2006/main");
            chartSpace1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            Chart.EditingLanguage editingLanguage1 = new Chart.EditingLanguage() { Val = "fr-FR" };

            Chart.Chart chart1 = new Chart.Chart();

            Chart.Title title1 = new Chart.Title();
            Chart.Layout layout1 = new Chart.Layout();

            title1.Append(layout1);

            Chart.PlotArea plotArea1 = new Chart.PlotArea();
            Chart.Layout layout2 = new Chart.Layout();

            Chart.PieChart pieChart1 = new Chart.PieChart();
            Chart.VaryColors varyColors1 = new Chart.VaryColors() { Val = true };

            Chart.PieChartSeries pieChartSeries1 = new Chart.PieChartSeries();
            Chart.Index index1 = new Chart.Index() { Val = (UInt32Value)0U };
            Chart.Order order1 = new Chart.Order() { Val = (UInt32Value)0U };

            Chart.SeriesText seriesText1 = new Chart.SeriesText();

            Chart.StringReference stringReference1 = new Chart.StringReference();
            Chart.Formula formula1 = new Chart.Formula();
            formula1.Text = "Feuil1!$D$7";

            Chart.StringCache stringCache1 = new Chart.StringCache();
            Chart.PointCount pointCount1 = new Chart.PointCount() { Val = (UInt32Value)1U };

            stringCache1.Append(pointCount1);

            stringReference1.Append(formula1);
            stringReference1.Append(stringCache1);

            seriesText1.Append(stringReference1);

            Chart.CategoryAxisData categoryAxisData1 = new Chart.CategoryAxisData();

            Chart.NumberReference numberReference1 = new Chart.NumberReference();
            Chart.Formula formula2 = new Chart.Formula();
            formula2.Text = "Feuil1!$E$6:$H$6";

            Chart.NumberingCache numberingCache1 = new Chart.NumberingCache();
            Chart.FormatCode formatCode1 = new Chart.FormatCode();
            formatCode1.Text = "General";
            Chart.PointCount pointCount2 = new Chart.PointCount() { Val = (UInt32Value)4U };

            numberingCache1.Append(formatCode1);
            numberingCache1.Append(pointCount2);

            numberReference1.Append(formula2);
            numberReference1.Append(numberingCache1);

            categoryAxisData1.Append(numberReference1);

            Chart.Values values1 = new Chart.Values();

            Chart.NumberReference numberReference2 = new Chart.NumberReference();
            Chart.Formula formula3 = new Chart.Formula();
            formula3.Text = "Feuil1!$E$7:$H$7";

            Chart.NumberingCache numberingCache2 = new Chart.NumberingCache();
            Chart.FormatCode formatCode2 = new Chart.FormatCode();
            formatCode2.Text = "General";
            Chart.PointCount pointCount3 = new Chart.PointCount() { Val = (UInt32Value)4U };

            numberingCache2.Append(formatCode2);
            numberingCache2.Append(pointCount3);

            numberReference2.Append(formula3);
            numberReference2.Append(numberingCache2);

            values1.Append(numberReference2);

            pieChartSeries1.Append(index1);
            pieChartSeries1.Append(order1);
            pieChartSeries1.Append(seriesText1);
            pieChartSeries1.Append(categoryAxisData1);
            pieChartSeries1.Append(values1);
            Chart.FirstSliceAngle firstSliceAngle1 = new Chart.FirstSliceAngle() { Val = (UInt16Value)0U };

            pieChart1.Append(varyColors1);
            pieChart1.Append(pieChartSeries1);
            pieChart1.Append(firstSliceAngle1);

            plotArea1.Append(layout2);
            plotArea1.Append(pieChart1);

            Chart.Legend legend1 = new Chart.Legend();
            Chart.LegendPosition legendPosition1 = new Chart.LegendPosition() { Val = Chart.LegendPositionValues.Right };
            Chart.Layout layout3 = new Chart.Layout();

            legend1.Append(legendPosition1);
            legend1.Append(layout3);
            Chart.PlotVisibleOnly plotVisibleOnly1 = new Chart.PlotVisibleOnly() { Val = true };

            chart1.Append(title1);
            chart1.Append(plotArea1);
            chart1.Append(legend1);
            chart1.Append(plotVisibleOnly1);

            Chart.PrintSettings printSettings1 = new Chart.PrintSettings();
            Chart.HeaderFooter headerFooter1 = new Chart.HeaderFooter();
            Chart.PageMargins pageMargins1 = new Chart.PageMargins() { Left = 0.70000000000000007D, Right = 0.70000000000000007D, Top = 0.75000000000000011D, Bottom = 0.75000000000000011D, Header = 0.30000000000000004D, Footer = 0.30000000000000004D };
            Chart.PageSetup pageSetup1 = new Chart.PageSetup();

            printSettings1.Append(headerFooter1);
            printSettings1.Append(pageMargins1);
            printSettings1.Append(pageSetup1);

            chartSpace1.Append(editingLanguage1);
            chartSpace1.Append(chart1);
            chartSpace1.Append(printSettings1);

            chartPart1.ChartSpace = chartSpace1;
        }



        #region inutilisé, mais utile pour la création de Chart et de ChartSerie
        public static Chart.BarChart GenerateBarChart(uint ax1, uint ax2)
        {
            Chart.BarChart barChart1 = new Chart.BarChart();
            Chart.BarDirection barDirection1 = new Chart.BarDirection() { Val = Chart.BarDirectionValues.Column };
            Chart.BarGrouping barGrouping1 = new Chart.BarGrouping() { Val = Chart.BarGroupingValues.Clustered };
            Chart.AxisId axisId1 = new Chart.AxisId() { Val = ax1 };
            Chart.AxisId axisId2 = new Chart.AxisId() { Val = ax2 };

            barChart1.Append(barDirection1);
            barChart1.Append(barGrouping1);
            barChart1.Append(axisId1);
            barChart1.Append(axisId2);
            return barChart1;
        }

        public static Chart.BarChartSeries GenerateBarChartSeries()
        {
            Chart.BarChartSeries barChartSeries1 = new Chart.BarChartSeries();
            Chart.Index index1 = new Chart.Index() { Val = (UInt32Value)1U };
            Chart.Order order1 = new Chart.Order() { Val = (UInt32Value)1U };

            Chart.CategoryAxisData categoryAxisData1 = new Chart.CategoryAxisData();

            Chart.StringReference stringReference1 = new Chart.StringReference();
            Formula formula1 = new Formula();
            formula1.Text = "Feuil1!$C$2:$C$7";

            Chart.StringCache stringCache1 = new Chart.StringCache();
            Chart.PointCount pointCount1 = new Chart.PointCount() { Val = (UInt32Value)6U };

            Chart.StringPoint stringPoint1 = new Chart.StringPoint() { Index = (UInt32Value)0U };
            Chart.NumericValue numericValue1 = new Chart.NumericValue();
            numericValue1.Text = "a";

            stringPoint1.Append(numericValue1);

            Chart.StringPoint stringPoint2 = new Chart.StringPoint() { Index = (UInt32Value)1U };
            Chart.NumericValue numericValue2 = new Chart.NumericValue();
            numericValue2.Text = "b";

            stringPoint2.Append(numericValue2);

            Chart.StringPoint stringPoint3 = new Chart.StringPoint() { Index = (UInt32Value)2U };
            Chart.NumericValue numericValue3 = new Chart.NumericValue();
            numericValue3.Text = "c";

            stringPoint3.Append(numericValue3);

            Chart.StringPoint stringPoint4 = new Chart.StringPoint() { Index = (UInt32Value)3U };
            Chart.NumericValue numericValue4 = new Chart.NumericValue();
            numericValue4.Text = "d";

            stringPoint4.Append(numericValue4);

            Chart.StringPoint stringPoint5 = new Chart.StringPoint() { Index = (UInt32Value)4U };
            Chart.NumericValue numericValue5 = new Chart.NumericValue();
            numericValue5.Text = "e";

            stringPoint5.Append(numericValue5);

            Chart.StringPoint stringPoint6 = new Chart.StringPoint() { Index = (UInt32Value)5U };
            Chart.NumericValue numericValue6 = new Chart.NumericValue();
            numericValue6.Text = "f";

            stringPoint6.Append(numericValue6);

            /*stringCache1.Append(pointCount1);
            stringCache1.Append(stringPoint1);
            stringCache1.Append(stringPoint2);
            stringCache1.Append(stringPoint3);
            stringCache1.Append(stringPoint4);
            stringCache1.Append(stringPoint5);
            stringCache1.Append(stringPoint6);
            */
            stringReference1.Append(formula1);
            stringReference1.Append(stringCache1);

            categoryAxisData1.Append(stringReference1);

            Chart.Values values1 = new Chart.Values();

            Chart.NumberReference numberReference1 = new Chart.NumberReference();
            Formula formula2 = new Formula();
            formula2.Text = "Feuil1!$E$2:$E$7";

            Chart.NumberingCache numberingCache1 = new Chart.NumberingCache();
            Chart.FormatCode formatCode1 = new Chart.FormatCode();
            formatCode1.Text = "General";
            Chart.PointCount pointCount2 = new Chart.PointCount() { Val = (UInt32Value)6U };

            Chart.NumericPoint numericPoint1 = new Chart.NumericPoint() { Index = (UInt32Value)0U };
            Chart.NumericValue numericValue7 = new Chart.NumericValue();
            numericValue7.Text = "80";

            numericPoint1.Append(numericValue7);

            Chart.NumericPoint numericPoint2 = new Chart.NumericPoint() { Index = (UInt32Value)1U };
            Chart.NumericValue numericValue8 = new Chart.NumericValue();
            numericValue8.Text = "90";

            numericPoint2.Append(numericValue8);

            Chart.NumericPoint numericPoint3 = new Chart.NumericPoint() { Index = (UInt32Value)2U };
            Chart.NumericValue numericValue9 = new Chart.NumericValue();
            numericValue9.Text = "60";

            numericPoint3.Append(numericValue9);

            Chart.NumericPoint numericPoint4 = new Chart.NumericPoint() { Index = (UInt32Value)3U };
            Chart.NumericValue numericValue10 = new Chart.NumericValue();
            numericValue10.Text = "80";

            numericPoint4.Append(numericValue10);

            Chart.NumericPoint numericPoint5 = new Chart.NumericPoint() { Index = (UInt32Value)4U };
            Chart.NumericValue numericValue11 = new Chart.NumericValue();
            numericValue11.Text = "100";

            numericPoint5.Append(numericValue11);

            Chart.NumericPoint numericPoint6 = new Chart.NumericPoint() { Index = (UInt32Value)5U };
            Chart.NumericValue numericValue12 = new Chart.NumericValue();
            numericValue12.Text = "100";

            numericPoint6.Append(numericValue12);
            /*
            numberingCache1.Append(formatCode1);
            numberingCache1.Append(pointCount2);
            numberingCache1.Append(numericPoint1);
            numberingCache1.Append(numericPoint2);
            numberingCache1.Append(numericPoint3);
            numberingCache1.Append(numericPoint4);
            numberingCache1.Append(numericPoint5);
            numberingCache1.Append(numericPoint6);
            */
            numberReference1.Append(formula2);
            numberReference1.Append(numberingCache1);//

            values1.Append(numberReference1);

            barChartSeries1.Append(index1);
            barChartSeries1.Append(order1);
            barChartSeries1.Append(categoryAxisData1);
            barChartSeries1.Append(values1);

            barChartSeries1.SeriesText = new Chart.SeriesText() { StringReference = 
                new Chart.StringReference() { Formula = new Chart.Formula()  } };

            categoryAxisData1.NumberReference = new Chart.NumberReference() { Formula = new Chart.Formula()};
            values1.NumberReference = new Chart.NumberReference() { Formula = new Chart.Formula() };

            return barChartSeries1;
        }

        public static void temporaire(ChartPart cp, List<string> formules)
        {

            Chart.BarChart bc = cp.ChartSpace.Descendants<Chart.BarChart>().First();


            for (int j = 0; j < (formules.Count - 1) / 2; j++)
            {
                Chart.BarChartSeries newSerie = GenerateBarChartSeries();

                newSerie.SeriesText.StringReference.Formula.Text = formules[2 * j + 1];
                newSerie.Index.Val = (uint)j;
                newSerie.Order.Val = (uint)j;
                newSerie.Descendants<Chart.CategoryAxisData>().First().NumberReference.Formula.Text = formules[0];
                newSerie.Descendants<Chart.Values>().First().NumberReference.Formula.Text = formules[2 * j + 2];

                bc.Append(newSerie);
            }
            bc.Elements<Chart.BarChartSeries>().First().Remove();
        }

        public static void temporaire2(ChartPart cp)
        {

            Chart.PlotArea pa= cp.ChartSpace.Descendants<Chart.PlotArea>().First();

            uint valAx = pa.GetFirstChild<Chart.ValueAxis>().AxisId.Val;
            uint catAx = pa.GetFirstChild<Chart.CategoryAxis>().AxisId.Val;

            pa.Descendants<Chart.BarChart>().First().Remove();

            Chart.BarChart bc = GenerateBarChart(catAx,valAx);
            pa.Append(bc);

        }
        #endregion

    }
}
