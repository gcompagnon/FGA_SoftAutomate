using System;
using System.Collections.Generic;
using System.Transactions;

namespace Saviso.EntityFramework
{
    public class CompositeFilter : IAopFilter
    {
        private readonly IEnumerable<IAopFilter> _aopFilters;

        public CompositeFilter(IEnumerable<IAopFilter> aopFilters)
        {
            _aopFilters = aopFilters;
        }

        #region Implementation of IAopFilter

        public void CommandDurationAndRowCount(Guid connectionId, long milliseconds, int? rowCount)
        {
            _aopFilters.Apply(f => f.CommandDurationAndRowCount(connectionId, milliseconds, rowCount));
        }

        public void ConnectionDisposed(Guid connectionId)
        {
            _aopFilters.Apply(f => f.ConnectionDisposed(connectionId));
            
        }

        public void ConnectionStarted(Guid connectionId)
        {
            _aopFilters.Apply(f => f.ConnectionStarted(connectionId));
        }

        public void DtcTransactionCompleted(Guid connectionId, TransactionStatus status)
        {
            _aopFilters.Apply(f => f.DtcTransactionCompleted(connectionId, status));
        }

        public void DtcTransactionEnlisted(Guid connectionId, IsolationLevel isolationLevel)
        {
            _aopFilters.Apply(f => f.DtcTransactionEnlisted(connectionId, isolationLevel));
        }

        public void StatementError(Guid connectionId, Exception exception)
        {
            _aopFilters.Apply(f => f.StatementError(connectionId, exception));
        }

        public void StatementExecuted(Guid connectionId, Guid statementId, string statement)
        {
            _aopFilters.Apply(f => f.StatementExecuted(connectionId, statementId, statement));
        }

        public void StatementRowCount(Guid connectionId, Guid statementId, int rowCount)
        {
            _aopFilters.Apply(f => f.StatementRowCount(connectionId, statementId, rowCount));
        }

        public void TransactionBegan(Guid connectionId, System.Data.IsolationLevel isolationLevel)
        {
            _aopFilters.Apply(f => f.TransactionBegan(connectionId, isolationLevel));
        }

        public void TransactionCommit(Guid connectionId)
        {
            _aopFilters.Apply(f => f.TransactionCommit(connectionId));
        }

        public void TransactionDisposed(Guid connectionId)
        {
            _aopFilters.Apply(f => f.TransactionDisposed(connectionId));
        }

        public void TransactionRolledBack(Guid connectionId)
        {
            _aopFilters.Apply(f => f.TransactionRolledBack(connectionId));
        }

        #endregion
    }
}