using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Internal.Networking
{
    /// <summary>
    /// Responsible for identifying the local player state
    /// </summary>
    public class LocalPlayerService
    {
        public PlayerState Player;
        public LocalPlayerService(GameState gameState)
        {
            Player = new PlayerState();
            gameState.AddPlayer(Player);
        }
    }
}