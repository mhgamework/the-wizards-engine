using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core.Elements
{
	public class ProcessEventArgs : EventArgs
	{
        BaseHoofdObject hoofdObj;
		float _elapsed;
		private int _time;
		//private KeyboardState _keyboard;
		//private KeyboardState _prevKeyboard;
		private MouseInfo _mouse;
		private KeyboardInfo keyboard;





		public ProcessEventArgs(BaseHoofdObject nHoofdObj)//Microsoft.Xna.Framework.GameTime nGameTime)
			: base()
		{
            hoofdObj = nHoofdObj;
			//_gameTime = nGameTime;
			//_elapsed = (float)nGameTime.ElapsedGameTime.TotalSeconds;

			Mouse = new MouseInfo();
			keyboard = new KeyboardInfo();
		}



        /// <summary>
        /// Deprectated
        /// </summary>
        /// <param name="keyboardState"></param>
        /// <param name="mouseState"></param>
		public void UpdateInput(KeyboardState keyboardState, MouseState mouseState)
		{
			//_prevKeyboard = _keyboard;
			//_keyboard = keyboardState;

			_mouse.UpdateMouseState( mouseState );
			keyboard.UpdateKeyboardState( keyboardState );

		}






		/// <summary>
		/// 
		/// </summary>
		public float Elapsed { get { return _elapsed; } set { _elapsed = value; } }

		//Microsoft.Xna.Framework.GameTime _gameTime;
		//public Microsoft.Xna.Framework.GameTime GameTime
		//{
		//    get { return _gameTime; }
		//    set
		//    {
		//        _gameTime = value;
		//        _elapsed = (float)value.ElapsedGameTime.TotalSeconds;
		//    }
		//}



		public int Time
		{
			get { return _time; }
			set { _time = value; }
		}



		public KeyboardState KeyboardState
		{
			get { return keyboard.KeyboardState; }
		}

		public MouseState MouseStateOld
		{
			get { return Mouse.MouseState; }
		}



		public MouseInfo Mouse
		{
			get { return _mouse; }
			private set { _mouse = value; }
		}

		public KeyboardInfo Keyboard
		{
			get { return keyboard; }
		}

        public BaseHoofdObject HoofdObj { get { return hoofdObj; } }


	}

}
