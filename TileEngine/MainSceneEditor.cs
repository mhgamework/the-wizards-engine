using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Editor;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient.Water;
using MHGameWork.TheWizards.Tests.Rendering;
using MHGameWork.TheWizards.Tests.TileEngine;
using MHGameWork.TheWizards.XML;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class MainSceneEditor
    {
        private XNAGame game = new XNAGame();

        private World world = new World();
        private TileSnapInformationBuilder builder = new TileSnapInformationBuilder();
        private SimpleMeshRenderer renderer;
        private WorldSerializer worldSerializer;

        private WorldObjectPlaceTool placeTool;
        private WorldObjectMoveTool moveTool;
        private SnapLearnTool snapLearnTool;

        private SimpleMeshFactory meshFactory = new SimpleMeshFactory();
        private SimpleTileFaceTypeFactory tileFaceFactory = new SimpleTileFaceTypeFactory();
        private TileDataFactory tileDataFactory;
        private SimpleTileFaceTypeFactory tileFaceTypeFactory = new SimpleTileFaceTypeFactory();
        private SimpleWorldObjectTypeFactory worldObjectTypeFactory = new SimpleWorldObjectTypeFactory();

        private List<WorldObjectType> typeList = new List<WorldObjectType>();

        private int scrollIndex;


        public MainSceneEditor()
        {
            var texturePool = new TexturePool();
            var meshpartPool = new MeshPartPool();
            var vertexDeclarationPool = new VertexDeclarationPool();
            vertexDeclarationPool.SetVertexElements<TangentVertex>(TangentVertex.VertexElements);
            renderer = new SimpleMeshRenderer(texturePool, meshpartPool, vertexDeclarationPool);

            game.IsFixedTimeStep = false;
            game.DrawFps = true;
            game.AddXNAObject(texturePool);
            game.AddXNAObject(meshpartPool);
            game.AddXNAObject(vertexDeclarationPool);
            game.AddXNAObject(renderer);

            placeTool = new WorldObjectPlaceTool(game, world, renderer, builder, meshFactory, tileFaceFactory);
            moveTool = new WorldObjectMoveTool(game, world, builder, renderer);
            snapLearnTool = new SnapLearnTool(world, renderer, builder);

            game.AddXNAObject(placeTool);
            game.AddXNAObject(moveTool);
            game.AddXNAObject(snapLearnTool);

            tileDataFactory = new TileDataFactory(meshFactory, tileFaceTypeFactory);
            worldSerializer = new WorldSerializer(meshFactory, tileDataFactory, game, renderer, worldObjectTypeFactory,
                                             builder);
        }


        public void Run()
        {
            EditorGrid grid;
            grid = new EditorGrid();
            grid.Size = new Vector2(100, 100);
            grid.Interval = 1;
            grid.MajorInterval = 10;

            bool mouseEnabled = false;
            scrollIndex = 0;

            game.UpdateEvent += delegate
            {
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.M))
                {
                    placeTool.Enabled = false;
                    moveTool.Enabled = true;
                    snapLearnTool.Enabled = false;
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.L))
                {
                    placeTool.Enabled = false;
                    moveTool.Enabled = false;
                    snapLearnTool.Enabled = true;
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
                {
                    placeTool.Enabled = !placeTool.Enabled;
                    moveTool.Enabled = false;
                    snapLearnTool.Enabled = false;
                }
                if (placeTool.Enabled)
                {
                    updatePlaceType(game, placeTool);
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftAlt))
                {
                    mouseEnabled = !mouseEnabled;
                }
                if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.C))
                {
                    createNewWorldObjectType(builder);
                }
                if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.S))
                {
                    save();
                }
                if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) && game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.L))
                {
                    load();
                }
                if (mouseEnabled)
                {
                    game.Mouse.CursorEnabled = true;
                    game.IsMouseVisible = true;
                    game.SpectaterCamera.Enabled = false;
                }
                else
                {
                    game.Mouse.CursorEnabled = false;
                    game.IsMouseVisible = false;
                    game.SpectaterCamera.Enabled = true;
                }

            };

            game.DrawEvent += delegate
            {
                grid.Render(game);

            };

            game.Run();
        }

        private void updatePlaceType(XNAGame game, WorldObjectPlaceTool placeTool)
        {
            if (typeList.Count == 0) return;

            if (game.Mouse.RelativeScrollWheel > 0)
                scrollIndex++;
            if (game.Mouse.RelativeScrollWheel < 0)
                scrollIndex--;
            if (scrollIndex < 0)
                scrollIndex += typeList.Count;

            scrollIndex = scrollIndex % typeList.Count;
            placeTool.PlaceType = typeList[scrollIndex];
        }

        private void save()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML Files|*.xml";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            dialog.CheckPathExists = true;

            String path = dialog.FileName;
            FileInfo fileInfo = new FileInfo(path);

            writeWorld(fileInfo);
            writeTileData(fileInfo);
            writeMeshes(fileInfo);
        }


        private void writeWorld(FileInfo fileInfo)
        {
            var namePart = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            FileStream stream = File.OpenWrite(fileInfo.Directory.CreateSubdirectory(namePart).FullName + "\\" + namePart + ".xml");
            worldSerializer.SerializeWorld(world, stream);
            stream.Close();
        }

        private void writeTileData(FileInfo fileInfo)
        {
            var namePart = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

            List<TileData> tileDataList = new List<TileData>();
            for (int i = 0; i < typeList.Count; i++)
            {
                tileDataList.Add(typeList[i].TileData);
            }

            for (int i = 0; i < tileDataList.Count; i++)
            {
                TileData cData = tileDataList[i];

                FileStream stream = File.OpenWrite(fileInfo.Directory.CreateSubdirectory(namePart).FullName + "\\TileData " + cData.Guid.ToString() + ".xml");
                tileDataFactory.SerializeTileData(cData, stream);
                stream.Close();
            }
        }

        private void writeMeshes(FileInfo fileInfo)
        {
            var namePart = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

            List<IMesh> meshes = new List<IMesh>();
            for (int i = 0; i < typeList.Count; i++)
            {
                meshes.Add(typeList[i].Mesh);
            }

            for (int i = 0; i < meshes.Count; i++)
            {
                IMesh cMesh = meshes[i];

                FileStream stream = File.OpenWrite(fileInfo.Directory.CreateSubdirectory(namePart).FullName + "\\Mesh " + cMesh.Guid.ToString() + ".xml");
                var s = new TWXmlSerializer<MeshCoreData>();
                s.AddCustomSerializer(AssetSerializer.CreateSerializer());
                s.Serialize(cMesh.GetCoreData(), stream);
                stream.Close();
            }
        }

        private void load()
        {
            //TODO: implement deserialization of tileData and meshes
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML Files|*.xml";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            dialog.CheckPathExists = true;
            String path = dialog.FileName;
            FileInfo fileInfo = new FileInfo(path);
            String mapPath = fileInfo.DirectoryName;

            loadMeshes(mapPath);
            loadTileData(mapPath);
            loadWorld(path);
            

            
           
            //var namePart = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);



            
        }

        private void loadMeshes(String path)
        {
            throw new NotImplementedException();
        }
        private void loadTileData(String path)
        {
            FileStream readStream = File.OpenRead(path);
            TileData readData = tileDataFactory.DeserializeTileData(readStream);
            readStream.Close();
        }
        private void loadWorld(String path)
        {
            FileStream readStream = File.OpenRead(path);
            var readWorld = new World();

            worldSerializer.DeserializeWorld(readWorld, readStream);
            readStream.Close();

            world = readWorld;
        }

        private void createNewWorldObjectType(TileSnapInformationBuilder builder)
        {
            RAMMesh mesh = getMeshFromUser();

            var worldObjectType = new WorldObjectType(mesh, Guid.NewGuid(), builder);
            var tileData = new TileData(Guid.NewGuid());
            tileData.Mesh = mesh;

            tileData.Dimensions = new Vector3(1.5f, 2, 1.5f) - new Vector3(-1.5f, -2, -1.5f);
            worldObjectType.TileData = tileData;

            typeList.Add(worldObjectType);
        }

        private RAMMesh getMeshFromUser()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "OBJ Files|*.obj";

            if (dialog.ShowDialog() != DialogResult.OK) return null;

            dialog.CheckPathExists = true;

            String path = dialog.FileName;

            FileInfo fileInfo = new FileInfo(path);
            var namePart = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);

            OBJParser.ObjImporter importer = new OBJParser.ObjImporter();
            var c = new OBJToRAMMeshConverter(new RAMTextureFactory());

            importer.AddMaterialFileStream(namePart + ".mtl", new FileStream(fileInfo.Directory.FullName + "/" + namePart + ".mtl", FileMode.Open));
            importer.ImportObjFile(path);
            var mesh = c.CreateMesh(importer);

            return mesh;
        }

    }
}
