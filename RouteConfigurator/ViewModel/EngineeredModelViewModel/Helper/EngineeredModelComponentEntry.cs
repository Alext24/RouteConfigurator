using System.ComponentModel;

namespace RouteConfigurator.ViewModel.EngineeredModelViewModel.Helper
{
    /// <summary>
    /// This helper class is used to hold the information that the user needs to 
    /// estimate the engineered model production time.
    /// The user will be able to enter the quantity and then the total time will be calculated.
    /// This class implements INotifyPropertyChanged in order to update the total time attribute for
    /// the dataGrid when the user enters the quantity and the total time is updated.
    /// </summary>
    public class EngineeredModelComponentEntry : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string ComponentName { get; set; }

        public int Quantity { get; set; }

        private decimal _TotalTime;
        public decimal TotalTime
        {
            get
            {
                return _TotalTime;
            }
            set
            {
                _TotalTime = value;
                OnPropertyChanged("TotalTime");
            }
        }
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
