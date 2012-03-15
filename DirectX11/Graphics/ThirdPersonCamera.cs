using System.Diagnostics;
using SlimDX;

namespace DirectX11.Graphics
{
    public class ThirdPersonCamera : ICamera
    {
        private Matrix view;
        private Matrix proj;
        private Matrix viewProj;
        private Matrix viewInverse;
        private float nearClip;
        private float farClip;

        public Vector3 Target { get; set; }

        //private Vector3 targetPoint;
        private float cameraDistance;

        private bool enabled;

        public bool Enabled
        {
            [DebuggerStepThrough]
            get { return enabled; }
            [DebuggerStepThrough]
            set { enabled = value; }
        }

        //public Vector3 TargetPoint
        //{
        //    [DebuggerStepThrough]
        //    get { return targetPoint; }
        //    [DebuggerStepThrough]
        //    set { targetPoint = value; }
        //}

        public float CameraDistance
        {
            [DebuggerStepThrough]
            get { return cameraDistance; }
            [DebuggerStepThrough]
            set { cameraDistance = value; }
        }


        private float scrollFactor;
        public float LookAngleVertical { get; private set; }
        public float LookAngleHorizontal { get; private set; }
        private float mouseSensitivity = 0.1f;
        public Vector3 CameraOffset = new Vector3(0, 1, 0);

        public ThirdPersonCamera()
        {
            scrollFactor = 0.5f;
            enabled = true;

            cameraDistance = 10;
            nearClip = 0.1f;
            farClip = 400;

            createProjectionMatrix();
            createViewMatrix();

            updateMatrices();

        }
        private void createProjectionMatrix()
        {
            proj = Matrix.PerspectiveFovRH(0.45f, 4 / 3F, nearClip, farClip);
        }

        private void createViewMatrix()
        {
            var camPos = Target + CameraOffset;
            Vector3 pos, lookDir, up;

            lookDir = MathHelper.Forward;
            lookDir = Vector3.TransformNormal(
                lookDir,
                Matrix.RotationX(LookAngleVertical) * Matrix.RotationY(LookAngleHorizontal)
                  );

            lookDir.Normalize();
            pos = camPos - lookDir * cameraDistance;

            up = MathHelper.Up;



            view = Matrix.LookAtRH(pos, camPos, up);

        }
        private void updateMatrices()
        {
            viewProj = view * proj;
            viewInverse = Matrix.Invert(view);
        }

        #region ICamera Members

        public Matrix View
        {
            get { return view; }
        }

        public Matrix Projection
        {
            get { return proj; }
        }

        public Matrix ViewProjection
        {
            get { return viewProj; }
        }

        public Matrix ViewInverse
        {
            get { return viewInverse; }
        }

        public float NearClip
        {
            get { return nearClip; }
        }

        public float FarClip
        {
            get { return farClip; }
        }

        #endregion


        public void Update(DX11Game _game)
        {
            if (enabled)
            {
                createViewMatrix();
                updateMatrices();


                if (_game.Mouse.RelativeScrollWheel != 0)
                {
                    float zoomSpeed = -1 / 10000f;

                    scrollFactor += _game.Mouse.RelativeScrollWheel * zoomSpeed;
                    scrollFactor = MathHelper.Clamp(scrollFactor, 0, 1);
                }

                if (_game.Mouse.RelativeX != 0)
                {
                    //TODO: mouse sensitivity
                    LookAngleHorizontal += _game.Mouse.RelativeX * -mouseSensitivity;
                }
                if (_game.Mouse.RelativeY != 0)
                {
                    LookAngleVertical += _game.Mouse.RelativeY * -mouseSensitivity;
                    if (LookAngleVertical < -MathHelper.PiOver2 * 0.95f) LookAngleVertical = -MathHelper.PiOver2 * 0.95f;
                    if (LookAngleVertical > MathHelper.PiOver2 * 0.95f) LookAngleVertical = MathHelper.PiOver2 * 0.95f;
                }

                // Gebruik 2degraadsvergl voor afstand: f(x) = ax^2 + bx + c
                // We zeggen dat x=0 volledig ingezoomd en x=1 volledig uitgezoomd
                // dus: minimum=p1->(0,minDist)    p->(1,maxDist)
                // Invullen geeft:
                //    []   maxDist = a + b + c
                //    []   0 = -b/(2a)
                //    []   minDist = c
                // a = maxDist - c
                // b = 0
                // c = minDist


                float minDist = 3;
                float maxDist = 100;


                float c = minDist;
                float b = 0;
                float a = maxDist - c;

                float x = scrollFactor;

                cameraDistance = a * x * x + b * x + c;


                /*
                // 3degraadsvergl
                // versnelling a, minimum=p1->(0,minDist)    p->(1,maxDist) 
                // ax^3 + cx + d
                // a = cte
                // c = max - a - d
                // d = min

                float minDist = 3;
                float maxDist = 100;
                float a = 30000;


                float d = minDist;
                float c = maxDist - a - d;

                float x = scrollFactor;

                cameraDistance = a * x * x * x + c * x + d;
                */

            }
        }
    }
}
