using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteConfigurator.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {

        #region PrivateVariables
        private readonly INavigationService _navigationService;
        private IDataAccessService _serviceProxy = new DataAccessService();

        private string _partNumber;
        private string _timeSearchPartNumber;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand timeSearchCommand { get; set; }
        public RelayCommand supervisorLoginCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public HomeViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            if (navigationService.Parameter != null)
            {
            }
            else
            {
            }

            timeSearchCommand = new RelayCommand(timeSearch);
            supervisorLoginCommand = new RelayCommand(supervisorLogin);
        }
        #endregion

        #region Commands
        /// <summary>
        /// 
        /// </summary>
        private void timeSearch()
        {
            informationText = "I am searching for times.  Not really";
        }

        private void supervisorLogin()
        {
            informationText = "Hello Mr. Supervisor";
//            _navigationService.NavigateTo("DaysView", _day);
        }
        #endregion

        #region Public Variables
        public string partNumber 
        {
            get
            {
                return _partNumber;
            }
            set
            {
                _partNumber = value.ToUpper();
                timeSearchPartNumber = partNumber;
                RaisePropertyChanged("partNumber");
            }
        }

        public string timeSearchPartNumber 
        {
            get
            {
                return _timeSearchPartNumber;
            }
            set
            {
                _timeSearchPartNumber = value.ToUpper();
                RaisePropertyChanged("timeSearchPartNumber");
            }
        }

        public string informationText 
        {
            get
            {
                return _informationText;
            }
            set
            {
                _informationText = value;
                RaisePropertyChanged("informationText");
            }
        }
        #endregion
    }
}
