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
            Root = new ProfilingNode();
            Root.Children.Add(new ProfilingNode(){ Name = "Boe"});
            Root.Children.Add(new ProfilingNode() { Name = "Ba" });
            BaseLevel = new ReadOnlyCollection<ProfilingNode>(new[] { Root });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProfilingNode : INotifyPropertyChanged
    {
        public ProfilingNode()
        {
            Children = new ObservableCollection<ProfilingNode>();
            Name = "hello";
        }
        public ObservableCollection<ProfilingNode> Children { get; private set; }
        public override string ToString()
        {
            return Name;
        }

        public string Name { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
