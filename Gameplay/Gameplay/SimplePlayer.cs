using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Player;

namespace MHGameWork.TheWizards.Gameplay
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
