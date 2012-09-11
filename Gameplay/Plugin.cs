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
using MHGameWork.TheWizards.Trigger;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class Plugin : IGameplayPlugin
    {
        public void Initialize(Engine engine)
        {
            //testPickupSimulator(engine);
            testTriggerSimulator(engine);
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

        private void testTriggerSimulator(Engine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Model.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            

            var cond01 = new PlayerPositionCondition(player,
                                                     new BoundingBox(new Vector3(10, 0, 10), new Vector3(15, 5, 15)));

            var act01 = new SpawnAction(Matrix.Translation(new Vector3(15, 0, 15)), MeshFactory.Load("Core\\Crate01"));

            var trig01 = new Trigger.Trigger();
            trig01.Conditions.Add(cond01);
            trig01.Actions.Add(act01);

            var triggerSim = new TriggerSimulator();
            triggerSim.AddTrigger(trig01);

            engine.AddSimulator(triggerSim);


            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new RenderingSimulator());
        }
    }
}
