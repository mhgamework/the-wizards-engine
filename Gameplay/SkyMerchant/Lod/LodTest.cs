using DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1;
using NUnit.Framework;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    [TestFixture]
    [EngineTest]
    public class LodTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestLineLod()
        {
            var mesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_WithLeaves");
            var seeder = new Seeder(0);

            var size = 200;

            for (int i = 0; i < 1000; i++)
            {
                var p = new LineLodPhysical();
                p.Mesh = mesh;
                p.WorldMatrix = Matrix.Translation(seeder.NextVector3((MathHelper.One * -size).xna(), (MathHelper.One * size).xna()).dx());
            }
       
            engine.AddSimulator( new BasicSimulator(delegate
                {
                    foreach (var i in TW.Data.Objects.OfType<LineLodPhysical>())
                    {
                        i.UpdateMeshVisibility();
                    }
                }));

            engine.AddSimulator( new PhysicalSimulator());
            engine.AddSimulator( new WorldRenderingSimulator());
        }
    }
}