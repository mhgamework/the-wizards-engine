using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay;
using MHGameWork.Game3DPlay.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.SpelObjecten
{
	public class Spectater : Camera, Core.Elements.IProcessable
	{
		public Spectater(SpelObject nParent)
			: base( nParent )
		{
			Positie = new Vector3( 0, 0, 0 );
			//_processElement = new MHGameWork.Game3DPlay.Core.Elements.ProcessElement(this);




			//this.world = Matrix.Identity;
			//this.viewport = this.dx.Device.Viewport;
			//this.proj = this.dx.ProjectionMatrix;
			//this.minZoomDist = 0.01f;
			//Vector3 vector = new Vector3(0f, 0f, 0f);
			//this.planeVec = vector;
			//vector = new Vector3(0f, 1f, 0f);
			//this.planeNormal = vector;
			//this.rotSpeed = 0.01f;
			//this.zoomSpeed = 0.01f;
			//this.zoomDist = 1f;

			/*if (this.dx.Initialized)
			{
				this.InitializeDeviceObjects(null, null);
			}*/

		}
		//Core.Elements.ProcessElement _processElement;


		//private Vector3 _positie;

		public Vector3 Positie
		{
			get { return CameraPosition; }
			set { CameraPosition = value; }
		}

		private Vector3 _snelheid;

		public Vector3 Snelheid
		{
			get { return _snelheid; }
			set { _snelheid = value; }
		}





		#region IProcessable Members

		public void OnProcess(object sender, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
		{
			// With Me.HoofdObj.DevContainer.DX.Input


			Vector3 vSnelheid = new Vector3();
			//Vector3 direction = new Vector3();

			if ( e.KeyboardState.IsKeyDown( Keys.S ) ) { vSnelheid += Vector3.Forward; }
			if ( e.KeyboardState.IsKeyDown( Keys.Z ) ) { vSnelheid += Vector3.Backward; }
			if ( e.KeyboardState.IsKeyDown( Keys.Q ) ) { vSnelheid += Vector3.Right; }
			if ( e.KeyboardState.IsKeyDown( Keys.D ) ) { vSnelheid += Vector3.Left; }
			if ( e.KeyboardState.IsKeyDown( Keys.Space ) ) { vSnelheid += Vector3.Up; }
			if ( e.KeyboardState.IsKeyDown( Keys.LeftControl ) ) { vSnelheid += Vector3.Down; }


			//If .KeyPressed(Me.KnopVooruit) Then
			//    Dir = Me.Camera.CameraDirection
			//    nSnelheid.Add(Dir)
			//End If
			//If .KeyPressed(Me.KnopRechts) Then
			//    Dir = Vector3.TransformCoordinate(Me.Camera.CameraDirection, Matrix.RotationY(Math.PI / 2))
			//    nSnelheid.Add(Dir)
			//End If
			//If .KeyPressed(Me.KnopAchteruit) Then
			//    Dir = Vector3.TransformCoordinate(Me.Camera.CameraDirection, Matrix.RotationY(Math.PI))
			//    nSnelheid.Add(Dir)
			//End If
			//If .KeyPressed(Me.KnopLinks) Then
			//    Dir = Vector3.TransformCoordinate(Me.Camera.CameraDirection, Matrix.RotationY(-Math.PI / 2))
			//    nSnelheid.Add(Dir)
			//End If
			//If .KeyPressed(Me.KnopOmhoog) Then
			//    Dir = New Vector3(0, 1, 0)
			//    nSnelheid.Add(Dir)
			//End If
			//If .KeyPressed(Me.KnopOmlaag) Then
			//    Dir = New Vector3(0, -1, 0)
			//    nSnelheid.Add(Dir)
			//End If

			vSnelheid = Vector3.Transform( vSnelheid, Matrix.CreateFromYawPitchRoll( -AngleHorizontal, -AngleVertical, -AngleRoll ) );

			if ( vSnelheid.Length() != 0 ) vSnelheid.Normalize();
			//If .KeyPressed(Me.KnopLopen) Then
			//    nSnelheid.Multiply(CSng(Me.BewegingsSnelheidLopen))
			//ElseIf .KeyPressed(DirectInput.Key.T) Then
			//    nSnelheid.Multiply(120)
			//Else
			//    nSnelheid.Multiply(CSng(Me.BewegingsSnelheid))
			if ( e.KeyboardState.IsKeyDown( Keys.T ) )
			{
				vSnelheid *= 300;
			}
			if ( e.KeyboardState.IsKeyDown( Keys.LeftShift ) )
			{
				vSnelheid *= 50;
			}
			else
			{
				vSnelheid *= 10;
			}



			//End If
			//If .MousePressed(DirectInput.MouseOffset.Button0) Then
			//    nSnelheid.Multiply(50)
			//End If
			Snelheid = vSnelheid;





			////If Spel.GetInstance.DX.Input.MouseState.X <> 0 Then Stop

			if ( e.Mouse.OppositeRelativeX != 0 )
			{
				AngleHorizontal -= MathHelper.ToRadians( e.Mouse.OppositeRelativeX );

				//AngleHorizontal = MathHelper.Clamp(AngleHorizontal, 0, MathHelper.TwoPi);
				//AngleHorizontal = CSng(Me.Camera.AngleHorizontal Mod (2 * Math.PI));
				if ( AngleHorizontal > MathHelper.TwoPi ) { AngleHorizontal -= MathHelper.TwoPi; }
				if ( AngleHorizontal < 0 ) { AngleHorizontal += MathHelper.TwoPi; }

			}
			if ( e.Mouse.OppositeRelativeY != 0 )
			{

				//Me.Camera.AngleVertical -= .MouseState.Y * CSng(Math.PI / 180)

				if ( MathHelper.ToRadians( e.Mouse.OppositeRelativeY ) > -MathHelper.PiOver2 ) { };
				AngleVertical = MathHelper.Clamp( AngleVertical + MathHelper.ToRadians( e.Mouse.OppositeRelativeY ), -MathHelper.PiOver2, MathHelper.PiOver2 );

			}





			//'Me.Camera.AngleHorizontal -= CSng(.MouseState.X) * 2
			//'Me.Camera.AngleVertical -= CSng(.MouseState.Y * 2)
			//Me.Camera.Process()
			Positie += Snelheid * e.Elapsed;
			CameraPosition = Positie;

			UpdateCameraInfo();

		}

		#endregion







	}
}
