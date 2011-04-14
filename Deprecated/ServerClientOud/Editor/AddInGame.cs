using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /// <summary>
    /// Taken from http://www.wilcob.com/Wilco/Pastecode/6132/showpaste.aspx
    /// This is the main type for your game
    /// </summary>
    public class AddInGame : Game
    {
        GameControlPanel panel;

        GraphicsDeviceManager graphics;
        IGraphicsDeviceService graphicsService;
        GraphicsDevice graphicsDevice;

        ContentManager content;
        Form windowForm;

        public AddInGame( GameControlPanel panel )
        {
            this.panel = panel;

            graphics = new GraphicsDeviceManager( this );
            content = new ContentManager( Services );

            graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;

            graphicsService = this.Services.GetService( typeof( IGraphicsDeviceService ) ) as IGraphicsDeviceService;

            windowForm = Control.FromHandle( this.Window.Handle ) as Form;

            // Those three events are necessary to keep a "fresh" state, see individual methods
            graphicsService.DeviceCreated += delegate { OnDeviceCreated(); };
            graphicsService.DeviceResetting += delegate { OnDeviceResetting(); };
            graphicsService.DeviceReset += delegate { OnDeviceReset(); };

            graphics.PreferredBackBufferWidth = panel.Width;
            graphics.PreferredBackBufferHeight = panel.Height;

            graphics.ApplyChanges();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            graphicsDevice = graphicsService.GraphicsDevice;
            graphicsDevice.Reset();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update( GameTime gameTime )
        {
            base.Update( gameTime );
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            graphics.GraphicsDevice.Clear( Color.CornflowerBlue );
            // TODO: Add your drawing code here

            base.Draw( gameTime );
        }

        private void OnPreparingDeviceSettings( object sender, PreparingDeviceSettingsEventArgs args )
        {
            // Oh good this actually works in 2.0
            args.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = panel.GetSafeHandle();
        }



        /// <summary>
        /// This is needed for multi-screen setups, where the device is killed and reset
        /// whenever the window is tossed from one screen to another. Probably other situations 
        /// would cause the device to be re-created too, so make sure you have it.
        /// </summary>
        void OnDeviceCreated()
        {
            graphicsDevice = graphicsService.GraphicsDevice;
            graphicsDevice.Reset();
        }

        /// <summary>
        /// The device is reset everytime the window is resized; but we need to tell the graphics
        /// device about the new size since we're in control of it now.
        /// </summary>
        void OnDeviceResetting()
        {
            // This worked in XNA 1 Refresh
            //graphicsDevice.PresentationParameters.DeviceWindowHandle = this.panel.GetSafeHandle();
            graphicsDevice.PresentationParameters.BackBufferWidth = this.panel.Width;
            graphicsDevice.PresentationParameters.BackBufferHeight = this.panel.Height;

            windowForm.Hide();
            windowForm.FormBorderStyle = FormBorderStyle.None;
            windowForm.Enabled = false;
            windowForm.Visible = false;
            windowForm.ShowInTaskbar = false;
            windowForm.SetDesktopLocation( -windowForm.Width, -windowForm.Height );
            windowForm.Invalidate();

            this.panel.Invalidate();
        }

        private void OnDeviceReset()
        {
            // Used to correct the viewport when abusing the XNA Form to shove windows controls  in to
        }

    }
}
