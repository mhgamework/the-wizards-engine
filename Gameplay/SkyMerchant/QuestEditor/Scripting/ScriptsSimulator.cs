using MHGameWork.TheWizards.Engine;
using System.Linq;
using MHGameWork.TheWizards.SkyMerchant.Worlding;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.Scripting
{
    /// <summary>
    /// Simulates the behaviour of the scripts
    /// </summary>
    public class ScriptsSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var obj in TW.Data.Objects.OfType<WorldObject>())
            {
                foreach (var s in obj.Scripts)
                {
                    s.Update();
                }
            }
        }
    }
}