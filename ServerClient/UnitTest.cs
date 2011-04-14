using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class UnitTest
    {
        public static void Run<T>()where T : IUnitTestGame, new()
        {
            T test = new T();
            test.Run();
        }
    }
}
