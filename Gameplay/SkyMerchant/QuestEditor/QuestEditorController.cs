﻿using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Controls the hotbar and inventory features of the quest editor.
    /// </summary>
    public class QuestEditorController
    {
        private HotbarController hotbarController;
        private InventoryController inventoryController;

        private bool inInventory;

        public QuestEditorController(InventoryController inventoryController, HotbarController hotbarController)
        {
            this.inventoryController = inventoryController;
            this.hotbarController = hotbarController;
        }

        public void Update()
        {
            tryToggleInventory();

            simulateInventory();
            simulateWorld();
        }

        private void simulateWorld()
        {
            if (inInventory) return;
            hotbarController.Update();
            var selected = hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot);
            if (selected != null) selected.Update();

        }

        private void simulateInventory()
        {
            if (!inInventory) return;

            inventoryController.Update();

        }

        private void tryToggleInventory()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.E)) return;
            inInventory = !inInventory;
        }
    }
}