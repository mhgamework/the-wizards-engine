using MHGameWork.TheWizards.Player;

namespace MHGameWork.TheWizards._XNA.Gameplay
{
    public class SimplePlayer : IPlayer
    {
        public SimplePlayer()
        {
            Data = new PlayerData();
        }

        public PlayerData Data { get; set; }
        public PlayerData GetData()
        {
            return Data;
        }
    }
}
