using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.ServerClient.Database
{
    public class UniqueIDService : IGameService
    {
        private TheWizards.Database.Database database;
        private int uniqueApplicationID = -1;
        private int lastIncrementalNumber = 0;

        /// <summary>
        /// This is an id that should be different for every installation of the wizards.
        /// Basically, this should be assigned by the server on first init and then never changed.
        /// Currently this is max a 5 digit number
        /// 
        /// </summary>
        public int UniqueApplicationID
        {
            get { return uniqueApplicationID; }
            //set { uniqueApplicationID = value; }
        }

        public UniqueIDService( TheWizards.Database.Database _database )
        {
            database = _database;
            Init();
        }

        public void Init()
        {
            // Load the application id from the settings file
            SettingsService ss = database.FindService<SettingsService>();
            uniqueApplicationID = int.Parse( ss.GetSetting( "UniqueApplicationID", "-1" ) );
            if ( uniqueApplicationID == -1 )
            {
                //throw new Exception( "There is no application ID set in the settings.ini file!!!!" );

                //TODO: WARNING: debugmode only!!!

                MessageBox.Show( "There is no application ID set in the settings.ini file!!!!" );
                string temp = InputBox.ShowInputBox( "Enter a UNIQUE applicationID:", "Enter application id! (DEBUG)" );
                if ( !int.TryParse( temp, out uniqueApplicationID ) )
                {
                    throw new Exception( "Invalid applicationID! (must be integer)" );
                }
                else if ( uniqueApplicationID < 2 ) 
                    throw new Exception( "Invalid applicationID! (must be > 1)" );


                ss.SetSetting( "UniqueApplicationID", uniqueApplicationID.ToString() );
                ss.SaveToDisk();
            }

        }



        /// <summary>
        /// WARNING: In het zeer onwaarschijnlijke scenario dat er een object wordt gemaakt binnen 1 seconde na een herstart
        /// zitten we met een probleem
        /// </summary>
        /// <returns></returns>
        public string GenerateUniqueID()
        {
            if ( uniqueApplicationID == -1 ) throw new InvalidOperationException( "This is not a valid applicationID" );

            DateTime time = DateTime.Now;
            lastIncrementalNumber++;
            if ( lastIncrementalNumber >= 10000 ) lastIncrementalNumber = 0;

            string id = uniqueApplicationID.ToString( "00000" );
            id += "-";
            id += time.ToString( "yyMMdd-HHmmss" );
            //id += "-";
            //id += time.Millisecond.ToString();
            id += "-";
            id += lastIncrementalNumber.ToString( "0000" );


            return id;
        }


        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}
