using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public class LoadingManager
    {
        public LoadingQueue PreProcessingTasks;
        public LoadingQueue NormalTasks;
        public LoadingQueue DetailTasks;
        public LoadingQueue UnloadTasks;

        public List<ILoadingTask> allTasks = new List<ILoadingTask>();



        public bool IsDoneLoading()
        {
            return PreProcessingTasks.IsDoneLoading()
                && NormalTasks.IsDoneLoading()
                && DetailTasks.IsDoneLoading();
        }


        public LoadingManager( int capacity )//ServerClientMainOud nEngine )
        {
            PreProcessingTasks = new LoadingQueue( capacity );
            NormalTasks = new LoadingQueue( capacity );
            DetailTasks = new LoadingQueue( capacity );
            UnloadTasks = new LoadingQueue( capacity );
            //engine = nEngine;
            //lastTimeProcesseds = new int[ System.Enum.GetNames( typeof( LoadingTaskType ) ).Length ];
        }








        //private AdvancedQueue<ILoadingTask> GetTaskQueue( LoadingTaskType taskType )
        //{
        //    switch ( taskType )
        //    {
        //        case LoadingTaskType.PreProccesing:
        //            return preProcessingTasks;
        //        case LoadingTaskType.Normal:
        //            return normalTasks;
        //        case LoadingTaskType.Detail:
        //            return detailTasks;
        //        case LoadingTaskType.Unload:
        //            return unloadTasks;
        //        default:
        //            throw new ArgumentException( "Invalid LoadingTaskType", "taskType" );
        //    }
        //}


        ///// <summary>
        ///// TODO: when the queues are cleaned, maybe this should also be updated.
        ///// </summary>
        ///// <param name="refTask"></param>
        ///// <returns></returns>
        //public ILoadingTask GetLoadingTask( int taskID )
        //{
        //    return allTasks[ taskID ];
        //}


    }


    /// <summary>
    /// Deprecated
    /// </summary>
    public interface ILoadingTask
    {
        LoadingTaskType Type
        { get;}
        LoadingTaskState State
        { get; set;}

        void Invoke();
    }
}
