using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class Render2DEventArgs : EventArgs
    {
        public Render2DEventArgs()//Microsoft.Xna.Framework.GraphicsDeviceManager nGraphics, CameraInfo nCamInfo)
            : base()
        {
            //_graphics = nGraphics;
            //_cameraInfo = nCamInfo;
        }

        private Microsoft.Xna.Framework.GraphicsDeviceManager _graphics;
        public Microsoft.Xna.Framework.GraphicsDeviceManager Graphics { get { return _graphics; } set { _graphics = value; } }

		private Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;

		public Microsoft.Xna.Framework.Graphics.SpriteBatch SpriteBatch
		{
			get { return _spriteBatch; }
			set { _spriteBatch = value; }
		}
	

        //private CameraInfo _cameraInfo;
        //public CameraInfo CameraInfo { get { return _cameraInfo; } set { _cameraInfo = value; } }
    }
}
