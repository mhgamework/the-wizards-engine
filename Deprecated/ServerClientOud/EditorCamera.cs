using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace MHGameWork.TheWizards.ServerClient
{
    public class EditorCamera : ICamera
    {
        public enum MoveMode
        {
            None = 0,
            MoveXZ,
            RotateYawRoll,
            MoveY,
            Orbit

        }
        private IXNAGame game;

        public string Tag;

        private MoveMode activeMoveMode;

        public MoveMode ActiveMoveMode
        {
            get { return activeMoveMode; }
            set { activeMoveMode = value; }
        }


        private bool enabled = true;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }


        private Matrix view;
        public Matrix View
        {
            get { return view; }
        }

        private Matrix projection;
        public Matrix Projection
        {
            get { return projection; }
        }

        private Matrix viewProjection;
        public Matrix ViewProjection
        {
            get { return viewProjection; }
        }

        private Matrix viewInverse;
        public Matrix ViewInverse
        {
            get { return viewInverse; }
        }

        private Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        private Quaternion orientation;

        public Quaternion Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        private float moveSpeed;

        public float MoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }

        private float rotateSpeed;

        public float RotateSpeed
        {
            get { return rotateSpeed; }
            set { rotateSpeed = value; }
        }

        private Vector3 orbitPoint;

        public Vector3 OrbitPoint
        {
            get { return orbitPoint; }
            set { orbitPoint = value; }
        }

        private float orbitSpeed;

        public float OrbitSpeed
        {
            get { return orbitSpeed; }
            set { orbitSpeed = value; }
        }

        private bool orbitLookAt = true;

        /// <summary>
        /// When true, the camera is forced to look at the orbit point during orbit
        /// </summary>
        public bool OrbitLookAt
        {
            get { return orbitLookAt; }
            set { orbitLookAt = value; }
        }

        private float nearClip;
        public float NearClip
        {
            get { return nearClip; }
            set
            {
                nearClip = value;
                CalculateProjectionMatrix();
            }
        }

        private float farClip;
        public float FarClip
        {
            get { return farClip; }
            set
            {
                farClip = value;
                CalculateProjectionMatrix();
            }
        }

        private float aspectRatio;

        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                CalculateProjectionMatrix();
            }
        }


        public EditorCamera(TWEditor nEditor)
            : this(nEditor.Game)
        {

        }

        public EditorCamera(IXNAGame nGame)
        {
            game = nGame;
            nearClip = 0.1f;
            farClip = 5000f;
            aspectRatio = (float)4 / 3;
            //editor = nEditor; ;
            //projection = Matrix.CreateOrthographic(400, 200, 0.1f, 5000f );
            CalculateProjectionMatrix();

            position = new Vector3(0, 10, 5);
            orientation = Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver4, 0);
            rotateSpeed = 0.03f;
            orbitSpeed = 0.02f;
            moveSpeed = 1;
            //orientation = Quaternion.Identity;
            CalculateMatrices();
        }

        private void CalculateProjectionMatrix()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, nearClip, farClip);
        }

        private void CalculateMatrices()
        {

            Vector3 dir = Vector3.Transform(Vector3.Forward, orientation);
            Vector3 up = Vector3.Transform(Vector3.Up, orientation); ;
            view = Matrix.CreateLookAt(position, position + dir, Vector3.Up);
            viewProjection = view * projection;
            viewInverse = Matrix.Invert(view);
        }

        /// <summary>
        /// Returns a rotation quaternion that makes the camera in its current position
        /// look at a given point.
        /// </summary>
        /// <returns></returns>
        public Quaternion CreateLookAt(Vector3 target)
        {
            //TODO: use MathExtra.Functions.CreateFromLookDir

            Vector3 pos = position;
            //Vector3 pos = new Vector3( 1, 1, 1 );
            Vector3 dir = target - pos;
            dir.Normalize();

            // now some magic trigoniometry
            // TODO: This probably can be done faster

            // dir.y ^ 2 + radius ^ 2 = dir.lengthsquared
            float radius = (float)Math.Sqrt(1 - dir.Y * dir.Y);

            float angleY;
            float angleX;
            if (radius < 0.0001)
            {
                if (dir.Y > 0)
                {
                    angleY = 0;
                    angleX = MathHelper.PiOver2;
                }
                else
                {
                    angleY = 0;
                    angleX = -MathHelper.PiOver2;
                }

            }
            else
            {

                angleY = (float)Math.Acos(MathHelper.Clamp(-dir.Z / radius, -1, 1));
                angleX = (float)Math.Asin(MathHelper.Clamp(dir.Y, -1, 1));



                if (dir.X > 0) angleY = -angleY;
                //if ( dir.Z > 0 && dir.Y > 0 ) angleX = angleX + MathHelper.PiOver2;
                //if ( dir.Z > 0 && dir.Y < 0 ) angleX = angleX - MathHelper.PiOver2;

            }

            Quaternion q;
            q = Quaternion.CreateFromYawPitchRoll(angleY, angleX, 0);

            //Since i was stupid enough to design this algoritm with the base vector vector.Right
            // i need to add this line to get vector3.forward
            //q = q * Quaternion.CreateFromAxisAngle( Vector3.Up, -MathHelper.PiOver2 );

#if (DEBUG)
            Vector3 newDir = Vector3.Transform(Vector3.Forward, q);
            if (!VectorsEqual(dir, newDir)) throw new Exception("This algoritm doesnt work!");
#endif
            return q;
        }

        private bool VectorsEqual(Vector3 v1, Vector3 v2)
        {
            //TODO: use MathExtra.Functions.VectorsEqual

            Vector3 diff = v1 - v2;
            if (Math.Abs(diff.X) > 0.01) return false;
            if (Math.Abs(diff.Y) > 0.01) return false;
            if (Math.Abs(diff.Z) > 0.01) return false;

            return true;
        }

        private void MoveY()
        {
            Position += Vector3.Up * (Game.Mouse.RelativeY * moveSpeed);
        }
        private void RotateYawRoll()
        {
            if (Game.Mouse.RelativeX != 0.0f)
            {
                orientation = Quaternion.CreateFromAxisAngle(Vector3.Up, Game.Mouse.RelativeX * rotateSpeed) * orientation;
            }
            if (Game.Mouse.RelativeY != 0.0f)
            {
                orientation = Quaternion.CreateFromAxisAngle(Vector3.Transform(Vector3.Right, orientation), Game.Mouse.RelativeY * rotateSpeed) * orientation;

            }

        }

        private void MoveXZ()
        {
            Vector3 forward = Vector3.Transform(Vector3.Forward, orientation);
            Vector3 right = Vector3.Transform(Vector3.Right, orientation);

            forward.Y = 0;
            right.Y = 0;

            forward.Normalize();
            right.Normalize();

            //if ( Game.Mouse.RelativeY != 0 ) throw new Exception();
            position += forward * Game.Mouse.RelativeY * moveSpeed;
            position += right * -Game.Mouse.RelativeX * moveSpeed;
        }

        private void Orbit()
        {
            // Create a matrix that first moves the camera so that the orbitPoint
            // is at 0,0,0. Then rotate the camera and move back to the original space.

            Quaternion lookAt = CreateLookAt(orbitPoint);

            Matrix m;
            m = Matrix.CreateTranslation(-orbitPoint);
            if (Game.Mouse.RelativeX != 0)
            {
                m *= Matrix.CreateFromYawPitchRoll(orbitSpeed * Game.Mouse.RelativeX, 0, 0);
            }
            if (Game.Mouse.RelativeY != 0)
            {
                m *= Matrix.CreateFromAxisAngle(Vector3.Transform(Vector3.Right, lookAt), orbitSpeed * Game.Mouse.RelativeY);
            }
            m *= Matrix.CreateTranslation(orbitPoint);


            if (orbitLookAt)
            {
                position = Vector3.Transform(position, m);
                orientation = CreateLookAt(orbitPoint);
            }
            else
            {
                Vector3 dir = Vector3.Transform(Vector3.Forward, orientation);
                dir.Normalize();
                Vector3 target = position + dir;
                target = Vector3.Transform(target, m);
                //game.LineManager3D.AddCenteredBox( target, 0.05f, Color.Orange );

                position = Vector3.Transform(position, m);

                orientation = CreateLookAt(target);



            }



        }


        //****
        //* Different camera styles. can be implemented later
        //**
        //*
        //if ( e.Mouse.MouseState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
        //{
        //    GuiPanel obj = guiRoot.FindClickedObject( cursor.GetClickPoint() );

        //    if ( obj == viewPanel )
        //    {
        //        camera.CameraPosition += camera.CameraUp * e.Mouse.RelativeY * 10;
        //        camera.CameraPosition += Vector3.Cross( camera.CameraUp, camera.CameraDirection ) * e.Mouse.RelativeX * 10;
        //    }
        //}
        //if ( e.Mouse.MouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed )
        //{
        //    Quaternion rot = Quaternion.Identity;
        //    if ( e.Mouse.RelativeX != 0.0f )
        //    {
        //        rot = Quaternion.CreateFromAxisAngle( camera.CameraUp, e.Mouse.RelativeX * 0.01f ) * rot;
        //    }
        //    if ( e.Mouse.RelativeY != 0.0f )
        //    {
        //        rot = Quaternion.CreateFromAxisAngle( Vector3.Cross( camera.CameraUp, camera.CameraDirection ), -e.Mouse.RelativeY * 0.01f ) * rot;
        //    }

        //    camera.CameraDirection = Vector3.Transform( camera.CameraDirection, rot );
        //    camera.CameraUp = Vector3.Transform( camera.CameraUp, rot );
        //}



        //    }



        public Vector3 CalculateDirection()
        {
            Vector3 newDir = Vector3.Transform(Vector3.Forward, orientation);
            return newDir;
        }

        public void Update()
        {
            switch (activeMoveMode)
            {
                case MoveMode.MoveXZ:
                    MoveXZ();
                    break;
                case MoveMode.MoveY:
                    MoveY();
                    break;
                case MoveMode.RotateYawRoll:
                    RotateYawRoll();
                    break;
                case MoveMode.Orbit:
                    Orbit();
                    break;
            }

#if (DEBUG)
            if (float.IsNaN(orientation.X)) throw new Exception("Error in camera code!");
#endif


            //Update the matrices
            CalculateMatrices();

            moveSpeed = Math.Abs(position.Y) * 0.05f;
            moveSpeed = MathHelper.Clamp(moveSpeed, 0.01f, 1000f);
        }

        public bool UpdateCameraMoveModeDefaultControls()
        {
            if (!Enabled) return false;
            if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
            {


                if (game.Mouse.LeftMousePressed && game.Mouse.RightMousePressed)
                {

                    ActiveMoveMode = EditorCamera.MoveMode.MoveY;
                }
                else if (game.Mouse.LeftMousePressed)
                {
                    ActiveMoveMode = EditorCamera.MoveMode.MoveXZ;
                }
                else if (game.Mouse.RightMousePressed)
                {
                    if (!game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                    {
                        ActiveMoveMode = EditorCamera.MoveMode.Orbit;
                        //if ( Mouse.CursorEnabled ) camera.OrbitPoint =
                        //      RaycastWereld( ImgWereldView.Size * 0.5f );

                    }
                    else
                    {
                        ActiveMoveMode = EditorCamera.MoveMode.RotateYawRoll;
                    }
                }
                else
                {
                    ActiveMoveMode = EditorCamera.MoveMode.None;
                }
            }
            else
            {
                ActiveMoveMode = EditorCamera.MoveMode.None;

            }

            if (ActiveMoveMode == EditorCamera.MoveMode.None)
            {
                game.Mouse.CursorEnabled = true;
            }
            else
            {
                game.Mouse.CursorEnabled = false;
                return true;
            }
            return false;
        }

        public IXNAGame Game { get { return game; } }

    }
}
