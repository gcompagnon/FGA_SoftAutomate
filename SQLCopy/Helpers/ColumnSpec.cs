using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace FGA.SQLCopy
{

    /// <summary>
    /// Utility class for storing Column infos of datatable
    /// </summary>
    public class ColumnSpec
    {
        public ColumnSpec(string column, string type, bool isPK,bool isNullable = false,int maxLength=255) :
            this(column, (SqlDbType)Enum.Parse(typeof(SqlDbType), type, true), isPK,isNullable,maxLength)
        {
        }

        public ColumnSpec(string column, SqlDbType type, bool isPK,bool isNullable = false,int maxLength=255)
        {
            this._Column = column;
            this._Type = type;
            this._isPK = isPK;
            this._MaximumLength = maxLength;
            this._isNullable = isNullable;
            this._isSQLChar = null;
            this._SQLType = null;

        }

        private void computeSQLTyping()
        {

            if (Type.Equals(SqlDbType.VarChar)
                ||
                Type.Equals(SqlDbType.NVarChar)
                ||
                Type.Equals(SqlDbType.Char)
                ||
                Type.Equals(SqlDbType.NChar)
                ||
                Type.Equals(SqlDbType.Text)
                ||
                Type.Equals(SqlDbType.NText)
                ||
                Type.Equals(SqlDbType.Binary))
            {
                _isSQLChar = true;
                _SQLType = Type.ToString() + "(" + MaximumLength + ") ";
            }
            else
            {
                _isSQLChar = false;
                _SQLType = Type.ToString();
            }

        }

        public string SQLType
        {
            get
            {
                if (_SQLType == null)
                    computeSQLTyping();
                return _SQLType;
            }
        }

        public bool isSQLCharType
        {
            get
            {
                if (_isSQLChar == null)
                    computeSQLTyping();
                return (bool)_isSQLChar;
            }
        }

        private string _Column;
        public string Column
        {
            get
            {
                return _Column;
            }
        }

        private SqlDbType _Type;
        public SqlDbType Type
        {
            get
            {
                return _Type;
            }
        }
        private bool _isPK;
        public bool isPK
        {
            get
            {
                return _isPK;
            }
        }

        private bool _isNullable;
        public bool isNullable
        {
            get
            {
                return _isNullable;
            }
        }

        private bool? _isSQLChar;
        private string _SQLType;
        
        private int _MaximumLength;
        public int MaximumLength
        {
            get
            {
                return _MaximumLength;
            }
        }


        private static Dictionary<Type,SqlDbType>  dbTypeTable;

        private SqlDbType ConvertToDbType(Type t)
        {
            if (dbTypeTable == null)
            {
                dbTypeTable = new Dictionary<Type, SqlDbType>();
                dbTypeTable.Add(typeof(System.Boolean), SqlDbType.Bit);
                dbTypeTable.Add(typeof(System.Int16), SqlDbType.SmallInt);
                dbTypeTable.Add(typeof(System.Int32), SqlDbType.Int);
                dbTypeTable.Add(typeof(System.Int64), SqlDbType.BigInt);
                dbTypeTable.Add(typeof(System.Double), SqlDbType.Float);
                dbTypeTable.Add(typeof(System.Decimal), SqlDbType.Decimal);
                dbTypeTable.Add(typeof(System.String), SqlDbType.VarChar);
                dbTypeTable.Add(typeof(System.DateTime), SqlDbType.DateTime);
                dbTypeTable.Add(typeof(System.Byte[]), SqlDbType.VarBinary);
                dbTypeTable.Add(typeof(System.Guid), SqlDbType.UniqueIdentifier);
            }
            SqlDbType dbtype;
            try
            {
                dbtype = (SqlDbType)dbTypeTable[t];
            }
            catch
            {
                dbtype = SqlDbType.Variant;
            }
            return dbtype;
        }

    }
}
