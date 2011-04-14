using System;
using System.Collections.Generic;
using System.Text;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class Texture : IDisposable
    {
        private ServerClientMainOud engine;
        private TextureData textureData;


        private Texture( ServerClientMainOud nEngine, TextureData data )
        {
            engine = nEngine;
            textureData = data;
        }

        /// <summary>
        /// DEPRECATED: DO NOT USE !!!
        /// </summary>
        /// <param name="nEngine"></param>
        /// <param name="nAssetName"></param>
        public Texture( ServerClientMainOud nEngine, string nAssetName )
        {
            throw new Exception( "DEPRECATED" );
            //engine = nEngine;
            //textureData = engine.TextureManager.GetTextureData( this, nAssetName );

        }
        ~Texture()
        {
            Dispose();
        }


        public void Dispose()
        {
            if ( textureData != null )
                textureData.RemoveReferenceTexture( this );
            //engine.TextureManager.ReleaseTextureData( this, textureData );
            textureData = null;
        }

        public static Texture FromFile( ServerClientMainOud engine,string filename)
        {
            Texture tex;
            tex = engine.TextureManager.FindTextureByAssetname( filename );
            if ( tex != null ) return tex;

            GameFileOud file = new GameFileOud( filename );

            return engine.TextureManager.CreateNewDebugTexture( file );

        }

        /// <summary>
        /// USED BY ENGINE ONLY!!
        /// </summary>
        /// <param name="nEngine"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Texture FromTextureData( ServerClientMainOud nEngine, TextureData data )
        {
            Texture tex = new Texture( nEngine, data );
            return tex;
        }


        public XnaTexture XNATexture
        { get { return textureData.XNATexture; } }

        public int ID { get { return textureData.ID; } }

    }
}
