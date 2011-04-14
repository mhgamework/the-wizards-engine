using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Gameplay
{
    public interface IPlayerMove
    {
        void StartPrimaryAttack();
        void EndPrimaryAttack();
        void StartSecondaryAttack();
        void EndSecondaryAttack();

    }
}
