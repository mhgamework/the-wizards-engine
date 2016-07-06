using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Tests;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.VoxelEngine.Environments;
using NUnit.Framework;

namespace MHGameWork.TheWizards.VoxelEngine
{
    /// <summary>
    /// Tests for the dualcontouring implementation on uniform grids
    /// </summary>
    public class UniformDCAlgorithmTest :EngineTestFixture
    {
        private DualContouringTestEnvironment environment;

        [SetUp]
        public void Setup()
        {
            environment = new DualContouringTestEnvironment();
            environment.AddToEngine(engine);
        }

        [Test]
        public void TestSphere()
        {
            environment.Grid = ExampleGrids.CreateSphereUniform();
        }
    }
}