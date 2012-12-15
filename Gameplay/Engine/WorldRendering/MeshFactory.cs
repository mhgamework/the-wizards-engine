using System;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Engine.WorldRendering
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

    }
}
