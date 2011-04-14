using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient.Terrain;

namespace MHGameWork.TheWizards.Terrain.Editor
{
    public class TerrainRaiseTool : IEditorTool
    {
        private WorldEditor editor;
        private TheWizards.Terrain.Editor.TerrainEditorForm guiForm;

        private List<EditorTerrainRenderHeightmap> renderDatas = new List<EditorTerrainRenderHeightmap>();
        private Dictionary<EditorTerrain, EditorTerrainRenderHeightmap> renderDataDict = new Dictionary<EditorTerrain, EditorTerrainRenderHeightmap>();

        private float range = 20;
        public float Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
                guiForm.spinRangeItem.EditValue = range;
                //if ( !editor.Editor.Form.txtWorldTerrainRange.Focused ) editor.Editor.Form.txtWorldTerrainRange.ControlText = Range.ToString();
            }
        }
        public float Strength = 10;


        public TerrainRaiseTool( WorldEditor _editor, TheWizards.Terrain.Editor.TerrainEditorForm _guiForm )
        {
            editor = _editor;
            guiForm = _guiForm;
            guiForm.btnRaise.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler( btnRaise_ItemClick );
        }

        void btnRaise_ItemClick( object sender, DevExpress.XtraBars.ItemClickEventArgs e )
        {
            editor.ActivateTool( this );
        }


        public void Activate()
        {
            guiForm.btnRaise.Down = true;

            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                EditorTerrainRenderHeightmap data = renderDatas[ i ];
                data.Dispose();
            }
            renderDatas.Clear();
            renderDataDict.Clear();
            for ( int i = 0; i < editor.Editor.Terrains.Count; i++ )
            {
                EditorTerrain terrain = editor.Editor.Terrains[ i ];
                EditorTerrainRenderHeightmap data = new EditorTerrainRenderHeightmap( editor.Editor );
                data.Initialize( editor.XNAGameControl, terrain.FullData );
                renderDatas.Add( data );
                renderDataDict.Add( terrain, data );
            }



        }

        public void Deactivate()
        {
            guiForm.btnRaise.Down = false;
        }

        public void Update()
        {
            WorldEditor.PickResult pick;

            Ray centerRay;
            Vector2 center = new Vector2( editor.XNAGameControl.Size.Width * 0.5f, editor.XNAGameControl.Size.Height * 0.5f );
            centerRay = editor.XNAGameControl.GetWereldViewRay( center );

            pick = editor.PickWorld( WorldEditor.PickType.GroundPlane | WorldEditor.PickType.Terrains, centerRay );

            editor.XNAGameControl.EditorCamera.OrbitPoint = pick.Point;

            pick = editor.PickWorld( WorldEditor.PickType.Terrains );

            editor.XNAGameControl.EditorCamera.OrbitPoint = pick.Point;

            /*if ( !float.TryParse( editor.Editor.Form.txtWorldTerrainRange.ControlText, out Range ) )
            {
                // TODO: Default range value
                Range = 50;
            }*/

            if ( editor.XNAGameControl.Mouse.RelativeScrollWheel != 0 )
            {
                Range += Range * 0.1f * (float)Math.Sign( editor.XNAGameControl.Mouse.RelativeScrollWheel );
                if ( Range < 0 ) Range = 1f; // TODO: minimum range
            }

            if ( pick.Type == WorldEditor.PickType.Terrains )
            {
                editor.XNAGameControl.LineManager3D.AddLine( pick.Point, pick.Point + Vector3.Up * 10f, Color.Green );
                editor.XNAGameControl.LineManager3D.AddCenteredBox( pick.Point, 1f, Color.Green );
                if ( editor.XNAGameControl.Mouse.LeftMousePressed )
                {
                    for ( int i = 0; i < editor.Editor.Terrains.Count; i++ )
                    {
                        EditorTerrain terrain = editor.Editor.Terrains[ i ];
                        raiseTerrainInternal( terrain, pick.Point.X, pick.Point.Z, Range, editor.XNAGameControl.Elapsed * Strength );

                    }

                }
                if ( editor.XNAGameControl.Mouse.RightMousePressed )
                {
                    for ( int i = 0; i < editor.Editor.Terrains.Count; i++ )
                    {
                        EditorTerrain terrain = editor.Editor.Terrains[ i ];
                        raiseTerrainInternal( terrain, pick.Point.X, pick.Point.Z, Range, -editor.XNAGameControl.Elapsed * Strength );

                    }

                }
            }


        }


        /// <summary>
        /// WARNING: this is a copy of RaiseTerrain!
        /// </summary>
        /// <param name="terr"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="range"></param>
        /// <param name="height"></param>
        [Obsolete("This is a copy of RaiseTerrain and should be removed/adapted")]
        private void raiseTerrainInternal( EditorTerrain terr, float x, float z, float range, float height )
        {

            /**
             * 
             * WARNING
             * WARNING
             * WARNING
             * WARNING
             * WARNING
             * 
             * This is a copy of RaiseTerrain and should be removed/adapted.
             * 
             * */


            TerrainFullData terrainData = terr.FullData;

            //Vector3 transformed = Vector3.Transform( new Vector3( position.X, 0, position.Y ), Matrix.Invert( WorldMatrix ) );
            //Vector2 localPosition = new Vector2( transformed.X, transformed.Z );

            float SizeX = terrainData.SizeX + 1;
            float SizeY = terrainData.SizeZ + 1;

            x -= terrainData.Position.X;
            z -= terrainData.Position.Z;


            int minX = (int)Math.Floor( x - range );
            int maxX = (int)Math.Floor( x + range ) + 1;
            int minZ = (int)Math.Floor( z - range );
            int maxZ = (int)Math.Floor( z + range ) + 1;



            if ( minX > SizeX - 1 ) return;
            if ( minZ > SizeY - 1 ) return;
            if ( maxX < 0 ) return;
            if ( maxZ < 0 ) return;

            minX = (int)MathHelper.Clamp( minX, 0, SizeX - 1 );
            maxX = (int)MathHelper.Clamp( maxX, 0, SizeX - 1 );
            minZ = (int)MathHelper.Clamp( minZ, 0, SizeY - 1 );
            maxZ = (int)MathHelper.Clamp( maxZ, 0, SizeY - 1 );


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

            float[] data = new float[ ( areaSizeX ) * ( areaSizeZ ) ];


            float rangeSq = range * range;

            for ( int ix = minX; ix <= maxX; ix++ )
            {
                for ( int iz = minZ; iz <= maxZ; iz++ )
                {
                    float distSq = Vector2.DistanceSquared( new Vector2( x, z ), new Vector2( ix, iz ) );
                    float iHeight;
                    if ( distSq <= rangeSq )
                    {
                        float dist = (float)Math.Sqrt( distSq );
                        float factor = 1 - dist / range;

                        iHeight = terrainData.HeightMap.AddHeight( ix, iz, height * factor );


                    }
                    else
                    {
                        iHeight = terrainData.HeightMap.GetHeight( ix, iz );
                    }
                    data[ ( iz - minZ ) * ( areaSizeX ) + ix - minX ] = iHeight;
                }

            }


            //NOTE: On certain ATI cards (bart zijn fout: ATI HD4850) 
            //NOTE:     a width or height of 1 results in a division by zero error by ati driver.
            if ( rect.Width <= 1 || rect.Height <= 1 ) return;

            renderDataDict[ terr ].HeightMapTexture.SetData<float>( 0, rect, data, 0, data.Length, SetDataOptions.None );


        }

        public static void RaiseTerrain( TerrainFullData terrainData, float x, float z, float range, float height )
        {

            //Vector3 transformed = Vector3.Transform( new Vector3( position.X, 0, position.Y ), Matrix.Invert( WorldMatrix ) );
            //Vector2 localPosition = new Vector2( transformed.X, transformed.Z );

            float SizeX = terrainData.SizeX + 1;
            float SizeY = terrainData.SizeZ + 1;

            x -= terrainData.Position.X;
            z -= terrainData.Position.Z;


            int minX = (int)Math.Floor( x - range );
            int maxX = (int)Math.Floor( x + range ) + 1;
            int minZ = (int)Math.Floor( z - range );
            int maxZ = (int)Math.Floor( z + range ) + 1;



            if ( minX > SizeX - 1 ) return;
            if ( minZ > SizeY - 1 ) return;
            if ( maxX < 0 ) return;
            if ( maxZ < 0 ) return;

            minX = (int)MathHelper.Clamp( minX, 0, SizeX - 1 );
            maxX = (int)MathHelper.Clamp( maxX, 0, SizeX - 1 );
            minZ = (int)MathHelper.Clamp( minZ, 0, SizeY - 1 );
            maxZ = (int)MathHelper.Clamp( maxZ, 0, SizeY - 1 );


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );


            float rangeSq = range * range;

            for ( int ix = minX; ix <= maxX; ix++ )
            {
                for ( int iz = minZ; iz <= maxZ; iz++ )
                {
                    float distSq = Vector2.DistanceSquared( new Vector2( x, z ), new Vector2( ix, iz ) );
                    float iHeight;
                    if ( distSq <= rangeSq )
                    {
                        float dist = (float)Math.Sqrt( distSq );
                        float factor = 1 - dist / range;

                        iHeight = terrainData.HeightMap.AddHeight( ix, iz, height * factor );


                    }
                    else
                    {
                        iHeight = terrainData.HeightMap.GetHeight( ix, iz );
                    }
                }

            }


        }

        public void Render()
        {

            //TODO


            /*if ( pickedTerrain )
            {
                Vector3 min = PickedTerrainPos;
                Vector3 max = PickedTerrainPos + tileSize;

                min.Y -= tileSize.X * 0.01f;
                max.Y += tileSize.X * 0.01f;


                editor.XNAGameControl.LineManager3D.AddBox( new BoundingBox( min, max ), Color.Red );
                editor.XNAGameControl.LineManager3D.AddLine( min, max, Color.Red );
                editor.XNAGameControl.LineManager3D.AddLine( min + new Vector3( tileSize.X, 0, 0 ),
                                                             max - new Vector3( tileSize.X, 0, 0 ), Color.Red );

            }
            editor.XNAGameControl.LineManager3D.Render( gridLines );*/

            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                EditorTerrainRenderHeightmap data = renderDatas[ i ];
                data.Render( editor.XNAGameControl );
            }
        }
    }
}