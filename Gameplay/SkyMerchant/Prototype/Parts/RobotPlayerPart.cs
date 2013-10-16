using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class RobotPlayerPart : EngineModelObject, IGameObjectComponent
    {
        #region "Injection"
        public IPositionComponent Physical { get; set; }
        public IWorldLocator WorldLocator { get; set; }
        public RobotPlayerNormalMovementPart NormalMovement { get; set; }
        public ISimulationEngine SimulationEngine { get; set; }
        public PrototypeObjectsFactory PrototypeObjectsFactory { get; set; }
        public IMeshRenderComponent MeshRenderComponent { get; set; }
        #endregion

        public RobotPlayerPart(IWorldLocator worldLocator, 
            RobotPlayerNormalMovementPart normalMovement, 
            ISimulationEngine simulationEngine, 
            PrototypeObjectsFactory prototypeObjectsFactory, 
            IMeshRenderComponent meshRenderComponent,
            IPositionComponent physical)
        {
            WorldLocator = worldLocator;
            NormalMovement = normalMovement;
            SimulationEngine = simulationEngine;
            PrototypeObjectsFactory = prototypeObjectsFactory;
            MeshRenderComponent = meshRenderComponent;
            Physical = physical;

            Items = new List<ItemPart>();
            Health = 100;
        }

        #region Items

        public List<ItemPart> Items { get; set; }

        public void Pickup(ItemPart item)
        {
            if (!CanPickup(item.Type, 1)) return;
            Contract.Requires(!HasItem(item));
            Items.Add(item);

            item.RemoveFromIsland();
        }

        public void Drop(ItemPart item)
        {
            Contract.Requires(HasItem(item));
            if (!NormalMovement.IsOnGround()) return;
            Items.Remove(item);

            item.PlaceOnIsland(NormalMovement.GetPositionIsland());
        }

        private bool HasItem(ItemPart item)
        {
            return Items.Contains(item);
        }

        public void PickupClosest()
        {
            var closest = WorldLocator.AtObject<ItemPart>(Physical, 2f);

            var item = closest.FirstOrDefault(i => !i.InStorage);
            if (item == null) return;

            Pickup(item);

        }

        /// <summary>
        /// Warning, storing this enumerable and using it at a later time may cause problems!
        /// Consume the enumerator directly or convert the return value to a list.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public IEnumerable<ItemPart> TakeItems(ItemType type, int amount)
        {
            Contract.Requires(HasItems(type, amount));
            for (int i = 0; i < amount; i++)
            {
                var item = Items.First(it => it.Type == type);
                Drop(item);
                yield return item;
            }

        }

        public bool HasItems(ItemType type, int amount)
        {
            return Items.Count(i => i.Type == type) >= amount;
        }

        public bool CanPickup(ItemType type, int amount)
        {
            return true; // unlimited inventory!
        }

        #endregion

        #region Movement + flying


        public bool Flying { get; set; }
        public IslandPart FlyingIsland { get; set; }

        public void ToggleFly()
        {
            if (Flying)
            {
                FlyingIsland = null;
                Flying = false;
                return;
            }

            if (NormalMovement.GetPositionIsland() == null) return; // Not on island
            FlyingIsland = NormalMovement.GetPositionIsland();
            Flying = true;

        }

        /// <summary>
        /// Alternative design idea: create a decorator for the RobotNormalMovementPart, and have the simulate method do nothing when flying is enabled here
        /// </summary>
        public void SimulateMovement()
        {
            if (Flying)
            {
                //TODO: copy flying from other unit test
            }
            else
            {
                NormalMovement.SimulateMovement();
            }
            if (Physical.Position.Y < -20)
            {
                Physical.Position = Physical.Position.ChangeY(50);
                NormalMovement.Velocity = new Vector3();
            }
        }

        #endregion

        #region Health and cogs

        public float Health { get; set; }

        public void SimulateCogConsumption()
        {
            Health -= SimulationEngine.Elapsed * 0.2f;

            var cogRepairAmount = 50;
            if (Health < cogRepairAmount)
            {
                // Try consume cog
                var cog = Items.FirstOrDefault(o => o.Type == PrototypeObjectsFactory.CogType);
                if (cog != null)
                {
                    Items.Remove(cog);
                    Health += cogRepairAmount;
                }
            }
        }

        public void SimulateDeath()
        {
            if (Health > 0) return;

            //TODO: die!!

        }

        public void ApplyDamage(float gunDamage)
        {
            Health -= gunDamage;
            Physical.Position = Physical.Position - NormalMovement.LookDirection.ChangeY(0) * 3;
        }

        #endregion


    }
}