using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.RTSTestCase1;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    [TestFixture]
    [EngineTest]
    public class ImportTest
    {
        private System.Collections.Concurrent.ConcurrentQueue<string> fileQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();
        private String testImportMeshPath = TWDir.GameData + "/Core/MaxScriptExporter/rodwen.twobj";
        private String testAbsoluteAnimPathUnmodified_Simple = TWDir.GameData + "/Core/MaxScriptExporter/absAnimTest_unmodified.twanim";
        private String testAbsoluteAnimPathUnmodified = TWDir.GameData + "/Core/MaxScriptExporter/robotAnimUnmodified.twanim";
        private String testImportSkinPath = TWDir.GameData + "/Core/MaxScriptExporter/skinTest.twskin";

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
            parser.LoadAnimation(testAbsoluteAnimPathUnmodified_Simple, out boneStructure, out frameData);
        }

       [Test]
        public void TestRenderBones()
        {
            Skeleton skeleton;
            Animation.Animation animation;
            var importer = new AnimationImporter();
            importer.ImportAnimation(testAbsoluteAnimPathUnmodified_Simple, out skeleton, out animation);

            var skeletonVisualizer = new SkeletonVisualizer();
            var engine = EngineFactory.CreateEngine();
            engine.AddSimulator(new BasicSimulator(delegate
            {
                skeleton.UpdateAbsoluteMatrices();
                skeletonVisualizer.VisualizeSkeleton(TW.Graphics, skeleton);
            }));

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        [Test]
        public void TestRenderAnimation()
        {
            Skeleton skeleton;
            Animation.Animation animation;
            var importer = new AnimationImporter();
            importer.ImportAnimation(testAbsoluteAnimPathUnmodified, out skeleton, out animation);

            var controller = new AnimationControllerSkeleton(skeleton);
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
        public void TestImportSkinData()
        {
            //TODO

            var importer = new SkinDataImporter();
            List<VertexSkinData> skinData;
            importer.LoadSkinData(testImportSkinPath, out skinData);
        }
    }
}
