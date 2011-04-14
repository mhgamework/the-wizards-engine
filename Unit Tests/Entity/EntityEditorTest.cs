using System.Collections.Generic;
using MHGameWork.TheWizards.Collada.COLLADA140;
using MHGameWork.TheWizards.Collada.Files;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Editor.Scene;
using MHGameWork.TheWizards.Entities.Editor;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Tests.Editor.Scene;
using MHGameWork.TheWizards.WorldDatabase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Entity
{
    [TestFixture]
    public class EntityEditorTest
    {
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestEntityEditor()
        {


            WizardsEditor editor = new WizardsEditor();


            EntityWizardsEditorExtension extension = new EntityWizardsEditorExtension(editor);


            editor.RunEditor();


        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestObjectFullDataFactory()
        {
            string dataDir = System.Windows.Forms.Application.StartupPath + "\\Test\\TestObjectFullDataFactory";

            TheWizards.WorldDatabase.WorldDatabase db = new MHGameWork.TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.RegisterDataElementType(typeof(ObjectFullData), "TestObjectFullData");
            db.AddDataElementFactory(new ObjectFullDataFactory(db), true);


            ObjectFullData data = new ObjectFullData();
            data.Name = "MyBuilding1";
            data.BoundingBox = new BoundingBox(Vector3.Forward, Vector3.One);
            data.BoundingSphere = new BoundingSphere(Vector3.One, 5);

            DataItem item = db.WorkingCopy.CreateNewDataItem(db.FindOrCreateDataItemType("TestObject"));
            db.WorkingCopy.PutDataElement(item, data);

            db.SaveWorkingCopy();


            db = new MHGameWork.TheWizards.WorldDatabase.WorldDatabase(dataDir);

            db.RegisterDataElementType(typeof(ObjectFullData), "TestObjectFullData");
            db.AddDataElementFactory(new ObjectFullDataFactory(db), true);



            Assert.AreEqual(data, db.LoadDataElement<ObjectFullData>(db.WorkingCopy.Revision.DataItems[db.WorkingCopy.Revision.DataItems.Count - 1]));


        }
        /// <summary>
        /// Creates a new Object using the ObjectEditor and save it using the WizardsEditor
        /// TODO: this test is not really okay, since the old design does not really allow testing.
        /// </summary>
        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestCreateObjectAndSaveWorkingCopy()
        {

            WizardsEditor wizardsEditor = new WizardsEditor();

            EntityWizardsEditorExtension extension = new EntityWizardsEditorExtension(wizardsEditor);
            EditorObject obj = new EditorObject();
            obj.FullData = new ObjectFullData();
            extension.EditorObjects.Add(obj);

            ObjectEditor objectEditor = new ObjectEditor(obj, wizardsEditor);
            objectEditor.AddColladaModel(ColladaModel.LoadWall001());

            wizardsEditor.SaveToWorkingCopy();


        }

        [Test]
        public void TestColladaImportMeshParts()
        {
            TheWizards.WorldDatabase.WorldDatabase db =
                new MHGameWork.TheWizards.WorldDatabase.WorldDatabase(System.Windows.Forms.Application.StartupPath +
                                                                      "\\Test\\Entity\\Editor\\ImportMeshParts");
            ColladaMeshImporter importer = new ColladaMeshImporter(db);

            COLLADA collada;
            List<EditorMeshPart> meshParts;

            collada = COLLADA.FromStream(ColladaFiles.GetSimplePlaneDAE());
            meshParts = importer.ImportMeshParts(collada);

            collada = COLLADA.FromStream(ColladaFiles.GetAdvancedScene001DAE());
            meshParts = importer.ImportMeshParts(collada);

            collada = COLLADA.FromStream(ColladaFiles.GetTeapot001DAE());
            meshParts = importer.ImportMeshParts(collada);
        }

        [Test]
        public void TestColladaImportMesh()
        {
            TheWizards.WorldDatabase.WorldDatabase db =
                new MHGameWork.TheWizards.WorldDatabase.WorldDatabase(System.Windows.Forms.Application.StartupPath +
                                                                      "\\Test\\Entity\\Editor\\ImportMeshParts");
            ColladaMeshImporter importer = new ColladaMeshImporter(db);

            COLLADA collada;
            EditorMesh mesh;



            collada = COLLADA.FromStream(ColladaFiles.GetSimplePlaneDAE());
            mesh = importer.ImportMesh(collada);

            collada = COLLADA.FromStream(ColladaFiles.GetAdvancedScene001DAE());
            mesh = importer.ImportMesh(collada);

            collada = COLLADA.FromStream(ColladaFiles.GetTeapot001DAE());
            mesh = importer.ImportMesh(collada);
        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestEditorMeshPartRenderDataSimple()
        {
            EditorMeshPart eMeshPart = new EditorMeshPart(null);

            eMeshPart.GeometryData = MeshPartGeometryData.CreateTestSquare();

            XNAGame game = new XNAGame();
            ColladaShader shader = null;
            EditorMeshPartRenderData renderData = null;


            game.InitializeEvent +=
                delegate
                {
                    renderData = new EditorMeshPartRenderData(eMeshPart);
                    renderData.Initialize(game);

                    shader = new ColladaShader(game, null);

                    shader.DiffuseColor = Color.Red.ToVector4();
                    shader.SpecularColor = Color.Green.ToVector4();
                    shader.LightDirection = new Vector3(0, 0, -1);
                    shader.Shininess = 1;
                };

            game.DrawEvent +=
                delegate
                {
                    game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
                    shader.World = Matrix.Identity;
                    shader.ViewInverse = game.Camera.ViewInverse;
                    shader.ViewProjection = game.Camera.ViewProjection;
                    shader.RenderPrimitiveSinglePass(renderData, Microsoft.Xna.Framework.Graphics.SaveStateMode.None);
                };

            game.Run();

        }

        

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestEditorMeshPartRenderDataAdvanced()
        {
            List<EditorMeshPart> parts = new List<EditorMeshPart>();

            TheWizards.WorldDatabase.WorldDatabase db = new MHGameWork.TheWizards.WorldDatabase.WorldDatabase(
                System.Windows.Forms.Application.StartupPath + "\\Test\\EntityEditor\\TestEMPRDAdvanced");
            ColladaMeshImporter importer = new ColladaMeshImporter(db);


            COLLADA collada;
            List<EditorMeshPart> meshParts;

            collada = COLLADA.FromStream(ColladaFiles.GetSimplePlaneDAE());
            parts.AddRange(importer.ImportMeshParts(collada));

            collada = COLLADA.FromStream(ColladaFiles.GetAdvancedScene001DAE());
            parts.AddRange(importer.ImportMeshParts(collada));

            collada = COLLADA.FromStream(ColladaFiles.GetTeapot001DAE());
            parts.AddRange(importer.ImportMeshParts(collada));


            XNAGame game = new XNAGame();
            ColladaShader shader = null;
            List<EditorMeshPartRenderData> renderDatas = new List<EditorMeshPartRenderData>();


            game.InitializeEvent +=
                delegate
                {

                    for (int i = 0; i < parts.Count; i++)
                    {
                        EditorMeshPartRenderData renderData = new EditorMeshPartRenderData(parts[i]);
                        renderData.Initialize(game);
                        renderDatas.Add(renderData);
                    }


                    shader = new ColladaShader(game, null);

                    shader.DiffuseColor = Color.Red.ToVector4();
                    shader.SpecularColor = Color.White.ToVector4();
                    shader.LightDirection = Vector3.Normalize(new Vector3(0, 0, 1));
                    shader.Shininess = 20;
                };

            game.DrawEvent +=
                delegate
                {
                    game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;

                    shader.ViewProjection = game.Camera.ViewProjection;
                    shader.ViewInverse = game.Camera.ViewInverse;

                    for (int i = 0; i < renderDatas.Count; i++)
                    {
                        shader.World = Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateTranslation(Vector3.UnitX * (-100 + 40 * i));
                        shader.RenderPrimitiveSinglePass(renderDatas[i], Microsoft.Xna.Framework.Graphics.SaveStateMode.None);

                    }

                };

            game.Run();

        }

        [Test]
        [RequiresThread(System.Threading.ApartmentState.STA)]
        public void TestPutObjectsTool()
        {
            SceneEditor editor = new SceneEditor(new EditorScene());

            PutObjectsTool tool = new PutObjectsTool(editor);

            EditorObject eObj = new EditorObject();

            eObj.AddMeshFromCollada(COLLADA.FromStream(ColladaFiles.GetPyramid001DAE()));
            eObj.Name = "Wall001";
            //eObj.Save();

            tool.SelectedPutObject = eObj;

            editor.ActivateTool(tool);

            SceneEditorTest.RunSceneEditorTestEnvironment(editor);

        }
    }
}
