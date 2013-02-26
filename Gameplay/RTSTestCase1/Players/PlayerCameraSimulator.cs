using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    public class PlayerCameraSimulator : ISimulator
    {
        private CustomCamera camera = new CustomCamera();

        private CameraInfo cameraInfo = TW.Data.Get<CameraInfo>();
        private LocalGameData gameData = TW.Data.Get<LocalGameData>();

        public PlayerCameraSimulator()
        {
            cameraInfo.ActivateCustomCamera(camera);
        }

        public void Simulate()
        {
            camera.SetProjectionMatrix(TW.Graphics.SpectaterCamera.Projection);
            var eye = gameData.LocalPlayer.Position;
            var target = eye + gameData.LocalPlayer.LookDirection;
            camera.SetViewMatrix(Matrix.LookAtRH(eye, target, Vector3.UnitY));

            
        }
    }
}
