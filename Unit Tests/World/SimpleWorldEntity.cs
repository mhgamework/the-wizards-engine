using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.World;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.World
{
    public class SimpleWorldEntity: IWorldEntity
    {
        public int Health;
        public Vector3 Position;
        public Matrix Orientation;

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
