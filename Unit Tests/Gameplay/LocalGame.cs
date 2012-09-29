using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    /// <summary>
    /// This is a base class that simulates TW game in offline mode, by running the server alongside in the same thread.
    /// </summary>
    public class LocalGame
    {
        private List<ISimulator> simulators;
        private GraphicsWrapper game;
        private PhysicsWrapper physX;


        public LocalGame()
        {
            simulators = new List<ISimulator>();

            game = new GraphicsWrapper();

            game.InitDirectX();

            var container = new DataWrapper();

            physX = new PhysicsWrapper();
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

        private void setTWGlobals(DataWrapper container)
        {
            var context = new TW.Context();
            context.Graphics = game;
            context.Data = container;
            context.Physics = physX;
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
