using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLCopy.Dbms
{
    /// <summary>
    /// Structure for identifying a Table : the table name 
    /// </summary>
    public struct DatabaseTable
    {
        public string database;
        public string schema;
        public string table;

        public DatabaseTable(string database, string schema, string table)
        {
            this.database = database;
            this.schema = schema;
            this.table = table;
        }
        public DatabaseTable(string schema, string table)
            : this(null, schema, table)
        {
        }

        public DatabaseTable(string table)
            : this(null, null, table)
        {
        }

        public override string ToString()
        {
            if (database != null)
            {
                return String.Format("[{0}].[{1}].[{2}]", database, schema, table);
            }
            else if (schema != null)
            {
                return String.Format("[{0}].[{1}]", schema, table);
            }
            else if (table != null)
            {
                return String.Format("[{0}]", table);
            }
            return "NULL";
        }
    }
    /// <summary>
    /// Extensions on DataTable Collection
    /// </summary>
    public static class DatabaseTableExtensions
    {
        public static bool Contains(this List<DatabaseTable> listTableName, string tableName, string schemaName = null, StringComparison comp = StringComparison.OrdinalIgnoreCase)
        {
            IEnumerable<DatabaseTable> t_candidates = listTableName.Where<DatabaseTable>(i => i.table != null && i.table.Equals(tableName, comp));
            if (t_candidates == null || !t_candidates.Any())
                return false;
            else
            {
                DatabaseTable t_candidate = t_candidates.First<DatabaseTable>();
                if (t_candidate.schema != null && schemaName != null && !t_candidate.schema.Equals(schemaName, comp))
                    return false;
            }
            return true;
        }

        public static List<DatabaseTable> ToDatabaseTableList(this List<string> listTableName)
        {
            List<DatabaseTable> result = new List<DatabaseTable>(listTableName.Count);
            foreach (string tableName in listTableName)
            {
                result.Add(new DatabaseTable(tableName));
            }
            return result;
        }

    }


}
