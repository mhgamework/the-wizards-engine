using System;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.OBJParser;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// This class provides static access point for loading meshes. It is TW scoped, meaning that it is linked to the TW state. 
    /// This is done by storing the data in the TW.Model
    /// </summary>
    [Obsolete]
    public class MeshFactory
    {
        /// <summary>
        /// Loads a mesh in the TWDir.GameData folder. The path is supposed to be without extension
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public static IMesh Load(string relativeCorePath)
        {
            return TW.Assets.LoadMesh(relativeCorePath);
        }

        /// <summary>
        /// Returns the path from which this mesh was loaded if it was loading using this meshfactory
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static string GetLoadedPath(IMesh mesh)
        {
            return TW.Assets.GetLoadedPath(mesh);
        }

        /// <summary>
        /// Returns a cached BoundingBox for given mesh.
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static BoundingBox GetBoundingBox(IMesh mesh)
        {
            return TW.Assets.GetBoundingBox(mesh);
        }

    }
}
