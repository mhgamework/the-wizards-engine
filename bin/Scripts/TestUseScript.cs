using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scripting.API;

namespace Scripts
{
    public class TestUseScript : IScript
    {
        public void Init(IEntityHandle handle)
        {
            handle.RegisterUseHandler(onPlayerUse);
        }

        private void onPlayerUse(IPlayer obj)
        {
            var data = obj.GetData();

            Console.WriteLine("Use called!");
            Console.WriteLine("Player (" + obj.GetData().Name + ") called use!");
        }

        public void Destroy()
        {
        }



    }
}
