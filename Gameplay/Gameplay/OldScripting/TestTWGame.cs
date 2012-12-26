using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            Renderer = InitDefaultMeshRenderer(Game);

            TextureFactory = new RAMTextureFactory();
            var converter = new OBJToRAMMeshConverter(TextureFactory);
            BarrelMesh = GetBarrelMesh(converter);

            ScriptRunner = new ScriptRunner(Game);
        }

        public static string BarrelObj { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.obj"; } }
        public static string BarrelMtl { get { return TWDir.GameData.CreateSubdirectory("Core") + @"\Barrel01.mtl"; } }

        public static RAMMesh GetBarrelMesh(OBJToRAMMeshConverter c)
        {
            var fsMat = new FileStream(BarrelMtl, FileMode.Open);

            var importer = new ObjImporter();
            importer.AddMaterialFileStream("Barrel01.mtl", fsMat);

            importer.ImportObjFile(BarrelObj);

            var meshes = c.CreateMeshesFromObjects(importer);

            fsMat.Close();

            return meshes[0];
        }
        public static SimpleMeshRenderer InitDefaultMeshRenderer(XNAGame game)
        {
            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);
            return renderer;
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
