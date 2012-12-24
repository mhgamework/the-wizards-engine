using System;
using System.Collections.Generic;
using System.Linq;
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

            var picker = new EngineTestPicker();
            var test = picker.SelectTest();
            if (test == null)
                return;

            TW.Data.GetSingleton<TestingData>().ActiveTestClass = TW.Data.TypeSerializer.Serialize(test.TestClass);
            TW.Data.GetSingleton<TestingData>().ActiveTestMethod = test.TestMethod.Name;
            TW.Debug.NeedsReload = true;
        }
    }
}
