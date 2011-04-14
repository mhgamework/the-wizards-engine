using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace MHGameWork.TheWizards.ServerClient
{
    public class ModeConnectToServer : IGameMode
    {
        ServerClientMainOud engine;

        private SpriteFont font;

        private bool completed;

        public ModeConnectToServer( ServerClientMainOud nEngine )
        {
            engine = nEngine;
        }

        #region IGameObject Members

        public void Initialize()
        {
            font = engine.XNAGame.Content.Load<SpriteFont>( @"Content\ComicSansMS" );
        }

        public void LoadGraphicsContent()
        {

        }

        public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        {
            if ( engine.Server.ConnectedToServer && !completed )
            {
                completed = true;
                if ( engine.Settings.DelayedLoading )
                    engine.Invoker.Invoke<EventArgs>( OnCompleted, this, null, engine.Time + engine.Settings.DelayedLoadingTime );
                else
                    engine.Invoker.Invoke<EventArgs>( OnCompleted, this, null );
            }
        }

        public void Render()
        {
            Microsoft.Xna.Framework.Graphics.GraphicsDevice dev = engine.XNAGame.GraphicsDevice;

            dev.Clear( Microsoft.Xna.Framework.Graphics.Color.Black );

            engine.SpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );


            WriteCenteredText( "Attempting to connect to server", -30 );
            WriteCenteredText( "Connecting to " + engine.Server.ServerIP + " ...", 0 );

            if ( engine.Server.ConnectedToServer )
                WriteCenteredText( "Connection established", 50 );


            engine.SpriteBatch.End();

        }

        private void WriteCenteredText( string text, float relativeY )
        {
            engine.SpriteBatch.DrawString( font, text, CalculateCenteredTextPos( text )
               + new Vector2( 0, relativeY ), Color.Red );
        }


        private Vector2 CalculateCenteredTextPos( string text )
        {
            Vector2 size = font.MeasureString( text );
            Vector2 screenSize = new Vector2( engine.XNAGame.Window.ClientBounds.Width, engine.XNAGame.Window.ClientBounds.Height );

            Vector2 pos = screenSize * 0.5f - size * 0.5f;

            return pos;
        }


        public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {

        }

        public void StartGameMode()
        {
            engine.Server.ServerIP = engine.Settings.ServerIP;
            engine.Server.ConnectAsync();
        }

        public void StopGameMode()
        {

        }

        public void OnCompleted( object sender, EventArgs e )
        {
            if ( Completed != null ) Completed( sender, e );
        }

        public event EventHandler Completed;

        #endregion
    }
}
