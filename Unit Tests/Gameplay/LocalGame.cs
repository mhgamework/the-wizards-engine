using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    /// <summary>
    /// This is a base class that simulates TW game in offline mode, by running the server alongside in the same thread.
    /// </summary>
    public class LocalGame
    {
        private List<ISimulator> simulators;
        private DX11Game game;
        private PhysicsEngine physX;


        public LocalGame()
        {
            simulators = new List<ISimulator>();

            game = new DX11Game();

            game.InitDirectX();

            var container = new ModelContainer.ModelContainer();

            physX = new PhysicsEngine();
            physX.Initialize();
            //PhysicsDebugRenderer debugRenderer =
            //    new PhysicsDebugRenderer(game, engine.Scene);

            //game.AddXNAObject(debugRenderer);

            //var boxDesc = new BoxShapeDescription();
            //boxDesc.Dimensions = new Vector3(100, 1, 100);
            //boxDesc.LocalPosition = Vector3.Up * -1;
            //var actorDesc = new ActorDescription(boxDesc);

            //engine.Scene.CreateActor(actorDesc);



            game.GameLoopEvent += delegate
            {
                foreach (var sim in simulators)
                {
                    sim.Simulate();
                }

                container.ClearDirty();
                physX.Update(game.Elapsed);

            };


            setTWGlobals(container);



        }

        private void setTWGlobals(ModelContainer.ModelContainer container)
        {
            var context = new TW.Context();
            context.Game = game;
            context.Model = container;
            context.PhysX = physX;
            context.Scene = physX.Scene;
            TW.SetContext(context);
        }

        public LocalGame AddSimulator(ISimulator sim)
        {
            simulators.Add(sim);
            return this;
        }

        public void Run()
        {
            game.Run();
        }
        public void Exit()
        {
            game.Exit();    
        }
    }
}
