﻿using System.Collections.Generic;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Scattered.Rendering;
using SlimDX;
using DirectX11;

namespace MHGameWork.TheWizards.Scattered.Model
{
    public class Island
    {
        private List<Island> connectedIslands = new List<Island>();
        public Island(Level level)
        {
            Level = level;
            Construction = level.createEmptyConstruction(this);
            RenderData = new IslandRenderData(this);
            Inventory = new Inventory();
            BridgeConnectors = new List<BridgeConnector>();
        }
        public Level Level { get; private set; }
        public Construction Construction { get; set; }

        public IslandType Type { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float RotationY { get; set; }

        public Vector3 GetForward()
        {
            return Matrix.RotationY(RotationY).xna().Forward.dx();
        }

        public Matrix GetWorldMatrix()
        {
            return Matrix.RotationY(RotationY)*Matrix.Translation(Position);
        }

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


        public override string ToString()
        {
            return "Island: " + Construction.Name;
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



        public List<BridgeConnector> BridgeConnectors { get; set; }

        public class BridgeConnector
        {
            private readonly Island island;

            public BridgeConnector(Island island)
            {
                this.island = island;
            }

            public Island Island
            {
                get { return island; }
            }

            public Vector3 RelativePosition;
            public Vector3 Direction;
            public BridgeConnector Connection;

            public Vector3 GetAbsolutePosition()
            {
                return (Matrix.Translation(RelativePosition)*island.GetWorldMatrix()).GetTranslation();
            }

            public Vector3 GetAbsoluteDirection()
            {
                return Vector3.TransformNormal(Direction, Matrix.RotationY(island.RotationY));
            }
        }
    }

  
}