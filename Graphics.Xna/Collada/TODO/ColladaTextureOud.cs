using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada.TODO
{
    public class ColladaTextureOud
    {
        public IXNAGame game;

        public string Name;
        public string Filename;

        public Microsoft.Xna.Framework.Graphics.Texture2D Texture;


        public ColladaTextureOud( ColladaModelOud nModel )
        {
            game = nModel.Game;
        }


        public void LoadTexture()
        {
            TextureCreationParameters parameters = new TextureCreationParameters(
                0, 0, 0, 0, SurfaceFormat.Unknown, TextureUsage.None, Color.White, FilterOptions.Triangle, FilterOptions.Linear );
            //TextureCreationParameters parameters = new TextureCreationParameters(
            //    0, 0, 0, 1, SurfaceFormat.Unknown, TextureUsage.None, Color.White, FilterOptions.Triangle, FilterOptions.Linear );

            try
            {
                if ( Filename.StartsWith( "file:///" ) )
                {
                    string path;
                    path = Filename.Substring( ( "file:///" ).Length );
                    path = path.Replace( "%20", " " );
                    path = path.Replace( "%28", "(" );
                    path = path.Replace( "%29", ")" );
                    Texture = Texture2D.FromFile( game.GraphicsDevice, path, parameters );

                }
                else
                {
                    string path;
                    path = Filename;
                    path = path.Replace( "%20", " " );
                    Texture = Texture2D.FromFile( game.GraphicsDevice, game.EngineFiles.RootDirectory + "\\Content\\" + path, parameters );
                }
            }
            catch
            {

            }
            finally
            {
            }

            //Texture.Save( "c:/temp.dds", ImageFileFormat.Dds );
            //Texture.GenerateMipMaps( TextureFilter.Linear );
        }

    }
}
