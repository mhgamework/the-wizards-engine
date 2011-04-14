namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public enum LoadingTaskState
    {
        /// <summary>
        /// Task has not yet started
        /// </summary>
        None,
        /// <summary>
        /// The task is waiting for some asynchronous function to complete and is not executing anything.
        /// This task should be ignored and the next one should be executed.
        /// </summary>
        Idle,
        /// <summary>
        /// The task has allready been run partially but has not yet loaded all data and should be called again.
        /// </summary>
        Subtasking,
        /// <summary>
        /// The task was completed successfully
        /// </summary>
        Completed
    }
}