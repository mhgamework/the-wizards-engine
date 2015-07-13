using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.DEPRECATED
{
    public class Cursor : ICursor
    {
        private XNAGame game;

        public XNAGame Game
        {
            get { return game; }
            set { game = value; }
        }
	

        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector2 textureClickPoint;

        public Vector2 TextureClickPoint
        {
            get { return textureClickPoint; }
            set { textureClickPoint = value; }
        }

        private IGameFile textureFile;

        public IGameFile TextureFile
        {
            get { return textureFile; }
            set { textureFile = value; }
        }

        private Texture2D texture;

        public Cursor(XNAGame nGame, IGameFile nTextureFile, Vector2 nTextureClickPoint)
        {
            game = nGame;
            textureFile = nTextureFile;
            textureClickPoint = nTextureClickPoint;

        }

        public void Load()
        {
            texture = Texture2D.FromFile( game.GraphicsDevice, TextureFile.GetFullFilename() );
        }


        public void Update()
        {
            //throw new Exception( "The method or operation is not implemented." );
        }

        public void Render()
        {
            //throw new Exception( "The method or operation is not implemented." );
            game.SpriteBatch.Begin();

            game.SpriteBatch.Draw( texture, position - textureClickPoint, Color.White );
            game.SpriteBatch.End();
        }


    }
}
