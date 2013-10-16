using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Gameplay;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using NSubstitute;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Stable
{
    /// <summary>
    /// Tests bridge functionality
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class BridgeTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();



        /// <summary>
        /// Test creation of a bridge
        /// DOCUMENTATION
        /// </summary>
        [Test]
        public void TestRenderBridge()
        {
            // Allows creating bridge meshes. Meshes created are in a unified format (length unit 1) 
            //  The should be transformed using the matrix returned from the bridgemeshbuilder
            var builder = new BridgeMeshBuilder(new AssetsRepository(), new DynamicAssetsFactory());

            // Start and end of the bridge
            var startPoint = new Vector3(-5, 0, -5);
            var endPoint = new Vector3(10, 5, 10);

            // Create bridges using the builder, and use the matrices returned from the builder to place it into the world
            new Physical()
                {
                    Mesh = builder.BuildMeshForEndpoint(endPoint),
                    WorldMatrix = builder.GetMatrixForEndpoint(endPoint) * Matrix.Translation(startPoint)
                };

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }


        /// <summary>
        /// Test creation of bridge meshes
        /// </summary>
        [Test]
        public void TestBridgeMesh()
        {
            // Allows creating bridge meshes. Meshes created are in a unified format (length unit 1) 
            //  The should be transformed using the matrix returned from the bridgemeshbuilder
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
                // Create bridges using the builder, and use the matrices returned from the builder to place it into the world
                new Physical() { Mesh = builder.BuildMeshForEndpoint(m), WorldMatrix = builder.GetMatrixForEndpoint(m) * Matrix.Translation(0, 0, 4 + count * 11) };
                count++;

            }

            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        /// <summary>
        /// Tests the creation of an in-game bridge part
        /// </summary>
        [Test]
        public void TestBridgePart()
        {

            var bridge = new BridgePart(new Physical(), new BridgeMeshBuilder(new AssetsRepository(), new DynamicAssetsFactory()));

            bridge.AnchorA = new BridgePart.BridgeAnchor()
                {
                    Island = createIsland(new Vector3(5, 0, 5)),
                    RelativePosition = new Vector3(4.5f, 0, 4.5f)
                };

            bridge.AnchorB = new BridgePart.BridgeAnchor()
                {
                    Island = createIsland(new Vector3(25, 5, 20)),
                    RelativePosition = new Vector3(-5, 0, -5)
                };

            engine.AddSimulator(new SkyMerchantRenderingSimulator());
            engine.AddSimulator(new PhysicalSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        private IPositionComponent createIsland(Vector3 pos)
        {
            throw new NotImplementedException();
            //var ph = new Physical();
            //var w = new WorldObject(ph);
            //w.Position = pos;

            //var ret = new IslandPart();
            //ret.Physical = new WorldObjectPhysicalPart(w, ph);
            //ret.IslandMeshFactory = new IslandMeshFactory(new VoxelMeshBuilder());



            //return w;
        }



    }
}