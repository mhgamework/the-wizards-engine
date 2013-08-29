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
        private EngineTestPicker picker;

        public TestUISimulator()
        {
            picker = new EngineTestPicker();
            
        }

        public void Simulate()
        {
            if (picker.PickCompleted)
            {
                var runner = new EngineTestRunner();
                runner.RunTest(picker.GetPickedTest());
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F5))
            {
                //TODO: disable all user input for simulators?
                picker.ShowTestPicker();
            }

        }
    }
}
