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
using System.Windows;

namespace RouteConfigurator.ViewModel
{
    public class AddTimeTrialPopupModel : ViewModelBase
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

        private string _modelText = "";

        private ObservableCollection<Model.Model> _models = new ObservableCollection<Model.Model>();

        private ObservableCollection<TimeTrial> _timeTrials = new ObservableCollection<TimeTrial>();

        private ObservableCollection<Option> _TTOptions = new ObservableCollection<Option>();

        private TimeTrial _selectedTT;

        private Model.Model _selectedModel;

        private bool _isTTSelected = true;

        private string _informationText;
        #endregion

        #region RelayCommands
        public RelayCommand submitCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public AddTimeTrialPopupModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            submitCommand = new RelayCommand(submit);
        }
        #endregion

        #region Commands
        private void submit()
        {
            MessageBox.Show("Hi\nPlaceholder for sending model to director");
        }
        #endregion

        #region Public Variables
        public string modelText
        {
            get
            {
                return _modelText;
            }
            set
            {
                _modelText = value.ToUpper();
                RaisePropertyChanged("modelText");
                models = _serviceProxy.getFilteredModels(modelText, "");
            }
        }

        public ObservableCollection<Model.Model> models 
        {
            get
            {
                return _models;
            }
            set
            {
                _models = value;
                RaisePropertyChanged("models");
            }
        }

        public ObservableCollection<TimeTrial> timeTrials 
        {
            get
            {
                return _timeTrials;
            }
            set
            {
                _timeTrials = value;
                RaisePropertyChanged("timeTrials");
            }
        }

        public ObservableCollection<Option> TTOptions
        {
            get
            {
                return _TTOptions;
            }
            set
            {
                _TTOptions = value;
                RaisePropertyChanged("TTOptions");
            }
        }

        /// <summary>
        /// sets isTTSelected to true if value is not null
        /// </summary>
        public TimeTrial selectedTT
        {
            get
            {
                return _selectedTT;
            }
            set
            {
                _selectedTT = value;
                RaisePropertyChanged("selectedTT");

                if (value != null ) 
                {
                    if (models == null)
                    {
                        models = _serviceProxy.getModels();
                    }
                    if(selectedTT.TTOptionTimes == null)
                    {
                        selectedTT.TTOptionTimes = new ObservableCollection<TimeTrialsOptionTime>();
                    }

                    isTTSelected = true;
                }
                else
                {
                    isTTSelected = false;
                }
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

                if (selectedModel != null)
                {
                    selectedTT.Model = selectedModel;
                }
            }
        }

        public bool isTTSelected
        {
            get
            {
                return _isTTSelected;
            }
            set
            {
                _isTTSelected = value;
                RaisePropertyChanged("isTTSelected");
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

        #region Private Functions

        #endregion
    }
}
