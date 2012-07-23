using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;


namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public class GuiServiceXNA : IGuiService
    {
        private XNAGame game;

        public XNAGame Game
        {
            get { return game; }
            set { game = value; }
        }

        private List<GuiControl> topLevelControls = new List<GuiControl>();

        public List<GuiControl> TopLevelControls
        {
            get { return topLevelControls; }
        }

        private bool inDrag;

        public bool InDrag
        {
            get { return inDrag; }
            set { inDrag = value; }
        }

        private GuiControl dragControl;
        private Point dragStartPoint;
        private Vector2 dragControlStartPosition;

        public GuiControl DragControl
        {
            get { return dragControl; }
            set { dragControl = value; }
        }

        public GuiServiceXNA( XNAGame nGame )
        {
            game = nGame;
        }

        public void StartDrag( GuiControl control )
        {
            if ( inDrag ) throw new InvalidOperationException();

            inDrag = true;
            dragControl = control;
            dragStartPoint = game.Mouse.CursorPosition;
            dragControlStartPosition = control.Position;

            control.OnDragStarted( null );

        }

        public void StopDrag()
        {
            if ( !InDrag ) throw new InvalidOperationException();

            inDrag = false;

            dragControl.OnDragEnded( null );

            dragControl = null;


        }
        private void RaiseTopControlsEvent<T>( T e, OnEventDelegate<T> del ) where T : EventArgs, IHandelable
        {
            for ( int i = 0; i < topLevelControls.Count; i++ )
            {
                RaiseControlsEvent( topLevelControls[ i ], e, del );
                if ( e.Handled ) break;
            }
        }

        private void RaiseControlsEvent<T>( GuiControl control, T e, OnEventDelegate<T> del ) where T : EventArgs, IHandelable
        {
            if ( e.Handled ) return;

            for ( int i = 0; i < control.Controls.Count; i++ )
            {
                RaiseControlsEvent( control.Controls[ i ], e, del );

            }

            del( control, e );
        }


        private delegate void OnEventDelegate<T>( GuiControl control, T args );

        public void DoMouseDown( GuiControl control, MouseEventArgs e )
        {
            if ( control.CheckOnControl( e.CursorPosition ) )
            {
                e.Handled = true;
                control.OnMouseDown( e );
            }
        }
        public void DoMouseUp( GuiControl control, MouseEventArgs e )
        {
            if ( control.CheckOnControl( e.CursorPosition ) )
            {
                e.Handled = true;
                control.OnMouseUp( e );
            }
        }
        public void DoMouseMove( GuiControl control, MouseEventArgs e )
        {
            if ( control.CheckOnControl( e.CursorPosition ) )
            {
                e.Handled = true;
                control.OnMouseMove( e );
            }
        }

        public void DoDraw( GuiControl control, DrawEventArgs e )
        {
            control.OnDraw( e );
        }

        public void DoUpdate( GuiControl control, DrawEventArgs e )
        {
            control.OnUpdate( e );
        }

        #region IGuiService Members

        public void Process()
        {
            //System.Threading.Thread.Sleep( 300 );
            //topLevelControls[ 0 ].Size = new Vector2( topLevelControls[ 0 ].Size.X + 1, topLevelControls[ 0 ].Size.Y );

            if ( game.Mouse.CursorEnabled )
            {
                if ( !InDrag )
                {
                    MouseEventArgs args = new MouseEventArgs();
                    args.CursorPosition = game.Mouse.CursorPosition;

                    if ( game.Mouse.LeftMouseJustPressed || game.Mouse.RightMouseJustPressed )
                    {
                        RaiseTopControlsEvent( args, DoMouseDown );
                    }
                    else if ( game.Mouse.LeftMouseJustReleased || game.Mouse.RightMouseJustReleased )
                    {
                        RaiseTopControlsEvent( args, DoMouseUp );
                    }
                    else if ( game.Mouse.LeftMousePressed || game.Mouse.RightMousePressed )
                    {
                        RaiseTopControlsEvent( args, DoMouseMove );
                    }
                }
                else
                {
                    //In Drag

                    Point mov = new Point( game.Mouse.CursorPosition.X - dragStartPoint.X,
                        game.Mouse.CursorPosition.Y - dragStartPoint.Y );


                    dragControl.Position = dragControlStartPosition + new Vector2( mov.X, mov.Y );


                    if ( game.Mouse.LeftMouseJustReleased )
                    {
                        StopDrag();
                    }
                }

            }

            {
                DrawEventArgs args = new DrawEventArgs();
                args.Device = game.GraphicsDevice;
                args.SpriteBatch = game.SpriteBatch;
                args.TempCreateGraphics();



                //Update all controls
                RaiseTopControlsEvent( args, DoUpdate );
            }
        }


        //public void DoMouseDown( GuiControl control, MouseEventArgs e)
        //{
        //    OnEventDelegate<MouseEventArgs> del;

        //    if ( e.Handled ) return;

        //    for ( int i = 0; i < control.Controls.Count; i++ )
        //    {
        //        DoMouseDown( control.Controls[ i ], e );

        //    }

        //    if ( control.CheckOnControl( e.CursorPosition ) )
        //    {
        //        e.Handled = true;
        //        control.OnMouseDown( e );
        //    }


        //}

        public void Render()
        {




            DrawEventArgs args = new DrawEventArgs();
            args.Device = game.GraphicsDevice;
            args.SpriteBatch = game.SpriteBatch;
            args.TempCreateGraphics();




            args.SpriteBatch.Begin( Microsoft.Xna.Framework.Graphics.SpriteBlendMode.AlphaBlend, Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate, Microsoft.Xna.Framework.Graphics.SaveStateMode.SaveState );

            args.Device.SamplerStates[ 0 ].MagFilter = Microsoft.Xna.Framework.Graphics.TextureFilter.Linear;
            args.Device.SamplerStates[ 0 ].MinFilter = Microsoft.Xna.Framework.Graphics.TextureFilter.Linear;


            for ( int i = 0; i < topLevelControls.Count; i++ )
            {
                DoDraw( topLevelControls[ i ], args );
            }

            //RaiseTopControlsEvent( args, DoDraw );

            args.SpriteBatch.End();

        }

        //public void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        //{

        //}



        #endregion
    }
}
