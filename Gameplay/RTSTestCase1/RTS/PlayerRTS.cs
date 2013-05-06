using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class PlayerRTS : EngineModelObject
    {
        public Thing Holding { get; set; }
        public bool Dead { get; set; }

        public Vector3 GetPosition()
        {
            return TW.Data.GetSingleton<CameraInfo>().ActiveCamera.ViewInverse.xna().Translation.dx();
        }

        public void Die()
        {
            Dead = true;
        }
    }
}
