using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class SupervisorViewModel : ViewModelBase
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
        #endregion

        #region RelayCommands
        public RelayCommand timeSearchCommand { get; set; }
        public RelayCommand supervisorLoginCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public SupervisorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            timeSearchCommand = new RelayCommand(timeSearch);
            supervisorLoginCommand = new RelayCommand(supervisorLogin);
        }
        #endregion

        #region Commands
        /// <summary>
        /// see searchTimeTrials
        /// </summary>
        private void timeSearch()
        {
            //modelNumberSections(timeSearchModelNumber);
        }

        private void supervisorLogin()
        {
            _navigationService.NavigateTo("HomeView");
        }
        #endregion

    }
}
