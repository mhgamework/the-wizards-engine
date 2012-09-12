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

            
            //Test One-time trigger
            var cond01 = new PlayerPositionCondition(player, new BoundingBox(new Vector3(10, 0, 10), new Vector3(15, 5, 15)));
            cond01.SetType(ConditionType.ONCE); //default is SWITCH
            var act01 = new SpawnAction(Matrix.Translation(new Vector3(12.5f, 2.5f, 12.5f)), MeshFactory.Load("Core\\Crate01"));
            var trig01 = new Trigger.Trigger();
            trig01.Conditions.Add(cond01);
            trig01.Actions.Add(act01);

            //Test Switch-type trigger
            var cond02 = new PlayerPositionCondition(player, new BoundingBox(new Vector3(0, 0, 10), new Vector3(5, 5, 15)));
            cond02.SetType(ConditionType.SWITCH);
            var act02 = new SpawnAction(Matrix.Translation(new Vector3(2.5f, 2.5f, 12.5f)), MeshFactory.Load("Core\\Crate01"));
            var trig02 = new Trigger.Trigger();
            trig02.Conditions.Add(cond02);
            trig02.Actions.Add(act02);

            //Test inverted switch trigger
            var cond03 = new PlayerPositionCondition(player, new BoundingBox(new Vector3(-10, 0, 10), new Vector3(-15, 5, 15)));
            cond03.Invert();
            var act03 = new SpawnAction(Matrix.Translation(new Vector3(-12.5f, 2.5f, 12.5f)), MeshFactory.Load("Core\\Crate01"));
            var trig03 = new Trigger.Trigger();
            trig03.Conditions.Add(cond03);
            trig03.Actions.Add(act03);

            //Test Or-type trigger
            var cond04 = new PlayerPositionCondition(player, new BoundingBox(new Vector3(5, 0, -10), new Vector3(10, 5, -5)));
            var cond05 = new PlayerPositionCondition(player, new BoundingBox(new Vector3(5, 0, -2), new Vector3(10, 5, 3)));
            var act04 = new SpawnAction(Matrix.Translation(new Vector3(7.5f, 2.5f, -3.5f)), MeshFactory.Load("Core\\Crate01"));
            var trig04 = new Trigger.Trigger();
            trig04.SetAndOr(true);
            trig04.Conditions.Add(cond04);
            trig04.Conditions.Add(cond05);
            trig04.Actions.Add(act04);



            

            var triggerSim = new TriggerSimulator();
            triggerSim.AddTrigger(trig01);
            triggerSim.AddTrigger(trig02);
            triggerSim.AddTrigger(trig03);
            triggerSim.AddTrigger(trig04);


            engine.AddSimulator(triggerSim);


            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new RenderingSimulator());
        }
    }
}
