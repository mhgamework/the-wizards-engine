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

            var picker = new EngineTestPicker();
            var test = picker.SelectTest();
            if (test == null)
                return;

            TW.Data.GetSingleton<TestingData>().ActiveTestClass = TW.Data.TypeSerializer.Serialize(test.TestClass);
            TW.Data.GetSingleton<TestingData>().ActiveTestMethod = test.TestMethod.Name;


            var mem = new MemoryStream();
            var f = new BinaryFormatter();
            f.Serialize(mem,test);

            TW.Data.GetSingleton<TestingData>().SerializedTest = mem.ToArray();


            TW.Debug.NeedsReload = true;
        }
    }
}
