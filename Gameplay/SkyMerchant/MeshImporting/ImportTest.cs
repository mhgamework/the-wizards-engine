using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.RTSTestCase1;
using NUnit.Framework;
using SlimDX;
using MHGameWork.TheWizards;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Engine;
namespace MHGameWork.TheWizards.SkyMerchant.MeshImporting
{
    [TestFixture]
    [EngineTest]
    public class ImportTest
    {
        private System.Collections.Concurrent.ConcurrentQueue<string> fileQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();

        //private String testImportMeshPath = "C:/Users/Simon/Documents/My Dropbox/Projects/SkyMerchant/MaxScriptExporter/geomTest.twobj";
        private String testImportMeshPath = "C:/Users/Simon/Documents/My Dropbox/Projects/SkyMerchant/MaxScriptExporter/rodwen.twobj";
        private String testImportAnimPath = "C:/Users/Simon/Documents/My Dropbox/Projects/SkyMerchant/MaxScriptExporter/animationTest.twanim";

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

                engine.AddSimulator(new BasicSimulator(delegate
                {
                    /*string result = null;
                    while (fileQueue.TryDequeue(out result))
                    {

                    }*/

                    physical.Mesh = mesh;
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
                importer.LoadAnimation(testImportAnimPath);
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
