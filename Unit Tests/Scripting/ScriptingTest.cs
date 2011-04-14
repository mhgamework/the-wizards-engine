using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Scripting;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Scripting
{
    [TestFixture]
    public class ScriptingTest
    {
        [Test]
        public void TestRunScript()
        {
            var game = new XNAGame();

            var runner = new ScriptRunner(game);

            game.InitializeEvent += delegate
                                        {
                                            runner.RunScript(new SimpleStateScript());
                                        };


            game.Run();
        }


    }
}
