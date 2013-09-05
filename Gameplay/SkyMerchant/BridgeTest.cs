using System;

using System.Collections.Generic;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using DirectX11;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._Windsor;
using NUnit.Framework;
using System.Linq;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant
{
    [EngineTest]
    [TestFixture]
    public class BridgeTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestBridgeMesh()
        {
            var builder = new BridgeMeshBuilder(new AssetsRepository(), new DynamicAssetsFactory());


            var count = 0;
            foreach (var num in new[] { 1, 5, 10 })
            {
                new Physical() { Mesh = builder.BuildMesh(num), WorldMatrix = Matrix.Translation(count * 3, 0, 0) };
                count++;

            }

            var endpoints = new[]
                {
                    new Vector3(10, 0, 0),
                    new Vector3(0, 0, 10),
                    new Vector3(10, 4, 0),
                    new Vector3(10, 4, 10)
                };

            count = 0;
            foreach (var m in endpoints)
            {
                new Physical() { Mesh = builder.BuildMeshForEndpoint(m), WorldMatrix = builder.GetMatrixForEndpoint(m) * Matrix.Translation(0, 0, 4 + count * 11) };
                count++;

            }

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

    }
}