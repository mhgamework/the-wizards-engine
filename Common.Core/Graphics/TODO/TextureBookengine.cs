using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture2D;

//From benjamin nitschkis book engine


namespace MHGameWork.TheWizards.ServerClient.Engine
{
	public class TextureBookengine :IDisposable
	{
		private IXNAGame engine;

		public TextureBookengine(IXNAGame _game, string nAssetName)
		{
			engine = _game;


		}

		public TextureBookengine(IXNAGame _game, XnaTexture xnaTexture)
		{
		}

		public XnaTexture XnaTexture
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
