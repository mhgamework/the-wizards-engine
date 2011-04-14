using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture2D;


namespace MHGameWork.TheWizards.ServerClient
{
    public class TWTexture
    {
        private XnaTexture xnaTexture;

        public XnaTexture XnaTexture
        {
            get
            {
                return xnaTexture;
            }
        }



        public TWTexture()
        {
        }

        public static TWTexture FromImageFile( IXNAGame game, IGameFile imageFile, TextureCreationParameters parameters )
        {
            TWTexture tex = new TWTexture();
            try
            {
                tex.xnaTexture = Texture2D.FromFile( game.GraphicsDevice, imageFile.GetFullFilename(), parameters );
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine( "Couldn't load texture at path '" + imageFile.GetFullFilename() + "' in TWTexture." );
                tex.xnaTexture = null;
            }
            return tex;
        }

        public static TWTexture FromImageFile( IXNAGame game, IGameFile imageFile )
        {
            TextureCreationParameters parameters = new TextureCreationParameters( 0, 0, 0, 0, SurfaceFormat.Unknown, TextureUsage.None, Color.TransparentBlack, FilterOptions.Linear, FilterOptions.Box );

            return FromImageFile( game, imageFile, parameters );
        }
    }
}
