using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient.Engine;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ModeLoading : IGameMode
    {
        ServerClientMainOud engine;

        private SpriteFont font;
        private Texture2D backgroundTexture;

        private LoadState state;

        private enum LoadState
        {
            None,
            GetGameFilesListStart,
            GetGameFilesListRecieving,
            GetGameIniFileIDStart,
            GetGameIniFileIDRecieving

        }

        public bool processLoadingTasks;

        private Editor.GuiOutputBox outputBox;

        public Editor.GuiOutputBox OutputBox
        {
            get { return outputBox; }
            set { outputBox = value; }
        }

        public ModeLoading( ServerClientMainOud nEngine )
        {
            engine = nEngine;

        }



        #region IGameObject Members

        public void Initialize()
        {
            font = engine.XNAGame.Content.Load<SpriteFont>( @"Content\ComicSansMS" );
            LoadGraphicsContent();
        }

        public void LoadGraphicsContent()
        {
            if ( backgroundTexture != null ) backgroundTexture.Dispose();
            backgroundTexture = Texture2D.FromFile( engine.XNAGame.GraphicsDevice, engine.XNAGame.Content.RootDirectory + @"\Content\LoadingBackground001.dds" );
        }

        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            LoadingTaskState taskState = LoadTask();
            if (taskState == MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking )
            {
                state++;
            }

            if ( processLoadingTasks )
            {
                engine.LoadingManager.ProcessNextTaskInterval( MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing, 100 );
                //if ( engine.LoadingManager.IsDoneLoading( Engine.LoadingTaskType.PreProccesing ) )
                //    engine.LoadingManager.ProcessNextTaskInterval( MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal, 0 );

                //engine.Wereld.AddLoadTasks();

                if ( engine.LoadingManager.IsDoneLoading( Engine.LoadingTaskType.PreProccesing ) )
                //if ( engine.LoadingManager.IsDoneLoading() )
                {
                    processLoadingTasks = false;
                    outputBox.AddLine( "" );
                    outputBox.AddLine( "Loading complete. Starting The Wizards ..." );
                    if ( engine.Settings.DelayedLoading )
                        engine.Invoker.Invoke<EventArgs>( OnCompleted, this, null, engine.Time + engine.Settings.DelayedLoadingTime );
                    else
                        engine.Invoker.Invoke<EventArgs>( OnCompleted, this, null, engine.Time + 1000 );
                }
            }


        }

        public Engine.LoadingTaskState LoadTask()
        {
            switch ( state )
            {
                case LoadState.GetGameFilesListStart:

                    outputBox.AddLine( "Retrieving GameFile list ..." );

                    engine.Server.GetGameFilesListCompleted += new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFilesListEventArgs>( Server_GetGameFilesListCompleted );
                    engine.Server.GetGameFilesListAsync();
                    return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;

                case LoadState.GetGameFilesListRecieving:
                    return LoadingTaskState.Idle;

                case LoadState.GetGameIniFileIDStart:
                    engine.Server.GetGameIniFileIDAsync();
                    return LoadingTaskState.Subtasking;

                case LoadState.GetGameIniFileIDRecieving:
                    if ( engine.GameIniFileID == -1 )
                        return LoadingTaskState.Idle;
                    else
                        return LoadingTaskState.Subtasking;

            }

            return LoadingTaskState.Completed;
        }

        void Server_GetGameFilesListCompleted( object sender, MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFilesListEventArgs e )
        {
            engine.Server.GetGameFilesListCompleted -= new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFilesListEventArgs>( Server_GetGameFilesListCompleted );

            outputBox.AddLine( "GameFile list received. Local GameFile list updated!" );
            engine.GameFileManager.LoadServerGameFilesListData( e.BR.BaseStream );
            outputBox.AddLine( "" );

            state++;

            //engine.Server.GetGameIniFileIDAsync();

        }



        public void Render()
        {
            Microsoft.Xna.Framework.Graphics.GraphicsDevice dev = engine.XNAGame.GraphicsDevice;

            //dev.Clear( Microsoft.Xna.Framework.Graphics.Color.Black );

            engine.SpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );



            engine.SpriteBatch.Draw( backgroundTexture
                , new Rectangle( 0, 0, engine.XNAGame.Window.ClientBounds.Width, engine.XNAGame.Window.ClientBounds.Height )
                , Color.White );

            outputBox.Draw( engine.SpriteBatch );


            engine.SpriteBatch.End();

        }



        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

        }

        public void StartGameMode()
        {
            //outputBox = new MHGameWork.TheWizards.ServerClient.Editor.GuiOutputBox( 0, 0, engine.XNAGame.Window.ClientBounds.Width, engine.XNAGame.Window.ClientBounds.Height );
            outputBox = new MHGameWork.TheWizards.ServerClient.Editor.GuiOutputBox( 217, 144, 805, 603 );
            outputBox.Font = font;
            outputBox.TextColor = Color.Black;
            outputBox.LineSpacing = 30;

            outputBox.AddLine( "Loading The Wizards ..." );
            outputBox.AddLine( "" );


            state = LoadState.None;
            state++;

            return;

            /*outputBox.AddLine( "Retrieving GameFile list ..." );

            engine.Server.GetGameFilesListCompleted += new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFilesListEventArgs>( Server_GetGameFilesListCompleted );
            engine.Server.GetGameFilesListAsync();*/

        }



        void RetrieveTerrainList()
        {

            outputBox.AddLine( "Retrieving Terrain list ..." );
            engine.Server.Wereld.GetTerrainsListCompleted += new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServerWereld.GetTerrainsListEventArgs>( Wereld_GetTerrainsListCompleted );
            engine.Server.Wereld.GetTerrainsListAsync();

        }

        //private List<Engine.GameFileOud> synchronizationFiles;

        void Wereld_GetTerrainsListCompleted( object sender, MHGameWork.TheWizards.ServerClient.Network.ProxyServerWereld.GetTerrainsListEventArgs e )
        {
            engine.Server.Wereld.GetTerrainsListCompleted -= new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServerWereld.GetTerrainsListEventArgs>( Wereld_GetTerrainsListCompleted );
            outputBox.AddLine( "-Terrain list received. Local list updated!" );
            engine.Wereld.TerrainManager.LoadServerData( e.BR.BaseStream );

            outputBox.AddLine( "Retrieving CoreFile list ..." );
            engine.Server.GetCoreFilesListCompleted += new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetCoreFilesListEventArgs>( Server_GetCoreFilesListCompleted );
            engine.Server.GetCoreFilesListAsync();


            /*outputBox.AddLine( "-Getting TerrainInfo file from the first terrain..." );

            engine.SynchronizeGameFileCompleted += new EventHandler<ServerClientMain.SynchronizeGameFileEventArgs>( engine_SynchronizeGameFileCompleted );

            synchronizationFiles = new List<MHGameWork.TheWizards.ServerClient.Engine.GameFile>();

            SynchronizeFile( engine.Wereld.TerrainManager.Terrains[ 0 ].TerrainFile );
            
            //engine.Server.GetGameFileDataAsync( engine.Wereld.TerrainManager.Terrains[ 0 ].TerrainFile.ID );*/



        }

        void Server_GetCoreFilesListCompleted( object sender, MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetCoreFilesListEventArgs e )
        {
            engine.Server.GetCoreFilesListCompleted -= new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetCoreFilesListEventArgs>( Server_GetCoreFilesListCompleted );
            outputBox.AddLine( "-CoreFile list received. Local list updated!" );
            engine.CoreFileManager.LoadXML( e.BR.BaseStream );

            processLoadingTasks = true;
            engine.LoadingManager.AddLoadTaskAdvanced( LoadGameTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );


            //engine.Invoker.Invoke( LoadGame, engine.Time + 1 );
        }

        /*void SynchronizeFile(Engine.GameFile file)
        {
            synchronizationFiles.Add( file );
            engine.SynchronizeGameFileAsync( file );
        }*/

        void engine_SynchronizeGameFileCompleted( object sender, ServerClientMainOud.SynchronizeGameFileEventArgs e )
        {
            /*synchronizationFiles.Remove( e.File );
            if ( e.File == engine.Wereld.TerrainManager.Terrains[ 0 ].TerrainFile )
            {
                engine.Wereld.TerrainManager.Terrains[ 0 ].LoadTerrainInfo();
                outputBox.AddLine( "-TerrainInfo recieved. Requesting heightmap..." );
                SynchronizeFile( engine.GameFileManager.GetGameFile( engine.Wereld.TerrainManager.Terrains[ 0 ].TerrainInfo.HeightMapFileID ) );
            }
            if ( e.File == engine.GameFileManager.GetGameFile( engine.Wereld.TerrainManager.Terrains[ 0 ].TerrainInfo.HeightMapFileID ) )
            {
                outputBox.AddLine( "-Heightmap recieved." );
            }
            if ( synchronizationFiles.Count == 0 )
            {
                engine.SynchronizeGameFileCompleted -= new EventHandler<ServerClientMain.SynchronizeGameFileEventArgs>( engine_SynchronizeGameFileCompleted );

                outputBox.AddLine( "Synchronization completed!" );

                outputBox.AddLine( "Loading..." );

                engine.Invoker.Invoke( LoadGame, engine.Time + 1 );
            }*/
        }

        List<Engine.CoreFile> outdatedCoreFiles;

        Engine.LoadingTaskState LoadGameTask( Engine.LoadingTaskType taskType )
        {

            if ( outdatedCoreFiles == null )
            {
                //System.Diagnostics.Debugger.Break();
                outdatedCoreFiles = new List<MHGameWork.TheWizards.ServerClient.Engine.CoreFile>();
                for ( int i = 0; i < engine.CoreFileManager.Files.Count; i++ )
                {
                    if ( engine.CoreFileManager.Files[ i ].GameFile.State == MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.OutOfDate )
                    {
                        outdatedCoreFiles.Add( engine.CoreFileManager.Files[ i ] );
                    }
                }

                outputBox.AddLine( "Synchronizing " + outdatedCoreFiles.Count + " CoreFile's..." );

                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            if ( outdatedCoreFiles.Count != 0 )
            {
                if ( outdatedCoreFiles[ 0 ].GameFile.State != MHGameWork.TheWizards.ServerClient.Engine.GameFileOud.GameFileState.UpToDate )
                {
                    outdatedCoreFiles[ 0 ].GameFile.SynchronizeAsync();
                }
                else
                {
                    outputBox.AddLine( "Synchronized " + outdatedCoreFiles[ 0 ].TargetFilename );
                    outdatedCoreFiles.RemoveAt( 0 );

                    if ( outdatedCoreFiles.Count == 0 && engine.Settings.AutoUpdateCoreFiles )
                    {
                        //Shutdown the wizards and start the corefileupdater

                        System.IO.File.Copy( engine.GameFileManager.RootDirectoryServerData + "\\Core\\CoreFileUpdater.exe"
                                , System.Windows.Forms.Application.StartupPath + "\\CoreFileUpdater.exe", true );



                        System.Diagnostics.Process.Start( System.Windows.Forms.Application.StartupPath + "\\CoreFileUpdater.exe",
                             "\"" + System.Windows.Forms.Application.StartupPath + "\\ServerClient.exe" + "\"" );
                        engine.Exit();
                    }
                }

                return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Subtasking;
            }

            XNAGeoMipMap.Terrain terr = engine.Wereld.TerrainManager.Terrains[ 0 ];

            //Load Terrain

            //terr.Initialize();
            //terr.LoadGraphicsContent();
            //engine.LoadingManager.AddLoadTaskAdvanced( terr.InitializeTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );
            //engine.LoadingManager.AddLoadTaskAdvanced( terr.LoadGraphicsContentTask, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );

            terr.AddLoadTasks( engine.LoadingManager, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.PreProccesing );
            terr.AddLoadTasks( engine.LoadingManager, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal );

            //engine.Wereld.AddLoadTasks( MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Normal, new BoundingSphere( Vector3.Zero, 10000f ), engine.Wereld.Tree, false );
            //terr.AddLoadTasks( engine.LoadingManager, MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskType.Detail );



            return MHGameWork.TheWizards.ServerClient.Engine.LoadingTaskState.Completed;

        }



        //void Server_GetGameFileDataCompleted( object sender, MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFileDataEventArgs e )
        //{
        //    if ( e.ID == engine.Wereld.TerrainManager.Terrains[ 0 ].TerrainFile.ID )
        //    {
        //        XNAGeoMipMap.Terrain terr = engine.Wereld.TerrainManager.Terrains[ 0 ];
        //        engine.Server.GetGameFileDataCompleted -= new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServer.GetGameFileDataEventArgs>( Server_GetGameFileDataCompleted );
        //        System.IO.FileStream fs = System.IO.File.Open( terr.TerrainFile.GetFullFilename(), System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None );
        //        fs.Write( e.data, 0, e.data.Length );
        //        fs.Close();

        //        outputBox.AddLine( "TerrainInfo file received. Succesfully saved to disk." );

        //        //Load Terrain
        //        terr.LoadTerrainInfo();

        //        terr.Initialize();

        //        terr.LoadGraphicsContent();








        //    }


        //}

        public void StopGameMode()
        {

        }

        public void OnCompleted( object sender, EventArgs e )
        {
            StopGameMode();
            if ( Completed != null ) Completed( sender, e );
        }

        public event EventHandler Completed;

        #endregion



    }
}

