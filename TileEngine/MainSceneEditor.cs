﻿using System;
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

        private SimpleTileFaceTypeFactory tileFaceFactory = new SimpleTileFaceTypeFactory();
        private TileDataFactory tileDataFactory;
        private SimpleTileFaceTypeFactory tileFaceTypeFactory = new SimpleTileFaceTypeFactory();
        private SimpleWorldObjectTypeFactory worldObjectTypeFactory = new SimpleWorldObjectTypeFactory();

        public List<WorldObjectType> typeList = new List<WorldObjectType>();
        private int scrollIndex;
        private DiskRenderingAssetFactory renderingFactory;


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

            renderingFactory = new DiskRenderingAssetFactory();


            placeTool = new WorldObjectPlaceTool(game, world, renderer, builder, renderingFactory, tileFaceFactory);
            moveTool = new WorldObjectMoveTool(game, world, builder, renderer);
            snapLearnTool = new SnapLearnTool(world, renderer, builder);

            game.AddXNAObject(placeTool);
            game.AddXNAObject(moveTool);
            game.AddXNAObject(snapLearnTool);

            tileDataFactory = new TileDataFactory(renderingFactory, tileFaceTypeFactory);
            worldSerializer = new WorldSerializer(renderingFactory, tileDataFactory, game, renderer, worldObjectTypeFactory,
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
            FileStream stream = File.Open(fileInfo.FullName, FileMode.Create);
            worldSerializer.SerializeWorld(world, stream);
            stream.Close();
        }

        private void writeTileData(FileInfo fileInfo)
        {

            List<TileData> tileDataList = new List<TileData>();
            for (int i = 0; i < typeList.Count; i++)
            {
                tileDataList.Add(typeList[i].TileData);
            }

            for (int i = 0; i < tileDataList.Count; i++)
            {
                TileData cData = tileDataList[i];

                FileStream stream = File.Open(getTileDataFilePath(fileInfo, cData), FileMode.Create);
                tileDataFactory.SerializeTileData(cData, stream);
                stream.Close();
            }
        }

        private string getTileDataFilePath(FileInfo worldFi, TileData tileData)
        {
            return getDirectoryForWorldFile(worldFi).FullName + "\\TileData " + tileData.Guid.ToString() + ".xml";
        }

        private void writeMeshes(FileInfo fileInfo)
        {

            renderingFactory.SaveDir = getDirectoryForWorldFile(fileInfo);
            renderingFactory.SaveAllAssets();

        }

        private DirectoryInfo getDirectoryForWorldFile(FileInfo worldFile)
        {
            var namePart = worldFile.Name.Substring(0, worldFile.Name.Length - worldFile.Extension.Length);

            return worldFile.Directory.CreateSubdirectory(namePart);
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

            loadMeshes(fileInfo);
            loadTileData(fileInfo);
            loadWorld(fileInfo);


            moveTool.World = world;
            placeTool.World = world;
            snapLearnTool.World = world;






        }

        private void loadMeshes(FileInfo worldFile)
        {
            renderingFactory.SaveDir = getDirectoryForWorldFile(worldFile);
        }
        private void loadTileData(FileInfo worldFile)
        {
            var di = getDirectoryForWorldFile(worldFile);
            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!fi.Name.StartsWith("TileData")) continue;
                FileStream readStream = File.OpenRead(fi.FullName);
                var readData = tileDataFactory.DeserializeTileData(readStream);
                tileDataFactory.AddTileData(readData);
                readStream.Close();
            }

        }
        private void loadWorld(FileInfo worldFile)
        {
            FileStream readStream = File.OpenRead(worldFile.FullName);
            var readWorld = new World();

            worldSerializer.DeserializeWorld(readWorld, readStream);
            readStream.Close();

            typeList.Clear();
            foreach (var obj in readWorld.WorldObjectList)
            {
                if (!typeList.Contains(obj.ObjectType))
                    typeList.Add(obj.ObjectType);
            }
            world = readWorld;
        }

        private void createNewWorldObjectType(TileSnapInformationBuilder builder)
        {
            RAMMesh mesh = getMeshFromUser();
            if (mesh == null) return;
            var worldObjectType = new WorldObjectType(mesh, Guid.NewGuid(), builder);
            var tileData = new TileData(Guid.NewGuid());
            tileData.Mesh = mesh;

            tileData.Dimensions = new Vector3(1.5f, 2, 1.5f) - new Vector3(-1.5f, -2, -1.5f);
            worldObjectType.TileData = tileData;

            addType(worldObjectType);
        }

        private void addType(WorldObjectType worldObjectType)
        {
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
            var c = new OBJToRAMMeshConverter(renderingFactory);

            importer.AddMaterialFileStream(namePart + ".mtl", new FileStream(fileInfo.Directory.FullName + "/" + namePart + ".mtl", FileMode.Open));
            importer.ImportObjFile(path);
            var mesh = c.CreateMesh(importer);
            renderingFactory.AddAsset(mesh);
            return mesh;
        }

    }
}
