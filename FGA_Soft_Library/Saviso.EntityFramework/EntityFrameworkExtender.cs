using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Saviso.EntityFramework.Log4Net;

namespace Saviso.EntityFramework
{
    public static class EntityFrameworkExtender
    {
        public static Func<IEnumerable<IAopFilter>> Filters = () => new[] { new Log4NetFilter(typeof(EntityFrameworkExtender).Name) };

        private static bool Initialized;

        private static void ForceDbProviderFactoriesInitialization()
        {
            try
            {
                DbProviderFactories.GetFactory("does not exists");
            }
            catch (ArgumentException)
            {
            }
        }

        private static DataTable GetDbProvidersFactoriesDataTable()
        {
            // This item is obfuscated and can not be translated.
            ForceDbProviderFactoriesInitialization();
            Type type = typeof(DbProviderFactories);
            FieldInfo providerTableField = type.GetField("_providerTable", BindingFlags.NonPublic | BindingFlags.Static);
            if(providerTableField == null)
            {
                throw  new InvalidOperationException(string.Format("The type {0} doesn't provide Db Providers data table!", type.FullName));
            }

            var providerTable = providerTableField.GetValue(null);
            if (providerTable is DataSet)
            {
                return ((DataSet)providerTable).Tables["DbProviderFactories"];
            }
            return (DataTable)providerTable;
        }

        public static void Initialize()
        {
            SetupEntityFrameworkIntegration();
        }

       
        private static void RewriteProvidersDefinition()
        {
            DataTable dbProvidersFactoriesDataTable = GetDbProvidersFactoriesDataTable();
            List<string> list = new List<string>();
            foreach (DataRow row in dbProvidersFactoriesDataTable.Rows)
            {
                list.Add((string) row["InvariantName"]);
            }
            foreach (string str in list)
            {
                DbProviderFactory factory;
                string cp;
                try
                {
                    factory = DbProviderFactories.GetFactory(str);
                }
                catch (Exception)
                {
                    continue;
                }
                if (factory.GetType().Assembly != typeof(EntityFrameworkExtender).Assembly)
                {
                    Type factoryType = typeof(DbProviderFactoryEx<>).MakeGenericType(new[] { factory.GetType() });
                    cp = str;
                    DataRow row2 = Enumerable.First(dbProvidersFactoriesDataTable.Rows.Cast<DataRow>(), dt => ((string) dt["InvariantName"]) == cp);
                    DataRow row3 = dbProvidersFactoriesDataTable.NewRow();
                    row3["Name"] = row2["Name"];
                    row3["Description"] = row2["Description"];
                    row3["InvariantName"] = row2["InvariantName"];
                    row3["AssemblyQualifiedName"] = factoryType.AssemblyQualifiedName;
                    dbProvidersFactoriesDataTable.Rows.Remove(row2);
                    dbProvidersFactoriesDataTable.Rows.Add(row3);
                }
            }
        }

        private static void SetupEntityFrameworkIntegration()
        {
            if (!Initialized)
            {
                RewriteProvidersDefinition();
                Initialized = true;
            }
        }

        public static void Shutdown()
        {
            Initialized = false;
        }
    }
}

