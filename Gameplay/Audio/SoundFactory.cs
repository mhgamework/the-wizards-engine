namespace MHGameWork.TheWizards.Audio
{
    /// <summary>
    /// TODO: unify to an AssetFactory
    /// Responsible for providing a TW scoped ISound factory
    /// </summary>
    public class SoundFactory
    {
        /// <summary>
        /// Loads a mesh in the TWDir.GameData folder. The path is supposed to be WITH extension!!!
        /// </summary>
        /// <param name="relativeCorePath"></param>
        /// <returns></returns>
        public static ISound Load(string relativeCorePath)
        {
            return new DiskSound { Path = TWDir.GameData + "\\" + relativeCorePath };
        }

    }
}
