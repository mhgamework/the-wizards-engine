using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Scripting
{
    /// <summary>
    /// This class is the facade between the scripts and the rest of the wizards.
    /// This class is implemented using an interchangable singleton. 
    /// This way, scripts are executed in a scope, where the scope is defined by what instance is set on this static class
    /// 
    /// This is not the final implementation, just for design testing
    /// 
    /// TODO: To Be Renamed to ScriptAPI
    /// </summary>
    public static class ScriptLayer
    {
        //Scope settings (these should be write only)
        public static IXNAGame Game;
        public static StillDesign.PhysX.Scene Scene;
        public static Physics.PhysicsEngine Physics;
        public static ScriptRunner ScriptRunner;

        //Facade
        //Contents of all functions should be strategies!! so the function shouldnt contain any logic. For simplicity they now do but this has to be cleaned up later
        public static float Elapsed { get { return Game.Elapsed; } }

        public static LineManager3D LineManager { get { return Game.LineManager3D; } }

        public static SphereMesh CreateSphereModel()
        {
            var m = new SphereMesh();
            m.Initialize(Game);
            Game.AddXNAObject(m);

            return m;

        }

        public static Actor CreateSphereActor(float radius, float mass)
        {
            var desc = new SphereShapeDescription(radius);
            var bodyDesc = new BodyDescription(mass);
            var actorDesc = new ActorDescription(desc);
            actorDesc.BodyDescription = bodyDesc;

            return Scene.CreateActor(actorDesc);
        }


        /// <summary>
        /// This releases all references to scoped objects
        /// </summary>
        public static void ClearScope()
        {
            Game = null;
            Scene = null;
            Physics = null;
            ScriptRunner = null;
            
        }

    }

}
