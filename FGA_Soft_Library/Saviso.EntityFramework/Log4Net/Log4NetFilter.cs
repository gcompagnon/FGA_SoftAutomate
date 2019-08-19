using System;
using System.Transactions;
using log4net;

namespace Saviso.EntityFramework.Log4Net
{
    public class Log4NetFilter : IAopFilter
    {
        private static readonly ILog LoggerStatements = LogManager.GetLogger(typeof(Log4NetFilter).FullName + ".Statements");
        private static readonly ILog LoggerConnections = LogManager.GetLogger(typeof(Log4NetFilter).FullName + ".Connections");
        private static readonly ILog LoggerTransactions = LogManager.GetLogger(typeof(Log4NetFilter).FullName + ".Transactions");

        [ThreadStatic]
        private static string LocalDtcTransactionId;

        public string Name { get; private set; }

        public Log4NetFilter(string name)
        {
            Name = name;
        }

        public void CommandDurationAndRowCount(Guid connectionId, long milliseconds, int? rowCount)
        {
            LoggerStatements.DebugFormat("Connection {0}: {1}", connectionId, string.Format("Executed in {0} ms with {1} rows", milliseconds, rowCount));
        }

        public void ConnectionDisposed(Guid connectionId)
        {
            LoggerConnections.DebugFormat("Connection {0}: {1}", connectionId, "Connection closed");
        }

        public void ConnectionStarted(Guid connectionId)
        {
            EnlistInDtcTransactionIfNeeded(connectionId);
            LoggerConnections.DebugFormat("Connection {0}: {1}", connectionId, "Connection opened");
        }

        public void DtcTransactionCompleted(Guid connectionId, TransactionStatus status)
        {
            LoggerTransactions.DebugFormat("Connection {0}: {1}", connectionId, "Transaction completed: " + status);
        }

        public void DtcTransactionEnlisted(Guid connectionId, IsolationLevel isolationLevel)
        {
            LoggerTransactions.DebugFormat("Connection {0}: {1}", connectionId, "Transaction enlisted: " + isolationLevel);
        }

        private void EnlistInDtcTransactionIfNeeded(Guid connectionId)
        {
            Transaction current = Transaction.Current;
            if (current != null)
            {
                string localIdentifier = current.TransactionInformation.LocalIdentifier;
                if (localIdentifier != LocalDtcTransactionId)
                {
                    LocalDtcTransactionId = localIdentifier;
                    DtcTransactionEnlisted(connectionId, current.IsolationLevel);
                    current.TransactionCompleted += (sender, args) => DtcTransactionCompleted(connectionId, args.Transaction.TransactionInformation.Status);
                }
            }
        }
       

        public void StatementError(Guid connectionId, Exception exception)
        {
            LoggerStatements.DebugFormat("Connection {0}: {1}", connectionId, exception);
        }

        public void StatementExecuted(Guid connectionId, Guid statementId, string statement)
        {
            EnlistInDtcTransactionIfNeeded(connectionId);
            LoggerStatements.DebugFormat("Connection {0}: {1}", connectionId, string.Concat(new object[] { "-- Statement ", statementId, Environment.NewLine, statement }));
        }

        public void StatementRowCount(Guid connectionId, Guid statementId, int rowCount)
        {
            LoggerStatements.DebugFormat("Connection {0}: {1}", connectionId, string.Format("Read {0} rows for {1}", rowCount, statementId));
        }

        public void TransactionBegan(Guid connectionId, System.Data.IsolationLevel isolationLevel)
        {
            LoggerTransactions.DebugFormat("Connection {0}: {1}", connectionId, "Transaction began: " + isolationLevel);
        }

        public void TransactionCommit(Guid connectionId)
        {
            LoggerTransactions.DebugFormat("Connection {0}: {1}", connectionId, "Transaction committed");
        }

        public void TransactionDisposed(Guid connectionId)
        {
            LoggerTransactions.DebugFormat("Connection {0}: {1}", connectionId, "Transaction disposed");
        }

        public void TransactionRolledBack(Guid connectionId)
        {
            LoggerTransactions.DebugFormat("Connection {0}: {1}", connectionId, "Transaction rollbacked");
        }

    }
}

