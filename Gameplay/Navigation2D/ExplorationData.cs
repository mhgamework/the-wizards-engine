using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Navigation2D
{
    [ModelObjectChanged]
    public class ExplorationData: EngineModelObject
    {
        public ExplorationData()
        {
            Waypoints = new List<Waypoint>();
        }
        public List<Waypoint> Waypoints { get; set; }
    }
}
