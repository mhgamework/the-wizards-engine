using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using Microsoft.Xna.Framework;

namespace MHGameWork.Game3DPlay.Gui
{
	public class PanelPart : SpelObject
	{

		protected PanelPart(SpelObject nParent)
			: base(nParent)
		{
			Panel = null;
			Size = new Vector2(100, 100);
			base.ParentChanged += new ValueChangedEventHandler<ISpelObject>(PanelPart_ParentChanged);
			PanelPart_ParentChanged(this, new ValueChangedEventArgs<ISpelObject>(null, nParent));

		}
		public PanelPart(Panel nParent)
			: base(nParent)
		{
			Panel = nParent;
			base.ParentChanged += new ValueChangedEventHandler<ISpelObject>(PanelPart_ParentChanged);
			PanelPart_ParentChanged(this, new ValueChangedEventArgs<ISpelObject>(null, nParent));
		}

		void PanelPart_ParentChanged(object sender, ValueChangedEventArgs<ISpelObject> e)
		{
			if (e.OldValue != null && e.OldValue is PanelPart) ((PanelPart)e.OldValue).SizeChanged -= new EventHandler<SizeChangedEventArgs>(PanelPart_SizeChanged);
			if (e.NewValue != null && e.NewValue is PanelPart) ((PanelPart)e.NewValue).SizeChanged += new EventHandler<SizeChangedEventArgs>(PanelPart_SizeChanged);
		}

		//Protected WithEvents Render2DElement As New Render2DElement(Me)


		private Panel _panel;
		public Panel Panel
		{
			get { return _panel; }
			private set { _panel = value; }
		}

		private Vector2 _positie;
		public Vector2 Positie
		{
			get { return _positie; }
			set
			{
				if (_positie == value) return;
				PositieChangedEventArgs e = new PositieChangedEventArgs(_positie,value);
				_positie = value;
				OnPositieChanged(this, e);
			}
		}

		private Vector2 _minSize;
		public Vector2 MinSize
		{
			get { return _minSize; }
			set { _minSize = value; }
		}

		private Vector2 _size;
		public Vector2 Size
		{
			get
			{
				if (_size.X < MinSize.X || _size.Y < MinSize.Y)
				{
					Vector2 ret = _size;
					if (_size.X < MinSize.X) ret.X = MinSize.X;
					if (_size.Y < MinSize.Y) ret.Y = MinSize.Y;
					return ret;
				}
				else
				{
					return _size;
				}
			}
			set
			{
				// 'If Not value.X > 0 Or Not value.Y > 0 Then Throw New Exception("De size moet groter zijn dan 0")
				if (_size == value) return;
				SizeChangedEventArgs e = new SizeChangedEventArgs(_size, value);
				_size = value;
				OnSizeChanged(this, e);


			}
		}

		public Vector2 ParentSize
		{
			get
			{
				if (Panel != null) { return Panel.Size; }
				else
				{

					return new Vector2(HoofdObj.XNAGame.Graphics.GraphicsDevice.DisplayMode.Width,
						HoofdObj.XNAGame.Graphics.GraphicsDevice.DisplayMode.Height);
				}
			}
		}

		public class SizeChangedEventArgs : EventArgs
		{
			public SizeChangedEventArgs(Vector2 nOldSize, Vector2 nNewSize)
				: base()
			{
				OldSize = nOldSize;
				NewSize = nNewSize;


			}

			private Vector2 _oldSize;

			public Vector2 OldSize
			{
				get { return _oldSize; }
				private set { _oldSize = value; }
			}
			private Vector2 _newSize;

			public Vector2 NewSize
			{
				get { return _newSize; }
				private set { _newSize = value; }
			}




		}
		public void OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (SizeChanged != null) SizeChanged(sender, e);

			UpdateAlign();
		}


		public event EventHandler<SizeChangedEventArgs> SizeChanged;
		//public delegate void SizeChangedEventHandler(object sender, SizeChangedEventArgs e);


