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
    public class ManagerViewModel : ViewModelBase
    {
        #region PrivateVariables

        /// <summary>
        /// Navigation service to help navigate to other pages
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Data access service to retrieve data from a data source
        /// </summary>
        private IDataAccessService _serviceProxy = new DataAccessService();

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand timeSearchCommand { get; set; }
        public RelayCommand supervisorLoginCommand { get; set; }
        public RelayCommand managerLoginCommand { get; set; }
        public RelayCommand goBackCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public ManagerViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            timeSearchCommand = new RelayCommand(timeSearch);
            supervisorLoginCommand = new RelayCommand(supervisorLogin);
            managerLoginCommand = new RelayCommand(managerLogin);
            goBackCommand = new RelayCommand(goBack);
        }
        #endregion

        #region Commands
        /// <summary>
        /// see searchTimeTrials
        /// </summary>
        private void timeSearch()
        {
        }

        private void supervisorLogin()
        {
            _navigationService.NavigateTo("SupervisorView");
        }

        private void managerLogin()
        {
            _navigationService.NavigateTo("ManagerView");
        }

        private void goBack()
        {
            _navigationService.GoBack();
        }
        #endregion

        #region Public Variables
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

        #region Private Functions
        #endregion
    }
}
