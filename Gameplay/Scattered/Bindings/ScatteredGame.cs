using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.GameLogic;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    public class ScatteredGame
    {
        private PlayerMovementSimulator PlayerMovementSimulator;
        private PlayerInteractionService playerInteractionService;
        private GameSimulationService gameSimulationService;
        private ClusterPhysicsService clusterPhysicsService;
        private PlayerCameraSimulator PlayerCameraSimulator;

        private ScatteredRenderingSimulator ScatteredRenderingSimulator;

        private WorldGenerationService gen;

        public ScatteredGame(PlayerMovementSimulator playerMovementSimulator,
                             PlayerInteractionService playerInteractionService, 
                             ClusterPhysicsService clusterPhysicsService, 
                             PlayerCameraSimulator playerCameraSimulator, 
                             ScatteredRenderingSimulator scatteredRenderingSimulator, 
                             WorldGenerationService gen, 
                             GameSimulationService gameSimulationService)
        {
            PlayerMovementSimulator = playerMovementSimulator;
            this.playerInteractionService = playerInteractionService;
            this.clusterPhysicsService = clusterPhysicsService;
            this.PlayerCameraSimulator = playerCameraSimulator;
            this.ScatteredRenderingSimulator = scatteredRenderingSimulator;
            this.gen = gen;
            this.gameSimulationService = gameSimulationService;
        }

        public void LoadIntoEngine(TWEngine engine)
        {
            engine.AddSimulator(PlayerMovementSimulator);
            engine.AddSimulator(gameSimulationService);
            engine.AddSimulator(PlayerCameraSimulator);

            engine.AddSimulator(ScatteredRenderingSimulator);
            //engine.AddSimulator(new AudioSimulator());

            TW.Graphics.SpectaterCamera.FarClip = 2000;
        }

        public void GenerateWorld()
        {
            gen.Generate();
            
        }
    }
}