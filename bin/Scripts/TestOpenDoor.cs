using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scripting.API;
using Microsoft.Xna.Framework;

namespace Scripts
{
    public class TestOpenDoor : IScript
    {
        private bool open;

        private IEntityHandle handle;
        public void Init(IEntityHandle handle)
        {
            this.handle = handle;
            handle.RegisterUseHandler(onUse);
        }

        private void onUse(IPlayer obj)
        {
            open = !open;
            if (open)
            {
                handle.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2);

            }
            else
            {
                handle.Rotation = Quaternion.Identity;
            }
        }

        public void Destroy()
        {
        }
    }
}
