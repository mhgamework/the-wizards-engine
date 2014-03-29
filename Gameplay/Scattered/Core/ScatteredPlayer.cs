﻿using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using MHGameWork.TheWizards.Scattered._Engine;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class ScatteredPlayer
    {
        private readonly Level level;
        private readonly SceneGraphNode node;
        private Vector3 position;
        private Vector3 direction;
        private EntityNode itemNode;
        private ItemType heldItem;

        public ScatteredPlayer(Level level, SceneGraphNode node)
        {
            this.level = level;
            this.node = node;

            itemNode = level.CreateEntityNode(node.CreateChild().Alter(c => c.Relative = Matrix.Translation(0, -0.5f, -3)));
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                updateNode();
            }
        }

        private void updateNode()
        {
            node.Absolute = Matrix.Invert(Matrix.LookAtRH(position, position + direction, Vector3.UnitY));
        }

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                updateNode();
            }
        }

        /// <summary>
        /// Contains the island the player is currently flying
        /// </summary>
        public Island FlyingIsland { get; set; }

        public Inventory Inventory = new Inventory();


        public void TryPickupResource(Resource resource)
        {
            if (Inventory.ItemCount > 0 && Inventory.Items.First() != resource.Type) return;
            Inventory.AddNewItems(resource.Type, resource.Amount);
            level.DestroyNode(resource.Node);

        }
        public void AttemptDropResource()
        {

            if (Inventory.ItemCount == 0) return;
            var island = GetLookedAtIsland();
            if (island == null) return;

            island.AddAddon(new Resource(level, island.Node.CreateChild(), Inventory.Items.First())
                .Alter(k => k.Amount = 1)
                .Alter(k => k.Node.Position = GetLookedAtIslandSurfacePoint().Value));
            Inventory.DestroyItems(Inventory.Items.First(), 1);


        }

        public Vector3? GetLookedAtIslandSurfacePoint()
        {
            var r = new IslandWalkPlaneRaycaster(level);
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            var dist = r.onRaycastIsland(ray);
            if (dist == null) return null;
            return ray.GetPoint(dist.Value);
        }

        public Island GetLookedAtIsland()
        {
            var r = new IslandWalkPlaneRaycaster(level);
            var ray = TW.Data.Get<CameraInfo>().GetCenterScreenRay();
            return r.onRaycastIslandReturnIsland(ray);
        }

    }
}