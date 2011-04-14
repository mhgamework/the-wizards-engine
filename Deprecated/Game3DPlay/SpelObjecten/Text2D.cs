using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.Game3DPlay.SpelObjecten
{
	public class Text2D : SpelObject, Core.Elements.IRenderable2D, Core.Elements.ILoadable
	{
		public Text2D(SpelObject nParent)
			: base(nParent)
		{
			_render2DElement = GetElement<Core.Elements.Render2DElement>();
			_text = "Geen text";
			_textColor = Color.DarkRed;
		}

		private string _text;
		public string Text
		{
			get { return _text; }
			set { _text = value; if (TextChanged != null) TextChanged(null, null); }
		}

		public delegate void TextChangedHandler(object sender, EventArgs e);
		public event TextChangedHandler TextChanged;

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

		private Microsoft.Xna.Framework.Graphics.Color _textColor;
		public Microsoft.Xna.Framework.Graphics.Color TextColor
		{
			get { return _textColor; }
			set { _textColor = value; }
		}

		private SpriteFont _font;
		public SpriteFont Font
		{
			get { return _font; }
			private set { _font = value; }
		}

		private string _fontFilename;

		public string FontFilename
		{
			get { return _fontFilename; }
			set { _fontFilename = value; }
		}


		public event EventHandler FontLoaded;




		#region IRenderable Members

		public void OnBeforeRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
		{

		}

		public void OnRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
		{

            e.SpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );
			
			e.SpriteBatch.DrawString(Font, Text, Positie, TextColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			e.SpriteBatch.End();
		}

		public void OnAfterRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
		{

		}

		#endregion

		#region ILoadable Members

		public void OnLoad(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			if (e.AllContent)
			{
				Font = e.ContentManager.Load<SpriteFont>(FontFilename);
				if (FontLoaded != null) FontLoaded(this, null);
			}
		}

		public void OnUnload(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{

		}

		#endregion
	}
}
