using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreeGenerator.NoiseGenerater
{
    public class SimpleTerrain
    {
        private XNAGame game;
        private VertexBuffer vertexBuffer;
        private DynamicVertexBuffer dVertexBuffer;
        private VertexDeclaration decl;
        private int vertexCount;
        private int triangleCount;
        private int vertexStride;
        private List<VertexPositionNormalColor> vertices = new List<VertexPositionNormalColor>();
        private BasicEffect effect;
        public int vertexCountWidth;
        private Matrix World = Matrix.Identity;
        public SimpleTerrain(MHGameWork.TheWizards.Graphics.XNAGame game, List<Microsoft.Xna.Framework.Vector3> positions, List<Color> colors, int vertexCountWidth, int vertexCountHeigth)
        {
            vertices = new List<VertexPositionNormalColor>();
            this.game = game;
            int positionCount = positions.Count;
            this.vertexCountWidth = vertexCountWidth;
            for (int count = 0; count < vertexCountWidth - 1; count++)
            {
                for (int i = 0; i < vertexCountWidth - 1; i++)
                {


                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth], GetAveragedNormal(i, count, positions), colors[i + count * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], GetAveragedNormal(i, count + 1, positions), colors[i + (count + 1) * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + count * vertexCountWidth + 1]));

                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + count * vertexCountWidth + 1]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], GetAveragedNormal(i, count + 1, positions), colors[i + (count + 1) * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + (count + 1) * vertexCountWidth + 1]));

                    //vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], colors[i + (count + 1) * vertexCountWidth]));
                    //vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], colors[i + count * vertexCountWidth + 1]));

                    //vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], colors[i + count * vertexCountWidth + 1]));
                    //vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], colors[i + (count + 1) * vertexCountWidth]));
                    //vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth + 1], colors[i + (count + 1) * vertexCountWidth + 1]));

                }
            }
            vertexCount = vertices.Count;
            triangleCount = vertexCount / 3;
            vertexStride = VertexPositionNormalColor.SizeInBytes;
        }
        public SimpleTerrain(MHGameWork.TheWizards.Graphics.XNAGame game, List<Microsoft.Xna.Framework.Vector3> positions, List<Color> colors, int vertexCountWidth, int vertexCountHeigth, Matrix world)
        {
            World = world;
            vertices = new List<VertexPositionNormalColor>();
            this.game = game;
            int positionCount = positions.Count;
            this.vertexCountWidth = vertexCountWidth;
            for (int count = 0; count < vertexCountWidth - 1; count++)
            {
                for (int i = 0; i < vertexCountWidth - 1; i++)
                {


                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth], GetAveragedNormal(i, count, positions), colors[i + count * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], GetAveragedNormal(i, count + 1, positions), colors[i + (count + 1) * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + count * vertexCountWidth + 1]));

                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + count * vertexCountWidth + 1]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], GetAveragedNormal(i, count + 1, positions), colors[i + (count + 1) * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + (count + 1) * vertexCountWidth + 1]));

                    //vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], colors[i + (count + 1) * vertexCountWidth]));
                    //vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], colors[i + count * vertexCountWidth + 1]));

                    //vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], colors[i + count * vertexCountWidth + 1]));
                    //vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], colors[i + (count + 1) * vertexCountWidth]));
                    //vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth + 1], colors[i + (count + 1) * vertexCountWidth + 1]));

                }
            }
            vertexCount = vertices.Count;
            triangleCount = vertexCount / 3;
            vertexStride = VertexPositionNormalColor.SizeInBytes;
        }

        public void CreateRenderData()
        {
            effect = new BasicEffect(game.GraphicsDevice, new EffectPool());
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.LightingEnabled = true;
            effect.EnableDefaultLighting();

            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, -1, 0.5f));

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalColor), vertexCount, BufferUsage.None);
            decl = new VertexDeclaration(game.GraphicsDevice, VertexPositionNormalColor.VertexElements);
            vertexBuffer.SetData(vertices.ToArray());

        }
        public void CreateDynamicRenderData(List<Microsoft.Xna.Framework.Vector3> positions, List<Color> colors)
        {
            vertices = new List<VertexPositionNormalColor>();
            for (int count = 0; count < vertexCountWidth - 1; count++)
            {
                for (int i = 0; i < vertexCountWidth - 1; i++)
                {

                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth], GetAveragedNormal(i, count, positions), colors[i + count * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], GetAveragedNormal(i, count + 1, positions), colors[i + (count + 1) * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + count * vertexCountWidth + 1]));

                    vertices.Add(new VertexPositionNormalColor(positions[i + count * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + count * vertexCountWidth + 1]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth], GetAveragedNormal(i, count + 1, positions), colors[i + (count + 1) * vertexCountWidth]));
                    vertices.Add(new VertexPositionNormalColor(positions[i + (count + 1) * vertexCountWidth + 1], GetAveragedNormal(i + 1, count, positions), colors[i + (count + 1) * vertexCountWidth + 1]));

                }
            }
            effect = new BasicEffect(game.GraphicsDevice, new EffectPool());
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;
            effect.LightingEnabled = true;
            effect.EnableDefaultLighting();

            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0.5f, -1, 0.5f));

            dVertexBuffer = new DynamicVertexBuffer(game.GraphicsDevice, typeof(VertexPositionNormalColor), vertexCount, BufferUsage.None);
            decl = new VertexDeclaration(game.GraphicsDevice, VertexPositionNormalColor.VertexElements);
            dVertexBuffer.SetData(vertices.ToArray());

        }
        //public Vector3 GetAveragedNormal(int x, int y,List<Vector3> positions)
        //{
        //    Vector3 normal = new Vector3();

        //    // top left
        //    if (x > 0 && y > 2 && y < vertexCountWidth - 1 && x < vertexCountWidth-1)//normaly should be length but for now good enough
        //        normal += GetNormal(x - 1, y - 1, positions);

        //    // top center
        //    if (y > 2 && y < vertexCountWidth - 1 && x < vertexCountWidth-1)
        //        normal += GetNormal(x, y - 1, positions);

        //    // top right
        //    if (x < heightMap.Width && z > 0)
        //        normal += GetNormal(x + 1, z - 1);

        //    // middle left
        //    if (x > 0 && y > 1 && y < vertexCountWidth - 1 && x < vertexCountWidth-1)
        //        normal += GetNormal(x - 1, y, positions);

        //    // middle center
        //    if (y > 1 && y < vertexCountWidth - 1 && x < vertexCountWidth-1)
        //    normal += GetNormal(x, y, positions);

        //    // middle right
        //    if (x < heightMap.Width)
        //        normal += GetNormal(x + 1, z);

        //    // lower left
        //    if (x > 0 && z < heightMap.Length)
        //        normal += GetNormal(x - 1, z + 1);

        //    // lower center
        //    if (z < heightMap.Length)
        //        normal += GetNormal(x, z + 1);

        //    // lower right
        //    if (x < heightMap.Width && z < heightMap.Length)
        //        normal += GetNormal(x + 1, z + 1);


        //    return Vector3.Normalize(normal);
        //}
        public Vector3 GetAveragedNormal(int x, int z, List<Vector3> positions)
        {
            Vector3 normal = new Vector3();

            // top left
            if (x > 0 && z > 0)
                normal += GetNormal(x - 1, z - 1, positions);

            // top center
            if (z > 0)
                normal += GetNormal(x, z - 1, positions);

            // top right
            if (x < vertexCountWidth && z > 0)
                normal += GetNormal(x + 1, z - 1, positions);

            // middle left
            if (x > 0)
                normal += GetNormal(x - 1, z, positions);

            // middle center
            normal += GetNormal(x, z, positions);

            // middle right
            if (x < vertexCountWidth)
                normal += GetNormal(x + 1, z, positions);

            // lower left
            if (x > 0 && z < vertexCountWidth)
                normal += GetNormal(x - 1, z + 1, positions);

            // lower center
            if (z < vertexCountWidth)
                normal += GetNormal(x, z + 1, positions);

            // lower right
            if (x < vertexCountWidth && z < vertexCountWidth)
                normal += GetNormal(x + 1, z + 1, positions);

            return Vector3.Normalize(normal);
        }

        public Vector3 GetNormal(int x, int y, List<Vector3> positions)
        {
            Vector3 v1 = getSafePosition(positions, x, y + 1);//x,z+
            Vector3 v2 = getSafePosition(positions, x, y - 1); //x z--
            Vector3 v3 = getSafePosition(positions, x + 1, y + 1); //x+1,z
            Vector3 v4 = getSafePosition(positions, x - 1, y + 1); //x-1, z

            return Vector3.Normalize(Vector3.Cross(v1 - v2, v3 - v4));
        }

        private Vector3 getSafePosition(List<Vector3> positions, int x, int y)
        {
            if (x < 0 || y < 0 || x > vertexCountWidth - 1 || y > vertexCountWidth - 1)
                return new Vector3(x, 0, y);
            return positions[x + y * vertexCountWidth];
        }

        public struct VertexPositionNormalColor
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Color Color;
            public VertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
            {
                Position = position;
                Normal = normal;
                Color = color;
            }
            public static readonly VertexElement[] VertexElements = new VertexElement[]
                                                                        {
                                                                            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
                                                                            new VertexElement(0, 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                                                                            new VertexElement(0, 24, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)

                                                                        };

            public static int SizeInBytes
            {
                get { return 28; }
            }

        }
        private void renderPrimitives()
        {
            game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            game.GraphicsDevice.VertexDeclaration = decl;
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);
        }
        public void Render()
        {
            //game.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
            game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            effect.World = World;
            effect.View = game.Camera.View;
            effect.Projection = game.Camera.Projection;


            effect.Begin(SaveStateMode.None);

            // Render all passes (usually just one)
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            for (int num = 0; num < effect.CurrentTechnique.Passes.Count; num++)
            {
                EffectPass pass = effect.CurrentTechnique.Passes[num];

                pass.Begin();
                renderPrimitives();
                pass.End();
            } // foreach (pass)

            // End shader
            effect.End();
        }
    }
}
