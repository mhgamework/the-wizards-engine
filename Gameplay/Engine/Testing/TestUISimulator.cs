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
            picker = new EngineTestPicker(DI.Get<EngineTestState>());
            
        }

        public void Simulate()
        {
            if (picker.PickCompleted)
            {
                var picked = picker.GetPickedTest();
                DI.Get<EngineTestState>().SetActiveTest(picked.TestMethod);
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F5))
            {
                //TODO: disable all user input for simulators?
                picker.ShowTestPicker();
            }

        }
    }
}
