using MHGameWork.TheWizards.Engine.Testing;

namespace MHGameWork.TheWizards.Engine.Files
{
    /// <summary>
    /// Responsible for access to engine's filesystem.
    /// </summary>
    public interface IEngineFilesystem
    {
        /// <summary>
        /// Filename without extension
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        void StoreXml(object data, string filename);

        T LoadXml<T>(string getFileName);
    }
}