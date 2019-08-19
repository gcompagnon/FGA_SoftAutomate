using System.Data.Common;

namespace Saviso.EntityFramework
{
    public class DbCommandDefinitionEx : DbCommandDefinition
    {
        private readonly IAopFilter appender;
        private readonly DbCommandDefinition inner;

        public DbCommandDefinitionEx(DbCommandDefinition inner, IAopFilter appender)
        {
            this.inner = inner;
            this.appender = appender;
        }

        public override DbCommand CreateCommand()
        {
            return new DbCommandEx(this.inner.CreateCommand(), this.appender);
        }
    }
}

