using DirectX11;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class ObjectsFactory
    {
        private readonly ITypedFactory factory;

        public ObjectsFactory(ITypedFactory factory)
        {
            this.factory = factory;
        }

        public IslandPart CreateIsland()
        {
            var ret = factory.CreateIsland();
            ret.Physical = factory.CreatePhysical();
            ret.Physics = factory.CreatePhysics();
            return ret;

        }
        public ItemPart CreateCog()
        {
            var part = factory.CreateItemPart();
            part.Physical = factory.CreatePhysical();
            part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/Cogs/Cog01");
            return part;
        }
        public ItemPart CreateTube()
        {
            var part = factory.CreateItemPart();
            part.Physical = factory.CreatePhysical();
            part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/TubePart/TubePart");
            return part;
        }

        public ItemPart CreateWoodBlock()
        {
            var part = factory.CreateItemPart();
            part.Physical = factory.CreatePhysical();
            part.Physical.Mesh = UtilityMeshes.CreateMeshWithText(0.4f, "Wood", TW.Graphics);
            return part;
        }

        public GenerationSourcePart CreateTree()
        {
            var part = factory.CreateGenerationSourcePart();
            part.GenerationPart = factory.CreateGenerationPart(new TreeItemFactory(this));
            part.Physical = factory.CreatePhysical();
            part.Physical.ObjectMatrix = Matrix.Scaling(MathHelper.One * 0.8f);

            part.EmptyMesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_NoLeaves");
            part.FullMesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_WithLeaves");

            return part;
        }

        public ProximityChaseEnemyPart CreateDrone()
        {
            var beh = factory.CreateEnemyBehaviourFactory();
            beh.Physical = factory.CreatePhysical();
            beh.Brain = new EnemyBrain();
            
            var part = factory.CreateProximityChaseEnemyPart(beh,beh.Brain);
            part.Physical = beh.Physical;
            part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/EnemyRobot/EnemyRobot");

            return part;
        }

        private class TreeItemFactory : GenerationPart.IItemFactory
        {
            private ObjectsFactory factory;

            public TreeItemFactory(ObjectsFactory factory)
            {
                this.factory = factory;
            }

            public ItemPart CreateItem()
            {
                return factory.CreateWoodBlock();
            }
        }
    }
}