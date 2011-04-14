using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace MHGameWork.Game3DPlay.Core
{
	public class HoofdObjectXNA : BaseHoofdObject, Elements.ILoadable
	{
		public HoofdObjectXNA()
			: base()
		{
			HoofdObj = this;
			CreateStandardElements(this);
			_processContainer = new Elements.ProcessContainer(this);
			_renderContainer = new Elements.RenderContainer(this);
			_loadElement = GetElement<Elements.LoadElement>();
			XNAGame = new XNAGame(this);


			_processEventArgs = new MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs();
			_processEventArgs.Mouse = new MouseInfo();


			_renderEventArgs = new MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs();
			_renderEventArgs.CameraInfo = new CameraInfo();
			_renderEventArgs.Graphics = XNAGame.Graphics;


			cameraInfo.ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, -4000), Vector3.Zero, Vector3.Up);
			cameraInfo.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
					4 / 3, 1.0f, 10000.0f);
			//aspectRatio, 1.0f, 10000.0f);




		}

		int _backbufferCenterX;
		int _backbufferCenterY;

		public override void Initialize()
		{
			base.Initialize();
			_backbufferCenterX = (int)(XNAGame.Graphics.GraphicsDevice.DisplayMode.Width * 0.5);
			_backbufferCenterY = (int)(XNAGame.Graphics.GraphicsDevice.DisplayMode.Height * 0.5);
		}



		Elements.ProcessContainer _processContainer;
		Elements.RenderContainer _renderContainer;
		Elements.LoadElement _loadElement;

		XNAGame _xnaGame;
		public XNAGame XNAGame
		{
			get { return _xnaGame; }
			private set { _xnaGame = value; }
		}

		private Elements.ProcessEventArgs _processEventArgs;

		public override void DoProcess(object sender, GameTime gameTime)
		{

			_processEventArgs.GameTime = gameTime;
			_processEventArgs.Keyboard = Keyboard.GetState();
			MouseState mouseState = Mouse.GetState();
			_processEventArgs.Mouse.ProcessMouseState(_backbufferCenterX - mouseState.X, _backbufferCenterY - mouseState.Y, mouseState);
			Mouse.SetPosition(_backbufferCenterX, _backbufferCenterY);

			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}
			_processContainer.OnProcess(sender, _processEventArgs);

		}

		private Elements.RenderEventArgs _renderEventArgs;

		private CameraInfo cameraInfo
		{
			get { return _renderEventArgs.CameraInfo; }
			set { _renderEventArgs.CameraInfo = value; }
		}

		private Camera _activeCamera;
		public Camera ActiveCamera
		{
			get { return _activeCamera; }

		}


		public void SetCamera(Camera cam)
		{
			_activeCamera = cam;
			cameraInfo = cam.CameraInfo;
		}

		private SpriteBatch _spriteBatch;
		private SpriteBatch SpriteBatch
		{
			get { return _spriteBatch; }
			set
			{
				_spriteBatch = value;
				_renderEventArgs.SpriteBatch = value;
			}
		}


		public virtual void DoBeforeRender(object sender)//, Elements.BaseRenderElement.RenderEventArgs e)
		{
			_renderContainer.OnBeforeRender(sender, _renderEventArgs);
		}
		public virtual void DoRender(object sender)//, Elements.BaseRenderElement.RenderEventArgs e)
		{
			_renderEventArgs.Graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

			_renderContainer.OnRender(sender, _renderEventArgs);

			SpriteBatch.End();
		}
		public virtual void DoAfterRender(object sender)//, Elements.BaseRenderElement.RenderEventArgs e)
		{
			_renderContainer.OnAfterRender(sender, _renderEventArgs);
		}

		public override void Run()
		{
			XNAGame.Run();
		}

		public override void Exit()
		{
			Dispose();
			XNAGame.Exit();
		}

		public override void CreateStandardElements(SpelObject nSpO)
		{
			
		}






		public override void DoInitialize()
		{
			Initialize();


		}


		/// <summary>
		/// Load your graphics content.  If loadAllContent is true, you should
		/// load content from both ResourceManagementMode pools.  Otherwise, just
		/// load ResourceManagementMode.Manual content.
		/// </summary>
		/// <param name="loadAllContent">Which type of content to load.</param>
		public virtual void DoLoadGraphicsContent(bool loadAllContent)
		{
			_loadElement.OnLoad(this, new MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs(XNAGame._content, loadAllContent));

		}


		/// <summary>
		/// Unload your graphics content.  If unloadAllContent is true, you should
		/// unload content from both ResourceManagementMode pools.  Otherwise, just
		/// unload ResourceManagementMode.Manual content.  Manual content will get
		/// Disposed by the GraphicsDevice during a Reset.
		/// </summary>
		/// <param name="unloadAllContent">Which type of content to unload.</param>
		public virtual void DoUnloadGraphicsContent(bool unloadAllContent)
		{
			_loadElement.OnUnload(this, new MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs(XNAGame._content, unloadAllContent));

		}






		#region ILoadable Members

		public void OnLoad(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			if (e.AllContent)
			{
				SpriteBatch = new SpriteBatch(XNAGame.Graphics.GraphicsDevice);
			}
		}

		public void OnUnload(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			if (e.AllContent)
			{
				//SpriteBatch
			}
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
