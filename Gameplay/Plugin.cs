using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Debugging;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.Tests.PhysX;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.LevelBuilding;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Pickup;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Serialization;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.TestRunner;
using MHGameWork.TheWizards.Trigger;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards
{
    public class Plugin : IGameplayPlugin
    {
        public void Initialize(TWEngine engine)
        {
            //testPickupSimulator(engine);
            //testTriggerSimulator(engine);
            //testWayPointTrigger(engine);
            //testWorldmatrixAnimation(engine);
            //testLoadLevel(engine);
            //testLevelBuilding(engine);

            //testAddStupidRedHelperMesh(engine);
            //testFirstPersonCamera(engine);
            testPlay(engine);
        }

        private void testFirstPersonCamera(TWEngine engine)
        {
            TW.Graphics.EscapeExists = false;
            TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            //TW.Data.GetSingleton<CameraInfo>().ActivateSpecatorCamera();

            engine.AddSimulator(new EngineUISimulator());
            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());

        }

        private void testPlay(TWEngine engine)
        {
            TW.Graphics.AcquireRenderer().ClearAll();

            TW.Graphics.EscapeExists = false;
            var testingData = TW.Data.GetSingleton<TestingData>();
            if (testingData.ActiveTestClass != null)
            {
                TW.Data.Objects.Clear();
                TW.Data.Objects.Add(testingData);
                try
                {
                    var runner = new EngineTestRunner();
                    runner.RunTestDataTest(engine);
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine(ex);
                }
                

                //var simulator = (ITestSimulator) Activator.CreateInstance(type);

                //engine.AddSimulator(simulator);
                //simulator.Initialize(engine);


                ////TODO: somewhat haxor
                //if (type.Namespace.Contains("MHGameWork.TheWizards.Engine"))
                //    loadBare(engine);
                //else
                //    loadEngine(engine);
                loadBare(engine);
            }
            else
            {
                loadEngine(engine);
            }
            
        }

        private void loadBare(TWEngine engine)
        {
            engine.AddSimulator(new EngineUISimulator());
            engine.AddSimulator(new TestUISimulator());
            engine.AddSimulator(new UISimulator());
        }
        private void loadEngine(TWEngine engine)
        {
            engine.AddSimulator(new EngineUISimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new PhysXSimulator());
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new FlashlightSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new TestUISimulator());
            engine.AddSimulator(new UISimulator());
        }
        private void testAddStupidRedHelperMesh(TWEngine engine)
        {
            var player = new PlayerData();
            player.Entity.Visible = false;
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            Engine.WorldRendering.Entity e = new Engine.WorldRendering.Entity();
            e.Mesh = MeshFactory.Load("Helpers\\RedHelperBIG\\RedHelperBIG"); //TODO: try to reconstruct bug

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new WorldRenderingSimulator());

        }

        /// <summary>
        /// Tests inventory functionalities.
        /// </summary>
        /// <param name="engine"></param>
        private void testPickupSimulator(TWEngine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            var itemFactory = new ItemEntityFactory();

            var random = new Random();

            for (int i = 0; i < 10; i++)
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

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        /// <summary>
        /// Tests some basic trigger setups.
        /// </summary>
        /// <param name="engine"></param>
        private void testTriggerSimulator(TWEngine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
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

            engine.AddSimulator(new WorldRenderingSimulator());
        }

        /// <summary>
        /// Test a trigger construction resembling a waypoint system.
        /// </summary>
        /// <param name="engine"></param>
        private void testWayPointTrigger(TWEngine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            var cond01 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(4, 0, 2), new Vector3(5, 2, 3) }));
            cond01.SetType(ConditionType.ONCE);
            var cond02 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(6, 0, 4), new Vector3(7, 2, 5) }));
            cond02.SetType(ConditionType.ONCE);
            var cond03 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(6, 0, 6), new Vector3(7, 2, 7) }));
            cond03.SetType(ConditionType.ONCE);
            var cond04 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(2, 0, 8), new Vector3(3, 2, 9) }));
            cond04.SetType(ConditionType.ONCE);
            var cond05 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(2, 0, 12), new Vector3(3, 2, 13) }));
            cond05.SetType(ConditionType.ONCE);
            var cond06 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(6, 0, 12), new Vector3(7, 2, 13) }));
            cond06.SetType(ConditionType.ONCE);
            var cond07 = new PlayerPositionCondition(player, BoundingBox.FromPoints(new[] { new Vector3(8, 0, 10), new Vector3(9, 2, 11) }));
            cond07.SetType(ConditionType.ONCE);

            var act01 = new CreateTriggerAction();
            var act02 = new CreateTriggerAction();
            var act03 = new CreateTriggerAction();
            var act04 = new CreateTriggerAction();
            var act05 = new CreateTriggerAction();
            var act06 = new CreateTriggerAction();
            var act07 = new SpawnAction(Matrix.Translation(new Vector3(10, 1, 8)), MeshFactory.Load("Core\\Crate01"));

            var triggerSim = new TriggerSimulator();

            var startTrigger = new Trigger.Trigger();
            startTrigger.Actions.Add(act01);
            startTrigger.Conditions.Add(cond01);

            bool orTrigger = false;
            bool invertTrigger = false;
            var triggerType = ConditionType.ONCE;
            act01.Setup(cond02, act02, orTrigger, invertTrigger, triggerType, triggerSim);
            act02.Setup(cond03, act03, orTrigger, invertTrigger, triggerType, triggerSim);
            act03.Setup(cond04, act04, orTrigger, invertTrigger, triggerType, triggerSim);
            act04.Setup(cond05, act05, orTrigger, invertTrigger, triggerType, triggerSim);
            act05.Setup(cond06, act06, orTrigger, invertTrigger, triggerType, triggerSim);
            act06.Setup(cond07, act07, orTrigger, invertTrigger, triggerType, triggerSim);

            triggerSim.AddTrigger(startTrigger);

            engine.AddSimulator(triggerSim);

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new WorldRenderingSimulator());

        }

        private void testWorldmatrixAnimation(TWEngine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            Vector3 p0 = new Vector3(1, 0, 1);
            Vector3 p1 = new Vector3(10, 0, 1);
            Vector3 p2 = new Vector3(10, 5, 10);
            Vector3 p3 = new Vector3(1, 0, 10);

            Quaternion r0 = Quaternion.RotationMatrix(Matrix.RotationX(0));
            Quaternion r1 = Quaternion.RotationMatrix(Matrix.RotationX((float)Math.PI));
            Quaternion r2 = Quaternion.RotationMatrix(Matrix.RotationZ((float)Math.PI));
            Quaternion r3 = Quaternion.RotationMatrix(Matrix.RotationY((float)Math.PI));
            Quaternion r4 = Quaternion.RotationMatrix(Matrix.RotationX(-(float)Math.PI * 0.5f));
            Quaternion r5 = Quaternion.RotationMatrix(Matrix.RotationY(-(float)Math.PI * 0.5f));

            Vector3 s0 = new Vector3(1, 1, 1);
            Vector3 s1 = new Vector3(2, 2, 2);
            Vector3 s2 = new Vector3(0.25f, 0.25f, 0.25f);

            var ent = new Engine.WorldRendering.Entity();
            ent.Mesh = MeshFactory.Load("Core\\Crate01");
            ent.WorldMatrix = Matrix.Translation(p0);

            var animatableTrans = new EntityTranslationAnimatable();
            animatableTrans.Entity = ent;

            var animatableRot = new EntityRotationAnimatable();
            animatableRot.Entity = ent;

            var animatableScale = new EntityScalingAnimatable();
            animatableScale.Entity = ent;

            var animationController = new AnimationController();
            animationController.AddKeyframe(animatableTrans, 0f, p0);
            animationController.AddKeyframe(animatableTrans, 2.5f, p1);
            animationController.AddKeyframe(animatableTrans, 6.5f, p2);
            animationController.AddKeyframe(animatableTrans, 7.5f, p3);
            animationController.AddKeyframe(animatableTrans, 10f, p0);

            animationController.AddKeyframe(animatableRot, 0f, r0);
            animationController.AddKeyframe(animatableRot, 0.5f, r1);
            animationController.AddKeyframe(animatableRot, 1.5f, r2);
            animationController.AddKeyframe(animatableRot, 2.5f, r3);
            animationController.AddKeyframe(animatableRot, 5f, r4);
            animationController.AddKeyframe(animatableRot, 8f, r5);
            animationController.AddKeyframe(animatableRot, 10f, r0);

            animationController.AddKeyframe(animatableScale, 0f, s0);
            animationController.AddKeyframe(animatableScale, 7.5f, s0);
            animationController.AddKeyframe(animatableScale, 8.25f, s1);
            animationController.AddKeyframe(animatableScale, 9f, s2);
            animationController.AddKeyframe(animatableScale, 10f, s0);

            animationController.Loop = true;

            var animationSim = new AnimationSimulator();
            animationSim.AddAnimationController(animationController);

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(animationSim);

            engine.AddSimulator(new WorldRenderingSimulator());


            animationController.Play();
        }

        private void testLoadLevel(TWEngine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;

            var ent = new Engine.WorldRendering.Entity();
            ent.Mesh = MeshFactory.Load("Level01\\Level01");
            ent.Solid = true;

            var barrel = new Engine.WorldRendering.Entity();
            barrel.Mesh = MeshFactory.Load("Core\\Barrel01");
            barrel.WorldMatrix = Matrix.Translation(new Vector3(0, 30, 0));
            barrel.Solid = true;
            barrel.Static = false;

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());
            engine.AddSimulator(new PhysXSimulator());

            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new PhysXDebugRendererSimulator());
        }

        private void testLevelBuilding(TWEngine engine)
        {
            var player = new PlayerData();
            //player.Entity.Visible = false;
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.FirstPersonCameraTarget = player.Entity;


            var factory = new LevelBuildingObjectFactory();

            var type01 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_Straight_01\\GreyBrick_Straight_01"));
            var type02 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_Straight_02\\GreyBrick_Straight_02"));
            var type03 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_Pillar_01\\GreyBrick_Pillar_01"));
            var type04 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_PillarCap_01\\GreyBrick_PillarCap_01"));
            var type05 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_RoofC_01\\GreyBrick_RoofC_01"));
            var type06 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_RoofT_01\\GreyBrick_RoofT_01"));
            var type07 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_RoofU_01\\GreyBrick_RoofU_01"));
            var type08 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_RoofX_01\\GreyBrick_RoofX_01"));
            var type09 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\GreyBrick_Stair_01\\GreyBrick_Stair_01"));
            var type10 = new LevelBuildingEntityType(MeshFactory.Load("TileSet01\\Floor_01\\Floor_01"));

            var triggerType = new LevelBuildingTriggerObjectType();


            string file = TWDir.GameData + "\\Level.txt";
            var stringSerializer = StringSerializer.Create();
            stringSerializer.AddConditional(new FilebasedAssetSerializer());

            if (File.Exists(file))
                using (var fs = File.OpenRead(file))
                    TW.Data.ModelSerializer.Deserialize(new StreamReader(fs));




            factory.AddLevelBuildingObjectType(type01);
            factory.AddLevelBuildingObjectType(type02);
            factory.AddLevelBuildingObjectType(type03);
            factory.AddLevelBuildingObjectType(type04);
            factory.AddLevelBuildingObjectType(type05);
            factory.AddLevelBuildingObjectType(type06);
            factory.AddLevelBuildingObjectType(type07);
            factory.AddLevelBuildingObjectType(type08);
            factory.AddLevelBuildingObjectType(type09);
            factory.AddLevelBuildingObjectType(type10);

            factory.AddLevelBuildingObjectType(triggerType);

            foreach (var ent in TW.Data.Objects.Where(t => t is Engine.WorldRendering.Entity).Select(t => (Engine.WorldRendering.Entity)t))
            {
                if (ent == player.Entity) continue;
                ent.Solid = true;
                ent.Static = true;
            }

            engine.AddSimulator(new LevelBuildingSimulator(player, cameraInfo, factory));

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new DebugSimulator());
            engine.AddSimulator(new EntityBatcherSimulator());

            //engine.AddSimulator(new PhysXSimulator());

            engine.AddSimulator(new ProfilerSimulator());

            engine.AddSimulator(new WorldRenderingSimulator());
            //engine.AddSimulator(new PhysXDebugRendererSimulator());


            //engine.AddSimulator(new AutoSaveSimulator(file, new TimeSpan(0, 0, 10), modelSerializer));
        }
    }




}
