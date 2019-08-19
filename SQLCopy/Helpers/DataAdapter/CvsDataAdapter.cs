using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SQLCopy.Dbms;
using System.Globalization;

namespace FGA.SQLCopy
{

    /// <summary>
    /// DataAdapter for a CSV File
    /// </summary>
    public class CsvDataAdapter : IDataAdapter
    {
        private LumenWorks.Framework.IO.Csv.CsvReader reader;
        public CsvDataAdapter(LumenWorks.Framework.IO.Csv.CsvReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// read the Csv File and fill the DataTable in the DataSet
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="dataTableName"></param>
        /// <returns></returns>
        public int Fill(DataSet dataSet, string dataTableName = "Table")
        {
            DataTable dt = new DataTable(dataTableName);
            dataSet.Tables.Add(dt);

            int fieldCount = reader.FieldCount;

            string[] headers = reader.GetFieldHeaders();

            foreach (string s in headers)
            {
                dt.Columns.Add(s);
            }

            int nbRows = 0;
            while (reader.ReadNextRecord())
            {
                DataRow row = dt.NewRow();

                for (int i = 0; i < fieldCount; i++)
                {
                    row.SetField(headers[i], reader[i]);
                }
                dt.Rows.Add(row);
                nbRows++;

            }
            return nbRows;
        }

        /// <summary>
        /// Read the Csv File and fill the Table on the given Connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dataTableSchema"></param>
        /// <param name="dataTableName">if not exists, create the table with collate option</param>
        /// <param name="tableCollation">by default, Latin1_General_CP1</param>
        /// <returns></returns>
        public int Fill(DBConnectionDelegate connection, DatabaseTable dataTableName, string date_format = "dd/MM/yyyy", string tableCollation = "COLLATE SQL_Latin1_General_CP1_CI_AS")
        {
            System.Diagnostics.Contracts.Contract.Assert(connection != null, "Connection must be setted");

            string createDataTableRequest = "create TABLE [{0}].[{1}] ({2})";
            string createDataTableColumns = null;
            string insertRequest = "insert into [{0}].[{1}] ({2}) VALUES ({3})";
            string insertRequestParameters = null;

            string[] fieldHeaders = reader.GetFieldHeaders();

            // intersept the file hearders with the columns name of the destination table
            Dictionary<string, ColumnSpec> databaseColumns = connection.GetColumnsSpec(dataTableName);
            IEnumerable<string> databaseColumnsName = databaseColumns.Keys.AsEnumerable<string>();
            IEnumerable<string> headers;
            if (databaseColumnsName.Count<string>() > 0)
            {
                headers = Enumerable.Intersect<string>(fieldHeaders.AsEnumerable<string>(), databaseColumnsName);
            }
            else
            {
                headers = fieldHeaders.AsEnumerable<string>();
            }
            int fieldCount = headers.Count<string>();
            int[] headersMaxWidth = new int[fieldCount];

            List<SqlParameter[]> wholeParams = new List<SqlParameter[]>();

            // TODO pour createDataTableColumns, prevoir une adaptation, car si il y a un objet mapping, create des champs typés, et pas tout le temps
            int i = 0;
            foreach (string h in headers)
            {
                if (createDataTableColumns == null)
                    createDataTableColumns = h + " NVARCHAR({" + i + "}) " + tableCollation;
                else
                    createDataTableColumns += ", " + h + " NVARCHAR({" + i + "}) " + tableCollation;

                if (insertRequestParameters == null)
                    insertRequestParameters = "{0}" + h;
                else
                    insertRequestParameters += ", {0}" + h;
                i++;
            }

            // Type Management: Each parameter is typed using the destination database columns type.
            //
            int nbRows = 0;
            while (reader.ReadNextRecord())
            {
                bool emptyRecord = true;
                SqlParameter[] entries = new SqlParameter[fieldCount];
                i = 0;
                foreach (string h in headers)
                {
                    SqlParameter p;
                    // Get the DbType                    
                    ColumnSpec spec = databaseColumns[h];
                    string fieldContent = reader[h];
                    int width = fieldContent.Length;

                    if (width > 0)
                    {
                        emptyRecord = false;

                        if (headersMaxWidth[i] < width)
                            headersMaxWidth[i] = width;

                        if (spec == null)
                        {// The parameter is in nvarchar
                            p = new SqlParameter(h, fieldContent);
                        }
                        else if (spec.isSQLCharType)
                        {
                            p = new SqlParameter(h, spec.Type, spec.MaximumLength);
                            p.Value = fieldContent;
                        }
                        else if (spec.Type.Equals(SqlDbType.Float) || spec.Type.Equals(SqlDbType.Decimal))
                        {
                                p = new SqlParameter(h, spec.Type, spec.MaximumLength);
                                double res;
                                // If type is float, but impossible to parse => Null Value
                                if (Double.TryParse(fieldContent, NumberStyles.Float | NumberStyles.Number, CultureInfo.InvariantCulture, out res))
                                {
                                    p.Value = res;
                                }
                                else
                                {
                                    p.Value = DBNull.Value;
                                }

                        }
                        else if (spec.Type.Equals(SqlDbType.DateTime) || spec.Type.Equals(SqlDbType.DateTime2))
                        {
                            p = new SqlParameter(h, spec.Type);
                            DateTime res;
                            // If type is datetime, but impossible to parse => Null Value
                            if (DateTime.TryParseExact(fieldContent, date_format,CultureInfo.InvariantCulture,DateTimeStyles.None, out res))
                            {
                                p.Value = res;
                            }
                            else
                            {
                                p.Value = DBNull.Value;
                            }

                        }
                        else
                        {
                            p = new SqlParameter(h, spec.Type, spec.MaximumLength);
                            p.Value = fieldContent;
                        }
                    }
                    else
                    {
                        p = new SqlParameter(h, spec.Type, spec.MaximumLength);
                        p.Value = DBNull.Value;
                    }


                    entries[i] = p;
                    i++;
                }
                if (!emptyRecord)
                {
                    wholeParams.Add(entries);
                    nbRows++;
                }
            }
            // Execution SQL
            // creation de la table destination
            if (!connection.isTableExist(dataTableName))
            {
                string[] values = headersMaxWidth.Select(x => x > 0 ? x.ToString() : "1").ToArray();
                createDataTableColumns = String.Format(createDataTableColumns, values);
                createDataTableRequest = String.Format(createDataTableRequest, dataTableName.schema, dataTableName.table, createDataTableColumns);
                connection.Execute(createDataTableRequest);
            }
            // effectuer les Insertions
            string insertParameters1 = String.Format(insertRequestParameters, ' ');
            string insertParameters2 = String.Format(insertRequestParameters,'@');

            insertRequest = String.Format(insertRequest, dataTableName.schema, dataTableName.table, insertParameters1,insertParameters2);

            IDbCommand command = new SqlCommand(insertRequest);
            for (i = 0; i < nbRows; i++)
            {
                IDataParameter[] p = wholeParams[i];
                connection.Execute(ref command, p);
            }


            return nbRows;
        }


        DataTable[] IDataAdapter.FillSchema(DataSet dataSet, SchemaType schemaType)
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

        ITableMappingCollection IDataAdapter.TableMappings
        {
            get { throw new NotImplementedException(); }
        }

        int IDataAdapter.Update(DataSet dataSet)
        {
            throw new NotImplementedException();
        }

        public int Fill(DataSet dataSet)
        {
            return this.Fill(dataSet, "Table");
        }

    }


}
