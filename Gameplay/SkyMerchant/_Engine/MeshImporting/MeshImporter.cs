using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    public class MeshImporter
    {
        private Material[] matStream;
        private Vector3[] posStream;
        private Vector3[] normStream;
        private Vector3[] texcoStream;

        public void ImportMesh(String path, out IMesh mesh)
        {
            var importer = new MeshParser();
            Dictionary<int, List<int>> posInd;
            Dictionary<int, List<int>> normInd;
            Dictionary<int, List<int>> texcoInd;
            importer.LoadMesh(path, out matStream, out posStream, out normStream, out texcoStream, out posInd, out normInd, out texcoInd);

            convertMaxToTW();

            var meshBuilder = new MeshBuilder();
            mesh = meshBuilder.BuildMesh(matStream, posStream, normStream, texcoStream, posInd, normInd, texcoInd);
        }

        private void convertMaxToTW()
        {
            //Swap vertex positions
            for (int i = 0; i < posStream.Count(); i++)
            {
                posStream[i] = swapXYZ(posStream[i]);
            }

            //Swap vertex normals
            for (int i = 0; i < normStream.Count(); i++)
            {
                normStream[i] = swapXYZ(normStream[i]);
            }

            //Flip Texture coordinates
            for (int i = 0; i < texcoStream.Count(); i++)
            {
                texcoStream[i] = flipTexCo(texcoStream[i]);
            }
        }

        private Vector3 swapXYZ(Vector3 toSwap)
        {
            return new Vector3(toSwap.Y, toSwap.Z, toSwap.X);
        }

        private Vector3 flipTexCo(Vector3 toFlip)
        {
            return new Vector3(toFlip.X, 1- toFlip.Y, toFlip.Z);
        }
    }
}
