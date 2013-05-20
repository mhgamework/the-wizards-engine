using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class WorldInputtingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private CommandFactory f;

        [SetUp]
        public void Setup()
        {
            f = TW.Data.Get<CommandFactory>();
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void TestShizzle()
        {

          
            var s = new WorldInputtingSimulator();
            s.Configuration.Menu.CreateItem("Rivers", enableRivers);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);
            s.Configuration.Menu.CreateItem("Walls", enableWalls);

            engine.AddSimulator(s);
            engine.AddSimulator(new WorldRenderingSimulator());


        }
        private void enableRivers()
        {
            
        }
        private void enableWalls()
        {
            
        }

  
    }
}
