using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using RouteConfigurator.Design;
using RouteConfigurator.Model;
using RouteConfigurator.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        private ObservableCollection<Model.Model> _models;

        private Model.Model _selectedModel;

        private string _modelFilter = "";

        private string _boxSizeFilter = "";

        private ObservableCollection<Option> _options;

        private string _optionFilter = "";

        private string _optionBoxSizeFilter = "";

//Set to true for visual in SupervisorView
        private bool _TTVisible = true;

        private ObservableCollection<TimeTrial> _timeTrials;

        private TimeTrial _selectedTimeTrial;

        private string _salesFilter = "";

        private string _productionNumFilter = "";
        #endregion

        #region RelayCommands
        public RelayCommand addModelCommand { get; set; }
        public RelayCommand addOptionCommand { get; set; }
        public RelayCommand addTimeTrialCommand { get; set; }
        public RelayCommand loadModelsCommand { get; set; }
        public RelayCommand loadOptionsCommand { get; set; }
        public RelayCommand editTTCommand { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// initializes view components
        /// </summary>
        public SupervisorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            addModelCommand = new RelayCommand(addModel);
            addOptionCommand = new RelayCommand(addOption);
            addTimeTrialCommand = new RelayCommand(addTimeTrial);
            loadModelsCommand = new RelayCommand(loadModels);
            loadOptionsCommand = new RelayCommand(loadOptions);
            editTTCommand = new RelayCommand(editTT);
        }
        #endregion

        #region Commands
        private void addModel()
        {
            AddModelPopup addModel = new AddModelPopup();
            addModel.Show();
        }

        private void addOption()
        {
            AddOptionPopup addOption = new AddOptionPopup();
            addOption.Show();
        }

        private void addTimeTrial()
        {
            AddTimeTrialPopup addTimeTrial = new AddTimeTrialPopup();
            addTimeTrial.Show();
        }

        private void loadModels()
        {
            models = _serviceProxy.getModels();
        }

        private void loadOptions()
        {
            options = _serviceProxy.getOptions();
        }

        private void editTT()
        {
            MessageBox.Show("Not implemented yet");
        }

        #endregion

        #region Public Variables
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

        /// <summary>
        /// Sets TTVisible based on if model is null
        /// </summary>
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

                if (value != null)
                {
                    TTVisible = true;
                    timeTrials = _serviceProxy.getTimeTrials(selectedModel.Base);
                }
                else
                {
                    TTVisible = false;
                }
            }
        }

        /// <summary>
        /// Calls updateFilter
        /// </summary>
        public string modelFilter
        {
            get
            {
                return _modelFilter;
            }
            set
            {
                _modelFilter = value.ToUpper();
                RaisePropertyChanged("modelFilter");
                updateFilter();
            }
        }

        /// <summary>
        /// Calls updateFilter
        /// </summary>
        public string boxSizeFilter
        {
            get
            {
                return _boxSizeFilter;
            }
            set
            {
                _boxSizeFilter = value.ToUpper();
                RaisePropertyChanged("boxSizeFilter");
                updateFilter();
            }
        }

        public ObservableCollection<Option> options 
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value;
                RaisePropertyChanged("options");
            }
        }

        /// <summary>
        /// Calls updateOptionFilter
        /// </summary>
        public string optionFilter
        {
            get
            {
                return _optionFilter;
            }
            set
            {
                _optionFilter = value.ToUpper();
                RaisePropertyChanged("optionFilter");
                updateOptionFilter();
            }
        }

        /// <summary>
        /// Calls updateOptionFilter
        /// </summary>
        public string optionBoxSizeFilter
        {
            get
            {
                return _optionBoxSizeFilter;
            }
            set
            {
                _optionBoxSizeFilter = value.ToUpper();
                RaisePropertyChanged("optionBoxSizeFilter");
                updateOptionFilter();
            }
        }

        public bool TTVisible
        {
            get
            {
                return _TTVisible;
            }
            set
            {
                _TTVisible = value;
                RaisePropertyChanged("TTVisible");
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

        public TimeTrial selectedTimeTrial
        {
            get
            {
                return _selectedTimeTrial;
            }
            set
            {
                _selectedTimeTrial = value;
                RaisePropertyChanged("selectedTimeTrial");
            }
        }

        /// <summary>
        /// Calls updateTTFilter
        /// </summary>
        public string salesFilter
        {
            get
            {
                return _salesFilter;
            }
            set
            {
                _salesFilter = value;
                RaisePropertyChanged("salesFilter");
                updateTTFilter();
            }
        }

        /// <summary>
        /// Calls updateTTFilter
        /// </summary>
        public string productionNumFilter
        {
            get
            {
                return _productionNumFilter;
            }
            set
            {
                _productionNumFilter = value;
                RaisePropertyChanged("productionNumFilter");
                updateTTFilter();
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Updates the model list to only show models with the specified filters
        /// </summary>
        private void updateFilter()
        {
            models = _serviceProxy.getFilteredModels(modelFilter, boxSizeFilter);
        }

        /// <summary>
        /// Updates the options list to only show options with the specified filters
        /// </summary>
        private void updateOptionFilter()
        {
            options = _serviceProxy.getFilteredOptions(optionFilter, optionBoxSizeFilter);
        }

        /// <summary>
        /// Updates the time trials list to only show time trials with the specified filters
        /// </summary>
        private void updateTTFilter()
        {
            timeTrials = _serviceProxy.getFilteredTimeTrials(selectedModel.Base, salesFilter, productionNumFilter);
        }
        #endregion
    }
}
