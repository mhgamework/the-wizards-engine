using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
	public class Model : IDisposable
	{
		private ModelData modelData;
		private ServerClientMainOud engine;

		public Model(ServerClientMainOud nEngine, string nAssetName)
		{
			engine = nEngine;
			modelData = engine.ModelManager.GetModelData( this, nAssetName );
		}

		~Model()
		{
			Dispose();
		}



		public void Dispose()
		{
			engine.ModelManager.ReleaseModelData( this, modelData );
			modelData = null;
		}

		public void TempRender(Matrix renderMatrix)
		{
			if ( modelData.XNAModel == null )
				return;

			// Apply objectMatrix
			renderMatrix = modelData.ObjectMatrix * renderMatrix;

			//Draw the model, a model can have multiple meshes, so loop
			foreach ( ModelMesh mesh in modelData.XNAModel.Meshes )
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				foreach ( BasicEffect effect in mesh.Effects )
				{
                    effect.PreferPerPixelLighting = true;
					effect.EnableDefaultLighting();
                    
					effect.World = modelData.Transforms[ mesh.ParentBone.Index ] * renderMatrix;

					effect.View = engine.ActiveCamera.CameraInfo.ViewMatrix;
					effect.Projection = engine.ActiveCamera.CameraInfo.ProjectionMatrix;

				}
				//Draw the mesh, will use the effects set above.
				mesh.Draw();
			}

		}


		public static void TestRenderModel()
		{


			Model testModel = null;
			Game3DPlay.SpelObjecten.Spectater spec;
			TestServerClientMain.Start( "TestRenderModel",
				delegate
				{
					testModel = new Model( TestServerClientMain.Instance, @"Content\Shuriken001" );
					spec = new MHGameWork.Game3DPlay.SpelObjecten.Spectater( TestServerClientMain.Instance );
					TestServerClientMain.Instance.SetCamera( spec );

				},
				delegate
				{
					testModel.TempRender( Matrix.CreateScale( 1 ) );
				} );
		}

	}
}
