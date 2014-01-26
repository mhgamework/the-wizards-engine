using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered
{
    public class Island
    {
        private List<Island> connectedIslands = new List<Island>();
        public Island()
        {

        }
        public Construction Construction { get; private set; }

        public Vector3 Position { get; set; }

        public void AddBridgeTo(Island isl2)
        {
            if (connectedIslands.Contains(isl2)) return;
            connectedIslands.Add(isl2);
            isl2.AddBridgeTo(this);
        }

        public IEnumerable<Island> ConnectedIslands { get { return connectedIslands; } }
    }
}