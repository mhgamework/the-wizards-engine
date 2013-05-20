using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTSTestCase1.Characters;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;
using StillDesign.PhysX;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    [ModelObjectChanged]
    public class Goblin : EngineModelObject, IRTSCharacter, IPhysical, IItemStorage
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
            ItemStorage = new ItemStoragePart();
            ItemStorage.Parent = this;
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

            //ItemStorage.ContainerArea = new BoundingBox(new Vector3(-0.3f, 0.6f, 0.3f), new Vector3(0.3f, 1.1f, 0.3f));
            ItemStorage.ContainerArea = new BoundingBox(new Vector3(0f, 1f, 0f), new Vector3(2f, 2f, 2f));
        }


        // Showing of commands

        public GoblinCommandsPart Commands { get; set; }



        public Cart Cart { get; set; }
        public ItemStoragePart ItemStorage { get; set; }


        public void UpdateBehaviour()
        {
            var g = this;
            if (g.Commands.ShowingCommands) return;
            if (g.Commands.Orbs.Count == 0) return;

            var fact = TW.Data.Get<CommandFactory>();

            if (g.hasCommand(fact.Follow))
            {
                var f = new GoblinFollowBehaviour();

                f.Update(g);
                return;
            }

            if (g.Commands. GetOrb(fact.MoveSource)!= null && g.ItemStorage.Items.Count == 0)
            {
                var orb = g.Commands.GetOrb(fact.MoveSource);
                if (orb.CurrentHolder is IItemStorage)
                {
                    var storage = (IItemStorage) orb.CurrentHolder;
                    var f = new GoblinMoveSourceBehaviour(storage);

                    f.Update(g);
                    return;
                }
                
            }
            if (g.Commands.GetOrb(fact.MoveTarget) != null && g.ItemStorage.Items.Count > 0)
            {
                var orb = g.Commands.GetOrb(fact.MoveTarget);
                if (orb.CurrentHolder is IItemStorage)
                {
                    var storage = (IItemStorage)orb.CurrentHolder;
                    var f = new GoblinMoveTargetBehaviour(storage);

                    f.Update(g);
                    return;
                }

            }

           


        }

       

        private bool hasCommand(GoblinCommandType type)
        {
            return Commands.Orbs.Count(f => f.Type == type) > 0;
        }

        public void UpdateMovement(Goblin g)
        {
            var toGoal = -(g.Physical.WorldMatrix.xna().Translation.dx() - g.Goal);

            if (toGoal.Length() < 0.01) return;
            toGoal.Normalize();
            toGoal = toGoal * 2;
            g.Physical.WorldMatrix = g.Physical.WorldMatrix*Matrix.Translation(toGoal*TW.Graphics.Elapsed);

            if (g.Cart != null)
            {
                g.Cart.Physical.WorldMatrix = g.Physical.WorldMatrix*Matrix.Translation(0, 0, 1.7f);
            }
        }
    }
}
