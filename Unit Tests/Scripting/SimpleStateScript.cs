using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Scripting;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Tests.Scripting
{
    public class SimpleStateScript : IStateScript
    {
        private float time;

        public void Init()
        {
            Console.WriteLine("SimpleStateScript Init!");
        }

        public void Destroy()
        {
            Console.WriteLine("SimpleStateScript Destroy!");

        }

        public void Update()
        {
            time += ScriptLayer.Elapsed;
        }

        public void Draw()
        {
            ScriptLayer.LineManager.DrawGroundShadows = true;
            ScriptLayer.LineManager.AddLine(Vector3.One, new Vector3(1+(float)Math.Sin(time), 2, 1+(float)Math.Cos(time)), Color.Red);
        }
    }
}
