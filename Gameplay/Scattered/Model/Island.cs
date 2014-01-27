using System.Collections.Generic;
using MHGameWork.TheWizards.Scattered.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class Island
    {
        private List<Island> connectedIslands = new List<Island>();
        public Island()
        {
            Construction = new Construction() { Name = "Empty" };
            RenderData = new IslandRenderData(this);
            Inventory = new Inventory();
        }
        public Construction Construction { get; set; }

        public Vector3 Position { get; set; }

        public void AddBridgeTo(Island isl2)
        {
            if (connectedIslands.Contains(isl2)) return;
            connectedIslands.Add(isl2);
            isl2.AddBridgeTo(this);
        }

        public IEnumerable<Island> ConnectedIslands { get { return connectedIslands; } }

        /// <summary>
        /// This is a layer leak. This should only be called from the Rendering layer and is here for simplicity of writing, since data is in aggregation with the Island anyways.
        /// </summary>
        public IslandRenderData RenderData { get; set; }

        public Inventory Inventory { get; private set; }
    }
}