using System;
using System.Linq;
using System.Timers;
using MHGameWork.TheWizards.SkyMerchant._Undocumented;
using bbv.Common.StateMachine;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.Inventory
{
    /// <summary>
    /// Responsible for processing user input and providing display for the user, for working with the inventory
    /// </summary>
    public class InventoryController
    {
        private readonly HotbarController hotbarController;
        private readonly InventoryView3D view;
      

        public InventoryController(HotbarController hotbarController, InventoryView3D view)
        {
            this.hotbarController = hotbarController;
            this.view = view;

            var timer = new Timer(1000);
            timer.Elapsed += (sender, args) => timerElapsed = true;
            fsm = createStateMachine(timer);

            fsm.Start();
        }
        public void Update()
        {
            updatePickupStateMachine();
        }

        #region Pickup state machine
        private PassiveStateMachine<States, Events> fsm;
        private bool timerElapsed = false;


        private void updatePickupStateMachine()
        {
            var down = hotbarController.GetDownSlots();

            if (down.Count() == 0) fsm.Fire(Events.NoSlotPressed);
            else if (down.Count() == 1) fsm.Fire(Events.SingleSlotPressed);
            else fsm.Fire(Events.MultipleSlotPressed);
            if (timerElapsed)
            {
                timerElapsed = false;
                fsm.Fire(Events.Timer);
            }
        }

        private PassiveStateMachine<States, Events> createStateMachine(Timer timer)
        {
            var fsm = new PassiveStateMachine<States, Events>();
            fsm.AddExtension(new ConsoleLoggingExtension<States, Events>());

            fsm.Initialize(States.None);


            fsm.In(States.None).On(Events.SingleSlotPressed).Goto(States.Holding);

            fsm.In(States.Holding).ExecuteOnEntry(timer.Start);
            fsm.In(States.Holding).ExecuteOnExit(timer.Stop);
            fsm.In(States.Holding)
               .On(Events.MultipleSlotPressed).Goto(States.None);
            fsm.In(States.Holding)
               .On(Events.NoSlotPressed).Goto(States.None);
            fsm.In(States.Holding)
               .On(Events.Timer).Goto(States.Complete).Execute(SelectTargetedInventoryItem);

            fsm.In(States.Complete).On(Events.NoSlotPressed).Goto(States.None);

            return fsm;
        }

        private enum States
        {
            None,
            Holding,
            Complete
        }
        private enum Events
        {
            SingleSlotPressed,
            MultipleSlotPressed,
            NoSlotPressed,
            Timer
        }

        #endregion

        public void SelectTargetedInventoryItem()
        {
            
        }

    }
}