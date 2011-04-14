using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.Game3DPlay.Core.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.Game3DPlay.SpelObjecten
{
	public class Image2D : SpelObject, IRenderable2D, ILoadable
	{
		public Image2D(SpelObject nParent)
			: base(nParent)
		{
			_render2DElement = GetElement<Core.Elements.Render2DElement>();
		}


		private Core.Elements.Render2DElement _render2DElement;
		public Core.Elements.Render2DElement Render2DElement
		{
			get { return _render2DElement; }
		}


		public Vector2 Positie
		{
			get { return _render2DElement.Positie; }
			set { _render2DElement.Positie = value; }
		}

		private Vector2 _size;
		public Vector2 Size
		{
			get { return _size; }
			set { _size = value; }
		}


		private string _filename;
		public string Filename
		{
			get { return _filename; }
			set
			{
				_filename = value;
			}
		}

		private Rectangle _sourceRectangle;

		public Rectangle SourceRectangle
		{
			get { return _sourceRectangle; }
			set { _sourceRectangle = value; }
		}

		private bool _useSourceRectangle;

		public bool UseSourceRectangle
		{
			get { return _useSourceRectangle; }
			set { _useSourceRectangle = value; }
		}



		private Texture2D _texture;
		public Texture2D Texture
		{
			get { return _texture; }
			private set { _texture = value; }
		}

		protected void LoadTexture(object sender, LoadEventArgs e)
		{
			DisposeTexture(sender, e);
			Texture = e.ContentManager.Load<Texture2D>(Filename);
		}

		protected void DisposeTexture(object sender, LoadEventArgs e)
		{
			if (Texture != null)
			{
				e.ContentManager.Unload(Texture);
				Texture = null;
			}
		}

		public override void Dispose()
		{
			base.Dispose();
		}



		#region IRenderable Members

		public void OnBeforeRender(object sender, RenderEventArgs e)
		{

		}

		public void OnRender(object sender, RenderEventArgs e)
		{
			e.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
			Rectangle destinationRectangle = new Rectangle((int)Positie.X, (int)Positie.Y, (int)Size.X, (int)Size.Y);
			if (UseSourceRectangle)
			{
				e.SpriteBatch.Draw(Texture, destinationRectangle, SourceRectangle, Color.White);
			}
			else
			{
				e.SpriteBatch.Draw(Texture, destinationRectangle, Color.White);
			}
			e.SpriteBatch.End();

		}

		public void OnAfterRender(object sender, RenderEventArgs e)
		{

		}

		#endregion

		#region ILoadable Members

		public void OnLoad(object sender, LoadEventArgs e)
		{
			if (e.AllContent)
			{
				LoadTexture(sender,e);
			}
		}

		public void OnUnload(object sender, LoadEventArgs e)
		{
			if (e.AllContent)
			{
				DisposeTexture(sender, e);
			}
		}

		#endregion
	}
}
