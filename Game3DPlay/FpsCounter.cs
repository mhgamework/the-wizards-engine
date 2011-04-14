using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MHGameWork.Game3DPlay
{
	public partial class FpsCounter : GameComponent
	{
		private float updateInterval = 1.0f;
		float timeSinceLastUpdate = 0.0f;
		float framecount = 0;
		float fps = 0;

		public FpsCounter(Game game)
			: base(game)
		{
			Enabled = true;
		}

		public override void Update(GameTime gameTime)
		{
			// TODO: Add your update code here
			float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
			framecount++;
			timeSinceLastUpdate += elapsed;

			if (timeSinceLastUpdate > updateInterval)
			{
				fps = framecount / timeSinceLastUpdate; //mean fps over updateIntrval
				framecount = 0;
				timeSinceLastUpdate -= updateInterval;

				if (Updated != null)
					Updated(this, new EventArgs());
			}

			base.Update(gameTime);
		}

		public float FPS
		{
			get { return fps; }
		}

		public event EventHandler<EventArgs> Updated;
	}
}