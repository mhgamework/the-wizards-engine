using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Pathing
{
    public class SimpleAIMover : IAIMover
    {
        private IAI ai;

        public float Speed { get; set; }

        public SimpleAIMover(IAI ai)
        {
            this.ai = ai;
        }

        private Vector3 target;
        public void SetTarget(Vector3 target)
        {
            this.target = target;
        }

        public void Update(float elapsed)
        {
            var dir = target - ai.Position;
            dir.Normalize();
            ai.Position = ai.Position + dir*Speed*elapsed;

        }
    }
}
