using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Saviso.EntityFramework
{
    public class DbCommandEx : DbCommand
    {
        private readonly IAopFilter appender;
        private DbConnectionEx _connectionEx;
        private readonly DbCommand inner;

        public DbCommandEx(DbCommand inner, IAopFilter appender) : this (inner, null, appender)
        {
        }

        public DbCommandEx(DbCommand inner, DbConnectionEx _connectionEx, IAopFilter appender)
        {
            this.inner = inner;
            this._connectionEx = _connectionEx;
            this.appender = appender;
        }

        public override void Cancel()
        {
            this.inner.Cancel();
        }

        protected override DbParameter CreateDbParameter()
        {
            return this.inner.CreateParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            DbDataReader reader;
            Guid statementId = Guid.NewGuid();
            this.LogCommand(statementId);
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                reader = this.inner.ExecuteReader(behavior);
            }
            catch (Exception exception)
            {
                this.appender.StatementError(this._connectionEx.ConnectionId, exception);
                throw;
            }
            this.appender.CommandDurationAndRowCount(this._connectionEx.ConnectionId, stopwatch.ElapsedMilliseconds, new int?(reader.RecordsAffected));
            return new DbDataReaderEx(reader, this.inner, this._connectionEx.ConnectionId, statementId, this.appender);
        }

        public override int ExecuteNonQuery()
        {
            int num;
            this.LogCommand(Guid.NewGuid());
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                num = this.inner.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                this.appender.StatementError(this._connectionEx.ConnectionId, exception);
                throw;
            }
            this.appender.CommandDurationAndRowCount(this._connectionEx.ConnectionId, stopwatch.ElapsedMilliseconds, new int?(num));
            return num;
        }

        public override object ExecuteScalar()
        {
            object obj2;
            this.LogCommand(Guid.NewGuid());
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                obj2 = this.inner.ExecuteScalar();
            }
            catch (Exception exception)
            {
                this.appender.StatementError(this._connectionEx.ConnectionId, exception);
                throw;
            }
            this.appender.CommandDurationAndRowCount(this._connectionEx.ConnectionId, stopwatch.ElapsedMilliseconds, null);
            return obj2;
        }

        private static object GetParameterValue(IDataParameter parameter)
        {
            if (parameter.Value == DBNull.Value)
            {
                return "NULL";
            }
            if (parameter.Value is byte[])
            {
                StringBuilder builder = new StringBuilder("0x");
                foreach (byte num2 in (byte[]) parameter.Value)
                {
                    builder.Append(num2.ToString("X2"));
                }
                return builder.ToString();
            }
            return parameter.Value;
        }

        private void LogCommand(Guid statementId)
        {
            StringBuilder builder = new StringBuilder(this.inner.CommandText).AppendLine().AppendLine("-- Parameters:");
            foreach (IDbDataParameter parameter in base.Parameters)
            {
                string parameterName = parameter.ParameterName;
                if (!parameterName.StartsWith("@"))
                {
                    parameterName = "@" + parameterName;
                }
                builder.Append("-- ").Append(parameterName).Append(" = [-[").Append(GetParameterValue(parameter)).Append("]-] [-[").Append(parameter.DbType).Append(" (").Append(parameter.Size).AppendLine(")]-]");
            }
            this.appender.StatementExecuted(this._connectionEx.ConnectionId, statementId, builder.ToString());
        }

        public override void Prepare()
        {
            this.inner.Prepare();
        }

        public bool BindByName
        {
            get
            {
                PropertyInfo property = this.inner.GetType().GetProperty("BindByName");
                if (property == null)
                {
                    return false;
                }
                return (bool) property.GetValue(this.inner, null);
            }
            set
            {
                PropertyInfo property = this.inner.GetType().GetProperty("BindByName");
                if (property != null)
                {
                    property.SetValue(this.inner, value, null);
                }
            }
        }

        public override string CommandText
        {
            get
            {
                return this.inner.CommandText;
            }
            set
            {
                this.inner.CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return this.inner.CommandTimeout;
            }
            set
            {
                this.inner.CommandTimeout = value;
            }
        }

        public override System.Data.CommandType CommandType
        {
            get
            {
                return this.inner.CommandType;
            }
            set
            {
                this.inner.CommandType = value;
            }
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                return this._connectionEx;
            }
            set
            {
                this._connectionEx = (DbConnectionEx) value;
                if (this.inner != null)
                {
                    this.inner.Connection = (this._connectionEx != null) ? this._connectionEx.Inner : null;
                }
            }
        }

        protected override System.Data.Common.DbParameterCollection DbParameterCollection
        {
            get
            {
                return this.inner.Parameters;
            }
        }

        protected override System.Data.Common.DbTransaction DbTransaction
        {
            get
            {
                if (this.inner.Transaction == null)
                {
                    return null;
                }
                return new DbTransactionEx(this.inner.Transaction, this.appender, this._connectionEx);
            }
            set
            {
                DbTransactionEx transactionEx = (DbTransactionEx) value;
                this.inner.Transaction = (transactionEx != null) ? transactionEx.Inner : null;
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                return this.inner.DesignTimeVisible;
            }
            set
            {
                this.inner.DesignTimeVisible = value;
            }
        }

        public DbCommand Inner
        {
            get
            {
                return this.inner;
            }
        }

        public override ISite Site
        {
            get
            {
                return this.inner.Site;
            }
            set
            {
                this.inner.Site = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this.inner.UpdatedRowSource;
            }
            set
            {
                this.inner.UpdatedRowSource = value;
            }
        }
    }
}

