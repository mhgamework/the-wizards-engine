using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Cogs
{
    public class CogRaycastReport : UserRaycastReport
    {
        public CogEngine CogEngine { get; set; }
        public List<RaycastHit> Hits { get; private set; }

        public CogRaycastReport ( CogEngine cogEngine)
        {
            CogEngine = cogEngine;
            Hits = new List<RaycastHit>();
        }

        public override bool OnHit(RaycastHit hits)
        {
            Vector3 pos;

            pos = hits.WorldImpact;

            Hits.Add(hits);

            return true; // Continue raycasting
        }
    }
}
