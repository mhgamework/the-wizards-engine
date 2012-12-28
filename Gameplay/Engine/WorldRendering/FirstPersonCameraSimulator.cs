using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    /// <summary>
    /// Simulato
    /// </summary>
    public class FirstPersonCameraSimulator : ISimulator
    {
        private CameraInfo info;

        private SpectaterCamera cam;


        private VoxelTerrain terrain;



        private FirstPersonCamera camData;
        private List<Vector3> points;

        public FirstPersonCameraSimulator()
        {
            info = TW.Data.GetSingleton<CameraInfo>();
            cam = new SpectaterCamera(TW.Graphics.Keyboard, TW.Graphics.Mouse);
            terrain = TW.Data.GetSingleton<VoxelTerrain>();
            camData = TW.Data.GetSingleton<FirstPersonCamera>();

            cam.CameraPosition = camData.Position;
            camData.LookDir -= Vector3.UnitX * camData.LookDir.X * 2;
            cam.CameraDirection = camData.LookDir;
            cam.UpdateCameraInfo();

            cam.AngleHorizontal = cam.AngleHorizontal;


            points = new List<Vector3>();
            points.Add(Vector3.UnitX * 0.5f);
            points.Add(Vector3.UnitY * 0.5f);
            points.Add(Vector3.UnitZ * 0.5f);
            points.Add(-Vector3.UnitX * 0.5f);
            points.Add(-Vector3.UnitY * 0.5f);
            points.Add(-Vector3.UnitY * 1.5f);
            points.Add(-Vector3.UnitZ * 0.5f);
        }

        public void Simulate()
        {
            if (info.Mode != CameraInfo.CameraMode.FirstPerson)
                return;



            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Space))
            {
                camData.JumpVelocity += 3;
            }

            camData.JumpVelocity = MathHelper.Clamp(camData.JumpVelocity ,  -100, 30);

            camData.JumpVelocity -= 9.81f * TW.Graphics.Elapsed;

            var voxelDown = terrain.GetVoxelAt(camData.Position - Vector3.UnitY);
            

            camData.JumpHeight += camData.JumpVelocity * TW.Graphics.Elapsed;



            info.ActiveCamera = cam;

            cam.Update(TW.Graphics.Elapsed);

            var newPos = cam.CameraPosition;
            newPos += camData.JumpVelocity * TW.Graphics.Elapsed * Vector3.UnitY;

            for (int i = 0; i < points.Count; i++)
            {
                var delta = camData.Position - newPos;

                var dir = points[i];



                var voxel = terrain.GetVoxelAt(newPos + dir);

                if (voxel != null)
                {

                    //TW.Graphics.LineManager3D.AddBox(new BoundingBox(voxel.Position + new Vector3(30, 0, 0), voxel.Position + new Vector3(30, 0, 0) + MathHelper.One), new Color4(1, 1, 0));
                }
                if (voxel != null && voxel.Filled)
                {
                    dir = Vector3.Normalize(dir);
                    var axisDelta = Vector3.Dot(delta, dir);

                    if (axisDelta > -0.01)
                    {
                        axisDelta = -0.01f;
                    }

                    newPos += axisDelta * dir;
                    //TW.Graphics.LineManager3D.AddCenteredBox(newPos + dir, 0.1f, new Color4(Color.Green));
                }
                else
                {
                    //TW.Graphics.LineManager3D.AddCenteredBox(newPos + dir, 0.1f, new Color4(Color.Red));
                }
            }

            if (newPos.Y < 1.5f)
            {
                newPos.Y = 1.5f;
                camData.JumpVelocity = 0;
            }


            if (Math.Abs(newPos.Y - camData.Position.Y ) < 0.000001f)
            {
                camData.JumpVelocity = 0;
            }

            camData.Position = newPos;
            camData.LookDir = cam.CameraDirection;

            cam.CameraPosition = camData.Position;
            cam.UpdateCameraInfo();







        }
    }
}
