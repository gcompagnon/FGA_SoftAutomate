using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer;



using System.Data.SqlClient;
using System.Configuration;





namespace FGA.SQLCopy
{
    /// <summary>
    /// Code utilisé 
    /// </summary>
    public class BackupCode
    {
        private static void CompletionStatusInPercent(object sender, PercentCompleteEventArgs args)
        {
            Console.Clear();
            Console.WriteLine("Percent completed: {0}%.", args.Percent);
        }
        private static void Backup_Completed(object sender, ServerMessageEventArgs args)
        {
            Console.WriteLine("Hurray...Backup completed.");
            Console.WriteLine(args.Error.Message);
        }
        private static void Restore_Completed(object sender, ServerMessageEventArgs args)
        {
            Console.WriteLine("Hurray...Restore completed.");
            Console.WriteLine(args.Error.Message);
        }

        public static void exec()
        {
            //Set destination connection string
            ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Connection2"];
            SqlConnection Connection = new SqlConnection(connectionString.ToString());

            //SMO Server object setup with SQLConnection.
            Server myServer = new Server(new ServerConnection(Connection));

            Backup bkpDBFull = new Backup();
            /* Specify whether you want to back up database or files or log */
            bkpDBFull.Action = BackupActionType.Database;
            /* Specify the name of the database to back up */
            bkpDBFull.Database = "FGA_DEV";
            /* You can take backup on several media type (disk or tape), here I am
             * using File type and storing backup on the file system */
            bkpDBFull.Devices.AddDevice(@"C:\AdventureWorksFull.bak", DeviceType.File);
            bkpDBFull.BackupSetName = "Adventureworks database Backup";
            bkpDBFull.BackupSetDescription = "Adventureworks database - Full Backup";
            /* You can specify the expiration date for your backup data
             * after that date backup data would not be relevant */
            bkpDBFull.ExpirationDate = DateTime.Today.AddDays(10);

            /* You can specify Initialize = false (default) to create a new 
             * backup set which will be appended as last backup set on the media. You
             * can specify Initialize = true to make the backup as first set on the
             * medium and to overwrite any other existing backup sets if the all the
             * backup sets have expired and specified backup set name matches with
             * the name on the medium */
            bkpDBFull.Initialize = false;

            /* Wiring up events for progress monitoring */
            bkpDBFull.PercentComplete += CompletionStatusInPercent;
            bkpDBFull.Complete += Backup_Completed;

            /* SqlBackup method starts to take back up
             * You can also use SqlBackupAsync method to perform the backup 
             * operation asynchronously */
            bkpDBFull.SqlBackup(myServer);
        }


        //-----------------------------------------------------------------

        public static bool backupBase(string baseASauvergarder, string fichierSauvegarde)
        {
            //baseASauvergarder : base de données que l'on souhaite sauvegarder
    	    //fichierSauvegarde : chemin complet de la sauvegarde, par exemple: "c:\maSauvegarde.bak"
            bool etatSauvegarde;

           
                // To Connect to our SQL Server - 
                // we Can use the Connection from the System.Data.SqlClient Namespace.
                ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Connection2"];
                SqlConnection sqlConnection = new SqlConnection(connectionString.ToString());


                //build a "serverConnection" with the information of the "sqlConnection"
                ServerConnection serverConnection =  new ServerConnection(sqlConnection);

                //The "serverConnection is used in the ctor of the Server.
                Server monServeur = new Server(serverConnection);

                //Instanciation d'un objet SMO.Backup qui va nous permettre de réaliser notre backup
                Backup maSauvegarde = new Backup();
            //Dim maSauvegarde As New Backup

                //Définition du type d'action de sauvergarde
                maSauvegarde.Action = BackupActionType.Database;
            //maSauvegarde.Action = BackupActionType.Database

                //Base de données à sauvegarder
            //            maSauvegarde.Database = nomBaseBackup
            maSauvegarde.Database = baseASauvergarder;

                //Choix du périph et de la destination de la sauvegarde
            maSauvegarde.Devices.AddDevice(fichierSauvegarde, DeviceType.File);
            

                //Réalisation de la sauvegarde
            maSauvegarde.SqlBackup(monServeur);

                etatSauvegarde = true;

       
            return etatSauvegarde;
        }

    	public static bool restaureBase(string cheminSauvegarde, string nomBase)
        {
            //nomBase : base dans laquelle la sauvegarde doit être restaurée
		    //cheminSauvegarde : chemin complet du fichier contenant la sauvegarde,
		    //                   par exemple "c:\maSauvegarde.bak"


            bool etatRestauration ;


                // To Connect to our SQL Server - 
                // we Can use the Connection from the System.Data.SqlClient Namespace.
                ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["Connection2"];
                SqlConnection sqlConnection = new SqlConnection(connectionString.ToString());


                //build a "serverConnection" with the information of the "sqlConnection"
                ServerConnection serverConnection =  new ServerConnection(sqlConnection);

                //The "serverConnection is used in the ctor of the Server.
                Server monServeur = new Server(serverConnection);

                //Instanciation d'un objet SMO.Restore qui va nous permettre de réaliser notre restauration
                Restore maRestauration = new Restore();

                //Nom de la base à restaurer
                maRestauration.Database = nomBase;

                //Type de restauration : restauration d'une base de données
                maRestauration.Action = RestoreActionType.Database;

                //Chemin vers le fichier ou se trouve la sauvegarde à restaurer
                maRestauration.Devices.AddDevice(cheminSauvegarde, DeviceType.File);

                //Action à effectuer si la base existe déjà
                maRestauration.ReplaceDatabase = true;

                //Réalisation de la restauration
                maRestauration.SqlRestore(monServeur);

                etatRestauration = true;


            return etatRestauration;
        }


    }
}
