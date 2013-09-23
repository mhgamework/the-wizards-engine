using System;
using bbv.Common.StateMachine;
using bbv.Common.StateMachine.Internals;

namespace MHGameWork.TheWizards.SkyMerchant._Undocumented
{
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