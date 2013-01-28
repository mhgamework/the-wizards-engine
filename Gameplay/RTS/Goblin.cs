using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTS.Commands;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [ModelObjectChanged]
    public class Goblin : EngineModelObject
    {
        public Vector3 Position { get; set; }
        public Vector3 Goal { get; set; }
        public Goblin BestFriend { get; set; }
        public Thing Holding { get; set; }
        public Vector3 LookDirection { get; set; }


        public bool IsMoving
        {
            get { return (Goal - Position).LengthSquared() > 0.00001f; }
            set { }
        }
        public void MoveTo(Vector3 position)
        {
            Goal = position;
        }

        public void DropHolding()
        {
            var pos = Position;
            pos.Y = 0.5f;
            new DroppedThing() { Thing = Holding, InitialPosition = CalculateHoldingResourcePosition() };

            Holding = null;
        }

        public bool IsHoldingResource(ResourceType type)
        {
            if (Holding == null) return false;
            return Holding.Type == type;
        }



        public Engine.WorldRendering.Entity GoblinEntity { get; set; }
        public Engine.WorldRendering.Entity HoldingEntity { get; set; }

        public IGoblinCommand CurrentCommand { get; set; }

        public Matrix calcGoblinMatrix()
        {
            var quat = Functions.CreateFromLookDir(-Vector3.Normalize(LookDirection).xna());

            return Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(quat).dx() * /*Matrix.Scaling(0.01f, 0.01f, 0.01f) **/
                   Matrix.Translation(Position);
        }

        public Matrix CalculateHoldingMatrix()
        {
            return Matrix.Translation(Vector3.UnitZ * 0.5f + Vector3.UnitY * 0.4f) * calcGoblinMatrix();
        }
        public Vector3 CalculateHoldingResourcePosition()
        {
            return CalculateHoldingMatrix().xna().Translation.dx();
        }
    }
}
