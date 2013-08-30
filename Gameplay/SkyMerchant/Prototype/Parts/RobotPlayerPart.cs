using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using System.Linq;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._Windsor;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class RobotPlayerPart : EngineModelObject, IPhysical
    {
        #region "Injection"
        public Physical Physical { get; set; }


        [NonOptional]
        public IWorldLocator WorldLocator { get; set; }
        public RobotPlayerNormalMovementPart NormalMovement { get; set; }
        [NonOptional]
        public ISimulationEngine SimulationEngine { get; set; }
        #endregion

        public RobotPlayerPart()
        {
            Items = new List<ItemPart>();
            Health = 100;
        }


        public void UpdatePhysical()
        {

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
        }

        #endregion

        #region Health and cogs

        public float Health { get; set; }
        public float Cogs { get; set; }

        public void SimulateCogConsumption()
        {
            Cogs -= SimulationEngine.Elapsed * 20;
            if (Cogs > 0) return;

            Cogs = 0;
            Health = 0;
        }

        public void SimulateDeath()
        {
            if (Health > 0) return;

            //TODO: die!!

        }

        public void ApplyDamage(float gunDamage)
        {
            Health -= gunDamage;
            Physical.SetPosition(Physical.GetPosition() - NormalMovement.LookDirection.ChangeY(0) * 3);
        }

        #endregion


    }
}