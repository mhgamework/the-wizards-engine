using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    /// <summary>
    /// - Hold al textures used in the game, defined by an ID that is referenced in models, shaders, terrain etc
    /// - Default state for textureData is not loaded. The textureData should be loaded when there is an instance of Texture referencing it.
    /// </summary>
    public class TextureManager
    {
        private ServerClientMainOud engine;

        public ServerClientMainOud Engine
        {
            get { return engine; }
            set { engine = value; }
        }
        private List<TextureData> textures;
        private bool bufferedLoading;
        private bool bufferedUnloading;

        public bool BufferedUnloading
        {
            get { return bufferedUnloading; }
            set { bufferedUnloading = value; }
        }

        public bool BufferedLoading
        {
            get { return bufferedLoading; }
            set { bufferedLoading = value; }
        }

        private int lastID;


        public TextureManager( ServerClientMainOud nEngine )
        {
            engine = nEngine;

            textures = new List<TextureData>();

        }

        public Texture FindTextureByAssetname( string assetname )
        {
            for ( int i = 0; i < textures.Count; i++ )
            {
                if ( textures[ i ].AssetName == assetname )
                {
                    return Texture.FromTextureData( engine, textures[ i ] );
                }
            }

            return null;
        }

        public Texture GetTexture( int nID )
        {
            //TODO: optimization needed

            for ( int i = 0; i < textures.Count; i++ )
            {
                if ( textures[ i ].ID == nID )
                {
                    return textures[ i ].CreateReferenceTexture();
                }
            }

            return null;
        }

        public TextureData AddTexture( int nID, GameFileOud nFile )
        {
            if ( nID > lastID ) lastID = nID;
            TextureData data = new TextureData( this, nID, nFile );

            textures.Add( data );

            return data;
        }
        /// <summary>
        /// TODO: define this as an debug texture
        /// </summary>
        public Texture CreateNewDebugTexture( GameFileOud file )
        {
            TextureData data = AddTexture( lastID + 1, file );
            return data.CreateReferenceTexture();
        }

        ///// <summary>
        ///// DEPRECATED
        ///// </summary>
        ///// <param name="nTexture"></param>
        ///// <param name="assetName"></param>
        ///// <returns></returns>
        //public TextureData GetTextureData( Texture nTexture, string assetName )
        //{
        //    throw new Exception( "DEPRECATED" );
        //    //TextureData textureData = null;

        //    ////Loop kan misschien sneller? met een dictionary?
        //    //for ( int i = 0; i < textures.Count; i++ )
        //    //{
        //    //    if ( textures[ i ].AssetName == assetName )
        //    //    {
        //    //        textureData = textures[ i ];
        //    //        break;
        //    //    }

        //    //}

        //    //if ( textureData == null )
        //    //{
        //    //    //textureData = new TextureData( -1, assetName );
        //    //    textures.Add( textureData );

        //    //    //if ( autoLoad ) textureData.Load( engine );
        //    //}



        //    ////textureData.AddReference( nTexture );

        //    //return textureData;
        //}
        ///// <summary>
        ///// DEPRECATED
        ///// </summary>
        ///// <returns></returns>
        //public void ReleaseTextureData( Texture nTexture, TextureData nTextureData )
        //{
        //    throw new Exception( "DEPRECATED" );
        //    //nTextureData.RemoveReference( nTexture );
        //    //            if ( nTextureData.ReferenceCount == 0 )
        //    //            {
        //    //                textures.Remove( nTextureData );
        //    //                nTextureData.Dispose();

        //    //            }

        //    //#if DEBUG
        //    //            if ( nTextureData.ReferenceCount < 0 )
        //    //            {
        //    //                throw new Exception( "Internal error! reference count kan niet kleiner zijn dan 0" );
        //    //            }
        //    //#endif

        //}


        /// <summary>
        /// Loads all buffered, referenced textures
        /// </summary>
        public virtual void Load()
        {

            for ( int i = 0; i < textures.Count; i++ )
            {

                // Load all textures currently in use.
                if ( textures[ i ].ReferenceCount > 0 )
                {
                    textures[ i ].Load();
                }
            }


            //autoLoad = true;

        }

        /// <summary>
        /// Unloads all buffered non-referenced textures
        /// </summary>
        public virtual void Unload()
        {
            // Unload all textures.
            for ( int i = 0; i < textures.Count; i++ )
            {
                if ( textures[ i ].ReferenceCount == 0 )
                {
                    textures[ i ].Unload();
                }
            }

        }

        /// <summary>
        /// Unloads all textures.
        /// </summary>
        public virtual void ForceUnload()
        {
            // Unload all textures.
            for ( int i = 0; i < textures.Count; i++ )
            {
                textures[ i ].Unload();
            }

        }

        public void OnReferenceCountChanged( TextureData data )
        {
            if ( data.ReferenceCount == 0 )
            {
                if ( !bufferedUnloading ) data.Unload();
            }
            else
            {
                if ( !bufferedLoading ) data.Load();
            }
        }



        public static void TestTextureManager001()
        {

            TestServerClientMain main = null;

            TestServerClientMain.Start( "TestTextureManager001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    GameFileOud file1 = new GameFileOud( main.GameFileManager, 1, @"\Content\Grass001.dds" );
                    GameFileOud file2 = new GameFileOud( main.GameFileManager, 2, @"\Content\Cursor001.dds" );


                    main.TextureManager.AddTexture( 1, file1 );
                    main.TextureManager.AddTexture( 2, file2 );


                    Texture tex1 = main.TextureManager.GetTexture( 1 );

                    main.TextureManager.BufferedLoading = true;
                    main.TextureManager.BufferedUnloading = true;
                    Texture tex2 = main.TextureManager.GetTexture( 2 );

                    tex1.Dispose();
                    tex1 = null;

                    main.TextureManager.Load();
                    main.TextureManager.Unload();


                    main.TextureManager.BufferedLoading = false;
                    main.TextureManager.BufferedUnloading = false;

                    tex2.Dispose();
                    tex2 = null;

                    main.TextureManager.ForceUnload();



                }
                , delegate
                {
                } );
        }


    }


}
