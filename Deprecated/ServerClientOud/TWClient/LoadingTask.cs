namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public delegate void LoadingTaskDelegate( LoadingTask task );

    public class LoadingTask : ILoadingTask
    {
        LoadingTaskDelegate _delegate;

        public LoadingTaskDelegate Delegate
        {
            get { return _delegate; }
            set { _delegate = value; }
        }
        LoadingTaskType type;
        LoadingTaskState state;




        public LoadingTask()
        {
            _delegate = null;
            type = LoadingTaskType.None;

            state = LoadingTaskState.None;

        }




        #region ILoadingTask Members

        public LoadingTaskType Type
        {
            get { return type; }
            set { type = value; }
        }

        public LoadingTaskState State
        {
            get { return state; }
            set { state = value; }
        }

        public void Invoke()
        {
            Delegate.Invoke( this );
        }

        #endregion
    }
}