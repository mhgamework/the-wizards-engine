using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Inputting
{
    /// <summary>
    /// Responsible for processing system user input into TW Data
    /// </summary>
    public class InputSimulator : ISimulator
    {
        public void Simulate()
        {
            TW.Data.Get<InputFactory>().Update();
        }
    }
}
