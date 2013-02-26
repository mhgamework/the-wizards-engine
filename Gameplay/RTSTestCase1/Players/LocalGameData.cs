using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Synchronization;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Holds data containing local game info, should not be synced!!
    /// </summary>
    [ModelObjectChanged]
    [NoSync]
    public class LocalGameData : EngineModelObject
    {
        public UserPlayer LocalPlayer { get; set; }

        public LocalGameData()
        {
            LocalPlayer = new UserPlayer();
        }
    }
}
