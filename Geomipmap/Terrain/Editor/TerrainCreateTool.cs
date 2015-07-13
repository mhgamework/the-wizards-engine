using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Editor;
using MHGameWork.TheWizards.ServerClient.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using IEditorTool=MHGameWork.TheWizards.ServerClient.Editor.IEditorTool;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    public class TerrainCreateTool : IEditorTool
    {
        private LineManager3DLines gridLines = new LineManager3DLines();
        private Vector3 halfTotalSize;
        private Vector3 tileSize;
        private Vector3 center;

        private WorldEditor editor;

        private bool pickedTerrain;
        private Vector3 PickedTerrainPos;

        private List<EditorTerrainRenderHeightmap> renderDatas = new List<EditorTerrainRenderHeightmap>();

        private TerrainEditorForm guiForm;

        [Obsolete("Use other constructor")]
        public TerrainCreateTool(WorldEditor _editor)
        {
            throw new InvalidOperationException("Use other constructor instead");
            editor = _editor;
        }
        public TerrainCreateTool(WorldEditor _editor, TerrainEditorForm _guiForm)
        {
            editor = _editor;
            guiForm = _guiForm;
            guiForm.btnCreate.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(btnCreate_ItemClick);
        }

        void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            editor.ActivateTool(this);
        }

        public void Deactivate()
        {
            guiForm.btnCreate.Down = false;
        }

        public void BuildTerrainCreateGrid(Vector2 _totalSize, Vector2 _tileSize, Vector2 _center)
        {
            halfTotalSize = new Vector3(_totalSize.X * 0.5f, 0, _totalSize.Y * 0.5f);
            tileSize = new Vector3(_tileSize.X, 0, _tileSize.Y);
            center = new Vector3(_center.X, 0, _center.Y);


            gridLines.ClearAllLines();



            float iX = -halfTotalSize.X;
            while (iX < halfTotalSize.X)
            {
                float iZ = -halfTotalSize.Z;

                while (iZ < halfTotalSize.Z)
                {
                    Vector3 min = new Vector3(iX, 0, iZ);
                    Vector3 max = min + tileSize;
                    gridLines.AddBox(new BoundingBox(min, max), Color.Yellow);

                    iZ += tileSize.Z;

                }

                iX += tileSize.X;


            }

            Vector3 temp = halfTotalSize;
            temp.Y -= tileSize.X * 0.01f;

            gridLines.AddBox(new BoundingBox(-temp + center, temp + center), Color.Black);


        }


        public void Activate()
        {
            guiForm.btnCreate.Down = true;

            for (int i = 0; i < renderDatas.Count; i++)
            {
                EditorTerrainRenderHeightmap data = renderDatas[i];
                data.Dispose();
            }
            renderDatas.Clear();
            //renderDataDict.Clear();
            for (int i = 0; i < editor.Editor.Terrains.Count; i++)
            {
                EditorTerrain terrain = editor.Editor.Terrains[i];
                EditorTerrainRenderHeightmap data = new EditorTerrainRenderHeightmap(editor.Editor);
                data.Initialize(editor.XNAGameControl, terrain.FullData);
                renderDatas.Add(data);
                //renderDataDict.Add( terrain, data );
            }

            // Create Lines

            Vector2 _totalSize = new Vector2(1024, 1024);
            Vector2 _tileSize = new Vector2(128, 128);
            Vector2 _center = new Vector2(0, 0);


            BuildTerrainCreateGrid(_totalSize, _tileSize, _center);
        }

        public void Update()
        {
            WorldEditor.PickResult pick = editor.PickWorld(WorldEditor.PickType.GroundPlane);

            pickedTerrain = false;
            if (pick.Picked)
            {
                editor.XNAGameControl.EditorCamera.OrbitPoint = pick.Point;
                if (pick.Point.X > -halfTotalSize.X && pick.Point.X < halfTotalSize.X
                    && pick.Point.Z > -halfTotalSize.Z && pick.Point.Z < halfTotalSize.Z)
                {
                    pickedTerrain = true;
                    Vector3 p = pick.Point + halfTotalSize;
                    p.X = (float)Math.Floor(p.X / tileSize.X) * tileSize.X;
                    p.Z = (float)Math.Floor(p.Z / tileSize.Z) * tileSize.Z;
                    p -= halfTotalSize;

                    PickedTerrainPos = p;

                }

            }


            if (editor.XNAGameControl.Mouse.LeftMouseJustPressed && pickedTerrain)
            {
                EditorTerrain t = editor.Editor.CreateTerrain();
                t.FullData.Position = PickedTerrainPos;
                t.FullData.SizeX = (int)tileSize.X;
                t.FullData.SizeZ = (int)tileSize.Z;
                t.FullData.BlockSize = 16;
                t.FullData.NumBlocksX = t.FullData.SizeX / t.FullData.BlockSize;
                t.FullData.NumBlocksZ = t.FullData.SizeZ / t.FullData.BlockSize;
                t.FullData.HeightMap = new HeightMap(t.FullData.SizeX + 1, t.FullData.SizeZ + 1);
                /*t.FullData.Textures.Add( new EditorTerrainFullData.TerrainTexture( t.FullData
                    , editor.Editor.Files.RootDirectory + @"\Content\CrackedEarth001.dds" ) );*/


                EditorTerrainRenderHeightmap renderData = new EditorTerrainRenderHeightmap(editor.Editor);
                renderData.Initialize(editor.XNAGameControl, t.FullData);

                renderDatas.Add(renderData);


            }

        }

        public void Render()
        {
            if (pickedTerrain)
            {
                Vector3 min = PickedTerrainPos;
                Vector3 max = PickedTerrainPos + tileSize;

                min.Y -= tileSize.X * 0.01f;
                max.Y += tileSize.X * 0.01f;


                editor.XNAGameControl.LineManager3D.AddBox(new BoundingBox(min, max), Color.Red);
                editor.XNAGameControl.LineManager3D.AddLine(min, max, Color.Red);
                editor.XNAGameControl.LineManager3D.AddLine(min + new Vector3(tileSize.X, 0, 0),
                                                            max - new Vector3(tileSize.X, 0, 0), Color.Red);

            }
            editor.XNAGameControl.LineManager3D.Render(gridLines);

            for (int i = 0; i < renderDatas.Count; i++)
            {
                EditorTerrainRenderHeightmap data = renderDatas[i];
                data.Render(editor.XNAGameControl);
            }
        }
    }
}