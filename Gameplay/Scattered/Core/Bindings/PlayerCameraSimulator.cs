using System.Linq;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Simulators;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class PlayerCameraSimulator : ISimulator
    {
        private readonly ScatteredPlayer player;

        private Entity camEntity;
        private ThirdPersonCameraSimulator firstPersonSim;

        public PlayerCameraSimulator(ScatteredPlayer player)
        {
            this.player = player;
            camEntity = new Entity();
            firstPersonSim = new ThirdPersonCameraSimulator();
        }

        public void Simulate()
        {
            var camInfo = TW.Data.Get<CameraInfo>();

            if (player.FlyingIsland == null)
                updateMovementCamera(camInfo);
            else
                updateFlightCamera(camInfo);

            firstPersonSim.Simulate();

        }

        private void updateFlightCamera(CameraInfo camInfo)
        {
            TW.Graphics.SpectaterCamera.Enabled = false;
            camInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            camInfo.ThirdPersonCameraTarget = camEntity;

            var cam = camInfo.ActiveCamera as ThirdPersonCamera;
            if (cam != null)
            {
                cam.FarClip = 2000;
                cam.AspectRatio = 1280 / 720f;
            }




            camEntity.WorldMatrix =
                Matrix.Translation(
                    player.FlyingIsland.GetIslandsInCluster().Aggregate(new Vector3(), (acc, el) => acc + el.Position) /
                    player.FlyingIsland.GetIslandsInCluster().Count());
        }

        private static void updateMovementCamera(CameraInfo camInfo)
        {
            camInfo.Mode = CameraInfo.CameraMode.Specator;
            camInfo.ActiveCamera = TW.Graphics.SpectaterCamera;
            TW.Graphics.SpectaterCamera.Enabled = true;
        }

    }
}