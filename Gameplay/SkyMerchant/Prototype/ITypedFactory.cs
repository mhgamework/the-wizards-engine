using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype
{
    public interface ITypedFactory
    {
        IslandPart CreateIsland();
        BasicPhysicsPart CreatePhysics();
        Physical CreatePhysical();
        ItemPart CreateItemPart();

        RobotPlayerPart CreateRobotPlayerPart();
        RobotPlayerNormalMovementPart CreateRobotMovementPart();

        GenerationPart CreateGenerationPart(GenerationPart.IItemFactory factory);
        GenerationSourcePart CreateGenerationSourcePart();
        ProximityChaseEnemyPart CreateProximityChaseEnemyPart(EnemyBehaviourFactory behaviourFactory, EnemyBrain Brain);
        EnemyBehaviourFactory CreateEnemyBehaviourFactory();
    }
}