using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS.Commands;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.RTS
{
    public class FactorySimulator : ISimulator
    {
        public void Simulate()
        {
            TW.Data.EnsureAttachment<Factory, FactoryBuilder>(o => new FactoryBuilder(o));
            foreach (Factory f in TW.Data.Objects.Where(o => o is Factory))
                f.get<FactoryBuilder>().Update();
        }
    }

    public class FactoryBuilder : IModelObjectAddon<Factory>
    {
        private readonly Factory factory;

        private float nextUpdate ;

        public FactoryBuilder(Factory factory)
        {
            this.factory = factory;
            nextUpdate = TW.Graphics.TotalRunTime + factory.BuildInterval;
        }

        public void Update()
        {
            if (nextUpdate < TW.Graphics.TotalRunTime)
            {
                buildSingle();
                nextUpdate += factory.BuildInterval;
            }
        }

        private void buildSingle()
        {
            var dropped = TW.Data.Objects.Where(o => o is DroppedThing)
              .Cast<DroppedThing>()
              .Where(o=>o.Thing.Type == factory.InputType)
              .FirstOrDefault(o => factory.GetInputArea().xna().Contains(o.Position.xna()) == ContainmentType.Contains);
            if (dropped == null) return;

            TW.Data.RemoveObject(dropped);

            var thing = new Thing() { Type = factory.OutputType };
            var area = factory.GetOutputArea();
            var pos = (area.Maximum + area.Minimum) * 0.5f;
            new DroppedThing() { Position = pos, Thing = thing };
        }

        public void Dispose()
        {
        }
    }
}
