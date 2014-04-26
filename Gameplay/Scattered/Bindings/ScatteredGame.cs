using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.GameLogic;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    public class ScatteredGame
    {
        private PlayerMovementSimulator PlayerMovementSimulator;
        private PlayerInteractionSimulator PlayerInteractionSimulator;
        private GameSimulationService gameSimulationService;
        private ClusterPhysicsSimulator ClusterPhysicsSimulator;
        private PlayerCameraSimulator PlayerCameraSimulator;

        private ScatteredRenderingSimulator ScatteredRenderingSimulator;
        private WorldRenderingSimulator WorldRenderingSimulator;

        private WorldGenerationService gen;

        public ScatteredGame(PlayerMovementSimulator playerMovementSimulator,
                             PlayerInteractionSimulator playerInteractionSimulator, 
                             ClusterPhysicsSimulator clusterPhysicsSimulator, 
                             PlayerCameraSimulator playerCameraSimulator, 
                             ScatteredRenderingSimulator scatteredRenderingSimulator, 
                             WorldRenderingSimulator worldRenderingSimulator, 
                             WorldGenerationService gen, 
                             GameSimulationService gameSimulationService)
        {
            PlayerMovementSimulator = playerMovementSimulator;
            PlayerInteractionSimulator = playerInteractionSimulator;
            ClusterPhysicsSimulator = clusterPhysicsSimulator;
            PlayerCameraSimulator = playerCameraSimulator;
            ScatteredRenderingSimulator = scatteredRenderingSimulator;
            WorldRenderingSimulator = worldRenderingSimulator;
            this.gen = gen;
            this.gameSimulationService = gameSimulationService;
        }

        public void LoadIntoEngine(TWEngine engine)
        {
            engine.AddSimulator(PlayerMovementSimulator);
            engine.AddSimulator(PlayerInteractionSimulator);
            engine.AddSimulator(gameSimulationService);
            engine.AddSimulator(ClusterPhysicsSimulator);
            engine.AddSimulator(PlayerCameraSimulator);

            engine.AddSimulator(ScatteredRenderingSimulator);
            engine.AddSimulator(WorldRenderingSimulator);
            //engine.AddSimulator(new AudioSimulator());

            gen.Generate();

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }
    }
}