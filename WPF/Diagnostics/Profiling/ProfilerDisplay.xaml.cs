using System.Windows.Controls;

namespace MHGameWork.TheWizards.Diagnostics.Profiling
{
    /// <summary>
    /// Interaction logic for ProfilerDisplay.xaml
    /// </summary>
    public partial class ProfilerDisplay : UserControl
    {
        public ProfilerDisplay()
        {
            InitializeComponent();

            // Get raw family tree data from a database.
            //Person rootPerson = new profi

            // Create UI-friendly wrappers around the 
            // raw data objects (i.e. the view-model).

            var model = new ProfilerDisplayModel();

            // Let the UI bind to the view-model.
            base.DataContext = model;
        }
    }
}
