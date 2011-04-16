using System;
using MHGameWork.TheWizards.Collada.COLLADA140;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Rendering;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Entity
{
    [TestFixture]
    public class EntityTest
    {

        /*[Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        [Obsolete()]
        public void TestMeshRenderDataFactorySimple()
        {
            IMesh mesh = RenderingTest.CreateSimpleTestMesh();

            DefaultRenderer renderer = new DefaultRenderer();

            MeshRenderDataFactory factory = new MeshRenderDataFactory(renderer);

            MeshRenderElement meshElement = factory.CreateRenderElement(mesh);

            meshElement.WorldMatrix = Matrix.Identity;

            renderer.RunTest();

        }*/

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestObjectRenderDataFactory()
        {
            IObject obj;

            obj = new RAMObject();

            DefaultRenderer renderer = new DefaultRenderer();


            MeshRenderDataFactory meshFactory = new MeshRenderDataFactory(renderer);
            ObjectRenderDataFactory factory = new ObjectRenderDataFactory();

            factory.AddObjectDataFactory(meshFactory);

            ObjectRenderElement el = factory.CreateRenderElement(obj);

            el.WorldMatrix = Matrix.Identity;


            renderer.RunTest();

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestEntityRenderDataFactory()
        {
            /*IEntity ent;

            ent = new RAMEntity();

            DefaultRenderer renderer = new DefaultRenderer();

            MeshRenderDataFactory meshFactory = new MeshRenderDataFactory(renderer);
            ObjectRenderDataFactory objectFactory = new ObjectRenderDataFactory();
            objectFactory.AddObjectDataFactory(meshFactory);

            EntityRenderDataFactory factory = new EntityRenderDataFactory(objectFactory);


            EntityRenderElement el = factory.CreateRenderElement(ent);


            renderer.RunTest();*/


        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestRenderScene()
        {
          /*  IScene scene;
            scene = createTestScene();

            DefaultRenderer renderer = new DefaultRenderer();

            MeshRenderDataFactory meshFactory = new MeshRenderDataFactory(renderer);
            ObjectRenderDataFactory objectFactory = new ObjectRenderDataFactory();
            objectFactory.AddObjectDataFactory(meshFactory);

            EntityRenderDataFactory factory = new EntityRenderDataFactory(objectFactory);


            SceneEntityData entityData = scene.GetEntityData();

            for (int i = 0; i < entityData.Entities.Count; i++)
            {
                factory.CreateRenderElement(entityData.Entities[i]);
            }


            renderer.RunTest();*/
        }

       /* private IScene createTestScene()
        {
            throw new NotImplementedException();
        }*/

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestSerializeScene()
        {
          /*  IScene scene = createTestScene();

            SceneXMLSerializer serializer = new SceneXMLSerializer();
            serializer.Serialize(scene, "Tests\\Entity\\TestScene.xml");


            IScene loadedEntity = serializer.Deserialize("Tests\\Entity\\TestScene.xml");
            */
        }

        
    }
}
