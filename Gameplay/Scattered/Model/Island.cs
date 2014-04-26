using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered.Core;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using DirectX11;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class Island
    {
        private List<Island> connectedIslands = new List<Island>();
        public Island(Level level, SceneGraphNode node)
        {
            Level = level;
            Inventory = new Inventory();
            Node = node;


            var ent = level.CreateEntityNode(node.CreateChild());
            //ent.Node.Relative = Matrix.Scaling(2, 2, 2) * ent.Node.Relative;
            entity = ent.Entity;
            //ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Island_Large");
            SpaceAllocator = new IslandSpaceAllocator();
        }
        public IslandSpaceAllocator SpaceAllocator { get; set; }
        private readonly Entity entity;
        private IMesh mesh;
        public IMesh Mesh
        {
            get { return mesh; }
            set
            {
                mesh = value;
                entity.Mesh = value;
            }
        }

        public Level Level { get; private set; }

        public IslandType Type { get; set; }

        public Vector3 Position { get { return Node.Absolute.GetTranslation(); } set { Node.Relative = Node.Relative * Matrix.Translation(value - Position); } }
        public Vector3 Velocity { get; set; }

        public Vector3 GetForward()
        {
            return Node.Absolute.xna().Forward.dx();// Matrix.RotationY(RotationY).xna().Forward.dx();
        }

        public Matrix GetWorldMatrix()
        {
            return Node.Absolute; //Matrix.RotationY(RotationY) * Matrix.Translation(Position);
        }

        public void AddBridgeTo(Island isl2)
        {
            if (connectedIslands.Contains(isl2)) return;
            connectedIslands.Add(isl2);
            isl2.AddBridgeTo(this);
        }

        public IEnumerable<Island> ConnectedIslands { get { return connectedIslands; } }

        public Inventory Inventory { get; private set; }


        public override string ToString()
        {
            return "Island: ";
        }

        public BoundingBox GetBoundingBox()
        {
            var range = new Vector3(5.1f, 0.01f, 5.1f);
            return new BoundingBox(Position - range, Position + range);
        }


        private void getIslandsInCluster(Island island, HashSet<Island> visited)
        {
            if (visited.Contains(island)) return;
            visited.Add(island);

            island.ConnectedIslands.ForEach(i => getIslandsInCluster(i, visited));

        }
        public HashSet<Island> GetIslandsInCluster()
        {
            var hashSet = new HashSet<Island>();
            getIslandsInCluster(this, hashSet);
            return hashSet;
        }

        public enum IslandType
        {
            Normal,
            Resource,
            Tower
        }


        public SceneGraphNode Node { get; private set; }


        public IEnumerable<Bridge> BridgeConnectors { get { return Addons.OfType<Bridge>(); } }

        //private List<IIslandAddon> addons = new List<IIslandAddon>();
        public IEnumerable<IIslandAddon> Addons
        {
            get { return Node.Children.Select(c => c.AssociatedObject).OfType<IIslandAddon>(); }
        }

        public WorldGenerationService.IslandDescriptor Descriptor { get; set; }

        public void AddAddon(IIslandAddon addon)
        {
            if (!Node.Children.Contains(addon.Node)) throw new InvalidOperationException("Addon should be a direct child of the island node.");
            if (addon.Node.AssociatedObject != addon) throw new InvalidOperationException("Addon should be associated with its node.");
            //addons.Add(addon);
        }
    }
}