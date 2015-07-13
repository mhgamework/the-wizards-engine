using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor.Terrain
{
    public class TerrainPaintTool : IEditorTool
    {

        private WorldEditor editor;

        private List<EditorTerrainRenderTextured> renderDatas = new List<EditorTerrainRenderTextured>();
        private Dictionary<EditorTerrain, EditorTerrainRenderTextured> renderDataDict = new Dictionary<EditorTerrain, EditorTerrainRenderTextured>();

        private string selectedTexturePath;

        public TerrainPaintTool( WorldEditor _editor )
        {
            editor = _editor;
        }


        public void Activate()
        {
            editor.Editor.Form.editorWindowTerrainTextures.textureChooser.SelectedItemChanged -= new EventHandler( textureChooser_SelectedItemChanged );
            editor.Editor.Form.editorWindowTerrainTextures.textureChooser.SelectedItemChanged += new EventHandler( textureChooser_SelectedItemChanged );

            for ( int i = 0; i < renderDatas.Count; i++ )
            {
                EditorTerrainRenderTextured data = renderDatas[ i ];
                data.Dispose();
            }
            renderDatas.Clear();
            renderDataDict.Clear();
            for ( int i = 0; i < editor.Editor.Terrains.Count; i++ )
            {
                EditorTerrain terrain = editor.Editor.Terrains[ i ];
                EditorTerrainRenderTextured data = new EditorTerrainRenderTextured( editor.Editor );
                data.Initialize( editor.XNAGameControl, terrain.FullData );
                renderDatas.Add( data );
                renderDataDict.Add( terrain, data );
            }



        }

        public void Deactivate()
        {
        }

        void textureChooser_SelectedItemChanged( object sender, EventArgs e )
        {
            if ( editor.Editor.Form.editorWindowTerrainTextures.textureChooser.SelectedItem == null ) selectedTexturePath = null;
            else
                selectedTexturePath = editor.Editor.Form.editorWindowTerrainTextures.textureChooser.SelectedItem.Path;

        }

        public void Update(  )
        {
            WorldEditor.PickResult pick;

            if ( editor.XNAGameControl.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
            {
                //Reload
                Activate();
            }

            Ray centerRay;
            Vector2 center = new Vector2( editor.XNAGameControl.Size.Width * 0.5f, editor.XNAGameControl.Size.Height * 0.5f );
            centerRay = editor.XNAGameControl.GetWereldViewRay( center );

            pick = editor.PickWorld( WorldEditor.PickType.Terrains, centerRay );
            if ( pick.Picked == false )
            {
                pick = editor.PickWorld( WorldEditor.PickType.GroundPlane, centerRay );

            }

            editor.XNAGameControl.EditorCamera.OrbitPoint = pick.Point;

            pick = editor.PickWorld( WorldEditor.PickType.Terrains );

            //editor.XNAGameControl.EditorCamera.OrbitPoint = pick.Point;

            float range;
            if ( !float.TryParse( editor.Editor.Form.txtWorldTerrainRange.ControlText, out range ) )
            {
                // TODO: Default range value
                range = 50;
            }

            if ( editor.XNAGameControl.Mouse.RelativeScrollWheel != 0 )
            {
                range += range * 0.1f * (float)Math.Sign( editor.XNAGameControl.Mouse.RelativeScrollWheel );
                if ( range < 0 ) range = 1f; // TODO: minimum range
            }

            if ( !editor.Editor.Form.txtWorldTerrainRange.Focused ) editor.Editor.Form.txtWorldTerrainRange.ControlText = range.ToString();

            if ( selectedTexturePath == null ) return;
            if ( pick.Type == WorldEditor.PickType.Terrains )
            {
                editor.XNAGameControl.LineManager3D.AddLine( pick.Point, pick.Point + Vector3.Up * 10f, Color.Green );
                editor.XNAGameControl.LineManager3D.AddCenteredBox( pick.Point, 1f, Color.Green );
                if ( editor.XNAGameControl.Mouse.LeftMousePressed )
                {
                    for ( int i = 0; i < editor.Editor.Terrains.Count; i++ )
                    {
                        EditorTerrain terrain = editor.Editor.Terrains[ i ];

                        int textureIndex = -1;

                        // Find the texture
                        for ( int iTex = 0; iTex < terrain.FullData.Textures.Count; iTex++ )
                        {
                            TerrainFullData.TerrainTexture texture = terrain.FullData.Textures[ iTex ];
                            if ( texture.DiffuseTexture == selectedTexturePath )
                            {
                                textureIndex = iTex;
                                break;
                            }
                        }
                        if ( textureIndex == -1 )
                        {
                            terrain.FullData.Textures.Add( new TerrainFullData.TerrainTexture( terrain.FullData, selectedTexturePath ) );
                            textureIndex = terrain.FullData.Textures.Count - 1;


                            renderDatas.Remove( renderDataDict[ terrain ] );
                            renderDataDict[ terrain ].Dispose();
                            renderDataDict.Remove( terrain );


                            EditorTerrainRenderTextured data = new EditorTerrainRenderTextured( editor.Editor );
                            data.Initialize( editor.XNAGameControl, terrain.FullData );
                            renderDatas.Add( data );
                            renderDataDict.Add( terrain, data );
                        }


                        PaintWeight( terrain, renderDataDict[ terrain ], pick.Point.X, pick.Point.Z, range, textureIndex, editor.XNAGameControl.Elapsed * 0.1f );

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
                EditorTerrainRenderTextured data = renderDatas[ i ];
                data.Render( editor.XNAGameControl );
            }
        }

        public void PaintWeight( EditorTerrain terr, EditorTerrainRenderTextured renderData, float x, float z, float range, int texNum, float weight )
        {
            /*Vector3 transformed = Vector3.Transform( new Vector3( x, 0, z ), Matrix.Invert( WorldMatrix ) );
            x = transformed.X;
            z = transformed.Z;*/
            x -= terr.FullData.Position.X;
            z -= terr.FullData.Position.Z;

            int SizeX = terr.FullData.SizeX + 1;
            int SizeY = terr.FullData.SizeZ + 1;



            List<TerrainFullData.TerrainTexture> textures = terr.FullData.Textures;
            List<Texture2D> weightMaps = renderData.WeightMapTextures;


            //int iWeightMap = (int)Math.Floor( texNum / (double)4 );
            int relativeTexNum = texNum % 4;


            //Texture2D weightMap = weightMaps[ iWeightMap ];



            int minX = (int)Math.Floor( x - range );
            int maxX = (int)Math.Floor( x + range ) + 1;
            int minZ = (int)Math.Floor( z - range );
            int maxZ = (int)Math.Floor( z + range ) + 1;

            // Check if out of terrain
            if ( maxX < 0 ) return;
            if ( minX > SizeX ) return;
            if ( maxZ < 0 ) return;
            if ( minZ > SizeY ) return;

            minX = (int)MathHelper.Clamp( minX, 0, SizeX - 1 );
            maxX = (int)MathHelper.Clamp( maxX, 0, SizeX - 1 );
            minZ = (int)MathHelper.Clamp( minZ, 0, SizeY - 1 );
            maxZ = (int)MathHelper.Clamp( maxZ, 0, SizeY - 1 );


            int areaSizeX = maxX - minX + 1;
            int areaSizeZ = maxZ - minZ + 1;

            Rectangle rect = new Rectangle( minX, minZ, areaSizeX, areaSizeZ );

            Color[][] data = new Color[ weightMaps.Count ][];

            for ( int iWeight = 0; iWeight < weightMaps.Count; iWeight++ )
            {
                data[ iWeight ] = new Color[ ( areaSizeX ) * ( areaSizeZ ) ];
            }


            float rangeSq = range * range;
            x -= minX;
            z -= minZ;

            for ( int ix = 0; ix < areaSizeX; ix++ )
            {
                for ( int iz = 0; iz < areaSizeZ; iz++ )
                {
                    float distSq = Vector2.DistanceSquared( new Vector2( ix, iz ), new Vector2( x, z ) );
                    if ( distSq < rangeSq )
                    {
                        float dist = (float)Math.Sqrt( distSq );

                        float factor = 1 - ( dist / range );
                        factor *= 255;


                        factor *= weight;
                        factor += 5;
                        factor = MathHelper.Clamp( factor, 0, 255 );


                        byte total = 0;

                        //Deel elke kleur door het nieuwe totaal * 255
                        for ( int iTex = 0; iTex < textures.Count; iTex++ )
                        {
                            float val = textures[ iTex ].AlphaMap.GetSample( ix + minX, iz + minZ );
                            val = val / ( 255 + factor ) * 255;
                            val = (float)Math.Floor( val );
                            textures[ iTex ].AlphaMap.SetSample( ix + minX, iz + minZ, (byte)val );
                            if ( iTex != texNum ) total += (byte)val;
                        }


                        //Zorgt dat de som exact 255 is, de overschot gaat naar de gekozen weight


                        textures[ texNum ].AlphaMap.SetSample( ix + minX, iz + minZ, (byte)( 255 - total ) );



                    }
                    for ( int iWeightMap = 0; iWeightMap < weightMaps.Count; iWeightMap++ )
                    {
                        data[ iWeightMap ][ iz * ( areaSizeX ) + ix ] = terr.FullData.CalculateWeightMapColor( ix + minX, iz + minZ, iWeightMap );
                    }
                }
            }


            //NOTE: On certain ATI cards (bart zijn fout: ATI HD4850) 
            //NOTE:     a width or height of 1 results in a division by zero error by ati driver.
            if ( rect.Width <= 1 || rect.Height <= 1 ) return;

            for ( int iWeightMap = 0; iWeightMap < weightMaps.Count; iWeightMap++ )
            {
                //TODO: this line??
                weightMaps[ iWeightMap ].GraphicsDevice.Textures[ 0 ] = null;
                //BaseTerrain.Device.Textures[ 0 ] = null;

                weightMaps[ iWeightMap ].SetData<Color>( 0, rect, data[ iWeightMap ], 0, data[ iWeightMap ].Length, SetDataOptions.None );


            }

        }
    }
}