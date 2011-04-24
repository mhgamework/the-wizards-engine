using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scripting;
using MHGameWork.TheWizards.Tests.OBJParser;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests
{
    /// <summary>
    /// For use above the NewModules layer
    /// </summary>
    public class TestTWGame
    {
        public MeshRenderer Renderer;
        public RAMTextureFactory TextureFactory;
        public RAMMesh BarrelMesh;
        public ClientPhysicsQuadTreeNode PhysicsTreeRoot;
        public XNAGame Game;
        public PhysicsEngine PhysicsEngine;
        public PhysicsDebugRenderer PhysicsDebugRenderer;
        public MeshPhysicsElementFactory PhysicsFactory;



        public ScriptRunner ScriptRunner;

        public TestTWGame()
        {
            Game = new XNAGame();
            Game.DrawFps = true;

            PhysicsEngine = new PhysicsEngine();
            PhysicsEngine.Initialize();
            PhysicsDebugRenderer = new PhysicsDebugRenderer(Game, PhysicsEngine.Scene);
            Game.AddXNAObject(PhysicsEngine);
            Game.AddXNAObject(PhysicsDebugRenderer);
            PhysicsTreeRoot = new ClientPhysicsQuadTreeNode(new Microsoft.Xna.Framework.BoundingBox(new Vector3(-1024, -4000, -1024),
                                                                                         new Vector3(1024, 4000, 1024)));
            QuadTree.Split(PhysicsTreeRoot, 6);

            PhysicsFactory = new MeshPhysicsElementFactory(PhysicsEngine, PhysicsTreeRoot);
            Game.AddXNAObject(PhysicsFactory);

            Renderer = Rendering.RenderingTest.InitDefaultMeshRenderer(Game);

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
