using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.TWClient;


namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// TODO: move to correct place
    /// </summary>
    public class XNATaskQueue : IXNAObject
    {
        private IXNAGame game;
        private AdvancedQueue<Task> updateTasks = new AdvancedQueue<Task>();


        public XNATaskQueue( IXNAGame _game )
        {
            game = _game;
        }

        public delegate void UpdateTaskDelegate( float elapsed );

        public void AddUpdateTask( float duration, UpdateTaskDelegate taskDelegate )
        {
            Task task = new Task( duration, taskDelegate );
            updateTasks.Enqueue( task );

        }

        public void Initialize( IXNAGame _game )
        {
        }

        public void Render( IXNAGame _game )
        {
        }

        public void Update( IXNAGame _game )
        {
            if ( updateTasks.Count == 0 ) return;

            float elapsedRemaining = _game.Elapsed;
            while ( elapsedRemaining > 0.001f && updateTasks.Count > 0 )
            {
                Task task = updateTasks.Peek();

                if ( task.Duration > elapsedRemaining )
                {
                    task.TaskDelegate( elapsedRemaining );
                    task.Duration -= elapsedRemaining;
                    break;
                }
                else
                {
                    task.TaskDelegate( task.Duration );
                    elapsedRemaining -= task.Duration;
                    updateTasks.Dequeue();
                }
            }
        }


        private class Task
        {
            private float duration;
            private UpdateTaskDelegate taskDelegate;

            public float Duration
            {
                get { return duration; }
                set { duration = value; }
            }

            public UpdateTaskDelegate TaskDelegate
            {
                get { return taskDelegate; }
            }

            public Task( float duration, UpdateTaskDelegate taskDelegate )
            {
                this.duration = duration;
                this.taskDelegate = taskDelegate;
            }
        }
    }
}
