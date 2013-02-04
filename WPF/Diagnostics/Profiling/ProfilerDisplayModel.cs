using System.Collections.ObjectModel;
using System.ComponentModel;
using MHGameWork.TheWizards.Annotations;

namespace MHGameWork.TheWizards.Diagnostics.Profiling
{
    public class ProfilerDisplayModel : INotifyPropertyChanged
    {
        private ProfilingNode Root { get; set; }
        public ReadOnlyCollection<ProfilingNode> BaseLevel { get; set; }


        public ProfilerDisplayModel()
        {
            var r = new ProfilingNode();
            r.Children.Add(new ProfilingNode() { Name = "Boe" });
            r.Children.Add(new ProfilingNode() { Name = "Ba" });
            SetRoot(r);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetRoot(ProfilingNode profilingNode)
        {
            Root = profilingNode;
            BaseLevel = new ReadOnlyCollection<ProfilingNode>(new[] { Root });
            OnPropertyChanged("BaseLevel");
            OnPropertyChanged("Root");
        }
    }

    public class ProfilingNode : INotifyPropertyChanged
    {
        private float duration;

        public ProfilingNode()
        {
            Children = new ObservableCollection<ProfilingNode>();
            Name = "hello";
            duration = 10;
        }
        public ObservableCollection<ProfilingNode> Children { get; private set; }
        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
        public float Duration
        {
            get { return duration; }
            set
            {
                if (value.Equals(duration)) return;
                duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
