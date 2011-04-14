using System;
using System.Collections.Generic;
using System.Text;
//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.Game3DPlay.Core.Elements
{
	public class Renderer2DContainer : BaseRender2DElement, Core.IElementContainer
	{

		public Renderer2DContainer(SpelObject nParent)
			: base(nParent)
		{
			_elements = new List<BaseRender2DElement>();

			nParent.AddIElementContainer(this);


			BackgroundColor = Color.Yellow;
			DrawBackgroundColor = true;
			_size = new Vector2(200, 200);

			//AddEventHandler(new SpelEventHandler(Of Render2DEvent)(AddressOf CancelEventChildren(Of Render2DEvent)))
			//AddEventHandler(new SpelEventHandler(Of ClickedEvent)(AddressOf Clicked))

		}

		private List<BaseRender2DElement> _elements;
		public List<BaseRender2DElement> Elements
		{
			get { return _elements; }
			private set { _elements = value; }
		}


		public BaseRender2DElement GetElement(int index) { return _elements[index]; }



		public void AddElement(Element IE)
		{
			if (IE.Container != null) { throw new Exception(); }
			if (IE.AcceptContainer(this)) { _elements.Add((BaseRender2DElement)IE); }
		}
		#region IElementContainer Members

		public bool AcceptsElement(Element IE)
		{
			if (IE is BaseRender2DElement) { return true; } else { return false; }

		}

		public void RemoveElement(Element IE)
		{
			if (IE.Container != this) { throw new Exception("Not in this container"); }
			if (_elements.Remove((BaseRender2DElement)IE) == false) { throw new Exception("De Element denkt dat hij in deze container zit maar zit er niet in"); }
			IE.AcceptContainer(null);

		}

		public bool TryAdd(Element IE)
		{
			if (AcceptsElement(IE)) { AddElement(IE); return true; } else { return false; }

		}

		#endregion


		/*public override void OnRender( ByVal sender As Object, ByVal e As RenderElement.RenderEventArgs)
        {
            //mybase.OnRender(sender,e)
            With HoofdObj.DevContainer.DX.Device.RenderState
                if HoofdObj.DevContainer.DX.Caps.PrimitiveMiscCaps.SupportsSeparateAlphaBlend Then
                    .AlphaBlendEnable = True
                    .AlphaBlendOperation = BlendOperation.Max

                    .SeparateAlphaBlendEnabled = True
                Else
                    .AlphaBlendEnable = True
                    .SourceBlend = Blend.SourceAlpha
                    .DestinationBlend = Blend.InvSourceAlpha
                    .SeparateAlphaBlendEnabled = False
                End if
            End With
            With HoofdObj.DevContainer.DX
                //if HoofdObj.DevContainer.DX.Input.KeyPressed(DirectInput.Key.T) Then
                //HoofdObj.DevContainer.DX.SpriteRenderer2D.Render(Tex, Positie, new Vector2(SurfDesc.Width, SurfDesc.Height))
                //Else
                HoofdObj.DevContainer.DX.Sprite.Begin(SpriteFlags.None)
                HoofdObj.DevContainer.DX.Sprite.Draw2D(Tex, new Rectangle(0, 0, CInt(Size.X), CInt(Size.Y)), new SizeF(Size.X, Size.Y), PointF.Empty, 0, new PointF(Positie.X, Positie.Y), Color.White)
                HoofdObj.DevContainer.DX.Sprite.End()
                //End if

            End With

            //ev.FireToChildren = False
            }

        */
		public override void OnBeforeRender(object sender, RenderEventArgs e)
		{
			//base.OnBeforeRender(sender, e);
			CheckForRedraw(sender, e);
		}


		public void CheckForRedraw(object sender, RenderEventArgs e)
		{
			//if NeedsRedraw = False Then Exit void

			BaseRenderElement IE;
			for (int i = _elements.Count - 1; i >= 0; i--)
			{
				IE = _elements[i];
				if (IE.Enabled) IE.OnBeforeRender(sender, e);
			}

			//RTS.BeginScene(Surf)
			//With HoofdObj.DevContainer.DX
			RenderToTexture(sender, e);
			//End With


			//RTS.EndScene(Filter.None)


			for (int i = _elements.Count - 1; i >= 0; i--)
			{
				IE = _elements[i];
				if (IE.Enabled) IE.OnAfterRender(sender, e);
			}

			//OnRedrawed()
		}

		public override void OnRender(object sender, RenderEventArgs e)
		{
			base.OnRender(sender, e);
			e.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
			e.SpriteBatch.Draw(Texture, Positie, Color.White);
			e.SpriteBatch.End();
		}

		public override void OnAfterRender(object sender, RenderEventArgs e)
		{
			//base.OnAfterRender(sender, e);
		}


		protected virtual void RenderToTexture(object sender, RenderEventArgs e)
		{
			if (RenderTarget == null) throw new Exception();
			/*With HoofdObj.DevContainer.DX
				if DrawBackgroundColor Then
					.Device.RenderState.AlphaBlendEnable = False
					//.Device.Clear(ClearFlags.Target Or ClearFlags.ZBuffer, BackgroundColor, 1, 0)
					.Device.Clear(ClearFlags.Target, BackgroundColor, 1, 0)

				End if
				With HoofdObj.DevContainer.DX.Device.RenderState
					if HoofdObj.DevContainer.DX.Caps.PrimitiveMiscCaps.SupportsSeparateAlphaBlend Then
						.AlphaBlendEnable = True
						.SourceBlend = Blend.SourceAlpha
						.DestinationBlend = Blend.InvSourceAlpha
						.SeparateAlphaBlendEnabled = True
						.AlphaBlendOperation = BlendOperation.Max
					Else
						.AlphaBlendEnable = True
						.SeparateAlphaBlendEnabled = False
						.SourceBlend = Blend.SourceAlpha
						.DestinationBlend = Blend.InvSourceAlpha
					End if





				End With

				HoofdObj.DevContainer.DX.SpriteRenderer2D.Begin()

				if Background IsNot Nothing Then .SpriteRenderer2D.Render(Background.Texture, Vector2.Empty, Size)*/


			Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice.SetRenderTarget(0, RenderTarget);
			Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.AntiqueWhite, 1.0f, 0);
			if (DrawBackgroundColor)
			{
				Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice.Clear(
								ClearOptions.Target | ClearOptions.DepthBuffer, BackgroundColor, 1.0f, 0);
			}

			//using (SpriteBatch sprite = new SpriteBatch(Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice))
			//{
			Microsoft.Xna.Framework.Graphics.SpriteBatch oldBatch = e.SpriteBatch;
			e.SpriteBatch = SpriteBatch;
			//SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);


			BaseRenderElement IE;
			for (int i = _elements.Count - 1; i >= 0; i--)
			{
				IE = _elements[i];
				if (IE.Enabled) IE.OnRender(sender, e);
			}

			//SpriteBatch.End();

			//}

			e.SpriteBatch = oldBatch;






			//Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice .ResolveRenderTarget(0);
            
			Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice.SetRenderTarget(0, null);
			Texture = RenderTarget.GetTexture();

			/*HoofdObj.DevContainer.DX.SpriteRenderer2D.End()
		End With*/
		}




		//public virtual void Clicked(ByVal ev As ClickedEvent) Implements IClickable.Clicked
		//    if ev.Handled Then Exit void
		//    ev.FireToChildren = False
		//    Dim nE As ClickedEvent = new ClickedEvent(ev.Cursor, ev.PointClicked - Positie, ev.Button, ev.State)
		//    OnChildrenEvent(nE)
		//    ev.Handled = nE.Handled
		//End void

		//public override void SaveAllTextures()
		//    MyBase.SaveAllTextures()
		//    System.IO.Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath & "\temp\")
		//    Direct3D.TextureLoader.Save(System.Windows.Forms.Application.StartupPath & "\temp\" & GetType.ToString & "-" & CInt(Rnd() * 200000).ToString & ".png", ImageFileFormat.Png, Tex)
		//End void




		//Implements IClickable


		/*private mRTS As RenderToSurface
		public ReadOnly Property RTS() As RenderToSurface
			<System.Diagnostics.DebuggerNonUserCode()> Get
				Return mRTS
			End Get
		End Property
		private void setRTS(ByVal value As RenderToSurface)
			mRTS = value
		End void


		private mTex As Texture
		public ReadOnly Property Tex() As Texture
			<System.Diagnostics.DebuggerNonUserCode()> Get
				Return mTex
			End Get
		End Property
		private void setTex(ByVal value As Texture)
			mTex = value
		End void


		private mSurf As Surface
		public ReadOnly Property Surf() As Surface
			<System.Diagnostics.DebuggerNonUserCode()> Get
				Return mSurf
			End Get
		End Property
		private void setSurf(ByVal value As Surface)
			mSurf = value
		End void


		private mSurfDesc As SurfaceDescription
		public ReadOnly Property SurfDesc() As SurfaceDescription
			<System.Diagnostics.DebuggerNonUserCode()> Get
				Return mSurfDesc
			End Get
		End Property
		private void setSurfDesc(ByVal value As SurfaceDescription)
			mSurfDesc = value
		End void*/

		private SpriteBatch _spriteBatch;

		public SpriteBatch SpriteBatch
		{
			get { return _spriteBatch; }
			set { _spriteBatch = value; }
		}


		private RenderTarget2D _renderTarget;
		private RenderTarget2D RenderTarget
		{
			get { return _renderTarget; }
			set { _renderTarget = value; }
		}

		private Texture2D _texture;

		public Texture2D Texture
		{
			get { return _texture; }
			private set { _texture = value; }
		}




		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				//OnEvent(new VisualChangedEvent)
			}
		}

		private bool _drawBackgroundColor;
		public bool DrawBackgroundColor
		{
			get { return _drawBackgroundColor; }
			set
			{
				_drawBackgroundColor = value;
				//OnEvent(new VisualChangedEvent)
			}
		}

		//public Property Visible() As Boolean
		//    Get
		//        Return MyBase.Visible
		//    End Get
		//    Set(ByVal value As Boolean)
		//        MyBase.Visible = value
		//        if Visible Then
		//            RemoveHandler Render2D, AddressOf CancelEventChildren(Of Render2DEvent) //TODO: deze regel is om ervoor te zorgen dat er altijd maar zo een handler is
		//            AddHandler Render2D, AddressOf CancelEventChildren(Of Render2DEvent)
		//            //Dim H As new SpelEventHandler(Of Render2DEvent)(AddressOf CancelEventChildren(Of Render2DEvent))
		//            //RemoveEventHandler(H) //TODO: deze regel is om ervoor te zorgen dat er altijd maar zo een handler is
		//            //AddEventHandler(H)
		//        End if
		//    End Set
		//End Property

		/*private mBackground As AHTexture
		public ReadOnly Property Background() As AHTexture
			<System.Diagnostics.DebuggerNonUserCode()> Get
				Return mBackground
			End Get
		End Property
		protected void setBackground(ByVal value As AHTexture)
			mBackground = value
		End void



		private mBackgroundDescription As SurfaceDescription
		public ReadOnly Property BackgroundDescription() As SurfaceDescription
			<System.Diagnostics.DebuggerNonUserCode()> Get
				Return mBackgroundDescription
			End Get
		End Property
		private void setBackgroundDescription(ByVal value As SurfaceDescription)
			mBackgroundDescription = value
		End void*/



		private Vector2 _size;
		public Vector2 Size
		{
			get { return _size; }
			set
			{
				_size = value;
				CreateRenderTarget();// (HoofdObj.DevContainer.DX.Device);
				OnSizeChanged(this, null);
				//OnEvent(new VisualChangedEvent)
			}
		}


		protected virtual void OnSizeChanged(object sender, EventArgs e)
		{

		}


		public override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);
			if (loadAllContent)
			{
				CreateRenderTarget();
				SpriteBatch = new SpriteBatch(Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice);

			}
		}

		public override void UnloadGraphicsContent(bool unloadAllContent)
		{
			base.UnloadGraphicsContent(unloadAllContent);
			
		}

		/*public override void OnDeviceReset( ByVal sender As Object, ByVal e As DeviceEventArgs)
		{
			MyBase.OnDeviceReset(sender, e)
			CreateRTS(e.Device)
			CreateBackground(e.Device)
	}*/

		public void CreateRenderTarget()//(ByVal Dev As Direct3D.Device)
		{
			/////if (!Parent.HoofdObj.InRun) return;
			if (Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice == null) return;
			//if RTS IsNot Nothing Then Exit void
			DisposeRTS();
			if (Size.X == 0 || Size.Y == 0) throw new Exception("Invalid Size");

			RenderTarget = new RenderTarget2D(Parent.HoofdObj.XNAGame.Graphics.GraphicsDevice, (int)Size.X, (int)Size.Y, 1, SurfaceFormat.Color);
			//TODO: check voor een maximum size

			/*setTex(new Texture(Dev, CInt(Size.X), CInt(Size.Y), 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default))
			setSurf(Tex.GetSurfaceLevel(0))
			setSurfDesc(Surf.Description)
			setRTS(new Direct3D.RenderToSurface(Dev, SurfDesc.Width, SurfDesc.Height, SurfDesc.Format, True, DepthFormat.D16))
			*/
		}

		public void DisposeRTS()
		{
			//if RTS IsNot Nothing Then RTS.Dispose()   //TODO: is dit geen memory leak?
			/*if Surf IsNot Nothing Then Surf.Dispose()
			if Tex IsNot Nothing Then Tex.Dispose()
			setRTS(Nothing)
			setSurf(Nothing)
			setTex(Nothing)
			setSurfDesc(Nothing)*/

		}

		public virtual void CreateBackground()//(ByVal Dev As Direct3D.Device)
		{
		}
		public virtual void DisposeBackground()
		{
		}

		/*protected override void Dispose(ByVal disposing As Boolean)
		{
			MyBase.Dispose(disposing)
			DisposeBackground()
			DisposeRTS()
	}*/



		public void SendElementToFront(BaseRender2DElement nRenderElement)
		{
			if (Elements.Remove(nRenderElement)) return;
			Elements.Insert(0, nRenderElement);
		}
	}
}
