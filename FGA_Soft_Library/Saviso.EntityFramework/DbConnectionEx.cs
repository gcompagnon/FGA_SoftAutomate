using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace Saviso.EntityFramework
{
    public class DbConnectionEx : DbConnection
    {
        private readonly IAopFilter appender;
        private readonly Guid connectionId;
        private readonly DbConnection inner;
        private readonly System.Data.Common.DbProviderFactory providerFactory;

        public event StateChangeEventHandler StateChange
        {
            add
            {
                this.inner.StateChange += value;
            }
            remove
            {
                this.inner.StateChange -= value;
            }
        }

        public DbConnectionEx(DbConnection inner, IAopFilter appender, Guid connectionId, System.Data.Common.DbProviderFactory providerFactory)
        {
            this.inner = inner;
            this.providerFactory = providerFactory;
            this.connectionId = connectionId;
            this.appender = appender;
            appender.ConnectionStarted(connectionId);
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new DbTransactionEx(this.inner.BeginTransaction(isolationLevel), this.appender, this);
        }

        public override void ChangeDatabase(string databaseName)
        {
            this.inner.ChangeDatabase(databaseName);
        }

        public override void Close()
        {
            this.inner.Close();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new DbCommandEx(this.inner.CreateCommand(), this, this.appender);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.NotifyClosing();
                this.inner.Dispose();
            }
            base.Dispose(disposing);
        }

        public override void EnlistTransaction(Transaction transaction)
        {
            this.inner.EnlistTransaction(transaction);
            if (transaction != null)
            {
                transaction.TransactionCompleted += new TransactionCompletedEventHandler(this.OnDtcTransactionCompleted);
                this.appender.DtcTransactionEnlisted(this.connectionId, transaction.IsolationLevel);
            }
        }

        public override DataTable GetSchema()
        {
            return this.inner.GetSchema();
        }

        public override DataTable GetSchema(string collectionName)
        {
            return this.inner.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return this.inner.GetSchema(collectionName, restrictionValues);
        }

        protected override object GetService(Type service)
        {
            return ((IServiceProvider) this.inner).GetService(service);
        }

        private void NotifyClosing()
        {
            this.appender.ConnectionDisposed(this.connectionId);
        }

        private void OnDtcTransactionCompleted(object sender, TransactionEventArgs args)
        {
            TransactionStatus aborted;
            try
            {
                aborted = args.Transaction.TransactionInformation.Status;
            }
            catch (ObjectDisposedException)
            {
                aborted = TransactionStatus.Aborted;
            }
            this.appender.DtcTransactionCompleted(this.connectionId, aborted);
        }

        public override void Open()
        {
            this.inner.Open();
        }

        public Guid ConnectionId
        {
            get
            {
                return this.connectionId;
            }
        }

        public override string ConnectionString
        {
            get
            {
                return this.inner.ConnectionString;
            }
            set
            {
                this.inner.ConnectionString = value;
            }
        }

        public override int ConnectionTimeout
        {
            get
            {
                return this.inner.ConnectionTimeout;
            }
        }

        public override string Database
        {
            get
            {
                return this.inner.Database;
            }
        }

        public override string DataSource
        {
            get
            {
                return this.inner.DataSource;
            }
        }

        protected override System.Data.Common.DbProviderFactory DbProviderFactory
        {
            get
            {
                return this.providerFactory;
            }
        }

        public DbConnection Inner
        {
            get
            {
                return this.inner;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return this.inner.ServerVersion;
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

        public override ConnectionState State
        {
            get
            {
                return this.inner.State;
            }
        }
    }
}

