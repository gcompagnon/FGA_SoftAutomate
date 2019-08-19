using System.Data;
using System.Data.Common;

namespace Saviso.EntityFramework
{
    public class DbTransactionEx : DbTransaction
    {
        private readonly IAopFilter appender;
        private readonly DbConnectionEx _connectionEx;
        private readonly DbTransaction inner;

        public DbTransactionEx(DbTransaction inner, IAopFilter appender, DbConnectionEx _connectionEx)
        {
            this.inner = inner;
            this.appender = appender;
            this._connectionEx = _connectionEx;
            appender.TransactionBegan(_connectionEx.ConnectionId, inner.IsolationLevel);
        }

        public override void Commit()
        {
            this.inner.Commit();
            this.appender.TransactionCommit(this._connectionEx.ConnectionId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.inner.Dispose();
                this.appender.TransactionDisposed(this._connectionEx.ConnectionId);
            }
            base.Dispose(disposing);
        }

        public override void Rollback()
        {
            this.inner.Rollback();
            this.appender.TransactionRolledBack(this._connectionEx.ConnectionId);
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return this._connectionEx;
            }
        }

        public DbTransaction Inner
        {
            get
            {
                return this.inner;
            }
        }

        public override IsolationLevel IsolationLevel
        {
            get
            {
                return this.inner.IsolationLevel;
            }
        }
    }
}

