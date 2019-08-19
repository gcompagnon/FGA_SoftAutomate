using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SQLCopy.Helpers.DataTable
{
    /// <summary>
    /// Extension method for DataTable 
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// For all Rows, and for all Key/Value of the given Dictionary,  add a column for each key in the DataTable, filled with the same value per Key
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columnsValue"></param>
        public static void AddConstantInColumn(this System.Data.DataTable dt, IDictionary<string, object> columnsValue)
        {
            // Manage the columns to add to the value for all rows
            foreach (DataRow dr in dt.Rows)
            {
                foreach (KeyValuePair<string, Object> pair in columnsValue)
                {
                    dr.SetField<object>(pair.Key, pair.Value);
                }
            }
        }
        /// <summary>
        /// For all Rows, add a new column in the DataTable, filled with the same value
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="d"></param>
        public static void AddConstantInColumn(this System.Data.DataTable dt, string columnsName, object value)
        {
            // Manage the columns to add to the value for all rows
            foreach (DataRow dr in dt.Rows)
            {
                    dr.SetField<object>(columnsName , value);
            }
        }


    }
}
