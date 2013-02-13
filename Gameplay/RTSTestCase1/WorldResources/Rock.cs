using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    [ModelObjectChanged]
    public class Rock : EngineModelObject, IWorldResource
    {
        public Rock()
        {

        }

        public float Height { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 GenerationPoint { get { return Position + Vector3.UnitY * 0.3f; } }
        public Vector3 OutputDirection
        {
            get
            {
                return new Vector3(1, 0, 0);
            }
        }
        public Thing TakeResource()
        {
            if (!ResourceAvailable) throw new InvalidOperationException();
            Height -= 1;
            return new Thing() { Type = TW.Data.Get<ResourceFactory>().Stone };
        }

        public bool ResourceAvailable { get { return Height > 1; } }
        public void Grow(float elapsed)
        {
            Height += elapsed;
        }
    }
}
