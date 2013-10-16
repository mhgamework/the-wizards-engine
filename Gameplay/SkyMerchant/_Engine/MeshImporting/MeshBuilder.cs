using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    /// <summary>
    /// Respinsible for converting imported mesh data into an IMesh
    /// </summary>
    public class MeshBuilder
    {

        public IMesh BuildMesh(Material[] materials, Vector3[] positions, Vector3[] normals, Vector3[] texcoords, Dictionary<int, List<int>> positionIndices, Dictionary<int, List<int>> normalIndices, Dictionary<int, List<int>>  texCoordIndices)
        {
            var mesh = new RAMMesh();
            var positionInd = new List<int>();
            var normalInd = new List<int>();
            var texcoordInd = new List<int>();
            Vector3[] pos;
            Vector3[] norm;
            Vector2[] texco;

            for (int index = 1; index<=materials.Length; index++)
            {
                String diffPath = materials[index-1].Diff;

                positionIndices.TryGetValue(index, out positionInd);
                pos = new Vector3[positionInd.Count];
                for (int p = 0; p < positionInd.Count; p++)
                {
                    pos[p] = positions[positionInd[p]];
                }

                normalIndices.TryGetValue(index, out normalInd);
                norm = new Vector3[normalInd.Count];
                for (int n = 0; n < normalInd.Count; n++)
                {
                    norm[n] = normals[normalInd[n]];
                }

                texCoordIndices.TryGetValue(index, out texcoordInd);
                texco = new Vector2[texcoordInd.Count];
                for (int t = 0; t < texcoordInd.Count; t++)
                {
                    texco[t] = texcoords[texcoordInd[t]].TakeXY();
                }

                addMeshPart(mesh, diffPath, pos, norm, texco);
            }
            return mesh;
        }


        private void addMeshPart(IMesh mesh, String diffPath, Vector3[] positions, Vector3[] normals, Vector2[] texcoords)
        {
            // One material
            var part = new MeshCoreData.Part();
            mesh.GetCoreData().Parts.Add(part);
            var mat = new MeshCoreData.Material();
            part.MeshMaterial = mat;

            mat.DiffuseColor = new Color(2, 3, 4);
            mat.DiffuseMap = getTexture(diffPath); 

            part.ObjectMatrix = Matrix.Identity.xna();
            part.MeshPart = new RAMMeshPart();

            part.MeshPart.GetGeometryData().Sources.Add(new MeshPartGeometryData.Source() { DataVector3 = positions.Select(o => o.xna()).ToArray(), Semantic = MeshPartGeometryData.Semantic.Position, Number = 0 });
            part.MeshPart.GetGeometryData().Sources.Add(new MeshPartGeometryData.Source() { DataVector3 = normals.Select(o => o.xna()).ToArray(), Semantic = MeshPartGeometryData.Semantic.Normal, Number = 0 });
            part.MeshPart.GetGeometryData().Sources.Add(new MeshPartGeometryData.Source() { DataVector2 = texcoords.Select(o => new Microsoft.Xna.Framework.Vector2(o.X, o.Y)).ToArray(), Semantic = MeshPartGeometryData.Semantic.Texcoord, Number = 0 });
        }

        private ITexture getTexture(String diffPath)
        {
            if (diffPath == null)
                return null;

            var diffuseMap = new RAMTexture();
            diffuseMap.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            diffuseMap.GetCoreData().DiskFilePath = diffPath;
            return diffuseMap;
        }

    }
}
