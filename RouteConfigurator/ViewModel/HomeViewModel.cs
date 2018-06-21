using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using System;
using System.Collections.Generic;
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

//        private bool _startShiftActive;
        #endregion

        #region RelayCommands
//        public RelayCommand startShiftCommand { get; set; }
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

//            startShiftCommand = new RelayCommand(startShift);
        }
        #endregion

        #region Commands
        /// <summary>
        /// User pressed start shift
        /// </summary>
        private void startShift()
        {

        }

        private void manageDays()
        {
//            _navigationService.NavigateTo("DaysView", _day);
        }
        #endregion

        #region Public Variables
        /*
        public bool startShiftActive
        {
            get
            {
                return _startShiftActive;
            }
            set
            {
                _startShiftActive = value;
                RaisePropertyChanged("startShiftActive");
            }
        }
        */
        #endregion
    }
}
