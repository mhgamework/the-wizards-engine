using System;
using MHGameWork.TheWizards.Engine.Tests;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine.DynamicWorld.Tests
{
    public class SDFTerrainGeneratorTest : EngineTestFixture
    {
        private SDFTerrainGenerator gen;

        [SetUp]
        public void Setup()
        {
            gen = new SDFTerrainGenerator();
        }

        [Test]
        [Ignore]
        public void TestGenerateChunkSigns()
        {
            var signs = gen.GenerateSigns( new Vector3( 0, 0, 0 ), new Vector3( 1, 1, 1 ), 128 );
            throw new NotImplementedException();


        }
    }
}