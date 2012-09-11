using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Pickup;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class Plugin : IGameplayPlugin
    {
        public void Initialize(Engine engine)
        {
            testPickupSimulator(engine);

        }

        private void testPickupSimulator(Engine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Model.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            var itemFactory = new ItemEntityFactory();

            var random = new Random();

            for(int i=0; i<10; i++)
            {
                itemFactory.CreateItemEntity(MeshFactory.Load("Core\\Crate01"), Matrix.Translation(new Vector3(random.Next(25), 0, random.Next(25))));
            }
            for (int i = 0; i < 10; i++)
            {
                itemFactory.CreateItemEntity(MeshFactory.Load("Core\\Barrel01"), Matrix.Translation(new Vector3(random.Next(25), 0, random.Next(25))));
            }

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new PickupSimulator(itemFactory, player));

            engine.AddSimulator(new RenderingSimulator());
        }

    }
}
