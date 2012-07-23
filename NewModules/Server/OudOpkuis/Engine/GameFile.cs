using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Server.Engine
{
    public class GameFile: Common.Engine.IGameFile
    {
        private GameFileManager manager;
        private int id;
        private byte[] hash;

        /// <summary>
        /// SHA256 hash of the local file data
        /// </summary>
        public byte[] Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public int ID
        {
            get { return id; }
            //set { id = value; }
        }
        private string assetName;

        public string AssetName
        {
            get { return assetName; }
            //set { assetName = value; }
        }

        private int version;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }


        public GameFile( GameFileManager nManager, int nID, string nAssetName )
        {
            manager = nManager;
            id = nID;
            assetName = nAssetName;

            version = 0;
            hash = null;
        }

        //public GameFile( GameFileManager nManager, int nID, string nAssetName, int nVersion )
        //{
        //    manager = nManager;
        //    id = nID;
        //    assetName = nAssetName;

        //    version = nVersion;


        //}

        public void UpdateHash()
        {
            string filename = GetFullFilename();
            if ( System.IO.File.Exists( filename ) == false )
            {
                hash = null;
                return;
            }
            byte[] newHash;
            System.IO.FileStream fs = null;
            System.Security.Cryptography.SHA256Managed hasher = null;
            try
            {
                fs = System.IO.File.Open( filename, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None );
                hasher = new System.Security.Cryptography.SHA256Managed();
                newHash = hasher.ComputeHash( fs );

            }
            finally
            {
                if ( fs != null )
                    fs.Close();
                fs = null;
                if ( hasher != null )
                    hasher.Clear();
                hasher = null;

            }

            if ( hash == null || CheckHashesEqual( newHash, hash ) == false )
            {
                version++;
                hash = newHash;
            }


        }

        public static bool CheckHashesEqual( byte[] hash1, byte[] hash2 )
        {
            if ( hash1.Length != hash2.Length ) return false;
            for ( int i = 0; i < hash1.Length; i++ )
            {
                if ( hash1[ i ] != hash2[ i ] ) return false;
            }
            return true;
        }

        public string GetFullFilename()
        {
            return BuildFileName( this, manager.RootDirectory );
        }

        public static string BuildFileName( GameFile file, string rootDirectory )
        {
            return rootDirectory + "\\" + file.AssetName;
        }

    }
}
