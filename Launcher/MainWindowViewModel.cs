using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MHGameWork.TheWizards.Launcher
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private double progress;
        public double Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                raiseP("Progress");
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                raiseP("Status");
            }
        }
        private bool playEnabled;
        public bool PlayEnabled
        {
            get { return playEnabled; }
            set
            {
                playEnabled = value;
                raiseP("PlayEnabled");
            }
        }
        public ICommand PlayCommand { get; private set; }

        public MainWindowViewModel()
            : this(null)
        {

        }
        public MainWindowViewModel(ICommand playCmd)
        {
            PlayCommand = playCmd;
            playEnabled = false;
            status = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void raiseP(string name)
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }



    }
}