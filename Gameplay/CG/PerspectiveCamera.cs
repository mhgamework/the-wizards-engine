using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Visualization;
using MHGameWork.TheWizards.DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    /// <summary>
    /// 
    /// </summary>
    public class PerspectiveCamera : ICamera
    {
        private float left;
        private float right;
        private float bottom;
        private float top;
        /// <summary>
        /// Projection plane size in worldspace
        /// </summary>
        private Vector2 screenSize;


        private Vector3 rightAxis;
        private Vector3 up;
        private Vector3 direction;
        private Vector3 position;
        private float projectionPlaneDistance;

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                updateRight();
            }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                updateRight();
            }
        }

        public float ProjectionPlaneDistance
        {
            get { return projectionPlaneDistance; }
            set { projectionPlaneDistance = value; }
        }

        public Vector3 Up
        {
            get { return up; }
            set { up = value; updateRight(); }
        }


        /// <summary>
        /// Projection plane size in worldspace
        /// </summary>
        public Vector2 ScreenSize
        {
            get { return screenSize; }
            set
            {
                screenSize = value;
                updateScreenBounds();
            }
        }

        private void updateScreenBounds()
        {
            left = -screenSize.X / 2;
            right = screenSize.X / 2;
            bottom = -screenSize.Y / 2;
            top = screenSize.Y / 2;
        }

        private void updateRight()
        {
            rightAxis = Vector3.Cross(Up, Direction);
        }

        public PerspectiveCamera()
        {

            Position = new Vector3(0, 0, 0);
            ProjectionPlaneDistance = 3;

            ScreenSize = new Vector2(1, 1);

            Up = new Vector3(0, 1, 0);
            Direction = new Vector3(0, 0, -1);


        }

        public Ray CalculateRay(Vector2 point)
        {
            // TODO optimize division.
            var u = (float)(left + (right - left) * (1-point.X)); //TODO: fix this stuff
            var v = (float)(bottom + (top - bottom) * (1-point.Y)); // Invert y!!
            var ret = new Ray { Direction = ProjectionPlaneDistance * Direction + u * rightAxis + v * Up, Position = Position };
            ret.Direction = Vector3.Normalize(ret.Direction); // TODO: Slowpoke, but for simplicity
            return ret;
        }



        public static void Test()
        {
            var game = new DX11Game();
            game.InitDirectX();

            var cam = new PerspectiveCamera();

            var visualizer = new CameraVisualizer(game);

            game.GameLoopEvent += delegate
                                      {
                                          game.LineManager3D.AddRectangle(cam.Position + cam.Direction * cam.ProjectionPlaneDistance,
                                                                          new Vector2(cam.right - cam.left,
                                                                                      cam.top - cam.bottom), cam.rightAxis, cam.Up, new Color4(0, 1, 0));

                                          visualizer.RenderRays(cam, new Point2(8, 8));


                                      };
            game.Run();
        }
    }
}
