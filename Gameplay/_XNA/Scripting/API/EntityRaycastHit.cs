using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Scripting.API
{
    public struct EntityRaycastHit
    {
        public bool IsHit;
        public float Distance;
        public Vector3 WorldImpact;
        public Vector3 WorldNormal;

        public IEntity Entity;


        public static EntityRaycastHit NoHit { get { return new EntityRaycastHit { IsHit = false }; } }

    }
}
