using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Rendering
{
    public static class GeometryHelper
    {

        public static void CreateUnitBoxVerticesAndIndices(out TangentVertex[] vertices, out short[] indices)
        {
            vertices = new TangentVertex[4 * 6];

            int i;

            //Note: right handed axis

            // Front (topleft, topright, bottomleft, bottomright)
            i = 4 * 0;
            vertices[i].pos = new Vector3(0, 1, 1); i++;
            vertices[i].pos = new Vector3(1, 1, 1); i++;
            vertices[i].pos = new Vector3(0, 0, 1); i++;
            vertices[i].pos = new Vector3(1, 0, 1); i++;
            i -= 4;
            vertices[i].uv = new Vector2(0, 0); i++;
            vertices[i].uv = new Vector2(1, 0); i++;
            vertices[i].uv = new Vector2(0, 1); i++;
            vertices[i].uv = new Vector2(1, 1); i++;

            for (int j = i - 4; j < i; j++)
            {
                vertices[j].normal = Vector3.Backward;
            }

            // Back (topleft, topright, bottomleft, bottomright)
            i = 4 * 1;
            vertices[i].pos = new Vector3(1, 1, 0); i++;
            vertices[i].pos = new Vector3(0, 1, 0); i++;
            vertices[i].pos = new Vector3(1, 0, 0); i++;
            vertices[i].pos = new Vector3(0, 0, 0); i++;
            i -= 4;
            vertices[i].uv = new Vector2(0, 0); i++;
            vertices[i].uv = new Vector2(1, 0); i++;
            vertices[i].uv = new Vector2(0, 1); i++;
            vertices[i].uv = new Vector2(1, 1); i++;

            for (int j = i - 4; j < i; j++)
            {
                vertices[j].normal = Vector3.Forward;
            }


            // Left (topleft, topright, bottomleft, bottomright)
            i = 4 * 2;
            vertices[i].pos = new Vector3(0, 1, 0); i++;
            vertices[i].pos = new Vector3(0, 1, 1); i++;
            vertices[i].pos = new Vector3(0, 0, 0); i++;
            vertices[i].pos = new Vector3(0, 0, 1); i++;
            i -= 4;
            vertices[i].uv = new Vector2(0, 0); i++;
            vertices[i].uv = new Vector2(1, 0); i++;
            vertices[i].uv = new Vector2(0, 1); i++;
            vertices[i].uv = new Vector2(1, 1); i++;

            for (int j = i - 4; j < i; j++)
            {
                vertices[j].normal = Vector3.Left;
            }

            // Right (topleft, topright, bottomleft, bottomright)
            i = 4 * 3;
            vertices[i].pos = new Vector3(1, 1, 1); i++;
            vertices[i].pos = new Vector3(1, 1, 0); i++;
            vertices[i].pos = new Vector3(1, 0, 1); i++;
            vertices[i].pos = new Vector3(1, 0, 0); i++;
            i -= 4;
            vertices[i].uv = new Vector2(0, 0); i++;
            vertices[i].uv = new Vector2(1, 0); i++;
            vertices[i].uv = new Vector2(0, 1); i++;
            vertices[i].uv = new Vector2(1, 1); i++;

            for (int j = i - 4; j < i; j++)
            {
                vertices[j].normal = Vector3.Right;
            }

            // Top (topleft, topright, bottomleft, bottomright)
            i = 4 * 4;
            vertices[i].pos = new Vector3(0, 1, 0); i++;
            vertices[i].pos = new Vector3(1, 1, 0); i++;
            vertices[i].pos = new Vector3(0, 1, 1); i++;
            vertices[i].pos = new Vector3(1, 1, 1); i++;
            i -= 4;
            vertices[i].uv = new Vector2(0, 0); i++;
            vertices[i].uv = new Vector2(1, 0); i++;
            vertices[i].uv = new Vector2(0, 1); i++;
            vertices[i].uv = new Vector2(1, 1); i++;

            for (int j = i - 4; j < i; j++)
            {
                vertices[j].normal = Vector3.Up;
            }

            // Bottom (topleft, topright, bottomleft, bottomright)
            i = 4 * 5;
            vertices[i].pos = new Vector3(0, 0, 1); i++;
            vertices[i].pos = new Vector3(1, 0, 1); i++;
            vertices[i].pos = new Vector3(0, 0, 0); i++;
            vertices[i].pos = new Vector3(1, 0, 0); i++;
            i -= 4;
            vertices[i].uv = new Vector2(0, 1); i++;
            vertices[i].uv = new Vector2(1, 1); i++;
            vertices[i].uv = new Vector2(0, 0); i++;
            vertices[i].uv = new Vector2(1, 0); i++;

            for (int j = i - 4; j < i; j++)
            {
                vertices[j].normal = Vector3.Down;
            }

            indices = new short[6 * 6];
            i = 0;
            for (short j = 0; j < 4 * 6; j += 4)
            {
                indices[i] = (short)(j + 0); i++;
                indices[i] = (short)(j + 1); i++;
                indices[i] = (short)(j + 2); i++;

                indices[i] = (short)(j + 1); i++;
                indices[i] = (short)(j + 3); i++;
                indices[i] = (short)(j + 2); i++;
            }
        }

    }
}