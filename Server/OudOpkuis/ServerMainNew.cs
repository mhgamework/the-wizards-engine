using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Server.Engine;
namespace MHGameWork.TheWizards.Server
{
    public class ServerMainNew : MHGameWork.TheWizards.Common.BaseMain, Common.IGameEngine
    {
        private Engine.TextureManager textureManager;


        private Engine.GameFileManager gameFileManager;
        private Engine.CoreFileManager coreFileManager;

        private GameSettings settings;

        public GameSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        private Common.GameIniFile gameIni;

        public Common.GameIniFile GameIni
        {
            get { return gameIni; }
            set { gameIni = value; }
        }


        public static ServerMainNew Instance;

        private Wereld.Wereld wereld;

        private Network.ServerCommunication communication;

        public ServerMainNew()
            : base()
        {
            Instance = this;
            LoadGameSettings();



            CreateGroundPlane();


            gameFileManager = new MHGameWork.TheWizards.Server.Engine.GameFileManager( System.Windows.Forms.Application.StartupPath + @"\ServerData" );
            textureManager = new MHGameWork.TheWizards.Server.Engine.TextureManager();
            coreFileManager = new MHGameWork.TheWizards.Server.Engine.CoreFileManager( this );


            wereld = new Wereld.Wereld( this );
            communication = new MHGameWork.TheWizards.Server.Network.ServerCommunication( this );



        }

        private void LoadGameSettings()
        {
            string filename = System.Windows.Forms.Application.StartupPath + @"\ServerSettings.xml";
            if ( System.IO.File.Exists( filename ) )
            {
                settings = GameSettings.LoadSettings( filename );
            }
            else
            {
                settings = new GameSettings();
            }

            SaveGameSettings();
        }

        private void SaveGameSettings()
        {
            string filename = System.Windows.Forms.Application.StartupPath + @"\ServerSettings.xml";
            settings.SaveSettings( filename );


        }

        private void LoadGameIni()
        {
            Engine.GameFile file = GameFileManager.FindGameFile( settings.GameIniFileID );
            if ( gameIni == null )
            {
                //Create new GameFile.
                file = GameFileManager.CreateNewGameFile( "GameIni.xml" );
                gameIni = new GameIniFile( this );
                settings.GameIniFileID = file.ID;

                SaveGameSettings();

            }
            else
            {
                gameIni = Common.GameIniFile.Load( this, file );
            }

            gameIni.Save( file );

        }

        private void LoadGame()
        {
            GameFile file = gameFileManager.FindGameFile( gameIni.TerrainListFileID );
            Common.GeoMipMap.TerrainListFile terrainList;
            if ( file == null )
            {
                file = gameFileManager.CreateNewGameFile("TerrainList.xml");

                terrainList = new MHGameWork.TheWizards.Common.GeoMipMap.TerrainListFile();
                gameIni.TerrainListFileID = file.ID;
            }
            else
            {
                terrainList = MHGameWork.TheWizards.Common.GeoMipMap.TerrainListFile.Load( file );
            }

            terrainList.Save( file );

            wereld.TerrainManager.ReadFromTerrainList( terrainList );

            //filename = System.Windows.Forms.Application.StartupPath + @"\ServerData\WereldTerrains.xml";

            //if ( System.IO.File.Exists( filename ) )
            //    wereld.TerrainManager.LoadFromDisk( filename );

            //for ( int i = 0; i < wereld.TerrainManager.Terrains.Count; i++ )
            //{
            //    wereld.TerrainManager.Terrains[ i ].LoadTerrainInfo();
            //    wereld.TerrainManager.Terrains[ i ].Initialize();
            //}
        }


        /// <summary>
        /// Network available
        /// </summary>
        /// <returns></returns>
        public int GetGameIniFileID()
        {
            return settings.GameIniFileID;
        }


        /// <summary>
        /// Client available
        /// </summary>
        /// <returns></returns>
        public byte[] GetGameFilesList()
        {
            ByteWriter br = new ByteWriter();

            GameFileManager.Save( br.MemStrm );

            return br.ToBytesAndClose();


        }
        /// <summary>
        /// Client available
        /// </summary>
        /// <returns></returns>
        public byte[] GetCoreFilesList()
        {
            ByteWriter br = new ByteWriter();

            coreFileManager.SaveXML( br.MemStrm );

            return br.ToBytesAndClose();


        }

