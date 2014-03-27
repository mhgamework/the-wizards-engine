using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scattered._Engine;
using ProceduralBuilder.Building;
using ProceduralBuilder.Shapes;

namespace MHGameWork.TheWizards.Scattered.ProcBuilder
{
    public class CachedIslandGenerator : IIslandGenerator
    {
        private readonly IIslandGenerator decorated;
        private readonly OBJExporter objExporter;

        public CachedIslandGenerator(IIslandGenerator decorated, OBJExporter objExporter)
        {
            this.decorated = decorated;
            this.objExporter = objExporter;
        }

        public List<IBuildingElement> GetIslandBase(int seed)
        {
            return decorated.GetIslandBase(seed);
        }

        public IMesh GetIslandMesh(List<IBuildingElement> islandBase, int seed)
        {
            var hash = GetIslandBaseHash(islandBase) - seed;

            var file = getCachedMeshFile(hash);

            if (!file.Exists)
            {
                file.Directory.Create();
                var gen = decorated.GetIslandMesh(islandBase, seed);
                objExporter.SaveToFile(objExporter.ConvertFromTWMesh(gen), file.FullName);
            }

            return TW.Assets.LoadMesh(getGamedataPath(hash));
        }

        private FileInfo getCachedMeshFile(int hash)
        {
            return new FileInfo(TWDir.GameData + "\\" + getGamedataPath(hash) + ".obj");
        }

        private string getGamedataPath(int hash)
        {
            return "Scattered\\Islands\\Island" + hash;
        }

        public int GetIslandBaseHash(List<IBuildingElement> elements)
        {
            var ret = 0;
            foreach (var el in elements)
            {
                if (!(el is Face)) throw new InvalidOperationException();
                var f = (Face)el;

                ret = (ret + f.WorldMatrix.GetHashCode()) % int.MaxValue / 2;
                ret = (ret + f.Size.GetHashCode()) % int.MaxValue / 2;
            }
            return ret;
        }
    }
}