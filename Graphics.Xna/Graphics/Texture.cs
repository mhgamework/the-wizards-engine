using System;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    /// <summary>
    /// DEPRECATED
    /// </summary>
    [Obsolete( "This class is now used, but is to be removed/changed in the future" )]
    public class TWTexture
    {
        private Texture2D xnaTexture;
        public Texture2D XnaTexture
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

        public static TWTexture FromTexture2D(Texture2D texture2D)
        {
            TWTexture tex = new TWTexture();
            tex.xnaTexture = texture2D;

            return tex;
        }
    }
}
