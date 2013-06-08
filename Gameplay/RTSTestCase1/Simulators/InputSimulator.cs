using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;

namespace MHGameWork.TheWizards.RTSTestCase1.Simulators
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
