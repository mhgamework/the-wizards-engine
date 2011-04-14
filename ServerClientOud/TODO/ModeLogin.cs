using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class ModeLogin : IGameMode
    {
        ServerClientMainOud engine;

        private SpriteFont font;

        private Editor.GuiOutputBox outputBox;

        public ModeLogin( ServerClientMainOud nEngine )
        {
            engine = nEngine;

        }

        void Wereld_LoginCallback( object sender, MHGameWork.TheWizards.ServerClient.Network.ProxyServerWereld.LoginCallbackEventArgs e )
        {
            engine.Server.Wereld.LoginCallback -= new EventHandler<MHGameWork.TheWizards.ServerClient.Network.ProxyServerWereld.LoginCallbackEventArgs>( Wereld_LoginCallback );

            outputBox.AddLine( "Succesfully logged in." );
            outputBox.AddLine( "LoginKey: " + e.LoginKey );
            outputBox.AddLine( "" );

            outputBox.AddLine( "Linking UDP connection ..." );

            engine.Server.UDPConnectionLinked += new EventHandler( Server_UDPConnectionLinked );

            engine.Server.LinkUDPConnection( e.LoginKey );
        }

        void Server_UDPConnectionLinked( object sender, EventArgs e )
        {
            engine.Server.UDPConnectionLinked -= new EventHandler( Server_UDPConnectionLinked );

            outputBox.AddLine( "UDP connection linked." );

            if ( engine.Settings.DelayedLoading )
                engine.Invoker.Invoke<EventArgs>( OnCompleted, this, null, engine.Time + engine.Settings.DelayedLoadingTime );
            else
                engine.Invoker.Invoke<EventArgs>( OnCompleted, this, null );
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

        }

        public void Render()
        {
            Microsoft.Xna.Framework.Graphics.GraphicsDevice dev = engine.XNAGame.GraphicsDevice;

            dev.Clear( Microsoft.Xna.Framework.Graphics.Color.Black );

            engine.SpriteBatch.Begin( SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState );

            outputBox.Draw( engine.SpriteBatch );
            //WriteCenteredText( "Attempting to login with username 'MHGW' ...", 0 );


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


            outputBox = new MHGameWork.TheWizards.ServerClient.Editor.GuiOutputBox( 0, 0, engine.XNAGame.Window.ClientBounds.Width, engine.XNAGame.Window.ClientBounds.Height );
            outputBox.Font = font;
            outputBox.TextColor = Color.Red;
            outputBox.LineSpacing = 30;

            outputBox.AddLine( "Attempting to login with username 'MHGW' ..." );
            outputBox.AddLine( "" );


            engine.Server.Wereld.LoginCallback += new EventHandler<Network.ProxyServerWereld.LoginCallbackEventArgs>( Wereld_LoginCallback );
            engine.Server.Wereld.LoginAsync( "MHGW", "pass" );

        }

        public void StopGameMode()
        {

        }

        public void OnCompleted( object sender, EventArgs e )
        {
            StopGameMode();
            if ( Completed != null ) Completed( sender, e );
        }

        public event EventHandler Completed;

        #endregion



    }
}
