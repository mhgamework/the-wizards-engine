using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MHGameWork.TheWizards.Graphics;


namespace MHGameWork.TheWizards.ServerClient
{
    public class SpectaterCamera : ICamera, IXNAObject
    {
        IXNAGame game;

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


        private float nearClip;
        public float NearClip
        {
            get { return nearClip; }
            set { nearClip = value; CalculateProjection(); }
        }

        private float farClip;
        public float FarClip
        {
            get { return farClip; }
            set { farClip = value; CalculateProjection(); }
        }

        private bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private bool enableUserInput;
        public bool EnableUserInput
        {
            get { return enableUserInput; }
            set { enableUserInput = value; }
        }

        private float fieldOfView;
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                fieldOfView = value;
                CalculateProjection();
            }
        }

        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                CalculateProjection();
            }
        }

        public SpectaterCamera(IXNAGame nGame, float nearPlane, float farPlane)
        {
            game = nGame;
            enabled = true;
            game.AddXNAObject(this);
            nearClip = nearPlane;
            farClip = farPlane;

            fieldOfView = MathHelper.ToRadians(45.0f);
            aspectRatio = 4 / 3f;
            view = Matrix.Identity;//Matrix.CreateLookAt(new Vector3(0, 0, -4000), Vector3.Zero, Vector3.Up);
            CalculateProjection();
            CameraUp = Vector3.Up;
            CameraDirection = Vector3.Forward;
            CameraPosition = new Vector3(0.1f, 0.1f, 0.1f);
            AngleVertical = 0;
            AngleHorizontal = 0;
            AngleRoll = 0;
            enableUserInput = true;

            


            UpdateCameraInfo();

        }
        private void CalculateProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView,
                //4 / 3F, 1.23456f, 10000.0f );
        aspectRatio, nearClip, farClip);

        }


        public SpectaterCamera(IXNAGame nGame)
            : this(nGame, 0.1f, 400.0f)
        {
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


        public void UpdateCameraInfo()
        {
            if (mChanged)
            {
                mChanged = false;
                view = Matrix.CreateLookAt(this.vLookEye, this.vLookAt, this.vLookUp);
                //_cameraInfo.Frustum = new BoundingFrustum( _cameraInfo.ViewMatrix * _cameraInfo.ProjectionMatrix );

                viewProjection = view * projection;
                viewInverse = Matrix.Invert(view);

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
                this.vLookAt = Vector3.Add(this.vLookEye, this.vLookDir);
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

                setCameraDirectionInternal(value);

                Vector3 groundProj = value;
                groundProj.Y = 0;
                groundProj.Normalize();
                if (Math.Abs(groundProj.X) < float.Epsilon && Math.Abs(groundProj.Z) < float.Epsilon)
                {
                    // Straight Up or down
                    AngleVertical = 0;
                }
                else
                {
                    AngleVertical = (float)Math.Acos(Vector3.Dot(groundProj, value));
                }
                Vector3 source = new Vector3(0f, 0f, 1f);
                float horizontal = (float)Math.Acos(Vector3.Dot(groundProj, source));
                if (groundProj.X < 0) horizontal = -horizontal;
                AngleHorizontal = horizontal;

                AngleRoll = 0;



            }
        }
        private void setCameraDirectionInternal(Vector3 value)
        {
            this.vLookDir = value;
            this.mChanged = true;
            this.vLookAt = Vector3.Add(this.vLookEye, this.vLookDir);
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
                if (value != this.angleX)
                {
                    this.angleX = value;
                    this.mChanged = true;
                    /*if (this.curStyle == CameraStyle.PositionBased)
                    {*/
                    Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                    Vector3 source = new Vector3(0f, 0f, 1f);
                    setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                    source = new Vector3(0f, 1f, 0f);
                    this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
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
                Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                Vector3 source = new Vector3(0f, 0f, 1f);
                setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                source = new Vector3(0f, 1f, 0f);
                this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
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
                if (value != this.angleY)
                {
                    this.angleY = value;
                    this.mChanged = true;
                    /*if (this.curStyle == CameraStyle.PositionBased)
                    {*/
                    Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                    Vector3 source = new Vector3(0f, 0f, 1f);
                    setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                    source = new Vector3(0f, 1f, 0f);
                    this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
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



        #region IXNAObject Members

        public void Initialize(IXNAGame game)
        {
        }

        public void Render(IXNAGame game)
        {
        }

        public void Update(IXNAGame game)
        {
            if (!Enabled) return;


            processUserInput(game);

            UpdateCameraInfo();
        }

        private void processUserInput(IXNAGame game)
        {
            if (!enableUserInput) return;
            Vector3 vSnelheid = new Vector3();

            if (game.Keyboard.IsKeyDown(Keys.S)) { vSnelheid += Vector3.Forward; }
            if (game.Keyboard.IsKeyDown(Keys.Z)) { vSnelheid += Vector3.Backward; }
            if (game.Keyboard.IsKeyDown(Keys.Q)) { vSnelheid += Vector3.Right; }
            if (game.Keyboard.IsKeyDown(Keys.D)) { vSnelheid += Vector3.Left; }
            if (game.Keyboard.IsKeyDown(Keys.Space)) { vSnelheid += Vector3.Up; }
            if (game.Keyboard.IsKeyDown(Keys.LeftControl)) { vSnelheid += Vector3.Down; }




            vSnelheid = Vector3.Transform(vSnelheid, Matrix.CreateFromYawPitchRoll(-AngleHorizontal, -AngleVertical, -AngleRoll));

            if (vSnelheid.Length() != 0) vSnelheid.Normalize();

            if (game.Keyboard.IsKeyDown(Keys.T))
            {
                vSnelheid *= 300;
            }
            if (game.Keyboard.IsKeyDown(Keys.LeftShift))
            {
                vSnelheid *= 50;
            }
            else
            {
                vSnelheid *= 10;
            }


            Snelheid = vSnelheid;






            if (game.Mouse.RelativeX != 0)
            {
                AngleHorizontal += MathHelper.ToRadians(game.Mouse.RelativeX);
                if (AngleHorizontal > MathHelper.TwoPi) { AngleHorizontal -= MathHelper.TwoPi; }
                if (AngleHorizontal < 0) { AngleHorizontal += MathHelper.TwoPi; }

            }
            if (game.Mouse.RelativeY != 0)
            {
                if (MathHelper.ToRadians(game.Mouse.RelativeY) < MathHelper.PiOver2) { };
                AngleVertical = MathHelper.Clamp(AngleVertical - MathHelper.ToRadians(game.Mouse.RelativeY), -MathHelper.PiOver2, MathHelper.PiOver2);

            }



            Positie += Snelheid * game.Elapsed;
            CameraPosition = Positie;
        }

        #endregion



        public void FitInView(BoundingSphere sphere)
        {
            Vector3 pos = sphere.Center;

            float distanceToCenter = sphere.Radius / (float)Math.Sin(fieldOfView / 2);
            CameraPosition = pos - CameraDirection * distanceToCenter;
        }
    }
}
