using System;
using DirectX11;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public class PrototypeObjectsFactory
    {
        private readonly IGameObjectsRepository repository;
        private readonly ITypedFactory factory;

        public ItemType CogType = new ItemType() { Name = "Cog" };
        public ItemType WoodType = new ItemType() { Name = "Wood" };
        public ItemType TubeType = new ItemType() { Name = "Tube" };

        public PrototypeObjectsFactory(IGameObjectsRepository repository)
        {
            this.repository = repository;
        }

        public IslandPart CreateIsland()
        {
            var obj = repository.CreateGameObject();
            return obj.GetComponent<IslandPart>();

        }
        public ItemPart CreateCog()
        {
            //var part = factory.CreateItemPart();
            //part.Physical = factory.CreatePhysicalPart();
            //part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/Cogs/Cog01");
            //part.Type = CogType;
            //return part;
            return null;
        }
        public ItemPart CreateTube()
        {
            //var part = factory.CreateItemPart();
            //part.Physical = factory.CreatePhysicalPart();
            //part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/TubePart/TubePart");
            //part.Type = TubeType;
            //return part;
            return null;
        }

        public ItemPart CreateWoodBlock()
        {
            //var part = factory.CreateItemPart();
            //part.Physical = factory.CreatePhysicalPart();
            //part.Physical.Mesh = UtilityMeshes.CreateMeshWithText(0.4f, "Wood", TW.Graphics);
            //part.Type = WoodType;
            //return part;
            return null;
        }

        public GenerationSourcePart CreateTree()
        {

            //var part = factory.CreateGenerationSourcePart();
            //part.GenerationPart = factory.CreateGenerationPart(new TreeItemFactory(this));
            //part.Physical = factory.CreatePhysicalPart();
            //part.Physical.ObjectMatrix = Matrix.Scaling(MathHelper.One * 0.8f);

            //part.EmptyMesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_NoLeaves");
            //part.FullMesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_WithLeaves");

            //return part;
            return null;
        }

        public ProximityChaseEnemyPart CreateDrone()
        {
            //var beh = factory.CreateEnemyBehaviourFactory();
            //beh.Physical = factory.CreatePhysicalPart();
            //beh.Brain = new EnemyBrain();
            
            //var part = factory.CreateProximityChaseEnemyPart(beh,beh.Brain);
            //part.Physical = beh.Physical;
            //part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/EnemyRobot/EnemyRobot");

            //return part;
            return null;
        }

        public PiratePart CreatePirate()
        {
            //var beh = factory.CreateEnemyBehaviourFactory();
            //beh.Physical = factory.CreatePhysicalPart();
            //beh.Brain = new EnemyBrain();

            //var part = factory.CreatePiratePart(beh,beh.Brain);
            //part.Physical = beh.Physical;
            //part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/DummyRobot/DummyRobot");
            //part.Physical.ObjectMatrix = Matrix.Scaling(MathHelper.One * 0.3f);


            //return part;
            return null;
        }

        public IPositionComponent CreateMeshObject()
        {
            var p = factory.CreatePhysicalPart();
            return p;
        }

        private class TreeItemFactory : GenerationPart.IItemFactory
        {
            private PrototypeObjectsFactory factory;

            public TreeItemFactory(PrototypeObjectsFactory factory)
            {
                this.factory = factory;
            }

            public ItemPart CreateItem()
            {
                return factory.CreateWoodBlock();
            }
        }

        public TraderVisualizerPart CreateTrader()
        {
            var obj = repository.CreateGameObject();
            return obj.GetComponent<TraderVisualizerPart>();
            return null;
        }

        public ItemPart CreateItem(ItemType type)
        {
            if (type == CogType)
                return CreateCog();
            if (type == WoodType)
                return CreateWoodBlock();
            if (type == TubeType)
                return CreateTube();

            throw new NotImplementedException();
        }
    }
}