using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Pickup;
using MHGameWork.TheWizards.Player;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Simulators
{
    /// <summary>
    /// Responsible for simulating the picking up of IItems
    /// </summary>
    public class PickupSimulator : ISimulator
    {
        private Inventory inventory;
        private ItemEntityFactory itemEntityFactory;
        private readonly PlayerData player;

        private bool showInventory;
        private bool showInventoryChanged;
        private bool selectChanged;
        private bool pickupChanged;
        private bool dropChanged;

        public PickupSimulator(ItemEntityFactory itemEntityFactory, PlayerData player)
        {
            inventory = new Inventory();
            this.itemEntityFactory = itemEntityFactory;
            this.player = player;
        }

        public void Simulate()
        {
            var game = TW.Graphics;

            if(game.Keyboard.IsKeyDown(Key.E) && !pickupChanged)
            {
                tryPickupItem();
                pickupChanged = true;
            }
            if (!game.Keyboard.IsKeyDown(Key.E))
            {
                pickupChanged = false;
            }

            if(game.Keyboard.IsKeyDown(Key.Q) && !dropChanged)
            {
                dropItem();
                dropChanged = true;
            }
            if (!game.Keyboard.IsKeyDown(Key.Q))
            {
                dropChanged = false;
            }

            if(game.Keyboard.IsKeyDown(Key.I) && !showInventoryChanged)
            {
                showInventory = !showInventory;
                showInventoryChanged = true;
            }
            if(!game.Keyboard.IsKeyDown(Key.I))
            {
                showInventoryChanged = false;
            }

            if(game.Keyboard.IsKeyDown(Key.F) && !selectChanged)
            {
                inventory.SelectNextItem();
                selectChanged = true;
            }
            if(!game.Keyboard.IsKeyDown(Key.F))
            {
                selectChanged = false;
            }
            

            inventory.SetVisibility(showInventory);
            inventory.Update();

        }

        private void tryPickupItem()
        {
            var itemList = itemEntityFactory.ItemList;

            ItemEntity selectedItem = null;
            float dist = 1000;

            var game = TW.Graphics;
            var rayPos = game.Camera.ViewInverse.xna().Translation.dx();
            var rayDir = game.Camera.ViewInverse.xna().Forward.dx();

            foreach (ItemEntity cItem in itemList)
            {
                if (!cItem.RayTraceable)
                    continue;

                Ray ray = new Ray(rayPos - cItem.GetPosition(), Vector3.TransformNormal(rayDir, Matrix.RotationQuaternion(cItem.GetRotation())));

                var intersects = ray.xna().Intersects(cItem.BB.xna());
                if (intersects != null && intersects < dist)
                {
                    dist = (float)intersects;
                    selectedItem = cItem;
                }
            }

            if(selectedItem != null)
            {
                inventory.AddItem(selectedItem);
                selectedItem.RayTraceable = false;
                selectedItem.SetVisibility(false);
            }
        }

        private void dropItem()
        {
            var drop = inventory.SelectedItem;

            if (drop == null)
                return;

            inventory.RemoveItem(drop);

            drop.SetPosition(Matrix.Translation(player.Position));
            drop.RayTraceable = true;
            drop.SetVisibility(true);
        }
    }
}
