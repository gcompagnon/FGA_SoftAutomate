using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

using System.IO;

using System.Data;

using Draw = DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Chart = DocumentFormat.OpenXml.Drawing.Charts;

namespace Reporting_Excel
{
    public class Ultimate
    {
        #region Parametres

        public List<uint[]> lineStyles = new List<uint[]>();
        public string template;
        public string copie;
        public string stylePath;
        public string graphPath;
        public int indiceLibelle;
        public int indiceRubrique;
        public int indiceGraph = -1;
        public int indiceSyle = -1;
        public int nbColonneConfig;

        public static string[] headerColumns = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP" };
        #endregion


        public Ultimate(string tmplt,string filePath, string stlPath,string grphPath)
        {
            template = tmplt;
            copie = filePath;
            stylePath = stlPath;
            graphPath = grphPath;
        }

        public void obtentionBornesColonnes(DataTable dt)
        {
            indiceSyle = -1;
            indiceGraph = -1;
            indiceLibelle = 0;
            indiceRubrique = 0;
            int i = 0;
            int j = 0;
            foreach (DataColumn DC in dt.Columns)
            {
                if (DC.ColumnName == "graph")
                    j++;
                if (DC.ColumnName == "style")
                    j++;
                if (DC.ColumnName == "style")
                    indiceSyle = i;
                if (DC.ColumnName == "graph")
                    indiceGraph = i;
                if (DC.ColumnName == "rubrique")
                    indiceRubrique = i;
                if (DC.ColumnName == "libelle")
                    indiceLibelle = i;
                i++;
            }
            if (indiceRubrique == 0)
                indiceRubrique = indiceLibelle;
            nbColonneConfig = j;
        }

        //Remplissage de la feuille en mode Report (mode normal)
        public void creerFeuille(WorksheetPart worksheetPart, DataTable dt)
        {
            //Atention  ne pas oublier l'obtention des bornes
            obtentionBornesColonnes(dt);

            SheetData sd = worksheetPart.Worksheet.GetFirstChild<SheetData>();

            Row r = XcelWin.creerTitres(dt,1);
            sd.AppendChild(r);

            int index = 2;

            r = new Row();
            foreach (DataRow item in dt.Rows)
            {
                uint[] a = new uint[6];
                if (indiceSyle != -1 && stylePath!=null)
                {
                    int tmp = Convert.ToInt32(item.ItemArray[indiceSyle]);
                    a = lineStyles[tmp];
                }

                if(indiceRubrique==0 && indiceLibelle==0)
                    r=XcelWin.creerLigne(item, index, a[0],nbColonneConfig);
                else
                    r = creerLigne(item, index, a, nbColonneConfig);

                sd.AppendChild(r);
                index++;
            }
        }

        //Remplissage de la feuille en mode monoSheet
        public SheetData creerFeuille2(WorksheetPart worksheetPart, DataSet ds)
        {
            SheetData sd = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            int index = 1;
            foreach (DataTable dt in ds.Tables)
            {
                Row r = new Row();
                if (!dt.Columns[0].ColumnName.Contains("Column"))
                {
                    r = XcelWin.creerTitres(dt, index);
                    sd.AppendChild(r);
                    index++;
                }

                r = new Row();
                foreach (DataRow item in dt.Rows)
                {
                    uint[] a = { 0, 0, 0, 0, 0, 0 };
                    r = creerLigne2(item, index, a);
                    sd.AppendChild(r);
                    index++;
                }
                index++;
            }
            return sd;
        }

        public void creerTableau(WorksheetPart worksheetPart, DataTable dt, int indice)
        {
            obtentionBornesColonnes(dt);

            //Obtention des titres pour la creation des colonnes de la tables
            List<string> titres = new List<string>();
            foreach (DataColumn t in dt.Columns)
            {
                if (t.ColumnName != "style" && t.ColumnName != "graph")
                {
                    if (t.ColumnName == " ")
                        titres.Add("%");
                    else
                       titres.Add(t.ColumnName);
                }
            }

            //ecriture des titres
            SheetData sd = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            Row rr = XcelWin.creerTitres(dt,1);
            sd.AppendChild(rr);

            //ecriture des données
            int index = 2;
            foreach (DataRow r in dt.Rows)
            {
                rr = XcelWin.creerLigne(r, index,0,nbColonneConfig);
                sd.AppendChild(rr);
                index++;
            }

            XcelWin.AddTableDefinitionPart(worksheetPart, titres, dt.Rows.Count + 1, dt.Columns.Count-nbColonneConfig, indice);
            TableParts tableParts1 = new TableParts() { Count = (UInt32Value)1U };
            TablePart tablePart1 = new TablePart() { Id = "vId1" };
            tableParts1.Append(tablePart1);

            worksheetPart.Worksheet.Append(tableParts1);
        }

