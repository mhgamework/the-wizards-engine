using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
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
            TW.Data.Get<CameraInfo>().ActivateSpecatorCamera();
            TW.Graphics.SpectaterCamera.Enabled = true;
            hotbarController.Update();
            var selected = hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot);
            if (selected != null) selected.Update();

        }

        private void simulateInventory()
        {
            if (!inInventory) return;
            TW.Graphics.SpectaterCamera.Enabled = false;

            hotbarController.Update();
            inventoryController.Update();

        }

        private void tryToggleInventory()
        {
            if (!TW.Graphics.Keyboard.IsKeyPressed(Key.E)) return;
            inInventory = !inInventory;
        }
    }
}