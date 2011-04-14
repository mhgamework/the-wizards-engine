using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
	public class Skydome
	{
		public GraphicsDevice device;
		private ServerClientMainOud main;

		private Model skyDome;
		private Texture2D skyDomeTexture;

		public  Engine.ShaderEffect effect;

		public float elapsedTime;

		public Skydome(ServerClientMainOud nMain)
		{
			main = nMain;
		}


		public void Load(GraphicsDevice nDevice)
		{
			device = nDevice;

			if ( effect != null ) { effect.Dispose(); effect = null; }
			//TODO: effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( main.XNAGame, "Content\\Skydome.fx" );
			effect.Load( main.XNAGame._content );

			skyDome = main.XNAGame._content.Load<Model>( "Content\\dome" );

			foreach ( ModelMesh mesh in skyDome.Meshes )
				foreach ( BasicEffect currenteffect in mesh.Effects )
					skyDomeTexture = currenteffect.Texture;

			foreach ( ModelMesh modmesh in skyDome.Meshes )
				foreach ( ModelMeshPart modmeshpart in modmesh.MeshParts )
					modmeshpart.Effect = effect.Effect.Clone( device );


		}

		public void RenderSkyDome()
		{
			foreach ( ModelMesh modmesh in skyDome.Meshes )
			{
				foreach ( Effect currenteffect in modmesh.Effects )
				{
					currenteffect.CurrentTechnique = effect.Effect.Techniques[ "Textured" ];
					Matrix worldMatrix = Matrix.CreateFromYawPitchRoll( MathHelper.PiOver2, -MathHelper.PiOver2, 0 )
						* Matrix.CreateScale( 7000, 7000, 7000 )
						* Matrix.CreateTranslation( main.ActiveCamera.CameraPosition - new Vector3( 0, 0, 3000 ) );
					currenteffect.Parameters[ "xWorld" ].SetValue( worldMatrix );
					currenteffect.Parameters[ "xView" ].SetValue( main.ActiveCamera.CameraInfo.ViewMatrix );
					currenteffect.Parameters[ "xProjection" ].SetValue( main.ActiveCamera.CameraInfo.ProjectionMatrix );
					currenteffect.Parameters[ "xTexture" ].SetValue( skyDomeTexture );
				}
				modmesh.Draw();
			}

			foreach ( ModelMesh modmesh in skyDome.Meshes )
			{
				foreach ( Effect currenteffect in modmesh.Effects )
				{
					currenteffect.CurrentTechnique = effect.Effect.Techniques[ "Textured" ];
					Matrix worldMatrix = Matrix.CreateFromYawPitchRoll( MathHelper.PiOver2, -MathHelper.PiOver2, 0 )
						* Matrix.CreateScale( 7000, 7000, 7000 )
						* Matrix.CreateRotationX( MathHelper.Pi )
						* Matrix.CreateRotationY( MathHelper.Pi )
						* Matrix.CreateTranslation( main.ActiveCamera.CameraPosition - new Vector3( 0, 0, 3000 ) );
					currenteffect.Parameters[ "xWorld" ].SetValue( worldMatrix );
					currenteffect.Parameters[ "xView" ].SetValue( main.ActiveCamera.CameraInfo.ViewMatrix );
					currenteffect.Parameters[ "xProjection" ].SetValue( main.ActiveCamera.CameraInfo.ProjectionMatrix );
					currenteffect.Parameters[ "xTexture" ].SetValue( skyDomeTexture );
					currenteffect.Parameters[ "xTime" ].SetValue( elapsedTime );
				}
				modmesh.Draw();
			}
		}


		public static void TestRenderSkydome()
		{

			XNAGeoMipMap.Skydome skydome = null;
			TestServerClientMain main = null;


			GraphicsDevice device = null;


			TestServerClientMain.Start( "TestWater002RenderWater",
				delegate
				{
					main = TestServerClientMain.Instance;
					device = main.XNAGame.Graphics.GraphicsDevice;

					skydome = new Skydome( main );




				},
				delegate
				{
					if ( skydome.device == null ) skydome.Load( main.XNAGame.Graphics.GraphicsDevice );
					skydome.RenderSkyDome();
				}
			);
		}
	}
}
