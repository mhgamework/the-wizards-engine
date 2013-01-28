using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    [ModelObjectChanged]
    public class Waypoint : EngineModelObject
    {
        public Waypoint()
        {
            Edges = new List<Edge>();
        }
        public Vector2 Position { get; set; }
        public List<Edge> Edges { get; set; }

        public struct Edge
        {
            public Waypoint Target;
            public float Distance;
        }
    }
}