using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class Cursor002
    {
        private ServerClientMainOud engine;

        private string filename;
        private Texture2D texture;
        private Vector2 position;

        private float speed;
        private Rectangle bounding;
        private Vector2 pointerPosition;

        public Cursor002( ServerClientMainOud nEngine )
        {
            engine = nEngine;
            position = new Vector2( 100, 100 );
            speed = 1;
        }

        public void Load( string nFilename )
        {
            filename = nFilename;

            texture = Texture2D.FromFile( engine.XNAGame.GraphicsDevice, nFilename );
        }

        public void Render( SpriteBatch batch )
        {
            batch.Draw( texture, new Rectangle( (int)( position.X - pointerPosition.X ), (int)( position.Y - pointerPosition.Y ), texture.Width, texture.Height ), Color.White );
        }

        //public void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e )
        //{
        //    // Why is this -= ? perhaps an engine error?
        //    // EDIT: 8 maand later: Lol inderdaad een error. TODO: aanpassen :P

        //    position.X -= e.Mouse.OppositeRelativeX * speed;
        //    position.Y -= e.Mouse.OppositeRelativeY * speed;


        //    if ( position.X < bounding.Left ) position.X = bounding.Left;
        //    if ( position.X > bounding.Right ) position.X = bounding.Right;
        //    if ( position.Y < bounding.Top ) position.Y = bounding.Top;
        //    if ( position.Y > bounding.Bottom ) position.Y = bounding.Bottom;
        //}

        public Point GetClickPoint()
        {
            return new Point( (int)position.X, (int)position.Y );
        }
        
        
        /// <summary>
        /// Specifies in which area this cursor can move
        /// </summary>
        public Rectangle Bounding
        { get { return bounding; } set { bounding = value; } }

        public float Speed
        { get { return speed; } set { speed = value; } }

        /// <summary>
        /// Defines where on the texture the point is that should be used as clicking target.
        /// </summary>
        public Vector2 PointerPosition
        { get { return pointerPosition; } set { pointerPosition = value; } }

        public Vector2 Position
        { get { return position; } set { position = value; } }
    }
}
