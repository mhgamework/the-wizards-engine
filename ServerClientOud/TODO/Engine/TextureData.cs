using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture2D;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class TextureData : IDisposable
    {
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private GameFileOud file;
        private int _referenceCount;
        private bool loaded;

        private TextureManager manager;
        public ServerClientMainOud Engine
        {
            get { return manager.Engine; }
        }

        private XnaTexture xnaTexture;

        public TextureData( TextureManager nManager, int nID, GameFileOud nFile )
        {
            manager = nManager;
            id = nID;
            file = nFile;
            loaded = false;
        }

        ~TextureData()
        {
            Dispose();
        }

        public void Dispose()
        {
            Unload();
        }


        /// <summary>
        /// Loads the texture into memory if it is not yet loaded
        /// </summary>
        public void Load()
        {
            if ( loaded == true ) return;

            xnaTexture = Texture2D.FromFile( Engine.XNAGame.GraphicsDevice, AssetName );
            loaded = true;
        }


        /// <summary>
        /// Forces the texture to reload itself into memory
        /// </summary>
        public void Reload()
        {
            Unload();

            Load();
        }

        public void Unload()
        {
            if ( xnaTexture != null )
            {
                xnaTexture.Dispose();
            }
            xnaTexture = null;
            loaded = false;
        }

        public override string ToString()
        {
            return "TextureData - ID: " + id.ToString() + " , Asset: " + AssetName + " " + ( loaded ? "Loaded" : "Not Loaded" );
        }

        public Texture CreateReferenceTexture()
        {
            Texture tex = Texture.FromTextureData( Engine, this );
            ReferenceCount++;

            return tex;
        }

        public void RemoveReferenceTexture( Texture nTexture )
        {
            ReferenceCount--;
        }


        public string AssetName { get { return file.GetFullFilename(); } }
        public bool Loaded { get { return loaded; } }
        public XnaTexture XNATexture { get { return xnaTexture; } }
        public int ReferenceCount
        {
            get { return _referenceCount; }
            private set
            {
                _referenceCount = value;
                manager.OnReferenceCountChanged( this );

            }
        }


    }
}
