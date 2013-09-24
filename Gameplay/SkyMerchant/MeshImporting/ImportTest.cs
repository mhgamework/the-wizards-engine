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

        private String testImportMeshPath = TWDir.GameData + "/MaxScriptExporter/rodwen.twobj";

        private String testImportAnimPathRenderBones = TWDir.GameData + "/MaxScriptExporter/animationTest1501.twanim";
        //private String testImportAnimPath = TWDir.GameData + "/MaxScriptExporter/animationTest.twanim";
        private String testImportAnimPath = TWDir.GameData + "/MaxScriptExporter/robotAnimTest01.twanim";
        //private String testImportAnimPath = TWDir.GameData + "/MaxScriptExporter/bigSAnim01.twanim";
        private String testAbsoluteAnimPath = TWDir.GameData + "/MaxScriptExporter/absAnimTest.twanim";
        private String testAbsoluteAnimPathUnmodified_Simple = TWDir.GameData + "/MaxScriptExporter/absAnimTest_unmodified.twanim";
        private String testAbsoluteAnimPathUnmodified = TWDir.GameData + "/MaxScriptExporter/robotAnimUnmodified.twanim";

        private String testImportSkinPath = TWDir.GameData + "/MaxScriptExporter/skinTest.twskin";


        [Test]
        public void TestParseMesh()
        {
            var parser = new MeshParser();
            Material[] matStream;
            Vector3[] posStream;
            Vector3[] normStream;
            Vector3[] texcoStream;
            Dictionary<int, List<int>> posInd;
            Dictionary<int, List<int>> normInd;
            Dictionary<int, List<int>> texcoInd;
            parser.LoadMesh(testImportMeshPath, out matStream, out posStream, out normStream, out texcoStream, out posInd, out normInd, out texcoInd);
        }

        [Test]
        public void TestRenderMesh()
        {
            var importer = new MeshImporter();
            IMesh mesh;
            importer.ImportMesh(testImportMeshPath, out mesh);
            
            var engine = EngineFactory.CreateEngine();
            var physical = new Physical();
            physical.Mesh = mesh;
            engine.AddSimulator(new BasicSimulator());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestParseAnimationData()
        {
            var parser = new AnimationParser();
            List<BoneData> boneStructure;
            List<Frame> frameData;
            parser.LoadAnimation(testImportAnimPath, out boneStructure, out frameData);
        }

        [Test]
        public void TestRenderBones()
        {
            //TODO: fix/remove?
            try
            {
                var importer = new AnimationParser();
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
            //TODO: fix/remove?
            var importer = new AnimationParser();
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
            //TODO: fix
            Skeleton skeleton;
            Animation.Animation animation;
            var importer = new AnimationImporter();
            importer.ImportAnimation(testAbsoluteAnimPathUnmodified, out skeleton, out animation);

            var skeletonVisualizer = new SkeletonVisualizer();
            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new BasicSimulator(delegate
            {
                skeleton.Joints.ForEach(j => j.AbsoluteMatrix = j.RelativeMatrix); //haxor to display skeleton structure while using absolute transforms
                skeletonVisualizer.VisualizeSkeleton(TW.Graphics, skeleton);
            }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestRenderAbsoluteAnimation()
        {
            Skeleton skeleton;
            Animation.Animation animation;
            var importer = new AnimationImporter();
            importer.ImportAnimation(testAbsoluteAnimPathUnmodified, out skeleton, out animation);

            var controller = new AnimationControllerSkeleton(skeleton);
            controller.SetAnimation(0, animation);
            var skeletonVisualizer = new SkeletonVisualizer();

            //skeleton.Joints.ForEach(j => j.Parent = null);// set parents to null

            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new BasicSimulator(delegate
            {
                controller.ProgressTime(TW.Graphics.Elapsed);
                controller.UpdateSkeleton();
                skeleton.Joints.ForEach(j => j.AbsoluteMatrix = j.RelativeMatrix); //haxor to display skeleton structure while using absolute transforms

                skeletonVisualizer.VisualizeSkeleton(TW.Graphics, skeleton);

            }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }


        [Test]
        public void TestImportSkinData()
        {
            //TODO

            var importer = new SkinDataImporter();
            List<VertexSkinData> skinData;
            importer.LoadSkinData(testImportSkinPath, out skinData);
        }
    }
}
