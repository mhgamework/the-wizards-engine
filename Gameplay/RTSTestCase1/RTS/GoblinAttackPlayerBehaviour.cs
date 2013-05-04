using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinAttackPlayerBehaviour
    {
        public void TryAttack(Goblin g)
        {
            g.get<GoblinMover>().MoveTo(TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx());
        }
    }
}
