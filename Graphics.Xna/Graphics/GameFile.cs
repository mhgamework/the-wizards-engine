using System;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    public class GameFile : IDisposable, IGameFile
    {
        //private GameFileManager manager;

        private int id;
        public int ID
        {
            get { return id; }
        }

        private string assetName;

        public string AssetName
        {
            get { return assetName; }
            set
            {
                assetName = value;
                if ( assetName.Contains( ".." ) ) throw new Exception( "The AssetName cannot contain '..' for security reasons" );
            }
        }


        public enum GameFileState
        {
            /// <summary>
            /// The server and client version of the file match and the file is ready for use.
            /// </summary>
            UpToDate,
            /// <summary>
            /// The version of the client's file doesn't match the server's version.
            /// </summary>
            OutOfDate,
            /// <summary>
            /// The file is being synchronized.
            /// </summary>
            Synchronizing,

            ///// <summary>
            ///// This GameFile was created by the client, it can't be be synchronized.
            ///// </summary>
            //ClientFile,

            ///// This file is not managed by the engine. It is merely used for debugging purposes.
            //DebugFile,

            //
            // OUD
            //

            /// <summary>
            /// DEPRECATED. The data is locally available.
            /// </summary>
            FileExists,
            /// <summary>
            /// DEPRECATED. The server synchronized a GameFile, but the data has not yet been sychronized
            /// </summary>
            FileDoesntExist,
            /// <summary>
            /// DEPRECATED. This GameFile is referenced by an entity, terrain, ... but the server has not yet reported its existance.
            /// </summary>
            Unknown,
            /// <summary>
            /// DEPRECATED. This gamefile is disposed and may no longer be used.
            /// </summary>
            Disposed
        }

        private GameFileState state;

        public GameFileState State
        {
            get { return state; }
            //set { state = value; }
        }

        private int version;
        //public int Version
        //{
        //    get { return version; }
        //    set { version = value; UpdateState(); }
        //}

        private int serverVersion;

        public int ServerVersion
        {
            get { return serverVersion; }
            //set { serverVersion = value; }
        }

        private byte[] serverHash;

        public byte[] ServerHash
        {
            get { return serverHash; }
            set { serverHash = value; }
        }


        //public GameFile( GameFileManager nManager, int nID, string nAssetName )
        //{
        //    manager = nManager;
        //    id = nID;
        //    assetName = nAssetName;

        //    version = 0;
        //    serverVersion = -1;
        //    serverHash = null;


        //    state = GameFileState.OutOfDate;

        //    if ( IsClientGameFile() ) state = GameFileState.UpToDate;

        //}

        /// <summary>
        /// Note: this was originally Debug only, but it will probably be used in the engine for preprocessing for example in the ColladaModel class
        /// DEBUG ONLY! This creates a gamefile that is not managed by the engine.
        /// </summary>
        /// <param name="fileName"></param>
        public GameFile( string fullfilename )
        {
            //manager = null;
            id = -2;
            assetName = fullfilename;

            version = 0;
            serverVersion = -1;
            serverHash = null;


            state = GameFileState.UpToDate;

            if ( version == 0 )
            {
            }
        }

        public override string ToString()
        {
            return "GameFile: " + assetName;
        }

        public void Dispose()
        {
            state = GameFileState.Disposed;
        }

        //public void UpdateState()
        //{
        //    if ( IsClientGameFile() ) state = GameFileState.UpToDate;
        //    if ( IsDebugFile() ) state = GameFileState.UpToDate;


        //    if ( state == GameFileState.Synchronizing ) return;
        //    if ( version == -1 )
        //        state = GameFileState.OutOfDate;
        //    else if ( version == serverVersion )
        //        state = GameFileState.UpToDate;
        //    else
        //        state = GameFileState.OutOfDate;
        //}

        ///// <summary>
        ///// Synchronizes this gamefile and its data.
        ///// Redundant calls we be ignored.
        ///// </summary>
        //public void SynchronizeAsync()
        //{
        //    if ( IsClientGameFile() || IsDebugFile() ) { state = GameFileState.UpToDate; return; }
        //    manager.Engine.SynchronizeGameFileAsync( this );
        //}

        //public void OnStartedSynchronizing()
        //{
        //    state = GameFileState.Synchronizing;
        //}

        //public void OnSynchronizingComplete( int newVersion )
        //{
        //    version = newVersion;

        //    //Just to clear the Synchronizing value
        //    state = GameFileState.OutOfDate;
        //    UpdateState();
        //}

        //public void SetServerVersion( int nServerVersion )
        //{
        //    serverVersion = nServerVersion;
        //    UpdateState();
        //}

        public string GetFullFilename()
        {
            //    string filename;

            //    if ( IsClientGameFile() )
            //    {
            //        filename = BuildFileName( this, manager.RootDirectoryClientData );
            //    }
            //    else if ( IsDebugFile() )
            //    {
            return assetName;
            //    }
            //    else
            //    {

            //        if ( state != GameFileState.UpToDate ) throw new InvalidOperationException( "Cannot use this GameFile as it is not up to date" );
            //        //if ( state == GameFileState.Unknown || state == GameFileState.Disposed ) throw new InvalidOperationException();


            //        filename = BuildFileName( this, manager.RootDirectoryServerData );


            //    }
            //    CreateDirectory( filename );
            //    return filename;
        }

        //public void CreateDirectory()
        //{
        //    CreateDirectory( GetFullFilename() );
        //}

        //private void CreateDirectory( string filename )
        //{
        //    System.IO.FileInfo file = new System.IO.FileInfo( filename );
        //    //System.Diagnostics.Debugger.Break();
        //    file.Directory.Create();
        //}


        //public bool IsEmpty
        //{
        //    get { return id == -1; }
        //}

        //public bool IsClientGameFile()
        //{
        //    return id > GameFileManager.ClientGameFileStartID;
        //}
        //public bool IsDebugFile()
        //{
        //    return manager == null;
        //}

        //private static string BuildFileName( GameFileOud file, string rootDirectory )
        //{
        //    if ( file.IsDebugFile() ) throw new InvalidOperationException( "Impossible to build a filename for a debugfile" );
        //    if ( !file.IsClientGameFile() && file.State != GameFileState.UpToDate ) throw new InvalidOperationException( "Cannot use this GameFile as it is not up to date" );
        //    if ( file.assetName.StartsWith( "." ) ) throw new Exception( "The assetname should not start with '.' for security reasons" );
        //    return rootDirectory + "\\" + file.AssetName;
        //}

    }
}
