using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public interface ITypedFactory
    {
        IslandPart CreateIsland();
        BasicPhysicsPart CreatePhysics();
        IPositionComponent CreatePhysicalPart();
        ItemPart CreateItemPart();

        RobotPlayerPart CreateRobotPlayerPart();
        RobotPlayerNormalMovementPart CreateRobotMovementPart();

        GenerationPart CreateGenerationPart(GenerationPart.IItemFactory factory);
        GenerationSourcePart CreateGenerationSourcePart();
        ProximityChaseEnemyPart CreateProximityChaseEnemyPart(EnemyBehaviourFactory behaviourFactory, EnemyBrain brain);
        EnemyBehaviourFactory CreateEnemyBehaviourFactory();
        PiratePart CreatePiratePart(EnemyBehaviourFactory behaviourFactory, EnemyBrain brain);

        TraderPart CreateTraderPart();
        TraderVisualizerPart CreateTraderVisualizerPart();
    }
}