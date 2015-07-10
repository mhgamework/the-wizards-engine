using System;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.DEPRECATED.Gui
{
    public class DrawEventArgs : EventArgs, IHandelable
    {
        private GraphicsDevice device;

        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }

        private SpriteBatch spriteBatch;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
            set { spriteBatch = value; }
        }

        private GuiGraphics graphics;

        public GuiGraphics Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        public void TempCreateGraphics()
        {
            graphics = new GuiGraphics( device, spriteBatch );
        }

        #region IHandelable Members

        public bool Handled
        {
            get
            {
                return false;
            }
            set
            {
                throw new InvalidOperationException( "DrawEventArgs can never be handled" );
            }
        }

        #endregion
    }
}

