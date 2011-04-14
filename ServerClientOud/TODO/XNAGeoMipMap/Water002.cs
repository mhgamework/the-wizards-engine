using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
	public class Water002
	{
		ServerClientMainOud main;
		GraphicsDevice device;
		RenderTarget2D refractionRenderTarget;
		Texture2D refractionMap;

		RenderTarget2D reflectionRenderTarget;
		Texture2D reflectionMap;

		private float waterHeight;

		private Matrix reflectionViewMatrix;

		private VertexPositionTexture[] waterVertices;

		private float width;
		private float height;

		public Engine.ShaderEffect effect;

		private float clippingError;

		private Texture2D waterBumpMap;

		public float elapsedTime;

		public delegate void RenderDelegate();

		public Water002(ServerClientMainOud nMain)
		{
			main = nMain;
			waterHeight = 70f;
			//width = 4000;
			//height = 4000;
			width = 10000;
			height = 10000;


			clippingError = 1f;

            //TODO: effect = new MHGameWork.TheWizards.ServerClient.Engine.ShaderEffect( main, "Content\\Water001.fx" );
		}


		public void Load(GraphicsDevice nDevice)
		{
			device = nDevice;
			if ( refractionRenderTarget != null ) refractionRenderTarget.Dispose();
			if ( refractionMap != null ) refractionMap.Dispose();

			if ( reflectionRenderTarget != null ) reflectionRenderTarget.Dispose();
			if ( reflectionMap != null ) reflectionMap.Dispose();

			refractionRenderTarget = null;
			refractionMap = null;

			reflectionRenderTarget = null;
			reflectionMap = null;


			refractionRenderTarget = new RenderTarget2D( device, 512, 512, 1, SurfaceFormat.Color );
			reflectionRenderTarget = new RenderTarget2D( device, 512, 512, 1, SurfaceFormat.Color );


			SetUpWaterVertices();


			waterBumpMap = main.XNAGame._content.Load<Texture2D>( "Content\\waterbump" );


		}


		private void SetUpWaterVertices()
		{
			waterVertices = new VertexPositionTexture[ 6 ];

			/*waterVertices[ 0 ] = new VertexPositionTexture( new Vector3( 0, 0, waterHeight ), new Vector2( 0, 1 ) );
			waterVertices[ 2 ] = new VertexPositionTexture( new Vector3( width, height, waterHeight ), new Vector2( 1, 0 ) );
			waterVertices[ 1 ] = new VertexPositionTexture( new Vector3( 0, height, waterHeight ), new Vector2( 0, 0 ) );

			waterVertices[ 3 ] = new VertexPositionTexture( new Vector3( 0, 0, waterHeight ), new Vector2( 0, 1 ) );
			waterVertices[ 5 ] = new VertexPositionTexture( new Vector3( width, 0, waterHeight ), new Vector2( 1, 1 ) );
			waterVertices[ 4 ] = new VertexPositionTexture( new Vector3( width, height, waterHeight ), new Vector2( 1, 0 ) );*/

			waterVertices[ 0 ] = new VertexPositionTexture( new Vector3( 0, waterHeight, 0 ), new Vector2( 0, 1 ) );
			waterVertices[ 1 ] = new VertexPositionTexture( new Vector3( 0, waterHeight, -height ), new Vector2( 0, 0 ) );
			waterVertices[ 2 ] = new VertexPositionTexture( new Vector3( width, waterHeight, -height ), new Vector2( 1, 0 ) );


			waterVertices[ 3 ] = new VertexPositionTexture( new Vector3( 0, waterHeight, 0 ), new Vector2( 0, 1 ) );

			waterVertices[ 5 ] = new VertexPositionTexture( new Vector3( width, waterHeight, 0 ), new Vector2( 1, 1 ) );
			waterVertices[ 4 ] = new VertexPositionTexture( new Vector3( width, waterHeight, -height ), new Vector2( 1, 0 ) );

		}


		public void DrawRefractionMap(RenderDelegate renderDelegate)
		{
			//Vector3 planeNormalDirection = new Vector3( 0, 0, -1 ); ?? (riemers code)



			Vector3 planeNormalDirection = new Vector3( 0, -1, 0 );

			planeNormalDirection.Normalize(); //optional

			Vector4 planeCoefficients = new Vector4( planeNormalDirection, waterHeight + clippingError + 50 );

			Matrix camMatrix = main.ActiveCamera.CameraInfo.ViewProjectionMatrix;
			Matrix invCamMatrix = Matrix.Invert( camMatrix );
			invCamMatrix = Matrix.Transpose( invCamMatrix );

			planeCoefficients = Vector4.Transform( planeCoefficients, invCamMatrix );
			Plane refractionClipPlane = new Plane( planeCoefficients );


			device.ClipPlanes[ 0 ].Plane = refractionClipPlane;
			device.ClipPlanes[ 0 ].IsEnabled = true;

			device.SetRenderTarget( 0, refractionRenderTarget );
			//device.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0 );
			device.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, new Color( 0, 0, 0, 0 ), 1.0f, 0 );

			renderDelegate();

			//device.ResolveRenderTarget( 0 );
            device.SetRenderTarget(0, null);
			refractionMap = refractionRenderTarget.GetTexture();
			

			device.ClipPlanes[ 0 ].IsEnabled = false;



		}

		public void DrawReflectionMap(RenderDelegate renderDelegate)
		{
			/*main.ActiveCamera.CameraPosition = new Vector3( 10, 500, 10 );
			main.ActiveCamera.AngleVertical = -MathHelper.PiOver4;
			main.ActiveCamera.AngleRoll = 0;
			main.ActiveCamera.AngleHorizontal = 0;*/

			float reflectionCamYCoord = -main.ActiveCamera.CameraPosition.Y + 2 * waterHeight;
			Vector3 reflectionCamPosition = new Vector3( main.ActiveCamera.CameraPosition.X
														, reflectionCamYCoord
														, main.ActiveCamera.CameraPosition.Z );

			//float reflectionTargetZCoord = -targetPos.Z + 2 * waterHeight;
			//Vector3 reflectionCamTarget = new Vector3( targetPos.X, targetPos.Y, reflectionTargetZCoord );



			Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll(
				-main.ActiveCamera.angleY
				, -main.ActiveCamera.angleX
				, -main.ActiveCamera.angleZ );

			Vector3 forwardVector = new Vector3( main.ActiveCamera.CameraDirection.X
												, -main.ActiveCamera.CameraDirection.Y
												, main.ActiveCamera.CameraDirection.Z );

			Vector3 reflectionCamTarget = reflectionCamPosition + forwardVector;

			Vector3 sideVector = Vector3.Transform( new Vector3( 1, 0, 0 ), sourceMatrix );
			//Vector3 reflectionCamUp = Vector3.Cross( sideVector, forwardVector );//forward,sidefector ? 
			Vector3 reflectionCamUp = Vector3.Cross( forwardVector, sideVector );//forward,sidefector ? 

			reflectionViewMatrix = Matrix.CreateLookAt( reflectionCamPosition, reflectionCamTarget, reflectionCamUp );


			//Vector3 planeNormalDirection = new Vector3( 0, 0, -1 ); ?? (riemers code)



			Vector3 planeNormalDirection = new Vector3( 0, 1, 0 );

			planeNormalDirection.Normalize(); //optional

			Vector4 planeCoefficients = new Vector4( planeNormalDirection, -waterHeight + clippingError );

			Matrix camMatrix = reflectionViewMatrix * main.ActiveCamera.CameraInfo.ProjectionMatrix;
			Matrix invCamMatrix = Matrix.Invert( camMatrix );
			invCamMatrix = Matrix.Transpose( invCamMatrix );

			planeCoefficients = Vector4.Transform( planeCoefficients, invCamMatrix );
			Plane reflectionClipPlane = new Plane( planeCoefficients );


			//Temporary
			Game3DPlay.Core.Camera cam = new MHGameWork.Game3DPlay.Core.Camera( main );
			cam.CameraPosition = reflectionCamPosition;
			cam.CameraDirection = forwardVector;
			cam.CameraUp = reflectionCamUp;

			cam.UpdateCameraInfo();

			Game3DPlay.Core.Camera oldCam = main.ActiveCamera;

			main.SetCamera( cam );







			device.ClipPlanes[ 0 ].Plane = reflectionClipPlane;
			device.ClipPlanes[ 0 ].IsEnabled = true;

			device.SetRenderTarget( 0, reflectionRenderTarget );
			device.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1.0f, 0 );
			//device.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, new Color( 0, 0, 0, 0 ), 1.0f, 0 );

			renderDelegate();

            device.SetRenderTarget(0, null);
			reflectionMap = reflectionRenderTarget.GetTexture();
			

			device.ClipPlanes[ 0 ].IsEnabled = false;


			main.SetCamera( oldCam );

		}


		public void RenderWater()
		{
			effect.Effect.CurrentTechnique = effect.Effect.Techniques[ "Water" ];
			//Matrix worldMatrix = Matrix.Identity;
			//Matrix worldMatrix = Matrix.CreateTranslation( -2000, 0, 2000 );
			Matrix worldMatrix = Matrix.CreateTranslation( -5000, 0, 5000 );
			effect.Effect.Parameters[ "xWorld" ].SetValue( worldMatrix );
			effect.Effect.Parameters[ "xView" ].SetValue( main.ActiveCamera.CameraInfo.ViewMatrix );
			effect.Effect.Parameters[ "xReflectionView" ].SetValue( reflectionViewMatrix );
			effect.Effect.Parameters[ "xProjection" ].SetValue( main.ActiveCamera.CameraInfo.ProjectionMatrix );
			effect.Effect.Parameters[ "xReflectionMap" ].SetValue( reflectionMap );
			effect.Effect.Parameters[ "xRefractionMap" ].SetValue( refractionMap );
			effect.Effect.Parameters[ "xWaterBumpMap" ].SetValue( waterBumpMap );
			effect.Effect.Parameters[ "xWaveLength" ].SetValue( 0.1f );
			//effect.Effect.Parameters[ "xWaveHeight" ].SetValue( 0.3f );
			effect.Effect.Parameters[ "xWaveHeight" ].SetValue( 0.3f );
			effect.Effect.Parameters[ "xCamPos" ].SetValue( main.ActiveCamera.CameraPosition );
			effect.Effect.Parameters[ "xTime" ].SetValue( elapsedTime );
			effect.Effect.Parameters[ "xWindForce" ].SetValue( 8.0f );
			Matrix windDirection = Matrix.CreateRotationZ( MathHelper.PiOver2 );
			effect.Effect.Parameters[ "xWindDirection" ].SetValue( windDirection );

			effect.Effect.Begin();
			foreach ( EffectPass pass in effect.Effect.CurrentTechnique.Passes )
			{
				pass.Begin();
				device.VertexDeclaration = new VertexDeclaration( device, VertexPositionTexture.VertexElements );
				device.DrawUserPrimitives( PrimitiveType.TriangleList, waterVertices, 0, 2 );
				pass.End();
			}
			effect.Effect.End();
		}


        //public static void TestRefractionMap()
        //{

        //    XNAGeoMipMap.Water002 water = null;
        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;


        //    TestServerClientMain.Start( "TestWater002Refractionmap",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;

        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = false;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            water = new MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Water002( main );
        //            water.Load( main.XNAGame.Graphics.GraphicsDevice );

        //            //water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 20 ) * Matrix.CreateTranslation( 0, 50, 0 );

        //        },
        //        delegate
        //        {
        //            if ( water.refractionRenderTarget.IsDisposed ) water.Load( main.XNAGame.Graphics.GraphicsDevice );
        //            terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //            terr.CameraPostion = main.ActiveCamera.CameraPosition;



        //            water.DrawRefractionMap( delegate
        //            {
        //                terr.Draw();

        //            } );

        //            main.SpriteBatch.Begin();
        //            main.SpriteBatch.Draw( water.refractionMap, new Vector2( 50, 50 ), Color.White );
        //            main.SpriteBatch.End();



        //            /*device.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1.0f, 0 );

        //            terr.Draw();*/
        //        }
        //    );
        //}

        //public static void TestReflectionMap()
        //{

        //    XNAGeoMipMap.Water002 water = null;
        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;


        //    TestServerClientMain.Start( "TestWater002ReflectionMap",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;

        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = false;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            water = new MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Water002( main );
        //            water.Load( main.XNAGame.Graphics.GraphicsDevice );

        //            //water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 20 ) * Matrix.CreateTranslation( 0, 50, 0 );

        //        },
        //        delegate
        //        {



        //            water.DrawReflectionMap( delegate
        //            {
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;

        //                terr.Draw();

        //            } );


        //            main.XNAGame.Graphics.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1.0f, 0 );

        //            if ( !main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.F ) )
        //            {

        //                main.SpriteBatch.Begin();
        //                main.SpriteBatch.Draw( water.reflectionMap, new Vector2( 50, 50 ), Color.White );
        //                main.SpriteBatch.End();


        //            }
        //            else
        //            {
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;


        //                terr.Draw();
        //            }
        //        }
        //    );
        //}

        //public static void TestRenderWater()
        //{
        //    XNAGeoMipMap.Skydome skydome = null;
        //    XNAGeoMipMap.Water002 water = null;
        //    TestServerClientMain main = null;


        //    XNAGeoMipMap.Terrain terr = null;


        //    GraphicsDevice device = null;

        //    //truukje
        //    float backwidth = 0;

        //    TestServerClientMain.Start( "TestWater002RenderWater",
        //        delegate
        //        {
        //            main = TestServerClientMain.Instance;
        //            device = main.XNAGame.Graphics.GraphicsDevice;

        //            terr = new XNAGeoMipMap.Terrain( TestServerClientMain.instance,
        //                TestServerClientMain.instance.XNAGame._content.RootDirectory
        //                + @"\Content\Terrain\Data.txt" );

        //            terr.HeightMap = new Common.GeoMipMap.HeightMap( 513, 513, @"Content\SmallTest.raw" );


        //            terr.Enabled = true;
        //            terr.Visible = false;
        //            TestServerClientMain.instance.XNAGame.Components.Add( terr );

        //            terr.LoadFromDisk();


        //            water = new MHGameWork.TheWizards.ServerClient.XNAGeoMipMap.Water002( main );
        //            //water.Load( main.XNAGame.Graphics.GraphicsDevice );

        //            //water.World = Matrix.CreateTranslation( -100, 0, -100 ) * Matrix.CreateScale( 20 ) * Matrix.CreateTranslation( 0, 50, 0 );
        //            //backwidth = device.ScissorRectangle.Width;

        //            skydome = new Skydome( main );
        //        },
        //        delegate
        //        {
        //            if ( skydome.device == null ) skydome.Load( main.XNAGame.Graphics.GraphicsDevice );
        //            if ( device.ScissorRectangle.Width != backwidth )
        //            {
        //                water.Load( device );

        //                backwidth = device.ScissorRectangle.Width;
        //            }
        //            water.elapsedTime += main.ProcessEventArgs.Elapsed / 100.0f;
        //            skydome.elapsedTime = water.elapsedTime;

        //            device.RenderState.CullMode = CullMode.None;


        //            water.DrawReflectionMap( delegate
        //            {
        //                skydome.RenderSkyDome();
        //                terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //                terr.CameraPostion = main.ActiveCamera.CameraPosition;

        //                terr.Update();

        //                terr.Draw();

        //            } );


        //            terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //            terr.CameraPostion = main.ActiveCamera.CameraPosition;

        //            terr.Update();


        //            water.DrawRefractionMap( delegate
        //            {
        //                terr.Draw();

        //            } );


        //            main.XNAGame.Graphics.GraphicsDevice.Clear( ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1.0f, 0 );

        //            if ( main.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.L ) )
        //            { water.effect.Load( main.XNAGame._content ); skydome.effect.Load( main.XNAGame._content ); }
        //            /*if ( !main.ProcessEventArgs.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.F ) )
        //            {

        //                main.SpriteBatch.Begin();
        //                main.SpriteBatch.Draw( water.reflectionMap, new Vector2( 50, 50 ), Color.White );
        //                main.SpriteBatch.End();


        //            }
        //            else
        //            {*/
        //            /*terr.Frustum = main.ActiveCamera.CameraInfo.Frustum;

        //            terr.CameraPostion = main.ActiveCamera.CameraPosition;*/

        //            skydome.RenderSkyDome();
        //            terr.Draw();
        //            water.RenderWater();
        //            //}
        //        }
        //    );
        //}

	}
}
