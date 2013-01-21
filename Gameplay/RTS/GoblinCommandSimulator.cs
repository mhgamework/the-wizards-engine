using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS.Commands;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinCommandSimulator : ISimulator
    {
        public void Simulate()
        {
            TW.Data.EnsureAttachment<Goblin,GoblinCommandState>(g => new GoblinCommandState(g));
            foreach (Goblin g in TW.Data.Objects.Where(o => o is Goblin))
                g.get<GoblinCommandState>(). Update();
        }
    }

    public class GoblinCommandState : IModelObjectAddon<Goblin>
    {
        private readonly Goblin goblin;

        public IGoblinCommand CurrentCommand { get; set; }

        public GoblinCommandState(Goblin goblin)
        {
            this.goblin = goblin;
        }

        public void Dispose()
        {
        }

        public void Update()
        {
            if (CurrentCommand == null) return;
            CurrentCommand.Update(goblin);
        }
    }
}
