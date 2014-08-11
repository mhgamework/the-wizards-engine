using System.Collections.Generic;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class GameState
    {
        private List<PlayerState> players;

        public GameState(World world)
        {
            World = world;
            players = new List<PlayerState>();

        }

        public World World { get; private set; }
        public IEnumerable<PlayerState> Players
        {
            get { return players; }
        }

        public void AddPlayer(PlayerState player)
        {
            players.Add(player);
        }


    }
}