using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MHGameWork.TheWizards.Annotations;

namespace MHGameWork.TheWizards.Diagnostics.Profiling
{
    public class ProfilerDisplayModel : INotifyPropertyChanged
    {
        private ObservableCollection<ProfilerCommand> buttons;
        private ProfilingNode selectedNode;
        private ProfilingNode Root { get; set; }
        public ReadOnlyCollection<ProfilingNode> BaseLevel { get; set; }
       

        public ObservableCollection<ProfilerCommand> Buttons
        {
            get { return buttons; }
            set
            {
                if (Equals(value, buttons)) return;
                buttons = value;
                OnPropertyChanged("Buttons");
            }
        }

        public ProfilingNode SelectedItem { get; internal set; }


        public ProfilerDisplayModel()
        {
            var r = new ProfilingNode();
            r.Children.Add(new ProfilingNode() { Name = "Boe" });
            r.Children.Add(new ProfilingNode() { Name = "Ba" });
            SetRoot(r);


            Buttons = new ObservableCollection<ProfilerCommand>();
            Buttons.Add(new ProfilerCommand("Start", delegate { Console.WriteLine("Start"); }));
            Buttons.Add(new ProfilerCommand("Stop", delegate { Console.WriteLine("Stop"); }));
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

        public class ProfilerCommand : ICommand, INotifyPropertyChanged
        {
            private readonly Action<object> action;
            private string name;

            public string Name
            {
                get { return name; }
                set
                {
                    if (value == name) return;
                    name = value;
                    OnPropertyChanged1("Name");
                }
            }

            public ProfilerCommand(string name, Action<object> action)
            {
                Name = name;
                this.action = action;
            }

            public void Execute(object parameter)
            {
                action(parameter);
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;
            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged1(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ProfilingNode : INotifyPropertyChanged
    {
        private float duration;
        private bool isSelected;

        public ProfilingNode()
        {
            Children = new ObservableCollection<ProfilingNode>();
            Name = "hello";
            duration = 10;
        }
        public ObservableCollection<ProfilingNode> Children { get; private set; }
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value.Equals(isSelected)) return;
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

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

        public object ProfilingPoint { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
