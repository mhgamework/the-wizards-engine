using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public class LoadingQueue
    {

        private FixedLengthQueue<LoadingTask> queue;
        public int lastTimeProcessed;


        public LoadingQueue(int capacity)
        {
            queue = new FixedLengthQueue<LoadingTask>( capacity );
        }


        public bool IsDoneLoading()
        {
            return queue.Count == 0;
        }

        public LoadingTask AddLoadingTask( LoadingTaskDelegate nDelegate )
        {
            LoadingTask task = queue.EnqueueNew();
            task.State = LoadingTaskState.Idle;
            return task;
        }



        public bool ProcessNextTaskInterval( int interval, int time )
        {
            if ( lastTimeProcessed + interval < time ) return ProcessNextTask( time );
            return false;

        }

        public bool ProcessNextTask( int time )
        {
            bool ret = ProcessTask( 0 );
            if ( ret )
                lastTimeProcessed = time;

            return ret;
        }

        public bool ProcessTask( int index )
        {
            LoadingTask iTask = queue.Peek( index );

            if ( iTask == null ) return false; // there are no more tasks

            if ( iTask.State == LoadingTaskState.Completed )
            {
                //This task was allready completed
                //If this is the first task in the queue then remove it
                if ( index == 0 )
                {
                    queue.Dequeue();
                    return ProcessTask( 0 );
                }
                else
                {
                    //Execute next task
                    return ProcessTask( index + 1 );
                }

            }


            // Set the task to completed. This means that unless the user specifies otherwise in the task itself,
            //  this task will be considered completed after this execution
            iTask.State = LoadingTaskState.Completed;

            iTask.Invoke();

            if ( iTask.State == LoadingTaskState.Completed )
            {
                //If this is the first task in the queue then remove it
                if ( index == 0 ) queue.Dequeue();



                return true;

            }
            else if ( iTask.State == LoadingTaskState.Idle )
            {
                return ProcessTask( index + 1 );
            }
            else
            {
                //Partial task executed
                return true;
            }

        }
    }
}
