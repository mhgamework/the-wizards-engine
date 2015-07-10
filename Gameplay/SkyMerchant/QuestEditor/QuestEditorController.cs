using System;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.HotbarCore;
using MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore;
using SlimDX.DirectInput;
using bbv.Common.StateMachine;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor
{
    /// <summary>
    /// Controls the hotbar and inventory features of the quest editor.
    /// </summary>
    public class QuestEditorController
    {
        private HotbarController hotbarController;
        private readonly PlayerRobotSimulator playerRobotSimulator;
        private InventoryController inventoryController;

        private PassiveStateMachine<States, Events> fsm;
        private States ActiveState;

        public QuestEditorController(InventoryController inventoryController, HotbarController hotbarController, PlayerRobotSimulator playerRobotSimulator)
        {
            this.inventoryController = inventoryController;
            this.hotbarController = hotbarController;
            this.playerRobotSimulator = playerRobotSimulator;

            createStateMachine();
            fsm.Start();
        }

        public void Update()
        {
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.E)) fsm.Fire(Events.InventoryKey);
            if (TW.Graphics.Keyboard.IsKeyPressed(Key.Return)) fsm.Fire(Events.PlayKey);


            simulateInventory();
            simulateWorld();
            simulatePlayMode();
        }

        private void simulateWorld()
        {
            if (ActiveState != States.Edit) return;
            TW.Data.Get<CameraInfo>().ActivateSpecatorCamera();
            TW.Graphics.SpectaterCamera.Enabled = true;
            hotbarController.Update();
            var selected = hotbarController.Bar.GetHotbarItem(hotbarController.Bar.SelectedSlot);
            if (selected != null) selected.Update();

        }

        private void simulateInventory()
        {
            if (ActiveState != States.Inventory) return;

            TW.Graphics.SpectaterCamera.Enabled = false;

            hotbarController.Update();
            inventoryController.Update();

        }

        private void simulatePlayMode()
        {
            if (ActiveState != States.Play) return;

            playerRobotSimulator.ActivateRobotCamera();
            playerRobotSimulator.SimulateRobotNonUserInput();
            playerRobotSimulator.SimulateRobotUserInput();
        }

        private void createStateMachine()
        {
            fsm = new PassiveStateMachine<States, Events>();
            fsm.In(States.Edit)
               .On(Events.InventoryKey).Goto(States.Inventory)
               .On(Events.PlayKey).Goto(States.Play);

            fsm.In(States.Inventory).On(Events.InventoryKey).Goto(States.Edit);

            fsm.In(States.Play).On(Events.PlayKey).Goto(States.Edit);


            foreach (var s in Enum.GetValues(typeof(States)).Cast<States>())
                fsm.In(s).ExecuteOnEntry(new Action(() => ActiveState = s));

            fsm.Initialize(States.Edit);



        }

        private enum States
        {
            Inventory,
            Edit,
            Play
        }
        private enum Events
        {
            InventoryKey,
            PlayKey
        }
    }
}