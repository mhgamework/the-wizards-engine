using System;
using DirectX11;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
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

        public ItemType CogType;
        public ItemType WoodType;
        public ItemType TubeType;

        public PrototypeObjectsFactory(IGameObjectsRepository repository)
        {
            this.repository = repository;

            TubeType = new ItemType() { Name = "Tube", Mesh = TW.Assets.LoadMesh("SkyMerchant/TubePart/TubePart") };
            CogType = new ItemType() { Name = "Cog", Mesh = TW.Assets.LoadMesh("SkyMerchant/Cogs/Cog01") };
            WoodType = new ItemType() { Name = "Wood", Mesh = UtilityMeshes.CreateMeshWithText(0.4f, "Wood", TW.Graphics) };
        }

        public IslandPart CreateIsland()
        {
            var obj = repository.CreateGameObject();
            return obj.GetComponent<IslandPart>();

        }
        public ItemPart CreateItem(ItemType type)
        {
            var obj = repository.CreateGameObject();
            var part = obj.GetComponent<ItemPart>();
            part.RenderComponent.Mesh = type.Mesh;
            part.Type = type;
            return part;
        }
        public ItemPart CreateCog()
        {
            return CreateItem(CogType);
        }
        public ItemPart CreateTube()
        {
            return CreateItem(TubeType);
        }

        public ItemPart CreateWoodBlock()
        {
            return CreateItem(WoodType);
        }

        public GenerationSourcePart CreateTree()
        {
            var obj = repository.CreateGameObject();
            var part = obj.GetComponent<GenerationSourcePart>();
            part.MeshRenderComponent.ObjectMatrix = Matrix.Scaling(MathHelper.One * 0.8f);

            part.EmptyMesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_NoLeaves");
            part.FullMesh = TW.Assets.LoadMesh("SkyMerchant/Tree/Tree_WithLeaves");

            part.GenerationPart.Factory = new TreeItemFactory(this);

            return part;
            
        }

        public ProximityChaseEnemyPart CreateDrone()
        {
            var obj = repository.CreateGameObject();
            var part = obj.GetComponent<ProximityChaseEnemyPart>();
            //var beh = factory.CreateEnemyBehaviourFactory();
            //beh.Physical = factory.CreatePhysicalPart();
            //beh.Brain = new EnemyBrain();

            //var part = factory.CreateProximityChaseEnemyPart(beh,beh.Brain);
            //part.Physical = beh.Physical;
            //part.Physical.Mesh = TW.Assets.LoadMesh("SkyMerchant/EnemyRobot/EnemyRobot");

            return part;
        }

        public PiratePart CreatePirate()
        {
            var obj = repository.CreateGameObject();
            var part = obj.GetComponent<PiratePart>();

            obj.GetComponent<IMeshRenderComponent>() .Mesh = TW.Assets.LoadMesh("SkyMerchant/DummyRobot/DummyRobot");
            obj.GetComponent<IMeshRenderComponent>().ObjectMatrix = Matrix.Scaling(MathHelper.One * 0.3f);

            return part;
        }

        public IPositionComponent CreateMeshObject(IMesh mesh)
        {
            var obj = repository.CreateGameObject();
            obj.GetComponent<IMeshRenderComponent>().Mesh = mesh;
            return obj.GetComponent<IPositionComponent>();
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
        }

        
    }
}