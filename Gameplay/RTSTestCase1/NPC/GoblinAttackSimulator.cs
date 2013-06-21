using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTS;
using MHGameWork.TheWizards.RTSTestCase1.Cannons;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    public class GoblinAttackSimulator : ISimulator
    {
        public GoblinMovementSimulatorSimple GoblinMovementSimulatorSimple { get; set; }
        public IWorldLocator WorldLocator { get; set; }
        public void Simulate()
        {
            var todo = new List<Action>();

            foreach (var g in TW.Data.Objects.OfType<Goblin>())
            {
                g.Goal = TW.Data.Get<LocalGameData>().LocalPlayer.Position;

                if (Vector3.Distance(g.Physical.GetPosition().TakeXZ().ToXZ(0)
                                     , TW.Data.Get<LocalGameData>().LocalPlayer.Position.TakeXZ().ToXZ(0)) 
                    < 1)
                    TW.Data.Get<LocalGameData>().LocalPlayer.Position = new Vector3();

                foreach (var close in WorldLocator.AtObject(g, 1))
                {
                    if (close is Cannon)
                    {
                        object close1 = close;
                        todo.Add(()=> TW.Data.Objects.Remove((IModelObject)close1));
                    }
                }
            }

            foreach (var t in todo) t();

            GoblinMovementSimulatorSimple.Simulate();
        }
    }
}