using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Saviso.EntityFramework
{
    public class DbDataReaderEx : DbDataReader
    {
        private readonly IAopFilter appender;
        private readonly DbCommand command;
        private readonly Guid connectionId;
        private bool disposed;
        private readonly DbDataReader inner;
        private int rowCount;
        private readonly Guid statementGuid;

        public DbDataReaderEx(DbDataReader inner, DbCommand command, Guid connectionId, Guid statementGuid, IAopFilter appender)
        {
            this.inner = inner;
            this.command = command;
            this.connectionId = connectionId;
            this.statementGuid = statementGuid;
            this.appender = appender;
        }

        public override void Close()
        {
            this.appender.StatementRowCount(this.connectionId, this.statementGuid, this.rowCount);
            SqlDataReader inner = this.inner as SqlDataReader;
            if (((!this.disposed && (inner != null)) && (this.command.Transaction == null)) && inner.Read())
            {
                this.command.Cancel();
            }
            this.disposed = true;
            this.inner.Close();
        }

        protected override void Dispose(bool disposing)
        {
            this.disposed = true;
            if (disposing)
            {
                this.inner.Dispose();
            }
            base.Dispose(disposing);
        }

        public override bool GetBoolean(int ordinal)
        {
            return this.inner.GetBoolean(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return this.inner.GetByte(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return this.inner.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override char GetChar(int ordinal)
        {
            return this.inner.GetChar(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return this.inner.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return this.inner.GetDataTypeName(ordinal);
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return this.inner.GetDateTime(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return this.inner.GetDecimal(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return this.inner.GetDouble(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public override Type GetFieldType(int ordinal)
        {
            return this.inner.GetFieldType(ordinal);
        }

        public override float GetFloat(int ordinal)
        {
            return this.inner.GetFloat(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return this.inner.GetGuid(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return this.inner.GetInt16(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return this.inner.GetInt32(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return this.inner.GetInt64(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return this.inner.GetName(ordinal);
        }

        public override int GetOrdinal(string name)
        {
            return this.inner.GetOrdinal(name);
        }

        public override Type GetProviderSpecificFieldType(int ordinal)
        {
            return this.inner.GetProviderSpecificFieldType(ordinal);
        }

        public override object GetProviderSpecificValue(int ordinal)
        {
            return this.inner.GetProviderSpecificValue(ordinal);
        }

        public override int GetProviderSpecificValues(object[] values)
        {
            return this.inner.GetProviderSpecificValues(values);
        }

        public override DataTable GetSchemaTable()
        {
            return this.inner.GetSchemaTable();
        }

        public override string GetString(int ordinal)
        {
            return this.inner.GetString(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return this.inner.GetValue(ordinal);
        }

        public override int GetValues(object[] values)
        {
            return this.inner.GetValues(values);
        }

        public override bool IsDBNull(int ordinal)
        {
            return this.inner.IsDBNull(ordinal);
        }

        public override bool NextResult()
        {
            return this.inner.NextResult();
        }

        public override bool Read()
        {
            bool flag = this.inner.Read();
            if (flag)
            {
                this.rowCount++;
            }
            return flag;
        }

        public override int Depth
        {
            get
            {
                return this.inner.Depth;
            }
        }

        public override int FieldCount
        {
            get
            {
                return this.inner.FieldCount;
            }
        }

        public override bool HasRows
        {
            get
            {
                return this.inner.HasRows;
            }
        }

        public override bool IsClosed
        {
            get
            {
                return this.inner.IsClosed;
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return this.inner[ordinal];
            }
        }

        public override object this[string name]
        {
            get
            {
                return this.inner[name];
            }
        }

        public override int RecordsAffected
        {
            get
            {
                return this.inner.RecordsAffected;
            }
        }

        public override int VisibleFieldCount
        {
            get
            {
                return this.inner.VisibleFieldCount;
            }
        }
    }
}

