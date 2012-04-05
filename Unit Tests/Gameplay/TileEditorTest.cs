using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Model;
using MHGameWork.TheWizards.Simulation;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class TileEditorTest
    {
        [Test]
        public void TestTileEditor()
        {
            var game = new LocalGame();


            game
                .AddSimulator(new TileEditorSimulator())
                .AddSimulator(new SimpleWorldRenderer());


            TW.Model.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.Specator;

            game.Run();
        }

    }
}
