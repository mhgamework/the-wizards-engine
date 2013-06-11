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
    public class Rock : EngineModelObject,IPhysical, IWorldResource
    {
        public Rock()
        {
            Physical = new Physical();
        }

        public float Height { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 GenerationPoint { get { return Position + Vector3.UnitY * 0.3f + Vector3.UnitX*3f; } }
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
            Height += elapsed * 0.05f;
            if (Height > 6) Height = 6;
        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var entity = Physical;

            entity.Mesh = TW.Assets.LoadMesh("RTS\\Rock");
            entity.Solid = true;
            entity.Static = true;
            var size = 5;

            float height = -size;
            height += 1f;
            height += Height * 0.1f;

            entity.WorldMatrix = Matrix.Scaling(size, size, size) * Matrix.Translation(Position + height * Vector3.UnitY);
        }
    }
}
