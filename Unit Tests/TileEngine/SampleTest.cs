using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    [TestFixture]
    public class SampleTest
    {
        [Test]
        public void TestSample()
        {
            XNAGame game = new XNAGame();


            var point = new SnapPoint();
            point.Position = new Vector3(float.NaN, 0, 0);
            

            game.InitializeEvent += delegate
            {
            };
            game.DrawEvent += delegate
            {
            };
            game.UpdateEvent += delegate
            {
            };
            game.Run();
        }

        [Test]
        public void TestJasper()
        {
            System.Console.WriteLine("Jasper is not testable!");
        }
    }
}
