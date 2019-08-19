using System;
using System.Transactions;

namespace Saviso.EntityFramework
{
    public interface IAopFilter
    {
        void CommandDurationAndRowCount(Guid connectionId, long milliseconds, int? rowCount);
        void ConnectionDisposed(Guid connectionId);
        void ConnectionStarted(Guid connectionId);
        void DtcTransactionCompleted(Guid connectionId, TransactionStatus status);
        void DtcTransactionEnlisted(Guid connectionId, IsolationLevel isolationLevel);
        void StatementError(Guid connectionId, Exception exception);
        void StatementExecuted(Guid connectionId, Guid statementId, string statement);
        void StatementRowCount(Guid connectionId, Guid statementId, int rowCount);
        void TransactionBegan(Guid connectionId, System.Data.IsolationLevel isolationLevel);
        void TransactionCommit(Guid connectionId);
        void TransactionDisposed(Guid connectionId);
        void TransactionRolledBack(Guid connectionId);
    }
}

