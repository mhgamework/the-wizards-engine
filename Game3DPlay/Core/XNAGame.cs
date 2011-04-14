#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace MHGameWork.Game3DPlay.Core
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class XNAGame : Microsoft.Xna.Framework.Game 
	{
		GraphicsDeviceManager _graphics;
		public GraphicsDeviceManager Graphics { get { return _graphics; } }
		public DynamicContentManager _content;

		BaseHoofdObject _hoofdObj;


		public XNAGame(BaseHoofdObject nHoofdObj)
		{
			_hoofdObj = nHoofdObj;
			_graphics = new GraphicsDeviceManager( this );
			_content = new DynamicContentManager( Services );

			if ( GraphicsAdapter.DefaultAdapter.GetCapabilities( DeviceType.Hardware ).MaxPixelShaderProfile < ShaderProfile.PS_1_1 )
			{
				System.Windows.Forms.MessageBox.Show( "No capable Shader 1.1 device found! Switching to reference mode" );
				_graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>( setToReference );
			}

            _graphics.PreferMultiSampling = true;
            
		}

		private void setToReference(object sender, PreparingDeviceSettingsEventArgs eventargs)
		{
			/*eventargs.GraphicsDeviceInformation.CreationOptions = CreateOptions.SoftwareVertexProcessing;
			eventargs.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
			eventargs.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = MultiSampleType.None;*/
		}


		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			_hoofdObj.DoInitialize();
			// TODO: Add your initialization logic here

			base.Initialize();
		}


		/// <summary>
		/// Load your graphics content.  If loadAllContent is true, you should
		/// load content from both ResourceManagementMode pools.  Otherwise, just
		/// load ResourceManagementMode.Manual content.
		/// </summary>
		/// <param name="loadAllContent">Which type of content to load.</param>
		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			_hoofdObj.DoLoadGraphicsContent( loadAllContent );
			if ( loadAllContent )
			{
				// TODO: Load any ResourceManagementMode.Automatic content
			}

			// TODO: Load any ResourceManagementMode.Manual content
		}


		/// <summary>
		/// Unload your graphics content.  If unloadAllContent is true, you should
		/// unload content from both ResourceManagementMode pools.  Otherwise, just
		/// unload ResourceManagementMode.Manual content.  Manual content will get
		/// Disposed by the GraphicsDevice during a Reset.
		/// </summary>
		/// <param name="unloadAllContent">Which type of content to unload.</param>
		protected override void UnloadGraphicsContent(bool unloadAllContent)
		{
			_hoofdObj.DoUnloadGraphicsContent( unloadAllContent );
			if ( unloadAllContent )
			{
				// TODO: Unload any ResourceManagementMode.Automatic content
				_content.Unload();
			}

			// TODO: Unload any ResourceManagementMode.Manual content
		}


		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{

			_hoofdObj.DoProcess( _hoofdObj, gameTime );

			base.Update( gameTime );
		}

		protected override bool BeginDraw()
		{
			_hoofdObj.DoBeforeRender( _hoofdObj );//, new Elements.BaseRenderElement.RenderEventArgs(_graphics));
			return base.BeginDraw();
		}
		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			Graphics.GraphicsDevice.Clear( Color.CornflowerBlue );
			base.Draw( gameTime );
			_hoofdObj.DoRender( _hoofdObj );//, new Elements.BaseRenderElement.RenderEventArgs(_graphics));


		}

		protected override void EndDraw()
		{
			_hoofdObj.DoAfterRender( _hoofdObj );//, new Elements.BaseRenderElement.RenderEventArgs(_graphics));
			base.EndDraw();
		}


		protected override void BeginRun()
		{
			_hoofdObj._inRun = true;
			base.BeginRun();

		}

		protected override void EndRun()
		{
			base.EndRun();
			_hoofdObj._inRun = false;
		}
	}
}
