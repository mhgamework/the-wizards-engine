using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace CoreFileUpdater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main( string[] args )
        {



            string updateRootStr = Application.StartupPath;
            string coreFilesRootStr = updateRootStr + @"\ServerSavedData\Core";

            DateTime time = DateTime.Now;
            Random rand = new Random();


            DirectoryInfo rootDir = new DirectoryInfo( updateRootStr );
            DirectoryInfo coreFilesDir = new DirectoryInfo( coreFilesRootStr );
            DirectoryInfo backupDir = new DirectoryInfo( updateRootStr + @"\CoreUpdateBackup\B"
                + string.Format( "{0:yyMMdd}-{0:HHmmss}-{1:0000}", time, rand.Next( 0, 9999 ) ) );
            //+ time.ToString("dd"
            //+ time.Year.ToString().Substring( 2, 2 ) + time.Month.ToString() + time.Day.ToString()
            //+ " "
            //+ time.Hour.ToString() + time.Minute.ToString() + time.Second.ToString()
            //+ " "
            //+ rand.Next( 0, 9999 ).ToString() );

            //backupDir.Create();


            List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();
            //processes.AddRange( System.Diagnostics.Process.GetProcessesByName( "ServerClient" ) );
            processes.AddRange( System.Diagnostics.Process.GetProcesses() );
            while ( processes.Count > 0 )
            {
                System.Diagnostics.Process[] procs = processes.ToArray();
                foreach ( System.Diagnostics.Process proc in procs )
                {
                    try
                    {
                        if ( proc.Id == System.Diagnostics.Process.GetCurrentProcess().Id )
                        {
                            processes.Remove( proc );
                        }
                        else if ( proc.MainModule.FileName.StartsWith( rootDir.FullName ) )
                        {

                            FileInfo execFile = new FileInfo( proc.MainModule.FileName );
                            Console.WriteLine( "Waiting for '" + execFile.Name + "' to shutdown..." );
                            System.Threading.Thread.Sleep( 2000 );

                            if ( proc.HasExited ) processes.Remove( proc );
                        }
                        else
                        {
                            processes.Remove( proc );
                        }
                    }
                    catch ( Exception ex )
                    {
                        processes.Remove( proc );
                    }
                }
            }



            UpdateDirectory( coreFilesDir, rootDir, backupDir );



            Console.WriteLine( "Updating of CoreFiles completed! Starting The Wizards in 3 seconds..." );

            System.Threading.Thread.Sleep( 3000 );

            if ( args.Length > 0 )
                System.Diagnostics.Process.Start( args[ 0 ] );
        }

        public static void UpdateDirectory( DirectoryInfo sourceDir, DirectoryInfo targetDir, DirectoryInfo backupDir )
        {
            FileInfo[] files = sourceDir.GetFiles();

            if ( files.Length == 0 ) return;
            targetDir.Create();
            backupDir.Create();

            for ( int i = 0; i < files.Length; i++ )
            {
                FileInfo sourceFile = new FileInfo( sourceDir.FullName + "\\" + files[ i ].Name );
                FileInfo targetFile = new FileInfo( targetDir.FullName + "\\" + files[ i ].Name );
                FileInfo backupFile = new FileInfo( backupDir.FullName + "\\" + files[ i ].Name );
                UpdateFile( sourceFile, targetFile, backupFile );
            }

            DirectoryInfo[] dirs = sourceDir.GetDirectories();
            for ( int i = 0; i < dirs.Length; i++ )
            {
                DirectoryInfo subSourceDir = new DirectoryInfo( sourceDir.FullName + "\\" + dirs[ i ].Name );
                DirectoryInfo subTargetDir = new DirectoryInfo( targetDir.FullName + "\\" + dirs[ i ].Name );
                DirectoryInfo subBackupDir = new DirectoryInfo( backupDir.FullName + "\\" + dirs[ i ].Name );
                UpdateDirectory( subSourceDir, subTargetDir, subBackupDir );
            }
        }

        public static void UpdateFile( FileInfo sourceFile, FileInfo targetFile, FileInfo backupFile )
        {
            //Do not update itself
            if ( sourceFile.Name == "CoreFileUpdater.exe" ) return;

            string targetPath = targetFile.FullName;
            if ( targetFile.Exists == true )
            {

                if ( backupFile.Exists ) throw new Exception( "Backupfile can't exist!" );
                targetFile.MoveTo( backupFile.FullName );


            }
            sourceFile.CopyTo( targetPath );
            Console.WriteLine( "Copied '" + sourceFile.Name + "'" );
        }

    }
}