		public class PositieChangedEventArgs : EventArgs
		{
			public PositieChangedEventArgs(Vector2 nOldPositie, Vector2 nNewPositie)
				: base()
			{
				OldPositie = nOldPositie;
				NewPositie = nNewPositie;


			}

			private Vector2 _oldPositie;

			public Vector2 OldPositie
			{
				get { return _oldPositie; }
				private set { _oldPositie = value; }
			}
			private Vector2 _newPositie;

			public Vector2 NewPositie
			{
				get { return _newPositie; }
				private set { _newPositie = value; }
			}




		}
		public void OnPositieChanged(object sender, PositieChangedEventArgs e)
		{
			if (PositieChanged != null) PositieChanged(sender, e);
		}

		public event EventHandler<PositieChangedEventArgs> PositieChanged;
		//public delegate void PositieChangedEventHandler(object sender, PositieChangedEventArgs e);


		public enum AlignType
		{
			None = 0,
			TopLeft,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight

		}

		private AlignType _align;
		public AlignType Align
		{
			get { return _align; }
			set { _align = value; UpdateAlign(); }
		}




		[Flags]
		public enum AnchorType
		{
			None = 0,
			Top = 1,
			Right = 2,
			Bottom = 4,
			Left = 8

		}

		private AnchorType _anchor;
		/// <summary>
		/// Combinaties mogelijk
		/// </summary>
		public AnchorType Anchor
		{
			get { return _anchor; }
			set { _anchor = value; }
		}

		/*
	'Public Enum DockType
	'    None = 1
	'    Fill
	'    Top
	'    Right
	'    Bottom
	'    Left

	'End Enum


	'Private mDock As DockType
	'Public Property Dock() As DockType
	'    <System.Diagnostics.DebuggerStepThrough()> Get
	'        Return mDock
	'    End Get
	'    <System.Diagnostics.DebuggerStepThrough()> Set(ByVal value As DockType)
	'        mDock = value
	'    End Set
	'End Property
	 */



		public float Left
		{
			get { return Positie.X; }
			set
			{
				// 'If CInt(Anchor And AnchorType.Left) > 0 Then Exit Property
				// 'If Not value < Right Then Throw New Exception("Left moet kleiner zijn dan right")
				Size = new Vector2(Size.X - Positie.X + value, Size.Y);
				Positie = new Vector2(value, Positie.Y);
			}
		}
		public float Right
		{
			get { return Positie.X + Size.X; }
			set
			{
				// 'If CInt(Anchor And AnchorType.Right) > 0 Then Exit Property
				// 'If Not value > Left Then Throw New Exception("Right moet groter zijn dan left")
				Size = new Vector2(value - Positie.X, Size.Y);
				Positie = new Vector2(value - Size.X, Positie.Y);
			}
		}
		public float Top
		{
			get { return Positie.Y; }
			set
			{
				// 'If CInt(Anchor And AnchorType.Top) > 0 Then Exit Property
				// 'If Not value < Bottom Then Throw New Exception("Top moet kleiner zijn dan bottom")
				Size = new Vector2(Size.X, Size.Y - Positie.Y + value);
				Positie = new Vector2(Positie.X, value);
			}
		}
		public float Bottom
		{
			get { return Positie.Y + Size.Y; }
			set
			{
				// 'If CInt(Anchor And AnchorType.Bottom) > 0 Then Exit Property
				// 'If Not value > Top Then Throw New Exception("Bottom moet groter zijn dan top")
				Size = new Vector2(Size.X, value - Size.Y);
				Positie = new Vector2(Positie.X, Positie.Y + Size.Y);
			}
		}


		private bool _hasFocus;
		public bool HasFocus
		{
			get { return _hasFocus; }
			set
			{
				if (_hasFocus == value) return;
				_hasFocus = value;
				if (value)
				{
					OnFocused(this, null);
				}
				else
				{
					OnFocusLost(this, null);
				}
			}
		}


		public void OnFocused(object sender, EventArgs e)
		{
			Focused(sender, e);
		}
		public event FocusedEventHandler Focused;
		public delegate void FocusedEventHandler(object sender, EventArgs e);

