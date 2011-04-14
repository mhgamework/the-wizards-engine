using System;
namespace MHGameWork.TheWizards.ServerClient
{
    public interface IGameObject2 : IDisposable
    {
        /// <summary>
        /// Load in RAM
        /// </summary>
        void Initialize();
        /// <summary>
        /// Load in GPU
        /// </summary>
        void LoadGraphicsContent();
        /// <summary>
        /// Unload GPU
        /// </summary>
        void UnloadGraphicsContent();
        /// <summary>
        /// Unload RAM
        /// </summary>
        void UnInitialize();

        /// <summary>
        /// LoadingManager task for loading in RAM
        /// </summary>
        Engine.LoadingTaskState InitializeTask( Engine.LoadingTaskType taskType );
        /// <summary>
        /// LoadingManager task for loading in GPU
        /// </summary>
        Engine.LoadingTaskState LoadGraphicsContentTask( Engine.LoadingTaskType taskType );
        /// <summary>
        /// LoadingManager task for unloading GPU
        /// </summary>
        Engine.LoadingTaskState UnloadGraphicsContentTask( Engine.LoadingTaskType taskType );
        /// <summary>
        /// LoadingManager task for unloading RAM
        /// </summary>
        Engine.LoadingTaskState UnInitializeTask( Engine.LoadingTaskType taskType );



        //
        // Heartbeat functions
        //
        void Process( MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e );
        void Render();
        void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e );
    }
}
