using System;

//From benjamin nitschkis book engine
using Microsoft.Xna.Framework.Graphics;


namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO
{
	public class TextureBookengine :IDisposable
	{
		private IXNAGame engine;

		public TextureBookengine(IXNAGame _game, string nAssetName)
		{
			engine = _game;


		}

		public TextureBookengine(IXNAGame _game, Texture2D xnaTexture)
		{
		}

		public Texture2D XnaTexture
		{ get { return null; } }

		public bool HasAlphaPixels
		{
			get
			{
				//TODO
				return true;
			}
		}



		public void Dispose()
		{
			throw new Exception( "The method or operation is not implemented." );
		}


	}
}
