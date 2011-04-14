using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.Game3DPlay.Core
{
	public class ValueChangedEventArgs<T> : EventArgs
	{
		public ValueChangedEventArgs(T nOldValue, T nNewValue)
			: base()
		{
			OldValue = nOldValue;
			NewValue = nNewValue;
		}

		private T _oldValue;

		public T OldValue
		{
			get { return _oldValue; }
			private set { _oldValue = value; }
		}

		private T _newValue;

		public T NewValue
		{
			get { return _newValue; }
			private set { _newValue = value; }
		}


	}

	public delegate void ValueChangedEventHandler<T>(object sender, ValueChangedEventArgs<T> e);
}
