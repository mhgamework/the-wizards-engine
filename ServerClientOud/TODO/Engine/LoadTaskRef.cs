using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public struct LoadTaskRef
    {
        private LoadingManager manager;

        public LoadingManager Manager
        {
            get { return manager; }
            set { manager = value; }
        }
        private int taskID;

        public int TaskID
        {
            get { return taskID; }
            set { taskID = value; }
        }

        public LoadTaskRef( LoadingManager nManager, int nTaskID )
        {
            manager = nManager;
            taskID = nTaskID;
        }

        public LoadingTaskState State
        {
            get
            {
                if ( manager == null ) throw new InvalidOperationException( "This task reference has not yet been initialized!" );
                ILoadingTask task = manager.GetLoadingTask( taskID );
                if ( task == null ) throw new Exception( "The task must exist, removal of old tasks not yet implemented" );
                return task.State;
            }
        }

        public static LoadTaskRef Empty { get { return new LoadTaskRef(); } }

        public bool IsEmpty { get { return manager == null; } }

    }
}
