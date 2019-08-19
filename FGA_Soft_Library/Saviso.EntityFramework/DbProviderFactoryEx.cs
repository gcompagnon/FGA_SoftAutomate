using System;
using System.Data.Common;
using System.Reflection;

namespace Saviso.EntityFramework
{
    public class DbProviderFactoryEx<TConnectionFactory> : DbProviderFactory, IServiceProvider where TConnectionFactory: DbProviderFactory
    {

        protected virtual IAopFilter Filter
        {
            get { return new CompositeFilter(EntityFrameworkExtender.Filters()); }
        }

        private readonly TConnectionFactory inner;
        public static readonly DbProviderFactoryEx<TConnectionFactory> Instance;

        static DbProviderFactoryEx()
        {
            Instance = new DbProviderFactoryEx<TConnectionFactory>();
        }

        public DbProviderFactoryEx()
        {
            FieldInfo field = typeof(TConnectionFactory).GetField("Instance", BindingFlags.Public | BindingFlags.Static);
            this.inner = (TConnectionFactory) field.GetValue(null);
        }

        public override DbCommand CreateCommand()
        {
            return new DbCommandEx(this.inner.CreateCommand(), Filter);
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            return this.inner.CreateCommandBuilder();
        }

        public override DbConnection CreateConnection()
        {
            return new DbConnectionEx(this.inner.CreateConnection(), Filter, Guid.NewGuid(), this);
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return this.inner.CreateConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new DbDataAdapterEx(this.inner.CreateDataAdapter());
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return this.inner.CreateDataSourceEnumerator();
        }

        public override DbParameter CreateParameter()
        {
            return this.inner.CreateParameter();
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == base.GetType())
            {
                return this.inner;
            }
            object service = ((IServiceProvider) this.inner).GetService(serviceType);
            DbProviderServices inner = service as DbProviderServices;
            if (inner != null)
            {
                return new DbProviderServicesEx(inner, Filter);
            }
            return service;
        }

        public override bool CanCreateDataSourceEnumerator
        {
            get
            {
                return this.inner.CanCreateDataSourceEnumerator;
            }
        }
    }
}

