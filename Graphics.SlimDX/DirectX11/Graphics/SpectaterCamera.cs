using System;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Input;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    /// <summary>
    /// Responsible for interpreting user input for a spectator camera
    /// Responsible for construction camera matrices for the spectator camera
    /// </summary>
    public class SpectaterCamera : ICamera
    {
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
            set { nearClip = value; calculateProjection(); }
        }

        private float farClip;
        public float FarClip
        {
            get { return farClip; }
            set { farClip = value; calculateProjection(); }
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
                calculateProjection();
            }
        }

        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                calculateProjection();
            }
        }

        public SpectaterCamera(float nearPlane, float farPlane)
        {
            enabled = true;
            nearClip = nearPlane;
            farClip = farPlane;
            fieldOfView = MathHelper.ToRadians(45.0f);
            aspectRatio = 4 / 3f;
            view = Matrix.Identity;//Matrix.CreateLookAt(new Vector3(0, 0, -4000), Vector3.Zero, Vector3.Up);
            calculateProjection();
            CameraUp = MathHelper.Up;
            CameraDirection = MathHelper.Forward;
            CameraPosition = new Vector3(0.1f, 0.1f, 0.1f);
            AngleVertical = 0;
            AngleHorizontal = 0;
            AngleRoll = 0;
            enableUserInput = true;
            MovementSpeed = 10;





            UpdateCameraInfo();

        }
        private void calculateProjection()
        {
            projection = Matrix.PerspectiveFovRH(fieldOfView,
                //4 / 3F, 1.23456f, 10000.0f );
        aspectRatio, nearClip, farClip);

        }


        public SpectaterCamera()
            : this(0.1f, 400.0f)
        {
            mChanged = true;
        }




        public Vector3 Positie
        {
            get { return CameraPosition; }
            set { CameraPosition = value; }
        }

        private Vector3 _snelheid;
        public float MovementSpeed { get; set; }

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
                view = Matrix.LookAtRH(this.vLookEye, this.vLookAt, this.vLookUp);
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
        /// <summary>
        /// TODO: WARNING setter is bugged!
        /// </summary>
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

                AngleVertical = (float)Math.Acos(Vector3.Dot(groundProj, value));

                if (value.Y < 0) AngleVertical = -AngleVertical;

                Vector3 source = new Vector3(0f, 0f, 1f);
                float horizontal = (float)Math.Acos(Vector3.Dot(groundProj, source));
                if (groundProj.X > 0) horizontal = -horizontal; // This is inverted since the angles are inverted in the angle properties to matrix conversion?
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
                    Matrix sourceMatrix = Matrix.RotationYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
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
                Matrix sourceMatrix = Matrix.RotationYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
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
                    Matrix sourceMatrix = Matrix.RotationYawPitchRoll(-this.angleY, -this.angleX, -this.angleZ);
                    Vector3 source = new Vector3(0f, 0f, 1f);
                    setCameraDirectionInternal(Vector3.TransformNormal(source, sourceMatrix));
                    source = new Vector3(0f, 1f, 0f);
                    this.CameraUp = Vector3.TransformNormal(source, sourceMatrix);
                    //}
                }
            }
        }


        #region ICamera Members

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public Matrix ViewProjection
        {
            get { return viewProjection; }
        }

        public Matrix ViewInverse
        {
            get { return viewInverse; }
        }

        #endregion




        public void Update(float elapsed, TWKeyboard keyboard, TWMouse mouse)
        {
            if (!Enabled) return;


            processUserInput(elapsed, keyboard, mouse);

            UpdateCameraInfo();
        }

        private void processUserInput(float elapsed, TWKeyboard keyboard, TWMouse mouse)
        {
            if (!enableUserInput) return;
            Vector3 vSnelheid = new Vector3();

            if (keyboard.IsKeyDown(Key.S)) { vSnelheid += MathHelper.Forward; }
            if (keyboard.IsKeyDown(Key.W)) { vSnelheid += MathHelper.Backward; }
            if (keyboard.IsKeyDown(Key.A)) { vSnelheid += MathHelper.Right; }
            if (keyboard.IsKeyDown(Key.D)) { vSnelheid += MathHelper.Left; }
            if (keyboard.IsKeyDown(Key.Space)) { vSnelheid += MathHelper.Up; }
            if (keyboard.IsKeyDown(Key.LeftControl)) { vSnelheid += MathHelper.Down; }




            vSnelheid = Vector3.TransformCoordinate(vSnelheid, Matrix.RotationYawPitchRoll(-AngleHorizontal, -AngleVertical, -AngleRoll));

            if (vSnelheid.Length() != 0) vSnelheid.Normalize();

            if (keyboard.IsKeyDown(Key.T))
            {
                vSnelheid *= 30;
            }
            if (keyboard.IsKeyDown(Key.LeftShift))
            {
                vSnelheid *= 5;
            }

            vSnelheid *= MovementSpeed;


            Snelheid = vSnelheid;


            Positie += Snelheid * elapsed;
            CameraPosition = Positie;



            ProcessMouseInput(mouse.RelativeX, mouse.RelativeY);
        }

        public void ProcessMouseInput(float mouseRelativeX, float mouseRelativeY)
        {
            if (mouseRelativeX != 0)
            {
                AngleHorizontal += MathHelper.ToRadians(mouseRelativeX);
                if (AngleHorizontal > MathHelper.TwoPi)
                {
                    AngleHorizontal -= MathHelper.TwoPi;
                }
                if (AngleHorizontal < 0)
                {
                    AngleHorizontal += MathHelper.TwoPi;
                }
            }
            if (mouseRelativeY != 0)
            {
                //TODO: wasda hieronder
                if (MathHelper.ToRadians(mouseRelativeY) < MathHelper.PiOver2)
                {
                }
                ;
                AngleVertical = MathHelper.Clamp(AngleVertical - MathHelper.ToRadians(mouseRelativeY), -MathHelper.PiOver2,
                                                 MathHelper.PiOver2);
            }
        }


        public void FitInView(BoundingSphere sphere)
        {
            Vector3 pos = sphere.Center;

            float distanceToCenter = sphere.Radius / (float)Math.Sin(fieldOfView / 2);
            CameraPosition = pos - CameraDirection * distanceToCenter;
        }
    }
}
