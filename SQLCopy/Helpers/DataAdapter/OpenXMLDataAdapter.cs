using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Text.RegularExpressions;
using SQLCopy.Helpers.DataReader;
using SQLCopy.Helpers.DataTable;
using System.Data.Common;

namespace SQLCopy.Helpers.DataAdapter
{

    /// <summary>
    /// DataAdapter for a XLSX File (Excel from 2007 / OpenXML Format)
    /// </summary>
    public class OpenXMLDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
    {
        private string XMLFilePath;
        private string WorksheetName;
        private double StartCell_Column;
        private double StartCell_Row;
        private double EndCell_Column;
        private double EndCell_Row;
        private bool WithHeaderOnFirstRow;

        private static Type STRING_TYPE = null;

        /// <summary>
        /// read the whole first worksheet of the given excel file
        /// </summary>
        /// <param name="xmlFilePath"></param>
        public OpenXMLDataAdapter(string xmlFilePath)
            : this(xmlFilePath, null, null, null)
        {
        }

        //TODO changer le fonctionnement pour utiliser un DataReader
        public OpenXMLDataAdapter(OpenXMLDataReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="worksheetName"></param>
        /// <param name="startCell">the area could be defined by startCell (upperLeft cell) </param>
        /// <param name="endCell">and the endCell (bottom right cell)</param>
        public OpenXMLDataAdapter(string xmlFilePath, string worksheetName, string startCell, string endCell, bool withHeaderOnFirstRow = true)
        {
            STRING_TYPE = System.Type.GetType("System.String");
            this.XMLFilePath = xmlFilePath;
            this.WorksheetName = worksheetName;
            if (startCell == null)
            {
                this.StartCell_Column = 0;
                this.StartCell_Row = 0;
            }
            else
            {
                double[] c = OpenXMLDataAdapter.GetColumnRowXY(startCell);
                this.StartCell_Column = c[0];
                this.StartCell_Row = c[1];
            }
            if (endCell == null)
            {
                this.EndCell_Column = double.MaxValue;
                this.EndCell_Row = double.MaxValue;
            }
            else
            {
                double[] c = OpenXMLDataAdapter.GetColumnRowXY(endCell);
                this.EndCell_Column = c[0];
                this.EndCell_Row = c[1];
            }
            this.WithHeaderOnFirstRow = withHeaderOnFirstRow;
        }

        private Dictionary<String, KeyValuePair<string, Type>> columnsFilling = new Dictionary<string, KeyValuePair<string, Type>>();
        /// <summary>
        /// Gives the extra settings for additionnal columns , where the Name is the key and the CellReference is the value.
        /// For all value of the column, the value will be the value inside the cell reference. (for example, give a new Column with  Date  and "A1" )
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="cellReference"></param>
        public void FillColumnWithCell(string columnName, string cellReference, Type type = null)
        {
            KeyValuePair<string, Type> pair = new KeyValuePair<string, Type>(columnName, type ?? STRING_TYPE);
            columnsFilling.Add(cellReference, pair);
            columnsType.Add(columnName, type ?? STRING_TYPE);
        }
        private Dictionary<String, Object> columnsFillingConstant = new Dictionary<string, Object>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        public void FillColumnWithValue(string columnName, Object value)
        {
            columnsFillingConstant.Add(columnName, value);
            columnsType.Add(columnName, value.GetType());
        }

        private Dictionary<string, Type> columnsType = new Dictionary<string, Type>();
        public void AddColumnMapping(string targetDataTableName, string sourceColumnName, string targetColumnName, Type columnType = null)
        {
            if (!this.HasTableMappings() && !this.TableMappings.Contains("Table") )
            {
                this.TableMappings.Add("Table", targetDataTableName);
            }
            ITableMapping mapping = this.TableMappings["Table"];
            mapping.ColumnMappings.Add(sourceColumnName, targetColumnName);
            columnsType.Add(targetColumnName, columnType ?? STRING_TYPE);
        }


        /// <summary>
        /// read the XMLX File and fill the DataTable in the DataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataTableName"></param>
        /// <returns></returns>
        public new int Fill(DataSet dataSet, string dtTableName = "Table")
        {

            string dataTableName = dtTableName;
            System.Data.DataTable dt = this.PrepareDataTable(dataSet, ref dataTableName);



            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(this.XMLFilePath, false))
            {
                // Output
                string[] headers = new string[(int)(this.EndCell_Column - this.StartCell_Column + 1)];
                int nbRows = 0;

                //Input: open the worksheet
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

                WorksheetPart worksheetPart;
                if (this.WorksheetName != null)
                    worksheetPart = GetWorksheetPartByName(spreadsheetDocument, WorksheetName);
                else
                    worksheetPart = GetFirstWorksheetPart(spreadsheetDocument);

                // Starting 
                OpenXmlReader reader = OpenXmlReader.Create(worksheetPart);
                string rowNum;
                while (reader.Read())
                {
                    if (reader.ElementType == typeof(SheetData))
                    {
                        //   SheetData sd = (SheetData)reader.LoadCurrentElement();

                        reader.ReadFirstChild();

                        do// while : Skip to the next row
                        {
                            if (reader.ElementType == typeof(Row))
                            {
                                rowNum = null;
                                DataRow row = null;
                                if (reader.HasAttributes)
                                {
                                    rowNum = reader.Attributes.First(a => a.LocalName == "r").Value;
                                }

                                if (rowNum != null)
                                {
                                    reader.ReadFirstChild();
                                    do// while: next Cell
                                    {
                                        if (reader.ElementType == typeof(Cell))
                                        {
                                            Cell c = (Cell)reader.LoadCurrentElement();

                                            // Calculate the transformation between Cell reference inside the excel file and the output datatable
                                            double[] coord; // coordinate in (int,int)
                                            double[] outputColumnRow = this.GetRecordedCellXY(c.CellReference, out coord);
                                            //1) if the cell is in the area defined by startCell/endCell
                                            if (outputColumnRow != null)
                                            {
                                                string value = this.GetCellValue(c, workbookPart);
                                                if (this.WithHeaderOnFirstRow && outputColumnRow[1] == 0)
                                                {
                                                    KeyValuePair<string,Type> headerName = this.GetMappingColumnName(dtTableName, value);
                                                    headers[(int)outputColumnRow[0]] = headerName.Key;
                                                    dt.Columns.Add(headerName.Key, headerName.Value/*Type of column*/ );
                                                }
                                                else
                                                {
                                                    if (row == null)
                                                    {
                                                        row = dt.NewRow();
                                                    }
                                                    string columnName = headers[(int)outputColumnRow[0]];
                                                    if (value!=null && value.Length > 0)
                                                    {
                                                        if (dt.Columns[columnName].DataType == System.Type.GetType("System.DateTime"))
                                                        {
                                                            double d;
                                                            if (double.TryParse(value, out d))
                                                            {
                                                               row.SetField<DateTime>(columnName, DateTime.FromOADate(d) );
                                                            }
                                                            else
                                                            {
                                                                row.SetField<Object>(columnName, value);
                                                            }
                                                        }
                                                        else
                                                        {
                                                         
                                                            row.SetField<Object>(columnName, value);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        row.SetField<Object>(columnName, null);
                                                    }
                                                }
                                            }

                                            KeyValuePair<string, Type> cn;
                                            // 2) if there are Column Filling with value of the cell
                                            if (this.columnsFilling.TryGetValue(c.CellReference, out cn))
                                            {
                                                string value = this.GetCellValue(c, workbookPart);
                                                Object v;
                                                if (cn.Value.Equals(typeof(System.DateTime)))
                                                {
                                                    v = Convert.ToDateTime(value);
                                                }
                                                else
                                                {
                                                    v = Convert.ChangeType(value, cn.Value);
                                                }
                                                columnsFillingConstant.Add(cn.Key, v);
                                            }
                                        }
                                    } while (reader.ReadNextSibling());// while: next Cell
                                }
                                if (row != null)
                                {
                                    dt.Rows.Add(row);
                                    nbRows++;
                                }
                            }

                            // 
                        } while (reader.ReadNextSibling());// while : Skip to the next row


                    }

                    if (reader.ElementType != typeof(Worksheet))
                        reader.Skip();
                }

                reader.Close();

                dt.AddConstantInColumn(this.columnsFillingConstant);

                return nbRows;
            }

        }


        /// <summary>
        /// Returns the columnName or the Mapped Column Name
        /// </summary>
        /// <param name="dataTableName"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private KeyValuePair<string,Type>  GetMappingColumnName(string dataTableName, string columnName)
        {
            string cn = columnName;
            if (this.HasTableMappings())
            {
                if (this.TableMappings.Contains(dataTableName))
                {
                    ITableMapping mapping = this.TableMappings[dataTableName];
                    if (mapping.ColumnMappings.Contains(columnName))
                    {
                        DataColumnMapping dcm = (DataColumnMapping)mapping.ColumnMappings[columnName];
                        cn = dcm.DataSetColumn;
                    }
                }
            }
            Type t;
            if (!this.columnsType.TryGetValue(cn, out t))
                t = STRING_TYPE;

            return new KeyValuePair<string,Type>( cn, t);
        }


        private System.Data.DataTable PrepareDataTable(DataSet dataSet, ref string dataTableName)
        {
            if (dataTableName == null)
                dataTableName = "Table";

            // MAPPING 
            if (this.HasTableMappings() && TableMappings.Contains(dataTableName))
            {
                ITableMapping mapping = this.TableMappings[dataTableName];
                if (mapping != null)
                {
                    dataTableName = mapping.DataSetTable;
                }
            }
            else             // Or No Mapping
            {
                dataTableName = dataSet.DataSetName;  
            }

            System.Data.DataTable dt = new System.Data.DataTable(dataTableName);
            dataSet.Tables.Add(dt);

            // For The filling columns, add their columns name
            foreach (KeyValuePair<string, Type> columnName in this.columnsFilling.Values)
            {
                dt.Columns.Add(columnName.Key, columnName.Value/*Type of Column*/);
            }
            foreach (string columnName in this.columnsFillingConstant.Keys)
            {
                dt.Columns.Add(columnName);
            }

            return dt;
        }

        private string GetCellValue(Cell c, WorkbookPart workbookPart)
        {
            string cellValue;

            if (c.DataType != null && c.DataType == CellValues.SharedString)
            {
                SharedStringItem ssi = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(c.CellValue.InnerText));

                cellValue = ssi.Text.Text;
            }
            else if (c.CellValue != null)
            {
                cellValue = c.CellValue.InnerText;
            }
            else
            {
                cellValue = null;
            }
            return cellValue;
        }

