using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.IO;
using System.Data.Entity.Infrastructure;

namespace FGABusinessComponent.BusinessComponent.Util
{
    /// <summary>
    /// Utilitaire pour créer le fichier SQL de création du Context
    /// </summary>
    public static class EFCodeFirstMethods
    {
        public static void DumpDbCreationScriptToFile(DbContext context)
        {
            var script = CreateDbScript(context);

            var appDomainPath = AppDomain.CurrentDomain.BaseDirectory;

            var scriptPath = GetScriptsDirectory(appDomainPath);

            var scriptName = Path.Combine(scriptPath, "db_create.sql");

            WriteTextToFile(script, DateTime.UtcNow.ToString("MM-dd-yyyy-H-mm-ss"), scriptName);
        }

        private static string GetScriptsDirectory(string appDomainPath)
        {
            var oneLevelUp = Directory.GetParent(appDomainPath);
            var twoLevelUp = Directory.GetParent(oneLevelUp.FullName);
            var threeLevelUp = Directory.GetParent(twoLevelUp.FullName);
            return Path.Combine(threeLevelUp.FullName, "scripts");
        }


        public static void WriteTextToFile(string text, string timestamp, string filename)
        {
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.WriteLine(timestamp);
                sw.Write(text);
            }
        }

        public static string CreateDbScript(DbContext context)
        {
            var script = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
            return script;
        }
    }
}
