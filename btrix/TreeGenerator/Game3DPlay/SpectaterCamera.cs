using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace MHGameWork.TheWizards.ServerClient
{
    public class SpectaterCamera : ICamera, IGameObject
    {
        XNAGame game;

        Matrix view;
        Matrix projection;
        Matrix viewProjection;
        Matrix viewInverse;
        bool mChanged = true;



        public float angleX;
        public float angleY;
        public float angleZ;
        private Vector3 vLookAt;
        private Vector3 vLookDir;
        private Vector3 vLookEye;
        private Vector3 vLookUp;

        public SpectaterCamera( XNAGame nGame )
        {
            game = nGame;
            game.AddGameObject( this );

            view = Matrix.Identity;//Matrix.CreateLookAt(new Vector3(0, 0, -4000), Vector3.Zero, Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView( MathHelper.ToRadians( 45.0f ),
                     4 / 3, 1.23456f, 10000.0f );

            CameraUp = Vector3.Up;
            CameraDirection = Vector3.Forward;
            CameraPosition = new Vector3( 0, 0, 0 );



            UpdateCameraInfo();

        }




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


        public void Render()
        {

        }

        public void Process()
        {
            //return;
            // With Me.HoofdObj.DevContainer.DX.Input


            Vector3 vSnelheid = new Vector3();
            //Vector3 direction = new Vector3();

            if ( game.Keyboard.IsKeyDown( Keys.S ) ) { vSnelheid += Vector3.Forward; }
            if ( game.Keyboard.IsKeyDown( Keys.Z ) ) { vSnelheid += Vector3.Backward; }
            if ( game.Keyboard.IsKeyDown( Keys.Q ) ) { vSnelheid += Vector3.Right; }
            if ( game.Keyboard.IsKeyDown( Keys.D ) ) { vSnelheid += Vector3.Left; }
            if ( game.Keyboard.IsKeyDown( Keys.Space ) ) { vSnelheid += Vector3.Up; }
            if ( game.Keyboard.IsKeyDown( Keys.LeftControl ) ) { vSnelheid += Vector3.Down; }


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
            //    nSnelheid.Ad(Dir)
            //End If

            vSnelheid = Vector3.Transform( vSnelheid, Matrix.CreateFromYawPitchRoll( -AngleHorizontal, -AngleVertical, -AngleRoll ) );

            if ( vSnelheid.Length() != 0 ) vSnelheid.Normalize();
            //If .KeyPressed(Me.KnopLopen) Then
            //    nSnelheid.Multiply(CSng(Me.BewegingsSnelheidLopen))
            //ElseIf .KeyPressed(DirectInput.Key.T) Then
            //    nSnelheid.Multiply(120)
            //Else
            //    nSnelheid.Multiply(CSng(Me.BewegingsSnelheid))
            if ( game.Keyboard.IsKeyDown( Keys.T ) )
            {
                vSnelheid *= 300;
            }
            if ( game.Keyboard.IsKeyDown( Keys.LeftShift ) )
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

            if ( game.Mouse.RelativeX != 0 )
            {
                AngleHorizontal += MathHelper.ToRadians( game.Mouse.RelativeX );

                //AngleHorizontal = MathHelper.Clamp(AngleHorizontal, 0, MathHelper.TwoPi);
                //AngleHorizontal = CSng(Me.Camera.AngleHorizontal Mod (2 * Math.PI));
                if ( AngleHorizontal > MathHelper.TwoPi ) { AngleHorizontal -= MathHelper.TwoPi; }
                if ( AngleHorizontal < 0 ) { AngleHorizontal += MathHelper.TwoPi; }

            }
            if ( game.Mouse.RelativeY != 0 )
            {

                //Me.Camera.AngleVertical -= .MouseState.Y * CSng(Math.PI / 180)

                if ( MathHelper.ToRadians( game.Mouse.RelativeY ) < MathHelper.PiOver2 ) { };
                AngleVertical = MathHelper.Clamp( AngleVertical - MathHelper.ToRadians( game.Mouse.RelativeY ), -MathHelper.PiOver2, MathHelper.PiOver2 );

            }





            //'Me.Camera.AngleHorizontal -= CSng(.MouseState.X) * 2
            //'Me.Camera.AngleVertical -= CSng(.MouseState.Y * 2)
            //Me.Camera.Process()
            Positie += Snelheid * game.Elapsed;
            CameraPosition = Positie;

            UpdateCameraInfo();

        }




        public void UpdateCameraInfo()
        {
            if ( mChanged )
            {
                mChanged = false;
                view = Matrix.CreateLookAt( this.vLookEye, this.vLookAt, this.vLookUp );
                //_cameraInfo.Frustum = new BoundingFrustum( _cameraInfo.ViewMatrix * _cameraInfo.ProjectionMatrix );

                viewProjection = view * projection;
                viewInverse = Matrix.Invert( view );

            }
        }



        public Vector3 CameraPosition
        {
            get
            {
                return this.vLookEye;
            }
            set
            {
                this.vLookEye = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                this.vLookAt = Vector3.Add( this.vLookEye, this.vLookDir );
                //}
            }
        }
        public Vector3 CameraDirection
        {
            get
            {
                return this.vLookDir;
            }
            set
            {
                this.vLookDir = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                this.vLookAt = Vector3.Add( this.vLookEye, this.vLookDir );
                //}
            }
        }
        public Vector3 CameraUp
        {
            get
            {
                return this.vLookUp;
            }
            set
            {
                this.vLookUp = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                this.mChanged = true;
                //}
            }
        }







        public float AngleVertical
        {
            get
            {
                return this.angleX;
            }
            set
            {
                if ( value != this.angleX )
                {
                    this.angleX = value;
                    this.mChanged = true;
                    /*if (this.curStyle == CameraStyle.PositionBased)
                    {*/
                    Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll( -this.angleY, -this.angleX, -this.angleZ );
                    Vector3 source = new Vector3( 0f, 0f, 1f );
                    this.CameraDirection = Vector3.TransformNormal( source, sourceMatrix );
                    source = new Vector3( 0f, 1f, 0f );
                    this.CameraUp = Vector3.TransformNormal( source, sourceMatrix );
                    //}
                }
            }
        }
        public float AngleRoll
        {
            get
            {
                return this.angleZ;
            }
            set
            {
                this.angleZ = value;
                this.mChanged = true;
                /*if (this.curStyle == CameraStyle.PositionBased)
                {*/
                Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll( -this.angleY, -this.angleX, -this.angleZ );
                Vector3 source = new Vector3( 0f, 0f, 1f );
                this.CameraDirection = Vector3.TransformNormal( source, sourceMatrix );
                source = new Vector3( 0f, 1f, 0f );
                this.CameraUp = Vector3.TransformNormal( source, sourceMatrix );
                //}
            }
        }
        public float AngleHorizontal
        {
            get
            {
                return this.angleY;
            }
            set
            {
                if ( value != this.angleY )
                {
                    this.angleY = value;
                    this.mChanged = true;
                    /*if (this.curStyle == CameraStyle.PositionBased)
                    {*/
                    Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll( -this.angleY, -this.angleX, -this.angleZ );
                    Vector3 source = new Vector3( 0f, 0f, 1f );
                    this.CameraDirection = Vector3.TransformNormal( source, sourceMatrix );
                    source = new Vector3( 0f, 1f, 0f );
                    this.CameraUp = Vector3.TransformNormal( source, sourceMatrix );
                    //}
                }
            }
        }


        #region ICamera Members

        public Microsoft.Xna.Framework.Matrix View
        {
            get { return view; }
        }

        public Microsoft.Xna.Framework.Matrix Projection
        {
            get { return projection; }
        }

        public Microsoft.Xna.Framework.Matrix ViewProjection
        {
            get { return viewProjection; }
        }

        public Microsoft.Xna.Framework.Matrix ViewInverse
        {
            get { return viewInverse; }
        }

        #endregion


    }
}
