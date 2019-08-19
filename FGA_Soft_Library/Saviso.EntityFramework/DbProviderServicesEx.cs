using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;

namespace Saviso.EntityFramework
{
    [CLSCompliant(false)]
    public class DbProviderServicesEx : DbProviderServices
    {
        private readonly IAopFilter appender;
        private readonly DbProviderServices inner;

        public DbProviderServicesEx(DbProviderServices inner, IAopFilter appender)
        {
            this.inner = inner;
            this.appender = appender;
        }

        public override DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
        {
            return new DbCommandDefinitionEx(this.inner.CreateCommandDefinition(prototype), this.appender);
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            return new DbCommandDefinitionEx(this.inner.CreateCommandDefinition(commandTree), this.appender);
        }

        protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            this.inner.CreateDatabase(((DbConnectionEx) connection).Inner, commandTimeout, storeItemCollection);
        }

        protected override string DbCreateDatabaseScript(string providerManifestToken, StoreItemCollection storeItemCollection)
        {
            return this.inner.CreateDatabaseScript(providerManifestToken, storeItemCollection);
        }

        protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            return this.inner.DatabaseExists(((DbConnectionEx) connection).Inner, commandTimeout, storeItemCollection);
        }

        protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            this.inner.DeleteDatabase(((DbConnectionEx) connection).Inner, commandTimeout, storeItemCollection);
        }

        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            return this.inner.GetProviderManifest(manifestToken);
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            return this.inner.GetProviderManifestToken(((DbConnectionEx) connection).Inner);
        }
    }
}

