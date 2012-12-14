﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Tests.Physics
{
    public class MeshPhysicsFactoryXNA : IXNAObject
    {
        public MeshPhysicsElementFactory Factory { get; private set; }

        public MeshPhysicsFactoryXNA(PhysicsEngine engine, ClientPhysicsQuadTreeNode root)
        {
            Factory = new MeshPhysicsElementFactory(engine, root);
        }

        public void Initialize(IXNAGame _game)
        {
            Factory.Initialize();
        }

        public void Render(IXNAGame _game)
        {
        }

        public void Update(IXNAGame _game)
        {
            Factory.Update();
        }
    }
}
