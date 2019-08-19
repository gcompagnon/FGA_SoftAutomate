using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using FGA.SQLCopy;

namespace SQLCopy.Dbms
{

    public static class AutoMappingHelper
    {
        public static IEnumerable<string> GetMapping(DBConnectionDelegate dbConnection1, DatabaseTable tableName1, DBConnectionDelegate dbConnection2, DatabaseTable tableName2)
        {
            return Enumerable.Intersect(
                dbConnection1.GetColumnsName(tableName1),
                dbConnection2.GetColumnsName(tableName2),
                new ColumnEqualityComparer());
        }

        public static IEnumerable<string> GetMapping(DataTable dt, DBConnectionDelegate dbConnection, DatabaseTable tableName)
        {
            return Enumerable.Intersect(
                dt.GetColumnsName(),
                dbConnection.GetColumnsName(tableName),
                new ColumnEqualityComparer());
        }

        internal class ColumnEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(string s)
            {
                return s.GetHashCode();
            }
        }

        public static IEnumerable<string> GetColumnsName(this DataTable dt)
        {
            foreach (DataColumn c in dt.Columns)
            {
                yield return c.ColumnName;
            }
        }

    }
}
