using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    public class WaypointVisualizer : ISimulator
    {
        public void Simulate()
        {
            var data = TW.Data.GetSingleton<ExplorationData>();

            foreach (Waypoint wp in data.Waypoints)
            {
                TW.Graphics.LineManager3D.AddCenteredBox(wp.Position.ToXZ(0.5f), 0.5f, new Color4(1, 0, 1));
                foreach (Waypoint.Edge e in wp.Edges)
                {
                    TW.Graphics.LineManager3D.AddLine(wp.Position.ToXZ(0.5f), e.Target.Position.ToXZ(0.5f), new Color4(0.5f, 0, 0.5f));    
                }
            }
        }
    }
}
