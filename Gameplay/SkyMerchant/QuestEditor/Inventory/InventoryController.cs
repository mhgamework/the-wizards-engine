using System;
using System.Linq;
using System.Timers;
using bbv.Common.StateMachine;
using bbv.Common.StateMachine.Internals;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.Inventory
{
    /// <summary>
    /// Responsible for processing user input and providing display for the user, for working with the inventory
    /// </summary>
    public class InventoryController
    {
        private readonly HotbarController hotbarController;
        private PassiveStateMachine<States, Events> fsm;

        private bool timerElapsed = false;

        public InventoryController(HotbarController hotbarController)
        {
            this.hotbarController = hotbarController;

            var timer = new Timer(1000);
            timer.Elapsed += (sender, args) => timerElapsed = true;
            fsm = createStateMachine(timer);

            fsm.Start();
        }
        public void Update()
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

        public void SelectTargetedInventoryItem()
        {
            Console.WriteLine("Selected!");
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

    }

    public class ConsoleLoggingExtension<T, U> : IExtension<T, U>
        where T : IComparable
        where U : IComparable
    {
        public void StartedStateMachine(IStateMachineInformation<T, U> stateMachine)
        {

        }

        public void StoppedStateMachine(IStateMachineInformation<T, U> stateMachine)
        {
        }

        public void EventQueued(IStateMachineInformation<T, U> stateMachine, U eventId, object[] eventArguments)
        {
        }

        public void EventQueuedWithPriority(IStateMachineInformation<T, U> stateMachine, U eventId, object[] eventArguments)
        {
        }

        public void SwitchedState(IStateMachineInformation<T, U> stateMachine, IState<T, U> oldState, IState<T, U> newState)
        {
            Console.WriteLine("Entered state {0}", newState.Id);
        }

        public void InitializingStateMachine(IStateMachineInformation<T, U> stateMachine, ref T initialState)
        {
        }

        public void InitializedStateMachine(IStateMachineInformation<T, U> stateMachine, T initialState)
        {
        }

        public void EnteringInitialState(IStateMachineInformation<T, U> stateMachine, T state)
        {
        }

        public void EnteredInitialState(IStateMachineInformation<T, U> stateMachine, T state, IStateContext<T, U> stateContext)
        {
        }

        public void FiringEvent(IStateMachineInformation<T, U> stateMachine, ref U eventId, ref object[] eventArguments)
        {
        }

        public void FiredEvent(IStateMachineInformation<T, U> stateMachine, ITransitionContext<T, U> context)
        {
        }

        public void HandlingEntryActionException(IStateMachineInformation<T, U> stateMachine, IState<T, U> state, IStateContext<T, U> stateContext,
                                                 ref Exception exception)
        {
        }

        public void HandledEntryActionException(IStateMachineInformation<T, U> stateMachine, IState<T, U> state, IStateContext<T, U> stateContext,
                                                Exception exception)
        {
        }

        public void HandlingExitActionException(IStateMachineInformation<T, U> stateMachine, IState<T, U> state, IStateContext<T, U> stateContext,
                                                ref Exception exception)
        {
        }

        public void HandledExitActionException(IStateMachineInformation<T, U> stateMachine, IState<T, U> state, IStateContext<T, U> stateContext,
                                               Exception exception)
        {
        }

        public void HandlingGuardException(IStateMachineInformation<T, U> stateMachine, ITransition<T, U> transition,
                                           ITransitionContext<T, U> transitionContext, ref Exception exception)
        {
        }

        public void HandledGuardException(IStateMachineInformation<T, U> stateMachine, ITransition<T, U> transition,
                                          ITransitionContext<T, U> transitionContext, Exception exception)
        {
        }

        public void HandlingTransitionException(IStateMachineInformation<T, U> stateMachine, ITransition<T, U> transition,
                                                ITransitionContext<T, U> context, ref Exception exception)
        {
        }

        public void HandledTransitionException(IStateMachineInformation<T, U> stateMachine, ITransition<T, U> transition,
                                               ITransitionContext<T, U> transitionContext, Exception exception)
        {
        }
    }
}