        public WorksheetPart CopySheet(WorkbookPart workbookPart, WorksheetPart sourceSheetPart, string clonedSheetName)
        {
            //Il faut uiliser AddPart() qui est bien plus puissante que AddNewPart pour le clonage de WorksheetPart
            SpreadsheetDocument tempSheet = SpreadsheetDocument.Create(new MemoryStream(), SpreadsheetDocumentType.Workbook);
            WorkbookPart tempWorkbookPart = tempSheet.AddWorkbookPart();
            WorksheetPart tempWorksheetPart = tempWorkbookPart.AddPart<WorksheetPart>(sourceSheetPart);
            //Ajoute le clone et ses fils au workbook
            WorksheetPart clonedSheet = workbookPart.AddPart<WorksheetPart>(tempWorksheetPart);

            //Table definition parts are somewhat special and need unique ids...so let's make an id based on count
            int numTableDefParts = sourceSheetPart.GetPartsCountOfType<TableDefinitionPart>();
            int tableId = numTableDefParts;
            //Clean up table definition parts (tables need unique ids)
            if (numTableDefParts != 0)
            {
                foreach (TableDefinitionPart tableDefPart in clonedSheet.TableDefinitionParts)
                {
                    tableId++;
                    tableDefPart.Table.Id = (uint)tableId;
                    tableDefPart.Table.DisplayName = "CopiedTable" + tableId;
                    tableDefPart.Table.Name = "CopiedTable" + tableId;
                    tableDefPart.Table.Save();
                }
            }
            //There can only be one sheet that has focus
            SheetViews views = clonedSheet.Worksheet.GetFirstChild<SheetViews>();
            if (views != null)
            {
                views.Remove();
                clonedSheet.Worksheet.Save();
            }

            //Add new sheet to main workbook part
            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            Sheet copiedSheet = new Sheet();
            copiedSheet.Name = clonedSheetName;
            copiedSheet.Id = workbookPart.GetIdOfPart(clonedSheet);
            copiedSheet.SheetId = (uint)sheets.ChildElements.Count() + 5;//+5 car le template possede deja 2 feuilles avec pour id 3 et 4
            sheets.Append(copiedSheet);

            //Save Changes
            workbookPart.Workbook.Save();

            return clonedSheet;
        }
        
        //Creation d'une ligne en mode Report (normal)
        public Row creerLigne(DataRow dr, int index, uint[] style, int nbColonneConfig)
        {
            Row r = new Row();
            Cell c = new Cell();
            int i = 0;
            //Skip pour ne pas écrire la colonne de style ni la colonne graph
            foreach (var att in dr.ItemArray.Skip(nbColonneConfig))
            {
                c = new Cell();
                //Partie gauche
                if (i < indiceRubrique - nbColonneConfig)
                {
                    try { c = XcelWin.createCellFloat(headerColumns[i], index, Convert.ToSingle(att), style[0]); }
                    catch (Exception) { string tmp; if (att == DBNull.Value) { tmp = ""; } else { tmp = (string)att; } c = XcelWin.createTextCell(headerColumns[i], index, tmp, style[0]); }
                }
                //debut partie texte
                else if (i == indiceRubrique - nbColonneConfig)
                {
                    c = XcelWin.createTextCell(headerColumns[i], index, (string)att, style[1]);
                }
                //partie texte
                else if (indiceRubrique - nbColonneConfig < i && i < indiceLibelle - nbColonneConfig)
                {
                    c = XcelWin.createTextCell(headerColumns[i], index, (string)att, style[2]);
                }
                //Fin partie texte
                else if (i == indiceLibelle - nbColonneConfig)
                {
                    c = XcelWin.createTextCell(headerColumns[i], index, (string)att, style[3]);
                }
                //Derniere colonne
                else if (i == dr.ItemArray.Count() - nbColonneConfig - 1)
                {
                    double resultat;
                    if (att == DBNull.Value)
                        resultat = 0.0;
                    else
                        resultat = (double)att;
                    c = XcelWin.createCellDouble(headerColumns[i], index, resultat, style[5]);
                }
                else
                {
                    double resultat;
                    if (att == DBNull.Value)
                        resultat = 0.0;
                    else
                        resultat = Convert.ToDouble(att);
                    c = XcelWin.createCellDouble(headerColumns[i], index, resultat, style[4]);
                }
                r.AppendChild(c);
                i++;
            }

            return r;
        }
        
