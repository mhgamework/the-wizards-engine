using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
	public class ModelManager
	{
		private ServerClientMainOud engine;
		private List<ModelData> models;
		private bool autoLoad;

		public ModelManager(ServerClientMainOud nEngine)
		{
			engine = nEngine;

			models = new List<ModelData>();

		}

		public ModelData GetModelData(Model nModel, string assetName)
		{
			ModelData modelData = null;

			//Loop kan misschien sneller? met een dictionary?
			for ( int i = 0; i < models.Count; i++ )
			{
				if ( models[ i ].AssetName == assetName )
				{
					modelData = models[ i ];
					break;
				}

			}

			if ( modelData == null )
			{
				modelData = new ModelData( assetName );
				models.Add( modelData );

				if ( autoLoad ) modelData.Load( engine.XNAGame._content );
			}



			modelData.AddReference( nModel );

			return modelData;
		}

		public void ReleaseModelData(Model nModel, ModelData nModelData)
		{
			nModelData.RemoveReference( nModel );
			if ( nModelData.ReferenceCount == 0 )
			{
				models.Remove( nModelData );
				nModelData.Dispose();

			}

#if DEBUG
			if ( nModelData.ReferenceCount < 0 )
			{
				throw new Exception( "Internal error! reference count kan niet kleiner zijn dan 0" );
			}
#endif

		}


		public virtual void Load(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			if ( e.AllContent )
			{
				for ( int i = 0; i < models.Count; i++ )
				{
					models[ i ].Load( engine.XNAGame._content );
				}
			}


			autoLoad = true;

		}

		public virtual void Unload(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			autoLoad = false;

			if ( e.AllContent )
			{
				for ( int i = 0; i < models.Count; i++ )
				{
					models[ i ].Unload( engine.XNAGame._content );
				}
			}
		}




		public static void TestRenderMultipleSameModels()
		{
			Model testModel1 = null;
			Model testModel2 = null;
			Model testModel3 = null;
			Game3DPlay.SpelObjecten.Spectater spec;
			TestServerClientMain.Start( "TestRenderMultipleSameModels",
				delegate
				{
					testModel1 = new Model( TestServerClientMain.Instance, @"Content\Shuriken001" );
					testModel2 = new Model( TestServerClientMain.Instance, @"Content\Shuriken001" );
					testModel3 = new Model( TestServerClientMain.Instance, @"Content\Shuriken001" );
					spec = new MHGameWork.Game3DPlay.SpelObjecten.Spectater( TestServerClientMain.Instance );
					TestServerClientMain.Instance.SetCamera( spec );

				},
				delegate
				{
					if ( TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyStateDown(Microsoft.Xna.Framework.Input.Keys.I))
					{
						TestServerClientMain.Instance.DoUnloadGraphicsContent(true);
					}
					if ( TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyStateUp ( Microsoft.Xna.Framework.Input.Keys.I ) )
					{
						TestServerClientMain.Instance.DoLoadGraphicsContent ( true );
					}
					if ( !TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.I ) )
					{
						if ( !TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.O ) )
							testModel1.TempRender( Matrix.CreateTranslation( 0, 0, 0 ) );
						testModel2.TempRender( Matrix.CreateTranslation( 2, 0, 2 ) );
						testModel3.TempRender( Matrix.CreateTranslation( -2, 0, 2 ) );
					}
				} );
		}


	}


}
