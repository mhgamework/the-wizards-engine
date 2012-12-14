using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Responsible for simulating the
    /// </summary>
    public class PersistenceInterfaceSimulator : ISimulator
    {
        public void Simulate()
        {
            var area = new Textarea();
            area.Position = new Vector2(0,0);
            area.Size = new Vector2(200,600);
            area.Text = "elleu!";
        }
    }
}
