using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Core.Networking;
using MHGameWork.TheWizards.Tests.Features.Rendering;
using MHGameWork.TheWizards.Tests.Features.Rendering.XNA;
using MHGameWork.TheWizards.Tests.Rendering;
using MHGameWork.TheWizards.World.Static;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Various.World.Static
{
    [TestFixture]
    public class StaticWorldSyncerTest
    {
        [Test]
        public void TestSyncOffline()
        {
            var serverpm = new SimpleServerPacketManager();
            var clientpm = serverpm.CreateClient();

            var vertexDeclarationPool = new VertexDeclarationPool();
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);

            var texturePool = new TexturePool();
            var meshPartPool = new MeshPartPool();

            var renderer = new SimpleMeshRenderer(texturePool, meshPartPool, vertexDeclarationPool);

            var mesh =
                RenderingTest.CreateGuildHouseMesh(
                    new TheWizards.OBJParser.OBJToRAMMeshConverter(new RAMTextureFactory()));

            var meshFactory = new SimpleMeshFactory();
            meshFactory.AddMesh(mesh);

            var builder = new SimpleStaticWorldObjectFactory(renderer, meshFactory);

            var client = new ClientStaticWorldObjectSyncer(clientpm, builder);
            var server = new ServerStaticWorldObjectSyncer(serverpm);

            var game = new XNAGame();



            game.AddXNAObject(renderer);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshPartPool);


            game.InitializeEvent += delegate
            {

            };

            game.UpdateEvent += delegate
                                    {
                                        if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F))
                                        {
                                            var o = server.CreateNew();
                                            var obj = o;
                                            obj.Mesh = mesh;
                                            obj.WorldMatrix = Matrix.CreateTranslation(game.SpectaterCamera.CameraPosition);
                                        }

                                        server.Update(game.Elapsed);

                                        client.Update(game.Elapsed);


                                    };

            game.Run();

        }
    }
}
