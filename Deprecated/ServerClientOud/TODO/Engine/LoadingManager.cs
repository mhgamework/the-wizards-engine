using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class LoadingManager
    {
        ServerClientMainOud engine;
        //public Queue<ILoadingTask> taskQueue = new Queue<ILoadingTask>();
        //public List<ILoadingTask> tasks = new List<ILoadingTask>();
        //public int firstTaskIndex = 0;

        public AdvancedQueue<ILoadingTask> preProcessingTasks = new AdvancedQueue<ILoadingTask>();
        public AdvancedQueue<ILoadingTask> normalTasks = new AdvancedQueue<ILoadingTask>();
        public AdvancedQueue<ILoadingTask> detailTasks = new AdvancedQueue<ILoadingTask>();
        public AdvancedQueue<ILoadingTask> unloadTasks = new AdvancedQueue<ILoadingTask>();

        public List<ILoadingTask> allTasks = new List<ILoadingTask>();

        public int[] lastTimeProcesseds;

        public bool IsDoneLoading( LoadingTaskType nType )
        {
            return GetTaskQueue( nType ).Count == 0;
        }
        public bool IsDoneLoading()
        {
            return IsDoneLoading( LoadingTaskType.PreProccesing )
                && IsDoneLoading( LoadingTaskType.Normal )
                && IsDoneLoading( LoadingTaskType.Detail );
        }

        //private int lastTimeProcessed;

        public LoadingManager( ServerClientMainOud nEngine )
        {
            engine = nEngine;
            lastTimeProcesseds = new int[ System.Enum.GetNames( typeof( LoadingTaskType ) ).Length ];
        }



        /// <summary>
        /// DEPRECATED
        /// Adds the given task to the queue
        /// If the given task is allready being processed then it is not added.
        /// DEPRECATED
        /// </summary>
        /// <param name="iTask"></param>
        public void AddLoadTask( ILoadingTask iTask )
        {
            if ( iTask.State == LoadingTaskState.None || iTask.State == LoadingTaskState.Completed )
            {
                GetTaskQueue( iTask.Type ).Enqueue( iTask );
                allTasks.Add( iTask );
            }
        }

        public LoadTaskRef AddLoadTask( LoadTaskDelegate nDelegate, LoadingTaskType nType )
        {
            LoadingTask task = new LoadingTask( nDelegate, nType );
            task.State = LoadingTaskState.Idle;
            GetTaskQueue( nType ).Enqueue( task );
            allTasks.Add( task );

            return new LoadTaskRef( this, allTasks.Count - 1 );
        }

        public LoadTaskRef AddLoadTaskAdvanced( LoadTaskAdvancedDelegate nDelegate, LoadingTaskType nType )
        {
            LoadingTaskAdvanced task = new LoadingTaskAdvanced( nDelegate, nType );
            task.State = LoadingTaskState.Idle;
            GetTaskQueue( nType ).Enqueue( new LoadingTaskAdvanced( nDelegate, nType ) );
            allTasks.Add( task );
            return new LoadTaskRef( this, allTasks.Count - 1 );
        }

        public bool ProcessNextTaskInterval( LoadingTaskType taskType, int interval )
        {
            //if ( lastTimeProcessed + interval < engine.Time ) return ProcessNextTask( taskType );
            if ( lastTimeProcesseds[ (int)taskType ] + interval < engine.Time ) return ProcessNextTask( taskType );
            return false;

        }

        public bool ProcessNextTask( LoadingTaskType taskType )
        {
            bool ret = ProcessTask( GetTaskQueue( taskType ), taskType, 0 );
            if ( ret == true )
                lastTimeProcesseds[ (int)taskType ] = engine.Time;
            //lastTimeProcessed = engine.Time;

            return ret;
            //ProcessTask( firstTaskIndex );

        }
        public bool ProcessTask( AdvancedQueue<ILoadingTask> tasks, LoadingTaskType taskType, int index )
        {
            ILoadingTask iTask = tasks[ index ];

            if ( iTask == null ) return false;

            if ( iTask.State == LoadingTaskState.Completed )
            {
                //This task was allready completed
                //If this is the first task in the queue then remove it
                if ( index == 0 ) tasks.Dequeue();

                //Execute next task
                return ProcessTask( tasks, taskType, index + 1 );

            }


            iTask.Invoke( taskType );

            if ( iTask.State == LoadingTaskState.Completed )
            {
                //If this is the first task in the queue then remove it
                if ( index == 0 ) tasks.Dequeue();

                //now old tasks remain in the list forever. A cleanup should occur from time to time;

                return true;

            }
            else if ( iTask.State == LoadingTaskState.Idle )
            {
                return ProcessTask( tasks, taskType, index + 1 );
            }
            else
            {
                //Partial task executed
                return true;
            }

        }

        private AdvancedQueue<ILoadingTask> GetTaskQueue( LoadingTaskType taskType )
        {
            switch ( taskType )
            {
                case LoadingTaskType.PreProccesing:
                    return preProcessingTasks;
                case LoadingTaskType.Normal:
                    return normalTasks;
                case LoadingTaskType.Detail:
                    return detailTasks;
                case LoadingTaskType.Unload:
                    return unloadTasks;
                default:
                    throw new ArgumentException( "Invalid LoadingTaskType", "taskType" );
            }
        }


        /// <summary>
        /// TODO: when the queues are cleaned, maybe this should also be updated.
        /// </summary>
        /// <param name="refTask"></param>
        /// <returns></returns>
        public ILoadingTask GetLoadingTask( int taskID )
        {
            return allTasks[ taskID ];
        }


    }
    public delegate void LoadTaskDelegate();
    public delegate LoadingTaskState LoadTaskAdvancedDelegate( LoadingTaskType taskType );

    public interface ILoadingTask
    {
        LoadingTaskType Type
        { get;}
        LoadingTaskState State
        { get; set;}

        void Invoke( LoadingTaskType taskType );
    }

    public struct LoadingTask : ILoadingTask
    {
        LoadTaskDelegate _delegate;

        public LoadTaskDelegate Delegate
        {
            get { return _delegate; }
            //set { _delegate = value; }
        }
        LoadingTaskType type;
        LoadingTaskState state;



        public LoadingTask( LoadTaskDelegate nDelegate, LoadingTaskType nType )
        {
            _delegate = nDelegate;
            type = nType;

            state = LoadingTaskState.None;

        }




        #region ILoadingTask Members

        public LoadingTaskType Type
        {
            get { return type; }
        }

        public LoadingTaskState State
        {
            get { return state; }
            set { state = value; }
        }

        public void Invoke( LoadingTaskType taskType )
        {
            Delegate.Invoke();
        }

        #endregion
    }

    public struct LoadingTaskAdvanced : ILoadingTask
    {
        LoadTaskAdvancedDelegate _delegate;

        public LoadTaskAdvancedDelegate Delegate
        {
            get { return _delegate; }
            //set { _delegate = value; }
        }
        LoadingTaskType type;
        LoadingTaskState state;



        public LoadingTaskAdvanced( LoadTaskAdvancedDelegate nDelegate, LoadingTaskType nType )
        {
            _delegate = nDelegate;
            type = nType;

            state = LoadingTaskState.None;

        }




        #region ILoadingTask Members

        public LoadingTaskType Type
        {
            get { return type; }
        }

        public LoadingTaskState State
        {
            get { return state; }
            set { state = value; }
        }

        public void Invoke( LoadingTaskType taskType )
        {
            state = Delegate.Invoke( taskType );
        }

        #endregion
    }

    public enum LoadingTaskType
    {
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