		public void OnFocusLost(object sender, EventArgs e)
		{
			FocusLost(sender, e);
		}

		public event FocusLostEventHandler FocusLost;
		public delegate void FocusLostEventHandler(object sender, EventArgs e);




		public virtual void Focus()
		{
			Panel.FocusPart(this);
		}



		protected virtual void OnRender2D(object sender, Core.Elements.RenderEventArgs e) //Handles Render2DElement.Render2D
		{
		}


		public void UpdateAlign()
		{
			Vector2 pos = Positie;
			//Top: do nothing

			//Middle:
			if (Align == AlignType.MiddleLeft ||
				Align == AlignType.MiddleRight ||
				Align == AlignType.MiddleCenter)
			{
				pos.Y = ParentSize.Y / 2 - Size.Y / 2;

			}

			//Bottom:
			if (Align == AlignType.BottomLeft ||
				Align == AlignType.BottomCenter ||
				Align == AlignType.BottomRight)
			{
				pos.Y = ParentSize.Y - Size.Y;

			}


			//Left: do nothing

			//Center:
			if (Align == AlignType.TopCenter ||
				Align == AlignType.MiddleCenter ||
				Align == AlignType.BottomCenter)
			{
				pos.X = ParentSize.X / 2 - Size.X / 2;
			}


			//Right
			if (Align == AlignType.TopRight ||
				Align == AlignType.MiddleRight ||
				Align == AlignType.BottomRight)
			{
				pos.X = ParentSize.X - Size.X;
			}
			Positie = pos;
		}


