using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class ClearGameStateChangesService : ISimulator
    {
        private readonly GameState state;

        public ClearGameStateChangesService(GameState state)
        {
            this.state = state;
        }

        public void Simulate()
        {
            state.World.ClearChangedFlags();
        }
    }
}