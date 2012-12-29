using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Threading;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.Testing
{
    public class TestUISimulator : ISimulator
    {
        public void Simulate()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.F5)) return;

            //TW.Graphics.InputDisabled = true;

            var picker = new EngineTestPicker();
            var test = picker.SelectTest();
            if (test == null)
                return;

            var runner = new EngineTestRunner();
            runner.RunTest(test);

        
        }
    }
}
