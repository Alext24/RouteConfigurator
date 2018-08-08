using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.DTOs
{
    public class EngineeredModelDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public string ComponentName { get; set; }

        public string EnclosureSize { get; set; }

        private int _Quantity;
        public int Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                _Quantity = value;
                OnPropertyChanged("Quantity");

                TotalTime = Time * Quantity;
                OnPropertyChanged("TotalTime");
            }
        }

        public decimal Time { get; set; }

        public decimal TotalTime { get; set; }
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
