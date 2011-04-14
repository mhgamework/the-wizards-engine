using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core
{
	public class KeyboardInfo
	{
		private KeyboardState prevKeyboardState;
		private KeyboardState keyboardState;

		public KeyboardInfo()
		{

		}


		public void UpdateKeyboardState(KeyboardState nKeyboardState)
		{
			prevKeyboardState = keyboardState;
			keyboardState = nKeyboardState;
		}


		public bool IsKeyStateDown(Keys key)
		{
			if ( prevKeyboardState.IsKeyUp( key ) && keyboardState.IsKeyDown( key ) )
				return true;
			else return false;
		}

		public bool IsKeyStatePressed(Keys key)
		{
			if ( prevKeyboardState.IsKeyDown( key ) && keyboardState.IsKeyDown( key ) )
				return true;
			else return false;
		}

		public bool IsKeyStateUp(Keys key)
		{
			if ( prevKeyboardState.IsKeyDown( key ) && keyboardState.IsKeyUp ( key ) )
				return true;
			else return false;
		}

		public bool IsKeyDown(Keys key)
		{
			if ( keyboardState.IsKeyDown( key ) )
				return true;
			else return false;
		}

		public bool IsKeyUp(Keys key)
		{
			if ( prevKeyboardState.IsKeyUp( key ) )
				return true;
			else return false;
		}



		public KeyboardState KeyboardState
		{
			get { return keyboardState; }
		}




	}
}
