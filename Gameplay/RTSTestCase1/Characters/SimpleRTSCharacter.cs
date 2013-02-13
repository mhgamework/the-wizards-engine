using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Characters
{
    public class SimpleRTSCharacter : IRTSCharacter
    {
        public Thing Holding { get; set; }
        public Entity Used { get; set; }
        public Entity Attacked { get; set; }
        public Vector3 Position { get; set; }
    }
}
