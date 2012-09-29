﻿using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Simulators
{
    public class ThirdPersonCameraSimulator : ISimulator
    {
        private CameraInfo info;
        private ThirdPersonCamera cam;


        public ThirdPersonCameraSimulator()
        {
            cam = new ThirdPersonCamera();

            info = TW.Data.GetSingleton<CameraInfo>();
        }

        public void Simulate()
        {
            if (info.Mode != CameraInfo.CameraMode.FirstPerson)
                return;

            cam.Target = Vector3.TransformCoordinate(new Vector3(), info.FirstPersonCameraTarget.WorldMatrix);
            cam.Update(TW.Graphics);

            info.ActiveCamera = cam;

        }
    }
}