        /// <summary>
        /// return the transformation of the recorded cell. the StartCell will be the (0,0)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private double[] GetRecordedCellXY(string cellReference, out double[] coord)
        {
            coord = OpenXMLDataAdapter.GetColumnRowXY(cellReference);

            if (this.StartCell_Column <= coord[0])
            {
                if (this.StartCell_Row <= coord[1])
                {
                    if (this.EndCell_Column >= coord[0])
                    {
                        if (this.EndCell_Row >= coord[1])
                        {
                            return new double[] { coord[0] - this.StartCell_Column, coord[1] - this.StartCell_Row };
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Return the 2 integer representing the Cell Reference in XLS
        /// </summary>
        /// <param name="Address">for exemple "A1" or "IV256"</param>
        /// <returns></returns>
        public static double[] GetColumnRowXY(string Address)
        {
            Regex rgx = new Regex("(^[A-Za-z]+)|([0-9]+$)", RegexOptions.IgnoreCase);
            MatchCollection col = rgx.Matches(Address);
            double columnNumber = -1;
            double rowNumber = -1;
            foreach (Match c in col)
            {
                if (columnNumber == -1)
                {
                    columnNumber = ConvertAlphaToInt(c.Value);
                }
                else if (rowNumber == -1)
                {
                    Double.TryParse(c.Value, out rowNumber);
                }


            }

            return new double[] { columnNumber, rowNumber }; ;
        }

        /// <summary>
        /// Give the value of a characters to a Int . for exemple A=1 , Z=26 , AA = 27, IV = 256
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static double ConvertAlphaToInt(string s)
        {
            int i = s.Length - 1;
            double result = 0;
            foreach (char c in s)
            {
                if (Char.IsUpper(c))
                {
                    result += (Convert.ToByte(c) - Convert.ToByte('A') + 1) * Math.Pow(26, i);
                }
                else
                {
                    result += (Convert.ToByte(c) - Convert.ToByte('a') + 1) * Math.Pow(26, i);
                }
                i--;
            }
            return result;

        }


        #region Utilities
        private static WorksheetPart
   GetFirstWorksheetPart(SpreadsheetDocument document)
        {
            Sheets sheets = document.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            Sheet s = ((Sheet)sheets.First());
            if (s == null)
            {
                // The specified worksheet does not exist.
                throw new ApplicationException(String.Format("No WorkSheet in the file : {0}", document.ToString()));

            }
            return (WorksheetPart)document.WorkbookPart.GetPartById(s.Id);
        }

        private static WorksheetPart
           GetWorksheetPartByName(SpreadsheetDocument document,
           string sheetName)
        {
            IEnumerable<Sheet> sheets =
               document.WorkbookPart.Workbook.GetFirstChild<Sheets>().
               Elements<Sheet>().Where(s => s.Name == sheetName);

            if (sheets.Count() == 0)
            {
                // The specified worksheet does not exist.
                throw new ApplicationException(String.Format("WorkSheetName {0} does not exist in the file : {1}", sheetName, document.ToString()));

            }

            string relationshipId = sheets.First().Id.Value;
            WorksheetPart worksheetPart = (WorksheetPart)
                 document.WorkbookPart.GetPartById(relationshipId);
            return worksheetPart;

        }

        // Given a worksheet, a column name, and a row index, 
        // gets the cell at the specified column and 
        private static Cell GetCell(Worksheet worksheet,
                  string columnName, uint rowIndex)
        {
            Row row = GetRow(worksheet, rowIndex);

            if (row == null)
                return null;

            return row.Elements<Cell>().Where(c => string.Compare
                   (c.CellReference.Value, columnName +
                   rowIndex, true) == 0).First();
        }


        // Given a worksheet and a row index, return the row.
        private static Row GetRow(Worksheet worksheet, uint rowIndex)
        {
            return worksheet.GetFirstChild<SheetData>().
              Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
        }
        #endregion


        System.Data.DataTable[] IDataAdapter.FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            throw new NotImplementedException();
        }

        IDataParameter[] IDataAdapter.GetFillParameters()
        {
            throw new NotImplementedException();
        }

        MissingMappingAction IDataAdapter.MissingMappingAction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        MissingSchemaAction IDataAdapter.MissingSchemaAction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        //DataTableMappingCollection mappingCollection;
        //public ITableMappingCollection TableMappings
        //{
        //    get
        //    {
        //        if (mappingCollection == null)
        //            mappingCollection = new DataTableMappingCollection();

        //            return mappingCollection;  }
        //}

        int IDataAdapter.Update(DataSet dataSet)
        {
            throw new NotImplementedException();
        }

    }
}
