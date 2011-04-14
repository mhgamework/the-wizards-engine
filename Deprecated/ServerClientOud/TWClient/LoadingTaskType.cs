namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public enum LoadingTaskType
    {
        None = 0,
        /// <summary>
        /// Used for loading creating PreProcessed data. This loading process usually takes longer as 2 seconds 
        /// so i will hang the application when loaded dynamically.
        /// </summary>
        PreProccesing,
        /// <summary>
        /// This content can be loaded while playing the game and will only cause a very small time to complete.
        /// </summary>
        Normal,
        /// <summary>
        /// Use for loading detail content. This data will only be loaded when all other content is loaded.
        /// </summary>
        Detail,

        Unload


    }
}