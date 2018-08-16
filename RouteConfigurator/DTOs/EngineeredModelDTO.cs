using System.ComponentModel;

namespace RouteConfigurator.DTOs
{
    public class EngineeredModelDTO : INotifyPropertyChanged
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
