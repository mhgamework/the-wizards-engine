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

		private float angle;
		
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
            var acceleration = handle.Position.Z * 10;
            velocity += acceleration * elapsed;
            handle.Position += (Vector3.Forward+Vector3.Up) * velocity * elapsed;
            if (Math.Abs(velocity) < 0.5 && Math.Abs(handle.Position.Z) < 1) velocity = 5;

			
			angle += elapsed;

			handle.Rotation = Quaternion.CreateFromAxisAngle(Vector3.Forward,angle);

        }
    }
}
