using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Debugging;
using MHGameWork.TheWizards.Engine.Files;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.Tests.PhysX;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Serialization;
using MHGameWork.TheWizards.Simulators;
using MHGameWork.TheWizards.TestRunner;
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

            TW.Graphics.MouseInputDisabled = true;
            TW.Graphics.Mouse.CursorEnabled = true;

            var fs = new EngineFileSystem(TWDir.GameData.FullName + "\\EngineFS");
            DI.Set<IEngineFilesystem>(fs);
            DI.Set<EngineTestState>(new EngineTestState(fs));
            DI.Set<TestSceneBuilder>(new TestSceneBuilder(DI.Get<EngineTestState>()));

            cleanData();
            var initializer = new EngineInitializer(DI.Get<EngineTestState>());

            initializer.SetupEngine(engine);
        }

        private void cleanData()
        {
            // Clear all objects
            TW.Data.Objects.Clear();
            TW.Debug.NeedsReload = true;


        }

        private void testAddStupidRedHelperMesh(TWEngine engine)
        {
            var player = new PlayerData();
            player.Entity.Visible = false;
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.ThirdPersonCameraTarget = player.Entity;

            Engine.WorldRendering.Entity e = new Engine.WorldRendering.Entity();
            e.Mesh = MeshFactory.Load("Helpers\\RedHelperBIG\\RedHelperBIG"); //TODO: try to reconstruct bug

            engine.AddSimulator(new LocalPlayerSimulator(player));
            engine.AddSimulator(new ThirdPersonCameraSimulator());

            engine.AddSimulator(new WorldRenderingSimulator());

        }




        private void testWorldmatrixAnimation(TWEngine engine)
        {
            var player = new PlayerData();
            var cameraInfo = TW.Data.GetSingleton<CameraInfo>();
            cameraInfo.Mode = CameraInfo.CameraMode.ThirdPerson;
            cameraInfo.ThirdPersonCameraTarget = player.Entity;

            Vector3 p0 = new Vector3(1, 0, 1);
            Vector3 p1 = new Vector3(10, 0, 1);
            Vector3 p2 = new Vector3(10, 5, 10);
            Vector3 p3 = new Vector3(1, 0, 10);

            Quaternion r0 = Quaternion.RotationMatrix(Matrix.RotationX(0));
            Quaternion r1 = Quaternion.RotationMatrix(Matrix.RotationX((float) Math.PI));
            Quaternion r2 = Quaternion.RotationMatrix(Matrix.RotationZ((float) Math.PI));
            Quaternion r3 = Quaternion.RotationMatrix(Matrix.RotationY((float) Math.PI));
            Quaternion r4 = Quaternion.RotationMatrix(Matrix.RotationX(-(float) Math.PI*0.5f));
            Quaternion r5 = Quaternion.RotationMatrix(Matrix.RotationY(-(float) Math.PI*0.5f));

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
            cameraInfo.ThirdPersonCameraTarget = player.Entity;

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

    }




}