        /// <summary>
        /// Client available
        /// </summary>
        /// <returns></returns>
        public byte[] GetGameFileData( int id, int clientVersion, out Engine.GameFile file )
        {
            file = GameFileManager.FindGameFile( id );
            if ( file == null ) return null;

            if ( clientVersion == file.Version ) return null;

            System.IO.FileStream fs = System.IO.File.OpenRead( file.GetFullFilename() );

            byte[] data = new byte[ fs.Length ];

            fs.Read( data, 0, (int)fs.Length );


            fs.Close();


            return data;


        }

        private NxActor groundPlane;
        public void CreateGroundPlane()
        {
            NxPlaneShapeDesc planeDesc = new NxPlaneShapeDesc();
            planeDesc.materialIndex = 0;
            NxActorDesc actorDesc = new NxActorDesc();
            actorDesc.addShapeDesc( planeDesc );

            groundPlane = PhysicsScene.createActor( actorDesc );


        }

        public override void OnProcess( object sender, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            base.OnProcess( sender, e );
            wereld.Process( e );
        }

        public override void OnTick( object sender, MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            base.OnTick( sender, e );
            wereld.Tick( e );
        }

        protected override void ProcessInput()
        {
            //base.ProcessInput();
        }

        public override void Initialize()
        {
            //base.Initialize();

            string filename = System.Windows.Forms.Application.StartupPath + @"\ServerData\GameFiles.xml";

            if ( System.IO.File.Exists( filename ) )
            {
                gameFileManager.LoadFromDisk( filename );
                gameFileManager.UpdateFileHashes();
                gameFileManager.SaveToDisk( filename );
            }



            LoadGameIni();


            





            filename = System.Windows.Forms.Application.StartupPath + @"\ServerData\CoreFiles.xml";

            if ( System.IO.File.Exists( filename ) )
            {
                coreFileManager.LoadFromDisk( filename ); // Should be backwards compatible

                //coreFileManager.CreateNewFile( gameFileManager.CreateNewGameFile( "Core\\Common.dll" ) );

                coreFileManager.SaveToDisk( filename );//Save in case the fileformat changed so it is updated to the new format

            }


            LoadGame();


            Communication.StartListening();


        }

        public override void Dispose()
        {
            string filename = System.Windows.Forms.Application.StartupPath + @"\ServerData\GameFiles.xml";
            gameFileManager.SaveToDisk( filename );

            gameIni.Save( gameFileManager.FindGameFile( GetGameIniFileID() ) );


            filename = System.Windows.Forms.Application.StartupPath + @"\ServerData\CoreFiles.xml";
            coreFileManager.SaveToDisk( filename );


            filename = System.Windows.Forms.Application.StartupPath + @"\ServerData\WereldTerrains.xml";
            wereld.TerrainManager.SaveToDisk( filename );


            for ( int i = 0; i < wereld.TerrainManager.Terrains.Count; i++ )
            {
                wereld.TerrainManager.Terrains[ i ].Dispose();
            }

            base.Dispose();
        }

        public override void DoBeforeRender( object sender )
        {
            //base.DoBeforeRender( sender );
        }
        public override void DoAfterRender( object sender )
        {
            //base.DoAfterRender( sender );
        }
        public override void DoRender( object sender )
        {
            //base.DoRender( sender );
        }


        public Wereld.Wereld Wereld { get { return wereld; } }
        public Network.ServerCommunication Communication { get { return communication; } }
        public Game3DPlay.Core.Elements.ProcessEventArgs ProcessEventArgs { get { return _processEventArgs; } }
        public int Time { get { return _processEventArgs.Time; } }

        public Engine.TextureManager TextureManager
        {
            get { return textureManager; }
            set { textureManager = value; }
        }
        public Engine.GameFileManager GameFileManager
        {
            get { return gameFileManager; }
            set { gameFileManager = value; }
        }

        #region IGameEngine Members

        MHGameWork.TheWizards.Common.Engine.IGameFileManager IGameEngine.GameFileManager
        {
            get { return GameFileManager; }
        }

        #endregion
    }
}

