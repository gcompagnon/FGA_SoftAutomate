using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace SQLCopy.Helpers
{
    /// <summary>
    /// A Delegate to DbParameterCollection for giving enhanced method on DbParameterCollection
    /// coming from a DbCommand.Parameters
    /// </summary>
    public class DbParameterCollectionDelegate
    {
        /// <summary>
        /// Gets or sets the command object where is coming the collection
        /// </summary>               
        public DbCommand Command
        {
            get;
            set;
        }

        public DbParameterCollectionDelegate(DbCommand cmd)
        {
            this.Command = cmd;
        }

        public int Add()
        {
            DbParameter parameter = Command.CreateParameter();
            return Command.Parameters.Add(parameter);
        }

        public int Add(string parameterName, object parameterValue)
        {
            DbParameter parameter = Command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = (parameterValue == null) ? DBNull.Value : parameterValue;
            return Command.Parameters.Add(parameter);
        }
    }
}
