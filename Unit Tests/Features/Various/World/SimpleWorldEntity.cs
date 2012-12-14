using System;
using MHGameWork.TheWizards.World;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various.World
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
