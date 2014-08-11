using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class ClearStateChangesSimulator : ISimulator
    {
        private readonly GameState state;

        public ClearStateChangesSimulator(GameState state)
        {
            this.state = state;
        }

        public void Simulate()
        {
            state.World.ClearChangedFlags();
        }
    }
}