		protected virtual void PanelPart_SizeChanged(object sender, PanelPart.SizeChangedEventArgs e)
		{
			/*
			'If Dock <> DockType.None Then 'Dock als primaire, ignore ancor
			'    Select Case Dock
			'        Case DockType.Fill
			'            Positie = New Vector2(0, 0)
			'            Size = e.NewSize

			'        Case DockType.Top
			'            Positie = New Vector2(0, 0)

			'            Top = 0
			'            Left = 0
			'            Right = 0
			'            Bottom = Bottom / e.OldSize.Y * e.NewSize.Y



			'    End Select




			'Else*/
			if (Align != AlignType.None) //align
			{
				UpdateAlign();
			}
			else
			{
				//Use the anchor point

				float newTop = Top;
				float newRight = Right;
				float newBottom = Bottom;
				float newLeft = Left;

				Vector2 verschil = e.NewSize - e.OldSize;

				//horizontaal

				if ((Anchor & AnchorType.Left) == AnchorType.Left && (Anchor & AnchorType.Right) == AnchorType.Right)
				{
					//horizontaal vast ==> stretchen

					//Overbodig: newleft = newleft
					newRight = Right + verschil.X;//e.NewSize.X - (e.OldSize.X - Right);
				}
				else if ((Anchor & AnchorType.Left) == AnchorType.Left)
				{
					//alleen left: laten staan
				}
				else if ((Anchor & AnchorType.Right) == AnchorType.Right)
				{
					//alleen right: rechts anchoren
					newRight = Right + verschil.X;//e.NewSize.X - (e.OldSize.X - Right);
					newLeft = Left + verschil.X;
				}
				else
				{
					//horizontaal zwevend  ==> horizontaal verplaatsen volgens vergrootting

					newLeft += verschil.X / 2;
					newRight += verschil.X / 2;



				}


				//Verticaal

				if ((Anchor & AnchorType.Top) == AnchorType.Top && (Anchor & AnchorType.Bottom) == AnchorType.Bottom)
				{
					//Verticaal vast ==> stretchen

					//Overbodig: newTop = newTop
					newBottom = Bottom + verschil.Y;// e.NewSize.Y - (e.OldSize.Y - Bottom);
				}
				else if ((Anchor & AnchorType.Top) == AnchorType.Top)
				{
					//alleen Top: laten staan
				}
				else if ((Anchor & AnchorType.Bottom) == AnchorType.Bottom)
				{
					//alleen Bottom: rechts anchoren
					newBottom = Bottom + verschil.Y; //e.NewSize.Y - (e.OldSize.Y - Bottom);
					newTop = Top + verschil.Y; //newBottom - Size.Y;
				}
				else
				{
					//Verticaal zwevend  ==> Verticaal verplaatsen volgens vergrootting

					newTop += verschil.Y / 2;
					newBottom += verschil.Y / 2;



				}



				/*
					'' ''If CInt(Anchor And AnchorType.Top) = 0 Then
					'' ''    If (e.OldSize.Y - Size.Y) = 0 Then
					'' ''        newTop = 0
					'' ''    Else
					'' ''        newTop = Top / (e.OldSize.Y - Size.Y) * (e.NewSize.Y - Size.Y)
					'' ''    End If

					'' ''    'Else
					'' ''    'niks doen
					'' ''End If
					'' ''If CInt(Anchor And AnchorType.Left) = 0 Then
					'' ''    If (e.OldSize.X - Size.X) = 0 Then
					'' ''        newLeft = 0
					'' ''    Else
					'' ''        newLeft = Left / (e.OldSize.X - Size.X) * (e.NewSize.X - Size.X)
					'' ''    End If
					'' ''    'Else
					'' ''    'niks doen
					'' ''End If
					' ''If CInt(Anchor And AnchorType.Right) = 0 Then
					' ''    'If (e.OldSize.X - Size.X) = 0 Then
					' ''    '    newRight = e.NewSize.X
					' ''    'Else
					' ''    '    newRight = Right / (e.OldSize.X - Size.X) * (e.NewSize.X - Size.X)
					' ''    'End If

					' ''Else
					' ''    newRight = e.NewSize.X - (e.OldSize.X - Right)
					' ''End If
					' ''If CInt(Anchor And AnchorType.Bottom) = 0 Then
					' ''    'If (e.OldSize.Y - Size.Y) = 0 Then
					' ''    '    newBottom = e.NewSize.Y
					' ''    'Else
					' ''    '    newBottom = Bottom / (e.OldSize.Y - Size.Y) * (e.NewSize.Y - Size.Y)
					' ''    'End If
					' ''Else
					' ''    newBottom = e.NewSize.Y - (e.OldSize.Y - Bottom)
					' ''End If



					'' ''If CInt(Anchor And AnchorType.Top) = 0 And CInt(Anchor And AnchorType.Bottom) = 0 Then
					'' ''    newTop += Verschil.Y / 2
					'' ''    newBottom += Verschil.Y / 2
					'' ''    'Pos.Y = Pos.Y / e.OldSize.Y * e.NewSize.Y
					' ''If CInt(Anchor And AnchorType.Top) = 0 Then
					' ''    newTop += Verschil.Y / 2

					' ''End If
					' ''If CInt(Anchor And AnchorType.Bottom) = 0 Then
					' ''    newBottom += Verschil.Y / 2

					' ''End If

					'' ''If CInt(Anchor And AnchorType.Right) = 0 And CInt(Anchor And AnchorType.Left) = 0 Then
					'' ''    newLeft += Verschil.X / 2
					'' ''    newRight += Verschil.X / 2
					'' ''    'Pos.X = Pos.X / e.OldSize.X * e.NewSize.X
					' ''If CInt(Anchor And AnchorType.Right) = 0 Then

					' ''    newRight += Verschil.X / 2

					' ''End If
					' ''If CInt(Anchor And AnchorType.Left) = 0 Then
					' ''    newLeft += Verschil.X / 2
					' ''End If
				*/






				Positie = new Vector2(newLeft, newTop);
				Size = new Vector2(newRight - newLeft, newBottom - newTop);









			}


			/*public virtual void OnClicked()//(ByVal sender As Object, ByVal e As ClickedElement.ClickedEventArgs)
			{
					RaiseEvent Clicked(sender, e)
					}

			//Public Event Clicked(ByVal sender As Object, ByVal e As ClickedElement.ClickedEventArgs)*/

		}
	}
}