        //Creation d'une ligne en mode monoSheet
        public Row creerLigne2(DataRow dr, int index, uint[] style)
        {
            Row r = new Row() { RowIndex = (uint)index };
            Cell c = new Cell();
            int i = 0;
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

        public void execution(DataSet dt, string date, string[] sheetNames,bool monoFeuille=false)
        {
            if (monoFeuille)
            {
                //Copie du template et ouverture du fichier
                System.IO.File.Copy(template, copie, true);
                SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(copie, true);

                //Access the main Workbook part, which contains all references.
                WorkbookPart workbookPart = myWorkbook.WorkbookPart;
                WorksheetPart wp = XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1");
                creerFeuille2(wp, dt);

                myWorkbook.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
                myWorkbook.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;

                //Sauvegarde du workbook et fermeture de l'objet fichier
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
            else
            {
                //Copie du template et ouverture du fichier
                System.IO.File.Copy(template, copie, true);
                SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(copie, true);

                //Access the main Workbook part, which contains all references.
                WorkbookPart workbookPart = myWorkbook.WorkbookPart;

                if (stylePath != null)
                {//Importation des styles contenus dans le fichier de style
                    //Copie du StyleSheet existant pour ne pas perdre les elements du template
                    var wsp = workbookPart.WorkbookStylesPart;
                    Stylesheet ss = wsp.Stylesheet;
                    // Comme on a fait une copie, on doit supprimer le WorkbookStylePart oiginal
                    workbookPart.DeletePart(wsp);
                    // Et on y ajoute le notre
                    WorkbookStylesPart wbsp = workbookPart.AddNewPart<WorkbookStylesPart>();
                    //Importation des styles contenus dans le fichier de style
                    Interface itf = new Interface();
                    itf.lectureFichier(ss, stylePath);
                    wbsp.Stylesheet = ss;
                    wbsp.Stylesheet.Save();
                    lineStyles = itf.lineStyles;
                }

                //WorksheetPart que l'on va copier 
                WorksheetPart worksheetPart = XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1");

                //Obtention des conf de graph : modeles, titres, colonnes ...
                Dictionary<string, Object> d = new Dictionary<string, Object>();
                if (graphPath != null)
                    d = Interface.lectureConfGraph(graphPath);
                WorksheetPart graphs = XcelWin.getWorksheetPartByName(myWorkbook, "Graphs");

                //Dimension du plus grand des graphs (pour les placer sans superposition par la suite)
                //maxJ pour savoir ou placer les metagraphs par la suite
                int maxLi = -1, maxCol = -1;
                int maxJ = 0;
                try
                { XcelWin.maxDimChart(graphs, out maxLi, out maxCol); }
                catch (NullReferenceException)
                { Console.WriteLine("Aucun graph présent dans la feuille 'graph' \nIgnore toutes les operations relatives aux graphs"); }


                //Creation de chacunes des feuilles
                for (int i = 0; i < dt.Tables.Count; i++)
                {
                    //obtention du nom pour la feuille, si absent : Sheet 1, 2, 3...
                    string nom = "";
                    try { nom = sheetNames[i]; }
                    catch (Exception) { nom = "Sheet" + (i + 1); }

                    WorksheetPart wp = CopySheet(workbookPart, worksheetPart, nom);

                    int tab = -1;
                    foreach (string clef in d.Keys.Where(s => s.Contains("tableau")))
                        if (((TableauData)d[clef]).indice == i)
                            tab = i;
                    if (tab != -1)
                        creerTableau(wp, dt.Tables[i], i + 1);
                    else
                        creerFeuille(wp, dt.Tables[i]);


                    //Obtention des lignes de données des graphs
                    d = Interface.razConfGraph(d);
                    if (indiceGraph != -1)
                        obtInfosGraphs(dt.Tables[i], d, nom);
                    int j = 0;

                    //Si on a trouvé des graphs lors de l'obtention de leurs tailles
                    if (maxLi != -1)
                    {
                        //Creation des graphs qui ont les clefs de type graph
                        foreach (string clef in d.Keys.Where(s => s.Contains("graph") && ((ChartData)d[s]).data.Count != 0))
                        {
                            ChartData tmp = (ChartData)d[clef];
                            ChartPart y = XcelWin.cloneChart(graphs, tmp.nomModele, i, j, maxLi, maxCol);
                            if (tmp.colValeurs == "last")
                                XcelWin.fixChartData(y, nom, tmp.data, headerColumns[dt.Tables[i].Columns.Count - nbColonneConfig - 1], getHeaderCol(tmp.colTitres, dt.Tables[i]));
                            else
                            {
                                try { XcelWin.fixChartData(y, nom, tmp.data, getHeaderCol(tmp.colValeurs, dt.Tables[i]), getHeaderCol(tmp.colTitres, dt.Tables[i])); }
                                catch (Exception) { Console.WriteLine("Echec de mise à jour des données du graph, verifier le fichier de configuration " + clef); }
                            }
                            XcelWin.fixChartTitle(y, tmp.titre + " " + nom);
                            j++;
                        }
                        if (j > maxJ) { maxJ = j; }
                    }
                }
                //Si on a trouvé des graphs lors de l'obtention de leurs tailles
                if (maxLi != -1)
                {
                    //Creation des metagraphs
                    WorksheetPart wp2 = XcelWin.addWorksheetPart(myWorkbook, "data");
                    ecrireMetaChartData(wp2, d);
                    int k = 0;
                    foreach (string key in d.Keys.Where(s => s.Contains("meta")))
                    {
                        MetaChartData tmp = (MetaChartData)d[key];
                        try
                        {
                            ChartPart cp = XcelWin.cloneChart(graphs, tmp.nomModele, k, maxJ, maxLi, maxCol);
                            List<string> formules;
                            if(tmp.transpose)
                                formules = creaFormuleTranspose(((ChartData)d[tmp.series.First()]).data.Count, tmp.series.Count, tmp.indice);
                            else
                                formules = creaFormule(((ChartData)d[tmp.series.First()]).data.Count, tmp.series.Count, tmp.indice);
                            XcelWin.majMetaChart(cp, formules);
                            XcelWin.fixChartTitle(cp, tmp.titre);
                            k++;
                        }
                        catch (Exception) { Console.WriteLine("Impossible de copier un graph (verifiez que le graph est présent dans le template avec le bon titre) : " + tmp.nomModele); }
                    }

                    //Cache la feuille de donnée des metagraph
                    Sheet sData = XcelWin.getSheetByName(workbookPart, "data");
                    sData.State = new SheetStateValues();
                    sData.State.Value = SheetStateValues.Hidden;

                    //suppression des modèles de graphs
                    List<string> listeModele = new List<string>();
                    foreach (string key in d.Keys.Where(s => s.Contains("graph") || s.Contains("meta")))
                    {
                        string nomModele = "";
                        try { nomModele = ((ChartData)d[key]).nomModele; }
                        catch (Exception) { nomModele = ((MetaChartData)d[key]).nomModele; }
                        if (!listeModele.Contains(nomModele))
                        {
                            listeModele.Add(nomModele);
                            try { XcelWin.supprChart(graphs, nomModele); }
                            catch (Exception) { Console.WriteLine("Impossible de supprimer le modèle car introuvable : " + nomModele); }
                        }
                    }
                }

                try
                {//Ajout de la date sur la page de garde
                    Cell cell = XcelWin.createTextCell("H", 27, date, 0);
                    Row r = new Row();
                    r.RowIndex = 27;
                    r.AppendChild(cell);
                    worksheetPart = XcelWin.getWorksheetPartByName(myWorkbook, "REPORTING"); ;
                    SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    sheetData.AppendChild(r);
                }
                catch (Exception)
                {
                    Console.WriteLine("Erreur lors de l'ajout de la date sur la page de garde 'REPORTING'");
                }

                //Suppression du worksheetPart qui a servis de modele
                workbookPart.DeletePart(XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1"));
                XcelWin.getSheetByName(workbookPart, "Feuil1").Remove();

                //Sauvegarde du workbook et fermeture de l'objet fichier
                workbookPart.Workbook.Save();
                myWorkbook.Close();
            }
        }

        /*
        public void execution(DataSet dt, string date, string[] sheetNames)
        {

            //Copie du template et ouverture du fichier
            System.IO.File.Copy(template, copie, true);
            SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(copie, true);

            //Access the main Workbook part, which contains all references.
            WorkbookPart workbookPart = myWorkbook.WorkbookPart;

            if (stylePath != null)
            {//Importation des styles contenus dans le fichier de style
                //Copie du StyleSheet existant pour ne pas perdre les elements du template
                var wsp = workbookPart.WorkbookStylesPart;
                Stylesheet ss = wsp.Stylesheet;
                // Comme on a fait une copie, on doit supprimer le WorkbookStylePart oiginal
                workbookPart.DeletePart(wsp);
                // Et on y ajoute le notre
                WorkbookStylesPart wbsp = workbookPart.AddNewPart<WorkbookStylesPart>();
                //Importation des styles contenus dans le fichier de style
                Interface itf = new Interface();
                itf.lectureFichier(ss, stylePath);
                wbsp.Stylesheet = ss;
                wbsp.Stylesheet.Save();
                lineStyles = itf.lineStyles;
            }

            //WorksheetPart que l'on va copier 
            WorksheetPart worksheetPart = XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1");

            //Obtention des conf de graph : modeles, titres, colonnes ...
            Dictionary<string, Object> d = new Dictionary<string, Object>();
            if (graphPath != null)
                d = Interface.lectureConfGraph(graphPath);
            WorksheetPart graphs = XcelWin.getWorksheetPartByName(myWorkbook, "Graphs");

            //Dimension du plus grand des graphs (pour les placer sans superposition par la suite)
            //maxJ pour savoir ou placer les metagraphs par la suite
            int maxLi = -1, maxCol = -1;
            int maxJ = 0;
            try
            { XcelWin.maxDimChart(graphs, out maxLi, out maxCol); }
            catch (NullReferenceException)
            { Console.WriteLine("Aucun graph présent dans la feuille 'graph' \nIgnore toutes les operations relatives aux graphs"); }


            //Creation de chacunes des feuilles
            for (int i = 0; i < dt.Tables.Count; i++)
            {
                //obtention du nom pour la feuille, si absent : Sheet 1, 2, 3...
                string nom = "";
                try { nom = sheetNames[i]; }
                catch (Exception) { nom = "Sheet" + (i + 1); }

                WorksheetPart wp = CopySheet(workbookPart, worksheetPart, nom);

                int tab = -1;
                foreach (string clef in d.Keys.Where(s => s.Contains("tableau")))
                    if (((TableauData)d[clef]).indice == i)
                        tab = i;
                if (tab != -1)
                    creerTableau(wp, dt.Tables[i], i + 1);
                else
                    creerFeuille(wp, dt.Tables[i]);


                //Obtention des lignes de données des graphs
                d = Interface.razConfGraph(d);
                if (indiceGraph != -1)
                    obtInfosGraphs(dt.Tables[i], d, nom);
                int j = 0;

                //Si on a trouvé des graphs lors de l'obtention de leurs tailles
                if (maxLi != -1)
                {
                    //Creation des graphs qui ont les clefs de type graph
                    foreach (string clef in d.Keys.Where(s => s.Contains("graph") && ((ChartData)d[s]).data.Count != 0))
                    {
                        ChartData tmp = (ChartData)d[clef];
                        ChartPart y = XcelWin.cloneChart(graphs, tmp.nomModele, i, j, maxLi, maxCol);
                        if (tmp.colValeurs == "last")
                            XcelWin.fixChartData(y, nom, tmp.data, headerColumns[dt.Tables[i].Columns.Count - nbColonneConfig - 1], getHeaderCol(tmp.colTitres, dt.Tables[i]));
                        else
                        {
                            try { XcelWin.fixChartData(y, nom, tmp.data, getHeaderCol(tmp.colValeurs, dt.Tables[i]), getHeaderCol(tmp.colTitres, dt.Tables[i])); }
                            catch (Exception) { Console.WriteLine("Echec de mise à jour des données du graph, verifier le fichier de configuration " + clef); }
                        }
                        XcelWin.fixChartTitle(y, tmp.titre + " " + nom);
                        j++;
                    }
                    if (j > maxJ) { maxJ = j; }
                }
            }
            //Si on a trouvé des graphs lors de l'obtention de leurs tailles
            if (maxLi != -1)
            {
                //Creation des metagraphs
                WorksheetPart wp2 = XcelWin.addWorksheetPart(myWorkbook, "data");
                ecrireMetaChartData(wp2, d);
                int k = 0;
                foreach (string key in d.Keys.Where(s => s.Contains("meta")))
                {
                    MetaChartData tmp = (MetaChartData)d[key];
                    try
                    {
                        ChartPart cp = XcelWin.cloneChart(graphs, tmp.nomModele, k, maxJ, maxLi, maxCol);
                        List<string> formules;
                        if (tmp.transpose)
                            formules = creaFormuleTranspose(((ChartData)d[tmp.series.First()]).data.Count, tmp.series.Count, tmp.indice);
                        else
                            formules = creaFormule(((ChartData)d[tmp.series.First()]).data.Count, tmp.series.Count, tmp.indice);
                        XcelWin.majMetaChart(cp, formules);
                        XcelWin.fixChartTitle(cp, tmp.titre);
                        k++;
                    }
                    catch (Exception) { Console.WriteLine("Impossible de copier un graph (verifiez que le graph est présent dans le template avec le bon titre) : " + tmp.nomModele); }
                }

                //Cache la feuille de donnée des metagraph
                Sheet sData = XcelWin.getSheetByName(workbookPart, "data");
                sData.State = new SheetStateValues();
                sData.State.Value = SheetStateValues.Hidden;

                //suppression des modèles de graphs
                List<string> listeModele = new List<string>();
                foreach (string key in d.Keys.Where(s => s.Contains("graph") || s.Contains("meta")))
                {
                    string nomModele = "";
                    try { nomModele = ((ChartData)d[key]).nomModele; }
                    catch (Exception) { nomModele = ((MetaChartData)d[key]).nomModele; }
                    if (!listeModele.Contains(nomModele))
                    {
                        listeModele.Add(nomModele);
                        try { XcelWin.supprChart(graphs, nomModele); }
                        catch (Exception) { Console.WriteLine("Impossible de supprimer le modèle car introuvable : " + nomModele); }
                    }
                }
            }

            {//Ajout de la date sur la page de garde
                Cell cell = XcelWin.createTextCell("H", 27, date, 0);
                Row r = new Row();
                r.RowIndex = 27;
                r.AppendChild(cell);
                worksheetPart = XcelWin.getWorksheetPartByName(myWorkbook, "REPORTING"); ;
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                sheetData.AppendChild(r);
            }
            //Suppression du worksheetPart qui a servis de modele
            workbookPart.DeletePart(XcelWin.getWorksheetPartByName(myWorkbook, "Feuil1"));
            XcelWin.getSheetByName(workbookPart, "Feuil1").Remove();

            //Sauvegarde du workbook et fermeture de l'objet fichier
            workbookPart.Workbook.Save();
            myWorkbook.Close();
            
        }
        */

        //recupere les lignes de données pour les clefs présentes dans le fichier de conf de graph
        public void obtInfosGraphs(DataTable dt, Dictionary<string, Object> d, string nom)
        {
            int i = 2;
            foreach (DataRow r in dt.Rows)
            {
                if ((string)r.ItemArray[indiceGraph] != "")//Si la colonne de graph n'es pas vide
                {
                    string[] a = r.ItemArray[indiceGraph].ToString().Split(';');
                    foreach (string b in a)//Pour chaque element 
                    {
                        if (d.Keys.Contains(b))//si l'element est présent dans le dictionnaire (donc s'il etait présent dans le fichier de conf)
                        {
                            ChartData tmp = (ChartData)d[b];
                            tmp.data.Add(i);
                            if (b.Contains("serie")&&tmp.nomModele=="")//s'il s'agit d'une série (et non d'un graph)
                            {
                                //nomModele conient le titre de colonne ou le nom de la sheet pour le graph
                                //titre contient le nom de feuille pour ecrire les formules dans la feuille data
                                //transposé vaut 1 si on transpose, -1 sinon
                                if (tmp.colValeurs == "last")
                                {
                                    //if (tmp.nomModele != "")
                                    //    tmp.transpose = 1;
                                    //else
                                    //    tmp.transpose = -1;
                                    if (tmp.titre != "")// si on veut etre en vrai mode tableau
                                        tmp.nomModele = dt.Columns[dt.Columns.Count - 1].ColumnName;
                                    else
                                        tmp.nomModele = nom;
                                    tmp.colValeurs=headerColumns[dt.Columns.Count - nbColonneConfig-1];
                                    tmp.colTitres = getHeaderCol(tmp.colTitres, dt);
                                    tmp.titre = nom;
                                }
                                else
                                {
                                    try
                                    {
                                        //if (tmp.nomModele != "")
                                        //    tmp.transpose = 1;
                                        //else
                                        //    tmp.transpose = -1;
                                        if (tmp.titre != "")// si on veut etre en vrai mode tableau
                                            tmp.nomModele = tmp.colValeurs;
                                        else
                                            tmp.nomModele = nom;
                                        tmp.colValeurs = getHeaderCol(tmp.colValeurs, dt);
                                        tmp.colTitres = getHeaderCol(tmp.colTitres, dt);
                                        tmp.titre = nom;
                                    }
                                    catch (Exception) { Console.WriteLine("Echec de mise à jour des données du graph, verifier le fichier de configuration " + b); }
                                }
                            }
                            d[b] = tmp;
                        }
                    }
                }
                i++;
            }
        }
    
        public string getHeaderCol(string nom, DataTable dt)
        {return headerColumns[dt.Columns.IndexOf(nom)-nbColonneConfig]; }

        public void ecrireMetaChartData(WorksheetPart worksheetPart, Dictionary<string, Object> d)
        {
            SheetData sd = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            int i = 1;
            foreach (string key in d.Keys.Where(s => s.Contains("meta")))
            {
                //Enregistre la ligne de début du tableau de ce metachart
                ((MetaChartData)d[key]).indice = i;

                //Ligne d'axisData
                Row rr = new Row();
                int k = 1;
                MetaChartData tmp = (MetaChartData)d[key];
                foreach (string serie in tmp.series)
                {
                    Cell ceee = XcelWin.createTextCell(headerColumns[k], i, ((ChartData)d[serie]).nomModele, 0);
                    rr.Append(ceee);
                    k++;
                }
                sd.Append(rr);
                i++;

                //le nombre de ligne de data
                //Les formules de data et titre de series
                foreach (int data in ((ChartData)d[tmp.series.First()]).data)
                {
                    Row r = new Row();

                    string titre = ((ChartData)d[tmp.series.First()]).colTitres;
                    string feuille = ((ChartData)d[tmp.series.First()]).titre;//nomModele

                    Cell cee = GenerateCell(headerColumns[0] + i, "'" + feuille + "'!$" + titre + "$" + data);
                    r.Append(cee);
                    //le nombre de serie
                    int j = 1;
                    foreach (string serie in tmp.series)
                    {
                        //string formule = "" + ((ChartData)d[serie]).nomModele + "!$" + ((ChartData)d[serie]).colValeurs + "$" + data;
                        string formule = "'" + ((ChartData)d[serie]).titre + "'!$" + ((ChartData)d[serie]).colValeurs + "$" + data;
                        Cell ce = GenerateCell(headerColumns[j] + i, formule);
                        r.Append(ce);
                        j++;
                    }
                    i++;
                    sd.Append(r);
                }
            }
        }

        public List<string> creaFormule(int nbreLi, int nbreCol,int debutTab)
        {
            List<string> res = new List<string>();
            //Creation formule des series
            string tmp = "" + "data!$B$"+debutTab;
            for (int i = 1; i < nbreCol; i++)
            {
                tmp += ",data!$" + headerColumns[i + 1] + "$"+debutTab;
            }

            res.Add(tmp);

            //creation formules legendes et data
            tmp = "";
            for (int i = 0; i < nbreLi; i++)
            {
                tmp = "data!$A$" + (debutTab+i + 1);
                res.Add(tmp);

                tmp = "data!$B$" + (debutTab+i + 1);

                for (int j = 1; j < nbreCol; j++)
                {
                    tmp += ",data!$" + headerColumns[j + 1] + "$" + (debutTab + i + 1);
                }
                res.Add(tmp);
            }
            return res;
        }

        public List<string> creaFormuleTranspose(int nbreLi, int nbreCol, int debutTab)
        {
            List<string> res = new List<string>();
            //Creation formule des series
            string tmp = "" + "data!$A$" + (debutTab+1);
            for (int i = 1; i < nbreLi; i++)
            {
                tmp += ",data!$A" + "$" + (debutTab+1+i);
            }

            res.Add(tmp);

            //creation formules legendes et data
            tmp = "";
            for (int i = 0; i < nbreCol; i++)
            {
                tmp = "data!$"+headerColumns[i+1]+"$"+debutTab;
                res.Add(tmp);

                tmp = "data!$"+headerColumns[i+1]+"$" + (debutTab + 1);

                for (int j = 1; j < nbreLi; j++)
                {
                    tmp += ",data!$" + headerColumns[i + 1] + "$" + (debutTab + 1 + j);
                }
                res.Add(tmp);
            }
            return res;
        }

        //inuilisé mais peut etre pour les metacharts avec des séries de types diférent
        public void aiguillageSeriesParType(ChartPart cp, List<string> formules, List<string> typesSeries)
        {
            if(typesSeries.Count==(formules.Count-1)/2)
            {
                List<int> auto = new List<int>();
                for (int i = 0; i < typesSeries.Count; i++)
                {
                    if (typesSeries[i] == "")
                        auto.Add(i);
                }

                List<string> formuleAuto = new List<string>() { formules[0] };
                foreach(int i in auto)
                {
                    formuleAuto.Add(formules[2*i+1]);
                    formuleAuto.Add(formules[2*i+2]);
                }
                XcelWin.majMetaChart(cp, formules);

            }
            else
                Console.WriteLine("Erreure Inconnue, fonction aiguillageSeriesParType");
            
        }




        //BLP(A2 & " " & B2;C1)
        public static void GenerateCalculationChainPart1Content(CalculationChainPart calculationChainPart, string cellRef, int sheetId)
        {
            CalculationChain calculationChain1 = new CalculationChain();
            CalculationCell calculationCell1 = new CalculationCell() { CellReference = cellRef, SheetId = sheetId };

            calculationChain1.Append(calculationCell1);

            calculationChainPart.CalculationChain = calculationChain1;
        }
        public static Cell GenerateCell(string cellRef, string formule)
        {
            Cell cell1 = new Cell() { CellReference = cellRef, DataType = CellValues.Error };
            CellFormula cellFormula1 = new CellFormula() { CalculateCell = true };
            cellFormula1.Text = formule;
            //CellValue cellValue1 = new CellValue();
            // cellValue1.Text = "#NAME?";


            cell1.Append(cellFormula1);
            //cell1.Append(cellValue1);
            return cell1;
        }
        public static void GenerateCalculationCell(CalculationChain calculationChain, string cellRef, int sheetId)
        {
            CalculationCell calculationCell1 = new CalculationCell() { CellReference = cellRef, SheetId = sheetId };
            calculationChain.Append(calculationCell1);
        }
        public static void bloom(string col,int li,string formule)
        {
            //Copie du template et ouverture du fichier
            System.IO.File.Copy("TemplateBloom.xlsx", "BloomGenerated.xlsx", true);
            SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open("BloomGenerated.xlsx", true);
            

             //myWorkbook.WorkbookPart.Workbook.NamespaceDeclarations

            //myWorkbook.WorkbookPart.Workbook.CalculationProperties.ForceFullCalculation = true;
            //myWorkbook.WorkbookPart.Workbook.CalculationProperties.FullCalculationOnLoad = true;

            //Access the main Workbook part, which contains all references.
            WorkbookPart workbookPart = myWorkbook.WorkbookPart;


            WorksheetPart worksheetPart = workbookPart.WorksheetParts.ElementAt<WorksheetPart>(2);
            SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();


            sheetData.RemoveNamespaceDeclaration("x");

            //CalculationChainPart calculationChainPart1 = workbookPart.AddNewPart<CalculationChainPart>("rId7");
            //GenerateCalculationChainPart1Content(calculationChainPart1,"C2",1);

            CalculationChain CC = workbookPart.GetPartsOfType<CalculationChainPart>().First().CalculationChain;

            CalculationCell calculationCell = new CalculationCell() { CellReference = col+li };
            CC.Append(calculationCell);
            
            //GenerateCalculationCell(CC, "C3", 1);

            

            Row r = (Row)sheetData.ChildElements.GetItem(2);
            Cell cell = GenerateCell(col + li, formule);
            //Cell cell = (Cell) r.ChildElements.GetItem(2);
            //cell.DataType = CellValues.Error;
            //cell.CellValue.Text="#NAME?";
            //cell.CellFormula.Text = formule;

            r.RemoveNamespaceDeclaration("x");
            cell.RemoveNamespaceDeclaration("x");

            r.AppendChild(cell);





            CalculationChainPart ccp = workbookPart.CalculationChainPart;
            workbookPart.DeletePart(ccp);





            myWorkbook.WorkbookPart.Workbook.Save();
            myWorkbook.Close();


        }
   
    
    }
}
