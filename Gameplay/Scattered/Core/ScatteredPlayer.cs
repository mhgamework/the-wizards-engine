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
    public class ScatteredPlayer : IHasNode
    {
        private readonly Level level;
        private readonly SceneGraphNode node;
        private Vector3 position;
        private Vector3 direction;
        private EntityNode itemNode;
        private ItemType heldItem;
        public float Health { get; private set; }

        public ScatteredPlayer(Level level, SceneGraphNode node)
        {
            this.level = level;
            this.node = node;

            itemNode = level.CreateEntityNode(node.CreateChild().Alter(c => c.Relative = Matrix.Translation(0, -0.5f, -3)));
            Health = 1;
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

        /// <summary>
        /// Todo: change this with an object which identifies the current movement type + who is requesting/invoking this type of movement
        /// Eg: flight, jumppad, ...
        /// This can be a single object since it is not possible in any case to have 2 movement types at the same time :)
        /// </summary>
        public bool MovementDisabled { get; set; }

        public FlightEngine FlyingEngine { get; set; }

        public Inventory Inventory = new Inventory();


        public void TryPickupResource(Resource resource)
        {
            if (Inventory.ItemCount > 0 && Inventory.Items.First() != resource.Type) return;

            var maxInventorySize = 40;

            var transferSize = Math.Min(resource.Amount, maxInventorySize - Inventory.ItemCount);
            Inventory.AddNewItems(resource.Type, transferSize);
            resource.Amount -= transferSize;

            if (resource.Amount < 0) throw new InvalidOperationException("Programming error check!");

            if (resource.Amount == 0)
                level.DestroyNode(resource.Node);

        }
        public void AttemptDropResource()
        {

            if (Inventory.ItemCount == 0) return;
            var island = GetLookedAtIsland();
            if (island == null) return;

            TW.Graphics.LineManager3D.AddCenteredBox(GetLookedAtIslandSurfacePoint().Value, 0.5f, new Color4(0, 1, 0));

            island.AddAddon(new Resource(level, island.Node.CreateChild(), Inventory.Items.First())
                .Alter(k => k.Amount = 1)
                .Alter(k => k.Node.Position = GetLookedAtIslandSurfacePoint().Value));
            Inventory.DestroyItems(Inventory.Items.First(), 1);


        }


        private float healing = 0;
        public void AttemptHeal()
        {

            if (healing > 0)
            {
                Health += TW.Graphics.Elapsed / 10f;
                healing -= TW.Graphics.Elapsed / 10f;
                if (Health > 1) Health = 1;
                return;
            }
            if (Health >= 1)
            {
                Health = 1;
                healing = 0;
                return;
            }
            var closeFries = level.Islands.SelectMany(i => i.Addons).OfType<Resource>()
                .Where(r => r.Type == level.FriesType && r.Amount != 0)
                .Where(r => Vector3.Distance(Position, r.Node.Position) < 3);

            if (!closeFries.Any()) return;
            var fries = closeFries.First();

            fries.Amount--;
            if (fries.Amount == 0)
                level.DestroyNode(fries.Node);

            healing = 0.1f;



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

        public Ray GetTargetingRay()
        {
            return TW.Data.Get<CameraInfo>().GetCenterScreenRay();
        }

        public void StopFlight()
        {
            if (FlyingIsland == null) return;
            Position = FlyingEngine.Node.Position + Vector3.UnitY * 5;
            Direction = TW.Graphics.Camera.ViewInverse.xna().Forward.dx();
            FlyingIsland = null;
            FlyingEngine = null;
        }

        public Enemy GetTargetedEnemy()
        {
            var ray = GetTargetingRay();
            var cast = level.Islands.SelectMany(i => i.Addons.OfType<Enemy>()).Raycast(e => e.LocalBoundingBox,
                                                                            e => e.Node.Absolute, ray);
            if (!cast.IsHit) return null;
            return (Enemy)cast.Object;
        }

        public void Shoot()
        {
            var target = GetTargetedEnemy();
            if (target == null) return;
            target.Kill();
            level.DestroyNode(target.Node);

        }
        public void TakeDamage(float amount)
        {
            Health -= amount;
        }

        public SceneGraphNode Node { get { return node; } }
    }
}