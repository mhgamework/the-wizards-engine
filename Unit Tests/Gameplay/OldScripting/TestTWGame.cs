using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Tests.Features.Data.OBJParser;
using MHGameWork.TheWizards.Tests.Features.Rendering;
using MHGameWork.TheWizards.Tests.Features.Simulation.Physics;
using MHGameWork.TheWizards._XNA.Scripting;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests
{
    /// <summary>
    /// For use above the NewModules layer
    /// </summary>
    public class TestTWGame
    {
        public SimpleMeshRenderer Renderer;
        public RAMTextureFactory TextureFactory;
        public RAMMesh BarrelMesh;
        public ClientPhysicsQuadTreeNode PhysicsTreeRoot;
        public XNAGame Game;
        public PhysicsEngine PhysicsEngine;
        public PhysicsDebugRendererXNA PhysicsDebugRendererXna;
        public MeshPhysicsElementFactory PhysicsFactory;



        public ScriptRunner ScriptRunner;

        public TestTWGame()
        {
            Game = new XNAGame();
            Game.DrawFps = true;

            PhysicsEngine = new PhysicsEngine();
            PhysicsEngine.Initialize();
            PhysicsDebugRendererXna = new PhysicsDebugRendererXNA(Game, PhysicsEngine.Scene);
            Game.AddXNAObject(PhysicsEngine);
            Game.AddXNAObject(PhysicsDebugRendererXna);
            PhysicsTreeRoot = new ClientPhysicsQuadTreeNode(new Microsoft.Xna.Framework.BoundingBox(new Vector3(-1024, -4000, -1024),
                                                                                         new Vector3(1024, 4000, 1024)));
            QuadTree.Split(PhysicsTreeRoot, 6);

            var xnafact = new MeshPhysicsFactoryXNA(PhysicsEngine, PhysicsTreeRoot);
            PhysicsFactory = xnafact.Factory;
            Game.AddXNAObject(xnafact);

            Renderer = RenderingTest.InitDefaultMeshRenderer(Game);

            TextureFactory = new RAMTextureFactory();
            var converter = new OBJToRAMMeshConverter(TextureFactory);
            BarrelMesh = OBJParserTest.GetBarrelMesh(converter);

            ScriptRunner = new ScriptRunner(Game);
        }


        public void SetScriptLayerScope()
        {
            ScriptLayer.Game = Game;
            ScriptLayer.Physics = PhysicsEngine;
            ScriptLayer.Scene = PhysicsEngine.Scene;
            ScriptLayer.ScriptRunner = ScriptRunner;
        }
    }
}
