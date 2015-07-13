using MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO;

namespace MHGameWork.TheWizards.Graphics.Xna.Collada
{
    public class ColladaTexture
    {
        ColladaModel model;
        //public IXNAGame game;

        public string Name;
        public string Filename;

        //public Microsoft.Xna.Framework.Graphics.Texture2D Texture;


        public ColladaTexture( ColladaModel nModel )
        {
            model = nModel;
            //engine = nModel.Game;
        }


        public string GetFullFilename()
        {
            string path = Filename;
            path = path.Replace( "%20", " " );
            path = path.Replace( "%28", "(" );
            path = path.Replace( "%29", ")" );
            path = path.Replace( "%EF", "ï" );

            if ( Filename.StartsWith( "file:///" ) )
            {
                path = path.Substring( ( "file:///" ).Length );

            }
            else if ( model.File != null )
            {
                path = Filename;
                path = StringHelper.GetDirectory( model.File.GetFullFilename() ) + "\\" + path;
                //TODO: path = game.Content.RootDirectory + "\\Content\\" + path;
            }

            return path;
        }

        public override string ToString()
        {
            return "ColladaTexture: " + Name;
        }


        //public void LoadTexture()
        //{
        //    TextureCreationParameters parameters = new TextureCreationParameters(
        //        0, 0, 0, 0, SurfaceFormat.Unknown, TextureUsage.None, Color.White, FilterOptions.Triangle, FilterOptions.Linear );
        //    //TextureCreationParameters parameters = new TextureCreationParameters(
        //    //    0, 0, 0, 1, SurfaceFormat.Unknown, TextureUsage.None, Color.White, FilterOptions.Triangle, FilterOptions.Linear );

        //    try
        //    {
        //        if ( Filename.StartsWith( "file:///" ) )
        //        {
        //            string path;
        //            path = Filename.Substring( ( "file:///" ).Length );
        //            path = path.Replace( "%20", " " );
        //            path = path.Replace( "%28", "(" );
        //            path = path.Replace( "%29", ")" );
        //            Texture = Texture2D.FromFile( game.GraphicsDevice, path, parameters );

        //        }
        //        else
        //        {
        //            string path;
        //            path = Filename;
        //            path = path.Replace( "%20", " " );
        //            Texture = Texture2D.FromFile( game.GraphicsDevice, game.Content.RootDirectory + "\\Content\\" + path, parameters );
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    finally
        //    {
        //    }

        //    //Texture.Save( "c:/temp.dds", ImageFileFormat.Dds );
        //    //Texture.GenerateMipMaps( TextureFilter.Linear );
        //}

    }
}
