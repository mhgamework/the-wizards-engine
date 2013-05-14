using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTSTestCase1.Characters;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    [ModelObjectChanged]
    public class Goblin : EngineModelObject, IRTSCharacter, IPhysical
    {
        public Entity Attacked { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Goal { get; set; }
        public Thing Holding { get; set; }
        public Entity Used { get; set; }
        public Vector3 LookDirection { get; set; }
        /// <summary>
        /// IsEvil
        /// </summary>
        public bool IsFriendly { get; set; }

        public Goblin()
        {
            IsFriendly = true;
            Physical = new Physical();
            Commands = new GoblinCommandsPart();
            Commands.Goblin = this; // TODO: do this automatically?
        }


        public bool IsMoving
        {
            get { return (Goal - Position).LengthSquared() > 0.00001f; }
            set { }
        }
        public void MoveTo(Vector3 position)
        {
            Goal = position;
        }

        public Actor GetHoldingActor()
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetHoldingPosition()
        {
            throw new System.NotImplementedException();
        }

        public void DropHolding()
        {
            var pos = Position;
            pos.Y = 0.5f;
            var drop = new DroppedThing() { Thing = Holding  };
            drop.Physical.WorldMatrix = Matrix.Translation(CalculateHoldingResourcePosition());
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


        // Physical

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            Physical.Mesh = TW.Assets.LoadMesh("Core\\Barrel01");//Load("Goblin\\GoblinLowRes");
            Physical.ObjectMatrix = Matrix.Translation(0, 0.9f / 2, 0);
            Physical.Solid = true;
            Physical.Static = false;
            Physical.Solid = false;
        }


        // Showing of commands

        public GoblinCommandsPart Commands { get; set; }



        public Cart Cart { get; set; }
    }
}
