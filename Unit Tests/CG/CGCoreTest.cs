using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComputerGraphics;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;
using TreeGenerator.EngineSynchronisation;

namespace MHGameWork.TheWizards.Tests.CG
{
    [TestFixture]
    public class CGCoreTest
    {
        [Test]
        public void TestGenerateRays()
        {
            PerspectiveCamera.Test();
        }

        [Test]
        public void TestSceneRaycaster()
        {
            var engine = new TWEngine();
            engine.Initialize();

            var raycaster = new SceneRaytracer();

            raycaster.AddEntity(createEntity());


            engine.AddSimulator(new WorldRenderingSimulator());

            engine.Run();

        }

        private WorldRendering.Entity createEntity()
        {
            var ret = new WorldRendering.Entity();
            ret.WorldMatrix = Matrix.Translation(new Vector3(0, 0, -5));
            ret.Mesh =
                OBJParser.OBJParserTest.GetBarrelMesh(
                    new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));


            return ret;

        }
    }
}
