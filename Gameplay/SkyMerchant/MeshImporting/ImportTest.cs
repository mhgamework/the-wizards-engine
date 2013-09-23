using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.RTSTestCase1;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine;

namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    [TestFixture]
    [EngineTest]
    public class ImportTest
    {
        private System.Collections.Concurrent.ConcurrentQueue<string> fileQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();

        //private String testImportMeshPath = "C:/Users/Simon/Documents/My Dropbox/Projects/SkyMerchant/MaxScriptExporter/geomTest.twobj";
        private String testImportMeshPath = TWDir.GameData + "/MaxScriptExporter/rodwen.twobj";
        private String testImportAnimPathRenderBones = TWDir.GameData + "/MaxScriptExporter/animationTest1501.twanim";
        //private String testImportAnimPath = TWDir.GameData + "/MaxScriptExporter/animationTest.twanim";
        private String testImportAnimPath = TWDir.GameData + "/MaxScriptExporter/robotAnimTest01.twanim";
        //private String testImportAnimPath = TWDir.GameData + "/MaxScriptExporter/bigSAnim01.twanim";
        private String testAbsoluteAnimPath = TWDir.GameData + "/MaxScriptExporter/absAnimTest.twanim";

        private String testImportSkinPath = TWDir.GameData + "/MaxScriptExporter/skinTest.twskin";


        [Test]
        public void TestImportMesh()
        {
            try
            {
                var importer = new MeshImporter();
                Material[] matStream;
                Vector3[] posStream;
                Vector3[] normStream;
                Vector3[] texcoStream;
                Dictionary<int, List<int>> posInd;
                Dictionary<int, List<int>> normInd;
                Dictionary<int, List<int>> texcoInd;
                importer.LoadMesh(testImportMeshPath, out matStream, out posStream, out normStream, out texcoStream, out posInd, out normInd, out texcoInd);

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }

        }

        [Test]
        public void TestRenderMesh()
        {
            try
            {
                var importer = new MeshImporter();
                Material[] matStream;
                Vector3[] posStream;
                Vector3[] normStream;
                Vector3[] texcoStream;
                Dictionary<int, List<int>> posInd;
                Dictionary<int, List<int>> normInd;
                Dictionary<int, List<int>> texcoInd;
                importer.LoadMesh(testImportMeshPath, out matStream, out posStream, out normStream, out texcoStream, out posInd, out normInd, out texcoInd);

                var meshBuilder = new MeshBuilder();
                var mesh = meshBuilder.BuildMesh(matStream, posStream, normStream, texcoStream, posInd, normInd, texcoInd);


                var engine = EngineFactory.CreateEngine();

                /*var watcher = new FileSystemWatcher();
                watcher.Changed += watcher_Changed;
                watcher.EnableRaisingEvents = true;
                watcher.Path = testImportMeshPath;
                watcher.Filter = "*";*/
                var physical = new Physical();
                physical.Mesh = mesh;

                engine.AddSimulator(new BasicSimulator(delegate
                {
                    /*string result = null;
                    while (fileQueue.TryDequeue(out result))
                    {

                    }*/
                    //TW.Data.RemoveObject(physical);

                }));

                engine.AddSimulator(new PhysicalSimulator());
                engine.AddSimulator(new WorldRenderingSimulator());

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }
        }

        [Test]
        public void TestImportAnimationData()
        {
            try
            {
                var importer = new AnimationImporter();
                List<BoneData> boneStructure;
                List<Frame> frameData;
                importer.LoadAnimation(testImportAnimPath, out boneStructure, out frameData);

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }
        }

        [Test]
        public void TestRenderBones()
        {
            try
            {
                var importer = new AnimationImporter();
                List<BoneData> boneStructure;
                List<Frame> frameData;
                importer.LoadAnimation(testImportAnimPathRenderBones, out boneStructure, out frameData);

                var skeletonBuilder = new SkeletonBuilder();
                var skeleton = skeletonBuilder.BuildSkeleton(boneStructure);

                var skeletonVisualizer = new SkeletonVisualizer();


                var engine = EngineFactory.CreateEngine();
                //var physical = new Physical();
                //physical.Mesh = mesh;

                engine.AddSimulator(new BasicSimulator(delegate
                {
                    skeletonVisualizer.VisualizeSkeleton(TW.Graphics, skeleton);
                }));

                engine.AddSimulator(new PhysicalSimulator());
                engine.AddSimulator(new WorldRenderingSimulator());

            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }
        }

        [Test]
        public void TestRenderAnimation()
        {
            var importer = new AnimationImporter();
            List<BoneData> boneStructure;
            List<Frame> frameData;
            importer.LoadAnimation(testImportAnimPath, out boneStructure, out frameData);

            var skeletonBuilder = new SkeletonBuilder();
            var skeleton = skeletonBuilder.BuildSkeleton(boneStructure);
            var controller = new AnimationControllerSkeleton(skeleton);

            var animationBuilder = new AnimationBuilder();
            var animation = animationBuilder.BuildAnimation(frameData, skeleton);

            controller.SetAnimation(0, animation);

            var skeletonVisualizer = new SkeletonVisualizer();

            var engine = EngineFactory.CreateEngine();

            engine.AddSimulator(new BasicSimulator(delegate
            {
                controller.ProgressTime(TW.Graphics.Elapsed);
                controller.UpdateSkeleton();
                skeleton.UpdateAbsoluteMatrices();
                skeletonVisualizer.VisualizeSkeleton(TW.Graphics, skeleton);
            }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }


        [Test]
        public void TestRenderAbsoluteBones()
        {
            var importer = new AnimationImporter();
            List<BoneData> boneStructure;
            List<Frame> frameData;
            importer.LoadAnimation(testAbsoluteAnimPath, out boneStructure, out frameData);

            var skeletonBuilder = new SkeletonBuilderAbsolute();
            var skeleton = skeletonBuilder.BuildSkeleton(boneStructure);

            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new BasicSimulator(delegate
            {
                /*if (TW.Graphics.Elapsed < 1 / 60f)
                {
                    Thread.Sleep(TimeSpan.FromSeconds((1 / 60 - TW.Graphics.Elapsed )*2));
                }*/
                foreach (var joint in skeleton.Joints)
                {
                    TW.Graphics.LineManager3D.AddMatrixAxes(joint.AbsoluteMatrix);
                }
            }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestRenderAbsoluteAnimation()
        {
            var importer = new AnimationImporter();
            List<BoneData> boneStructure;
            List<Frame> frameData;
            importer.LoadAnimation(testAbsoluteAnimPath, out boneStructure, out frameData);

            var skeletonBuilder = new SkeletonBuilder();
            var skeleton = skeletonBuilder.BuildSkeleton(boneStructure);
            var controller = new AnimationControllerSkeleton(skeleton);

            var animationBuilder = new AnimationBuilder();
            var animation = animationBuilder.BuildAnimation(frameData, skeleton);

            controller.SetAnimation(0, animation);



            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new BasicSimulator(delegate
            {
                controller.ProgressTime(TW.Graphics.Elapsed);
                controller.UpdateSkeleton();
                skeleton.UpdateAbsoluteMatrices();

                TW.Graphics.LineManager3D.DrawGroundShadows = true;
                foreach (var joint in skeleton.Joints)
                {
                    TW.Graphics.LineManager3D.AddMatrixAxes(joint.AbsoluteMatrix);
                }
            }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }


        [Test]
        public void TestImportSkinData()
        {
            try
            {
                var importer = new SkinDataImporter();
                List<VertexSkinData> skinData;
                importer.LoadSkinData(testImportSkinPath, out skinData);
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Init prototype");
            }
        }


        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Other thread!!!!!!
            fileQueue.Enqueue(e.FullPath);
        }
    }
}
