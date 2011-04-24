using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Scripting.API;
using Microsoft.Xna.Framework;

namespace Scripts
{
    public class TestScript : IScript, IUpdateHandler
    {
        private IEntityHandle handle;

        private float velocity;

        public void Init(IEntityHandle handle)
        {
            this.handle = handle;
            handle.RegisterUpdateHandler();
        }

        public void Destroy()
        {
        }




        public void Update()
        {
            var elapsed = 0.01f;
            var acceleration = handle.Position.Z * 2;
            velocity += acceleration * elapsed;
            handle.Position += Vector3.Forward * velocity * elapsed;
            if (Math.Abs(velocity) < 0.5 && Math.Abs(handle.Position.Z) < 1) velocity = 5;



        }
    }
}
