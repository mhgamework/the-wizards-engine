#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace MHGameWork.Game3DPlay.Core
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XNAGameNoRender : Microsoft.Xna.Framework.Game
    {

		BaseHoofdObject _hoofdObj;


		public XNAGameNoRender(BaseHoofdObject nHoofdObj)
			{
				_hoofdObj = nHoofdObj;


	
			}




			/// <summary>
			/// Allows the game to perform any initialization it needs to before starting to run.
			/// This is where it can query for any required services and load any non-graphic
			/// related content.  Calling base.Initialize will enumerate through any components
			/// and initialize them as well.
			/// </summary>
			protected override void Initialize()
			{
				_hoofdObj.DoInitialize();
				// TODO: Add your initialization logic here

				base.Initialize();
			}






			/// <summary>
			/// Allows the game to run logic such as updating the world,
			/// checking for collisions, gathering input and playing audio.
			/// </summary>
			/// <param name="gameTime">Provides a snapshot of timing values.</param>
			protected override void Update(GameTime gameTime)
			{

				_hoofdObj.DoProcess(_hoofdObj, gameTime);

				base.Update(gameTime);
			}

			
			protected override void BeginRun()
			{
				_hoofdObj._inRun = true;
				base.BeginRun();

			}

			protected override void EndRun()
			{
				base.EndRun();
				_hoofdObj._inRun = false;
			}
    }
}
