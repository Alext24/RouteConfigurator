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
    public class EditTimeTrialViewModel : ViewModelBase
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

        private Model.Model _selectedModel;

        private DateTime? _date;

        private TimeTrial _timeTrial;

        private string _informationText;
        #endregion

        #region RelayCommands
//        public RelayCommand loadedCommand { get; set; }
        public RelayCommand cancelCommand { get; set; }
        public RelayCommand addTTCommand { get; set; }
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public EditTimeTrialViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            timeTrial = navigationService.Parameter as TimeTrial;

            selectedModel = timeTrial.Model;
            date = timeTrial.Date;

            cancelCommand = new RelayCommand(cancel);

//            loadedCommand = new RelayCommand(loaded);
            //addTTCommand = new RelayCommand(addTT);
            //submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        /*
                private void loaded()
                {
                    models = _serviceProxy.getModels();
                }
        */

        private void cancel()
        {
            _navigationService.GoBack();
        }
        #endregion

        #region Public Variables
        public TimeTrial timeTrial 
        {
            get
            {
                return _timeTrial;
            }
            set
            {
                _timeTrial = value;
                RaisePropertyChanged("timeTrial");
            }
        }

        public Model.Model selectedModel
        {
            get
            {
                return _selectedModel;
            }
            set
            {
                _selectedModel = value;
                RaisePropertyChanged("selectedModel");
            }
        }

        public DateTime? date
        {
            get
            {
                return _date;
            }
            set
            {
                informationText = "";
                _date = value;
                RaisePropertyChanged("date");
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
